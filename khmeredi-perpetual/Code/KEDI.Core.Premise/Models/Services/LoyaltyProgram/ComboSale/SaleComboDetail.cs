using KEDI.Core.Premise.Models.Sync;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CKBS.Models.Services.LoyaltyProgram.ComboSale
{
    [Table("ComboDetails")]
    public class SaleComboDetail : ISyncEntity
    {
        [Key]
        public int ID { get; set; }
        public int SaleComboID { get; set; }
        public int ItemID { get; set; }
        [NotMapped]
        public string LineID { get; set; }
        [NotMapped]
        public string Code { get; set; }
        public double Qty { get; set; }
        [NotMapped]
        public List<SelectListItem> UomSelect { get; set; }
        [NotMapped]
        public string KhmerName { get; set; }
        public int UomID { get; set; }
        public int GUomID { get; set; }
        public bool Detele { get; set; }

        //ISyncEntity
        public Guid RowId { set; get; }
        public DateTimeOffset ChangeLog { set; get; }
        public int Spk { get; set; } // Server Primary Key
        public int Cpk { get; set; } // Client Primary Key
        public bool Synced { get; set; }
        public DateTime SyncDate { get; set; }
    }
}
