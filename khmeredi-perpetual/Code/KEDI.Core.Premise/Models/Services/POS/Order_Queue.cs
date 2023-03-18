using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.POS
{
    [Table("tbOrder_Queue", Schema="dbo")]
    public class Order_Queue
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int BranchID { get; set; }
        public string OrderNo { get; set; }
        public DateTime DateTime { get; set; }

    }
}
