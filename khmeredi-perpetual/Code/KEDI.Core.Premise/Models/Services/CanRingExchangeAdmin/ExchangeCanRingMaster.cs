using CKBS.Models.ServicesClass;
using KEDI.Core.Premise.Models.ServicesClass.GeneralSettingAdminView;
using KEDI.Core.Premise.Models.ServicesClass.SerialBatches.Purchases;
using KEDI.Core.Premise.Models.ServicesClass.SerialBatches.Sales;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KEDI.Core.Premise.Models.Services.CanRingExchangeAdmin
{
    public class ExchangeCanRingMaster
    {
        public int ID { get; set; }
        [NotMapped]
        public string DocCode { get; set; }
        public string Number { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "0:yyyy-MM-dd")]
        [Column(TypeName = "Date")]
        public DateTime CreatedAt { get; set; } = DateTime.Today;
        [NotMapped]
        public string VendorName { get; set; }
        [NotMapped]
        public string CurrencyName { get; set; }
        [NotMapped]
        public string UserName { get; set; }
        [NotMapped]
        public string WarehouseName { get; set; }
        [NotMapped]
        public string PriceList { get; set; }
        [NotMapped]
        public string PaymentMeans { get; set; }
        public int DocTypeID { get; set; }
        public int SeriesID { get; set; }
        public int SeriesDID { get; set; }
        public int CusId { get; set; }
        public int PriceListID { get; set; }
        public int UserID { get; set; }
        public List<ExchangeCanRingDetail> ExchangeCanRingDetails { get; set; }
        public int WarehouseID { get; set; }
        public int BranchID { get; set; }
        public int PaymentMeanID { get; set; }
        public double LocalSetRate { get; set; }
        public int LocalCurrencyID { get; set; }
        public int CompanyID { get; set; }
        public int SysCurrencyID { get; set; }
        public decimal ExchangeRate { get; set; }
        public decimal TotalSystem { get; set; }
        public decimal Total { get; set; }
        [NotMapped]
        public string TotalDis { get; set; }
        [NotMapped]
        public string TotalSystemDis { get; set; }
        [NotMapped]
        public string TotalLocal { get; set; }
        [NotMapped]
        public string CampanyLogo { get; set; }
    }

    public class ExchangeCanRingParam
    {
        public ExchangeCanRingMaster ExchangeCanRingMaster { get; set; }
        public List<SerialViewModelPurchase> SerialPurViews { get; set; }
        public List<BatchViewModelPurchase> BatchPurViews { get; set; }
        public List<SerialNumber> SerialNumbers { get; set; }
        public List<BatchNo> BatchNos { get; set; }
        public List<SeriesInPurchasePoViewModel> SeriesEC { get; set; }
        public GeneralSettingAdminViewModel GenSetting { get; set; }
    }
    public class HistoryExchangeCanRingParamFilter
    {
        public int BranchID { get; set; }
        public int UserID { get; set; }
        public int VendorID { get; set; }
        public int PriceListID { get; set; }
        public int WarehouseID { get; set; }
        public int PaymentMeansID { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
    }
}
