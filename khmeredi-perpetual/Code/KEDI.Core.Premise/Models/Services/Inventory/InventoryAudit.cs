using CKBS.Models.Services.Account;
using CKBS.Models.Services.Administrator.General;
using CKBS.Models.Services.Administrator.Inventory;
using CKBS.Models.Services.Banking;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Inventory
{
    [Table("tbInventoryAudit", Schema = "dbo")]
    public class InventoryAudit
    {
        [Key]
        public int ID { get; set; }
        public int WarehouseID { get; set; }
        public int BranchID { get; set; }
        public int UserID { get; set; }
        public int ItemID { get; set; }
        public int CurrencyID { get; set; }
        public int UomID { get; set; }
        public int DocumentTypeID { get; set; }
        public int SeriesID { get; set; }
        public int SeriesDetailID { get; set; }
        public int CompanyID { get; set; }
        public string InvoiceNo { get; set; }
        public string Trans_Type { get; set; }
        public string Process { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Column(TypeName = "Date")]
        public DateTime SystemDate { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Column(TypeName = "Date")]
        public DateTime PostingDate { get; set; }
        public string TimeIn { get; set; }
        public double Qty { get; set; }
        public double Cost { get; set; }
        public double Price { get; set; }
        public double CumulativeQty { get; set; }
        public double CumulativeValue { get; set; }
        public double Trans_Valuse { get; set; }
        public string TypeItem { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Column(TypeName = "Date")]
        public DateTime ExpireDate { get; set; }
        public int LocalCurID { get; set; }
        public double LocalSetRate { get; set; }
        public string LineID { get; set; }
        //ForeignKey
        [ForeignKey("WarehouseID")]
        public Warehouse Warehouse { get; set; }
        [ForeignKey("BranchID")]
        public Branch Branch { get; set; }
        [ForeignKey("UserID")]
        public UserAccount UserAccount { get; set; }
        [ForeignKey("ItemID")]
        public ItemMasterData ItemMasterData { get; set; }
        [ForeignKey("CurrencyID")]
        public Currency Currency { get; set; }
        [ForeignKey("UomID")]
        public UnitofMeasure UnitofMeasure { get; set; }
    }
}
