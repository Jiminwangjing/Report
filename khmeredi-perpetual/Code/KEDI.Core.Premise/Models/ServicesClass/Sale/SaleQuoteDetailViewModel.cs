using CKBS.Models.Services.Banking;
using CKBS.Models.ServicesClass.GoodsIssue;
using CKBS.Models.ServicesClass.Property;
using KEDI.Core.Premise.Models.Sale;
using KEDI.Core.Premise.Models.ServicesClass.TaxGroup;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace KEDI.Core.Premise.Models.ServicesClass.Sale
{
    public class SaleQuoteDetailViewModel
    {
        public int ID { get; set; }
        public int SQDID { get; set; }
        public int SQID { get; set; }
        public int GUomID { get; set; }
        public int UomID { get; set; }
        public int ItemID { get; set; }
        public int TaxGroupID { get; set; }
        public int CurrencyID { get; set; }
        public string LineID { get; set; }
        public string ItemCode { get; set; }
        public string BarCode { get; set; }
        public string ItemNameKH { get; set; }
        public string ItemNameEN { get; set; }
        public decimal Qty { get; set; }
        public decimal OpenQty { get; set; }
        public List<SelectListItem> UoMs { get; set; }
        public string Currency { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Cost { get; set; }

        public List<SelectListItem> TaxGroupList { get; set; }
        public string UomName { get; set; }
        public decimal Factor { get; set; }
        public decimal TaxRate { get; set; }
        public decimal TaxValue { get; set; }
        public decimal TaxOfFinDisValue { get; set; }
        public decimal TaxDownPaymentValue { get; set; }
        public decimal DisRate { get; set; }
        public decimal DisValue { get; set; }
        public decimal FinDisRate { get; set; } // Final Discount Rate
        public decimal FinDisValue { get; set; } // Final Discount Value
        public decimal Total { get; set; } // Column Name Total After Discount
        public decimal TotalWTax { get; set; }
        public decimal FinTotalValue { get; set; }

        public string Remarks { get; set; }
        public int LoanPartnerID{get;set;}
        public string LoanPartnerName { get; set; }
        
        public List<TaxGroupViewModel> TaxGroups { get; set; }
        public List<UOMSViewModel> UoMsList { get; set; }
        public string TypeDis { get; set; }
        public decimal VatRate { get; set; }
        public decimal VatValue { get; set; }
        public decimal TotalSys { get; set; }
        public string Process { get; set; }
        public DateTime ExpireDate { get; set; }
        public string ItemType { get; set; }
        public bool Delete { get; set; }
        public int BaseUoMID { get; set; }
        public int InvenUoMID { get; set; }
        public SaleCopyType SaleCopyType { get; set; }
        public List<UomPriceList> UomPriceLists { get; set; }
        public string TotalSy2 { get; set; }
        public double TotalWTaxSys { get; set; }
        [NotMapped]
        public Dictionary<string, PropertydetailsViewModel> AddictionProps { get; set; }
        public string Image { get; set; }
    }
    public class SaleQuoteUpdateViewModel
    {
        public SaleQuoteViewModel SaleQuote { get; set; }
        public List<SaleQuoteDetailViewModel> SaleQuoteDetails { get; set; }
        public Currency Currency { get; set; }
    }
    public class SaleQuoteViewModel
    {
        public int SQID { get; set; }
        public int BaseonProjCostANID { get; set; }
        public string IvoiceDisvalue { get; set; }
        public int CusID { get; set; }
        public int BranchID { get; set; }
        public int BaseOnID { get; set; }
        public int WarehouseID { get; set; }
        public int UserID { get; set; }
        public int SaleCurrencyID { get; set; }
        public int CompanyID { get; set; }
        public int DocTypeID { get; set; }
        public int SeriesID { get; set; }
        public int SeriesDID { get; set; }
        public string InvoiceNumber { get; set; }
        public string RefNo { get; set; }
        public string InvoiceNo { get; set; }
        public decimal ExchangeRate { get; set; }
        public DateTime PostingDate { get; set; }
        public DateTime ValidUntilDate { get; set; }
        public DateTime DeliveryDate { get; set; }
        public DateTime DocumentDate { get; set; }
        public bool IncludeVat { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
        public decimal SubTotalBefDis { get; set; }// Subtotal Before Discount
        public decimal SubTotalBefDisSys { get; set; }// Subtotal Before Discount System
        public decimal SubTotalAfterDis { get; set; }// Subtotal After Discount
        public decimal SubTotalAfterDisSys { get; set; }// Subtotal After Discount System
        public decimal FreightAmount { get; set; }
        public decimal FreightAmountSys { get; set; }
        public decimal SubTotal { get; set; }
        public decimal SubTotalSys { get; set; }
        public decimal DisRate { get; set; }
        public decimal DisValue { get; set; }
        public string TypeDis { get; set; }
        public decimal VatRate { get; set; }
        public decimal VatValue { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalAmountSys { get; set; }
        public DateTime ChangeLog { get; set; }
        public int PriceListID { get; set; }
        public int LocalCurID { get; set; }
        public decimal LocalSetRate { get; set; }
        public FreightSaleView FreightSalesView { get; set; }
        public int SaleEmID { get; set; }
        public string SaleEmName { get; set; }
        public string ExchangeRate2 { get; set; }
        public string SubTotalSys2 { get; set; }
        public string TotalAmountSys2 { get; set; }
        public string FreightAmountSys2 { get; set; }
        public string SubTotalBefDisSys2 { get; set; }
        public string SubTotalAfterDisSys2 { get; set; }
        public int RequestedByID { get; set; }
        public int ShippedByID { get; set; }
        public int ReceivedByID { get; set; }

        public string RequestedByName { get; set; }
        public string ShippedByName { get; set; }
        public string ReceivedByName { get; set; }
    }
    public class UomPriceList
    {
        public int UoMID { get; set; }
        public decimal UnitPrice { get; set; }
    }
}