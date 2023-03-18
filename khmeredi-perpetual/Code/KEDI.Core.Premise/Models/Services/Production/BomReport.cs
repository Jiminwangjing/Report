using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.BOM
{
    public class BomReport
    {
        public int ID { get; set; }
        public int BID { get; set; }
        public string KhmerName { get; set; }
        public string PostingDate { get; set; }
        public string Uom { get; set; }
        public double TotalCost { get; set; }
        public bool Active { get; set; }
        public string SysCy { get; set; }
        public List<BomDetail> BomDetails { get; set; }
    }
      public class BomDetail
    {
        public int ID { get; set; }
        public double Qty { get; set; }
        public double Amount { get; set; }
        public double Cost { get; set; }
        public string UomD { get; set; }
        public string ItemName { get; set; }
        public bool Negativestock { get; set; }
    }
           
}
