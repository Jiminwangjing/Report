using CKBS.AppContext;
using CKBS.Models.Services.Administrator.General;
using CKBS.Models.Services.Administrator.SystemInitialization;
using CKBS.Models.Services.Responsitory;
using CKBS.Models.ServicesClass;
using KEDI.Core.Localization.Resources;
using KEDI.Core.Models.Validation;
using KEDI.Core.Premise.Models.GeneralSettingAdmin;
using KEDI.Core.Premise.Models.ProjectCostAnalysis;
using KEDI.Core.Premise.Models.Sale;
using KEDI.Core.Premise.Models.Services.Banking;
using KEDI.Core.Premise.Models.ServicesClass.GeneralSettingAdminView;
using KEDI.Core.Premise.Models.SolutionDataManagement;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Controllers
{
    public class ProjectCostAnalysisController : Controller
    {
        private readonly DataContext _context;
        private readonly IProjectCostAnalysisRepository _projRepo;
        private readonly CultureLocalizer _culLocal;
        public ProjectCostAnalysisController(DataContext context, IProjectCostAnalysisRepository projectCost, CultureLocalizer cultureLocalizer)
        {
            _context = context;
            _projRepo = projectCost;
            _culLocal = cultureLocalizer;
        }
        public IActionResult Index()
        {
            ViewBag.ProjectCostAS = "highlight";
            var seriesPA = (from dt in _context.DocumentTypes.Where(i => i.Code == "PA")
                            join sr in _context.Series.Where(w => !w.Lock) on dt.ID equals sr.DocuTypeID
                            select new SeriesInPurchasePoViewModel
                            {
                                ID = sr.ID,
                                Name = sr.Name,
                                Default = sr.Default,
                                NextNo = sr.NextNo,
                                DocumentTypeID = sr.DocuTypeID,
                                SeriesDetailID = _context.SeriesDetails.Where(i => i.SeriesID == sr.ID).FirstOrDefault() == null ? 0 :
                                _context.SeriesDetails.Where(i => i.SeriesID == sr.ID).FirstOrDefault().ID
                            }).ToList();
            return View(new { seriesPA, genSetting = GetGeneralSettingAdmin() });
        }

        private GeneralSettingAdminViewModel GetGeneralSettingAdmin()
        {
            Display display = _context.Displays.FirstOrDefault() ?? new Display();
            GeneralSettingAdminViewModel data = new()
            {
                Display = display
            };
            return data;
        }

        [HttpGet]
        public IActionResult GetCustomer(int? id)
        {
            if (id != null)
            {
                return Ok(_context.BusinessPartners.FirstOrDefault(c => c.ID == id));
            }
            else
            {
                var cus = _context.BusinessPartners.Where(x => x.Delete == false && x.Type == "Customer").ToList();
                return Ok(cus);
            }
        }
        [HttpGet]
        public IActionResult GetWarehouse()
        {
            var ware = _context.Warehouses.Where(x => x.Delete == false && x.BranchID == int.Parse(User.FindFirst("BranchID").Value)).ToList();
            return Ok(ware);
        }
        [HttpGet]
        public IActionResult GetDefaultCurrency()
        {
            var currency = GetSystemCurrencies().First();
            return Ok(currency);
        }
        private IEnumerable<SystemCurrency> GetSystemCurrencies()
        {
            IEnumerable<SystemCurrency> currencies =
                                        (from com in _context.Company.Where(x => x.Delete == false)
                                         join c in _context.Currency.Where(x => x.Delete == false) on com.SystemCurrencyID equals c.ID
                                         select new SystemCurrency
                                         {
                                             ID = c.ID,
                                             Description = c.Description
                                         });
            return currencies;
        }
        [HttpGet]
        public IActionResult GetPriceList()
        {
            var pri = _context.PriceLists.Include(w => w.Currency).Where(W => !W.Delete);
            return Ok(pri);
        }
        [HttpGet]
        public IActionResult GetExchange()
        {
            var ex = from e in _context.ExchangeRates.Where(x => x.Delete == false)
                     join c in _context.Currency.Where(x => x.Delete == false) on e.CurrencyID equals c.ID
                     select new TpExchange
                     {
                         ID = e.ID,
                         CurID = e.CurrencyID,
                         CurName = c.Description,
                         Rate = e.Rate,
                         SetRate = e.SetRate
                     };
            return Ok(ex);
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
        [HttpGet]
        public IActionResult GetItem(int PriLi)
        {

            var ls_item = _projRepo.GetItemMaster(PriLi, GetCompany().ID).ToList();
            return Ok(new { status = "T", ls_item });
        }
        public IActionResult GetItemDetails(int PriLi, int itemId = 0, string barCode = "", int uomId = 0)
        {
            var ls_item = _projRepo.GetItemDetails(PriLi, GetCompany().ID, itemId, barCode, uomId).ToList();
            return ls_item.Count == 0 && barCode != ""
                ? Ok(new { IsError = true, Error = $"No item with this Barcode \"{barCode}\"" })
                : Ok(ls_item);
        }
        [HttpGet]
        public IActionResult GetSaleCur(int PriLi)
        {
            var prili = _context.PriceLists.FirstOrDefault(w => w.ID == PriLi);
            return Ok(prili);
        }
        public IActionResult GetFreights()
        {
            var freights = _projRepo.GetFreights();
            return Ok(freights);
        }
        [HttpGet]
        public async Task<IActionResult> GetSaleEmployee()
        {
            var list = await _projRepo.GetSaleEmployeeAsync();
            return Ok(list);
        }
        private void ValidateSummary(ProjectCostAnalysis master, IEnumerable<dynamic> details)
        {
            if (string.IsNullOrWhiteSpace(master.Name))
                ModelState.AddModelError("Name", "Please Input Name...!");
            if (master.ConTactID == 0)
                ModelState.AddModelError("ConTactID", "Please Input Contact Person ...!");
            if (master.WarehouseID == 0)
            {
                ModelState.AddModelError("WarehouseID", _culLocal["Warehouse need to be selected."]);
            }
            if (master.BranchID == 0)
            {
                ModelState.AddModelError("BranchID", "Branch not matched with warehouse.");
            }
            if (master.CusID == 0)
            {
                ModelState.AddModelError("CusID", "Please choose any customer.");
            }
            if (master.PostingDate.Year == 1)
                ModelState.AddModelError("PostingDate", "Please Input PostingDate...!");
            if (master.PostingDate.Year < master.ValidUntilDate.Year)
                ModelState.AddModelError("PostingDate", "PostingDate must bigger than ValidUntilDate or equal ValidUntilDate...!");
            if (master.ValidUntilDate.Year == 1)
                ModelState.AddModelError("ValidUntilDate", "please Input ValidUntilDate...!");
            if (master.ValidUntilDate.Year < master.DocumentDate.Year)
                ModelState.AddModelError("ValidUntilDate", "ValidUntilDate must bigger than Documentdate or equal ValidUntilDate...!");
            if (master.DocumentDate.Year == 1)
                ModelState.AddModelError("Documentate", "Please Input Documentdate...!");
            if (master.SaleEMID == 0)
                ModelState.AddModelError("SaleEmID", "Please Input Sale Emaployee...!");
            if (master.OwnerID == 0)
                ModelState.AddModelError("OwnerID", "Please Input Owner...!");
            if (!details.Any())
            {
                ModelState.AddModelError("Details", "Please choose at least one detail item.");
            }
            double subtotal = 0;
            foreach (var dt in details)
            {
                subtotal += dt.Total;
                if (dt.Qty <= 0)
                {
                    ModelState.AddModelError("Details", "Required item detail quantity greater than 0.");
                }
                if (dt.UnitPrice <= 0)
                    ModelState.AddModelError("Details", "Required item detail UnitPrice greater than 0.");
                if (dt.Cost <= 0)
                    ModelState.AddModelError("Cost", "Please Input Cost...!");
                dt.TotalSys = dt.Total * master.ExchangeRate;
                dt.TotalWTaxSys = dt.TotalWTax * master.ExchangeRate;
                var postingPeriod = _context.PostingPeriods.Where(w => w.PeroidStatus == PeroidStatus.Unlocked).ToList();
                if (postingPeriod.Count <= 0)
                {
                    ModelState.AddModelError("PostingDate", "PeroiStatus is closed or locked");
                }
                else
                {
                    bool isValidPostingDate = false,
                        isValidDocumentDate = false;
                    foreach (var item in postingPeriod)
                    {
                        if (DateTime.Compare(master.PostingDate, item.PostingDateFrom) >= 0 && DateTime.Compare(master.PostingDate, item.PostingDateTo) <= 0)
                        {
                            isValidPostingDate = true;
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

                    if (!isValidDocumentDate)
                    {
                        ModelState.AddModelError("DocumentDate", "DocumentDate is closed or locked");
                    }
                }
            }
            var doctype = _context.DocumentTypes.Find(master.DocTypeID) ?? new DocumentType();
            master.SubTotal = subtotal;
            master.SubTotalSys = subtotal * master.ExchangeRate;
            master.TotalAmountSys = master.TotalAmount * master.ExchangeRate;
            if (doctype.Code != "CD" && doctype.Code != null)
                master.FreightAmountSys = master.FreightAmount * master.ExchangeRate;
            master.SubTotalBefDisSys = master.SubTotalBefDis * master.ExchangeRate;
            master.SubTotalAfterDisSys = master.SubTotalAfterDis * master.ExchangeRate;
        }
        private void CheckTaxAcc(IEnumerable<dynamic> details)
        {
            foreach (var dt in details)
            {
                if (dt.TaxGroupID > 0)
                {
                    var taxg = _context.TaxGroups.Find(dt.TaxGroupID) ?? new TaxGroup();
                    if (taxg.GLID <= 0)
                    {
                        ModelState.AddModelError("GLAcc", $"Item's code {dt.ItemCode} connected with tax {taxg.Code}-{taxg.Name} does not have account for tax!");
                    }
                }
            }
        }

        [HttpPost]
        public IActionResult UpdateProjCost(string data)
        {
            ProjectCostAnalysis projcost = JsonConvert.DeserializeObject<ProjectCostAnalysis>(data, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            projcost.ChangeLog = DateTime.Now;
            ModelMessage msg = new();
            SeriesDetail seriesDetail = new();
            var seriesSQ = _context.Series.FirstOrDefault(w => w.ID == projcost.SeriesID);
            var douType = _context.DocumentTypes.Where(i => i.ID == projcost.DocTypeID).FirstOrDefault();
            var localSetRate = _context.ExchangeRates.FirstOrDefault(w => w.CurrencyID == GetCompany().LocalCurrencyID).SetRate;

            projcost.UserID = GetUserID();
            projcost.BranchID = int.Parse(User.FindFirst("BranchID").Value);
            projcost.InvoiceNo = "PA-" + projcost.InvoiceNumber;

            var g = _projRepo.GetAllGroupDefind().ToList();

            ValidateSummary(projcost, projcost.ProjCostAnalysisDetails);
            CheckTaxAcc(projcost.ProjCostAnalysisDetails);
            foreach (var d in projcost.ProjCostAnalysisDetails)
            {
                var factor = g.FirstOrDefault(w => w.GroupUoMID == d.GUomID && w.AltUOM == d.UomID).Factor;
                d.Factor = factor;
                if (projcost.ID == 0)
                {
                    d.OpenQty = d.Qty;
                }
            }

            using (var t = _context.Database.BeginTransaction())
            {
                if (ModelState.IsValid)
                {
                    if (projcost.ID > 0)
                    {
                        _context.ProjectCostAnalyses.Update(projcost);
                    }
                    else
                    {
                        seriesDetail.Number = seriesSQ.NextNo;
                        seriesDetail.SeriesID = projcost.SeriesID;
                        _context.SeriesDetails.Update(seriesDetail);
                        _context.SaveChanges();
                        var seriesDetailID = seriesDetail.ID;
                        string Sno = seriesSQ.NextNo;
                        long No = long.Parse(Sno);
                        projcost.InvoiceNumber = seriesSQ.NextNo;
                        seriesSQ.NextNo = Convert.ToString(No + 1);
                        if (long.Parse(seriesSQ.NextNo) > long.Parse(seriesSQ.LastNo))
                        {
                            ModelState.AddModelError("SumInvoice", "Your Invoice has reached the limitation!!");
                            return Ok(msg.Bind(ModelState));
                        }
                        projcost.LocalSetRate = localSetRate;
                        projcost.LocalCurID = GetCompany().LocalCurrencyID;
                        projcost.LocalSetRate = localSetRate;
                        projcost.SeriesDID = seriesDetailID;
                        projcost.CompanyID = GetCompany().ID;

                        _context.ProjectCostAnalyses.Update(projcost);
                    }
                    if (projcost.CopyType == CopyType.SDM)
                    {
                        var solutiondata = _context.SolutionDataManagements.Where(s => s.ID == projcost.BaseOnID).FirstOrDefault() ?? new SolutionDataManagement();
                        solutiondata.Status = Models.SolutionDataManagement.Status.Closed;
                        _context.SolutionDataManagements.Update(solutiondata);
                        _context.SaveChanges();
                    }
                    _context.SaveChanges();
                    var freight = projcost.FreightProjectCost;
                    if (freight != null)
                    {
                        //freight.FreightSaleDetails = freight.FreightSaleDetails.Where(i => i.Amount > 0).ToList();
                        FreightProjCostDetail fdobj = new();
                        freight.ProjCAID = projcost.ID;

                        freight.SaleType = SaleCopyType.ProjectCostAnalysisDetail;
                        freight.OpenAmountReven = freight.AmountReven;
                        _context.FreightProjectCosts.Update(freight);
                        _context.SaveChanges();
                    }
                    t.Commit();
                    ModelState.AddModelError("success", "Item save successfully.");
                    msg.Approve();
                }
            }
            return Ok(msg.Bind(ModelState));
        }
        [HttpGet]
        public IActionResult FindProjectCost(string number, int seriesID)
        {
            var list = _projRepo.FindProjectCostAnalysis(number, seriesID, GetCompany().ID);
            if (list.ProjectCostAnalysis != null)
            {
                return Ok(list);
            }
            return Ok();
        }
        // get story Project costAnalysis
        [HttpGet]
        public async Task<IActionResult> GetstoryProjectCost(string seriesID)
        {
            var list = await _projRepo.GetStoryProjcostAsyce(seriesID);
            if (list != null)
            {
                return Ok(list);
            }
            return Ok();
        }
        public async Task<IActionResult> GetProjAnalysisinQuote(int id)
        {
            var list = await _projRepo.GetStoryProjcosQuotetAsyce(id);
            if (list != null)
                return Ok(list);
            else
                return Ok();
        }
        // start Project SolutionDAtamanagement
        public IActionResult SolutionDataMangement()
        {
            ViewBag.SolutionDatamanagement = "highlight";
            var seriesPA = (from dt in _context.DocumentTypes.Where(i => i.Code == "SD")
                            join sr in _context.Series.Where(w => !w.Lock) on dt.ID equals sr.DocuTypeID
                            select new SeriesInPurchasePoViewModel
                            {
                                ID = sr.ID,
                                Name = sr.Name,
                                Default = sr.Default,
                                NextNo = sr.NextNo,
                                DocumentTypeID = sr.DocuTypeID,
                                SeriesDetailID = _context.SeriesDetails.Where(i => i.SeriesID == sr.ID).FirstOrDefault() == null ? 0 :
                                _context.SeriesDetails.Where(i => i.SeriesID == sr.ID).FirstOrDefault().ID
                            }).ToList();
            return View(new { seriesPA, genSetting = GetGeneralSettingAdmin() });
        }
        [HttpPost]
        public IActionResult UpdateSolutionDataMG(string data)
        {
            SolutionDataManagement projcost = JsonConvert.DeserializeObject<SolutionDataManagement>(data, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });

            ModelMessage msg = new();
            SeriesDetail seriesDetail = new();
            var seriesSQ = _context.Series.FirstOrDefault(w => w.ID == projcost.SeriesID);
            var douType = _context.DocumentTypes.Where(i => i.ID == projcost.DocTypeID).FirstOrDefault();
            projcost.UserID = GetUserID();
            projcost.BranchID = int.Parse(User.FindFirst("BranchID").Value);
            projcost.InvoiceNo = "SD-" + projcost.InvoiceNumber;

            var g = _projRepo.GetAllGroupDefind().ToList();

            // ValidateSummary(projcost, projcost.ProjCostAnalysisDetails);
            using (var t = _context.Database.BeginTransaction())
            {
                if (ModelState.IsValid)
                {
                    if (projcost.ID > 0)
                    {
                        _context.SolutionDataManagements.Update(projcost);
                    }
                    else
                    {
                        seriesDetail.Number = seriesSQ.NextNo;
                        seriesDetail.SeriesID = projcost.SeriesID;
                        _context.SeriesDetails.Update(seriesDetail);
                        _context.SaveChanges();
                        var seriesDetailID = seriesDetail.ID;
                        string Sno = seriesSQ.NextNo;
                        long No = long.Parse(Sno);
                        projcost.InvoiceNumber = seriesSQ.NextNo;
                        seriesSQ.NextNo = Convert.ToString(No + 1);
                        if (long.Parse(seriesSQ.NextNo) > long.Parse(seriesSQ.LastNo))
                        {
                            ModelState.AddModelError("SumInvoice", "Your Invoice has reached the limitation!!");
                            return Ok(msg.Bind(ModelState));
                        }

                        projcost.SeriesDID = seriesDetailID;
                        projcost.CompanyID = GetCompany().ID;

                        _context.SolutionDataManagements.Update(projcost);
                    }
                    _context.SaveChanges();

                    t.Commit();
                    ModelState.AddModelError("success", "Item save successfully.");
                    msg.Approve();
                }
            }
            return Ok(msg.Bind(ModelState));
        }
        [HttpGet]
        public IActionResult FindSolutionData(string number, int seriesID)
        {
            var list = _projRepo.FindSolutionDatas(number, seriesID, GetCompany().ID);
            if (list.ProjectCostAnalysis != null)
            {
                return Ok(list);
            }
            return Ok();
        }
        // get story solution Data management
        [HttpGet]
        public async Task<IActionResult> GetstorySolutionData(int id)
        {
            var list = await _projRepo.GetStorySolutionDataAsyce(id);
            if (list != null)
            {
                return Ok(list);
            }
            return Ok();
        }
        // get story solution Data management
        [HttpGet]
        public async Task<IActionResult> GetHiststorySolution()
        {
            var list = await _projRepo.GetHistStorySolutionDataAsyce();
            if (list != null)
            {
                return Ok(list);
            }
            return Ok();
        }
        public IActionResult CopySolutionData(string number, int seriesID)
        {
            var list = _projRepo.CopySolutionDataMGAsynce(number, seriesID, GetCompany().ID);
            if (list != null)
                return Ok(list);
            else
                return Ok();
        }

    }
}
