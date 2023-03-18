using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using KEDI.Core.Security.Cryptography;
using CKBS.AppContext;
using CKBS.Models.Services.Account;
using CKBS.Models.Services.POS;
using Microsoft.EntityFrameworkCore;
using CKBS.Models.Services.HumanResources;
using CKBS.Models.Services.Administrator.General;
using KEDI.Core.Premise.Models.Services.Account;
using KEDI.Core.System.Models;
using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;
using KEDI.Core.Cryptography;
using KEDI.Core.Premise.Models.Services.GeneralSettingAdmin;
using KEDI.Core.Premise.Models.Credentials;
using KEDI.Core.Premise.Models.ClientApi.UserAccount;
using KEDI.Core.Models.ControlCenter.ApiManagement;

namespace KEDI.Core.Premise.Repository
{
    public class UserManager
    {
        readonly ILogger<UserManager> _logger;
        readonly DataContext _dataContext;
        readonly SystemManager _systemModule;
        readonly IHttpContextAccessor _httpAccessor;
        readonly JwtManager _jwtManager;
        static readonly Dictionary<string, string> _loggedUsers = new();
        public UserManager(ILogger<UserManager> logger, SystemManager systemModule,
            DataContext dataContext, IHttpContextAccessor httpAccessor, JwtManager jwtManager)
        {
            _logger = logger;
            _dataContext = dataContext;
            _systemModule = systemModule;
            _httpAccessor = httpAccessor;
            _jwtManager = jwtManager;
        }

        public UserAccount CurrentUser => GetCurrentUser();
        public UserAccount GetCurrentUser()
        {
            return FindById(GetUserId()) ?? GetBlankUser();
        }

        public async Task<UserAccount> GetUserQrAsync()
        {
            var user = await _dataContext.UserAccounts.FirstOrDefaultAsync(u => u.IsUserOrder && !u.Delete);
            return user;
        }

        public IQueryable<UserAccount> Users
        {
            get
            {
                return _dataContext.UserAccounts 
                    .Where(u => !u.Company.Delete && !u.Branch.Delete && !u.Employee.Delete)
                    .Include(u => u.Company)
                    .Include(u => u.Branch)
                    .Include(u => u.Employee);
            }
        }

        public List<UserAccount> GetUsers()
        {
            return Users.ToList();
        }

        public bool ContainSpecialChars(string name)
        {
            return Regex.IsMatch(name ?? string.Empty, "[^a-zA-Z0-9_.]");
        }

        public string GetRawName(string text, bool lowerCase = true)
        {
            string _text = Regex.Replace(text, "[^a-zA-Z0-9_.]", string.Empty);
            if (lowerCase) { return _text.ToLower(); }
            return _text;
        }

        public string GetIpAddress()
        {
            // get source ip address for the current request
            if (_httpAccessor.HttpContext?.Request.Headers.ContainsKey(RequestHeader.ForwardedIp) ?? false)
            {
                return _httpAccessor.HttpContext.Request.Headers[RequestHeader.ForwardedIp];
            }
            else
            {
                return _httpAccessor.HttpContext?.Connection?.RemoteIpAddress?.MapToIPv4().ToString() ?? string.Empty;
            }
        }

        public void AddLoggedUser(string userId, string value)
        {
            _loggedUsers.TryAdd(userId, value);
        }

        public void RemoveLoggedUser(string userId)
        {
            _loggedUsers.Remove(userId);
        }

        public IDictionary<string, string> GetLoggedUsers()
        {
            return _loggedUsers;
        }

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

        public bool Check(string code)
        {
            return Check(CurrentUser.ID, code);
        }

        public bool CheckEdition(SystemEdition edition)
        {
            var license = GetLicense();
            return license.Edition == edition;
        }
        public bool Check(int userId, string code)
        {
            try
            {
                var functions = from up in  _dataContext.UserPrivilleges.Where(up => up.UserID == userId && string.Compare(up.Code, code, true) == 0 && up.Used)
                            join f in _dataContext.Functions.Where(x => x.Enable) on up.FunctionID equals f.ID
                            select f;
           
                return functions.Any(); 
            }
            catch
            {
                return false;
            }
        }

        public bool ReachedMaxUsers()
        {
            var activeUsers = GetLoggedUsers();
            var license = _dataContext.SystemLicenses.FirstOrDefault();
            if (license == null) { return false; }
            return license.MaximumUsers <= activeUsers.Keys.Count;
        }

        public Dictionary<string, bool> SystemTypes
        {
            get
            {
                return _dataContext.SystemType.ToDictionary(k => k.Type, v => v.Status);
            }
        }

        public string GetLicenseLifetime()
        {
            return _systemModule.GetLicenseLifetime();
        }

        public bool IsActivated()
        {
            return _systemModule.IsActivated();
        }

        public bool CheckUniqueDevice()
        {
            return _systemModule.CheckUniqueDevice();
        }

        public string GetLicenseExt()
        {
            return _systemModule.GetLicenseExt();
        }

        public string FindDeviceKey()
        {
            return _systemModule.GetDeviceKey();
        }

        public SystemLicense GetLicense()
        {
            var sl = _dataContext.SystemLicenses.FirstOrDefault() ?? new SystemLicense();
            return sl;
        }

        public void RemoveLicense()
        {
            try
            {
                var sl = GetLicense();
                if (sl != null)
                {
                    _dataContext.SystemLicenses.Remove(GetLicense());
                    _dataContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
            }
        }

        public void AddUserPrivillege(int userId)
        {
            List<UserPrivillege> userPrivilleges = new();
            var privileges = _dataContext.Functions
            .Select(f => new UserPrivillege
            {
                UserID = userId,
                FunctionID = f.ID,
                Code = f.Code,
                Used = false,
                Delete = false
            }).ToList();

            foreach (var p in privileges)
            {
                if (!_dataContext.UserPrivilleges.ToList().Any(_p => _p.FunctionID == p.FunctionID && _p.UserID == p.UserID))
                {
                    _dataContext.UserPrivilleges.Update(p);
                    _dataContext.SaveChanges();
                }
            }
        }

        public async Task<ModelStateDictionary> CreateUserAsync(UserAccount user, ModelStateDictionary modelState)
        {
            try
            {
                if (user.Username.Trim().Any(x => Char.IsWhiteSpace(x)))
                {
                    modelState.AddModelError("Username", "Username Can not space!");
                }

                if (user == null)
                {
                    modelState.AddModelError("Username", "User is undefined.");
                }

                if (user.EmployeeID <= 0)
                {
                    modelState.AddModelError("EmployeeID", "Employee is required.");
                }

                if (user.CompanyID <= 0)
                {
                    modelState.AddModelError("CompanyID", "Company is required.");
                }

                if (user.BranchID <= 0)
                {
                    modelState.AddModelError("BranchID", "Branch is required.");
                }

                if (string.IsNullOrWhiteSpace(user?.Username) || string.IsNullOrWhiteSpace(user?.Password))
                {
                    modelState.AddModelError("Username", "Username and password is required.");
                }

                if (_dataContext.UserAccounts.Any(u => IsEqualNotEmpty(u.Username, user.Username, true)))
                {
                    modelState.AddModelError("Username", "Username is already taken.");
                }

                //if (string.IsNullOrWhiteSpace(user?.Email))
                //{
                //    modelState.AddModelError("Username", "Email is required.");
                //}

                if (modelState.IsValid)
                {
                    using var t = await _dataContext.Database.BeginTransactionAsync();
                    HashPassword(user, user.Password);
                    await _dataContext.UserAccounts.AddAsync(user);
                    await _dataContext.SaveChangesAsync();
                    var employee = await _dataContext.Employees.FindAsync(user.EmployeeID);
                    if (employee != null)
                    {
                        employee.IsUser = true;
                    }

                    var userAlerts = _dataContext.UserAlerts.Where(i => i.UserAccountID == user.ID).ToList();
                    if (user.TelegramUserID != null)
                    {
                        foreach (var i in userAlerts)
                        {
                            i.TelegramUserID = user.TelegramUserID;
                            _dataContext.UserAlerts.Update(i);
                        }
                    }

                    _dataContext.SaveChanges();
                    AddUserPrivillege(user.ID);
                    t.Commit();
                    List<SkinUser> skinUser = new List<SkinUser>();
                    var ColorSettings = _dataContext.ColorSettings.ToList();
                    foreach (var item in ColorSettings)
                    {
                        skinUser.Add(new SkinUser
                        {
                            SkinID = item.ID,
                            UserID = user.ID,
                            Unable = false
                        });
                        _dataContext.AddRange(skinUser);
                    }
                    _dataContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                modelState.AddModelError("Exception", ex.Message);
            }
            return modelState;
        }

        public async Task<ModelStateDictionary> EditUserAsync(UserAccount user, ModelStateDictionary modelState)
        {
            try
            {
                if (user.Username.Trim().Any(x => char.IsWhiteSpace(x)))
                {
                    modelState.AddModelError("Username", "Username Can not space!");
                }

                if (user == null)
                {
                    modelState.AddModelError("Username", "User is undefined.");
                }

                if (user.EmployeeID <= 0)
                {
                    modelState.AddModelError("EmployeeID", "Employee is required.");
                }

                if (user.BranchID <= 0)
                {
                    modelState.AddModelError("BranchID", "Branch is required.");
                }

                if (string.IsNullOrWhiteSpace(user?.Username))
                {
                    modelState.AddModelError("Username", "Username is required.");
                }

                if (modelState.IsValid)
                {
                    using var t = await _dataContext.Database.BeginTransactionAsync();
                    var _user = _dataContext.UserAccounts.Find(user.ID);
                    if (!string.IsNullOrEmpty(user.Password))
                    {
                        HashPassword(user, user.Password);
                    }
                    else
                    {
                        user.PasswordHash = _user.PasswordHash;
                        user.Password = _user.Password;
                        user.PasswordStamp = _user.PasswordStamp;
                    }

                    _user.TelegramUserID = user.TelegramUserID;
                    _user.IsUserOrder = user.IsUserOrder;
                    _user.UserPos = user.UserPos;
                    _user.EmployeeID = user.EmployeeID;
                    _user.Email = user.Email;
                    _user.Username = user.Username;
                    _user.Status = user.Status;
                    _user.BranchID = user.BranchID;
                    _user.PasswordHash = user.PasswordHash;
                    _user.Password = user.Password;
                    _user.PasswordStamp = user.PasswordStamp;
                    _dataContext.UserAccounts.Update(_user);
                    await _dataContext.SaveChangesAsync();

                    var setting = _dataContext.GeneralSettings.FirstOrDefault(w => w.UserID == user.ID) ?? new GeneralSetting();
                    if (setting.ID != 0)
                    {
                        setting.BranchID = user.BranchID;
                        _dataContext.GeneralSettings.Update(setting);
                    }

                    var employee = await _dataContext.Employees.FindAsync(user.EmployeeID);
                    if (employee != null)
                    {
                        employee.IsUser = true;
                    }

                    var userAlerts = _dataContext.UserAlerts.Where(i => i.UserAccountID == user.ID).ToList();
                    if (user.TelegramUserID != null)
                    {
                        foreach (var i in userAlerts)
                        {
                            i.TelegramUserID = user.TelegramUserID;
                            _dataContext.UserAlerts.Update(i);
                        }
                    }
                    _dataContext.SaveChanges();
                    AddUserPrivillege(user.ID);
                    t.Commit();
                }
            }
            catch (Exception ex)
            {
                modelState.AddModelError("Exception", ex.Message);
            }
            return modelState;
        }

        public UserAccount GetUser(ClaimsPrincipal principal)
        {
            try
            {
                _ = int.TryParse(principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int _userId);
                return FindById(_userId) ?? GetBlankUser();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return GetBlankUser();
            }
        }

        public bool IsUserPos(ClaimsPrincipal principal)
        {
            var user = GetUser(principal);
            return user.UserPos;
        }

        public UserAccount GetBlankUser()
        {
            return new UserAccount
            {
                Employee = new Employee(),
                Company = new Company()
            };
        }

        public async Task<UserAccount> FindByIdAsync(int userId)
        {
            return await Users.FirstOrDefaultAsync(u => u.ID == userId && !u.Delete);
        }
        public UserAccount FindById(int userId)
        {
            return Users.FirstOrDefault(u => u.ID == userId && !u.Delete);
        }

        public async Task<UserAccount> FindByEmailAsync(string email)
        {
            var user = await Users.FirstOrDefaultAsync(u => IsEqualNotEmpty(u.Email, email, true));
            return user;
        }

        public async Task<UserAccount> FindByUsernameAsync(string username)
        {
            var user = Users.FirstOrDefault(u => IsEqualNotEmpty(u.Username, username, true) && !u.Delete);
            return await Task.FromResult(user);
        }

        public async Task<UserAccount> FindUserAsync(string usernameOrEmail)
        {
            var user = await FindByUsernameAsync(usernameOrEmail) ?? await FindByEmailAsync(usernameOrEmail);
            return await Task.FromResult(user);
        }

        public string HashPassword(UserAccount user, string password)
        {
            if (user == null) { return string.Empty; }
            string _password = password;
            if (string.IsNullOrWhiteSpace(password))
            {
                _password = user.Password;
            }

            //Change to password hash field
            HashSecurity.TryCompute(_password, out string _hash, out string _salt);
            user.PasswordHash = $"{_hash}.{_salt}";

            user.Password = HashFactory.TryCompute(_password, out string _token);
            user.PasswordStamp = _token;
            return user.Password;
        }

        public bool VerifyPassword(string username, string password)
        {
            var user = FindUserAsync(username).Result;
            if (user == null) { return false; }
            if (!string.IsNullOrWhiteSpace(user.PasswordHash))
            {
                string[] digests = user.PasswordHash.Split(".");
                if (digests.Length == 2)
                {
                    return HashSecurity.Verify(password, digests[0], digests[1]);
                }
            }
            return HashFactory.Verify(password, user.Password, user.PasswordStamp);
        }

        public UserAccount ValidateUser(string username, string password)
        {
            bool verified = VerifyPassword(username, password);
            if (verified) { return FindUserAsync(username).Result; }
            return null;
        }

        public bool IsPasswordConfirmed(string password, string confirmedPassword)
        {
            return IsEqualNotEmpty(password, confirmedPassword, false);
        }

        public bool ChangePassword(int userId, ChangePasswordModel model, ModelStateDictionary modelState)
        {
            if (model == null)
            {
                return false;
            }

            if (IsEqualNotEmpty(model.OldPassword, model.NewPassword))
            {
                modelState.AddModelError("NewPassword", "New password cannot be the same as old password.");
            }

            if (!IsEqualNotEmpty(model.NewPassword, model.ConfirmedNewPassword))
            {
                modelState.AddModelError("NewPassword", "New password does not matched.");
            }

            var user = FindUserAsync(model.Username).Result;
            if (user == null)
            {
                modelState.AddModelError("Username", "User does not exist.");
            }

            if (user.ID != userId)
            {
                modelState.AddModelError("Username", "Cannot change password of another user.");
            }

            bool verified = VerifyPassword(model.Username, model.OldPassword);
            if (!verified)
            {
                modelState.AddModelError("Username", "Old password does not matched.");
            }

            if (modelState.IsValid)
            {
                HashPassword(user, model.NewPassword);
                user.SecurityStamp = Guid.NewGuid().ToString("N");
                _dataContext.SaveChanges();
            }
            return verified;
        }

        public bool VerifyUserAccess(UserCredentials creds)
        {
            try
            {
                bool verified = false;
                if (string.IsNullOrWhiteSpace(creds.AccessToken))
                {
                    var validUser = ValidateUser(creds.Username, creds.Password);
                    if (validUser != null)
                    {
                        return Check(validUser.ID, creds.Code);
                    }
                }
                else
                {
                    verified = VerifyAccessToken(creds.Code, creds.AccessToken);
                }
                return verified;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }

        }

        public string CreateAccessToken(string username)
        {
            var user = FindUserAsync(username).Result;
            string guid = Guid.NewGuid().ToString("N");
            string key = user.Username.ToLower() + " " + guid;
            RsaFactory.TryExportKeys(out byte[] privateKey, out byte[] publicKey);
            user.Signature = RsaFactory.SignDataSHA256(key, privateKey);
            user.PublicKey = Convert.ToBase64String(publicKey);
            _dataContext.UserAccounts.Update(user);
            _dataContext.SaveChanges();
            return key;
        }

        public bool VerifyAccessToken(string code, string key)
        {
            try
            {
                string[] values = key.Split(" ");
                var user = FindUserAsync(values[0]).Result;
                if (user == null) { return false; }
                bool checkedPriv = Check(user.ID, code);
                var pubKey = Convert.FromBase64String(user.PublicKey);
                key = user.Username.ToLower() + " " + values[1].Trim();
                var verified = RsaFactory.VerifyDataSHA256(key, user.Signature, pubKey);
                return checkedPriv && verified;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
        }

        //Web API with JSON Web Token
        public string[] GetActiveSystemTypes()
        {
            return _dataContext.SystemType.Where(s => s.Status).Select(s => s.Type).ToArray();
        }

        public int GetUserId(string jwtToken = "")
        {
            try
            {
                //int userId = 0;
                var principal = _httpAccessor.HttpContext?.User ?? new ClaimsPrincipal();
                if (!IsAuthenticated)
                {
                    string authToken = !string.IsNullOrWhiteSpace(jwtToken) ? jwtToken
                    : _httpAccessor.HttpContext?.Request?.Headers["Authorization"]
                    .FirstOrDefault()?.Split(" ").LastOrDefault() ?? string.Empty;
                    principal = _jwtManager.ValidateJwtToken(authToken);
                }
                _ = int.TryParse(principal.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int _userId);
                return _userId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return 0;
            }
        }

        public bool IsAuthenticated => _httpAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

        public BearerResponse Authenticate(UserLogin model, string ipAddress = "")
        {     
            if(string.IsNullOrWhiteSpace(ipAddress)){
                ipAddress = GetIpAddress();
            }

            var authResponse = new BearerResponse
            {
                Messages = new List<string>()
            };

            bool isVerified = VerifyPassword(model.Username, model.Password);
            if (!isVerified)
            {
                authResponse.Messages.Add("Username or password is incorrect.");
                return authResponse;
            }
            var user = _dataContext.UserAccounts.SingleOrDefault(x => x.Username == model.Username);
            if(user == null)
            {
                authResponse.Messages.Add("User not found.");
                return authResponse;
            }

            authResponse = CreateJwtToken(user, ipAddress);
            return authResponse;
        }

        public BearerResponse CreateJwtToken(UserAccount user, string ipAddress)
        {
            var authResponse = new BearerResponse
            {
                Messages = new List<string>()
            };
            
            var jwtToken = _jwtManager.NewJwtToken(user);       
            if(!string.IsNullOrWhiteSpace(jwtToken)){
                var refreshToken = _jwtManager.NewRefreshToken(ipAddress);
                authResponse.RefreshToken = refreshToken.Token;
                user.RefreshTokens.Add(refreshToken);                 
            }
            // remove old refresh tokens from user         
            RemoveOldRefreshTokens(user);
            _dataContext.UserAccounts.Update(user); 
            _dataContext.SaveChanges();
            authResponse.Username = user.Username;
            authResponse.AccessToken = jwtToken;
            return authResponse;
        }

        public BearerResponse CreateJwtToken(ClientApp clientApi, string ipAddress)
        {
            var authResponse = new BearerResponse
            {
                Messages = new List<string>()
            };

            var jwtToken = _jwtManager.NewJwtToken(clientApi);
            var user = FindById(clientApi.UserId);
            if (!string.IsNullOrWhiteSpace(jwtToken))
            {
                var refreshToken = _jwtManager.NewRefreshToken(ipAddress);
                authResponse.RefreshToken = refreshToken.Token;
                user.RefreshTokens.Add(refreshToken);
            }
            // remove old refresh tokens from user         
            RemoveOldRefreshTokens(user);
            _dataContext.UserAccounts.Update(user);
            _dataContext.SaveChanges();
            authResponse.Username = user.Username;
            authResponse.AccessToken = jwtToken;
            return authResponse;
        }

        public BearerResponse RefreshToken(string token = "")
        {
            string ipAddress = GetIpAddress();
            if(string.IsNullOrWhiteSpace(token)){
                token = _httpAccessor.HttpContext.Request.Cookies["X-REFRESH-TOKEN"];
            }
            var user = GetUserByRefreshToken(token);
            var refreshToken = user.RefreshTokens?.SingleOrDefault(x => x.Token == token) ?? new RefreshToken();

            if (refreshToken.IsRevoked)
            {
                // revoke all descendant tokens in case this token has been compromised
                RevokeDescendantRefreshTokens(refreshToken, user, ipAddress, $"Attempted reuse of revoked ancestor token: {token}");
                _dataContext.Update(user);
                _dataContext.SaveChanges();
            }

            if (refreshToken.IsActive)
            {
                // replace old refresh token with a new one (rotate token)
                var newRefreshToken = RotateRefreshToken(refreshToken, ipAddress);
                user.RefreshTokens.Add(newRefreshToken);

                // remove old refresh tokens from user
                RemoveOldRefreshTokens(user);

                // save changes to db
                _dataContext.Update(user);
                _dataContext.SaveChanges();

                // generate new jwt
                var jwtToken = _jwtManager.NewJwtToken(user);
                return new BearerResponse
                {
                    Username = user.Username,
                    AccessToken = jwtToken,
                    RefreshToken = refreshToken.Token
                };
            }

            return new BearerResponse();
        }

        public void RevokeToken(string refreshToken)
        {
            string ipAddress = GetIpAddress();
            var user = GetUserByRefreshToken(refreshToken);
            if(user == null){ return; }
            var _refreshToken = user.RefreshTokens.FirstOrDefault(x => x.Token == refreshToken) ?? new RefreshToken();
            if (_refreshToken.IsActive)
            {
                // revoke token and save
                RevokeRefreshToken(_refreshToken, ipAddress, "Revoked without replacement");
                _dataContext.Update(user);
                _dataContext.SaveChanges();
            }
        }

        private UserAccount GetUserByRefreshToken(string token)
        {
            var user = _dataContext.UserAccounts
                .SingleOrDefault(u => u.RefreshTokens.Any(t => t.Token == token))
                ?? new UserAccount
                {
                    RefreshTokens = new List<RefreshToken>()
                };
            return user;
        }

        private RefreshToken RotateRefreshToken(RefreshToken refreshToken, string ipAddress)
        {
            var newRefreshToken = _jwtManager.NewRefreshToken(ipAddress);
            RevokeRefreshToken(refreshToken, ipAddress, "Replaced by new token", newRefreshToken.Token);
            return newRefreshToken;
        }

        public void RemoveOldRefreshTokens(UserAccount user)
        {
            // remove old inactive refresh tokens from user based on TTL in app settings
            user.RefreshTokens.RemoveAll(x =>
                !x.IsActive &&
                x.Created.AddDays(1) <= DateTime.UtcNow);
        }

        private void RevokeDescendantRefreshTokens(RefreshToken refreshToken, UserAccount user, string ipAddress, string reason)
        {
            // recursively traverse the refresh token chain and ensure all descendants are revoked
            if (!string.IsNullOrEmpty(refreshToken.ReplacedByToken))
            {
                var childToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken.ReplacedByToken);
                if (childToken.IsActive){
                    RevokeRefreshToken(childToken, ipAddress, reason);
                }             
                else{
                    RevokeDescendantRefreshTokens(childToken, user, ipAddress, reason);
                }                   
            }
        }

        private static void RevokeRefreshToken(RefreshToken token, string ipAddress, string reason = null, string replacedByToken = null)
        {
            token.Revoked = DateTime.UtcNow;
            token.RevokedByIp = ipAddress;
            token.ReasonRevoked = reason;
            token.ReplacedByToken = replacedByToken;
        }

        public void SetRefreshTokenCookie(string token)
        {
            // append cookie with refresh token to the http response
            if (!string.IsNullOrWhiteSpace(token))
            {
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Expires = DateTime.UtcNow.AddDays(1)
                };
                _httpAccessor.HttpContext.Response.Cookies.Append(RequestHeader.RefreshToken, token, cookieOptions);       
            }   
        }

        public string GetRefreshTokenCookie(){
            return _httpAccessor.HttpContext.Request.Cookies[RequestHeader.RefreshToken];
        }
    }
}
