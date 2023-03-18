using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.Services.ReportPurchase.SummaryOutgoinPaymentDetail
{
    public class SummaryOutgoingPaymentsInfo
    {
        public string VendorName { get; set; }
        public string MasDocumentNo { get; set; }
        public string MasDate { get; set; }
        public string DocType { get; set; }
        public string MasTotal { get; set; }
        public string MasApplied { get; set; }
        public string MasBalanceDue { get; set; }
        public string MasStatus { get; set; }
        public string DetailDete { get; set; }
        public string DetailTotal { get; set; }
        public string DetailCash { get; set; }
        public string DetailDate { get; set; }
        public string DetailBalanceDue { get; set; }
        public string SumTotal { get; set; }
        public string SumBalanceDue { get; set; }
        public string SumAppliedSSC { get; set; }
        public string SumApplied { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
    }
}
