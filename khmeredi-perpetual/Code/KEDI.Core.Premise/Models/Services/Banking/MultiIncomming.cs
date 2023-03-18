using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.Services.Banking
{
    [Table("MultPayIncomming")]
    public class MultiIncomming
    {
        [Key]
        public int ID { get; set; }
        public int PaymentMeanID { get; set; }
        public decimal SCRate { get; set; }
 [NotMapped]
 public int LineID { get; set; }
        public int IncomingPaymentID { get; set; }
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
