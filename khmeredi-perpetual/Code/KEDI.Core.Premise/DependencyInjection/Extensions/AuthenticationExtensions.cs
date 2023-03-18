using System;
using System.Text;
using CKBS.AppContext;
using KEDI.Core.Premise.DependencyInjection;
using KEDI.Core.Premise.Models.Credentials;
using KEDI.Core.Premise.Repository;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace KEDI.Core.Premise.DependencyInjection.Extensions
{
    public static class AuthenticationExtensions {
        public static  IServiceCollection AddKediAuthentication(this IServiceCollection services){
           
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options => {
                options.LoginPath = "/account/login";
            })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>{
                var jwtManager = services.BuildServiceProvider()
                            .GetRequiredService<JwtManager>();
                options.TokenValidationParameters = jwtManager.GetTokenValidationParameters();
            });
            return services;
        }
    }
}
