using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KEDI.Core.Premise.Models.Services.POS
{
    [Table("CustomerTips")]
    public class CustomerTips
    {
        [Key]
        public int ID { get; set; }

        public int ReceiptID { get; set; }
        public decimal Amount { get; set; }
        public int AltCurrencyID { get; set; }
        public string AltCurrency { get; set; }
        public decimal AltRate { get; set; }
        [Column(TypeName = "decimal(36,18)")]
        public decimal SCRate { get; set; }
        public decimal LCRate { get; set; }
        public string BaseCurrency { get; set; }
        public int BaseCurrencyID { get; set; }


    }
}
