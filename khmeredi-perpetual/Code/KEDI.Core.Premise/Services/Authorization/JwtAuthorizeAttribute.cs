
using KEDI.Core.Premise.Repository;
using KEDI.Core.Repository;
using KEDI.Core.Services.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Security.Claims;

namespace KEDI.Core.Services.Authorization
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class JwtAuthorizeAttribute : ActionFilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            bool allowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
            if (allowAnonymous) { return; }

            bool allowApi = context.ActionDescriptor.EndpointMetadata.OfType<ApiAuthorizeAttribute>().Any();
            if (allowApi) { return; }

            var jwtManager = context.HttpContext.RequestServices.GetService(typeof(JwtManager)) as JwtManager;
            if(!(jwtManager?.Authenticated()?? false))
            {
                var jResponse = new HttpResult { StatusCode = StatusCodes.Status401Unauthorized, Message = "Unauthorized" };
                context.Result = new JsonResult(jResponse) { StatusCode = jResponse.StatusCode };
            }           
        }
    }
}