using CKBS.Models.Services.Administrator.Setup;
using CKBS.Models.Services.Banking;
using CKBS.Models.Services.HumanResources;
using CKBS.Models.Services.LoyaltyProgram.BuyXAmountGetXDiscount;
using CKBS.Models.Services.POS.Template;
using KEDI.Core.Premise.Models.Services.POS;
using KEDI.Core.Premise.Models.ServicesClass.SerialBatches.Sales;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CKBS.Models.Services.POS
{
    [Table("tbOrder", Schema = "dbo")]
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int OrderID { get; set; }
        public DateTime PostingDate { get; set; }

        [Required]
        public string OrderNo { get; set; }
        [NotMapped]
        public string CustomerCode { get; set; }
        [NotMapped]
        public string CustomerName { get; set; }
        [Required]
        public int TableID { get; set; }
        [Required]
        public string ReceiptNo { get; set; } = string.Empty;
        [Required]
        public string QueueNo { get; set; }
        [Required]
        [Column(TypeName = "Date")]
        public DateTime DateIn { get; set; }
        [Required]
        public string TimeIn { get; set; }
        [Required]
        [Column(TypeName = "Date")]
        public DateTime DateOut { get; set; }
        [Required]
        public string TimeOut { get; set; }
        [NotMapped]
        public List<MultiPaymentMeans> MultiPaymentMeans { get; set; }
        public int WaiterID { get; set; }
        [Required]
        public int UserOrderID { get; set; }
        [Required]
        public int UserDiscountID { get; set; }
        [Required]
        public int CustomerID { get; set; }
        [NotMapped]
        public BusinessPartner Customer { set; get; }
        [Required]
        public int CustomerCount { get; set; }
        [Required]
        public int PriceListID { get; set; }
        [Required]
        public int LocalCurrencyID { get; set; }
        [Required]
        public int SysCurrencyID { get; set; }
        [Required]
        public double ExchangeRate { get; set; }
        [Required]
        public int WarehouseID { get; set; }
        [Required]
        public int BranchID { get; set; }
        [Required]
        public int CompanyID { get; set; }
        [Required]
        public double Sub_Total { get; set; }
        [Required]
        public double DiscountRate { get; set; }
        [Required]
        public double DiscountValue { get; set; }
        [Required]
        public string TypeDis { get; set; } = "Percent";
        [Required]
        public double TaxRate { get; set; }
        [Required]
        public double TaxValue { get; set; }
        public decimal OtherPaymentGrandTotal { get; set; }
        [Required]
        public double GrandTotal { get; set; }
        [Required]
        public double GrandTotal_Sys { get; set; }
        public decimal AppliedAmount { get; set; } //27-09-2021
        public double Tip { get; set; } = 0;
        [Required]
        public double Received { get; set; }
        [Required]
        public double Change { get; set; }
        public string CurrencyDisplay { get; set; }
        public double DisplayRate { get; set; }
        public double GrandTotal_Display { get; set; }
        public double Change_Display { get; set; }
        [Required]
        public int PaymentMeansID { get; set; }
        [Required]
        public char CheckBill { get; set; }
        public bool Cancel { get; set; } = false;
        public bool Delete { get; set; } = false;
        public int PLCurrencyID { get; set; }
        public double PLRate { get; set; }
        public double LocalSetRate { get; set; }
        public int SeriesID { get; set; }
        public int SeriesDID { get; set; }
        public List<OrderDetail> OrderDetail { get; set; }
        [NotMapped]
        public List<SerialNumber> SerialNumbers { get; set; }
        [NotMapped]
        public List<BatchNo> BatchNos { get; set; }
        [ForeignKey("PLCurrencyID")]
        public Currency Currency { get; set; }
        public string Remark { get; set; }
        public string Reason { get; set; }
        public int Male { get; set; }
        public int Female { get; set; }
        public int Children { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Sending;
        [NotMapped]
        public List<FreightReceipt> Freights { set; get; }
        public decimal FreightAmount { set; get; }
        [NotMapped]
        public string TitleNote { get; set; }
        public int VehicleID { get; set; }
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
        public string RefNo { get; set; }
        public TypeDiscountBuyXAmountGetXDiscount BuyXAmGetXDisType { get; set; } // Rate=1, Value=2
        public int TaxGroupID { get; set; }
        [NotMapped]
        public List<DisplayPayCurrencyModel> GrandTotalCurrencies { get; set; }
        [NotMapped]
        public List<DisplayPayCurrencyModel> ChangeCurrencies { get; set; }
        public string GrandTotalCurrenciesDisplay { get; set; }
        public string GrandTotalOtherCurrenciesDisplay { get; set; }
        [NotMapped]
        public List<DisplayPayCurrencyModel> GrandTotalOtherCurrencies { get; set; }
        public string ChangeCurrenciesDisplay { get; set; }
        [NotMapped]
        public List<DisplayPayCurrencyModel> DisplayPayOtherCurrency { get; set; }
        public PaymentType PaymentType { get; set; }
        [NotMapped]
        public bool Selected { set; get; }
        [NotMapped]
        public CustomerTips CustomerTips { get; set; }
    }
    public enum OrderStatus { Sending = 1, Billing = 2, Paid = 3 }
    public enum PaymentType { None = 0, CardMember = 1 }
}
