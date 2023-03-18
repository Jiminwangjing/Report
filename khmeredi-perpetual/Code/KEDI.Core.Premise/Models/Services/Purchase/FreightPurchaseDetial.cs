using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KEDI.Core.Premise.Models.Services.Purchase
{

    [Table("FreightPurchaseDetial")]
    public class FreightPurchaseDetial
    {
        [Key]
        public int ID { get; set; }
        public int TaxGroupID { get; set; }
        public int FreightID { set; get; }
        public int FreightPurchaseID { get; set; } 
        public string Name { get; set; }
        public string TaxGroup { get; set; }
        public decimal TaxRate { get; set; }
        public decimal TotalTaxAmount { get; set; }
        public decimal Amount { get; set; }
        public decimal AmountWithTax { get; set; }
    }

}
