using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.System.Models
{
    public enum SystemEdition { Starter = 1, Basic = 2, Standard = 3, Professional = 4, Enterprise = 5 }
    public enum SystemRole { Premise, Cloud }
    [Table("SystemLicense")]
    public class SystemLicense
    {
        [Key]
        public Guid ID { set; get; }
        [Required]
        public string TenantID { set; get; }
        public string Name { set; get; }
        public string ApiKey { set; get; }
        public string DeviceKey { set; get; }
        public string EntryKey { set; get; }
        public string PrivateKey { set; get; }
        public string PublicKey { set; get; }
        public string SecretKey { set; get; }       
        public int MaximumUsers { set; get; }
        public long Expiration { set; get; }
        public bool Unlimited { set; get; }
        public string Version { set; get; }
        public string TimeStamp { set; get; }
        public string Signature { set; get; }
        public string Certificate { set; get; }
        public string ServerHost { set; get; }
        public string TenantHost { set; get; }
        public SystemRole Role { set; get; }
        public SystemEdition Edition { get; set; }
        [NotMapped]
        public Dictionary<string, bool> SystemTypes { set; get; }
    }
}
