using CKBS.Models.Services.Purchase;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.Services.Purchase
{
    [Table("FreightPurchase")]
    public class FreightPurchase
    {
        [Key]
        public int ID { set; get; }
        public int PurID { set; get; }
        public PurCopyType PurType { get; set; }
        public decimal ExpenceAmount{ set; get; }
        public decimal OpenExpenceAmount { set; get; }
        public decimal TaxSumValue { set; get; }
        public IEnumerable<FreightPurchaseDetial> FreightPurchaseDetials { get; set; } 
    }
}
