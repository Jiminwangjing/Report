using System;

namespace KEDI.Core.Premise.Models.ServicesClass.SerialBatches.Purchases
{
    public class BatchDetialViewModelPurchase
    {
        public string LineID { get; set; } = DateTime.Now.Ticks.ToString();
        public string Batch { get; set; }
        public decimal Qty { get; set; }
        public string BatchAttribute1 { get; set; }
        public string BatchAttribute2 { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public DateTime? MfrDate { get; set; }
        public DateTime? AdmissionDate { get; set; }
        public string Location { get; set; }
        public string Detials { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
