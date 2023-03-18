using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.ServicesClass.Report
{
    public class DevSummarySale
    {
        //======================
        public string LineID { get; set; } = DateTime.Now.Ticks.ToString();
        public string GrandTotalBrand { get; set; }
        public string EmpCode { get; set; }
        public string EmpName { get; set; }
        public string BranchName { get; set; }
        public int BranchID { get; set; }
        public string ReceiptNo { get; set; }
        public long ReceiptNmber { get; set; }
        //==========pro contract billing=======
        public int Expires { get; set; }
        public List<SelectListItem> Status { get; set; }
        public List<SelectListItem> ConfrimRenew { get; set; }
        public List<SelectListItem> Payment { get; set; }
        public DateTime NewContractStartDate { get; set; }
        public DateTime NewContractEndDate { get; set; }
        public DateTime NextOpenRenewalDate { get; set; }
        public DateTime Renewalstartdate { get; set; }
        public DateTime Renewalenddate { get; set; }
        public DateTime TerminateDate { get; set; }
        public List<SelectListItem> ContractType { get; set; }
        public string ContractName { get; set; }
        public string SetupContractName { get; set; }
        public int Activities { get; set; }
        public decimal EstimateSupportCost { get; set; }
        public string Remark { get; set; }
        public string Attachement { get; set; }
        //==========end pro contract billing=======
        public string DouType { get; set; }
        public string DateOut { get; set; }
        public string DisRemark { get; set; }
        public string Currency { get; set; }
        public string Reasons { get; set; }
        public string GrandTotal { get; set; }
        public int ReceiptID { get; set; }
        public string TimeOut { get; set; }
        public string DiscountItem { get; set; }
        public Decimal AmmountFreightss { get; set; }
        public double Distotalin { get; set; }
        public double DisItem { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public string SCount { get; set; }
        public string SDiscountItem { get; set; }
        public string SDiscountTotal { get; set; }
        public string SVat { get; set; }
        public string SGrandTotalSys { get; set; }
        public string SGrandTotal { get; set; }
        public decimal TotalDiscountItem { get; set; }
        public double DiscountTotal { get; set; }
        public double Vat { get; set; }
        public double GrandTotalSys { get; set; }
        public double UnitPrice { get; set; }
        public double Total { get; set; }
        public double MGrandTotal { get; set; }
        public string RefNo { get; set; }
        public string AmountFreight { get; set; }
        public string ItemCode { get; set; }
        public string ItemNameKhmer { get; set; }
        public string ItemNameEng { get; set; }
        public double Qty { get; set; }
        public string Uom { get; set; }
        public string ShipBy { get; set; }
        public int ItemID { get; set; }
        public double TotalVat { get; set; }
        public double TotalGrandTotal { get; set; }
        public double TotalGrandTotalSys { get; set; }
        public string Process { get; set; }
        public string InvoiceNo { get; set; }
    }
    public class TopSaleQtyReport
    {
        public int ItemID { get; set; }
        public int CurrencyId { get; set; }
        public string GroupName { get; set; }
        public string Group1 { get; set; }
        public string Group2 { get; set; }
        public string Group3 { get; set; }
        public string Barcode { get; set; }
        public string Code { get; set; }
        public string KhmerName { get; set; }
        public double Qty { get; set; }
        public double ReturnQty { get; set; }
        public string Uom { get; set; }
        public string SysCurrency { get; set; }
        public string Currency { get; set; }
        public string LocalCurrency { get; set; }
        public string Price { get; set; }
        public double TotalQty { get; set; }
        public string Total { get; set; }
        public string SubTotal { get; set; }
        public string DateTo { get; set; }
        public string DateFrom { get; set; }
        public string SDiscountItem { get; set; }
        public string SDiscountTotal { get; set; }
        public string SVat { get; set; }
        public double SGrandTotalSys { get; set; }
        public double SGrandTotal { get; set; }
        public double SumGrandTotalSys { get; set; }
        public int BrandID { get; set; }

        /// Calculation props
        public double PriceCal { get; set; }
        public double SubTotalCal { get; set; }
        public double TotalCal { get; set; }
        public double TotalQtyCal { get; set; }
        public double SDiscountItemCal { get; set; }
        public double SDiscountTotalCal { get; set; }
        public double SVatCal { get; set; }
        public string SGrandTotalSysCal { get; set; }
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
        public double TaxValue { get; set; }
        public double SDiscountItemCallocal { set; get; }
        public double ExchangeRate { set; get; }

    }

}
