using CKBS.Models.Services.Banking;
using CKBS.Models.Services.Inventory.PriceList;
using KEDI.Core.Premise.Models.Sync;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KEDI.Core.Premise.Models.Services.Administrator.CanRingSetup
{
    [Table("CanRing")]
    public class CanRing : MainCanRing
    {
        public int ID { get; set; }
        [NotMapped]
        public int CanRingID { get; set; }
    }

    public enum ActiveCanRingType { All = 0, Active = 1, InActive = 2}
    public class MainCanRing : ISyncEntity
    {
        [NotMapped]
        public string LineID { get; set; } = DateTime.Now.Ticks.ToString();
        public int ItemID { get; set; }
        public int ItemChangeID { get; set; }
        public int UomID { get; set; }
        public int UomChangeID { get; set; }
        public int PriceListID { get; set; }
        public int UserID { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "0:yyyy-MM-dd")]
        [Column(TypeName = "Date")]
        public DateTime CreatedAt { get; set; } = DateTime.Today;
        [NotMapped]
        public string CreatedAtDis { get; set; }
        public string Name { get; set; }
        [NotMapped]
        public List<SelectListItem> PriceLists { get; set; }
        [NotMapped]
        public string PriceList { get; set; }
        [NotMapped]
        public string ItemName { get; set; }
        public decimal Qty { get; set; }
        [NotMapped]
        public string QtyDis { get; set; }
        [NotMapped]
        public List<SelectListItem> UomLists { get; set; }
        [NotMapped]
        public string UomName { get; set; }
        [NotMapped]
        public string ItemChangeName { get; set; }
        public decimal ChangeQty { get; set; }
        [NotMapped]
        public string ChangeQtyDis { get; set; }
        [NotMapped]
        public List<SelectListItem> UomChangeLists { get; set; }
        [NotMapped]
        public string UomChangeName { get; set; }
        public decimal ChargePrice { get; set; }
        [NotMapped]
        public decimal ExchangRate { get; set; }
        [NotMapped]
        public string Total { get; set; }
        [NotMapped]
        public string Currency { get; set; }
        [NotMapped]
        public List<ExchangeRate> ExchangeRates { get; set; }
        [NotMapped]
        public List<PriceLists> ListPriceLists { get; set; }
        public bool IsActive { get; set; }


        //ISyncEntity
        public Guid RowId { set; get; }
        public DateTimeOffset ChangeLog { set; get; }
        public int Spk { get; set; } // Server Primary Key
        public int Cpk { get; set; } // Client Primary Key
        public bool Synced { get; set; }
        public DateTime SyncDate { get; set; }
    }
}
