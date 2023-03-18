using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.ServicesClass
{
    [Table("tpItemCopyToWH", Schema = "dbo")]
    public class ItemCopyToWH
    {
        [Key]
        public int ID { get; set; }

        public int ToWHID { get; set; }
        public List<ItemCopyToWHDetail> ItemCopyToWHDetail { get; set; }
    }
}
