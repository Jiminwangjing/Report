using KEDI.Core.Premise.Models.Sync;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CKBS.Models.Services.Inventory.PriceList
{
    [Table("tbPriceListDetail", Schema = "dbo")]
    public class PriceListDetail : ISyncEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int PriceListID { get; set; }
        public int ItemID { get; set; }
        public int UserID { get; set; }
        public int? UomID { get; set; }
        public int CurrencyID { get; set; }
        public double Quantity { get; set; } = 1;
        public float Discount { get; set; } = 0;
        public string TypeDis { get; set; } = "Percent";
        public int PromotionID { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ExpireDate { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime SystemDate { get; set; }
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh:mm-ss}", ApplyFormatInEditMode = true)]
        public DateTime TimeIn { get; set; }

        [MinLength(0)]
        public double Cost { get; set; }
        [MinLength(0)]
        public double UnitPrice { get; set; }
        [ForeignKey("PriceListID")]
        public PriceLists PriceLists { get; set; }
        public string Barcode { get; set; }

        //ISyncEntity
        public Guid RowId { set; get; }
        public DateTimeOffset ChangeLog { set; get; }
        public int Spk { get; set; } // Server Primary Key
        public int Cpk { get; set; } // Client Primary Key
        public bool Synced { get; set; }
        public DateTime SyncDate { get; set; }
    }
}
