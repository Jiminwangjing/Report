using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using KEDI.Core.Premise.Models.Services.POS;

namespace KEDI.Core.Premise
{
    public class PaymentMeanExport
    {
        public string SerialCode { get; set; }
        public string SeriesNumber { get; set; }
        public string PaymentMeanType { get; set; }
        public string AltCurrencyName { get; set; }
        public string AltCurrency { get; set; }
        public int AltRate { get; set; }
        public string PLCurrencyName { get; set; }
        public string PLCurrency { get; set; }
        public double PLRate { get; set; }
        [Column(TypeName = "decimal(36,18)")]
        public decimal Amount { get; set; }
        [Column(TypeName = "decimal(36,18)")]
        public decimal OpenAmount { get; set; }
        [Column(TypeName = "decimal(36,18)")]
        public decimal Total { get; set; }
        [Column(TypeName = "decimal(36,18)")]
        public decimal SCRate { get; set; }
        [Column(TypeName = "decimal(36,18)")]
        public decimal LCRate { get; set; }

        public bool ReturnStatus { get; set; }
        public PaymentMeanType Type { get; set; }
        public bool Exceed { get; set; }
    }
}