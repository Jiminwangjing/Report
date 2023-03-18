using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.HumanResources
{
    [Table("tbCashDicount", Schema = "dbo")]

    public class CashDiscount
    {
        [Key]
        public int ID { get; set; }
        public string CodeName { get; set; }
        public string Name { get; set; }
        public bool ByDate { get; set; }
        public bool Freight { get; set; }
        public int Day { get; set; }
        public int Month { get; set; }
        public float Discount { get; set; }
        public float CashDiscountDay { get; set; }
        public float DiscountPercent { get; set; }
    }
}
