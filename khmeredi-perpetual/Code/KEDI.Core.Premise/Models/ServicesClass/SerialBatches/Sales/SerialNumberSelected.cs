using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace KEDI.Core.Premise.Models.ServicesClass.SerialBatches.Sales
{
    [Table("TmpSerialNumberSelected")]
    public class SerialNumberSelected {
        public int Id { get; set; }
        public int SerialNumberID { get; set; }
        public decimal TotalSelected { get; set; }
        public List<SerialNumberSelectedDetail> SerialNumberSelectedDetails { get; set; }
    }
}
