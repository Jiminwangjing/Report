using KEDI.Core.Security.Cryptography;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Hosting.Models
{
    public class ContractResponse<TResult> : IActionResult
    {
        public ContractResponse() { }
        public ContractResponse(TResult data)
        {
            Result = data;
        }

        public TResult Result { set; get; }
        public string RedirectUrl { set; get; }
        public string SessionToken { set; get; }
        public string JwtToken { set; get; }
        public StatusCodeResult StatusCode { set; get; }
        public async Task ExecuteResultAsync(ActionContext context)
        {
            var result = new OkObjectResult(this)
            {
                StatusCode = StatusCode != null
                ? StatusCodes.Status500InternalServerError
                : StatusCodes.Status200OK
            };
            
            await result.ExecuteResultAsync(context);
        }
    }
}
