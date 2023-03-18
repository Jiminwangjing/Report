using System;

namespace KEDI.Core.Premise.Models.ServicesClass.EquipmentCard
{
    public class EServiceCallViewModel
    {
        public string LineID { get; set; } = DateTime.Now.Ticks.ToString();
        public string CallName { get; set; }
        public string Creation { get; set; }
        public string Subject { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string CustomerName { get; set; }
        public string Technician { get; set; }
        //public string Status { get; set; }
    }
}
