using System;

namespace KEDI.Core.Premise.Models.ServicesClass.SerialBatches.SerialNumbersManagement
{
    public class ItemSerialNumberDetialView
    {
        public string LineID { get; set; } = DateTime.Now.Ticks.ToString();
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string WhsName { get; set; }
        public string MfrSerialNo { get; set; }
        public string SerialNumber { get; set; }
        public string SerialNumberOG { get; set; }
        public string LotNumber { get; set; }
        public int SystemNo { get; set; }
        public DateTime? AdmissionDate { get; set; }
        public DateTime? ManufacturingDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public DateTime? MfrWarrantyStart { get; set; }
        public DateTime? MfrWarrantyEnd { get; set; }
        public string Location { get; set; }
        public string Details { get; set; }
    }
}
