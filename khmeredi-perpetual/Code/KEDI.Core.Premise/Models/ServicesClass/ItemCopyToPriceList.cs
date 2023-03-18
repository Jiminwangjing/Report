using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.ServicesClass
{
    [Table("tpItemCopyToPriceList",Schema ="dbo")]
    public class ItemCopyToPriceList
    {
        [Key]
        public int ID { get; set; }
        public int ToPriceListID { get; set; }
        public int FromPriceListID { get; set; }
        public List<ItemCopyToPriceListDetail> ItemCopyToPriceListDetail { get; set; }
    }
}
