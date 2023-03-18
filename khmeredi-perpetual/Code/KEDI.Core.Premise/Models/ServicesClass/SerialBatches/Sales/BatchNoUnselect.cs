using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace KEDI.Core.Premise.Models.ServicesClass.SerialBatches.Sales
{
    public class BatchNoUnselect
    {
        public decimal TotalAvailableQty { get; set; }
        public List<BatchNoUnselectDetail> BatchNoUnselectDetails { get; set; }
    }
}
