using KEDI.Core.Premise.Models.Services.Inventory;
using System;

namespace KEDI.Core.Premise.Models.ServicesClass.ServiceCall
{
    public class ServiceCallMasterViewModel
    {
        public int ID { get; set; }
        public int BPID { get; set; }
        public string BPCode { get; set; }
        public string BPName { get; set; }
        public string TelephoneNo { get; set; }
        public string BPRefNo { get; set; }
        public string MfrSerialNo { get; set; }
        public string SerialNumber { get; set; }
        public int ItemID { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public int ItemGroupID { get; set; }
        public string ItemGroupName { get; set; }
        public int SeriesID { get; set; }
        public int SeriesDID { get; set; }
        public int DocTypeID { get; set; }
        public string Number { get; set; }
        public CallStatusType CallStatus { get; set; }
        public PriorityType Priority { get; set; }
        public DateTime CreatedOnDate { get; set; }
        public string CreatedOnTime { get; set; }
        public DateTime? ClosedOnDate { get; set; }
        public string ClosedOnTime { get; set; }
        public string ContractNo { get; set; }
        public DateTime? EndDate { get; set; }
        public string Subject { get; set; }
    }
}
