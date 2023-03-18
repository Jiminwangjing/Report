using KEDI.Core.Premise.Models.Sync;
using System;

namespace CKBS.Models.Services.LoyaltyProgram.BuyXAmountGetXDiscount
{
    public class BuyXQtyGetXDis : ISyncEntity
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public DateTime DateF { get; set; }
        public DateTime DateT { get; set; }
        public int BuyItemID { get; set; }
        public decimal Qty { get; set; }
        public int UomID { get; set; }
        public int DisItemID { get; set; }
        public decimal DisRate { get; set; }
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
