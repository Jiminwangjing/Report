using System;

namespace KEDI.Core.Premise.Models.ServicesClass.EquipmentCard
{
    public class ETransactionViewModel
    {
        public string LineID { get; set; } = DateTime.Now.Ticks.ToString();
        public string Source { get; set; }
        public string DocumentNo { get; set; }
        public string Date { get; set; }
        public string WhsName { get; set; }
        public string GLAccBPCode { get; set; }
        public string GLAccBPName { get; set; }
        public string Direction { get; set; }
    }
}
