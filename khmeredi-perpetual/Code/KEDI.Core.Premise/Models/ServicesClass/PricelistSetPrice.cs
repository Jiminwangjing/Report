using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CKBS.Models.ServicesClass
{
    [Table("ServicePricelistSetPrice", Schema = "dbo")]
    public class PricelistSetPrice
    {
        [Key]
        public int ID { get; set; }
        public string Code { get; set; }
        public string KhmerName { get; set; }
        public string EnglishName { get; set; }
        public string Uom { get; set; }
        public double Cost { get; set; }
        public double Makup { get; set; }
        public double Price { get; set; }
        public string Currency { get; set; }
        public double Discount { get; set; }
        public string TypeDis { get; set; }
        public string Process { get; set; }
       public string SysCurrency { get; set; }
        public string Barcode { get; set; }
       
    }
}
