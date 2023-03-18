using CKBS.Models.Services.Banking;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CKBS.Models.Services.LoyaltyProgram.BuyXAmountGetXDiscount;
using CKBS.Models.Services.POS.Template;
using KEDI.Core.Premise.Models.Sync;

namespace CKBS.Models.Services.POS
{
    [Table("tbVoidOrder", Schema = "dbo")]
    public class VoidOrder : ISyncEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderID { get; set; }
        public int OpenShiftID {get; set;}

        [Required]
        public string OrderNo { get; set; }
        [Required]
        public int TableID { get; set; }
        public string ReceiptNo { get; set; }
        [Required]
        public string QueueNo { get; set; }
        [Required]
        [Column(TypeName = "Date")]
        public DateTime DateIn { get; set; }
        [Required]
        [Column(TypeName = "Date")]
        public DateTime DateOut { get; set; }
        [Required]
        public string TimeIn { get; set; }
        [Required]
        public string TimeOut { get; set; }

        public int WaiterID { get; set; }
        [Required]
        public int UserOrderID { get; set; }
        [Required]
        public int UserDiscountID { get; set; }
        [Required]
        public int CustomerID { get; set; }
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

        public List<VoidOrderDetail> VoidOrderDetail { get; set; }
        [ForeignKey("PLCurrencyID")]
        public Currency Currency { get; set; }
        public string Reason { get; set; }
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

        //ISyncEntity
        public Guid RowId { set; get; }
        public DateTimeOffset ChangeLog { set; get; }
        public int Spk { get; set; } // Server Primary Key
        public int Cpk { get; set; } // Client Primary Key
        public bool Synced { get; set; }
        public DateTime SyncDate { get; set; }
    }
}
