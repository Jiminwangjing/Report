using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.ServicesClass.SerialBatches.Sales
{
    [Table("TmpSerialNumber")]
    public class SerialNumber
    {
        public int Id { get; set; }
        public int OrderDetailID { get; set; }
        public string LineID { get; set; } = DateTime.Now.Ticks.ToString();
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string WhsCode { get; set; }
        public decimal Qty { get; set; }
        public decimal TotalSelected { get; set; }
        public decimal OpenQty { get; set; }
        public string Direction { get; set; }
        [NotMapped]
        public SerialNumberUnselected SerialNumberUnselected { get; set; }
        public SerialNumberSelected SerialNumberSelected { get; set; }
        public int ItemID { get; set; }
        public decimal Cost { get; set; }
        public int BpId { get; set; }
        public int UomID { get; set; }
        public int SaleID { get; set; }
        public decimal BaseQty { get; set; }
        public int WareId { get; set; }
        public string Barcode { get; set; }
    }
}
