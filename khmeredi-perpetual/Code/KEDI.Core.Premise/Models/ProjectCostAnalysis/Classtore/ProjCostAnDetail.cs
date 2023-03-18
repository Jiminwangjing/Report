namespace KEDI.Core.Premise.Models.ProjectCostAnalysis.Classtore
{
    public class ProjCostAnDetail
    {
        public int ID { get; set; }
        public int ItemID { get; set; }
        public string Code { get; set; }
        public int Group1 { get; set; }
        public int Group2 { get; set; }
        public int Group3 { get; set; }
        public string KhmerName { get; set; }
        public string EnglishName { get; set; }
        public double InStock { get; set; }
        public double Qty { get; set; }
        public double PrintQty { get; set; }
        public double Cost { get; set; }
        public double UnitPrice { get; set; }
        public float DiscountRate { get; set; }
        public double DiscountValue { get; set; }
        public string TypeDis { get; set; }
        public double VAT { get; set; }

        public int CurrencyID { get; set; }
        public string Currency { get; set; }
        public int UomID { get; set; }
        public string UoM { get; set; }
        public string Barcode { get; set; }
        public string Process { get; set; }
        public string Image { get; set; }
        public int PricListID { get; set; }
        public int GroupUomID { get; set; }
        public string PrintTo { get; set; }
        public string ItemType { get; set; }
        public string Description { get; set; }
        public bool IsAddon { get; set; }
       
        public bool IsScale { set; get; }
        public int TaxGroupSaleID { get; set; }
    }
}
