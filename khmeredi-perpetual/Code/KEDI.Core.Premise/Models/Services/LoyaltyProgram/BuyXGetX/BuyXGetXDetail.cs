using KEDI.Core.Premise.Models.Sync;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.Services.LoyaltyProgram.BuyXGetX
{
    [Table("BuyXGetXDetail")]
    public class BuyXGetXDetail : ISyncEntity
    {
        [Key]
        public int ID { get; set; }
        public int BuyXGetXID { get; set; }
        public string Procode { get; set; }
        public int BuyItemID { get; set; }
        public int ItemUomID { get; set; }
        public decimal BuyQty { get; set; }
        //Get Item
        public int GetItemID { get; set; }
        public int GetUomID { get; set; }
        public decimal GetQty { get; set; }
        public string LineID { get; set; }
        public bool Delete { get; set; }


        //ISyncEntity
        public Guid RowId { set; get; }
        public DateTimeOffset ChangeLog { set; get; }
        public int Spk { get; set; } // Server Primary Key
        public int Cpk { get; set; } // Client Primary Key
        public bool Synced { get; set; }
        public DateTime SyncDate { get; set; }

    }
}
