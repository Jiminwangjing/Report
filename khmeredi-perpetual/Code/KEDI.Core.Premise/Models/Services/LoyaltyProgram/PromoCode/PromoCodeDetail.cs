using KEDI.Core.Premise.Models.Sync;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.Services.LoyaltyProgram.PromoCode
{
    [Table("PromoCodeDetail")]
    public class PromoCodeDetail : ISyncEntity
    {
        [Key]
        public int PromoCodeID { get; set; }
        public int ID { get; set; }
        public string PromoCode { get; set; }
        public int UseCount { get; set; }
        public int MaxUse { get; set; }
        public Status Status { get; set; }

        //ISyncEntity
        public Guid RowId { set; get; }
        public DateTimeOffset ChangeLog { set; get; }
        public int Spk { get; set; } // Server Primary Key
        public int Cpk { get; set; } // Client Primary Key
        public bool Synced { get; set; }
        public DateTime SyncDate { get; set; }
    }
    public enum Status { Open = 1, Closed = 2 }


}
