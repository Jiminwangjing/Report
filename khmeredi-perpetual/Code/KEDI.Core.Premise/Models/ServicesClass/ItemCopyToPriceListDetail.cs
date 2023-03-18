using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.ServicesClass
{
    [Table("tpItemCopyToPriceListDetail",Schema ="dbo")]
    public class ItemCopyToPriceListDetail
    {
        [Key]
        public int ID { get; set; }
        public int ItemCopyToPriceListID { get; set; }
        public int ItemID { get; set; }
       public string Barcode { get; set; }
       
       
    }
}
