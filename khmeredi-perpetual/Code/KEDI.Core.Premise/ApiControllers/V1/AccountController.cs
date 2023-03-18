using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using KEDI.Core.Premise.Authorization;
using System;
using KEDI.Core.Premise.Repository;
using KEDI.Core.Services.Authorization;
using System.Threading.Tasks;
using KEDI.Core.Repository;
using KEDI.Core.Premise.Models.ClientApi.UserAccount;
using CKBS.Models.Services.Account;
using CKBS.AppContext;

namespace KEDI.Core.Premise.ApiControllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Privilege(AuthenticationSchemes = "Bearer")]
    public class AccountController : ControllerBase
    {
        readonly UserManager _userModule;
        readonly IClientApiRepo _apiManager;
       
        public AccountController(UserManager userModule, IClientApiRepo apiManager)
        {
            _userModule = userModule;
            _apiManager = apiManager;
           
        }

        [AllowAnonymous]
        [HttpPost("getSystemTypes")]
        public IActionResult GetSystemTypes()
        {
            var systemTypes = _userModule.GetActiveSystemTypes();
            return Ok(systemTypes);
        }

        [AllowAnonymous]
        [HttpPost("loginApiKey")]
        public async Task<IActionResult> LoginApiKey([FromBody] ApiKeyLogin model)
        {
            var response = await _apiManager.LoginAsync(model);
            _userModule.SetRefreshTokenCookie(response.RefreshToken);
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Authenticate([FromBody] UserLogin model)
        {
            var response = _userModule.Authenticate(model);
            _userModule.SetRefreshTokenCookie(response.RefreshToken);
            return Ok(response);
        }
        
        [AllowAnonymous]
        [HttpPost("refreshToken")]
        public IActionResult RefreshToken()
        {
            var response = _userModule.RefreshToken();
            _userModule.SetRefreshTokenCookie(response.RefreshToken);
            return Ok(response);
        }

        [HttpPost("revokeToken")]
        public IActionResult RevokeToken(RevokeTokenRequest model)
        {
            var token = model.RefreshToken ?? _userModule.GetRefreshTokenCookie();
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest(new { Message = "Token is required" });
            }

            _userModule.RevokeToken(token);
            return Ok(new { Message = "Token revoked" });
        }

        [HttpPost("{id}/refreshTokens")]
        public IActionResult GetRefreshTokens(int id)
        {
            var user = _userModule.FindById(id);
            return Ok(user.RefreshTokens);
        }
    }
}
