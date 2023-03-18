using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CKBS.Models.Services.LoyaltyProgram.ComboSale;
using CKBS.Models.Services.POS;

namespace KEDI.Core.Premise
{
    public class ReceiptDetailsExport
    {
        public string SerialCode { get; set; }
        public string SeriesNumber { get; set; }
        public string LineID { get; set; }
        public int Line_ID { get; set; }
        public int ItemID { get; set; }
        public string Code { get; set; }
        public string KhmerName { get; set; }
        public decimal AmountFreight { get; set; }
        public string EnglishName { get; set; }
        public double Qty { get; set; }
        public double PrintQty { get; set; }
        public double OpenQty { get; set; }
        public double UnitPrice { get; set; }
        public double Cost { get; set; }
        public double DiscountRate { get; set; }
        public double DiscountValue { get; set; }
        public string TypeDis { get; set; }
        public string TaxGroupCode { set; get; }
        public decimal TaxRate { set; get; }
        public decimal TaxValue { set; get; }
        public double Total { get; set; }
        public double Total_Sys { get; set; }
        public double TotalNet { get; set; }
        public string UomCode { get; set; }
        public string ItemStatus { get; set; }//new,old
        public string ItemPrintTo { get; set; }
        public string Currency { get; set; }
        public string Comment { get; set; }
        public string ItemType { get; set; }
        public string Description { get; set; }
        public string ParentLineID { get; set; }
        public string ParentLevel { get; set; }
        public PromotionType PromoType { set; get; }
        public LinePosition LinePosition { set; get; }
        //public UnitofMeasure UnitofMeansure { get; set; }

        public bool IsKsms { get; set; }
        public bool IsKsmsMaster { get; set; }
        public int KSServiceSetupId { get; set; }
        public int VehicleId { get; set; }
        public bool IsScale { set; get; }
        public SaleType ComboSaleType { get; set; }
        public bool IsReadonly { get; set; }
        public int RemarkDiscountID { get; set; }
    }
}