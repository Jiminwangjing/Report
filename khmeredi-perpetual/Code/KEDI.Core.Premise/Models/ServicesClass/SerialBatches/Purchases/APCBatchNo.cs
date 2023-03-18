using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.ServicesClass.SerialBatches.Purchases
{
    public class APCBatchNo
    {
        public string LineID { get; set; } = DateTime.Now.Ticks.ToString();
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string WhsCode { get; set; }
        public string WhsName { get; set; }
        public decimal Qty { get; set; }
        public decimal TotalNeeded { get; set; }
        public decimal TotalSelected { get; set; }
        public decimal TotalBatches { get; set; }
        public string Direction { get; set; }
        public APCBatchNoUnselect APCBatchNoUnselect { get; set; }
        public APCBatchNoSelected APCBatchNoSelected { get; set; }
        public int ItemID { get; set; }
        public int BpId { get; set; }
        public int UomID { get; set; }
        public decimal Cost { get; set; }
    }
    public class APCBatchNoUnselect
    {
        public decimal TotalAvailableQty { get; set; }
        public List<APCBatchNoUnselectDetial> APCBatchNoUnselectDetials { get; set; }
    }
    public class APCBatchNoUnselectDetial
    {
        public string LineID { get; set; } = DateTime.Now.Ticks.ToString();
        public string BatchNo { get; set; }
        public decimal AvailableQty { get; set; }
        public decimal SelectedQty { get; set; } 
        public decimal UnitCost { get; set; }
        public int BPID { get; set; }
        public decimal OrigialQty { get; set; }
    }
    public class APCBatchNoSelected
    {
        public decimal TotalSelected { get; set; }
        public List<APCBatchNoSelectedDetail> APCBatchNoSelectedDetails { get; set; }
    }
    public class APCBatchNoSelectedDetail : APCBatchNoUnselectDetial
    {
    }
}
