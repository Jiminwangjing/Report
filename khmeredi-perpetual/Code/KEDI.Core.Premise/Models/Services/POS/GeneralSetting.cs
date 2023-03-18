using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CKBS.Models.Services.POS
{
    public enum CloseShiftType
    {
        [Display(Name = "None")]
        None = 0,
        [Display(Name = "Category")]
        Category = 1,
        [Display(Name = "Payment Mean")]
        PaymentMean = 2,
        [Display(Name = "Category Summary")]
        CategorySummary = 3,
        [Display(Name = "Payment Mean Summary")]
        PaymentMeanSummary = 4,
        [Display(Name = "Category And Payment")]
        CategoryAndPayment = 5,
        [Display(Name = "CategorySummary And PaymentSummary")]
        CategorySummaryAndPaymentSummary = 6
    }
    public enum PanelViewMode { Split, Single }
    public enum ItemViewType { Grid, List }
    public enum PrintReceiptOption
    {
 Khmer,
        English,
        [Display(Name = " No Header Eng")]
        NoHeader,
        [Display(Name = " Serial")]
        Serial,
        [Display(Name = "Left QR Code")]
        QRCode,
        [Display(Name = "Bottom QR Code")]
        BottomQRCode,
        [Display(Name = "Two QR Code")]
        TwoQRCode,
        [Display(Name = "English Logo Top Center")]
        EnglishCenter,
        [Display(Name = "English Logo Top Center Left QR")]
        EnglishCenterLeftQR,
        [Display(Name = "English Logo Top Center Bottom QR")]
        EnglishCenterBottomQR,
        [Display(Name = "English Logo Top Center two QR")]
        EnglishCentertwoQR,
        [Display(Name = "Receipt ItemName kh+Eng")]
        Receipt_ItemName,
        [Display(Name = "Receipt Item Name Kh+Eng left QR")]
        Receipt_ItemName_leftQR,
        [Display(Name = " Receipt Item Name Kh+Eng Bottom QR")]
        Receipt_ItemName_bottomQR,
        [Display(Name = "Receipt ItemName kh+Eng two QR")]
        Receipt_ItemName_twoQr,
        [Display(Name = " No Header Kh")]
        NoHeaderKhmer,
        [Display(Name = "Khmer Left QR Code")]
        Kh_QRCode,
        [Display(Name = "khmer Bottom QR Code")]
        kh_BottomQRCode,
        [Display(Name = "khmer Two QR Code")]
        kh_TwoQRCode,
        [Display(Name = "Khmer Logo Top Center")]
        KhmerCenter,
        [Display(Name = "Khmer Logo Top Center Left QR")]
        KhmerCenter_leftQR,
        [Display(Name = "Khmer Logo Top Center Bottom QR")]
        KhmerCenter_bottomQR,
        [Display(Name = "Khmer Logo Top Center Two QR")]
        KhmerCenter_twoQr,

    }
    public enum QueueOptions { Counter, Sheet, Day }
    public enum TaxOptions
    {
        [Display(Name = "None")]
        None,
        [Display(Name = "Exclude (VAT)")]
        Exclude,
        [Display(Name = "Include (Commercial)")]
        Include,
        [Display(Name = "Invoice (VAT)")]
        InvoiceVAT
    }
    [Table("tbGeneralSetting", Schema = "dbo")]
    public class GeneralSetting
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int BranchID { get; set; }
        public int DelayHours { get; set; }
        public string Receiptsize { get; set; }
        public string ReceiptTemplate { get; set; }
        public bool DaulScreen { get; set; }
        public bool PrintReceiptOrder { get; set; }
        public bool PrintReceiptTender { get; set; }
        public int PrintCountReceipt { get; set; }
        public int PrintCountBill { get; set; }
        public int QueueCount { get; set; }
        public int SysCurrencyID { get; set; }
        public int LocalCurrencyID { get; set; }
        public double RateIn { get; set; }
        public double RateOut { get; set; }
        public string Printer { get; set; }
        public int PaymentMeansID { get; set; }
        public int CompanyID { get; set; }
        public int WarehouseID { get; set; }
        public int CustomerID { get; set; }
        public int PriceListID { get; set; }
        public bool VatAble { get; set; }
        public string VatNum { get; set; }
        public string Wifi { get; set; }
        public string MacAddress { get; set; }
        public bool PreviewReceipt { get; set; }
        public bool AutoQueue { get; set; } = true;
        public bool PrintLabel { get; set; } = false;
        public CloseShiftType CloseShift { get; set; }
        public int UserID { get; set; }
        public ItemViewType ItemViewType { get; set; }
        public int ItemPageSize { get; set; } = 12;
        public int SeriesID { get; set; }
        public bool IsOrderByQR { get; set; }
        public PanelViewMode PanelViewMode { get; set; } = PanelViewMode.Split;
        public PrintReceiptOption PrintReceiptOption { get; set; }
        public string PrinterOrder { get; set; }
        public int PrintOrderCount { get; set; }
        public string PrintLabelName { get; set; }
        public string PrintBillName { get; set; }
        public QueueOptions QueueOption { get; set; }
        public TaxOptions TaxOption { set; get; }
        public int Tax { set; get; }
        public bool EnablePromoCode { set; get; }
        public bool IsCusPriceList { set; get; }
        public bool RememberCustomer { set; get; }
        public bool Cash { set; get; }
        public bool EnableCountMember { get; set; }
        [NotMapped]
        public Dictionary<string, object> SortBy { set; get; }
        public bool Portraite { get; set; }
        public bool CustomerTips { set; get; }
        public bool SlideShow { get; set; }
        public int TimeOut { get; set; }
        public bool DefualtAmount {get;set;}
        // public int LimitOfSaleItems { get; set; }
    }
}
