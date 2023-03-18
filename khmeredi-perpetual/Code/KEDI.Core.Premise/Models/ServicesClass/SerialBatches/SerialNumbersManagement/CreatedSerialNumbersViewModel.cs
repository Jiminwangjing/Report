using CKBS.Models.Services.Administrator.Inventory;
using System;

namespace KEDI.Core.Premise.Models.ServicesClass.SerialBatches.SerialNumbersManagement
{
    public class CreatedSerialNumbersViewModel
    {
        public int WarehouseID { get; set; }
        public int RefWarehouseID { get; set; }
        public TransTypeWD TransType { get; set; }
        public string LineID { get; set; } = DateTime.Now.Ticks.ToString();
        public string MfrSerialNo { get; set; }
        public string SerialNumber { get; set; }
        public string LotNumber { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public DateTime? MfrDate { get; set; }
        public DateTime? AdmissionDate { get; set; }
        public DateTime? MfrWarrantyStart { get; set; }
        public DateTime? MfrWarrantyEnd { get; set; }
        public string Location { get; set; }
        public string Details { get; set; }
        public decimal UnitCost { get; set; }
        public string SerialNumberOG { get; set; }
    }
}
