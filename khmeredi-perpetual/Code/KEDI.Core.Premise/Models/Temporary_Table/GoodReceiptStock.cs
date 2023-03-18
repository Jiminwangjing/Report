
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Temporary_Table 
{
    [Table("tpGoodReciptStock",Schema ="dbo")]
    public class GoodReceiptStock
    {
        [Key]
        public int ID { get; set; }
        public int ItemID { get; set; }
        public double Cost { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Column(TypeName = "Date")]
        public DateTime ExpireDate { get; set; }
        public double Qty { get; set; }
        public int UomID { get; set; }
        public int LocalcurrencyID { get; set; }
        public int WarehouseID { get; set; }

    }
}
