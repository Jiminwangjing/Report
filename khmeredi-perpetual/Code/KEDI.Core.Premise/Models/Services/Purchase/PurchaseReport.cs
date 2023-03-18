using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.Services.Purchase
{
    public class PurchaseReport
    {
        public int ID { get; set; }
        public string Invoice { get; set; }
        public string Requester { get; set; }
        public string VendorCode { get; set; }
        public string VendorName { get; set; }
        public string UserName { get; set; }
        public double Balance { get; set; }
        public double ExchangeRate { get; set; }
        public string Status { get; set; }
        public string Cancele { get; set; }
        [NotMapped]
        public List<SelectListItem> VatType { get; set; }
    }
}
