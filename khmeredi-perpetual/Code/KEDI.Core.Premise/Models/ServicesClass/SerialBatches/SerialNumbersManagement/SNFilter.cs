using System;

namespace KEDI.Core.Premise.Models.ServicesClass.SerialBatches.SerialNumbersManagement
{
    public class SNFilter
    {
        public Operation Operations { get; set; } 
        public string ItemNo { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public bool GoodsReceiptPO { get; set; }
        public bool APInvoices { get; set; }
        public bool APCreditMemos { get; set; }
        public bool Deliveries { get; set; }
        public bool ReturnDeliveries { get; set; }
        public bool ARInvoices { get; set; }
        public bool ARCreditMemos { get; set; }
        public bool GoodsReciept { get; set; }
        public bool GoodsIssue { get; set; }
    }
    public enum Operation
    {
        Update = 1,
        //Completed = 2
    }
}
