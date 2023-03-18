using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.ServicesClass.Purchase
{
    public class DraftDataViewModel
    {
        public string LineID { get; set; }
        public int CustomerID { get; set; }
        public string DocType { get; set; }
        public string DraftName { get; set; }
        public string PostingDate { get; set; }
        public string CurrencyName { get; set; }
        public string SubTotal { get; set; }
        public string BalanceDue { get; set; }
        public string Remarks { get; set; }
        public int SeriesDetailID { get; set; }
        public int DraftID { get; set; }
    }
}
