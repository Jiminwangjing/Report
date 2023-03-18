using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.ServicesClass
{
    public class ServicePriceListCopyItem
    {
        [Key]
        public int ItemID { get; set; }
        public string Code { get; set; }
        public string KhmerName { get; set; }
        public string EnglishName { get; set; }
        public string UoM { get; set; }
        public string Barcode { get; set; }
        public string Process { get; set; }
        [NotMapped]
        public bool Active{get;set;}
    }
}
