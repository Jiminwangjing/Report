using CKBS.Models.Services.Inventory.Category;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.ReportSale.dev
{
    public class GroupTopSaleQty
    {
        //Group
        public string GroupName { get; set; }
        public string Group1 { get; set; }
        public string Group2 { get; set; }
        public string Group3 { get; set; }     
        public int ItemID { get; set; }
        public double Qty { get; set; }
        //List<Topsaleviewmodel> ViewTopsaleDatas { get;set }
        public Header Header { get; set; }
        public List<Topsaleviewmodel> Topsaleviewmodels { get; set; }
        public Footer Footer { get; set; }
        /// Calculation props
        public double PriceCal { get; set; }
        public double SubTotalCal { get; set; }
        public double SubTotal { get; set; }
        public double TotalCal { get; set; }
        public double TotalQtyCal { get; set; }
        public double SDiscountItemCal { get; set; }
        public double SDiscountTotalCal { get; set; }
        public double SVatCal { get; set; }
        public double SGrandTotalSysCal { get; set; }
        public double SGrandTotalCal { get; set; }
        public string GrandTotalBrand { get; set; }
        public double GranTotalbr { get; set; }
        public double UnitPrice { get; set; }
        public double LDiscountItemCal { get; set; }
        public double LDiscountTotalCal { get; set; }
        public double LVatCal { get; set; }
        public double LGrandTotalSysCal { get; set; }
        public double LGrandTotalCal { get; set; }
    }
    public class Topsaleviewmodel
    {
       public int ID{ get; set; }
       public Header Header { get; set; }
        public Footer Footer { get; set; }
        public List<Topsaleviewmodel> Topsaleviewmodels { get; set; }
        public int ItemID { get; set; }
        public string Barcode { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public double Qty { get; set; }
        public double ReturnQty { get; set; }
        public double TotalQty { get; set; }
        public string Uom { get; set; }
        public string Price { get; set; }
        public double Total { get; set; }
        public int C { get; set; }
        public int CurrencyId { get; set; }
        public string Group1 { get; set; }
        public string Group2 { get; set; }
        public string Group3 { get; set; }
        public string GroupName { get; set; }
        public string Code { get; set; }
        public string KhmerName { get; set; }
        public string SysCurrency { get; set; }
        public string Currency { get; set; }
        public string LocalCurrency { get; set; }
        public double SubTotal { get; set; }
        public string DateTo { get; set; }
        public string DateFrom { get; set; }
        public string SDiscountItem { get; set; }
        public string SDiscountTotal { get; set; }
        public string SVat { get; set; }
        public double SGrandTotalSys { get; set; }
        public double SGrandTotal { get; set; }
        public int BrandID { get; set; }
        /// Calculation props
        public double PriceCal { get; set; }
        public double SubTotalCal { get; set; }
        public double TotalCal { get; set; }
        public double TotalQtyCal { get; set; }
        public double SDiscountItemCal { get; set; }
        public double SDiscountTotalCal { get; set; }
        public double SDiscountItemCallocal { set; get; }
        public double SVatCal { get; set; }
        public double SGrandTotalSysCal { get; set; }
        public string SGrandTotalCal { get; set; }
        public string GrandTotalBrand { get; set; }
        public double GranTotalbr { get; set; }
        public double UnitPrice { get; set; }
        public double LDiscountItemCal { get; set; }
        public double LDiscountTotalCal { get; set; }
        public double LVatCal { get; set; }
        public double LGrandTotalSysCal { get; set; }
        public double LGrandTotalCal { get; set; }
        public double LocalSetRate { get; set; }
    }

    public class ViewTopsaleData
    {
        public string Barcode { get; set; }
        public string Code { get; set; }
        public string Currency { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public string KhmerName { get; set; }
        public double TotalCal { get; set; }
        public string Price { get; set; }
        public double SubTotalCal { get; set; }
        public string Uom { get; set; }
        public double Qty { get; set; }
        public double ReturnQty { get; set; }
        public double TotalQty { get; set; }
        public double PriceCal { get; set; }
        public double Total { get; set; }

    }
}