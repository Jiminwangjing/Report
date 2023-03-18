using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.ClientApi.UserAccount
{
    public class ApiKeyLogin
    {
        [Required]
        public string ApiKey { set; get; }
        [Required]
        public string SecretKey { set; get; }

    }
}
