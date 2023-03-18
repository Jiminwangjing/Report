using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KEDI.Core.Premise.Models.Sync;

namespace Models.Services.LoyaltyProgram.PromotionDiscount
{
    [Table("tbPromotionDetail", Schema = "dbo")]
    public class PromotionDetail : ISyncEntity
    {
        [Key]
        public int ID { get; set; }
        public int PriceListID { get; set; }
        public int CurrencyID { get; set; }
        public int UomID {get;set;}
        public int ItemID { get; set; }
        public double Cost {get;set;}
        public double UnitPrice {get;set;}
        public int UserID { get; set; }
        public float Discount { get; set; } = 0;
        public string TypeDis { get; set; } = "Percent";
        public int PromotionID { get; set; }
        public TimeSpan StartTime {get;set;}
        public TimeSpan StopTime {get;set;}
        public DateTime? StartDate { get; set; }
        public DateTime? StopDate { get; set; }
        public Guid RowId { set; get; }
        public DateTimeOffset ChangeLog { set; get; }
        public int Spk { get; set; } // Server Primary Key
        public int Cpk { get; set; } // Client Primary Key
        public bool Synced { get; set; }
        public DateTime SyncDate { get; set; }
    }
}