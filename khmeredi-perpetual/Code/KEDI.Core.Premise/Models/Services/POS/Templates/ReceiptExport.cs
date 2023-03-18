using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CKBS.Models.Services.LoyaltyProgram.BuyXAmountGetXDiscount;
using CKBS.Models.Services.POS;
using CKBS.Models.Services.POS.KVMS;

namespace KEDI.Core.Premise
{
    public class ReceiptExport
    {
        public DateTime PostingDate { get; set; }
        public string OrderNo { get; set; }
        public int TableID { get; set; }
        public string ReceiptNo { get; set; } = "0";
        public string QueueNo { get; set; }
        public DateTime DateIn { get; set; }
        public DateTime DateOut { get; set; }
        public string TimeIn { get; set; }
        public string TimeOut { get; set; }
        public int WaiterID { get; set; }
        public string Username { get; set; } //tbUserAccount
        public string UserDiscountID { get; set; }//tbUserAccount
        public string CustomerCode { get; set; }//tblBusinessPartner
        public int CustomerCount { get; set; }
        public string PriceListName { get; set; }//tbPriceList-Name
        public string LocalCurrency { get; set; }//tbCurrency-Symbol
        public string SysCurrency   { get; set; }//tbCurrency-Symbol
        public double ExchangeRate { get; set; }
        public string WarehouseCode { get; set; }//tbWarehouse-code
        public string BranchName { get; set; }//tbBranch-Name
        public string CompanyID { get; set; }//tbCompany-Name
        public double Sub_Total { get; set; }
        public double DiscountRate { get; set; }
        public double DiscountValue { get; set; }
        public string TypeDis { get; set; } = "Percent";
        public double TaxRate { get; set; }
        public double TaxValue { get; set; }
        public decimal OtherPaymentGrandTotal { get; set; }
        public decimal OpenOtherPaymentGrandTotal { get; set; }
        public double GrandTotal { get; set; }
        public double GrandTotal_Sys { get; set; }
        public double Tip { get; set; } = 0;
        public double Received { get; set; }
        public double Change { get; set; }
        public string CurrencyDisplay { get; set; }
        public double DisplayRate { get; set; }
        public double GrandTotal_Display { get; set; }
        public double Change_Display { get; set; }
        public int PaymentMeansID { get; set; }
        public char CheckBill { get; set; }
        public bool Cancel { get; set; } = false;
        public bool Delete { get; set; } = false;
        public bool Return { get; set; } = false;
        public string PLCurrencyID { get; set; } //tbCurrent-Symbol
        public double PLRate { get; set; }
        public string SeriesCode { get; set; }//tbSeries-Name
        public string SeriesNumber { get; set; }
        public decimal AppliedAmount { get; set; }
        public double LocalSetRate { get; set; }
        public int Male { get; set; }
        public int Female { get; set; }
        public int Children { get; set; }
        public StatusReceipt Status { get; set; }
        public decimal AmountFreight { get; set; }
        public int VehicleID { get; set; }
        public TaxOptions TaxOption { set; get; }//tbTaxGroup - Code
        public int PromoCodeID { get; set; }
        public double PromoCodeDiscRate { get; set; }
        public double PromoCodeDiscValue { get; set; }
        public string RemarkDiscount { get; set; } = "NULL";
        public int BuyXAmountGetXDisID { get; set; }
        public decimal BuyXAmGetXDisRate { get; set; }
        public decimal BuyXAmGetXDisValue { get; set; }
        public decimal CardMemberDiscountRate { get; set; }
        public decimal CardMemberDiscountValue { get; set; }
        public double ReceivedPoint { get; set; }
        public double CumulativePoint { get; set; }
        public TypeDiscountBuyXAmountGetXDiscount BuyXAmGetXDisType { get; set; } // Rate=1, Value=2
        public string TaxGroupID { get; set; }//TaxGroup-Code
        //public List<DisplayPayCurrencyModel> GrandTotalCurrencies { get; set; }
        public string GrandTotalCurrenciesDisplay { get; set; }
       // public List<DisplayPayCurrencyModel> ChangeCurrencies { get; set; }
        public string ChangeCurrenciesDisplay { get; set; }
        public string GrandTotalOtherCurrenciesDisplay { get; set; }
        // public List<DisplayPayCurrencyModel> GrandTotalOtherCurrencies { get; set; }
        // public List<DisplayPayCurrencyModel> DisplayPayOtherCurrency { get; set; }
        public PaymentType PaymentType { get; set; }
        public decimal BalanceReturn { get; set; }
        public decimal BalancePay { get; set; }
        public decimal BalanceToPay { get; set; }
    }
}