using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.ServicesClass
{
    [Table("tpItemCopyToWHDetail", Schema ="dbo")]
    public class ItemCopyToWHDetail
    {
        [Key]
        public int ID { get; set; }
        public int ItemID { get; set; }
       
       
    }
}
