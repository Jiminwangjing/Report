using CKBS.Models.Services.Account;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace KEDI.Core.Premise.Models.ClientApi.UserAccount
{
    public class BearerResponse
    {
        public string Username { get; set; }
        public string AccessToken { get; set; }

        [JsonIgnore] // refresh token is returned in http only cookie
        public string RefreshToken { get; set; }
        public List<string> Messages { set; get; } = new List<string>();
    }
}