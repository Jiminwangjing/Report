using CKBS.Models.Services.Purchase;
using KEDI.Core.Premise.Models.ServicesClass.TaxGroup;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.ServicesClass.Purchase
{

    public class FreightPurchaseViewModel
    {
        public int ID { set; get; }
        public int PurID { set; get; }
        public PurCopyType PurType { get; set; }
        public decimal ExpenceAmount { set; get; }
        public decimal OpenExpenceAmount { set; get; }
        public decimal TaxSumValue { set; get; }
        public IEnumerable<FreightPurchaseDetailViewModel> FreightPurchaseDetailViewModels { get; set; }
    }

    public class FreightPurchaseDetailViewModel
    {
        public int ID { get; set; }
        public int TaxGroupID { get; set; }
        public int FreightID { set; get; }
        public int FreightPurchaseID { get; set; }
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

