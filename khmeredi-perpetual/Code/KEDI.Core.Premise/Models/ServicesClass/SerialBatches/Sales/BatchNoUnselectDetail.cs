using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace KEDI.Core.Premise.Models.ServicesClass.SerialBatches.Sales
{
    public class BatchNoUnselectDetail : BatchNoDetailMasterClass
    {}
    public class BatchNoDetailMasterClass
    {
        public string LineID { get; set; } = DateTime.Now.Ticks.ToString();
        public string BatchNo { get; set; }
        public decimal AvailableQty { get; set; }
        public decimal SelectedQty { get; set; }
        public decimal UnitCost { get; set; }
        public int BPID { get; set; }
        public decimal OrigialQty { get; set; }
        public DateTime ExpireDate { get; set; }
    }
}
