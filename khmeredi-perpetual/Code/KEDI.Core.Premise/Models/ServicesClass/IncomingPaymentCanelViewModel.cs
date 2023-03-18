using CKBS.Models.Services.Account;
using CKBS.Models.Services.Administrator.General;
using CKBS.Models.Services.Banking;
using CKBS.Models.Services.ChartOfAccounts;
using CKBS.Models.Services.HumanResources;
using KEDI.Core.Premise.Models.Services.Banking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.ServicesClass
{
    public class IncomingPamentCancelViewModel
    {
        public int IncomingPaymentID { get; set; }
        public int UserID { get; set; }
          public string UserName{get;set;}
        public int SeriesID { get; set; }
        public int SeriesDID { get; set; }
        public int DocTypeID { get; set; }
        public int CompanyID { get; set; }
        public string InvoiceNumber { get; set; }
        public string Status { get; set; }
        public int PaymentMeanID { get; set; }
        public int CustomerID { get; set; }
         public string CustomerName{ get; set; }
         public string Type{ get; set; }
        public int BranchID { get; set; }
        public string Ref_No { get; set; }
        public DateTime PostingDate { get; set; }
        public DateTime DocumentDate { get; set; }
        public double TotalAmountDue { get; set; }
           public double TotalApplied { get; set; }
        public string Number { get; set; }
        public string Remark { get; set; }

        public List<IncomingPaymentDetail> IncomingPaymentDetails { get; set; }
         public List<IncomingPaymentOrderDetail> IncomingPaymentOrderDetails { get; set; }
        public List<MultiIncomming> MultPayIncommings { get; set; }
        public List<PaymentMeans> PaymentMean { get; set; }
        public UserAccount UserAccount { get; set; }
        public BusinessPartner BusinessPartner { get; set; }
        public Branch Branch { get; set; }
        public PaymentMeans PaymentMeans { get; set; }
        public GLAccount GLAccount { get; set; }
        public int LocalCurID { get; set; }
        public double LocalSetRate { get; set; }
    }
}
