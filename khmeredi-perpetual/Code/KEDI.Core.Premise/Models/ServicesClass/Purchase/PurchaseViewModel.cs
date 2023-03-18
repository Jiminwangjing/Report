using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.ServicesClass.Purchase
{
    public class PurchaseViewModel
    {
        public string LineID { get; set; }
        public string DocType { get; set; }
        public string Invoice { get; set; }
        public string PostingDate { get; set; }
        public string CurrencyName { get; set; }
        public string SubTotal { get; set; }
        public string BalanceDue { get; set; }
        public string Remarks { get; set; }
        public int SeriesID { get; set; }
        public string Number { get; set; }
    }
}
