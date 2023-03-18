using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KEDI.Core.Premise.Models.Services.POS.SecondScreen
{
    public class InvoiceDisplay
    {
        public string OrderNo { get; set; }     
        public string BaseCurrency { set; get; }
        public string AltCurrency { set; get; }
        public double ExchangeRate { set; get; }
        public decimal DiscountRate { set; get; }
        public decimal DiscountValue { set; get; }
        public decimal TaxValue { set; get; }
        public decimal Freight { set; get; }
        public decimal SubTotal { set; get; }
        public decimal GrandTotal { set; get; }
        public decimal Recevied { set; get; }
        public decimal Changed { set; get; }
        public List<LineItemDisplay> LineItems { set; get; }

    }
}
