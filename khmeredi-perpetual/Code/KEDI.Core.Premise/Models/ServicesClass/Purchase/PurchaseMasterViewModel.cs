using CKBS.Models.ServicesClass.Property;
using System.Collections.Generic;

namespace KEDI.Core.Premise.Models.ServicesClass.Purchase
{
    public class PurchaseMasterViewModel
    {
        public int ID { get; set; }
        public string Code { get; set; }
        public string ItemName1 { get; set; }
        public string ItemName2 { get; set; }
        public decimal Cost { get; set; }
        public string Currency { get; set; }
        public string UomName { get; set; }
        public string Barcode { get; set; }
        public string Process { get; set; }
        public decimal InStock { get; set; }
        public Dictionary<string, PropertydetailsViewModel> AddictionProps { get; set; }
    }
}
