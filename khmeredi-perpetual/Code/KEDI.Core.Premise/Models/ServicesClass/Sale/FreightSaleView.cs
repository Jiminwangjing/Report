using KEDI.Core.Premise.Models.Sale;
using KEDI.Core.Premise.Models.ServicesClass.TaxGroup;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace KEDI.Core.Premise.Models.ServicesClass.Sale
{
    public class FreightSaleView
    {
        public int ID { set; get; }
        public int StorID { get; set; }
        public int SaleID { set; get; }
        public SaleCopyType SaleType { get; set; }
        public decimal AmountReven { set; get; }
        public decimal OpenAmountReven { set; get; }
        public decimal TaxSumValue { set; get; }
        public IEnumerable<FreightSaleDetailViewModel> FreightSaleDetailViewModels { get; set; }
    }

    public class FreightSaleDetailViewModel
    {
        public string LineID { get; set; }
        public int StorDID { get; set; }
        public int ID { get; set; }
        public int TaxGroupID { get; set; }
        public int FreightID { set; get; }
        public int FreightSaleID { get; set; }
        public string Name { get; set; }
        public string TaxGroup { get; set; }
        public List<SelectListItem> TaxGroupSelect { get; set; }
        public List<TaxGroupViewModel> TaxGroups { get; set; }
        public decimal TaxRate { get; set; }
        public decimal TotalTaxAmount { get; set; }
        public decimal Amount { get; set; }
        public decimal AmountWithTax { get; set; }
    }
}
