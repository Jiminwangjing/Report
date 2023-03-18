using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.ServicesClass
{
    [Table("ServiceDiscountItemDetail",Schema ="dbo")]
    public class DiscountItemDetail
    {
        [Key]
        public int ID { get; set; }
        public string Code { get; set; }
        public string KhmerName { get; set; }
        public string EnglishName { get; set; }
        public string Uom { get; set; }
        public double Price { get; set; }
        public string Currency { get; set; }
        public double Discount { get; set; }
        public string TypeDis { get; set; }
       
    }
}
 