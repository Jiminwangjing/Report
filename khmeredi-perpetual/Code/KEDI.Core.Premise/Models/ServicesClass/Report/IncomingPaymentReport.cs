using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.ServicesClass.Report
{
    public class IncomingPaymentReport
    {
        public int IncommingPaymentID { get; set; }
        public int LindID { get; set; }
        public string Vendor { get; set; }
        public string User { get; set; }
        public string Status { get; set; }
        public string Date { get; set; }
        public string ItemInvoice { get; set; }
        public string TotalAmountDue { get; set; }
        public double Totalpayment { get; set; }
        public string Currency { get; set; }
        public string InvoiceNumber { get; set; }
        public int IncomingPaymentDetail { get; set; }
    }
}
