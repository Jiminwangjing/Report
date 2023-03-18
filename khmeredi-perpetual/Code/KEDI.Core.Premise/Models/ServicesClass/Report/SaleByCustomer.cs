namespace KEDI.Core.Premise.Models.ServicesClass.Report
{
    public class SaleByCustomer
    {
        public string MCustomer { get; set; }
        public double MCusTotal { get; set; }
        public string MReceiptNo { get; set; }
        public string MSubTotal { get; set; }
        // detail
        public int ID { get; set; }
        public int DetailID { get; set; }
        public string ItemCode { get; set; }
        public string KhmerName { get; set; }
        public double Qty { get; set; }
        public string Uom { get; set; }
        public string UnitPrice { get; set; }
        public string DisItem { get; set; }
        public string Total { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public string DateIn { get; set; }
        public string DateOut { get; set; }
        public string SCount { get; set; }
        public string SDiscountItem { get; set; }
        public string SDiscountTotal { get; set; }
        public string SVat { get; set; }
        public string SGrandTotalSys { get; set; }
        public string SGrandTotal { get; set; }

        // prop ref
        public int UserId { get; set; }
        public int UomId { get; set; }
        public int CusId { get; set; }
        public int CurLCId { get; set; }
        public int CurSysId { get; set; }
        public int CurPLId { get; set; }
        public double UnitPriceCal { get; set; }
        public double DisItemCal { get; set; }
        public double TotalCal { get; set; }
        public double DiscountItem { get; set; }
        public double DiscountTotal { get; set; }
        public double TaxValue { get; set; }
        public int ItemId { get; set; }
        public double GrandTotal { get; set; }
        public double Rate { get; set; }
        public double RateLocal { get; set; }
        public string MReceiptGroup { get; set; }
        public string MBranch { get; set; }
        public int BranchId{ get; set; }
        public string PLCurrency { get; set; }
    }
}
