using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace KEDI.Core.Premise.Models.Services.Inventory
{
    public class ServiceCall
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int BPID { get; set; }
        public string BPRefNo { get; set; }
        public string MfrSerialNo { get; set; }
        public string SerialNumber { get; set; }
        public int HandledByID { get; set; }//F
        public int TechnicianID { get; set; }//F
        public int ChannelID { get; set; }//F
        public int ItemID { get; set; }
        public int ItemGroupID { get; set; }
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
        public int CallID { get; set; }
        public string Resolution { get; set; }
        [NotMapped]
        public string BCode { get; set; }
        [NotMapped]
        public string BName { get; set; }
        [NotMapped]
        public string BPhone { get; set; }
        [NotMapped]
        public string ItemCode { get; set; }
        [NotMapped]
        public string ItemName { get; set; }
        [NotMapped]
        public string GName { get; set; }
        [NotMapped]
        public string HandleName { get; set; }
        [NotMapped]
        public string TectnicalName { get; set; }
        public List<ServiceData> ServiceDatas { get; set; }
    }
    public enum CallStatusType
    {
        Open = 1,
        Closed = 2,
        Pending = 3,
    }
    public enum PriorityType
    {
        Low = 1,
        Medium = 2,
        High = 3,
        Critical = 4,
    }
}
