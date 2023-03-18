using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace KEDI.Core.Premise.Models.ServicesClass.SerialBatches.Sales
{
    public class SerialNumberUnselected {
        public decimal TotalAvailableQty { get; set; }
        public List<SerialNumberUnselectedDetial> SerialNumberUnselectedDetials { get; set; }
    }
}
