using System.Collections.Generic;

namespace KEDI.Core.Premise.Models.Services.POS.SecondScreen
{
    public class LineItemDisplay
    {
        public string LineID { get; set; }
        public string ParentLineID { get; set; }
        public int ItemID { get; set; }
        public string Code { get; set; }    
        public string ItemName { get; set; }
        public string ItemName2 { get; set; }
        public string ItemType { get; set; }
        public string Qty { get; set; }
        public string UoM { get; set; }
        public string UnitPrice { get; set; }
        public string DiscountRate { get; set; }
        public string DiscountValue { get; set; }
        public string TaxValue { set; get; }
        public string Total { get; set; }
    }
}
