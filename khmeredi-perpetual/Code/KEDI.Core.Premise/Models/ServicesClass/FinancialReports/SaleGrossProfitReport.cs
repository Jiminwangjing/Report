using CKBS.Models.Services.Inventory;
using System;

namespace KEDI.Core.Premise.Models.ServicesClass.FinancialReports
{
    public class SaleGrossProfitReport
    {
        public string LineID { get; set; } = DateTime.Now.Ticks.ToString();
        public string Code { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string InvoiceNo { get; set; }
        public string PostingDate { get; set; }
        public string DateOut { get; set; }
        public decimal Cost { get; set; }
        public decimal Price { get; set; }
        public string UoMName { get; set; }
        public decimal Qty { get; set; }
        public decimal Total { get; set; }
        public decimal TotalItem { get; set; }
        public decimal TotalGrossProfit { get; set; }

        public string TimeIn { get; set; }
        public string TimeOut { get; set; }
        public string CostF { get; set; }
        public string PriceF { get; set; }
        public string QtyF { get; set; }
        public string TotalF { get; set; }
        public string TotalItemF { get; set; }
        public string TotalGrossProfitF { get; set; }
        public int ItemID { get; set; }
        public ManageItemBy ManItemBy { get; set; }
        public decimal Discount { get; set; }
        public string DiscountF { get; set; }
        public decimal GrossProfit { get; set; }
        public string GrossProfitF { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal TotalCost { get; set; }
        public decimal TotalItemCost { get; set; }
        public decimal TotalItemPrice { get; set; }
        public string TotalItemCostF { get; set; }
        public string TotalItemPriceF { get; set; }
        public decimal GrossProfitItem { get; set; }
        public string GrossProfitItemF { get; set; }
        public string TotalPriceF { get; set; }
        public string TotalCostF { get; set; }
        public string TotalAfterDisF { get; set; }
        public decimal TotalAfterDis { get; set; }
        public decimal TotalAfterDisItem { get; set; }
        public string TotalAfterDisItemF { get; set; }
        public string TotalAfterDisAllF { get; set; }
        public double DisRateM { get; set; }
        public double DisRate { get; set; }
        public string TotalDiscountF { get; set; }
        public string TotalAllCostF { get; set; }
        public string TotalAllPriceF { get; set; }
        public decimal TotalDiscountItem { get; set; }
        public string TotalDiscountItemF { get; set; }
        public decimal GrossProfitItemP { get; set; }
        public string GrossProfitItemPF { get; set; }
        public decimal GrossProfitP { get; set; }
        public string GrossProfitPF { get; set; }
        public decimal TotalGrossProfitP { get; set; }
        public string TotalGrossProfitPF { get; set; }
        public int ABID { get; set; }
    }
}
