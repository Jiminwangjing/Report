using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.Sale
{
    [Table("FreightSale")]
    public class FreightSale
    {
        [Key]
        public int ID { set; get; }
        public int SaleID { set; get; }
        public SaleCopyType SaleType { get; set; }
        public decimal AmountReven { set; get; }
        public decimal OpenAmountReven { set; get; }
        public decimal TaxSumValue { set; get; }
        public IEnumerable<FreightSaleDetail> FreightSaleDetails { get; set; }
    }
}
