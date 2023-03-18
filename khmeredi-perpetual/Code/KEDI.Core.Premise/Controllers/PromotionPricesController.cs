using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CKBS.AppContext;
using CKBS.Models.Services.Pagination;
using CKBS.Models.Services.Promotions;
using CKBS.Models.Services.Responsitory;
using KEDI.Core.Premise.Authorization;

namespace CKBS.Controllers
{
    [Privilege]
    public class PromotionPricesController : Controller
    {
        private readonly DataContext _context;
        private readonly IPromitionPrice _promotionprice;
        private readonly IWebHostEnvironment _appEnvironment;

        public PromotionPricesController(DataContext context, IPromitionPrice promotionprice, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _promotionprice = promotionprice;
            _appEnvironment = hostingEnvironment;
        }

        // GET: PromotionPrices
        [Privilege("A037")]
        public async Task<IActionResult> Index(string minpage = "5", string sortOrder = null, string currentFilter = null, string searchString = null, int? page = null)
        {
            ViewBag.style = "fa-cogs";
            ViewBag.Main = "Administrator";
            ViewBag.Page = "General";
            ViewBag.Subpage = "Branch";
            ViewBag.Menu = "show";

            var data = _promotionprice.GetPromotionPrice().OrderBy(g => g.ID);
            var Cate = from s in data select s;
            int pageSize = 0, MaxSize = 0;
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["AmountSortParm"] = sortOrder == "Amount" ? "Amount_desc" : "Amount";
            ViewData["CurrencyIDSortParm"] = sortOrder == "CurrencyID" ? "CurrencyID_desc" : "CurrencyID";
            ViewData["DiscountSortParm"] = sortOrder == "Discount" ? "Discount_desc" : "Discount";
            ViewData["TypeDisSortParm"] = sortOrder == "TypeDis" ? "TypeDis_desc" : "TypeDis";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;

            }
            ViewData["CurrentFilter"] = searchString;

            //var Cate = from s in data select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                Cate = Cate.Where(s => s.Name.Contains(searchString) ||
                s.Amount.ToString().Contains(searchString) || s.Currency.Symbol.Contains(searchString) ||
                 s.Discount.ToString().Contains(searchString) || s.TypeDis.ToString().Contains(searchString));


            }
            switch (sortOrder)
            {
                case "name_desc":
                    Cate = Cate.OrderByDescending(s => s.Name);
                    break;
                case "Amount":
                    Cate = Cate.OrderBy(s => s.Amount);
                    break;
                case "Amount_desc":
                    Cate = Cate.OrderByDescending(s => s.Amount);
                    break;
                case "CurrencyID":
                    Cate = Cate.OrderBy(s => s.CurrencyID);
                    break;
                case "CurrencyID_desc":
                    Cate = Cate.OrderByDescending(s => s.CurrencyID);
                    break;
                case "Discount":
                    Cate = Cate.OrderBy(s => s.Discount);
                    break;
                case "Discount_desc":
                    Cate = Cate.OrderByDescending(s => s.Discount);
                    break;
                case "TypeDis":
                    Cate = Cate.OrderBy(s => s.TypeDis);
                    break;
                case "TypeDis_desc":
                    Cate = Cate.OrderByDescending(s => s.Discount);
                    break;
            }
            //int pageSize = 0, MaxSize = 0;
            int.TryParse(minpage, out MaxSize);

            if (MaxSize == 0)
            {
                int d = data.Count();
                pageSize = d;
                ViewBag.sizepage5 = "All";
            }
            else
            {
                if (MaxSize == 5)
                {
                    ViewBag.sizepage1 = minpage;
                }
                else if (MaxSize == 10)
                {
                    ViewBag.sizepage2 = minpage;
                }
                else if (MaxSize == 15)
                {
                    ViewBag.sizepage3 = minpage;
                }
                else if (MaxSize == 20)
                {
                    ViewBag.sizepage4 = minpage;
                }
                else
                {
                    ViewBag.sizepage5 = minpage;
                }
                pageSize = MaxSize;


            }
            return View(await Pagination<PromotionPrice>.CreateAsync(Cate, page ?? 1, pageSize));
        }

        // GET: PromotionPrices/Create
        [Privilege("A037")]
        public IActionResult Create()
        {
            ViewBag.style = "fa-cogs";
            ViewBag.Main = "Promotion";
            ViewBag.Page = "";
            ViewBag.Subpage = "PromotionPrice";
            ViewBag.type = "create";
            ViewBag.button = "fa-plus-circle";
            ViewBag.Menu = "show";

            ViewData["CurrencyID"] = new SelectList(_context.Currency.Where(c => c.Delete == false), "ID", "Symbol");
            return View();
        }
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,Amount,CurrencyID,Discount,TypeDis,Delete")] PromotionPrice promotionPrice)
        {
            ViewBag.style = "fa-cogs";
            ViewBag.Main = "Promotion";
            ViewBag.Page = "";
            ViewBag.Subpage = "PromotionPrice";
            ViewBag.type = "create";
            ViewBag.button = "fa-plus-circle";
            ViewBag.Menu = "show";

            bool valid = false;

            if (promotionPrice.Name == null)
            {
                ViewData["error.Name"] = "Please input name!";
                valid = true;
            }
            if (promotionPrice.Amount == 0)
            {
                ViewData["error.Amount"] = "Please input amount!";
                valid = true;
            }
            if (promotionPrice.CurrencyID == 0)
            {
                ViewData["error.Currency"] = "Please select currency!";
                valid = true;
            }
            if (promotionPrice.Discount == 0)
            {
                ViewData["error.Discount"] = "Please input discount!";
                valid = true;
            }
            if (promotionPrice.TypeDis == "0")
            {
                ViewData["error.TypeDis"] = "Please select Type discount!";
                valid = true;
            }
            if (!valid)
            {
                await _promotionprice.AddOrEdit(promotionPrice);
                return RedirectToAction(nameof(Index));
            }

            ViewData["CurrencyID"] = new SelectList(_context.Currency.Where(c => c.Delete == false), "ID", "Symbol", promotionPrice.CurrencyID);
            return View(promotionPrice);
        }

        // GET: PromotionPrices/Edit/5
        [Privilege("A037")]
        public async Task<IActionResult> Edit(int? id)
        {
            ViewBag.style = "fa-cogs";
            ViewBag.Main = "Promotion";
            ViewBag.Page = "";
            ViewBag.Subpage = "PromotionPrice";
            ViewBag.type = "Edit";
            ViewBag.button = "fa-edit";
            ViewBag.Menu = "show";

            if (id == null)
            {
                return NotFound();
            }

            var promotionPrice = await _context.PromotionPrice.FindAsync(id);
            if (promotionPrice == null)
            {
                return NotFound();
            }
            ViewData["CurrencyID"] = new SelectList(_context.Currency, "ID", "Symbol", promotionPrice.CurrencyID);
            return View(promotionPrice);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            await _promotionprice.Delete(id);
            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,Amount,CurrencyID,Discount,TypeDis,Delete")] PromotionPrice promotionPrice)
        {
            ViewBag.style = "fa-cogs";
            ViewBag.Main = "Promotion";
            ViewBag.Page = "";
            ViewBag.Subpage = "PromotionPrice";
            ViewBag.type = "Edit";
            ViewBag.button = "fa-edit";
            ViewBag.Menu = "show";
            if (id != promotionPrice.ID)
            {
                return NotFound();
            }


            if (ModelState.IsValid)
            {
                bool valid = false;
                try
                {
                    if (promotionPrice.Name == null)
                    {
                        ViewData["error.Name"] = "Please input name!";
                        valid = true;
                    }
                    if (promotionPrice.Amount == 0)
                    {
                        ViewData["error.Amount"] = "Please input amount!";
                        valid = true;
                    }
                    if (promotionPrice.CurrencyID == 0)
                    {
                        ViewData["error.Currency"] = "Please select currency!";
                        valid = true;
                    }
                    if (promotionPrice.Discount == 0)
                    {
                        ViewData["error.Discount"] = "Please input discount!";
                        valid = true;
                    }
                    if (promotionPrice.TypeDis == "0")
                    {
                        ViewData["error.TypeDis"] = "Please select Type discount!";
                        valid = true;
                    }
                    if(!valid)
                    {
                       await _promotionprice.AddOrEdit(promotionPrice);
                       return RedirectToAction(nameof(Index));
                    }
                    
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PromotionPriceExists(promotionPrice.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                
            }

            ViewData["CurrencyID"] = new SelectList(_context.Currency.Where(c=>c.Delete == false), "ID", "Symbol", promotionPrice.CurrencyID);
            return View(promotionPrice);
        }

        // GET: PromotionPrices/Delete/5

        // POST: PromotionPrices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var promotionPrice = await _context.PromotionPrice.FindAsync(id);
            _context.PromotionPrice.Remove(promotionPrice);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PromotionPriceExists(int id)
        {
            return _context.PromotionPrice.Any(e => e.ID == id);
        }
    }
}
