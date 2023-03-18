using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KEDI.Core.Premise.Models.Sale
{
    [Table("FreightSaleDetail")]
    public class FreightSaleDetail
    {
        [Key]
        public int ID { get; set; }
        public int TaxGroupID { get; set; }
        public int FreightID { set; get; }
        public int FreightSaleID { get; set; }
        public string Name { get; set; }
        public string TaxGroup { get; set; }
        public decimal TaxRate { get; set; }
        public decimal TotalTaxAmount { get; set; }
        public decimal Amount { get; set; }
        public decimal AmountWithTax { get; set; }
    }
}
