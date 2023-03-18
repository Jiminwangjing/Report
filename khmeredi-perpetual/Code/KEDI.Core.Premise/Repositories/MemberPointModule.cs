using CKBS.AppContext;
using CKBS.Models.Services.HumanResources;
using CKBS.Models.Services.Inventory;
using CKBS.Models.Services.Responsitory;
using KEDI.Core.Premise.Models.Services.LoyaltyProgram.MemberPoints;
using KEDI.Core.Security.Cryptography;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Repository
{
    public class MemberPointModule
    {
        readonly ILogger<MemberPointModule> _logger;
        readonly DataContext _dataContext;
        readonly IBusinessPartner _partner;
        readonly IItemMasterData _itemMaster;
        public MemberPointModule(ILogger<MemberPointModule> logger, DataContext dataContext, IBusinessPartner partner,
        IItemMasterData itemMaster)
        {
            _logger = logger;
            _dataContext = dataContext;
            _partner = partner;
            _itemMaster = itemMaster;
        }

        private static bool IsBetweenDate(DateTime dateFrom, DateTime dateTo)
        {
            var now = DateTime.Now;
            return dateFrom.CompareTo(now) <= 0 && dateTo.CompareTo(now) >= 0;
        }

        public async Task<PointRedemption> FindPointRedemptAsync(int pointRedemptId)
        {
            var pointRedempt = _dataContext.PointRedemptions.FirstOrDefault(pr => pr.ID == pointRedemptId);
            pointRedempt.PointItems = await _dataContext.PointItems.Where(pt => pt.PointRedemptID == pointRedempt.ID && !pt.Deleted).ToListAsync();
            pointRedempt?.PointItems.ToList().ForEach(pi =>
            {
                var item = _dataContext.ItemMasterDatas.Find(pi.ItemID) ?? new ItemMasterData();
                pi.ItemCode = item.Code;
                pi.ItemName = item.KhmerName;
                pi.ItemUoms = _itemMaster.SelectItemUoms(item.GroupUomID, pi.UomID);
            });

            return await Task.FromResult(pointRedempt);
        }

        public async Task<List<PointRedemption>> GetPointRedemptsAsync()
        {
            var pointRedempts = await _dataContext.PointRedemptions.Include(pr => pr.PointItems).ToListAsync();
            return pointRedempts;
        }
        public dynamic RemoveRedemptionPointDetail(int id)
        {
            var pointRedempts = _dataContext.PointItems.FirstOrDefault(i => i.ID == id);
            if (pointRedempts == null)
            {
                return new { Error = true, Message = "Counld not remove item!" };
            }
            else
            {
                _dataContext.PointItems.Remove(pointRedempts);
                _dataContext.SaveChanges();
                return new { Error = false, Message = "Item removed successfully!" };
            }
        }
        public async Task<List<PointRedemption>> GetPointRedemptsWarehouseAsync(int warehouseId)
        {
            var pointRedempts = await _dataContext.PointRedemptions
                .Include(pr => pr.PointItems)
                .Where(pr => pr.PointItems.FirstOrDefault().WarehouseID == warehouseId)
                .Select(i => new PointRedemption
                {
                    Active = i.Active,
                    BasePoints = i.BasePoints,
                    Code = i.Code,
                    CustomerID = i.CustomerID,
                    DateFrom = i.DateFrom,
                    DateTo = i.DateTo,
                    Factor = i.Factor,
                    ID = 0,
                    LineID = i.ID,
                    PointItems = i.PointItems,
                    PointQty = i.PointQty,
                    Redeemed = i.Redeemed,
                    Title = i.Title,
                    WarehouseID = i.WarehouseID,
                })
                .ToListAsync();
            return pointRedempts;
        }
        // add new 
        public async Task<List<PointRedemption>> PostPointRedemptsAsync()
        {
            var pointRedempts = await _dataContext.PointRedemptions.Include(pr => pr.PointItems).ToListAsync();
            return pointRedempts;
        }
        public async Task<PointItem> FindPointItemAsync(int itemId)
        {
            var item = _itemMaster.GetbyId(itemId) ?? new ItemMasterData();
            var pointItem = new PointItem
            {
                LineID = item.ID.ToString(),
                ItemID = item.ID,
                WarehouseID = item.WarehouseID,
                ItemCode = item.Code,
                ItemName = item.KhmerName,
                ItemUoms = _itemMaster.SelectItemUoms(item.GroupUomID),

            };
            return await Task.FromResult(pointItem);
        }

        public async Task<List<ItemMasterData>> GetItemMastersAsync(int priceListId = 0)
        {
            return await _itemMaster.GetItemMastersAsync(priceListId);
        }

        public async Task<List<BusinessPartner>> GetCustomersAsync()
        {
            var customers = await _partner.GetCustomersAsync();
            return await Task.FromResult(customers);
        }

        public async Task<PointCard> FindPointCardAsync(int pointCardId)
        {
            var pointCard = await _dataContext.PointCards.FindAsync(pointCardId);
            if (pointCard == null)
            {
                return await CreatePointCardAsync(pointCardId);
            }
            pointCard.PriceLists = SelectPriceLists(pointCard.PriceListID, pointCardId == pointCard?.ID);
            return pointCard;
        }

        public async Task<List<PointCard>> GetPointCardsAsync()
        {
            var pointCards = await _dataContext.PointCards.ToListAsync();
            pointCards.ForEach(pc =>
            {
                pc.PriceLists = SelectPriceLists(pc.PriceListID, false);
            });

            return pointCards;
        }

        public async Task<PointCard> CreatePointCardAsync(int priceListId = 0)
        {
            var pointCard = new PointCard
            {
                Code = CreatePointCardCode("PTC"),
                LineID = DateTime.Now.Ticks.ToString(),
                PriceLists = SelectPriceLists(priceListId),
                DateFrom = DateTime.Today,
                DateTo = DateTime.Today
            };
            return await Task.FromResult(pointCard);
        }

        public async Task<PointSetup> GetPointSetupAsync()
        {
            var pointSetup = new PointSetup
            {
                Redemption = new PointRedemption
                {
                    Code = CreatePointRedemptCode("PTR"),
                    DateFrom = DateTime.Today,
                    DateTo = DateTime.Today
                }
            };
            return await Task.FromResult(pointSetup);
        }

        private bool EqualNotEmpty(string a, string b, bool ignoreCase = false)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(a) && string.IsNullOrWhiteSpace(b))
                {
                    return false;
                }
                a = Regex.Replace(a, "\\s+", "");
                b = Regex.Replace(b, "\\s+", "");
                return string.Compare(a, b, ignoreCase) == 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
        }

        public string CreateRedeemCode()
        {
            var code = HashFactory.RandomizeKey(10, true, Guid.NewGuid().ToString("N"));
            return code;
        }

        public async Task<List<PointCard>> SavePointCardsAsync(IEnumerable<PointCard> pointcards, ModelStateDictionary modelState)
        {
            try
            {
                using var t = await _dataContext.Database.BeginTransactionAsync();
                if (pointcards.Where(c => c.Active).Count() > 1)
                {
                    modelState.AddModelError("Active", "Active field can be activated only one.");
                }

                var activePointcard = pointcards.FirstOrDefault(p => p.Active);
                List<PointCard> _pointcards = new();
                foreach (var m in pointcards.ToList())
                {
                    if (_pointcards.Any(p => EqualNotEmpty(p?.Code, m?.Code, true)))
                    {
                        modelState.AddModelError("Code", "Code is duplicated.");
                    }
                    else
                    {
                        _pointcards.Add(m);
                    }

                    ValidatePointCard(m, modelState);
                }

                if (modelState.IsValid)
                {
                    _dataContext.PointCards.UpdateRange(pointcards);
                }

                _dataContext.PointCards.ToList().ForEach(p =>
                {
                    if (activePointcard?.ID != p.ID)
                    {
                        p.Active = false;
                    }
                });
                await _dataContext.SaveChangesAsync();
                t.Commit();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                modelState.AddModelError("Exception", ex.Message);
            }

            return await Task.FromResult(pointcards.ToList());
        }

        private ModelStateDictionary ValidatePointCard(PointCard pointCard, ModelStateDictionary modelState)
        {
            try
            {
                if (_dataContext.PointCards.Any(c => EqualNotEmpty(c.Code, pointCard.Code, true)))
                {
                    if (pointCard.ID <= 0)
                    {
                        modelState.AddModelError("Code", "Field [Code] is already existed.");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                modelState.AddModelError("Exception", ex.Message);
            }
            return modelState;
        }

        public string CreatePointCardCode(string prefix, int padLength = 3)
        {
            return prefix.ToUpper() + (_dataContext.PointCards.Count() + 1).ToString().PadLeft(padLength, '0');
        }

        public string CreatePointRedemptCode(string prefix, int padLength = 3)
        {
            return prefix.ToUpper() + (_dataContext.PointRedemptions.Count() + 1).ToString().PadLeft(padLength, '0');
        }

        public List<SelectListItem> SelectPriceLists(int selectedValue, bool disabled = false)
        {
            var priceLists = _dataContext.PriceLists.Where(p => !p.Delete)
                .Select(p => new SelectListItem
                {
                    Value = p.ID.ToString(),
                    Text = p.Name,
                    Selected = p.ID == selectedValue,
                    Disabled = disabled
                }).ToList();
            return priceLists;
        }

        public async Task<PointRedemption> SubmitPointRedemptionAsync(PointRedemption redemption, ModelStateDictionary modelState)
        {
            try
            {
                ValidatePointRedemption(redemption, modelState);
                if (modelState.IsValid)
                {
                    using var t = await _dataContext.Database.BeginTransactionAsync();
                    if (redemption.ID <= 0)
                    {
                        await _dataContext.PointRedemptions.AddAsync(redemption);
                        await _dataContext.SaveChangesAsync();
                        foreach (var pi in redemption.PointItems.ToList())
                        {
                            pi.PointRedemptID = redemption.ID;
                        }

                    }
                    else
                    {
                        _dataContext.PointRedemptions.Update(redemption);
                    }

                    await _dataContext.SaveChangesAsync();
                    t.Commit();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                modelState.AddModelError("Exception", ex.Message);
            }

            return redemption;
        }

        private ModelStateDictionary ValidatePointRedemption(PointRedemption redemption, ModelStateDictionary modelState)
        {

            if (redemption.ID <= 0)
            {
                if (_dataContext.PointRedemptions.ToList().Any(pr => EqualNotEmpty(pr.Code, redemption.Code, true)))
                {
                    modelState.AddModelError("Code", $"{redemption.Code} already existed.");
                }
            }
            return modelState;
        }

        public async Task<PointCard> FindActivePointCardAsync(int priceListId)
        {
            // var pointcard = _dataContext.PointCards.FirstOrDefault(pc => pc.Active
            //     && pc.PriceListID == priceListId) ?? new PointCard();
            var pointcard = _dataContext.PointCards.FirstOrDefault(pc => pc.Active
          && pc.PriceListID == priceListId && IsBetweenDate(pc.DateFrom, pc.DateTo)) ?? new PointCard();
            return await Task.FromResult(pointcard);
        }
    }
}
