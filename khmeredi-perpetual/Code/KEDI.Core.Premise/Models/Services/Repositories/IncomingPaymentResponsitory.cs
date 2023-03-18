using CKBS.AppContext;
using System;
using System.Collections.Generic;
using System.Linq;
using CKBS.Models.Services.Financials;
using CKBS.Models.Services.Administrator.SystemInitialization;
using CKBS.Models.Services.ChartOfAccounts;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using CKBS.Models.ServicesClass;
using CKBS.Models.Services.Account;
using CKBS.Models.Services.Banking;
using KEDI.Core.Premise.Models.Services.Banking;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CKBS.Models.Services.Responsitory
{

    public interface IIncomingPayment
    {
        void IncomingPaymentSeriesAccounting(int id);
        void IncomingPaymentSeriesAccountingCancel(string id, int seriesDID);
        void IncomingPaymentCancelPOS(int InpaymentID, int? PaymentmeansID = 0);
         Task<IncomingPamentCancelViewModel> GetPaymentOrder(int id=0,string invnumber="",int seriesID=0);
    }
    public class IncomingPaymentResponsitory : IIncomingPayment
    {
        private readonly DataContext _context;
        public IncomingPaymentResponsitory(DataContext context)
        {
            _context = context;
        }
        public void IncomingPaymentSeriesAccounting(int id)
        {
            var ICP = _context.IncomingPayments.FirstOrDefault(i => i.IncomingPaymentID == id);
            var ICPD = _context.IncomingPaymentDetails.Where(i => i.IncomingPaymentID == ICP.IncomingPaymentID && i.Delete == false).ToList();
            var TotalAmountDue = ICPD.Sum(i => i.Totalpayment * i.ExchangeRate);
            var docType = _context.DocumentTypes.Find(ICP.DocTypeID);
            var company = _context.Company.Find(ICP.CompanyID);
            var series = _context.Series.Find(ICP.SeriesID);
            JournalEntry journalEntry = new();
            SeriesDetail seriesDetail = new();
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
            journalEntry.BranchID=ICP.BranchID;
            journalEntry.DouTypeID = defaultJE.DocuTypeID;
            journalEntry.Creator = ICP.UserID;
            journalEntry.TransNo = ICP.Number;
            journalEntry.PostingDate = ICP.PostingDate;
            journalEntry.DocumentDate = ICP.DocumentDate;
            journalEntry.DueDate = ICP.PostingDate;
            journalEntry.SSCID = company.SystemCurrencyID;
            journalEntry.LLCID = ICP.LocalCurID;
            journalEntry.CompanyID = ICP.CompanyID;
            journalEntry.LocalSetRate = (decimal)ICP.LocalSetRate;
            journalEntry.SeriesDID = seriesDetail.ID;
            journalEntry.Remarks = series.Name + " " + ICP.Number;
            _context.Update(journalEntry);
            _context.SaveChanges();
            //IssuseInstock
            // AccountReceice
            var accountReceive = _context.BusinessPartners.FirstOrDefault(w => w.ID == ICP.CustomerID);
            var glAccD = _context.GLAccounts.FirstOrDefault(w => w.ID == accountReceive.GLAccID);
            List<JournalEntryDetail> journalEntryDetail = new();
            journalEntryDetail.Add(new JournalEntryDetail
            {
                JEID = journalEntry.ID,
                Type = Financials.Type.BPCode,
                ItemID = accountReceive.GLAccID,
                Credit = (decimal)TotalAmountDue,
                BPAcctID = ICP.CustomerID,
            });
            //Insert 
            List<AccountBalance> accountBalance = new();
            glAccD.Balance -= (decimal)TotalAmountDue;
            accountBalance.Add(new AccountBalance
            {
                JEID = journalEntry.ID,
                PostingDate = ICP.PostingDate,
                Origin = docType.ID,
                OriginNo = ICP.InvoiceNumber,
                OffsetAccount = glAccD.Code,
                Details = douTypeID.Name + " - " + glAccD.Code,
                CumulativeBalance = glAccD.Balance,
                Credit = (decimal)TotalAmountDue,
                LocalSetRate = (decimal)ICP.LocalSetRate,
                GLAID = accountReceive.GLAccID,
                BPAcctID = ICP.CustomerID,
                Creator = ICP.UserID,
                Effective = EffectiveBlance.Credit
            });
            foreach (var multiincom in ICP.MultiIncommings)
            {

                var accountSelect = _context.PaymentMeans.Find(multiincom.PaymentMeanID);
                var glAcc = _context.GLAccounts.FirstOrDefault(w => w.ID == accountSelect.AccountID) ?? new GLAccount();
                journalEntryDetail.Add(new JournalEntryDetail
                {
                    JEID = journalEntry.ID,
                    Type = Financials.Type.GLAcct,
                    ItemID = accountSelect.AccountID,
                    Debit = multiincom.AmmountSys,
                    BPAcctID = ICP.CustomerID,
                });
                //Insert 
                glAcc.Balance += multiincom.AmmountSys;
                accountBalance.Add(new AccountBalance
                {
                    JEID = journalEntry.ID,
                    PostingDate = ICP.PostingDate,
                    Origin = docType.ID,
                    OriginNo = ICP.InvoiceNumber,
                    OffsetAccount = glAcc.Code,
                    Details = douTypeID.Name + " - " + glAcc.Code,
                    CumulativeBalance = glAcc.Balance,
                    Debit = multiincom.AmmountSys,
                    LocalSetRate = (decimal)ICP.LocalSetRate,
                    GLAID = accountSelect.AccountID,
                    Effective = EffectiveBlance.Debit
                });
                _context.Update(glAcc);
                _context.Update(glAccD);
            }
            #region old account incommingpayment

            #endregion

            _context.UpdateRange(accountBalance);
            _context.UpdateRange(journalEntryDetail);
            _context.SaveChanges();
        }
        public void IncomingPaymentSeriesAccountingCancel(string id, int seriesDID)
        {
            //var incomingCus = _context.IncomingPaymentCustomers.FirstOrDefault(w=>w.SeriesDID == seriesDID);

            var ICP = _context.IncomingPayments.Include(x => x.MultiIncommings).FirstOrDefault(i => i.SeriesDID == seriesDID);
            var checkpay = _context.IncomingPaymentDetails.Where(x => x.IncomingPaymentID == ICP.IncomingPaymentID && x.Delete == false).ToList();
            var TotalAmountDue = checkpay.Sum(i => i.Totalpayment);
            var docType = _context.DocumentTypes.Find(ICP.DocTypeID);
            var company = _context.Company.Find(ICP.CompanyID);
            // var series = _context.SeriesDetails.Find(ICP.SeriesDetailID);
            JournalEntry journalEntry = new();
            SeriesDetail seriesDetail = new();
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
            journalEntry.Creator = ICP.UserID;
            journalEntry.TransNo = Sno;
            journalEntry.PostingDate = ICP.PostingDate;
            journalEntry.DocumentDate = ICP.DocumentDate;
            journalEntry.DueDate = ICP.PostingDate;
            journalEntry.SSCID = company.SystemCurrencyID;
            journalEntry.LLCID = ICP.LocalCurID;
            journalEntry.CompanyID = ICP.CompanyID;
            journalEntry.LocalSetRate = (decimal)ICP.LocalSetRate;
            journalEntry.SeriesDID = seriesDetail.ID;
            journalEntry.Remarks = defaultJE.Name + " " + Sno;
            _context.Update(journalEntry);
            _context.SaveChanges();
            //IssuseInstock
            // AccountReceice
            var accountReceive = _context.BusinessPartners.FirstOrDefault(w => w.ID == ICP.CustomerID);
            var glAccD = _context.GLAccounts.FirstOrDefault(w => w.ID == accountReceive.GLAccID);
            List<JournalEntryDetail> journalEntryDetail = new();
            journalEntryDetail.Add(new JournalEntryDetail
            {
                JEID = journalEntry.ID,
                Type = Financials.Type.BPCode,
                ItemID = accountReceive.GLAccID,
                Debit = (decimal)TotalAmountDue,
                BPAcctID = ICP.CustomerID,
            });
            //Insert 
            List<AccountBalance> accountBalance = new();
            glAccD.Balance += (decimal)TotalAmountDue;
            accountBalance.Add(new AccountBalance
            {
                JEID = journalEntry.ID,
                PostingDate = ICP.PostingDate,
                Origin = docType.ID,
                OriginNo = ICP.InvoiceNumber,
                OffsetAccount = glAccD.Code,
                Details = douTypeID.Name + " - " + glAccD.Code,
                CumulativeBalance = glAccD.Balance,
                Debit = (decimal)TotalAmountDue,
                LocalSetRate = (decimal)ICP.LocalSetRate,
                GLAID = accountReceive.GLAccID,
                BPAcctID = ICP.CustomerID,
                Creator = ICP.UserID,
                Effective = EffectiveBlance.Debit
            });

            foreach (var multiincom in ICP.MultiIncommings)
            {
                var accountSelect = _context.PaymentMeans.Find(multiincom.PaymentMeanID);
                var glAcc = _context.GLAccounts.FirstOrDefault(w => w.ID == accountSelect.AccountID) ?? new GLAccount();
                journalEntryDetail.Add(new JournalEntryDetail
                {
                    JEID = journalEntry.ID,
                    Type = Financials.Type.GLAcct,
                    ItemID = accountSelect.AccountID,
                    Credit = multiincom.AmmountSys,
                    BPAcctID = ICP.CustomerID,
                });
                //Insert 
                glAcc.Balance += multiincom.AmmountSys;
                accountBalance.Add(new AccountBalance
                {
                    JEID = journalEntry.ID,
                    PostingDate = ICP.PostingDate,
                    Origin = docType.ID,
                    OriginNo = ICP.InvoiceNumber,
                    OffsetAccount = glAcc.Code,
                    Details = douTypeID.Name + " - " + glAcc.Code,
                    CumulativeBalance = glAcc.Balance,
                    Credit = multiincom.AmmountSys,
                    LocalSetRate = (decimal)ICP.LocalSetRate,
                    GLAID = accountSelect.AccountID,
                    Effective = EffectiveBlance.Credit
                });
                _context.Update(glAcc);
                _context.Update(glAccD);
            }
            _context.UpdateRange(accountBalance);
            _context.UpdateRange(journalEntryDetail);
            _context.SaveChanges();
        }
        //
        public void IncomingPaymentCancelPOS(int InpaymentID, int? PaymentmeansID = 0)
        {
            var ICP = _context.IncomingPayments.FirstOrDefault(i => i.IncomingPaymentID == InpaymentID);
            var checkpay = _context.IncomingPaymentDetails.Where(x => x.IncomingPaymentID == ICP.IncomingPaymentID && x.Delete == false).ToList();
            var TotalAmountDue = checkpay.Sum(i => i.Totalpayment);
            var docType = _context.DocumentTypes.Find(ICP.DocTypeID);
            var company = _context.Company.Find(ICP.CompanyID);
            // var series = _context.SeriesDetails.Find(ICP.SeriesDetailID);
            JournalEntry journalEntry = new();
            SeriesDetail seriesDetail = new();
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
            journalEntry.Creator = ICP.UserID;
            journalEntry.TransNo = Sno;
            journalEntry.PostingDate = ICP.PostingDate;
            journalEntry.DocumentDate = ICP.DocumentDate;
            journalEntry.DueDate = ICP.PostingDate;
            journalEntry.SSCID = company.SystemCurrencyID;
            journalEntry.LLCID = ICP.LocalCurID;
            journalEntry.CompanyID = ICP.CompanyID;
            journalEntry.LocalSetRate = (decimal)ICP.LocalSetRate;
            journalEntry.SeriesDID = seriesDetail.ID;
            journalEntry.Remarks = defaultJE.Name + " " + Sno;
            _context.Update(journalEntry);
            _context.SaveChanges();
            //IssuseInstock
            // AccountReceice
            var accountReceive = _context.BusinessPartners.FirstOrDefault(w => w.ID == ICP.CustomerID) ?? new HumanResources.BusinessPartner();
            var glAccD = _context.GLAccounts.FirstOrDefault(w => w.ID == accountReceive.GLAccID) ?? new GLAccount();
            List<JournalEntryDetail> journalEntryDetail = new();
            journalEntryDetail.Add(new JournalEntryDetail
            {
                JEID = journalEntry.ID,
                Type = Financials.Type.BPCode,
                ItemID = accountReceive.GLAccID,
                Debit = (decimal)TotalAmountDue,
                BPAcctID = ICP.CustomerID,
            });
            //Insert 
            List<AccountBalance> accountBalance = new();
            glAccD.Balance += (decimal)TotalAmountDue;
            accountBalance.Add(new AccountBalance
            {
                JEID = journalEntry.ID,

                PostingDate = ICP.PostingDate,
                Origin = docType.ID,
                OriginNo = ICP.InvoiceNumber,
                OffsetAccount = glAccD.Code,
                Details = douTypeID.Name + " - " + glAccD.Code,
                CumulativeBalance = glAccD.Balance,
                Debit = (decimal)TotalAmountDue,
                LocalSetRate = (decimal)ICP.LocalSetRate,
                GLAID = accountReceive.GLAccID,
                BPAcctID = ICP.CustomerID,
                Creator = ICP.UserID,
                Effective = EffectiveBlance.Debit
            });
            // var accountSelected = _context.PaymentMeans.Find(ICP.PaymentMeanID);
            var accountSelected = _context.PaymentMeans.Find(PaymentmeansID) ?? new Banking.PaymentMeans();
            var glAccC = _context.GLAccounts.FirstOrDefault(w => w.ID == accountSelected.AccountID) ?? new GLAccount();
            journalEntryDetail.Add(new JournalEntryDetail
            {
                JEID = journalEntry.ID,
                Type = Financials.Type.BPCode,
                ItemID = accountSelected.AccountID,
                Credit = (decimal)TotalAmountDue,
                BPAcctID = ICP.CustomerID,
            });
            //Insert 
            glAccC.Balance -= (decimal)TotalAmountDue;
            accountBalance.Add(new AccountBalance
            {
                JEID = journalEntry.ID,

                PostingDate = ICP.PostingDate,
                Origin = docType.ID,
                OriginNo = ICP.InvoiceNumber,
                OffsetAccount = glAccC.Code,
                Details = douTypeID.Name + " - " + glAccC.Code,
                CumulativeBalance = glAccC.Balance,
                Credit = (decimal)TotalAmountDue,
                LocalSetRate = (decimal)ICP.LocalSetRate,
                GLAID = accountSelected.AccountID,
                BPAcctID = ICP.CustomerID,
                Creator = ICP.UserID,
                Effective = EffectiveBlance.Credit
            });
            _context.Update(glAccC);
            _context.Update(glAccD);
            _context.UpdateRange(accountBalance);
            _context.UpdateRange(journalEntryDetail);
            _context.SaveChanges();
        }
         private List<Currency> Currency()
        {
            var data = _context.Currency.ToList();
            data.Insert(0, new Currency
            {
                Description = data.FirstOrDefault().Description,

                ID = data.FirstOrDefault().ID,
            });
            return data;
        }
        public async Task<IncomingPamentCancelViewModel> GetPaymentOrder(int id=0,string invnumber="",int seriesID=0)
        {
            IncomingPamentCancelViewModel icomingPayment=new IncomingPamentCancelViewModel();

           var ICP = id==0? await _context.IncomingPaymentOrders.FirstOrDefaultAsync(i => i.InvoiceNumber == invnumber && i.SeriesID == seriesID && i.Status == "open")
                          : await _context.IncomingPaymentOrders.FirstOrDefaultAsync(i => i.ID == id  && i.Status == "open");
            if (ICP != null)
            {
                var details = _context.IncomingPaymentOrderDetails.Include(i => i.Currency).Where(i => i.IncomingPaymentOrderID == ICP.ID).ToList();
                var bus     = _context.BusinessPartners.FirstOrDefault(i => i.ID == ICP.CustomerID) ?? new Models.Services.HumanResources.BusinessPartner();
                var user    = _context.UserAccounts.FirstOrDefault(i => i.ID == ICP.UserID) ?? new UserAccount();
                var payMentmeans = _context.PaymentMeans.FirstOrDefault(i => i.ID == ICP.PaymentMeanID) ?? new PaymentMeans();
                var GLAccount    = _context.GLAccounts.FirstOrDefault(i => i.ID == payMentmeans.AccountID) ?? new Models.Services.ChartOfAccounts.GLAccount();

                 icomingPayment = new IncomingPamentCancelViewModel
                {
                    IncomingPaymentID = ICP.ID,
                    CustomerID  = ICP.CustomerID,
                    CustomerName= bus.Name,
                    Type        = ICP.Type,  
                    Ref_No      = ICP.Ref_No,
                    UserID      = ICP.UserID,
                    UserName    = user.Username,
                    DocTypeID   = ICP.DocTypeID,
                    InvoiceNumber = ICP.InvoiceNumber,
                    PostingDate  = ICP.PostingDate,
                    DocumentDate = ICP.DocumentDate,
                    SeriesID    = ICP.SeriesID,
                    SeriesDID   = ICP.SeriesDID,
                    CompanyID   = ICP.CompanyID,
                    Status      = ICP.Status,
                    PaymentMeanID = ICP.PaymentMeanID,
                    BranchID    = ICP.BranchID,
                    TotalAmountDue = ICP.TotalAmountDue,
                    Number = ICP.Number,
                    Remark = ICP.Remark,
                    TotalApplied = (double)ICP.TotalApplied,
                    
                   // IncomingPaymentDetails = details,
                    IncomingPaymentOrderDetails =details,
                    LocalCurID      = ICP.LocalCurID,
                    LocalSetRate    = ICP.LocalSetRate,
                    BusinessPartner = bus,
                    UserAccount     = user,
                    PaymentMeans    = payMentmeans,
                    GLAccount       = GLAccount,
                    MultPayIncommings = await (from incom in _context.MultiIncomingPaymentOrders.Where(i => i.IncomingPaymentOrderID == ICP.ID)
                                         join paymean in _context.PaymentMeans on incom.PaymentMeanID equals paymean.ID
                                         join glAcc in _context.GLAccounts on paymean.AccountID equals glAcc.ID
                                         let ex = _context.ExchangeRates.FirstOrDefault(x => x.ID == Currency().FirstOrDefault().ID) ?? new ExchangeRate()
                                         select new MultiIncomming
                                         {
                                             ID = incom.ID,
                                             LineID = incom.ID,
                                             IncomingPaymentID = incom.ID,
                                             PaymentMeanID = incom.PaymentMeanID,
                                             SCRate = (decimal)ex.SetRate,
                                             Amount = incom.Amount,
                                             AmmountSys = incom.AmmountSys,
                                             PMName = paymean.Type,
                                             Currency = Currency().GroupBy(i => i.ID).Select(i => i.FirstOrDefault()).Select(i => new SelectListItem
                                             {
                                                 Text = i.Description, //+ i.Name,
                                                 Value = i.ID.ToString(),
                                                 Selected = incom.CurrID == i.ID,
                                             }).ToList(),
                                             ExchangeRate = (decimal)ex.Rate,

                                             GLAccID = glAcc.ID,
                                             CurrID = Currency().FirstOrDefault().ID

                                         }).ToListAsync(),
                };
                return icomingPayment;
            }
            return icomingPayment;
        }
    }
}
