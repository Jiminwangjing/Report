using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KEDI.Core.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using KEDI.Core.Cloud.Models.System;
using KEDI.Core.System.Models;
using CKBS.AppContext;
using KEDI.Core.Hosting;
using KEDI.Core.Security;
using CKBS.Models.Services.POS;
using KEDI.Core.Hosting.Models;
using CKBS.Models.Services.Administrator.General;
using System.IO;
using System.Management;
using System.Net.Sockets;
using System.Globalization;
using KEDI.Core.Cryptography;
using KEDI.Core.Premise.Models.Credentials;
using KEDI.Core.Premise.Utilities;
using System.Diagnostics;
using System.Security.AccessControl;
using KEDI.Core.Premise.Models.ClientApi;

namespace KEDI.Core.Premise.Repository
{
    public class SystemManager
    {
        readonly ILogger<SystemManager> _logger;
        readonly DataContext _dataContext;
        readonly HostMessenger _messenger;
        readonly UtilityModule _utils;
        readonly ISecuritySigner _signer;
        readonly IConfiguration _config;
        private readonly HostSetting _settings;
        public SystemManager(
            ILogger<SystemManager> logger, DataContext dataContext,
            HostMessenger messenger, UtilityModule utils,
            ISecuritySigner signer, IConfiguration config,
            IDictionary<string, HostSetting> hostSettings
        ){
            _logger = logger;
            _dataContext = dataContext;
            _messenger = messenger;
            _utils = utils;
            _signer = signer;
            _config = config;
            _ = hostSettings.TryGetValue("InternalSync", out _settings);
        }

        private bool IsServerHost()
        {
            return Convert.ToBoolean(_config["IsServerHost"]);
        }

        private readonly LicenseFactory _licenseFactory = new();
        private bool IsEqualNotEmpty(string a, string b, bool ignoreCase = false)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(a) && string.IsNullOrWhiteSpace(b)) { return false; }
                if (ignoreCase)
                {
                    return string.Compare(a?.Trim(), b?.Trim(), ignoreCase) == 0;
                }
                return string.Compare(a?.Trim(), b?.Trim()) == 0;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                return false;
            }
        }

        public string GetProcessorId()
        {
            try
            {
                ManagementClass mc = new("win32_processor");
                ManagementObjectCollection moc = mc.GetInstances();
                string pId = String.Empty;
                foreach (ManagementObject mo in moc)
                {
                    pId = mo.Properties["processorID"].Value.ToString();
                    break;
                }
                return pId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return string.Empty;
            }
        }

        public string CreateCode()
        {
            return "s" + (_dataContext.SystemLicenses.Count() + 1).ToString().PadLeft(3, '0');
        }

        public string CreateKey()
        {
            string guid = Guid.NewGuid().ToString("N");
            return HashFactory.RandomizeKey(guid.Length, true, guid);
        }

        public string CreatePassword()
        {
            string guid = Guid.NewGuid().ToString("N");
            return HashFactory.RandomizeKey(guid.Length * 2, true, guid);
        }

        public string CreateTimeStamp()
        {
            return DateTimeOffset.UtcNow.Ticks.ToString();
        }

        public string VerifyCredentialsAsync(TenantModel creds)
        {
            try
            {
                if (creds == null) { return string.Empty; }
                var license = GetLicense(creds.TenantID);
                if (license != null)
                {
                    string sessionKey = license.TenantID + license.SecretKey + license.TimeStamp;
                    bool verified = RsaFactory.VerifyDataSHA256(sessionKey, license.ApiKey, license.PublicKey);
                    if (verified)
                    {
                        string redirectUrl = license.TenantHost;
                    }
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return string.Empty;
            }
        }

        public async Task<int> UpdateSystemTypesAsync(SystemLicense license)
        {
            try
            {
                List<SystemType> systemTypes = _dataContext.SystemType.ToList();
                foreach (var st in license.SystemTypes.ToList())
                {
                    if (!_dataContext.SystemType.Any(_st => string.Compare(_st.Type, st.Key, true) == 0))
                    {
                        systemTypes.Add(new SystemType
                        {
                            Type = st.Key,
                            Status = st.Value
                        });
                    }

                    foreach (var _st in systemTypes)
                    {
                        if (string.Compare(st.Key, _st.Type, true) == 0)
                        {
                            _st.Status = st.Value;
                        }
                    }
                }

                _dataContext.UpdateRange(systemTypes);
                _dataContext.SaveChanges();
                return await Task.FromResult(0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return await Task.FromResult(-1);
            }
        }

        public SystemLicense SignLicense(SystemLicense license, byte[] privateKey)
        {
            string licenseKey = $@"{license.TenantID}{license.EntryKey}{license.DeviceKey}{license.MaximumUsers}{license.Expiration}";
            license.Signature = Convert.ToBase64String(_signer.SignHash(licenseKey, privateKey));
            SetSigningSecret();
            return license;
        }

        public bool VerifyLicense(SystemLicense license)
        {
            if (license == null) { return false; };
            if (string.IsNullOrWhiteSpace(license.EntryKey)) { return false; }
            if (string.IsNullOrWhiteSpace(license.DeviceKey)) { return false; }
            if (string.IsNullOrWhiteSpace(license.Signature)) { return false; }
            if (license.MaximumUsers <= 0) { return false; }
            if (!license.Unlimited && license.Expiration < GetNtpDateBinary())
            {
                return false;
            }

            string licenseKey = $"{license.TenantID}{license.EntryKey}{license.DeviceKey}{license.MaximumUsers}{license.Expiration}";
            byte[] signatureBytes = Convert.FromBase64String(license.Signature);
            bool verified = _signer.VerifyHash(licenseKey, signatureBytes, Convert.FromBase64String(license.PublicKey));
            if (IsServerHost())
            {
                byte[] signingSecretBytes = GetSigningSecret();
                verified = verified && signingSecretBytes.Length > 0;
            }
            return verified;
        }

        public bool IsCloudHosting()
        {
            var license = GetLicense();
            return license?.Role == SystemRole.Cloud;
        }

        public async Task<SystemLicense> AutoActivateAsync(ModelStateDictionary modelState)
        {
            var license = _licenseFactory.Read();
            return await ActivateAsync(license, modelState);
        }

        public bool RemoveLicense(SystemLicense license)
        {
            try
            {
                _dataContext.SystemLicenses.Remove(license);
                _dataContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
        }

        public bool RemoveLicense(string tenantId = "")
        {
            return RemoveLicense(GetLicense(tenantId));
        }

        public async Task<SystemLicense> ReactivateAsync(Stream licenseInput, ModelStateDictionary modelState)
        {
            _dataContext.SystemLicenses.Remove(GetLicense());
            return await ActivateAsync(licenseInput, modelState);
        }

        public async Task<SystemLicense> ActivateAsync(Stream licenseInput, ModelStateDictionary modelState)
        {
            var license = _licenseFactory.Read(licenseInput);
            return await ActivateAsync(license, modelState);
        }

        public async Task<SystemLicense> ActivateAsync(SystemLicense license, ModelStateDictionary modelState)
        {
            try
            {
                if (license == null || string.IsNullOrEmpty(license?.TenantID)) { return new SystemLicense(); }
                if (license.DeviceKey != GetDeviceKey())
                {
                    modelState.AddModelError("DeviceKey", "Device key does not match.");
                    return license;
                }

                var company = GetCompany();
                if (!string.IsNullOrWhiteSpace(company.TenantID))
                {
                    license.TenantID = company.TenantID;
                }

                license.TimeStamp = CreateTimeStamp();
                RsaSecurity.TryExportKeys(out byte[] _privateKey, out byte[] _publicKey);
                string publicKey = Convert.ToBase64String(_publicKey);
                license.PublicKey = publicKey;
                SignLicense(license, _privateKey);

                if (modelState.IsValid)
                {
                    using var t = _dataContext.Database.BeginTransaction();
                    var _license = GetLicense(license.TenantID);
                    if (license.Role == SystemRole.Premise)
                    {
                        license.TenantHost = string.Empty;
                    }

                    if (_license != null)
                    {
                        license.ID = _license.ID;
                        RemoveLicense(_license);
                    }

                    company.TenantID = license.TenantID;
                    license.PrivateKey = string.Empty;
                    _dataContext.SystemLicenses.Add(license);
                    await _dataContext.SaveChangesAsync();
                    await UpdateSystemTypesAsync(license);

                    t.Commit();
                    return license;
                }
            }
            catch (Exception ex)
            {
                modelState.AddModelError("Exception", ex.Message);
            }
            return new SystemLicense();
        }

        public async Task<ContractResponse<SystemLicense>> UploadLicenseAsync(SystemLicense license, string encPrivateKey)
        {
            license.PrivateKey = encPrivateKey;
            var response = await _messenger.SendAsync<SystemLicense>("/api/system/activate", license);
            return response;
        }
        public long GetNtpDateBinary()
        {
            return _licenseFactory.GetNtpUtc().Date.ToBinary();
        }

        public string GetLicenseLifetime()
        {
            var license = GetLicense() ?? new SystemLicense();
            if (license.Unlimited) { return double.PositiveInfinity.ToString(); }
            return (license.Expiration - DateTime.Now.ToBinary()).ToString();
        }

        public bool IsActivated()
        {
            var license = GetLicense();
            return VerifyLicense(license) && CheckUniqueDevice();
        }

        public bool CheckUniqueDevice()
        {
            string deviceKey = GetDeviceKey();
            return IsEqualNotEmpty(deviceKey, GetLicense()?.DeviceKey) || string.IsNullOrWhiteSpace(deviceKey);
        }

        public string GetLicenseExt()
        {
            return _licenseFactory.DefaultExt;
        }

        public string GetDeviceKey()
        {
            return GetProcessorId();
        }

        public Company GetCompany()
        {
            return _dataContext.Company.FirstOrDefault() ?? new Company();
        }

        public SystemLicense GetLicense(string tenantId = "")
        {
            var license = _dataContext.SystemLicenses.FirstOrDefault(s => s.TenantID == tenantId);
            if (license == null)
            {
                license = _dataContext.SystemLicenses.FirstOrDefault();
            }
            return license;
        }

        public string NewSigningSecretKey()
        {
            string secretKey = Encoding.UTF8.GetString(HashSecurity.RandomizeKey(128));
            return secretKey;
        }

        public void SetSigningSecret()
        {
            if (IsServerHost())
            {
                string encSecret = NewSigningSecretKey();
                Environment.SetEnvironmentVariable(SecurityVault.SigningSecret, encSecret, EnvironmentVariableTarget.User);         
            }
        }

        public byte[] GetSigningSecret()
        {
            string secret = Environment.GetEnvironmentVariable(
                SecurityVault.SigningSecret, EnvironmentVariableTarget.User
            ) ?? _settings.SecretKey ?? string.Empty;          
            return Encoding.UTF8.GetBytes(secret);
        }

        public bool ConfirmPassphraseAsync(string password, string confirmedPassword)
        {
            return IsEqualNotEmpty(password, confirmedPassword, false);
        }

        public byte[] GetPrivateKey(SystemLicense license)
        {
            return Convert.FromBase64String(AesFactory.Decrypt(license.PrivateKey));
        }

        public DateTimeOffset GetUtc()
        {
            var tcp = new TcpClient("time.nist.gov", 13);
            string resp;
            using (var rdr = new StreamReader(tcp.GetStream()))
            {
                resp = rdr.ReadToEnd();
            }

            string utc = resp.Substring(7, 17);
            _ = DateTimeOffset.TryParseExact(utc, "yy-MM-dd HH:mm:ss",
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal,
                out DateTimeOffset _utc);
            return _utc;
        }

        public async Task<ContractResponse<string>> RedirectAsync()
        {
            var license = GetLicense();
            var response = new ContractResponse<string>();
            if (license == null)
            {
                response.Result = "/system/activate";
            }
            else
            {
                response = await _messenger.SendAsync<string>("/api/system/navigate", new TenantModel
                {
                    TenantID = license?.TenantID,
                    ApiKey = license?.ApiKey
                });
            }

            return response;
        }
    }
}
