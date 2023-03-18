using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace KEDI.Core.Premise.Models.ClientApi.Sale
{
    public class SaleReceipt
    {
        public string TxId { set; get; }
        public string ReceiptNo { get; set; }
        public string Currency { get; set; }
        public string PostingDate { get; set; }
        public string DueDate { get; set; }
        public string TaxDate { get; set; }
        public string SeriesName { get; set; }
        public string SeriesPrefix { get; set; }
        public double DiscountRate { get; set; }
        public double DiscountValue { get; set; }
        public double TaxRate { get; set; }
        public double TaxValue { get; set; }
        public string Comments { get; set; }
        public string UserId { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Grandtotal { get; set; }
        public bool Cancelled { set; get; }
        public IEnumerable<SaleReceiptDetail> SaleReceiptDetails { get; set; }
    }

    public class SaleReceiptDetail
    {
        public string LineID { set; get; }
        public string ItemCode { get; set; }
        public string UoM { get; set; }
        public decimal Price { get; set; }
        public decimal Qty { get; set; }
        public decimal Total { get; set; }
        public decimal TaxRate { set; get; }
        public decimal Taxvalue { set; get; }
        public double DiscountRate { set; get; }
        public decimal DiscountValue { set; get; }
    }
}
