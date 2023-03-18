using System.ComponentModel.DataAnnotations.Schema;

namespace KEDI.Core.Premise.Models.ServicesClass.SerialBatches.Sales
{
    [Table("TmpSerialNumberSelectedDetail")]
    public class SerialNumberSelectedDetail : SerialNumberDetialMasterClass {
        public int Id { get; set; }
        public int SerialNumberSelectedID { get; set; }
    }

    public class SerialNumberBarcodeFind
    {
        public SerialNumberSelectedDetail SerialNumberSelectedDetail { get; set; }
        public SerialNumber SerialNumber { get; set; }
        public bool IsOutOfStock { get; set; }
    }
}
