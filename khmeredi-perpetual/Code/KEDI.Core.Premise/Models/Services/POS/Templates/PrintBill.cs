using CKBS.Models.Services.Banking;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.POS.service
{
    public class PrintBill
    {
        public int? OrderID { get; set; }
        public string Logo { get; set; }
        public string Logo2 { get; set; }
        public string CompanyName { get; set; }
        public string BranchName { get; set; }
        public string BrandKh { get; set; }
        public string Address { get; set; }
        public string ReceiptTitle1 { get; set; }
        public string ReceiptTitle2 { get; set; }
        public string ReceiptAddress2 { get; set; }
        public string ReceiptEmail { get; set; }
        public string RecWebsite { get; set; }
        public string PowerBy { get; set; }
        public string RecVATTIN { get; set; }
        public string Team2 { get; set; }
        public string Tel1 { get; set; }
        public string Tel2 { get; set; }
        public string CustomerPhone { get; set; }
        public string Table { get; set; }
        public string OrderNo { get; set; }
        public string ReceiptNo { get; set; }
        public string Cashier { get; set; }
        public string DateTimeIn { get; set; }
        public string DateTimeOut { get; set; }
        public string Point { get; set; }
        public string OutStandingPoint { get; set; }
        public int ReceiptCount { set; get; }
        [NotMapped]
        public string Plate { set; get; }
        public List<PaymentMeans> PaymentMean { get; set; }

        //Detail
        public List<PrintviewDetail> PrintviewDetail { get; set; }
        public string Code { get; set; }
        public string Item { get; set; }
        public string Photo { get; set; }
        public string ItemEn { get; set; }
        public string Qty { get; set; }
        public string Uom { get; set; }
        public string Price { get; set; }
        public string DisItem { get; set; }
        public string Amount { get; set; }

        public double LocalCurRate { get; set; }
        //Summary
        public string SubTotal { get; set; }
        public string DisRate { get; set; }
        public string DisValue { get; set; }
        public string TypeDis { get; set; }
        public string GrandTotal { get; set; }
        public string GrandTotalSys { get; set; }
        public string VatRate { get; set; }
        public string VatValue { get; set; }
        public string LabelUSA { get; set; }
        public string LabelReal { get; set; }
        public double AmountFrieght { get; set; }
        public string RemakrDis { get; set; }
        //Receive
        public string Received { get; set; }
        public string Change { get; set; }

        public string ChangeSys { get; set; }

        //Fother
        public string DescKh { get; set; }
        public string DescEn { get; set; }

        public string ExchangeRate { get; set; }
        public string PaymentMeans { get; set; }

        public string Printer { get; set; }
        public string Print { get; set; }
        public string ItemDesc { get; set; }
        public string CustomerInfo { get; set; }
        public string Team { get; set; }
        public string ItemType { get; set; }
        public string Remark { get; set; }
        public string TotalQty { get; set; }
        public string VatNumber { get; set; }
        public string BarCode { get; set; }
        public string TaxValue { get; set; }
        public string TaxRate { get; set; }
        public string TaxTotal { get; set; }
        public string Freights { get; set; }
        public string LinePosition { get; set; }
        public string GrandTotalCurrenciesDisplay { get; set; }
        public string ChangeCurrenciesDisplay { get; set; }
        public string PlateNumber { set; get; }
        public string Brand { get; set; }
        public string Condition { get; set; }
        public string Type { get; set; }
        public string Power { get; set; }
        public string Colors { get; set; }
        public string Year { get; set; }
        public string Frame { get; set; }
        public string Engine { get; set; }
        public bool Reprint { get; set; } = false;
        public string DisValueDetail { get; set; }
        public string PreFix { get; set; }
        public string PreName { get; set; }
        public string Details { get; set; }
    }
    public class PointTemplate
    {
        public decimal Point { get; set; }
        public decimal OutStandingPoint { get; set; }
    }
    public class PrintviewDetail
    {
        public int ID { get; set; }
        public string Description { get; set; }
        public double UnitPice { get; set; }
        public string Currency { get; set; }
        public string Code { get; set; }
        public string Item { get; set; }
        public string Photo { get; set; }
        public string ItemEn { get; set; }
        public double Qty { get; set; }
        public string Uom { get; set; }
        public double Price { get; set; }
        public double DisItem { get; set; }
        public double Amount { get; set; }

        public double LocalCurRate { get; set; }
    }

}
