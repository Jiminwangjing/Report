using CKBS.Models.Services.Purchase;
using CKBS.Models.ServicesClass.GoodsIssue;
using CKBS.Models.ServicesClass.Property;
using KEDI.Core.Premise.Models.ServicesClass.TaxGroup;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace KEDI.Core.Premise.Models.ServicesClass.Purchase
{
    public class PurchaseAPDetialViewModel
    {
        public int PurchaseDetailAPID { get; set; }
        public int PurchaseAPID { get; set; }
        public string LineIDUN { get; set; }
        public int ItemID { get; set; }
        public int UomID { get; set; }
        public int LocalCurrencyID { get; set; }
        public string Code { get; set; }
        public string Barcode { get; set; }
        public string ItemName { get; set; }
        public decimal Qty { get; set; }
        public List<SelectListItem> UoMSelect { get; set; }
        public string CurrencyName { get; set; }
        public decimal PurchasPrice { get; set; }
        public List<SelectListItem> TaxGroupSelect { get; set; }
        public int TaxGroupID { get; set; }
        public decimal TaxRate { get; set; }
        public decimal TaxValue { get; set; }
        public decimal TaxOfFinDisValue { get; set; }
        public decimal DiscountRate { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal FinDisRate { get; set; } // Final Discount Rate
        public decimal FinDisValue { get; set; } // Final Discount Value
        public decimal Total { get; set; }
        public decimal TotalWTax { get; set; } //Total With Tax
        public decimal TotalWTaxSys { get; set; }//Total With Tax system
        public decimal FinTotalValue { get; set; }
        public string Remark { get; set; }
        public string TypeDis { get; set; }//Percent ,cash
        public DateTime ExpireDate { get; set; }
        public decimal AlertStock { get; set; }
        public decimal TotalSys { get; set; }
        public bool Delete { get; set; }
        public decimal OpenQty { get; set; }
        public decimal OldQty { get; set; }
        public List<TaxGroupViewModel> TaxGroups { get; set; }
        public List<UOMSViewModel> UoMsList { get; set; }
        public string Process { get; set; }
        public int OrderID { get; set; }
        public int LineID { get; set; }
        public int BaseOnID { get; set; }
        public PurCopyType PurCopyType { get; set; }
        public Dictionary<string, PropertydetailsViewModel> AddictionProps { get; set; }
    }
    public class PurchaseAPViewModel
    {
        public int PurchaseAPID { get; set; }
        public int VendorID { get; set; }
        public int BranchID { get; set; }
        public int PurCurrencyID { get; set; }
        public int SysCurrencyID { get; set; }
        public int WarehouseID { get; set; }
        public int UserID { get; set; }
        public int DocumentTypeID { get; set; }
        public int SeriesID { get; set; }
        public int SeriesDetailID { get; set; }
        public string ReffNo { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime PostingDate { get; set; }
        public DateTime DocumentDate { get; set; }
        public DateTime DeliveryDate { get; set; }
        public decimal SubTotal { get; set; }
        public decimal SubTotalSys { get; set; }
        public decimal SubTotalAfterDis { get; set; }
        public decimal SubTotalAfterDisSys { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal DiscountRate { get; set; }
        public string TypeDis { get; set; }
        public decimal TaxRate { get; set; }
        public decimal TaxValue { get; set; }
        public decimal BalanceDue { get; set; }
        public decimal PurRate { get; set; }
        public decimal BalanceDueSys { get; set; }
        public string Remark { get; set; }
        public decimal DownPayment { get; set; }
        public decimal DownPaymentSys { get; set; }
        public decimal AppliedAmount { get; set; }
        public decimal AppliedAmountSys { get; set; }
        public decimal FrieghtAmount { get; set; }
        public decimal FrieghtAmountSys { get; set; }
        public decimal ReturnAmount { get; set; }
        public decimal AdditionalExpense { get; set; } // no calualte but save in database => textbox
        public string AdditionalNote { get; set; }// no calualte but save in database => textbox
        public string Status { get; set; }
        public decimal LocalSetRate { get; set; }
        public int LocalCurID { get; set; }
        public int CompanyID { get; set; }
        public string Number { get; set; }
        public FreightPurchaseViewModel FreightPurchaseView { get; set; }
        public DateTime DueDate { get; set; }
        public int BaseOnID { get; set; }
    }
    public class PurchaseAPUpdateViewModel
    {
        public PurchaseAPViewModel PurchaseAP { get; set; }
        public List<PurchaseAPDetialViewModel> PurchaseAPDetials { get; set; }

    }
}
