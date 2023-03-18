using KEDI.Core.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace KEDI.Core.Services.Authorization
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class ApiAuthorizeAttribute : ActionFilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var apiManager = context.HttpContext.RequestServices.GetService(typeof(IClientApiRepo)) as IClientApiRepo;
            if (!(bool)apiManager?.VerifyApiKey()!)
            {
                var jResponse = new HttpResult { StatusCode = StatusCodes.Status401Unauthorized, Message = "Unauthorized" };
                context.Result = new JsonResult(jResponse) { StatusCode = jResponse.StatusCode };
            }
        }
    }
}
