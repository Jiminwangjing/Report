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

namespace CKBS.Controllers
{
    [Privilege]
    public class CardTypeController : Controller
    {
        private readonly DataContext _context;
        private readonly ICardType _cardType;

        public CardTypeController(DataContext context,ICardType cardType)
        {
            _context = context;
            _cardType = cardType;
        }

        [HttpGet]
        [Privilege("A034")]
        public IActionResult Index()
        {
            ViewBag.CardType = "highlight";
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

        public IActionResult GetCardType(string keyword = "")
        {
            int userid = int.Parse(User.FindFirst("UserID").Value);
            var cardTypes = _context.CardTypes.Where(x => x.Delete == false);

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = RawWord(keyword);
                StringComparison ignoreCase = StringComparison.InvariantCultureIgnoreCase;
                cardTypes = cardTypes.Where(c => RawWord(c.Name).Contains(keyword, ignoreCase));
            }
            return Ok(cardTypes.ToList());
        }

        [Privilege("A034")]
        public IActionResult Create()
        {
            ViewBag.CardType = "highlight";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Privilege("A034")]
        public async Task<IActionResult> Create([Bind("ID,Name,Discount,TypeDis,Delete")] CardType cardType)
        {
            ViewBag.style = "fa fa-percent";
            ViewBag.Main = "Promotion";
            ViewBag.Page = "Member";
            ViewBag.Subpage = "Create Type";
            ViewBag.Member = "show";
            ViewBag.CardType = "highlight";
            ViewBag.type = "create";
            ViewBag.button = "fa-plus-circle";
            if (cardType.TypeDis == null)
            {
                ViewBag.type_requried = "Please select type discount !";
            }

            if (ModelState.IsValid)
            {
                await _cardType.AddorEdit(cardType);
                return RedirectToAction(nameof(Index));
            }
            return View(cardType);

        }

        public IActionResult Edit(int id)
        {
            ViewBag.style = "fa fa-percent";
            ViewBag.Main = "Promotion";
            ViewBag.Page = "Member";
            ViewBag.Subpage = "Create Type";
            ViewBag.Member = "show";
            ViewBag.CardType = "highlight";
            ViewBag.type = "Edit";
            ViewBag.button = "fa-edit";

            var cardType = _cardType.GetId(id);
            if (cardType.Delete == true)
            {
                return RedirectToAction(nameof(Index));
            }
            if (cardType == null)
            {
                return NotFound();
            }
            return View(cardType);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,Discount,TypeDis,Delete")] CardType cardType)
        {
            ViewBag.style = "fa fa-percent";
            ViewBag.Main = "Promotion";
            ViewBag.Page = "Member";
            ViewBag.Subpage = "Create Type";
            ViewBag.Member = "show";
            ViewBag.CardType = "highlight";
            ViewBag.type = "Edit";
            ViewBag.button = "fa-edit";
            if (cardType.TypeDis == null)
            {
                ViewBag.type_requried = "Please select type discount !";
            }
            if (id != cardType.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _cardType.AddorEdit(cardType);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CardTypeExists(cardType.ID))
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
            return View(cardType);
        }
      
        private bool CardTypeExists(int id)
        {
            return _context.CardTypes.Any(e => e.ID == id);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int ID)
        {
            await _cardType.DeleteCardType(ID);
            return Ok();
        }
    }
}
