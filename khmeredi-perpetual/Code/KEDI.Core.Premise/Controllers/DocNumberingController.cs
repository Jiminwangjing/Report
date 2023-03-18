using CKBS.AppContext;
using CKBS.Models.Services.Administrator.SystemInitialization;
using KEDI.Core.Models.Validation;
using KEDI.Core.Premise.Authorization;
using KEDI.Core.Premise.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace CKBS.Controllers
{
    [Privilege]
    public class DocNumberingController : Controller
    {
        private readonly DataContext _context;
        private readonly UserManager _userModule;
        public DocNumberingController(DataContext context, UserManager userModule)
        {
            _context = context;
            _userModule = userModule;
        }

        [Privilege("DN003")]
        public IActionResult Index()
        {
            ViewBag.DocNumbering = "highlight";
            return View(new Series());
        }

        int GetCompanyID()
        {
            return _userModule.CurrentUser.Company.ID;
        }

        [HttpGet]
        public IActionResult GetAllDocNumbering()
        {
            var docNumberings = _context.DocumentTypes.ToList();
            List<DocNumberingViewModel> docNumViewModel = new();
            var index = 1;
            foreach (var value in docNumberings)
            {
                var defaultSeries = _context.Series.Where(i => i.DocuTypeID == value.ID && i.Default == true && i.CompanyID == GetCompanyID()).ToList().FirstOrDefault();
                docNumViewModel.Add(new DocNumberingViewModel
                {
                    LineID = index,
                    Document = value.Name,
                    Code = value.Code,
                    DefaultSeries = defaultSeries == null ? "" : defaultSeries.Name,
                    FirstNo = defaultSeries == null ? "" : defaultSeries.FirstNo,
                    NextNo = defaultSeries == null ? "" : defaultSeries.NextNo,
                    LastNo = defaultSeries == null ? "" : defaultSeries.LastNo,
                    DocuTypeID = value.ID,
                    ID = value.ID,
                });
                index++;
            }
            return Ok(docNumViewModel);
        }

        [HttpGet]
        public IActionResult GetOneCreate(int id)
        {
            var docNumberings = _context.Series.Where(i => i.DocuTypeID == id && i.CompanyID == GetCompanyID()).ToList().Count;
            var periodIn = _context.PeriodIndicators.Where(i => !i.Delete).ToList();
            periodIn.Insert(0, new PeriodIndicator
            {
                CompanyID = 0,
                Delete = false,
                ID = 0,
                Name = "-- Select --"
            });
            SeriesCreateViewModel seriesCreateViewModel = new()
            {
                LineID = docNumberings + 1,
                Name = "",
                PreFix = "",
                FirstNo = "",
                LastNo = "",
                NextNo = "",
                DocuTypeID = 0,
                ID = 0,
                PeriodIndID = 0,
                PeriodIndecator = new SelectList(periodIn, "ID", "Name"),
                Default = false,
                Lock = false
            };
            return Ok(seriesCreateViewModel);
        }

        [HttpGet]
        public IActionResult GetAllSeries(int id)
        {
            var docNumberings = _context.Series.Where(i => i.DocuTypeID == id && i.CompanyID == GetCompanyID()).ToList();
            List<SeriesViewModel> seriesViewModel = new();
            var index = 1;
            foreach (var value in docNumberings)
            {
                seriesViewModel.Add(new SeriesViewModel
                {
                    LineID = index,
                    Name = value.Name,
                    PreFix = value.PreFix,
                    FirstNo = value.FirstNo,
                    NextNo = value.NextNo,
                    LastNo = value.LastNo,
                    DocuNumberingID = value.DocuTypeID,
                    ID = value.ID,
                    PeriodIndID = value.PeriodIndID,
                    PeriodIndecator = _context.PeriodIndicators.Select(i => new SelectListItem
                    {
                        Value = i.ID.ToString(),
                        Text = i.Name,
                        Selected = i.ID == value.PeriodIndID
                    }).ToList(),
                    Default = value.Default,
                    Lock = value.Lock
                });
                index++;
            }
            return Ok(seriesViewModel);
        }

        [HttpPost]
        public IActionResult CreateSeries(Series series)
        {
            ModelMessage msg = new();
            var count = 0;
            if (string.IsNullOrEmpty(series.Name))
            {
                ModelState.AddModelError("Name", "Please Input Name!");
                count++;
            }
            if (string.IsNullOrEmpty(series.FirstNo))
            {
                ModelState.AddModelError("Name", "Please Input First No!");
                count++;
            }
            if (string.IsNullOrEmpty(series.LastNo))
            {
                ModelState.AddModelError("Name", "Please Input Last No!");
                count++;
            }
            if (series.PeriodIndID == 0)
            {
                ModelState.AddModelError("Name", "Please Select Period Indecator!");
                count++;
            }
            if (count > 0)
            {
                return Ok(msg.Bind(ModelState));
            }
            if (series.ID == 0)
            {
                ModelState.AddModelError("Success", "Item was Created!");
            }
            else
            {
                ModelState.AddModelError("Success", "Item was Updated!");
            }
            series.CompanyID = GetCompanyID();
            _context.Series.Update(series);
            _context.SaveChanges();
            msg.Approve();
            return Ok(msg.Bind(ModelState));
        }

        [HttpPost]
        public IActionResult SetSeriesDefault(int id, int idDocNum)
        {
            ModelMessage msg = new();
            var series = _context.Series.Where(i => i.DocuTypeID == idDocNum && i.CompanyID == GetCompanyID()).ToList();
            var seriesOne = _context.Series.Where(i => i.DocuTypeID == idDocNum && i.ID == id).ToList().FirstOrDefault();
            if (seriesOne.Lock == true)
            {
                ModelState.AddModelError("error", $"Item Name \"{seriesOne.Name}\" is Locked!");
                return Ok(msg.Bind(ModelState));
            }
            else
            {
                foreach (var item in series)
                {
                    if (item.ID == id)
                    {
                        item.Default = true;
                        ModelState.AddModelError("success", $"{item.Name} is set to default");
                        msg.Approve();
                    }
                    else
                    {
                        item.Default = false;
                    }
                    _context.SaveChanges();

                }
            }

            return Ok(msg.Bind(ModelState));
        }
    }
}
