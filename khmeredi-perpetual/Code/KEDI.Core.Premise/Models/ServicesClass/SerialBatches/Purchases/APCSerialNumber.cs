using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace KEDI.Core.Premise.Models.ServicesClass.SerialBatches.Purchases
{
    public class APCSerialNumber
    {
        public string LineID { get; set; } = DateTime.Now.Ticks.ToString();
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string WhsCode { get; set; }
        public string WhsName { get; set; }
        public decimal Qty { get; set; }
        public decimal TotalSelected { get; set; }
        public decimal OpenQty { get; set; }
        public string Direction { get; set; }
        public APCSerialNumberDetial APCSerialNumberDetial { get; set; }
        public APCSNSelected APCSNSelected { get; set; }
        public int ItemID { get; set; }
        public int BpId { get; set; }
        public int UomID { get; set; }
        public decimal Cost { get; set; }
         [NotMapped]
         public int BaseOnID { get; set; }
         [NotMapped]
         public int PurCopyType { get; set; }
          [NotMapped]
         public bool Newrecord { get; set; }=true;
    }
    public class APCSerialNumberDetial
    {
        public decimal TotalAvailableQty { get; set; }
        public List<APCSNDUnselectDetial> APCSNDDetials { get; set; }
    }
    public class APCSNDUnselectDetial
    {


        public string LineID { get; set; } = DateTime.Now.Ticks.ToString();
        public string MfrSerialNo { get; set; }
        public string SerialNumber { get; set; }
        public string PlateNumber { get; set; }
        public string LotNumber { get; set; }

        public string Color { get; set; }
        public string Brand { get; set; }
        public string Condition { get; set; }
        public string Type { get; set; }
        public string Power { get; set; }
        public string Year { get; set; }
        public string ExpirationDate { get; set; }
        public string MfrDate { get; set; }
        public string AdmissionDate { get; set; }
        public string MfrWarrantyStart { get; set; }
        public string MfrWarrantyEnd { get; set; }
        public string Location { get; set; }
        public string Detials { get; set; }
        public decimal UnitCost { get; set; }
        public decimal Qty { get; set; }
        public int BPID { get; set; }
          [NotMapped]
         public int BaseOnID { get; set; }
         [NotMapped]
         public int PurCopyType { get; set; }
       


    }
    public class APCSNSelected
    {
        public decimal TotalSelected { get; set; }
        public List<APCSNDSelectedDetail> APCSNDSelectedDetails { get; set; }
    }
    public class APCSNDSelectedDetail : APCSNDUnselectDetial
    {
    }
}
