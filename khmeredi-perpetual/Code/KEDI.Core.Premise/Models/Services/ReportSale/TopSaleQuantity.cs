
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CKBS.Models.Services.ReportSale
{
    [Table("rp_TopSaleQuantity",Schema ="dbo")]
    public class TopSaleQuantity
    {
        [Key]
        public int ID { get; set; }
        public string Code { get; set; }
        public string KhmerName { get; set; }
        public string EnglishName { get; set; }
        public double Qty { get; set; }
        public string Uom { get; set; }
        public string DateOut { get; set; }
        public string TimeOut { get; set; }
        public string Branch { get; set; }
        public int ItemID { get; set; }
        public int UomID { get; set; }
        [NotMapped]
        public string DateFrom { get; set; }
        [NotMapped]
        public string DateTo { get; set; }

    }
}
