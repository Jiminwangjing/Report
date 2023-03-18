using CKBS.AppContext;
using CKBS.Models.Services.Administrator.General;
using CKBS.Models.Services.Administrator.SystemInitialization;
using CKBS.Models.Services.ChartOfAccounts;
using CKBS.Models.Services.Financials;
using KEDI.Core.Models.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Type = CKBS.Models.Services.Financials.Type;
using KEDI.Core.Premise.Authorization;
using KEDI.Core.Premise.Repository;
using CKBS.Models.Services.Account;

namespace CKBS.Controllers
{
    [Privilege]
    public class FinancialsController : Controller
    {
        private readonly DataContext _context;
          private readonly UserManager _userManager;
        public FinancialsController(DataContext dataContext, UserManager userManager)
        {
            _context = dataContext;
             _userManager=userManager;
        }

        [Privilege("JE005")]
        public IActionResult JournalEntry()
        {
            ViewBag.JournalEntry = "highlight";
            return View(CreateJETemplates(10));
        }

        int GetCompanyID()
        {
            _ = int.TryParse(User.FindFirstValue("CompanyID"), out int _companyId);
            return _companyId;
        }
         public async Task<IActionResult> GetBranch()
        {
            var listbranch= (from mbr in _context.MultiBrands.Where(s=> s.UserID==_userManager.CurrentUser.ID  && s.Active==true)
                            join br in  _context.Branches on mbr.BranchID equals br.ID
                            select new MultiBrand
                            {
                                ID=br.ID,
                                Name = br.Name,
                                Active =false,
                                
                            }).ToList();
                    listbranch.ForEach(i=>{
                        if(i.ID==_userManager.CurrentUser.BranchID)
                            i.Active=true;

                    });
            return Ok(await Task.FromResult(listbranch));
        }

        private JournalEntry CreateJETemplates(int count)
        {
            var series = (from dt in _context.DocumentTypes.Where(w => w.Code == "JE")
                          join sr in _context.Series.Where(w => !w.Lock && w.CompanyID == GetCompanyID()) on dt.ID equals sr.DocuTypeID

                          select new Series
                          {
                              ID = sr.ID,
                              Name = sr.Name,
                              Default = sr.Default,
                              NextNo = sr.NextNo
                          }).ToList();
            JournalEntry journalEntry = new();
            var sysCurr = _context.Currency.FirstOrDefault(i => i.ID == GetCompany().SystemCurrencyID);
            if (series.Count > 0)
            {
                journalEntry = new JournalEntry
                {
                    SeriesID = series.FirstOrDefault(w => w.Default == true).ID,
                    Number = series.FirstOrDefault(w => w.Default == true).NextNo,
                    PostingDate = DateTime.Today,
                    DueDate = DateTime.Today,
                    DocumentDate = DateTime.Today,
                    SysCurrency = sysCurr.Symbol,
                    JournalEntryDetails = new List<JournalEntryDetail>()

                };
                List<JournalEntryDetail> JournalEntryDetails = new();
                for (int i = 1; i <= count; i++)
                {
                    JournalEntryDetails.Add(new JournalEntryDetail
                    {
                        LineID = i,
                        ItemID = 0,
                        CodeTM = "",
                        NameTM = "",
                        DebitTM = "",
                        CreditTM = "",
                        Remarks = "",
                    });
                }
                journalEntry.Series = series;
                journalEntry.JournalEntryDetails = JournalEntryDetails;
            }
            return journalEntry;
        }

        public IActionResult GetActiveGLAccounts()
        {
            var acc = _context.GLAccounts.Where(w => w.IsActive && !w.IsControlAccount && w.CompanyID == GetCompanyID());
            return Ok(acc);
        }

        private void ValidateSummary(dynamic master, IEnumerable<dynamic> details)
        {
            var postingPeriod = _context.PostingPeriods.Where(w => w.PeroidStatus == PeroidStatus.Unlocked).ToList();
            if (postingPeriod.Count <= 0)
            {
                ModelState.AddModelError("PostingDate", "PeroiStatus is closed or locked");
            }
            else
            {
                bool isValidPostingDate = false,
                    isValidDueDate = false,
                    isValidDocumentDate = false;
                foreach (var item in postingPeriod)
                {
                    if (DateTime.Compare(master.PostingDate, item.PostingDateFrom) >= 0 && DateTime.Compare(master.PostingDate, item.PostingDateTo) <= 0)
                    {
                        isValidPostingDate = true;
                    }

                    if (DateTime.Compare(master.DueDate, item.DueDateFrom) >= 0 && DateTime.Compare(master.DueDate, item.DueDateTo) <= 0)
                    {
                        isValidDueDate = true;
                    }

                    if (DateTime.Compare(master.DocumentDate, item.DocuDateFrom) >= 0 && DateTime.Compare(master.DocumentDate, item.DocuDateTo) <= 0)
                    {
                        isValidDocumentDate = true;
                    }
                }
                if (!isValidPostingDate)
                {
                    ModelState.AddModelError("PostingDate", "PostingDate is closed or locked");
                }
                if (!isValidDueDate)
                {
                    ModelState.AddModelError("DueDate", "DueDate is closed or locked");
                }
                if (!isValidDocumentDate)
                {
                    ModelState.AddModelError("DocumentDate", "DocumentDate is closed or locked");
                }
            }
            if (master.TotalDebit != master.TotalCredit)
            {
                ModelState.AddModelError("TotalDebit", "TotalDebit and TotalCredit not equals !");
            }
            var d = details.ToList();
            if (d.Count <= 0)
            {
                ModelState.AddModelError("ItemID", "Journal Entry Details is required !");
            }
            foreach (var jed in details)
            {
                if (jed.ItemID == null)
                {
                    ModelState.AddModelError("ItemID", "G/L Account field is required !");
                }
                if (jed.Debit != 0 && jed.Credit != 0)
                {
                    ModelState.AddModelError("Debit", "Allow only Debit or Credit exclusively !");
                }
                if (jed.Debit == 0 && jed.Credit == 0)
                {
                    ModelState.AddModelError("Credit", "Debit or Credit is required !");
                }
            }
        }

        private int GetUserID()
        {
            _ = int.TryParse(User.FindFirst("UserID").Value, out int _id);
            return _id;
        }

        private Company GetCompany()
        {
            var com = (from us in _context.UserAccounts.Where(w => w.ID == GetUserID())
                       join br in _context.Branches on us.BranchID equals br.ID
                       join co in _context.Company on br.CompanyID equals co.ID
                       select co
                       ).FirstOrDefault();
            return com;
        }

        public IActionResult SubmitJournal(string data)
        {
            JournalEntry journalEntry = JsonConvert.DeserializeObject<JournalEntry>(data);
            ModelMessage msg = new();
            SeriesDetail seriesDetail = new();
            var douTypeID = _context.DocumentTypes.FirstOrDefault(w => w.Code == "JE");
            var localSetRate = _context.ExchangeRates.FirstOrDefault(w => w.CurrencyID == GetCompany().LocalCurrencyID).SetRate;
            var series = _context.Series.FirstOrDefault(w => w.ID == journalEntry.SeriesID);
            journalEntry.TotalCredit = journalEntry.JournalEntryDetails.Sum(s => s.Credit);
            journalEntry.TotalDebit = journalEntry.JournalEntryDetails.Sum(s => s.Debit);
            ValidateSummary(journalEntry, journalEntry.JournalEntryDetails);
            using (var t = _context.Database.BeginTransaction())
            {
                seriesDetail.Number = series.NextNo;
                seriesDetail.SeriesID = journalEntry.SeriesID;
                _context.SeriesDetails.Update(seriesDetail);
                _context.SaveChanges();
                var seriesDetailID = seriesDetail.ID;
                string Sno = series.NextNo;
                long No = long.Parse(Sno);
                series.NextNo = Convert.ToString(No + 1);
                if (No > long.Parse(series.LastNo))
                {
                    ModelState.AddModelError("SumInvoice", "Your Journal Entry has reached the limitation!!");
                }
                if (ModelState.IsValid)
                {
                    journalEntry.DouTypeID = douTypeID.ID;
                    journalEntry.Creator = GetUserID();
                    journalEntry.TransNo = journalEntry.Number;
                    journalEntry.SSCID = GetCompany().SystemCurrencyID;
                    journalEntry.LLCID = GetCompany().LocalCurrencyID;
                    journalEntry.CompanyID = GetCompany().ID;
                    journalEntry.LocalSetRate = (decimal)localSetRate;
                    journalEntry.SeriesDID = seriesDetailID;
                    journalEntry.Number = seriesDetail.Number;
                    journalEntry.Remarks = journalEntry.Remarks;
                    _context.JournalEntries.Update(journalEntry);
                    _context.SaveChanges();
                    foreach (var jed in journalEntry.JournalEntryDetails)
                    {
                        //update GLAccount

                        AccountBalance accountBalance = new();
                        var glAcc = _context.GLAccounts.FirstOrDefault(w => w.ID == jed.ItemID);
                        decimal balance = 0;
                        if (jed.Credit > 0)
                        {
                            accountBalance.JEID = journalEntry.ID;
                            balance = jed.Credit * (-1);
                            accountBalance.Effective = EffectiveBlance.Credit;
                        }
                        else if (jed.Debit > 0)
                        {
                            accountBalance.JEID = journalEntry.ID;
                            balance = jed.Debit;
                            accountBalance.Effective = EffectiveBlance.Debit;
                        }
                        glAcc.Balance += balance;

                        //insert account balance
                        decimal cumbalance = glAcc.Balance;
                        accountBalance.PostingDate = journalEntry.PostingDate;
                        accountBalance.Origin = journalEntry.DouTypeID;
                        accountBalance.OriginNo = journalEntry.TransNo;
                        accountBalance.OffsetAccount = glAcc.Code;
                        accountBalance.Details = douTypeID.Name + " - " + glAcc.Code;
                        accountBalance.CumulativeBalance = cumbalance;
                        accountBalance.Debit = jed.Debit;
                        accountBalance.Credit = jed.Credit;
                        accountBalance.LocalSetRate = journalEntry.LocalSetRate;
                        accountBalance.GLAID = jed.ItemID;
                        accountBalance.Remarks = jed.Remarks;

                        jed.Type = Type.GLAcct;
                        jed.JEID = journalEntry.ID;

                        _context.AccountBalances.Update(accountBalance);
                        _context.Update(glAcc);
                        _context.SaveChanges();
                    }
                    _context.JournalEntryDetails.UpdateRange(journalEntry.JournalEntryDetails);
                    _context.SaveChanges();
                    t.Commit();
                    ModelState.AddModelError("success", "Item save successfully.");
                    msg.Approve();
                }
            }
            return Ok(new { Model = msg.Bind(ModelState) });
        }
    }
}
