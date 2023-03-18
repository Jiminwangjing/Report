using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.ServicesClass
{
    [Table("ServicePointDetail",Schema ="dbo")]
    public class ServicePointDetail
    {
        [Key]
        public int ID { get; set; }
        public int ItemID { get; set; }
        public int Qty { get; set; }
        public string Code { get; set; }
        public string KhmerName { get; set; }
        public string EnglishName { get; set; }
        public string Barcode { get; set; }
        public double UnitPrice { get; set; }
        public int CurrencyID { get; set; }
        public string CurrencyName { get; set; }
        public int UomID { get; set; }
        public string UomName { get; set; }


    }
}
