using System;

namespace KEDI.Core.Premise.Models.ServicesClass.SerialBatches.Purchases
{
    public class SerialDetialViewModelPurchase
    {
        public string LineID { get; set; }
        public string MfrSerialNo { get; set; }
        public string SerialNumber { get; set; }
        public string PlateNumber { get; set; }
        public string LotNumber { get; set; }
        public string Color { get; set; }
        public string Brand { get; set; }
        public string Condition { get; set; }
        public string Type { get; set; }
        public string Power { get; set; }
        public string Year { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public DateTime? MfrDate { get; set; }
        public DateTime? AdmissionDate { get; set; }
        public DateTime? MfrWarrantyStart { get; set; }
        public DateTime? MfrWarrantyEnd { get; set; }
        public string Location { get; set; }
        public string Detials { get; set; }
        public string LineMID { get; set; }
    }
}
