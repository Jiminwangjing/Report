using CKBS.Models.Services.Administrator.Inventory;
using CKBS.Models.Services.LoyaltyProgram.ComboSale;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CKBS.Models.ServicesClass.Property;
using KEDI.Core.Premise.Models.ServicesClass.SerialBatches.Sales;

namespace CKBS.Models.Services.POS
{
    public enum PromotionType
    {
        [Display(Name = "None")]
        None,
        [Display(Name = "Buy one item get one item")]
        BuyXGetX,
        [Display(Name = "Combo Sale")]
        ComboSale,
        [Display(Name = "Buy X Qty Get X Discount")]
        BuyXQtyGetXDiscount
    }

    public enum LinePosition
    {
        [Display(Name = "Parent")]
        Parent,
        [Display(Name = "Children")]
        Children
    }

    [Table("tbOrderDetail", Schema = "dbo")]
    public class OrderDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderDetailID { get; set; } // Price list detail identity
        public int? OrderID { get; set; }
        public int Line_ID { get; set; }
        public string LineID { get; set; }
        public string CopyID { set; get; }
        public int ItemID { get; set; }
        public string Prefix { set; get; }
        public string Code { get; set; }
        public string KhmerName { get; set; }
        public string EnglishName { get; set; }
        public double Cost { get; set; }

        public double BaseQty { set; get; }
        public double Qty { get; set; }
        public double ReturnQty { get; set; }
        public double PrintQty { get; set; }
        public int UomID { get; set; }
        public string Uom { get; set; }
        [NotMapped]
        public List<SelectListItem> ItemUoMs { set; get; }
        public int GroupUomID { set; get; }
        public double UnitPrice { get; set; }
        public double DiscountRate { get; set; }
        public double DiscountValue { get; set; }
        [NotMapped]
        public List<SelectListItem> RemarkDiscounts { get; set; }
        public string TypeDis { get; set; }
        [NotMapped]
        public List<SelectListItem> TaxGroups { set; get; }
        public int TaxGroupID { set; get; }
        public decimal TaxRate { set; get; }
        public decimal TaxValue { set; get; }
        public double Total { get; set; }
        public double Total_Sys { get; set; }
        public double TotalNet { get; set; }
        public string ItemStatus { get; set; } //new,old
        public string ItemPrintTo { get; set; }
        public string Currency { get; set; }
        public string Comment { get; set; }
        [NotMapped]
        public string CommentUpdate { get; set; }
        public string ItemType { get; set; }
        public string Description { get; set; }
        public string ParentLineID { get; set; }
        public string ParentLevel { get; set; } //Line_ID+auto count line add on
        public bool IsReadonly { set; get; }
        [NotMapped]
        public List<SelectListItem> Printers { set; get; }
        [NotMapped]
        public string PromoTypeDisplay { set; get; }
        public PromotionType PromoType { set; get; }
        public LinePosition LinePosition { set; get; }
        [ForeignKey("UomID")]
        public UnitofMeasure UnitofMeansure { get; set; }
        public bool IsVoided { get; set; }
        public int KSServiceSetupId { get; set; }
        public int VehicleId { get; set; }
        public bool IsKsms { get; set; }
        public bool IsKsmsMaster { get; set; }
        public bool IsScale { get; set; }
        public SaleType ComboSaleType { get; set; }
        public int RemarkDiscountID { get; set; }
        public List<SerialNumber> SerialNumbers { get; set; }
        public List<BatchNo> BatchNos { get; set; }

        [NotMapped]
        public Dictionary<string, PropertydetailsViewModel> AddictionProps { get; set; }
        [NotMapped]
        public SerialNumberSelectedDetail SerialNumberSelectedDetial { get; set; }
        [NotMapped]
        public SerialNumber SerialNumber { get; set; }
        [NotMapped]
        public BatchNo BatchNo { get; set; }
        [NotMapped]
        public bool IsOutOfStock { get; set; }
        public bool IsSerialNumber { get; set; }
        public bool IsBatchNo { get; set; }
        [NotMapped]
        public double Instock { get; set; }
    }
}
