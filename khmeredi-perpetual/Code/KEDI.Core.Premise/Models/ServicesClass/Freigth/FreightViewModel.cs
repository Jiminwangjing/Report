using CKBS.Models.Services.Administrator.Setup;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace CKBS.Models.ServicesClass.Friegh
{
    public class FreightViewModel
    {
        public string LineID { get; set; }
        public int ID { get; set; }
        public string Name { get; set; }
        public int RevenAcctID { get; set; }
        public string RevenAcctCode { get; set; }
        public int ExpenAcctID { get; set; }
        public string ExpenAcctCode { get; set; }
        public int OutTaxID { get; set; }
        public List<SelectListItem> OutTaxList { get; set; }
        public int InTaxID { get; set; }
        public List<SelectListItem> InTaxList { get; set; }
        public List<SelectListItem> FreightReceiptTypes { get; set; }
        public string AmountReven { get; set; }
        public string AmountExpen { get; set; }
        public FreightReceiptType FreightReceiptType { get; set; }
        public bool Default { get; set; }
        public decimal OutTaxRate { get; set; }
        public decimal InTaxRate { get; set; }
        public bool IsActive { get; set; }
    }
}
