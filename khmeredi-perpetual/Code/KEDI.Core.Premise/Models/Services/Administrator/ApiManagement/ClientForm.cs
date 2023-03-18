using System;
using System.ComponentModel.DataAnnotations;

namespace KEDI.Core.Models.ControlCenter.ApiManagement
{
    public class ClientForm
    {
        public long Id { get; set; }
        [Required]
        public string ClientCode { set; get; }
        [Required]
        public string ClientName { set; get; }
        public string ApiKey { set; get; }
        public string SecretKey { set; get; }
        public bool StrictIpAddress { set; get; }
        public bool Readonly { set; get; }
        public bool Revoked { set; get; }
    }
}
