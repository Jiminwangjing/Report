using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using CKBS.Models.ServicesClass;

namespace CKBS.Models.Services.POS.Template
{
    public class SettingModel
    {      
        public GeneralSetting Setting { get; set; }
        public SelectList Customers { get; set; }
        public List<SelectListItem> PriceLists { get; set; }
        public SelectList Warehouses { get; set; }
        public SelectList PaymentMeans { get; set; }
        public SelectList SystemTypes { get; set; }        
        public string RedirectUrl { get; set; }
        public SelectList PrinterNames { get; set; }
        public SelectList Series { get; set; }
        public List<SeriesInPurchasePoViewModel> SeriesPS { get; set; }
        public List<SelectListItem> TaxGroups { set; get; }
        public List<SelectListItem> Taxes { set; get; }
        public List<SelectListItem> QueueGroups { get; set; }
        public List<SelectListItem> ReceiptTemplateGroups { get; set; }
    }
}
