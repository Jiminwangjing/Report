using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.InventoryAuditReport
{
    public class StockMovingView
    {
        public int WhID { get; set; }
        public int ItemID { get; set; }
        public string ItemCode { get; set; }
        public string KhmerName { get; set; }
        public string EnglishName { get; set; }
        public string Barcode { get; set; }
        public string Uom { get; set; }
        public string Group1 { get; set; }
        public string Group2 { get; set; }
        public string Group3 { get; set; }
        public string Whcode { get; set; }
        public double OB { get; set; }
        public double PU { get; set; }
        public double PD { get; set; }
        public double GR { get; set; }
        public double PC { get; set; }
        public double IN { get; set; }
        public double CN { get; set; }
        public double GI { get; set; }
        public double SP { get; set; }
        public double RP { get; set; }
        public double RE { get; set; }
        public double ST { get; set; }
        public double STT { get; set; }
        public double DN { get; set; }
        public double INEIN { get; set; }
        public double INEOUT { get; set; }
        public double EB { get; set; }
        public string TotalCost { get; set; }
        public string totalcosttt { get; set; }
        public string Currency { get; set; }
    }
}