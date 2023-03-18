using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.Services.Account
{
    public class UserCredentials
    {
        public string Code { set; get; }
        public string Username { set; get; }
        public string Password { set; get; }
        public string AccessToken { set; get; }
    }
}
