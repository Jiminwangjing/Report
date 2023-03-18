using Microsoft.EntityFrameworkCore;
using CKBS.AppContext;
using CKBS.Models.Services.Banking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CKBS.Models.Services.Financials;
using CKBS.Models.Services.Administrator.SystemInitialization;
using CKBS.Models.Services.ChartOfAccounts;
using KEDI.Core.Premise.Models.Services.Banking;

namespace CKBS.Models.Services.Responsitory
{

    public interface IOutgoingPayment
    {
        IEnumerable<OutgoingPaymentVendor> GetOutgoingPaymentVendor(int VendorID);
        void OutgoingPaymentSeriesAccounting(int id);
        void OutgoingPaymentSeriesAccountingCancel(string id, int seriesID);
        OutgoingPaymentOrder FindOutgoing(int id=0,string invoiceNumber="", int seriesID=0);
    }
    public class OutgoingPaymentResponsitory : IOutgoingPayment
    {
        private readonly DataContext _context;
        public OutgoingPaymentResponsitory(DataContext context)
        {
            _context = context;
        }
        public  OutgoingPaymentOrder FindOutgoing(int id=0,string invoiceNumber="", int seriesID=0)
        {
                    var list=id==0?_context.OutgoingPaymentOrders.Where(s=> s.Number==invoiceNumber&& s.SeriesID==seriesID&& s.Status=="open").ToList()
                                 :_context.OutgoingPaymentOrders.Where(s=> s.ID==id).ToList();
                    var obj=(from ob in list
                              join paymentmean in _context.PaymentMeans on ob.PaymentMeanID equals paymentmean.ID
                               join glAcc in _context.GLAccounts on paymentmean.AccountID equals glAcc.ID
                             join useracc in _context.UserAccounts on ob.UserID equals useracc.ID
                             select new OutgoingPaymentOrder
                             {
                                ID                          =   ob.ID,
                                Branch                      =   ob.Branch,
                                BranchID                    =   ob.BranchID,
                                BusinessPartner             =   ob.BusinessPartner,
                                CompanyID                   =   ob.CompanyID,
                                DocumentDate                =   ob.DocumentDate,
                                DocumentID                  =   ob.DocumentID,
                                LocalCurID                  =   ob.LocalCurID,
                                LocalSetRate                =   ob.LocalSetRate,
                                Number                      =   ob.Number,
                                NumberInvioce               =   ob.NumberInvioce,
                                PaymentMeanID               =   ob.PaymentMeanID,
                                PaymentMeanName             =   paymentmean.Type+" "+glAcc.Code+" - "+glAcc.Name,
                                PostingDate                 =   ob.PostingDate,
                                Ref_No                      =   ob.Ref_No,
                                Remark                      =   ob.Remark,
                                SeriesDetailID              =   ob.SeriesDetailID,
                                SeriesID                    =   ob.SeriesID,
                                Status                      =   ob.Status,
                                TotalAmountDue              =   ob.TotalAmountDue,
                                TypePurchase                =   ob.TypePurchase,
                                UserAccount                 =   ob.UserAccount,
                                UserID                      =   ob.UserID,
                                UserName                    =   useracc.Username,
                                VendorID                    =   ob.VendorID,
                                OutgoingPaymentOrderDetail  =   (from ogd in _context.OutgoingPaymentOrderDetails.Where(i=> i.OutgoingPaymentOrderID==ob.ID)
                                                                select new OutgoingPaymentOrderDetail
                                                                {
                                                                    Applied_Amount=ogd.Applied_Amount,
                                                                    BalanceDue=ogd.BalanceDue,
                                                                    BasedOnID=ogd.BasedOnID,
                                                                    CashDiscount=ogd.CashDiscount,
                                                                    CheckPay=ogd.CheckPay,
                                                                    Currency=ogd.Currency,
                                                                    CurrencyID=ogd.CurrencyID,
                                                                    CurrencyName=ogd.CurrencyName,
                                                                    Date=ogd.Date,
                                                                    Delete=ogd.Delete,
                                                                    DocNo=ogd.DocNo,
                                                                    DocTypeID=ogd.DocTypeID,
                                                                    ExchangeRate=ogd.ExchangeRate,
                                                                    ID=ogd.ID,
                                                                    ItemInvoice=ogd.ItemInvoice,
                                                                    LocalCurID=ogd.LocalCurID,
                                                                    LocalSetRate=ogd.LocalSetRate,
                                                                    NumberInvioce=ogd.NumberInvioce,
                                                                    OutgoingPaymentOrderID=ogd.OutgoingPaymentOrderID,
                                                                    OverdueDays=ogd.OverdueDays,
                                                                    Total=ogd.Total,
                                                                    TotalDiscount=ogd.TotalDiscount,
                                                                    Totalpayment=ogd.Totalpayment,

                                                                }).ToList(),
                        

                             }).FirstOrDefault();
                             return obj;
        }

        public IEnumerable<OutgoingPaymentVendor> GetOutgoingPaymentVendor(int VendorID) => _context.OutgoingPaymentVendors.FromSql("sp_GetOutgoingpayment @VendorID={0}",
            parameters: new[] {
                VendorID.ToString()
            });

        public void OutgoingPaymentSeriesAccounting(int id)
        {
            var OGP = _context.OutgoingPayments.FirstOrDefault(i => i.OutgoingPaymentID == id);
            var OGPD = _context.OutgoingPaymentDetails.Where(i=> i.OutgoingPaymentID == id && i.Delete == false).ToList();
            var totalAmountDue = OGPD.Sum(i=> i.Totalpayment);
            var docType = _context.DocumentTypes.Find(OGP.DocumentID);
            var company = _context.Company.Find(OGP.CompanyID);
            var series = _context.Series.Find(OGP.SeriesID);   
            JournalEntry journalEntry = new ();
            SeriesDetail seriesDetail = new ();
            var douTypeID = _context.DocumentTypes.FirstOrDefault(w => w.Code == "JE");
            var defaultJE = _context.Series.FirstOrDefault(w => w.Default == true && w.DocuTypeID == douTypeID.ID);
            // update series
            string Sno = defaultJE.NextNo;
            long No = long.Parse(Sno);
            defaultJE.NextNo = Convert.ToString(No + 1);
            // update series details
            seriesDetail.SeriesID = defaultJE.ID;
            seriesDetail.Number = Sno;
            _context.Update(defaultJE);
            _context.Update(seriesDetail);
            _context.SaveChanges();
            // Insert Journal Entry
            journalEntry.SeriesID = defaultJE.ID;
            journalEntry.Number = Sno;
            journalEntry.DouTypeID = defaultJE.DocuTypeID;
            journalEntry.Creator = OGP.UserID;
            journalEntry.TransNo = OGP.NumberInvioce;
            journalEntry.PostingDate = OGP.PostingDate;
            journalEntry.DocumentDate = OGP.DocumentDate;
            journalEntry.DueDate = OGP.PostingDate;
            journalEntry.SSCID = company.SystemCurrencyID;
            journalEntry.LLCID = OGP.LocalCurID;
            journalEntry.CompanyID = OGP.CompanyID;
            journalEntry.LocalSetRate = (decimal)OGP.LocalSetRate;
            journalEntry.SeriesDID = seriesDetail.ID;
            journalEntry.Remarks = series.Name + " " + OGP.NumberInvioce;
            _context.Update(journalEntry);
            _context.SaveChanges();
            // AccountReceice
            var accountReceive = _context.BusinessPartners.FirstOrDefault(w => w.ID == OGP.VendorID);
            var glAccD = _context.GLAccounts.FirstOrDefault(w => w.ID == accountReceive.GLAccID);
            List<JournalEntryDetail> journalEntryDetail = new ();
            journalEntryDetail.Add(new JournalEntryDetail
            {
                JEID = journalEntry.ID,
                Type = Financials.Type.BPCode,
                ItemID = accountReceive.GLAccID,
                Debit = (decimal)totalAmountDue,
                BPAcctID = OGP.VendorID,
            });
            //Insert 
            List<AccountBalance> accountBalance = new ();
            glAccD.Balance += (decimal)totalAmountDue;
            accountBalance.Add(new AccountBalance
            {
                JEID = journalEntry.ID,
                PostingDate = OGP.PostingDate,
                Origin = docType.ID,
                OriginNo = OGP.NumberInvioce,
                OffsetAccount = glAccD.Code,
                Details = douTypeID.Name + " - " + glAccD.Code,
                CumulativeBalance = glAccD.Balance,
                Debit = (decimal)totalAmountDue,
                LocalSetRate = (decimal)OGP.LocalSetRate,
                GLAID = accountReceive.GLAccID,
                BPAcctID = OGP.VendorID,
                Creator = OGP.UserID,
                Effective=EffectiveBlance.Debit
            });
            var accountSelected = _context.PaymentMeans.Find(OGP.PaymentMeanID);
            var glAccC = _context.GLAccounts.FirstOrDefault(w => w.ID == accountSelected.AccountID);
            journalEntryDetail.Add(new JournalEntryDetail
            {
                JEID = journalEntry.ID,
                Type = Financials.Type.GLAcct,
                ItemID = accountSelected.AccountID,
                Credit = (decimal)totalAmountDue,
                BPAcctID = OGP.VendorID,
            });
            //Insert 
            glAccC.Balance -= (decimal)totalAmountDue;
            accountBalance.Add(new AccountBalance
            {
                JEID = journalEntry.ID,
                PostingDate = OGP.PostingDate,
                Origin = docType.ID,
                OriginNo = OGP.NumberInvioce,
                OffsetAccount = glAccC.Code,
                Details = douTypeID.Name + " - " + glAccC.Code,
                CumulativeBalance = glAccC.Balance,
                Credit = (decimal)totalAmountDue,
                LocalSetRate = (decimal)OGP.LocalSetRate,
                GLAID = accountSelected.AccountID,
                BPAcctID = OGP.VendorID,
                Creator = OGP.UserID,
                Effective=EffectiveBlance.Credit
            });
            _context.Update(glAccC);
            _context.Update(glAccD);
            _context.UpdateRange(accountBalance);
            _context.UpdateRange(journalEntryDetail);
            _context.SaveChanges();
        }
        public void OutgoingPaymentSeriesAccountingCancel(string id, int seriesDID)
        {
            var OGP = _context.OutgoingPayments.FirstOrDefault(i => i.SeriesDetailID == seriesDID);
            var checkpay = _context.OutgoingPaymentDetails.Where(x => x.OutgoingPaymentID == OGP.OutgoingPaymentID && x.Delete == false).ToList();
            var totalAmountDue = checkpay.Sum(i=> i.Totalpayment);
            var docType = _context.DocumentTypes.Find(OGP.DocumentID);
            var company = _context.Company.Find(OGP.CompanyID);
            var series = _context.Series.Find(OGP.SeriesID);
            JournalEntry journalEntry = new ();
            SeriesDetail seriesDetail = new ();
            var douTypeID = _context.DocumentTypes.FirstOrDefault(w => w.Code == "JE");
            var defaultJE = _context.Series.FirstOrDefault(w => w.Default == true && w.DocuTypeID == douTypeID.ID);
            // update series
            string Sno = defaultJE.NextNo;
            long No = long.Parse(Sno);
            defaultJE.NextNo = Convert.ToString(No + 1);
            // update series details
            seriesDetail.SeriesID = defaultJE.ID;
            seriesDetail.Number = Sno;
            _context.Update(defaultJE);
            _context.Update(seriesDetail);
            _context.SaveChanges();
            // Insert Journal Entry
            journalEntry.SeriesID = defaultJE.ID;
            journalEntry.Number = Sno;
            journalEntry.DouTypeID = defaultJE.DocuTypeID;
            journalEntry.Creator = OGP.UserID;
            journalEntry.TransNo = OGP.NumberInvioce;
            journalEntry.PostingDate = OGP.PostingDate;
            journalEntry.DocumentDate = OGP.DocumentDate;
            journalEntry.DueDate = OGP.PostingDate;
            journalEntry.SSCID = company.SystemCurrencyID;
            journalEntry.LLCID = OGP.LocalCurID;
            journalEntry.CompanyID = OGP.CompanyID;
            journalEntry.LocalSetRate = (decimal)OGP.LocalSetRate;
            journalEntry.SeriesDID = seriesDetail.ID;
            journalEntry.Remarks = series.Name + " " + OGP.NumberInvioce;
            _context.Update(journalEntry);
            _context.SaveChanges();
            //IssuseInstock
            // AccountReceice
            var accountReceive = _context.BusinessPartners.FirstOrDefault(w => w.ID == OGP.VendorID);
            var glAccD = _context.GLAccounts.FirstOrDefault(w => w.ID == accountReceive.GLAccID);
            List<JournalEntryDetail> journalEntryDetail = new ();
            journalEntryDetail.Add(new JournalEntryDetail
            {
                JEID = journalEntry.ID,
                Type = Financials.Type.BPCode,
                ItemID = accountReceive.GLAccID,
                Credit = (decimal)totalAmountDue,
                BPAcctID = OGP.VendorID,
            });
            //Insert 
            List<AccountBalance> accountBalance = new ();
            glAccD.Balance -= (decimal)totalAmountDue;
            accountBalance.Add(new AccountBalance
            {
                JEID = journalEntry.ID,
                PostingDate = OGP.PostingDate,
                Origin = docType.ID,
                OriginNo = OGP.NumberInvioce,
                OffsetAccount = glAccD.Code,
                Details = douTypeID.Name + " - " + glAccD.Code,
                CumulativeBalance = glAccD.Balance,
                Credit = (decimal)totalAmountDue,
                LocalSetRate = (decimal)OGP.LocalSetRate,
                GLAID = accountReceive.GLAccID,
                BPAcctID = OGP.VendorID,
                Creator = OGP.UserID,
                Effective=EffectiveBlance.Credit
            });
            var accountSelected = _context.PaymentMeans.Find(OGP.PaymentMeanID);
            var glAccC = _context.GLAccounts.FirstOrDefault(w => w.ID == accountSelected.AccountID);
            journalEntryDetail.Add(new JournalEntryDetail
            {
                JEID = journalEntry.ID,
                Type = Financials.Type.GLAcct,
                ItemID = accountSelected.AccountID,
                Debit = (decimal)totalAmountDue,
                BPAcctID = OGP.VendorID,
            });
            //Insert 
            glAccC.Balance += (decimal)totalAmountDue;
            accountBalance.Add(new AccountBalance
            {
                JEID = journalEntry.ID,
                PostingDate = OGP.PostingDate,
                Origin = docType.ID,
                OriginNo = OGP.NumberInvioce,
                OffsetAccount = glAccC.Code,
                Details = douTypeID.Name + " - " + glAccC.Code,
                CumulativeBalance = glAccC.Balance,
                Debit = (decimal)totalAmountDue,
                LocalSetRate = (decimal)OGP.LocalSetRate,
                GLAID = accountSelected.AccountID,
                BPAcctID = OGP.VendorID,
                Creator = OGP.UserID,
                Effective=EffectiveBlance.Debit
            });
            _context.Update(glAccC);
            _context.Update(glAccD);
            _context.UpdateRange(accountBalance);
            _context.UpdateRange(journalEntryDetail);
            _context.SaveChanges();
         }
    }
}
