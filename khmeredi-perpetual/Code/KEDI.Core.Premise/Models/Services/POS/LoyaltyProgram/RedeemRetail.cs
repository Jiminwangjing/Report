using KEDI.Core.Premise.Models.Sync;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.Services.POS.LoyaltyProgram
{
    [Table("RedeemRetail")]
    public class RedeemRetail : ISyncEntity
    {
        [Key]
        public int ID { get; set; }
        public int RID { get; set; }
        public int ItemID { get; set; }
       
        public string ItemName { get; set; }
        public string Uom { get; set; }
        public int UomID { get; set; }
        public decimal Qty { get; set; }
        public int WarehouseID { get; set; }

        //ISyncEntity
        public Guid RowId { set; get; }
        public DateTimeOffset ChangeLog { set; get; }
        public int Spk { get; set; } // Server Primary Key
        public int Cpk { get; set; } // Client Primary Key
        public bool Synced { get; set; }
        public DateTime SyncDate { get; set; }
    }
}
