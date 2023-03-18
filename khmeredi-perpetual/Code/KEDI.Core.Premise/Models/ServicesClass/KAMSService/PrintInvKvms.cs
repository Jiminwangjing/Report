using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.ServicesClass.KAMSService
{
    public class PrintInvKvms
    {
        public string QNo { get; set; }
        public string Code { get; set; } // Customer
        public string Name { get; set; }
        public string Phone { get; set; }
        public string PriceListName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Plate { get; set; } //Auto Mobile
        public string Frame { get; set; }
        public string Engine { get; set; }
        public string TypeName { get; set; }
        public string BrandName { get; set; }
        public string ModelName { get; set; }
        public string Year { get; set; }
        public string ColorName { get; set; }
        //Quote Master Invoice
        public string Username { get; set; }
        public string Count { get; set; }
        public string Subtotal { get; set; }
        public string DisRate { get; set; }
        public string TaxValue { get; set; }
        public string GrandTotal { get; set; }
        public string AppliedAmount { get; set; }
        public string BalanceDue { get; set; }
        //Company Info
        public string ComBName { get; set; }
        public string ComBAddress { get; set; }
        public string ComBPhone { get; set; }
        public List<PrintDetailQuotes> PrintDetailQuotes { get; set; }
    }
}
