using CKBS.AppContext;
using CKBS.Models.Services.Administrator.SystemInitialization;
using KEDI.Core.Models.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using KEDI.Core.Premise.Authorization;

namespace CKBS.Controllers
{
    [Privilege]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class PostingPeriodsController : Controller
    {
        private readonly DataContext _context;

        public PostingPeriodsController(DataContext context)
        {
            _context = context;
        }

        [Route("/postingperiod/index")]
        [Privilege("PP002")]
        public IActionResult Index()
        {
            ViewBag.PostingPeriods = "highlight";
            return View();
        }

        int GetCompanyID()
        {
            int.TryParse(User.FindFirstValue("CompanyID"), out int _companyId);
            return _companyId;
        }

        [Route("/postingperiod/create")]
        public IActionResult Create()
        {
            ViewBag.PostingPeriods = "highlight";
            ViewBag.PeriodIndecator = new SelectList(_context.PeriodIndicators.Where(d => d.Delete == false && d.CompanyID == GetCompanyID()), "ID", "Name");
            //ViewBag.Months = GetMonths(0);
            return View(new PostingPeriod());
        }

        [Route("/postingperiod/edit")]
        [HttpGet]
        public IActionResult Edit(int id)
        {
            ViewBag.PostingPeriods = "highlight";
            ViewBag.PeriodIndecator = new SelectList(_context.PeriodIndicators.Where(d => d.Delete == false && d.CompanyID == GetCompanyID()), "ID", "Name");
            var postingPeriod = _context.PostingPeriods.Find(id);
            return View(postingPeriod);
        }

        [Route("/postingperiod/getallpostingperiods")]
        public IActionResult GetAllPostingPeriod()
        {
            var postingPeriods = _context.PostingPeriods.Where(w => w.CompanyID == GetCompanyID()).ToList();
            List<PostingPeriodViewModel> postingPeriodView = new List<PostingPeriodViewModel>();
            foreach (var value in postingPeriods)
            {
                postingPeriodView.Add(new PostingPeriodViewModel
                {
                    ID = value.ID,
                    PeriodCode = value.PeriodCode,
                    PeriodName = value.PeriodName,
                    SubPeriod = value.SubPeriod == (SubPeriod)1 ? "Year" : "Month",
                    NoOfPeroid = value.NoOfPeroid,
                    PeroidIndID = value.PeroidIndID,
                    PeroidStatus = value.PeroidStatus == (PeroidStatus)1 ? "Unlocked" :
                    value.PeroidStatus == (PeroidStatus)2 ? "UnlockedExceptSale" :
                    value.PeroidStatus == (PeroidStatus)3 ? "ClosingPeriod" : "Locked",
                    PostingDateFrom = value.PostingDateFrom.ToString("MM-dd-yyyy"),
                    PostingDateTo = value.PostingDateTo.ToString("MM-dd-yyyy"),
                    DueDateFrom = value.DueDateFrom.ToString("MM-dd-yyyy"),
                    DueDateTo = value.DueDateTo.ToString("MM-dd-yyyy"),
                    DocuDateFrom = value.DocuDateFrom.ToString("MM-dd-yyyy"),
                    DocuDateTo = value.DocuDateTo.ToString("MMMM-dddd-dd-yyyy"),
                    StartOfFiscalYear = value.StartOfFiscalYear.ToString("MM-dd-yyyy"),
                    FiscalYear = value.FiscalYear
                });
            }
            return Ok(postingPeriodView);
        }

        [Route("/postingperiod/addpostingperiod")]
        [HttpPost]
        public IActionResult AddPostingPeriod(PostingPeriod postingPeriod, int id)
        {
            ModelMessage msg = new ModelMessage();
            var count = 0;
            if (String.IsNullOrEmpty(postingPeriod.PeriodCode))
            {
                ModelState.AddModelError("PeriodCode", "Please Input Period Code!");
                count++;
            }
            if (postingPeriod.SubPeriod <= 0)
            {
                ModelState.AddModelError("SubPeriod", "Please Select SubPeriod!");
                count++;
            }
            if (postingPeriod.NoOfPeroid == null)
            {
                ModelState.AddModelError("NoOfPeroid", "Please Select SubPeriod First!");
                count++;
            }
            if (String.IsNullOrEmpty(postingPeriod.PeriodName))
            {
                ModelState.AddModelError("PeriodName", "Please Input Period Name!");
                count++;
            }
            if (postingPeriod.PeroidIndID <= 0)
            {
                ModelState.AddModelError("PeroidIndID", "Please Select Period Indecator!");
                count++;
            }
            if (count > 0)
            {
                return Ok(msg.Bind(ModelState));
            }
            var month = postingPeriod.PeriodCode.Split("-");
            var monthNum = _context.PostingPeriods.AsNoTracking().Where(i => i.PeriodCode.Substring(0, 4) == month[0]).ToList();
            var year = monthNum.Count() > 0 ? monthNum.FirstOrDefault().PostingDateTo.Year : 0;
            if (id == 0)
            {
                var checkperiodcode = _context.PostingPeriods.FirstOrDefault(w => w.PeriodCode == postingPeriod.PeriodCode);
                if (checkperiodcode != null)
                {
                    ModelState.AddModelError("PeriodCode", "This year already created!");
                    return Ok(msg.Bind(ModelState));
                }
                //Year
                if (postingPeriod.NoOfPeroid.Length <= 1)
                {                  
                    var monthNums = _context.PostingPeriods.AsNoTracking().Where(i => i.PeriodCode.Substring(0, 4) == month[0]).ToList();
                    if (Convert.ToInt32(month[0]) == year && monthNums.Max(i => i.PostingDateTo.Month) == 12)
                    {
                        ModelState.AddModelError("PeriodCode", "This year already created!");
                        count++;
                    }
                    if (count > 0)
                    {
                        return Ok(msg.Bind(ModelState));
                    }
                    _context.PostingPeriods.Update(postingPeriod);
                    _context.SaveChanges();
                    ModelState.AddModelError("Success", "Posting Period was Created!");
                }
                //Month
                else if (postingPeriod.NoOfPeroid.Length >= 2)
                {                 
                    var monthNums = _context.PostingPeriods.AsNoTracking().Where(i => i.PeriodCode.Substring(0, 4) == month[0]).ToList();
                    if (Convert.ToInt32(month[0]) == year && monthNums.Max(i => i.PostingDateTo.Month) == 12)
                    {
                        ModelState.AddModelError("PeriodCode", "This year already created!");
                        count++;
                    }
                }

                if (count > 0)
                {
                    return Ok(msg.Bind(ModelState));
                }

                postingPeriod.CompanyID = GetCompanyID();
                if (postingPeriod.ID > 0)
                {
                    _context.PostingPeriods.Update(postingPeriod);
                    _context.SaveChanges();
                    ModelState.AddModelError("Success", "Posting Period was Updated!");
                }
                else
                {
                    for (var i = 1; i <= 12; i++)
                    {
                        if (monthNum.Count() == 0)
                        {
                            var posting = new PostingPeriod
                            {
                                CompanyID = postingPeriod.CompanyID,
                                PeriodCode = $"{month[0]}-{i.ToString().PadLeft(2, '0')}",
                                PeriodName = $"{month[0]}-{i.ToString().PadLeft(2, '0')}",
                                SubPeriod = postingPeriod.SubPeriod,
                                PostingDateTo = new DateTime(Convert.ToInt32(month[0]), i, DateTime.DaysInMonth(Convert.ToInt32(month[0]), i)),
                                PostingDateFrom = new DateTime(Convert.ToInt32(month[0]), i, 1),
                                DocuDateTo = new DateTime(Convert.ToInt32(month[0]), i, DateTime.DaysInMonth(Convert.ToInt32(month[0]), i)),
                                DocuDateFrom = new DateTime(Convert.ToInt32(month[0]), i, 1),
                                DueDateTo = new DateTime(Convert.ToInt32(month[0]), i, DateTime.DaysInMonth(Convert.ToInt32(month[0]), i)),
                                DueDateFrom = new DateTime(Convert.ToInt32(month[0]), i, 1),
                                FiscalYear = postingPeriod.FiscalYear,
                                NoOfPeroid = postingPeriod.NoOfPeroid,
                                PeroidIndID = postingPeriod.PeroidIndID,
                                PeroidStatus = postingPeriod.PeroidStatus,
                                StartOfFiscalYear = postingPeriod.StartOfFiscalYear
                            };
                            _context.PostingPeriods.Update(posting);
                            _context.SaveChanges();
                        }
                        else if (monthNum.Count() > 0 && monthNum.Max(i => i.PostingDateFrom).Month < i)
                        {
                            var posting = new PostingPeriod
                            {
                                CompanyID = postingPeriod.CompanyID,
                                PeriodCode = $"{month[0]}-{i.ToString().PadLeft(2, '0')}",
                                PeriodName = $"{month[0]}-{i.ToString().PadLeft(2, '0')}",
                                SubPeriod = postingPeriod.SubPeriod,
                                PostingDateTo = new DateTime(Convert.ToInt32(month[0]), i, DateTime.DaysInMonth(Convert.ToInt32(month[0]), i)),
                                PostingDateFrom = new DateTime(Convert.ToInt32(month[0]), i, 1),
                                DocuDateTo = new DateTime(Convert.ToInt32(month[0]), i, DateTime.DaysInMonth(Convert.ToInt32(month[0]), i)),
                                DocuDateFrom = new DateTime(Convert.ToInt32(month[0]), i, 1),
                                DueDateTo = new DateTime(Convert.ToInt32(month[0]), i, DateTime.DaysInMonth(Convert.ToInt32(month[0]), i)),
                                DueDateFrom = new DateTime(Convert.ToInt32(month[0]), i, 1),
                                FiscalYear = postingPeriod.FiscalYear,
                                NoOfPeroid = postingPeriod.NoOfPeroid,
                                PeroidIndID = postingPeriod.PeroidIndID,
                                PeroidStatus = postingPeriod.PeroidStatus,
                                StartOfFiscalYear = postingPeriod.StartOfFiscalYear
                            };
                            _context.PostingPeriods.Update(posting);
                            _context.SaveChanges();
                        }
                    }
                    ModelState.AddModelError("Success", "Posting Period was Created!");
                }
                msg.Approve();
            }
            if (id > 0)
            {
                _context.PostingPeriods.Update(postingPeriod);
                _context.SaveChanges();
                ModelState.AddModelError("Success", "Posting Period was Updated!");
                msg.Approve();
            }
            return Ok(msg.Bind(ModelState));
        }

        public IEnumerable<SelectListItem> GetMonths(int selectedValue)
        {
            Dictionary<int, string> _months = new Dictionary<int, string>
            {
                {1, "January" },
                {2, "February" },
                {3, "March" },
                {4, "April" },
                {5, "May" },
                {6, "June" },
                {7, "July" },
                {8, "August" },
                {9, "September" },
                {10, "October" },
                {11, "November" },
                {12, "December" }
            };
            var months = _months.Select(m => new SelectListItem
            {
                Value = m.Key.ToString(),
                Text = m.Value,
                Selected = m.Key == selectedValue
            });
            return months;
        }

        [Route("/postingperiod/addperiodindecator")]
        [HttpPost]
        public IActionResult AddPeriodIndecator(PeriodIndicator indecator)
        {
            indecator.CompanyID = GetCompanyID();
            _context.PeriodIndicators.Update(indecator);
            _context.SaveChanges();
            return Ok();
        }

        [Route("/postingperiod/getperiodindecator")]
        [HttpGet]
        public IActionResult GetPeriodIndecator()
        {
            var indecator = _context.PeriodIndicators.Where(i => !i.Delete && i.CompanyID == GetCompanyID()).ToList().OrderByDescending(i => i.ID);

            return Ok(indecator);
        }
    }
}
