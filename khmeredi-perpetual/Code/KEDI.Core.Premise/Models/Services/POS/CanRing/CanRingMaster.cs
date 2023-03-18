using CKBS.Models.Services.POS.Template;
using KEDI.Core.Premise.Models.Sync;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KEDI.Core.Premise.Models.Services.POS.CanRing
{
    [Table("CanRingMaster")]
    public class CanRingMaster : ISyncEntity
    {
        public int ID { get; set; }
        public string Number { get; set; }
        public int DocTypeID { get; set; }
        public int SeriesID { get; set; }
        public int SeriesDID { get; set; }
        public int CusId { get; set; }
        public int PriceListID { get; set; }
        public int UserID { get; set; }
        public List<CanRingDetail> CanRingDetials { get; set; }
        public int WarehouseID { get; set; }
        public int BranchID { get; set; }
        public int PaymentMeanID { get; set; }
        public double LocalSetRate { get; set; }
        public int LocalCurrencyID { get; set; }
        public int CompanyID { get; set; }
        public int SysCurrencyID { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "0:yyyy-MM-dd")]
        [Column(TypeName = "Date")]
        public DateTime CreatedAt { get; set; } = DateTime.Today;
        public decimal ExchangeRate { get; set; }
        public decimal TotalSystem { get; set; }
        public decimal Total { get; set; }
        public decimal TotalAlt { get; set; }
        public decimal Change { get; set; }
        public decimal ChangeAlt { get; set; }
        public decimal Received { get; set; }
        public decimal ReceivedAlt { get; set; }
        public decimal OtherPaymentGrandTotal { get; set; }
        [NotMapped]
        public List<DisplayPayCurrencyModel> GrandTotalAndChangeCurrencies { get; set; }
        public string GrandTotalCurrenciesDisplay { get; set; }
        public string ChangeCurrenciesDisplay { get; set; }
        [NotMapped]
        public List<DisplayPayCurrencyModel> DisplayPayOtherCurrency { get; set; }

        //ISyncEntity
        public Guid RowId { set; get; }
        public DateTimeOffset ChangeLog { set; get; }
        public int Spk { get; set; } // Server Primary Key
        public int Cpk { get; set; } // Client Primary Key
        public bool Synced { get; set; }
        public DateTime SyncDate { get; set; }
    }

    public class CanRingReport
    {
        // Master
        public int ID { get; set; }
        public string DocCode { get; set; }
        public string Number { get; set; }
        public string PaymentMeans { get; set; }
        public string TotalDis { get; set; }
        public string TotalSystemDis { get; set; }
        public string TotalLocal { get; set; }
        public string PriceList { get; set; }
        public string CustomerName { get; set; }
        public string UserName { get; set; }
        public string WarehouseName { get; set; }
        public string CreatedAt { get; set; }
        public string ExchangeRate { get; set; }
        // Detial
        public string ChangeQty { get; set; }
        public string Total { get; set; }
        public string ItemName { get; set; }
        public string ItemChangeName { get; set; }
        public string Name { get; set; }
        public string Qty { get; set; }
    }
}
