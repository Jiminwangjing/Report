using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CKBS.AppContext;
using CKBS.Models.Services.Pagination;
using CKBS.Models.Services.Promotions;
using CKBS.Models.Services.Responsitory;
using System.Text.RegularExpressions;
using KEDI.Core.Premise.Authorization;
using System.Security.Claims;
using KEDI.Core.Models.Validation;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CKBS.Controllers
{
    [Privilege]
    public class PromotionController : Controller
    {
        private readonly DataContext _context;
        private readonly IPromotion _promotion;

        public PromotionController(DataContext context, IPromotion promotion)
        {
            _context = context;
            _promotion = promotion;

        }

        [HttpGet]
        [Privilege("A029")]
        public IActionResult Index()
        {
            ViewBag.Promotion = "highlight";
            return View();
        }

        string RawWord(string keyword)
        {
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                return Regex.Replace(keyword, "\\+s", string.Empty);
            }
            return string.Empty;
        }

        public IActionResult GetPromotion(string keyword = "")
        {
            int userid = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var promotion = from pr in _context.Promotions
                                //  join pri in _context.PriceLists on pr.PriceListID equals pri.ID
                            let pri = _context.PriceLists.FirstOrDefault(x => x.ID == pr.PriceListID) ?? new Models.Services.Inventory.PriceList.PriceLists()
                            select new
                            {
                                ID = pr.ID,
                                Name = pr.Name,
                                StartDate = pr.StartDate,
                                StopDate = pr.StopDate,
                                StartTime = pr.StartTime,
                                StopTime = pr.StopTime,
                                PriceList = pri.Name
                            };

            if (!string.IsNullOrWhiteSpace(keyword))

            {
                keyword = RawWord(keyword);
                StringComparison ignoreCase = StringComparison.InvariantCultureIgnoreCase;
                promotion = promotion.Where(c => RawWord(c.Name).Contains(keyword, ignoreCase));
            }
            return Ok(promotion.ToList());
        }

        [Privilege("A029")]
        public IActionResult Create()
        {
            ViewBag.style = "fa fa-percent";
            ViewBag.Main = "Promotion";
            ViewBag.Page = "Promotion";
            ViewBag.Subpage = "";
            ViewBag.type = "create";
            ViewBag.button = "fa-plus-circle";
            ViewBag.PromotionMenu = "show";
            ViewBag.Promotion = "highlight";
            ViewData["PrceiList"] = new SelectList(_context.PriceLists.Where(x => x.Delete == false), "ID", "Name");
            return View();
        }
        [HttpPost]
            [ValidateAntiForgeryToken]
         public async Task<IActionResult> Create(Promotion promotion)
        {

            ViewBag.style = "fa fa-percent";
            ViewBag.Main = "Promotion";
            ViewBag.Page = "Promotion";
            ViewBag.Subpage = "";
            ViewBag.type = "create";
            ViewBag.button = "fa-plus-circle";
            ViewBag.PromotionMenu = "show";
            ViewBag.Promotion = "highlight";
            ViewData["PrceiList"] = new SelectList(_context.PriceLists.Where(x => x.Delete == false), "ID", "Name");
            // ModelMessage msg = new();
            // promo.StartTime=default(TimeSpan);
            // promo.StopTime=default(TimeSpan);
            // if (promo.Name == null)
            // {
            //     ModelState.AddModelError("Name", "Please Input Name");
            // }
            // if (promo.PriceListID == 0)
            // {
            //     ModelState.AddModelError("PriceListID", "Please Selete PriceList");
            // }
            // if (promo.StopDate == null)
            // {
            //     ModelState.AddModelError("StopDate", "Please Choose StopDate");
            // }
            // if (promo.StartDate == null)
            // {
            //     ModelState.AddModelError("StartDate", "Please Choose StartDate");
            // }
            ModelState["StartTime"].ValidationState = ModelValidationState.Valid;
            ModelState["StopTime"].ValidationState = ModelValidationState.Valid;
            if (ModelState.IsValid)
            {
                 await _promotion.AddOrEdit(promotion);
                return RedirectToAction(nameof(Index));
            }
           return View(promotion);

        }

        [Privilege("A029")]
        public IActionResult Edit(int id)
        {
            ViewBag.style = "fa fa-percent";
            ViewBag.Main = "Promotion";
            ViewBag.Page = "Promotion";
            ViewBag.Subpage = "";
            ViewBag.type = "Edit";
            ViewBag.button = "fa-edit";
            ViewBag.PromotionMenu = "show";
            ViewBag.Promotion = "highlight";
            ViewData["PrceiList"] = new SelectList(_context.PriceLists.Where(x => x.Delete == false), "ID", "Name");
            var promotion = _promotion.GetID(id);
            if (promotion == null)
            {
                return NotFound();
            }
            return View(promotion);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Promotion promotion)
        {
            ViewBag.style = "fa fa-percent";
            ViewBag.Main = "Promotion";
            ViewBag.Page = "Promotion";
            ViewBag.Subpage = "";
            ViewBag.type = "Edit";
            ViewBag.button = "fa-edit";
            ViewBag.PromotionMenu = "show";
            ViewBag.Promotion = "highlight";
            if (id != promotion.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var prod = await _context.PromotionDetails.Where(x => x.PromotionID == promotion.ID).ToListAsync();
                    foreach (var p in prod)
                    {
                        p.StartDate = promotion.StartDate;
                        p.StopDate = promotion.StopDate;
                        p.StartTime = promotion.StartTime;
                        p.StopTime = promotion.StopTime;
                    }
                    _context.UpdateRange(prod);
                    _context.SaveChanges();
                    await _promotion.AddOrEdit(promotion);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PromotionExists(promotion.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(promotion);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var promotion = await _context.Promotions
                .FirstOrDefaultAsync(m => m.ID == id);

            if (promotion == null)
            {
                return NotFound();
            }
            _context.Promotions.Remove(promotion);
            await _context.SaveChangesAsync();
            return Ok();
        }

        private bool PromotionExists(int id)
        {
            return _context.Promotions.Any(e => e.ID == id);
        }

        [HttpPost]
        public async Task<IActionResult> SetActive(int ID)
        {
            await _promotion.SetActive(ID);
            return Ok();
        }

    }
}
