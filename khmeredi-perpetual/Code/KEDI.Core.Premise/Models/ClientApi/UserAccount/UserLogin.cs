using System.ComponentModel.DataAnnotations;

namespace KEDI.Core.Premise.Models.ClientApi.UserAccount
{
    public class UserLogin
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}