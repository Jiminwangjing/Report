using CKBS.Controllers;
using KEDI.Core.Premise.Controllers;
using KEDI.Core.Premise.Repository;
using KEDI.Core.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Authorization
{
    public sealed class PrivilegeAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public PrivilegeAttribute() { }
        public PrivilegeAttribute(string code)
        {
            Code = code;
        }
        
        public string Code { set; get; }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            try
            {                            
                var allowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
                if (allowAnonymous) { return; }
              
                var userModule = (UserManager)context.HttpContext.RequestServices.GetService(typeof(UserManager));       
                var user = userModule.CurrentUser;
                if (!string.IsNullOrWhiteSpace(Code))
                {
                    if (!userModule.Check(user.ID, Code))
                    {
                        context.Result = new ForbidResult();
                    }
                }               
            }
            catch
            {
                context.Result = new JsonResult(new { Message = "InternalServerError" }) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }
    }
}
