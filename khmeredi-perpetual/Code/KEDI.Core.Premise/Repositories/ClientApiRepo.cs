using CKBS.AppContext;
using CKBS.Models.Services.Account;
using KEDI.Core.Cryptography;
using KEDI.Core.Models.ControlCenter.ApiManagement;
using KEDI.Core.Premise.DependencyInjection;
using KEDI.Core.Premise.Models.ClientApi.UserAccount;
using KEDI.Core.Premise.Models.Credentials;
using KEDI.Core.Premise.Models.Services.Administrator.ApiManagement;
using KEDI.Core.Premise.Repository;
using KEDI.Core.Premise.Utilities;
using KEDI.Core.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NPOI.POIFS.Crypt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KEDI.Core.Repository
{
    public interface IClientApiRepo
    {
        ClientApp GetCurrentClient();
        Task<BearerResponse> LoginAsync(ApiKeyLogin model, string ipAddress = "");
        ClientForm CreateApiKey(ModelStateDictionary modelState, ClientForm form);
        bool VerifyApiKey(string apiKey, string secretKey);
        bool VerifyApiKey();
        List<ClientAppDisplay> GetClientAppDisplays();
        ClientApp GetByCode(string clientCode);
        ClientApp FindById(long id);
    }

    public class ClientApiRepo : IClientApiRepo
    {
        readonly ILogger<ClientApiRepo> _logger;
        readonly DataContext _dataContext;
        readonly IHttpContextAccessor _accessor;
        readonly UtilityModule _utils;
        readonly ISecuritySigner _signer;
        readonly JwtManager _jwt;
        readonly UserManager _userManager;
        public ClientApiRepo(ILogger<ClientApiRepo> logger,
            JwtManager jwt, DataContext dataContext, UtilityModule utility,
            ISecuritySigner signer,
            UserManager userManager,
            IHttpContextAccessor accessor)
        {
            _logger = logger;
            _accessor = accessor;
            _dataContext = dataContext;
            _jwt = jwt;
            _utils = utility;
            _signer = signer;
            _userManager = userManager;
        }

        private IQueryable<ClientApp> _clientApis => _dataContext.ClientApps.AsNoTracking();
        public async Task<BearerResponse> LoginAsync(ApiKeyLogin model, string ipAddress)
        {
            if (string.IsNullOrWhiteSpace(ipAddress))
            {
                ipAddress = _userManager.GetIpAddress();
            }

            var authResponse = new BearerResponse
            {
                Messages = new List<string>()
            };

            bool isVerified = VerifyApiKey(model.ApiKey, model.SecretKey);
            if (!isVerified)
            {
                authResponse.Messages.Add("Api key or secret key is invalid.");
                return authResponse;
            }

            var clientApi = await FindByApiKeyAsync(model.ApiKey);
            var user = await _userManager.FindByIdAsync(clientApi.UserId);
            if (user == null)
            {
                authResponse.Messages.Add("User not found.");
                return authResponse;
            }

            // authentication successful so generate jwt and refresh tokens
            authResponse = _userManager.CreateJwtToken(clientApi, ipAddress);
            if (string.IsNullOrWhiteSpace(authResponse.AccessToken))
            {
                authResponse.Messages.Add("Invalid signing credentials.");
                authResponse.RefreshToken = string.Empty;
                authResponse.Username = string.Empty;
                return authResponse;
            }
            return authResponse;
        }

        public ClientApp GetCurrentClient()
        {
            _ = long.TryParse(_accessor.HttpContext?.User?.Identity?.Name, out long id);
            return FindById(id);
        }

        public ClientApp GetByAppId(string appId)
        {
            return _clientApis.FirstOrDefault(x => x.AppId == appId) ?? new ClientApp();
        }

        public ClientApp FindById(long id)
        {
            return _clientApis.FirstOrDefault(x => x.Id == id);
        }

        public async Task<ClientApp> FindByApiKeyAsync(string apiKey)
        {
            string appId = GetAppId(apiKey);
            return await _dataContext.ClientApps.SingleOrDefaultAsync(c => c.AppId == appId);
        }

        public ClientApp FindByApiKey(string apiKey)
        {
            string appId = GetAppId(apiKey);
            return _dataContext.ClientApps.SingleOrDefault(c => c.AppId == appId);
        }

        public ClientApp GetByCode(string clientCode)
        {
            return _clientApis.FirstOrDefault(c =>
                StringHelper.Equals(c.Code, clientCode, true, false)
            ) ?? new ClientApp();
        }

        public ClientForm CreateApiKey(ModelStateDictionary modelState, ClientForm form)
        {
            string clientCode = Regex.Replace(form.ClientCode ?? string.Empty, "\\s+", string.Empty);
            if (_userManager.ContainSpecialChars(clientCode))
            {
                modelState.AddModelError("ClientCode", "Client Code is not allowed special characters excepts for dot(.) and underscore(_).");
            }

            if (clientCode.Length < 4)
            {
                modelState.AddModelError("ClientCode", "Client Code is required for at least 4 minimum lengths.");
            }

            if (_clientApis.Any(c => StringHelper.Equals(c.Code, form.ClientCode, true, false)))
            {
                modelState.AddModelError("ClientCode", "Client Code is not allowed duplicate.");
            }

            if (modelState.IsValid)
            {
                string ipAddress = _userManager.GetIpAddress();
                string apiKey = NewApiKey(out string _appId);
                string secretKey = NewSecretKey();
                int userId = _userManager.CurrentUser.ID;
                string key = $"{apiKey}{userId}{form.Readonly}{form.StrictIpAddress}{form.Revoked}";
                RsaSecurity.TryExportKeys(out byte[] _privateKey, out byte[] _publicKey);
                byte[] signedBytes = _signer.SignHash(key, secretKey, _privateKey);
                string publicKey = Convert.ToBase64String(_publicKey);
                var client = new ClientApp
                {
                    PublicKey = publicKey,
                    AppId = _appId,
                    Code = clientCode,
                    Name = form?.ClientName!,
                    Signature = Convert.ToBase64String(signedBytes),
                    IpAddress = ipAddress,
                    CreatedDate = DateTimeOffset.Now,
                    UserId = userId,
                    IsReadonly = (bool)form?.Readonly!,
                    StrictIpAddress = (bool)form?.StrictIpAddress!
                };
                _dataContext.ClientApps.Add(client);
                _dataContext.SaveChanges();
                form.Id = client.Id;
                form.ApiKey = apiKey;
                form.SecretKey = secretKey;
            }
            return form;
        }

        public bool VerifyApiKey()
        {
            string apiKey = _accessor.HttpContext.Request.Headers[ApiKeyHeader.ApiKey].FirstOrDefault() ?? string.Empty;
            string secret = _accessor.HttpContext.Request.Headers[ApiKeyHeader.Secret].FirstOrDefault() ?? string.Empty;
            if(string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(secret))
            {
                return false;
            }
            bool isVerified = VerifyApiKey(apiKey, secret);
            return isVerified;
        }
        public bool VerifyApiKey(string apiKey, string secretKey)
        {
            try
            {
                string ipAddress = _userManager.GetIpAddress();
                var keyVault = FindByApiKey(apiKey);
                if(keyVault == null) { return false; }
                string userId = $"{keyVault.UserId}";
                string key = $"{apiKey}{userId}{keyVault.IsReadonly}{keyVault.StrictIpAddress}{keyVault.IsRevoked}";
                byte[] pubKeyBytes = Convert.FromBase64String(keyVault.PublicKey);
                byte[] signatureBytes = Convert.FromBase64String(keyVault.Signature);
                bool verifiedApiKey = _signer.VerifyHash(key, secretKey, signatureBytes, pubKeyBytes);
                if (keyVault.StrictIpAddress)
                {
                    verifiedApiKey = verifiedApiKey && (keyVault.IpAddress == ipAddress);
                }
                return verifiedApiKey;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                return false;
            }
        }

        public bool RevokeApiKey(string apiKey)
        {
            var clientApi = FindByApiKey(apiKey);
            clientApi.IsRevoked = true;
            _dataContext.ClientApps.Update(clientApi);
            _dataContext.SaveChanges();
            return clientApi.IsRevoked;
        }

        public string NewApiKey(out string appId)
        {
            string rndKey = Convert.ToBase64String(HashSecurity.RandomizeKey(64));
            rndKey = _utils.ToAlphaNumeric(rndKey);
            string newAppId = Guid.NewGuid().ToString("N");
            string apiKey = $"{newAppId}.{rndKey}";
            appId = newAppId;
            return apiKey;
        }

        public string NewSecretKey()
        {
            var secretKey = Convert.ToBase64String(HashSecurity.RandomizeKey(64));
            string alphaNum = _utils.ToAlphaNumeric(secretKey);
            int missingSize = secretKey.Length - alphaNum.Length;
            return $"{alphaNum}{HashSecurity.Randomize(missingSize, Guid.NewGuid().ToString("N"))}";
        }

        public string GetApiKey(string apiKey = "")
        {
            try
            {
                string _apiKey = !string.IsNullOrWhiteSpace(apiKey) ? apiKey :
                   _accessor.HttpContext?.Request.Headers[ApiKeyHeader.ApiKey].FirstOrDefault()!;
                return _apiKey;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                return string.Empty;
            }
        }

        public string GetAppId(string apiKey)
        {
            if (string.IsNullOrWhiteSpace(apiKey)) { return string.Empty; }
            return GetApiKey(apiKey).Split(".")[0] ?? string.Empty;
        }

        public List<ClientAppDisplay> GetClientAppDisplays()
        {
            return _clientApis.Select(ci => new ClientAppDisplay
            {
                ClientCode = ci.Code,
                ClientName = ci.Name,
                CreatedDate = ci.CreatedDate.LocalDateTime.ToShortDateString(),
                CreatorName = _userManager.FindById(ci.UserId).Username,
                IpAddress = ci.IpAddress,
                Revoked = ci.IsRevoked ? "Y" : "N",
                StrictIpAddress = ci.StrictIpAddress ? "Y" : "N"
            }).ToList();
        }
    }
}
