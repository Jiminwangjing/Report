namespace KEDI.Core.Premise.Models.ServicesClass.Report
{
    public class SaleHistoryByCustomers
    {
        public int SaleID { get; set; }
        public string ReceiptNmber { get; set; }
        public string DouType { get; set; }
        public string DateOut { get; set; }
        public string GrandTotal { get; set; }

        public string LineID {get;set;}
        public string ItemName {get;set;}
        public string Uom { get; set; }
        public string  Price {get;set;}
        public double  Qty {get;set;}
        public string  Total {get;set;}
      

        //Summary
        //--Calculate Prop--//
        public double SGrandTotalLCCCal { get; set; }
        public double SGrandTotalSysCal { get; set; }
        public double SVatCal { get; set; }
        public double SDiscountTotalCal { get; set; }
        public double SDiscountItemCal { get; set; }
        //--Display Prop--//
        public string SGrandTotalLCC { get; set; }
        public string SGrandTotalSys { get; set; }
        public string SVat { get; set; }
        public string SDiscountTotal { get; set; }
        public string SDiscountItem { get; set; }
    }
}
