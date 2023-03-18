using CKBS.Models.Services.Administrator.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.ServicesClass.SerialBatches.BatchNoManagement
{
    public class CreatedBatchNoViewModel
    {
        public int WarehouseID { get; set; }
        public int RefWarehouseID { get; set; }
        public TransTypeWD TransType { get; set; }
        public string LineID { get; set; } = DateTime.Now.Ticks.ToString();
        public string Batch { get; set; }
        public string BatchAttribute1 { get; set; }
        public string BatchAttribute2 { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public DateTime? MfrDate { get; set; }
        public DateTime? AdmissionDate { get; set; }
        public string Location { get; set; }
        public string Details { get; set; }
        public decimal UnitCost { get; set; }
        public string BatchOG { get; set; }
    }
}
