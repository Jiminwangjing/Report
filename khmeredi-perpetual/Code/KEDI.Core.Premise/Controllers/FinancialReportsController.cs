using CKBS.AppContext;
using CKBS.Models.Services.Responsitory;
using Microsoft.AspNetCore.Mvc;
using CKBS.Models.Services.Administrator.General;
using System.Linq;
using System.Collections.Generic;
using KEDI.Core.Premise.Authorization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using CKBS.Models.Services.Account;
using Newtonsoft.Json;
using KEDI.Core.Premise.Repository;

namespace CKBS.Controllers
{
    [Privilege]
    public class FinancialReportsController : Controller
    {
        private readonly DataContext _context;
        private readonly IFinancialReports _financial;
         private readonly UserManager _userManager;
        public FinancialReportsController(DataContext dataContext, IFinancialReports financialReports,UserManager userManager)
        {
            _context = dataContext;
            _financial = financialReports;
              _userManager=userManager;
        }

        private int GetUserID()
        {
            _ = int.TryParse(User.FindFirst("UserID").Value, out int _id);
            return _id;
        }

        private Company GetCompany()
        {
            var com = (from us in _context.UserAccounts.Where(w => w.ID == GetUserID())
                       join co in _context.Company on us.CompanyID equals co.ID
                       select co
                       ).FirstOrDefault();
            return com;
        }

        public IActionResult BalanceSheet()
        {
            ViewBag.style = "fa fa-random";
            ViewBag.FinancialReports = "show";
            ViewBag.BalanceSheet = "highlight";
            return View();
        }

        public IActionResult ProfitAndLost()
        {
            ViewBag.style = "fa fa-random";
            ViewBag.FinancialReports = "show";
            ViewBag.ProfitAndLost = "highlight";
            return View();
        }

        public IActionResult TrialBalance()
        {
            ViewBag.style = "fa fa-random";
            ViewBag.FinancialReports = "show";
            ViewBag.TrialBalance = "highlight";
            return View();
        }

        public IActionResult CashFlowForTreasury()
        {
            ViewBag.style = "fa fa-random";
            ViewBag.FinancialReports = "show";
            ViewBag.CashFlowForTreasury = "highlight";
            return View();
        }
        public IActionResult SaleGrossProfit()
        {
            ViewBag.style = "fa fa-random";
            ViewBag.FinancialReports = "show";
            ViewBag.SaleGrossProfit = "highlight";
            ViewBag.Users = new SelectList(_context.UserAccounts.Where(i => !i.Delete), "ID", "Username");
            return View();
        }
        public IActionResult TransactionJournal()
        {
            ViewBag.TransactionJournal = "highlight";
            return View();
        }
        public IActionResult GerneralLedger()
        {
            ViewBag.style = "fa fa-random";
            ViewBag.FinancialReports = "show";
            ViewBag.GerneralLedger = "highlight";
            return View();
        }
        public IActionResult GetBranch()
        {
           var list = from br in _context.Branches.Where(s => !s.Delete).ToList()
                        select new MultiBrand
                        {
                            ID=br.ID,
                            BranchID =br.ID,
                            Name= br.Name,
                            UserName =_userManager.CurrentUser.Username,
                            Location = br.Location,
                            Address = br.Address,
                            Active  = true,
                        };
            return Ok(list);
        }
        public IActionResult GetPostingPeriods()
        {
            var pp = _financial.GetPostingPeriods();
            return Ok(pp);
        }

        public IActionResult GetGLAccountsActiveOnly(string date)
        {
            var accbal = _financial.GetGLAccountsActiveOnly(date, GetCompany().ID, GetCompany().SystemCurrencyID);
            return Ok(accbal);
        }

        public IActionResult GetGategories(string code)
        {
            var gategories = _financial.GetGategories(code, GetCompany().SystemCurrencyID);
            return Ok(gategories);
        }

        public IActionResult GetBalanceSheet(string date, int TypeDisplayReport, bool showZeroAcc)
        {
            var accbal = _financial.GetBalanceSheet(date, GetCompany().ID, GetCompany().SystemCurrencyID, TypeDisplayReport, showZeroAcc);

            return Ok(accbal);
        }

        public IActionResult GetGLAccountsPL(string fromDate, string toDate, int typeDisplay, bool showZeroAcc, string branchs )
        {
            List<MultiBrand> branch = JsonConvert.DeserializeObject<List<MultiBrand>>(branchs, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            var accbal = _financial.GetGLAccountsPL(fromDate, toDate, GetCompany().ID, GetCompany().SystemCurrencyID, typeDisplay, showZeroAcc, branch);
            return Ok(accbal);
        }

       public IActionResult GetGLAccountsActiveOnlyPL(string fromDate, string toDate, string branchs)
        {
            List<MultiBrand> branch = JsonConvert.DeserializeObject<List<MultiBrand>>(branchs, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            var accbal = _financial.GetGLAccountsActiveOnlyPL(fromDate, toDate, GetCompany().ID, GetCompany().SystemCurrencyID, branch);
            return Ok(accbal);
        }

        public IActionResult GetGategoriesPL(string code)
        {
            var gategories = _financial.GetGategoriesPL(code, GetCompany().SystemCurrencyID);
            return Ok(gategories);
        }

        public IActionResult GetBusPartnersAll(string type)
        {
            var allBP = _financial.GetBusPartnersAll(type);
            return Ok(allBP);
        }

        public IActionResult GetBusPartners(
            string VType, string CType,
            int fromId, int toId,
            string fromDate, string toDate,
            int displayType, bool showZeroAcc, bool showGla)
        {
            var BPs = _financial.GetBusPartners(
                VType, CType, fromId,
                toId, fromDate, toDate,
                GetCompany().SystemCurrencyID, GetCompany().ID,
                displayType, showZeroAcc, showGla);
            return Ok(BPs);
        }

        public IActionResult GetCashFlowForTreasuryReport(string fromDate, string toDate)
        {
            var data = _financial.GetCashFlowForTreasuryReport(fromDate, toDate, GetCompany());
            return Ok(data);
        }
        public async Task<IActionResult> GetSaleGrossProfitReport(string fromDate, string toDate, int userId, string timeFrom, string timeTo)
        {
            var data = await _financial.GetSaleGrossProfitReportAsync(fromDate, toDate, userId, timeFrom, timeTo, GetCompany());
            return Ok(data);
        }

        public IActionResult GetLvl1(string code)
        {
            var Lvl1 = _context.GLAccounts.FirstOrDefault(i => i.Code == code);
            return Ok(Lvl1);
        }
        public IActionResult GetTransactionReport(string journalvalue, string fromDate, string toDate)
        {
            var data = _financial.GetTransactionJournalReport(journalvalue, fromDate, toDate, GetCompany());
            return Ok(data);
        }
        public async Task<IActionResult> GetDocumentTYpe()
        {
            var list = _context.DocumentTypes.Where(s => s.Code != "PO" && s.Code != "SQ" && s.Code != "SO" && s.Code != "PQ").ToList();
            return Ok(await Task.FromResult(list));
        }

        public IActionResult GetFinancialReports(string dateFrom, string dateTo)
        {
            var data = _financial.GetGeneralLedgerReports(dateFrom, dateTo, GetCompany());
            return Ok(data);
        }

    }
}