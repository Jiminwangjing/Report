using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.ServicesClass.Report
{
    public class BPAging
    {
        public string InvoiceNo { get; set; }
        public string PostingDate { get; set; }
        public string DueDate { get; set; }
        public string CustomerName { get; set; }
        public string VendorName { get; set; }
        public string BalanceDue { get; set; }
        public string CustomerCode { get; set; }
        public int SaleCNID { get; set; }
        public int SaleARID { get; set; }
        public int IncomingID { get; set; }
        public double TotalAmount { get; set; }
        public double SBalanceDue { get; set; }
        public int PurchaseAPID { get; set; }
        public int PurchaseMemoID { get; set; }
        public int OutgoingPaymentID { get; set; }
        public string DouType { get; set; }
        public string RefNo { get; set; }

        public string VendorCode { get; set; }
        public string Currencysys { get; set; }
        public int SeriesDID { get; set; }

    }
}
