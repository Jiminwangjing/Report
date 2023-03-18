using System;

namespace KEDI.Core.Premise.Models.ServicesClass.EquipmentCard
{
    public class EServiceContractViewModel
    {
        public string LineID { get; set; } = DateTime.Now.Ticks.ToString();
        public string ContractName { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string TerminationDate { get; set; }
        public string ServiceType { get; set; }
    }
}
