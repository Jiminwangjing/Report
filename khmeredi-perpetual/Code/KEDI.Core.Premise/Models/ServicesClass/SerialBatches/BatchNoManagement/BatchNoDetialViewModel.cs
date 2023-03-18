using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.ServicesClass.SerialBatches.BatchNoManagement
{
    public class BatchNoDetialViewModel
    {
        public string LineID { get; set; } = DateTime.Now.Ticks.ToString();
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string WhsName { get; set; }
        public string Batch { get; set; }
        public string BatchOG { get; set; }
        public string BatchAttribute1 { get; set; }
        public string BatchAttribute2 { get; set; }
        public DateTime? AdmissionDate { get; set; }
        public DateTime? ManufacturingDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string Location { get; set; }
        public string Details { get; set; }
    }
}
