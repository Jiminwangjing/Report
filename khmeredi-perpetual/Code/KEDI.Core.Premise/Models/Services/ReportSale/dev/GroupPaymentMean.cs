using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.ReportSale.dev
{
    public class GroupPaymentMean
    {
        //Group
        public string PaymentType { get; set; }
        public double SubTotal { get; set; }
        public List<Receipts> Receipts { get; set; }
        public Header Header { get; set; }
        public Footer Footer { get; set; }
    }
    
}
