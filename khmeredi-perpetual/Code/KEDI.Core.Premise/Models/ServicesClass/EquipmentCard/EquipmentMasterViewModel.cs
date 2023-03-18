using System.Collections.Generic;

namespace KEDI.Core.Premise.Models.ServicesClass.EquipmentCard
{
    public class EquipmentMasterViewModel
    {
        public string MfrSerialNo { get; set; }
        public string SerialNumber { get; set; }
        public string PlateNumber { get; set; }
        public string ItemCode { get; set; }
        public StatusEquipmentCard Status { get; set; }
        public string PreviousSN { get; set; }
        public string NewSN { get; set; }
        public string BPCode { get; set; }
        public string BPName { get; set; }
        public string Technician { get; set; }
        public string Territory { get; set; }
        public string TelephoneNo { get; set; }
        public List<EServiceCallViewModel> ServiceCalls { get; set; }
        public EServiceContractViewModel ServiceContract { get; set; }
        public List<ETransactionViewModel> Transactions { get; set; }
        public bool IsError { get; set; }
        public Error Error { get; set; }
    }
    public class Error
    {
        public string Message { get; set; }
    }
}
