using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace KEDI.Core.Premise.Models.ServicesClass.SerialBatches.Sales
{
    public class SerialNumberUnselectedDetial : SerialNumberDetialMasterClass
    {}

    public class SerialNumberDetialMasterClass
    {
        // public string LineID { get; set; } = DateTime.Now.Ticks.ToString();
        // public string SerialNumber { get; set; }
        // public string PlateNumber { get; set; }
        // public string MfrSerialNo { get; set; }
        // public string LotNumber { get; set; }

        // public string Color { get; set; }
        // public string Brand { get; set; }
        // public string Condition { get; set; }
        //  public string Type { get; set; }
        // public string Power { get; set; }
        // public string Year { get; set; }
            
        // public decimal UnitCost { get; set; }
        // public decimal Qty { get; set; }
        // public DateTime ExpireDate { get; set; }
        // public int ItemID { get; set; }
          public string LineID { get; set; } = DateTime.Now.Ticks.ToString();
        public string SerialNumber { get; set; }
        public string MfrSerialNo { get; set; }
        public string PlateNumber { get; set; }
        public string LotNumber { get; set; }
        public string Color { get; set; }
        public string Brand { get; set; }
        public string Condition { get; set; }
        public string Type { get; set; }
        public string Power { get; set; }
        public string Year { get; set; }
        
        public decimal UnitCost { get; set; }
        public decimal Qty { get; set; }
        public string ExpireDate { get; set; }
        public string MfrDate { get; set; }
        public string AdmissionDate { get; set; }
        public string MfrWarDateStart { get; set; }
        public string MfrWarDateEnd { get; set; }
        public string  Location { get; set; }
        public string Details { get; set; }
        public int ItemID { get; set; }

    }
}
