using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KEDI.Core.Premise.Models.Services.POS
{
    public class VoidReason
    {
        [Key]
        public int ID { set; get; }
        public string Reason { get; set; }
        public bool Delete { get; set; }
        [NotMapped]
        public string Action { get; set; }
    }
}
