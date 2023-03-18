using KEDI.Core.Premise.Models.Sync;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CKBS.Models.Services.LoyaltyProgram.BuyXAmountGetXDiscount
{
    public enum TypeDiscountBuyXAmountGetXDiscount
    {
        Rate = 1,
        Value = 2,
    }
    [Table("PBuyXGetXDis")]
    public class PBuyXAmountGetXDis : ISyncEntity
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int PriListID { get; set; }
        public DateTime DateF { get; set; }
        public DateTime DateT { get; set; }
        public decimal Amount { get; set; }
        public TypeDiscountBuyXAmountGetXDiscount DisType { get; set; }
        public decimal DisRateValue { get; set; }
        public bool Active { get; set; }

        //ISyncEntity
        public Guid RowId { set; get; }
        public DateTimeOffset ChangeLog { set; get; }
        public int Spk { get; set; } // Server Primary Key
        public int Cpk { get; set; } // Client Primary Key
        public bool Synced { get; set; }
        public DateTime SyncDate { get; set; }
    }
}
