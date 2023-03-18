using KEDI.Core.Premise.Models.Sync;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.Services.LoyaltyProgram.PromoCode
{
    [Table("PromoCodeDiscount")]
    public class PromoCodeDiscount : ISyncEntity
    {
         [Key]
         public int ID { get; set; }
         public string Code { get; set; }
         public string Name { get; set; }
         [Required]
         [Column(TypeName = "Date")]
         public DateTime DateF { get; set; }
         public string TimeF { get; set; }
         [Required]
         [Column(TypeName = "Date")]
         public DateTime DateT { get; set; }
         public string TimeT { get; set; }
         public PromoType PromoType { get; set; }
         public Decimal PromoValue { get; set; }
         public int PriceListID { get; set; } 
         public int PromoCount { get; set; }
         public int UseCountCode { get; set; }
         public int StringCount { get; set; }
         public bool Active { get; set; }
         public bool Used { get; set; }

        //ISyncEntity
        public Guid RowId { set; get; }
        public DateTimeOffset ChangeLog { set; get; }
        public int Spk { get; set; } // Server Primary Key
        public int Cpk { get; set; } // Client Primary Key
        public bool Synced { get; set; }
        public DateTime SyncDate { get; set; }

    }
    public enum PromoType { Percent = 1,Value=2 }
}
