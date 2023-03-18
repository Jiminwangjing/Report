using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CKBS.AppContext;
using CKBS.Models.ServicesClass;
using KEDI.Core.Premise.Authorization;
using Newtonsoft.Json;
using Models.Services.LoyaltyProgram.PromotionDiscount;

namespace CKBS.Controllers
{
    [Privilege]
    public class DiscountItemController : Controller
    {
        private readonly DataContext _context;
        public DiscountItemController(DataContext context)
        {
            _context = context;
        }
        [HttpGet]
        [Privilege("A036")]
        public IActionResult Index()
        {
            ViewBag.style = "fa fa-percent";
            ViewBag.Main = "Promotion";
            ViewBag.Page = "Discount Item";
            ViewBag.Subpage = "List";
            ViewBag.PromotionMenu = "show";
            ViewBag.DiscountItem = "highlight";
            return View();
        }

         public IActionResult GetItems(int PriceListID, int Group1, int Group2, int Group3, int PromoID)
        {
            IEnumerable<DiscountItemDetail> items = _context.DiscountItemDetail.FromSql("sp_GetItemDiscount @PriceListID={0},@Group1={1},@Group2={2},@Group3={3},@PromotionId={4}",
             parameters: new[] {
                    PriceListID.ToString(),
                    Group1.ToString(),
                    Group2.ToString(),
                    Group3.ToString(),
                    PromoID.ToString()      
             }).ToList();
            return Ok(items);
        }

        [HttpGet]
        public IActionResult GetPromotion()
        {
            var promotion = _context.Promotions;
            return Ok(promotion);
        }

        public IActionResult GetPriceList()
        {
            var pricelist = _context.PriceLists.Where(w => w.Delete == false);
            return Ok(pricelist);
        }

        public IActionResult GetGroup1()
        {
            var groups = _context.ItemGroup1.Where(w => w.Delete == false);
            return Ok(groups);
        }

        public IActionResult GetGroup2(int group1)
        {
            var groups = _context.ItemGroup2.Where(w => w.ItemG1ID == group1 && w.Delete == false && w.Name != "None");
            return Ok(groups);
        }

        public IActionResult GetGroup3(int group1, int group2)
        {
            var groups = _context.ItemGroup3.Where(w => w.ItemG1ID == group1 && w.ItemG2ID == group2 && w.Delete == false && w.Name != "None");
            return Ok(groups);
        }
        [HttpPost]
        public IActionResult SetPromotionDisountItem(string data, string type)
        {
            DiscountItem discountItem = JsonConvert.DeserializeObject<DiscountItem>(data, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            using var t = _context.Database.BeginTransaction();
            var dateNow = DateTime.Now;
            var promo = _context.Promotions.AsNoTracking().FirstOrDefault(s => s.ID == discountItem.PromotionID);
             
            foreach (var item in discountItem.DiscountItemDetails.ToList())
            {
                _ = float.TryParse(item.Discount.ToString(), out float discount);
                var item_update = _context.PriceListDetails.Find(item.ID);
                //=================update pricelistdetail=======================
                item_update.PromotionID = discountItem.PromotionID;
                item_update.Discount = discount;
                item_update.TypeDis = type;
                _context.PriceListDetails.Update(item_update);
                _context.SaveChanges();
                //=======================add to promotion detail===============
                var datapro=_context.PromotionDetails.Where(x=>x.ItemID==item_update.ItemID && x.PromotionID==discountItem.PromotionID).ToList();
                 if (datapro.Count > 0)
                {
                    foreach (var pr in datapro)
                    {
                        pr.TypeDis = type;
                        pr.Discount = discount;
                        pr.StartDate = promo.StartDate;
                        pr.StopDate = promo.StopDate;
                        pr.StartTime = promo.StartTime;
                        pr.StopTime = promo.StopTime;
                    }
                    _context.UpdateRange(datapro);
                    _context.SaveChanges();
                }
                else {
                    PromotionDetail promotionDetail=new();
                    promotionDetail.ID=0;
                    promotionDetail.PriceListID=item_update.PriceListID;
                    promotionDetail.Cost=item_update.Cost;
                    promotionDetail.UnitPrice=item_update.UnitPrice;
                    promotionDetail.CurrencyID=item_update.CurrencyID;
                    promotionDetail.ItemID=item_update.ItemID;
                    promotionDetail.UserID=item_update.UserID;
                    promotionDetail.UomID=(int)item_update.UomID;
                    promotionDetail.Discount=discount;
                    promotionDetail.TypeDis=type;
                    promotionDetail.PromotionID=discountItem.PromotionID;
                    promotionDetail.StartDate=promo.StartDate;
                    promotionDetail.StopDate=promo.StopDate;
                    promotionDetail.StartTime=promo.StartTime;
                    promotionDetail.StopTime=promo.StopTime;
                   _context.PromotionDetails.Update(promotionDetail);
                    _context.SaveChanges();
                }

            }

            t.Commit();
            return Ok('Y');
        }
      
        // [HttpPost]
        // public IActionResult SetPromotionDisountItem(string data, string type)
        // {
        //     DiscountItem discountItem = JsonConvert.DeserializeObject<DiscountItem>(data, new JsonSerializerSettings
        //     {
        //         NullValueHandling = NullValueHandling.Ignore
        //     });
        //     using var t = _context.Database.BeginTransaction();
        //     var dateNow = DateTime.Now;
        //     var promo = _context.Promotions.AsNoTracking().FirstOrDefault(s => s.ID == discountItem.PromotionID);
        //     foreach (var item in discountItem.DiscountItemDetails.ToList())
        //     {
        //         _ = float.TryParse(item.Discount.ToString(), out float discount);
        //         var item_update = _context.PriceListDetails.Find(item.ID);
        //         //  var checkPromotion = _context.PromotionDetails.FirstOrDefault(s => s.PromotionID == discountItem.PromotionID && s.ItemID == item.ID);
        //         var prodd = _context.PromotionDetails.Where(x => x.PriceListID == item_update.ID && x.PromotionID==discountItem.PromotionID).ToList();
        //         //=================update pricelistdetail=======================
        //         item_update.PromotionID = discountItem.PromotionID;
        //         item_update.Discount = discount;
        //         item_update.TypeDis = type;
        //         _context.PriceListDetails.Update(item_update);
        //         _context.SaveChanges();
        //         PromotionDetail promotion = new();
        //         if (prodd.Count > 0)
        //         {
        //             foreach (var pr in prodd)
        //             {

        //                 pr.ItemID = item.ID;
        //                 pr.UserID = item_update.UserID;
        //                 pr.PriceListID = item_update.ID;
        //                 pr.PromotionID = discountItem.PromotionID;
        //                 pr.TypeDis = type;
        //                 pr.Discount = discount;
        //                 pr.StartDate = promo.StartDate;
        //                 pr.StopDate = promo.StopDate;

        //             }
        //             _context.UpdateRange(prodd);
        //             _context.SaveChanges();
        //         }
        //         else if (prodd.Count <= 0 && discount > 0)
        //         {
        //             promotion.ID = 0;
        //             promotion.ItemID = item.ID;
        //             promotion.UserID = item_update.UserID;
        //             promotion.PriceListID = item_update.ID;
        //             promotion.PromotionID = discountItem.PromotionID;
        //             promotion.TypeDis = type;
        //             promotion.Discount = discount;
        //             promotion.StartDate = promo.StartDate;
        //             promotion.StopDate = promo.StopDate;
        //             _context.PromotionDetails.Add(promotion);
        //             _context.SaveChanges();
        //         }

        //     }

        //     t.Commit();
        //     return Ok('Y');
        // }

    }
}