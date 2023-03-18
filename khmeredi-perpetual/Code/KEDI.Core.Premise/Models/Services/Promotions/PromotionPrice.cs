using CKBS.Models.Services.Banking;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Promotions
{
    [Table("tbPromotionPrice", Schema = "dbo")]
    public class PromotionPrice
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string Name { get; set; }
        public double Amount { get; set; }
        public int CurrencyID { get; set; }
        public float Discount { get; set; }
        public string TypeDis { get; set; }
        public bool Delete { get; set; }

        [ForeignKey("CurrencyID")]
        public  Currency Currency { get; set; }
    }
}
