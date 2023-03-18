using Microsoft.AspNetCore.Mvc.Rendering;
using CKBS.Models.Services.Administrator.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace CKBS.Models.Services.Account
{
    public class LoginViewModel
    {
        [Required]
        public string Username { set; get; }
        [Required]
        public string Password { set; get; }
        public string Logo { set; get; }
        public string JwtToken { set; get; }
    }
}
