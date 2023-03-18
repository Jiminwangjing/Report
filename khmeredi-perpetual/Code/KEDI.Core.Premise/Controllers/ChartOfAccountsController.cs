using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using CKBS.AppContext;
using CKBS.Models.Services.Banking;
using CKBS.Models.Services.ChartOfAccounts;
using System.Security.Claims;
using KEDI.Core.Premise.Authorization;
using CKBS.Models.Services.Inventory.Category;
using CKBS.Models.Services.Inventory;
using KEDI.Core.Models.Validation;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using CKBS.Models.Services.Financials;
using System.Threading.Tasks;
using KEDI.Core.Premise.Models.Services.ChartOfAccounts;
using Microsoft.EntityFrameworkCore;
using KEDI.Core.Helpers.Enumerations;

namespace CKBS.Controllers
{
    [Privilege]
    public class ChartOfAccountsController : Controller
    {
        private readonly DataContext _context;

        public ChartOfAccountsController(DataContext dataContext)
        {
            _context = dataContext;
        }

        public async Task<IActionResult> Category(int id = 0)
        {
            ViewBag.Category = "highlight";
            ViewBag.TableRowActive = "active";

            int parentId = id;
            var categories = await GetCategoriesAsync();
            var glAccount = await _context.GLAccounts.FindAsync(id);
            if (parentId <= 0)
            {
                ViewBag.Disabled = "disabled";
                if (categories.Count() > 0)
                {
                    parentId = categories.FirstOrDefault().ID;
                }
            }

            return View(new GLAccountViewModel
            {
                GLAccount = glAccount ?? new GLAccount(),
                Categories = categories,
                Details = await GetDetailsAsync(parentId)
            });
        }

        public async Task<IEnumerable<GLAccount>> GetCategoriesAsync()
        {
            return await _context.GLAccounts.Where(m => m.Level == 1 && m.CompanyID == GetCompanyID()).ToListAsync();
        }

        public async Task<IEnumerable<GLAccount>> GetDetailsAsync(int parentId)
        {
            return await _context.GLAccounts
                .Where(m => m.ParentId == parentId && m.Level > 1 && m.CompanyID == GetCompanyID())
                .ToListAsync();
        }

        public IActionResult Getglcode(int mainId)
        {
            var gl = _context.GLAccounts.Where(x => x.Level == 4 && x.MainParentId == mainId).ToList();
            return Ok(gl);
        }

        public async Task<IActionResult> CreateCategoryByDefault()
        {
            string[] cnames = new string[] { "Assets", "Liabilities", "Capital & Reserves", "Turnover", "Cost of Sales",
                "Operating Costs", "Non-Operating Income & Expenditure", "Taxation & Extraordinary Items" };
            var catalogTypes = EnumHelper.ToDictionary<CategoryType>();
            var glAccs = await GetCategoriesAsync();
            for (short i = 0; i < 8; i++)
            {
                CategoryType cType = (CategoryType)catalogTypes.Keys.ToArray()[i];
                var gl = glAccs.ToArray()[i] ?? new GLAccount
                {
                    Code = (_context.GLAccounts.Count() + 1).ToString().PadRight(15, '0'),
                    Name = cnames[i],
                    ExternalCode = "",
                    Level = 1,
                    IsTitle = false,
                    CompanyID = GetCompanyID(),
                    ChangeLog = DateTime.Now,
                    CategoryType = cType
                };
                gl.CategoryType = cType;
                await CreateCategoriesAsync(gl);
            }
            return RedirectToAction(nameof(Category));
        }

        private int GetCompanyID()
        {
            _ = int.TryParse(User.FindFirstValue("CompanyID"), out int _companyId);
            return _companyId;
        }

        private async Task CreateCategoriesAsync(GLAccount gl)
        {
            _context.GLAccounts.Update(gl);
            await _context.SaveChangesAsync();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateCategory(GLAccountViewModel viewModel)
        {
            viewModel.GLAccount.IsTitle = false;
            viewModel.GLAccount.ChangeLog = DateTime.Now;
            if (ModelState.IsValid && viewModel.GLAccount.ID > 0)
            {
                _context.Update(viewModel.GLAccount);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Category));
        }

        private async Task<GLAccountViewModel> CreateGLAccountViewModelAsync(int glAccountId)
        {
            return new GLAccountViewModel
            {
                GLAccount = _context.GLAccounts.Find(glAccountId) ?? new GLAccount(),
                Categories = await GetCategoriesAsync(),
                Currencies = new SelectList(GetCurrencies(), "ID", "Description"),
                Details = new List<GLAccount>()
            };
        }

        public IActionResult Categories()
        {
            return View();
        }

        public IEnumerable<Currency> GetCurrencies()
        {
            _ = int.TryParse(User.FindFirst("CompanyID").Value, out int _companyId);
            var currencies = (from c in _context.Company.Where(c => !c.Delete && c.ID == _companyId)
                              join cr in _context.Currency.Where(cr => !cr.Delete)
                              on c.SystemCurrencyID equals cr.ID
                              select new Currency
                              {
                                  ID = cr.ID,
                                  Symbol = cr.Description
                              }).ToList() ?? new List<Currency>();
            return currencies;
        }

        [Privilege("CA004")]
        public async Task<IActionResult> Detail(int? id)
        {
            ViewBag.ChartOfAccounts = "highlight";
            ViewBag.TableRowActive = "active";
            //.gllavel = new SelectList(Getglcode(mainId), "ID", "Code");
            var categories = await GetCategoriesAsync();
            var glAccount = _context.GLAccounts.Find(id) ?? new GLAccount();
            var currencies = GetCurrencies();
            if (glAccount.Level == 1 && glAccount.IsTitle)
            {
                currencies = new List<Currency>();
            }

            return View(new GLAccountViewModel
            {
                GLAccount = glAccount ?? new GLAccount(),
                Currencies = new SelectList(currencies, "ID", "Description"),
                Categories = categories,
            });
        }

        public IActionResult CreateDetailByCategory(int parentId)
        {
            var category = _context.GLAccounts.Find(parentId);
            var details = _context.GLAccounts.Where(gl => gl.ParentId == category.ID);
            string code = string.Format($"{category.Level}{details.Count() + 1}");
            var __glAccount = new GLAccount
            {
                Code = code,
                ParentId = category.ID,
                Level = category.Level + 1
            };
            var __currencies = __glAccount.IsTitle ? GetCurrencies() : new List<Currency>();
            var __obj = new { GLAccount = __glAccount, Currencies = new SelectList(__currencies, "ID", "Name") };
            return Ok(__obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateGLAccount(GLAccountViewModel viewModel)
        {
            var title = Request.Form["IsTitle"];
            if (viewModel.GLAccount.ID == 0)
            {
                var account = _context.GLAccounts.FirstOrDefault(w => w.Code == viewModel.GLAccount.Code) ?? new GLAccount();
                if (account.ID > 0)
                {
                    ViewBag.Code = "This code already in use !";
                    return View(nameof(Detail), await CreateGLAccountViewModelAsync(viewModel.GLAccount.ID));
                }
            }
            else
            {
                var account = _context.GLAccounts.FirstOrDefault(w => w.Code == viewModel.GLAccount.Code && w.ID != viewModel.GLAccount.ID) ?? new GLAccount();
                if (account.ID > 0)
                {
                    ViewBag.Code = "This code already in use !";
                    return View(nameof(Detail), await CreateGLAccountViewModelAsync(viewModel.GLAccount.ID));
                }
            }
            if (ModelState.IsValid)
            {
                var acc = viewModel.GLAccount;
                acc.CompanyID = GetCompanyID();
                acc.IsTitle = Convert.ToBoolean(title);
                if (acc.ID == 0)
                {
                    if (acc.IsTitle == true)
                    {
                        acc.IsActive = false;
                    }
                    else
                    {
                        acc.IsTitle = false;
                        acc.IsActive = true;
                    }
                    acc.ChangeLog = DateTime.Now;
                    _context.GLAccounts.Add(acc);
                }
                else
                {
                    if (acc.IsTitle == true)
                    {
                        acc.IsActive = false;
                    }
                    else
                    {
                        acc.IsTitle = false;
                        acc.IsActive = true;
                    }
                    acc.ChangeLog = DateTime.Now;
                    _context.GLAccounts.Update(acc);
                }
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Detail));
        }

        [HttpPost]
        public IActionResult GetGroup(int id)
        {
            var item = _context.GLAccounts.Find(id);
            return Ok(item);
        }
        public IActionResult GetTitleGroupWithLevel(int id)
        {
            var item = _context.GLAccounts.Where(i => i.ParentId == id).OrderBy(e => e.Code).ToList();
            return Ok(item);
        }

        public IActionResult AllGlAcc()
        {
            var currencies = GetCurrencies();
            var item = _context.GLAccounts.Where(i => i.Level > 1).ToList();
            foreach (var i in item)
            {
                var acb = _context.AccountBalances.FirstOrDefault(w => w.GLAID == i.ID) ?? new AccountBalance();
                i.TotalBalance = currencies.FirstOrDefault().Symbol + " " + string.Format("{0:#,0.000}", i.Balance);
                if (acb.ID == 0 && i.IsActive == true)
                {
                    i.Edit = true;
                }
                else
                {
                    i.Edit = false;
                }
            }
            return Ok(item);

        }
        public IActionResult GlAccBalance(int id)
        {
            var show = 0;
            var glAcc = _context.AccountBalances.ToList();
            foreach (var value in glAcc)
            {
                if (value.GLAID == id)
                {
                    show = 1;
                }
            }
            return Ok(new { show });
        }
        public IActionResult GetGlAccBalanceGLID(int id, string dateFrom, string dateTo)
        {
            var currencies = GetCurrencies();
            var glAcc = from gl in _context.AccountBalances.Where(i => i.GLAID == id)
                        select gl;

            var lineId = 1;
            List<AccountBalancesViewModel> accountBalances = new();
            foreach (var value in glAcc)
            {
                if (DateTime.TryParse(dateFrom, out DateTime _dateFrom)
                && DateTime.TryParse(dateTo, out DateTime _dateTo))
                {
                    if (_dateFrom.CompareTo(value.PostingDate) <= 0 && _dateTo.CompareTo(value.PostingDate) >= 0)
                    {
                        accountBalances.Add(new AccountBalancesViewModel
                        {
                            LineID = lineId,
                            PostingDate = value.PostingDate.ToShortDateString(),
                            Origin = value.Origin,
                            Credit = (value.Credit == 0) ? "" : currencies.FirstOrDefault().Symbol + " " + string.Format("{0:#,0.000}", value.Credit),
                            CumulativeBalance = currencies.FirstOrDefault().Symbol + " " + string.Format("{0:#,0.000}", value.CumulativeBalance),
                            Debit = (value.Debit == 0) ? "" : currencies.FirstOrDefault().Symbol + " " + string.Format("{0:#,0.000}", value.Debit),
                            Details = value.Details,
                            OffsetAccount = value.OffsetAccount,
                            ID = value.ID,
                            GLAID = value.GLAID,
                            LocalSetRate = value.LocalSetRate,
                            OriginNo = value.OriginNo,
                            Code = _context.DocumentTypes.Find(value.Origin).Code,
                        }); ;
                        lineId++;
                    }
                }

            }
            return Ok(accountBalances.OrderBy(x => x.PostingDate));
        }


        public IActionResult DeleteglAccount(int id, string code)
        {
            string[] GLProps = {
                "RevenueAccount", "InventoryAccount", "ExpenseAccount", "AllocationAccount",
                "ExchangeRateDifferencesAccount",  "CostofGoodsSoldAccount", "VarianceAccount", "PriceDifferenceAccount",
                 "NegativeInventoryAdjustmentAcct", "InventoryOffsetDecreaseAccount", "InventoryOffsetIncreaseAccount",
                 "SalesReturnsAccount", "RevenueAccountEU", "RevenueAccountForeign", "ExpenseAccountForeign", "ExchangeRateDifferencesAccount",
                 "GoodsClearingAccount", "GLDecreaseAccount", "GLIncreaseAccount", "WIPInventoryAccount", "WIPInventoryVarianceAccount",
                 "WIPOffsetPLAccount", "InventoryOffsetPLAccount", "ExpenseClearingAccount", "StockInTransitAccount", "ShippedGoodsAccount",
                 "SalesCreditAccount", "PurchaseCreditAccount", "SalesCreditAccountForeign", "PurchaseCreditAccountForeign", "SalesCreditAccountEU",
                 "PurchaseCreditAccountEU"
            };
            ModelMessage msg = new();
            var itemAccs = _context.ItemAccountings.ToList();
            foreach (var i in itemAccs)
            {
                foreach (var prop in GLProps)
                {
                    string itemPropCodeValue = i.GetType().GetProperty(prop).GetValue(i)?.ToString();
                    if (itemPropCodeValue == code)
                    {
                        CheckItemAccounting(ModelState, code, i, prop);
                    }
                }

            }
            //Freight
            var isInFreight = _context.Freights.Any(i => i.ExpenAcctID == id || i.RevenAcctID == id);
            if (isInFreight) ModelState.AddModelError("Freight", $"Account code \"{code}\" has mapped with Freight.");

            //SaleGLAccountDetermination
            var isInSaleGLAccountDetermination = _context.SaleGLAccountDeterminations.Any(i => i.GLID == id);
            if (isInSaleGLAccountDetermination) ModelState.AddModelError("SaleGLAccountDetermination", $"Account code \"{code}\" has mapped with SaleGLAccountDetermination.");

            //AccountMemberCards
            var isInAccountMemberCards = _context.AccountMemberCards.Any(i => i.CashAccID == id);
            if (isInAccountMemberCards) ModelState.AddModelError("AccountMemberCards", $"Account code \"{code}\" has mapped with AccountMemberCards");

            //TaxGroups
            var isInTaxGroup = _context.TaxGroups.Any(i => i.GLID == id);
            if (isInAccountMemberCards) ModelState.AddModelError("TaxGroup", $"Account code \"{code}\" has mapped with TaxGroup");
            //isIntbAccountBalance
            var isIntbAccountBalance = _context.AccountBalances.Any(i => i.GLAID == id);
            if (isInAccountMemberCards) ModelState.AddModelError("tbAccountBalance", $"Account code \"{code}\" already has transactions.");
            if (ModelState.IsValid)
            {
                var removeglac = _context.GLAccounts.FirstOrDefault(x => x.ID == id);
                if (removeglac == null)
                {
                    ModelState.AddModelError("notfound", $"Account code \"{code}\" not found.");
                    msg.Reject();
                }
                else
                {
                    _context.Remove(removeglac);
                    _context.SaveChanges();
                    ModelState.AddModelError("success", $"Account code \"{code}\" deleted successfully.");
                    msg.Approve();
                }

            }

            return Ok(msg.Bind(ModelState));
        }

        private void CheckItemAccounting(ModelStateDictionary modelState, string code, ItemAccounting i, string keyDictionary)
        {
            if (i.ItemGroupID != null || i.ItemGroupID > 0)
            {
                var itemGroup = _context.ItemGroup1.Find(i.ItemGroupID) ?? new ItemGroup1();
                modelState.AddModelError($"{keyDictionary}GroupItem", $"Account code \"{code}\" has mapped with Item Group 1 \"{itemGroup.Name}\"");
            }
            if (i.ItemID != null || i.ItemID > 0)
            {
                var itemMaster = _context.ItemMasterDatas.Find(i.ItemID) ?? new ItemMasterData();
                modelState.AddModelError($"{keyDictionary}Level", $"Account code \"{code}\" has mapped with Item \"{itemMaster.KhmerName}\"");
            }

        }

        ///-------------------------------------
        [HttpGet]
        public IActionResult BindRowSubType()
        {
            List<SubTypeAcount> subtype = new();
            List<SubTypeAcount> datas = new();
            var data = _context.SubTypeAcounts.Where(s => s.Delete == false).ToList();
            //  datas= data.Select(t => new SelectListItem
            //  {
            //      Value = t.ID.ToString(),
            //      Text = t.Name,
            //      Selected = t.Default == true
            //  }).ToList();
            subtype.AddRange(data);
            for (var i = 1; i <= 10; i++)
            {
                var bindsubtype = new SubTypeAcount
                {
                    LineID = DateTime.Now.Ticks.ToString(),
                    ID = 0,
                    Name = ""
                };
                subtype.Add(bindsubtype);
            }
            return Ok(subtype);

        }
        [HttpPost]
        public IActionResult SaveSubTypeAccount(List<SubTypeAcount> subTypes)
        {
            ModelMessage msg = new();

            if (ModelState.IsValid)
            {
                _context.SubTypeAcounts.UpdateRange(subTypes);
                _context.SaveChanges();
                ModelState.AddModelError("success", "Item created succussfully!");
                msg.Approve();
            }
            var data = _context.SubTypeAcounts.Where(s => s.Delete == false).ToList();
            return Ok(new { Model = msg.Bind(ModelState), Data = data });
        }

        public IActionResult GetSubTypeAcount()
        {
            var list = _context.SubTypeAcounts.Where(s => !string.IsNullOrWhiteSpace(s.Name) && s.Delete == false).ToList();

            return Ok(list);
        }

    }
}