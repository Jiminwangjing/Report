using CKBS.Models.Services.LoyaltyProgram.ComboSale;
using System.Collections.Generic;

namespace KEDI.Core.Premise.Models.Services.POS.LoyaltyProgram.ComboSale
{
    public class ComboSaleViewModel
    {
        public int ID { get; set; }
        public int ItemID { get; set; }
        public int UomID { get; set; }
        public int TaxGroupID { get; set; }
        public string LineID { get; set; }
        public string Name { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public string Type { get; set; }
        public SaleType TypeEnum { get; set; }
        public double Qty { get; set; }
        public string QtyF { get; set; }
        public string UoM { get; set; }
        public decimal UnitPrice { get; set; }
        public string UnitPriceF { get; set; }
        public List<ComboSaleViewModel> ComboSaleDetials { get; set; }
    }
}
