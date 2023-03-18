using Microsoft.AspNetCore.Mvc.Rendering;
using CKBS.Models.Services.HumanResources;
using CKBS.Models.Services.Inventory.PriceList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.HumanResources
{
    public class BusinessPartnerViewModel
    {
        public BusinessPartner BusinessPartner { get; set; }
        public List<AutoMobileViewModel> AutoMobiles { get; set; }
        public SelectList PriceLists { get; set; }
        public int GLAccID { get; set; }
        public string GLAccName { get; set; }
        public string GLAccCode { get; set; }
        public int SaleEmpID { get; internal set; }
        //public string SaleEmpID { get; set; }
        public string SaleEmpName { get; set; }
        public List<SelectListItem> PaymentTerms { get; set; }
        public List<SelectListItem> DueDate { get; set; }
        public List<SelectListItem> OpenIncomingPayment { get; set; }
        public List<SelectListItem> CreditMethod { get; set; }
    }

    public class AutoMobileViewModel
    {
        public int AutoMID { get; set; }
        public int BusinessPartnerID { get; set; }
        public int TypeID { get; set; }
        public int BrandID { get; set; }
        public int ModelID { get; set; }
        public int ColorID { get; set; }
        public string Plate { get; set; }
        public string Frame { get; set; }
        public string Engine { get; set; }
        public List<SelectListItem> VehiTypes { get; set; }
        public List<SelectListItem> VehiBrands { get; set; }
        public List<SelectListItem> VehiModels { get; set; }
        public string Year { get; set; }
        public List<SelectListItem> VehiColors { get; set; }
        public bool Deleted { get; set; }
        public string KeyID { get; set; }
    }
}
