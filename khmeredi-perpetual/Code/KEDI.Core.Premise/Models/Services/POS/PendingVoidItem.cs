using CKBS.Models.Services.Banking;
using CKBS.Models.Services.LoyaltyProgram.BuyXAmountGetXDiscount;
using CKBS.Models.Services.POS;
using CKBS.Models.Services.POS.Template;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KEDI.Core.Premise.Models.Services.POS
{
    [Table("PendingVoidItem")]
    public class PendingVoidItem
    {
        [Key]
        public int ID { set; get; }
        public int OrderID { get; set; }
       
        public string OrderNo { get; set; }
      
        public int TableID { get; set; }
       
        public string ReceiptNo { get; set; }
       
        public string QueueNo { get; set; }
       
        [Column(TypeName = "Date")]
        public DateTime DateIn { get; set; }
       
        [Column(TypeName = "Date")]
        public DateTime DateOut { get; set; }
      
        public string TimeIn { get; set; }
       
        public string TimeOut { get; set; }

        public int WaiterID { get; set; }
       
        public int UserOrderID { get; set; }
       
        public int UserDiscountID { get; set; }
        
        public int CustomerID { get; set; }
       
        public int CustomerCount { get; set; }
       
        public int PriceListID { get; set; }
       
        public int LocalCurrencyID { get; set; }
      
        public int SysCurrencyID { get; set; }
     
        public double ExchangeRate { get; set; }
      
        public int WarehouseID { get; set; }
       
        public int BranchID { get; set; }
  
        public int CompanyID { get; set; }
       
        public double Sub_Total { get; set; }
      
        public double DiscountRate { get; set; }
     
        public double DiscountValue { get; set; }
       
        public string TypeDis { get; set; } = "Percent";
      
        public double TaxRate { get; set; }
       
        public double TaxValue { get; set; }
        public decimal OtherPaymentGrandTotal { get; set; }
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
        public int PLCurrencyID { get; set; }
        public double PLRate { get; set; }
        public double LocalSetRate { get; set; }

        public List<PendingVoidItemDetail> PendingVoidItemDetails { get; set; }
        [NotMapped]
        [ForeignKey("PLCurrencyID")]
        public Currency Currency { get; set; }
        public string Reason { get; set; }
        public bool IsVoided { get; set; }
        public TaxOptions TaxOption { set; get; }
        public int PromoCodeID { get; set; }
        public double PromoCodeDiscRate { get; set; }
        public double PromoCodeDiscValue { get; set; }
        public int RemarkDiscountID { get; set; }
        public int BuyXAmountGetXDisID { get; set; }
        public decimal BuyXAmGetXDisRate { get; set; }
        public decimal BuyXAmGetXDisValue { get; set; }
        public decimal CardMemberDiscountRate { get; set; }
        public decimal CardMemberDiscountValue { get; set; }
        public TypeDiscountBuyXAmountGetXDiscount BuyXAmGetXDisType { get; set; } // Rate=1, Value=2
        public int TaxGroupID { get; set; }
        [NotMapped]
        public List<DisplayPayCurrencyModel> GrandTotalCurrencies { get; set; }
        public string GrandTotalCurrenciesDisplay { get; set; }
        [NotMapped]
        public List<DisplayPayCurrencyModel> ChangeCurrencies { get; set; }
        public string ChangeCurrenciesDisplay { get; set; }
        public string GrandTotalOtherCurrenciesDisplay { get; set; }
        [NotMapped]
        public List<DisplayPayCurrencyModel> GrandTotalOtherCurrencies { get; set; }
        public PaymentType PaymentType { get; set; }
    }
}
