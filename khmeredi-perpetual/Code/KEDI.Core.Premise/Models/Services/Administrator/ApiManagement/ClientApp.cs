using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KEDI.Core.Models.ControlCenter.ApiManagement
{
    [Table("ClientApp")]
    public class ClientApp
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { set; get; }
        public string Code { set; get; }
        public string Name { set; get; }  
        public string AppId { set; get; }
        public string IpAddress { set; get; }
        [JsonIgnore]
        public string Signature { set; get; }
        [JsonIgnore]
        public string PublicKey { set; get; }
        public DateTimeOffset CreatedDate { set; get; }
        public int UserId { set; get; }
        public bool StrictIpAddress { set; get; }
        public bool IsReadonly { set; get; }
        public bool IsRevoked { set; get; }
        public string Description { set; get; }
        public ClientEnvironment Environment { set; get; }
    }

    public enum ClientEnvironment { Sandbox, Staging, Production }
}
