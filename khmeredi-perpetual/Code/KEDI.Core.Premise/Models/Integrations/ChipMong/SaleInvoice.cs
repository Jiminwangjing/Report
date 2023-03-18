namespace KEDI.Core.Premise.Models.Integrations.ChipMong
{
    public class SaleInvoice
    {
        public string mallName { set; get; }
        public string tenantName { set; get; }
        public string date { set; get; }
        public decimal grossSale { set; get; }
        public decimal taxAmount { set; get; }
        public decimal netSale { set; get; }
        public decimal cashAmount { set; get; }
        public decimal creditCardAmount { set; get; }
        public decimal otherAmount { set; get; }
        public int totalCreditCardTransaction { set; get; }
        public int totalTransaction { set; get; }
        public decimal cashAmountUsd { set; get; }
        public decimal cashAmountRiel { set; get; }
        public decimal depositAmountUsd { set; get; }
        public decimal depositAmountRiel { set; get; }
        public decimal exchangeRate { set; get; }
        public string posId { set; get; }
    }
}
