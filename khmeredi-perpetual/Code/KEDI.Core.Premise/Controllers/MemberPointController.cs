using KEDI.Core.Models.Validation;
using KEDI.Core.Premise.Authorization;
using KEDI.Core.Premise.Models.Services.LoyaltyProgram.MemberPoints;
using KEDI.Core.Premise.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Controllers
{
    [Privilege]
    public class MemberPointController : Controller
    {
        readonly ILogger<MemberPointController> _logger;
        readonly MemberPointModule _memberPoint;
        public MemberPointController(ILogger<MemberPointController> logger, MemberPointModule memberPoint)
        {
            _logger = logger;
            _memberPoint = memberPoint;
        }

        public async Task<IActionResult> PointSetup()
        {
            ViewBag.PointSetup = "highlight";
            ViewBag.PointCard = "active";
            var pointSetup = await _memberPoint.GetPointSetupAsync();
            return View(pointSetup);
        }
        [HttpPost]
        public async Task<IActionResult> FindPointItem(int itemId, int priceListId = 0)
        {
            var pointItem = await _memberPoint.FindPointItemAsync(itemId);
            return Ok(pointItem);
        }

        [HttpPost]
        public async Task<IActionResult> GetItemMasters(int priceListId = 0)
        {
            var items = await _memberPoint.GetItemMastersAsync(priceListId);
            return Ok(items);
        }

        [HttpPost]
        public async Task<IActionResult> GetCustomers()
        {
            var customers = await _memberPoint.GetCustomersAsync();
            return Ok(customers);
        }

        [HttpPost]
        public async Task<IActionResult> FindPointCard(int pointCardId = 0)
        {
            var pointCard = await _memberPoint.FindPointCardAsync(pointCardId);
            return Ok(pointCard);
        }

        [HttpPost]
        public async Task<IActionResult> GetPointCards()
        {
            var pointCards = await _memberPoint.GetPointCardsAsync();
            return Ok(pointCards);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SavePointCards(List<PointCard> pointCards)
        {
            ViewBag.PointSetup = "highlight";
            ViewBag.PointCard = "active";
            pointCards = await _memberPoint.SavePointCardsAsync(pointCards, ModelState);
            var _pointcards = await _memberPoint.GetPointCardsAsync();
            _pointcards.ForEach(pc =>
            {
                pc.PriceLists = _memberPoint.SelectPriceLists(pc.PriceListID, false);
            });
            var message = new ModelMessage(ModelState);
            if (ModelState.IsValid)
            {
                message.Add("PointCard", "Saving completed...");
                message.AddItem(_pointcards, "PointCards");
                message.Approve();
            }

            return Ok(message);
        }

        [HttpPost]
        public async Task<IActionResult> GetPointRedemptions()
        {
            var redempts = await _memberPoint.GetPointRedemptsAsync();
            return Ok(redempts);
        }
        [HttpPost]
        public IActionResult RemoveRedemptionPointDetail(int id)
        {
            var data = _memberPoint.RemoveRedemptionPointDetail(id);
            return Ok(data);
        }
        [HttpPost]
        public async Task<IActionResult> FindPointRedempt(int pointRedemptId)
        {
            var redemptions = await _memberPoint.FindPointRedemptAsync(pointRedemptId);
            return Ok(redemptions);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitPointRedemption(PointRedemption redemption)
        {
            ViewBag.PointSetup = "highlight";
            ViewBag.PointRedempt = "active";

            await _memberPoint.SubmitPointRedemptionAsync(redemption, ModelState);
            var message = new ModelMessage(ModelState);
            var redempts = await _memberPoint.GetPointRedemptsAsync();
            if (ModelState.IsValid)
            {
                message.Add("PointRedemption", "Saving completed...");
                message.AddItem(redempts, "PointRedemptions");
                message.Approve();
            }
            return Ok(message);
        }
    }
}
