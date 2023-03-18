using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace KEDI.Core.Premise.Models.Services.Banking
{
    public class MultiIncomingPaymentOrder
    {
        [Key]
        public int ID { get; set; }
        public int PaymentMeanID { get; set; }
        public decimal SCRate { get; set; }
        [NotMapped]
        public int LineID { get; set; }
        public int IncomingPaymentOrderID { get; set; }
        public decimal OpenAmount { get; set; }
        public decimal Amount { get; set; }
        public decimal AmmountSys { get; set; }
        [NotMapped]
        public decimal ExchangeRate { get; set; }
        public int GLAccID { get; set; }
        public int CurrID { get; set; }
        [NotMapped]
        public string PMName { get; set; }
        [NotMapped]
        public List<SelectListItem> Currency { get; set; }
    }
}
