using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.Services.Account
{
    public class ChangePasswordModel
    {
        [Required]
        public string Username { set; get; }
        [Required]
        public string OldPassword { set; get; }
        [Required]
        public string NewPassword { set; get; }
        [Required]
        public string ConfirmedNewPassword { set; get; }
    }
}
