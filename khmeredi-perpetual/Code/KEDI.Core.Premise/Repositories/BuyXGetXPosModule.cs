using CKBS.AppContext;
using KEDI.Core.Premise.Models.Services.LoyaltyProgram.BuyXGetX;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Repository
{
    public class BuyXGetXPosModule
    {
        readonly ILogger<BuyXGetXPosModule> _logger;
        readonly DataContext _dataContext;
        public BuyXGetXPosModule(ILogger<BuyXGetXPosModule> logger, DataContext dataContext)
        {
            _logger = logger;
            _dataContext = dataContext;
        }

        public BuyXGetX FindBuyXGetX(int priceListId)
        {
            var item = _dataContext.BuyXGetXes.FirstOrDefault(bx => bx.PriListID == priceListId && bx.Active
                 && IsBetweenDate(bx.DateF, bx.DateT)) ?? new BuyXGetX();
            return item;
        }

        public async Task<List<BuyXGetXDetailModel>> GetBuyXGetXDetailsAsync(int priceListId)
        {
            var buyXgetX = FindBuyXGetX(priceListId);
            var items = (from bxd in _dataContext.BuyXGetXDetails.Where(bxd => bxd.BuyXGetXID == buyXgetX.ID)
                         join itm in _dataContext.ItemMasterDatas.Where(i => !i.Delete)
                         on bxd.BuyItemID equals itm.ID
                         join uom in _dataContext.UnitofMeasures.Where(u => !u.Delete)
                         on bxd.ItemUomID equals uom.ID
                         let promoItem = _dataContext.ItemMasterDatas.FirstOrDefault(i => i.ID == bxd.GetItemID)
                         let promoUom = _dataContext.UnitofMeasures.FirstOrDefault(u => u.ID == bxd.GetUomID)
                         select new BuyXGetXDetailModel
                         {
                             LineID = bxd.LineID,
                             ProCode = bxd.Procode,
                             BuyXGetXID = bxd.BuyXGetXID,
                             BuyItemID = bxd.BuyItemID,
                             ItemCode = itm.Code,
                             ItemName = itm.KhmerName,
                             BuyItemName = itm.KhmerName,
                             BuyQty = bxd.BuyQty,
                             ItemUomID = bxd.ItemUomID,
                             UoM = uom.Name,
                             Item = "<i class='fas fa-long-arrow-alt-right'></i>",
                             GetItemID = bxd.GetItemID,
                             GetItemCode = itm.Code,
                             GetItemName = promoItem.KhmerName,
                             GetQty = bxd.GetQty,
                             GetUomID = bxd.GetUomID,
                             GetUomName = promoUom.Name
                         }).ToList();
            return await Task.FromResult(items);
        }

        private bool IsBetweenDate(DateTime dateFrom, DateTime dateTo)
        {
            var now = DateTime.Now;
            return dateFrom.CompareTo(now) <= 0 && dateTo.CompareTo(now) >= 0;
        }

        private string RawWord(string keyword)
        {
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                return Regex.Replace(keyword, "\\+s", string.Empty);
            }
            return string.Empty;
        }
    }
}
