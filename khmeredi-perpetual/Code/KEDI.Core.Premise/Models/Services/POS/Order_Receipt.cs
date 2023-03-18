using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.POS
{
    [Table("tbOrder_Receipt", Schema="dbo")]
    public class Order_Receipt
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int BranchID { get; set; }
        public string ReceiptID { get; set; }
        public DateTime DateTime { get; set; }
        

    }
}
