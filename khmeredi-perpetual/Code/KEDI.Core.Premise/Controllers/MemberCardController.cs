using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using CKBS.AppContext;
using CKBS.Models.Services.Promotions;
using CKBS.Models.Services.Responsitory;
using KEDI.Core.Premise.Authorization;

namespace CKBS.Controllers
{
    [Privilege]
    public class MemberCardController : Controller
    {
        private readonly IMemberCard _memberCard;
        private readonly DataContext _context;
        public MemberCardController(IMemberCard memberCard,DataContext dataContext)
        {
            _memberCard = memberCard;
            _context = dataContext;
        }

        [Privilege("A035")]
        public IActionResult MemberCard()
        {
            ViewBag.style = "fa fa-percent";
            ViewBag.Main = "Promotion";
            ViewBag.Page = "Member";
            ViewBag.Subpage = "Create Card";
            ViewBag.PromotionMenu = "show";
            ViewBag.Member = "show";
            ViewBag.MemberCard = "highlight";

            ViewData["CardTypeID"] = new SelectList(_context.CardTypes.Where(x => x.Delete == false), "ID", "Name");
            return View();
        }

        [HttpGet]
        public IActionResult GetMemberCard()
        {
            var list = _memberCard.GetMemberCards().ToList();
            return Ok(list);
        }

        [HttpPost]
        public IActionResult InsertMemberCard(MemberCard memberCard)
        {
            _memberCard.AddorEdit(memberCard);
            var list = _memberCard.GetMemberCards().ToList();
            return Ok(list);
        }

        [HttpPost]
        public IActionResult CheckReferentNumber(string Check)
        {
            var list = _context.MemberCards.Where(x => x.Ref_No == Check && x.Delete == false);
            return Ok(list);
        }

        [HttpPost]
        public IActionResult DeleteMemberCard(int ID)
        {
            _memberCard.DeleteMember(ID);
            var list = _memberCard.GetMemberCards().ToList();
            return Ok(list);
        }

        [Privilege("A033")]
        public IActionResult Register()
        {
            ViewBag.style = "fa fa-percent";
            ViewBag.Main = "Promotion";
            ViewBag.Page = "Member";
            ViewBag.Subpage = "Register";
            ViewBag.PromotionMenu = "show";
            ViewBag.Member = "show";
            ViewBag.Register = "highlight";
            ViewData["CardTypeID"] = new SelectList(_context.CardTypes.Where(x => x.Delete == false), "ID", "Name");
            return View();

        }
    }
}
