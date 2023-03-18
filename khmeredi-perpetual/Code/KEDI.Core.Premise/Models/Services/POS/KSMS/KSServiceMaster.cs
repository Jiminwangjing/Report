using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace KEDI.Core.Premise.Models.Services.POS.KSMS
{
    public class KSServiceMaster
    {
        public int ID { get; set; }
        public string Number { get; set; }
        public int DocTypeID { get; set; }
        public int SeriesID { get; set; }
        public int SeriesDID { get; set; }
        public int CusId { get; set; }
        public int PriceListID { get; set; }
        public int UserID { get; set; }
        [NotMapped]
        public List<KSService> KsServiceDetials { get; set; }
        public int WarehouseID { get; set; }
        public int BranchID { get; set; }
        public double LocalSetRate { get; set; }
        public int LocalCurrencyID { get; set; }
        public int CompanyID { get; set; }
        public int SysCurrencyID { get; set; }
        public DateTime CreatedAt { get; set; }
        [Column(TypeName = "decimal(18, 8)")]
        public decimal ExchangeRate { get; set; }
    }
}
