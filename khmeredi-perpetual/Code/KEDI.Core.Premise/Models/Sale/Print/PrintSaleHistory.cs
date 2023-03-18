using System.Collections.Generic;

namespace KEDI.Core.Premise.Models.Sale.Print
{
    public class PrintSaleHistory
    {
        // Master
        public int ID { get; set; }
        public string Invoice { get; set; }
        public double ExchangeRate { get; set; }
        public string RequestedBy { get; set; }
        public string ShippedBy { get; set; }
        public string ReceivedBy { get; set; }
        public string EmpName { get; set; }
        public string Name2 { get; set; }
        public string Title { get; set; }
        public double TotalQty { get; set; }
        public double TotalUniprice { get; set; }
        public decimal TotalVat { get; set; }
        public double SubTotal { get; set; }
        public double TotalWTax { get; set; }
        public double TotalBeforDis { get; set; }
        public double Balance_Due_Sys { get; set; }
        public double Balance_Due_Local { get; set; }
        public string PostingDate { get; set; }
        public string DocumentDate { get; set; }
        public string DueDate { get; set; }
        public double Sub_Total { get; set; }
        public double Sub_Total_Sys { get; set; }
        public double DiscountValue { get; set; }
        public double TotalDisValue { get; set; }
        public double TotalOfAmountBeforDis { get; set; }
        public double Disvalue { get; set; }
        public double DiscountRate { get; set; }
        public decimal TaxValue { get; set; }
        public decimal TaxRate { get; set; }
        public double Applied_Amount { get; set; }
        public string TypeDis { get; set; }
        public string CusName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string UserName { get; set; }
        public string LocalCurrency { get; set; }
        public string SysCurrency { get; set; }
        public string CusNo { get; set; }
        public string BaseOn { get; set; }
        public string Branch { get; set; }
        public string RefNo { get; set; }
        public double Amount { get; set; }
        public double TotalAmount { get; set; }
        public double Debit { get; set; }
        public double DebitSys { get; set; }
        public double TotalAmountSys { get; set; }
        public double VatValue { get; set; }
        public double TotalWTaxSys { get; set; }
        public double DiscountValueTotal { get; set; }
        public double AmountafterDis { get; set; }
        public double AmountafterDisTotal { get; set; }
        public decimal VatValues { get; set; }
        public double totalsys { get; set; }
        public string VatNumber { get; set; }

        // from Receipt Information
        public string KhmerDesc { get; set; }
        public string EnglishDesc { get; set; }
        public List<string> PhoneList { get; set; }
        public string Addresskh { get; set; }
        public string AddressEng { get; set; }
        public string Addre { get; set; }
        public string Logo { get; set; }
        public string Tel1 { get; set; }
        public string Tel2 { get; set; }
        public double Exange { get; set; }
        public string CompanyName { get; set; }
        public string Brand { get; set; }
        public string PreFix { get; set; }
        //Detail

        public string ItemCode { get; set; }
        public string ItemNameEn { get; set; }
        public string ItemNameKh { get; set; }
        public double Qty { get; set; }
        public double Price { get; set; }
        public double DiscountValue_Detail { get; set; }
        public double DiscountRate_Detail { get; set; }
        public double Total { get; set; }
        public double Total_Sys { get; set; }
        public string UomName { get; set; }
        public object Remarks { get; set; }
        public string Barcode { get; set; }
        public double LocalSetRate { get; set; }
        public string PriceList { get; set; }
        public string LabelUSA { get; set; }
        public string LabelReal { get; set; }
        public decimal Sub_totalAfterdis { get; set; }
        public decimal TotalDetail { get; set; }
        public string Email { get; set; }
        public string BPBrandName { get; set; }
        public string QSNumber { get; set; }
        public string OrderNumber { get; set; }
        public string DSNumber { get; set; }
        public string StoreName { get; set; }
        public int Paymenterm { get; set; }
        public string Image { get; set; }
        public string ShipTo { get; set; }
        public string SaleEmploye { get; set; }
        public decimal DPMRate { set; get; }
        public decimal DPMValue { set; get; }
        public decimal DownPayment { get; set; }
        public double TotalBancedue { get; set; }
        public decimal SubTotalAfterDis { get; set; }
        public decimal BalanceDue {get;set;}
    }
}