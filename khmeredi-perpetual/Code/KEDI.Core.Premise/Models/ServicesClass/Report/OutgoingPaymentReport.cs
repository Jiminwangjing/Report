using CKBS.Models.Services.Banking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.ServicesClass.Report
{
    public class OutgoingPaymentReport
    {
        public int OutgoingPaymentID { get; set; }
        public int LindID { get; set; }
        public string  Vendor { get; set; }
        public string User { get; set; }
        public string Status { get; set; }
        public string Date { get; set; }
        public string ItemInvoice { get; set; }
        public string TotalAmountDue { get; set; }
        public string Totalpayment { get; set; }
       
    }
   

}
