using CKBS.Models.Services.Account;
using KEDI.Core.Cryptography;
using KEDI.Core.Models.ControlCenter.ApiManagement;
using KEDI.Core.Premise.Models.ClientApi.UserAccount;
using KEDI.Core.Premise.Models.Credentials;
using KEDI.Core.Security.Cryptography;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace KEDI.Core.Premise.Repository
{
    public class JwtManager
    {
        readonly ILogger<JwtManager> _logger;
        readonly IHttpContextAccessor _accessor;
        readonly SystemManager _system;
        readonly IDictionary<string, string> _secrets;
        public JwtManager(
            ILogger<JwtManager> logger, IHttpContextAccessor accessor, SystemManager system,
            UtilityModule utils, IConfiguration config)
        {
            _logger = logger;
            _accessor = accessor;
            _system = system;
            _secrets = new Dictionary<string, string>();
        }

         public string RenewSignigPepper(){        
            var secretsId = Assembly.GetExecutingAssembly().GetCustomAttribute<UserSecretsIdAttribute>().UserSecretsId;
            var secretsPath = PathHelper.GetSecretsPathFromSecretsId(secretsId);
            var secretsJson = File.ReadAllText(secretsPath);
            var secrets = JsonConvert.DeserializeObject<UserSecrets>(secretsJson) ?? new UserSecrets();
            secrets.SigningPepper = Encoding.UTF8.GetString(HashSecurity.RandomizeKey(32));
            var updatedSecretsJson = JsonConvert.SerializeObject(secrets, Formatting.Indented);
            File.WriteAllText(secretsPath, updatedSecretsJson);
            return secrets.SigningPepper;     
        }

        public string NewJwtToken(ClientApp clientApi)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, $"{clientApi.Id}")
            };
            return NewJwtToken(claims);
        }

        public string NewJwtToken(UserAccount user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, $"{user.ID}"),
                new Claim(ClaimTypes.Name, $"{user.Username}")
            };
            return NewJwtToken(claims);
        }

        public string NewJwtToken(IEnumerable<Claim> claims)
        {
            try
            {
                byte[] key = _system.GetSigningSecret();
                if (key.Length <= 0) { return string.Empty; }
                var jwtHandler = new JwtSecurityTokenHandler();
                var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha512)
                {
                    CryptoProviderFactory = new CryptoProviderFactory { CacheSignatureProviders = false }
                };

                var token = new JwtSecurityToken(
                            issuer: "Issuer", //Issuer  
                            audience: "Audience", //Audience
                            claims: claims,
                            notBefore: DateTime.UtcNow,
                            expires: DateTime.UtcNow.AddMinutes(30),
                            signingCredentials: signingCredentials);
                return jwtHandler.WriteToken(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return string.Empty;
            }
        }

        public ClaimsPrincipal ValidateJwtToken(string token = "")
        {
            ClaimsPrincipal principal = new ClaimsPrincipal();
            try
            {
                token = !string.IsNullOrWhiteSpace(token)? token : GetJwtToken();
                if (string.IsNullOrEmpty(token))
                {
                    return new ClaimsPrincipal();
                }

                var tokenHandler = new JwtSecurityTokenHandler();
                principal = tokenHandler.ValidateToken(
                                token,
                                GetTokenValidationParameters(), 
                                out SecurityToken validatedToken
                            );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            return principal ?? new ClaimsPrincipal();
        }

        public TokenValidationParameters GetTokenValidationParameters(){         
            var validationParam = new TokenValidationParameters
            {
                ValidIssuer = "Issuer",
                ValidAudience = "Audience",
                RequireSignedTokens = true,
                RequireExpirationTime = true,
                ValidateIssuerSigningKey = true,
                TryAllIssuerSigningKeys = true,
                SaveSigninToken = true,
                ValidateLifetime = true,
                CryptoProviderFactory = new CryptoProviderFactory()
                {
                    CacheSignatureProviders = false
                },
                ValidateIssuer = true,
                ValidateAudience = true,
                // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                ClockSkew = TimeSpan.Zero
            };
            
            byte[] key = _system.GetSigningSecret();
            if(key.Length > 0){
                validationParam.IssuerSigningKey = new SymmetricSecurityKey(key);
            }
            return validationParam;
        }

        public string GetJwtToken()
        {
            return _accessor?.HttpContext?.Request.Headers["Authorization"]
                .FirstOrDefault()?.Split(" ").LastOrDefault() ?? string.Empty;
        }

        public bool Authenticated(string jwtToken = "")
        {
            ClaimsPrincipal principal = ValidateJwtToken(jwtToken);
            return principal?.Identity?.IsAuthenticated ?? false;
        }

        public RefreshToken NewRefreshToken(string ipAddress)
        {
            // generate token that is valid for 7 days
            using var rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            var randomBytes = new byte[64];
            rngCryptoServiceProvider.GetBytes(randomBytes);
            var refreshToken = new RefreshToken
            {
                Token = Convert.ToBase64String(randomBytes) ?? string.Empty,
                Expires = DateTime.UtcNow.AddDays(1),
                Created = DateTime.UtcNow,
                CreatedByIp = ipAddress
            };

            return refreshToken;
        }

    }
}
