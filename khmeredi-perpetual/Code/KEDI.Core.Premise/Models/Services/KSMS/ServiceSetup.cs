using System;
using System.Collections.Generic;

namespace KEDI.Core.Premise.Models.Services.KSMS
{
    public class ServiceSetup
    {
        public int ID { get; set; }
        public int PriceListID { get; set; }
        public DateTime CreatedAt { get; set; }
        public string SetupCode { get; set; }
        public decimal Price { get; set; }
        public List<ServiceSetupDetial> ServiceSetupDetials { get; set; }
        public int ItemID { get; set; }
        public bool Active { get; set; }
        public string Remark { get; set; }
        public int UomID { get; set; }
        public int UserID { get; set; }
    }
    public class ServiceSetupDetial
    {
        public int ID { get; set; }
        public int ServiceSetupID { get; set; }
        public int ItemID { get; set; }
        public decimal Qty { get; set; }
        public int GUomID { get; set; }
        public int UomID { get; set; }
        public decimal Cost { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Factor { get; set; }
        public int CurrencyID { get; set; }
    }
}
