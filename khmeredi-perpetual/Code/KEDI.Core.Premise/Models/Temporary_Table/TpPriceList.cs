using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Temporary_Table
{
    [Table("tpPriceList", Schema = "dbo")]
    public class TpPriceList
    {
        [Key]
        public int ID { get; set; }
        public int currencyID { get; set; }
        public int PirceListID { get; set; }
        
    }
}
