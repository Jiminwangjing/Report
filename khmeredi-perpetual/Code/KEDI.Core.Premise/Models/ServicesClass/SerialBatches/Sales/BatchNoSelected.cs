using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace KEDI.Core.Premise.Models.ServicesClass.SerialBatches.Sales
{
    [Table("TmpBatchNoSelected")]
    public class BatchNoSelected
    {
        public int Id { get; set; }
        public int BatchNoID { get; set; }
        public decimal TotalSelected { get; set; }
        public List<BatchNoSelectedDetail> BatchNoSelectedDetails { get; set; }
    }
}
