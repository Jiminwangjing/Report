using CKBS.Models.Services.Administrator.Inventory;
using CKBS.Models.Services.LoyaltyProgram.ComboSale;
using KEDI.Core.Premise.Models.Sync;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CKBS.Models.Services.POS
{
    [Table("tbVoidOrderDetail", Schema = "dbo")]
    public class VoidOrderDetail : ISyncEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderDetailID { get; set; }// Price list detail identity
        public int? OrderID { get; set; }
        public int Line_ID { get; set; }
        public string LineID { get; set; }
        public int ItemID { get; set; }
        public string Code { get; set; }
        public string KhmerName { get; set; }
        public string EnglishName { get; set; }
        public double Qty { get; set; }
        public double PrintQty { get; set; }
        public double UnitPrice { get; set; }
        public double Cost { get; set; }
        public double DiscountRate { get; set; }
        public double DiscountValue { get; set; }
        public string TypeDis { get; set; }
        public int TaxGroupID { set; get; }
        public decimal TaxRate { set; get; }
        public decimal TaxValue { set; get; }
        public double Total { get; set; }
        public double Total_Sys { get; set; }
        public double TotalNet { get; set; }
        public int UomID { get; set; }
        public string ItemStatus { get; set; }//new,old
        public string ItemPrintTo { get; set; }
        public string Currency { get; set; }
        public string Comment { get; set; }
        public string ItemType { get; set; }
        public string Description { get; set; }
        public string ParentLineID { get; set; }
        public string ParentLevel { get; set; }//Line_ID+auto count line add on

        [ForeignKey("UomID")]
        public UnitofMeasure UnitofMeansure { get; set; }
        public int KSServiceSetupId { get; set; }
        public int VehicleId { get; set; }
        public bool IsKsms { get; set; }
        public bool IsKsmsMaster { get; set; }
        public bool IsScale { get; set; }
        public bool IsReadonly { get; set; }
        public SaleType ComboSaleType { get; set; }
        public int RemarkDiscountID { get; set; }

        //ISyncEntity
        public Guid RowId { set; get; }
        public DateTimeOffset ChangeLog { set; get; }
        public int Spk { get; set; } // Server Primary Key
        public int Cpk { get; set; } // Client Primary Key
        public bool Synced { get; set; }
        public DateTime SyncDate { get; set; }
    }
}
