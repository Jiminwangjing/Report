using System.Collections.Generic;

namespace KEDI.Core.Premise.Models.ServicesClass.KSMS
{
    //public class KSServiceViewModel
    //{
    //    public string LineId { get; set; }
    //    public string LineID { get; set; }
    //    public string SetupCode { get; set; }
    //    public string ItemCode { get; set; }
    //    public string ItemName { get; set; }
    //    public decimal Qty { get; set; }
    //    public string UoM { get; set; }
    //    public decimal UnitPrice { get; set; }
    //    public decimal Cost { get; set; }
    //    public string CurName { get; set; }
    //    public double MaxCount { get; set; }
    //    public double UsedCount { get; set; }
    //    public double UsedCountM { get; set; }
    //    public int ItemID { get; set; }
    //    public int UoMID { get; set; }
    //    public int ServiceSetUpID { get; set; }
    //    public List<KSServiceViewModel> ServiceSetupDetials { get; set; }
    //    public decimal Factor { get; set; }
    //    public int GUomID { get; set; }
    //    public int CurrencyId { get; set; }
    //    public string ParentLineID { get; set; }
    //    public int LinePosition { get; set; }
    //    public bool IsKsmsMaster { get; set; } = false;
    //    public bool IsKsms { get; set; } = false;
    //}
    public class KSServiceViewModel
    {
        public int ID { get; set; }
        public int CusId { get; set; }
        public int ReceiptID { get; set; }
        public int ReceiptDID { get; set; }
        public int KSServiceSetupId { get; set; }
        public int VehicleID { get; set; }
        public int PriceListID { get; set; }
        public string LineID { get; set; }
        public string SetupCode { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public decimal Qty { get; set; }
        public string UoM { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Cost { get; set; }
        public string CurName { get; set; }
        public double MaxCount { get; set; }
        public double UsedCount { get; set; }
        public double UsedCountM { get; set; }
        public int ItemID { get; set; }
        public int UoMID { get; set; }
        public List<KSServiceViewModel> ServiceSetupDetials { get; set; }
        public decimal Factor { get; set; }
        public int GUomID { get; set; }
        public int CurrencyId { get; set; }
        public string ParentLineID { get; set; }
        public int LinePosition { get; set; }
        public bool IsKsmsMaster { get; set; } = false;
        public bool IsKsms { get; set; } = false;
    }
}
