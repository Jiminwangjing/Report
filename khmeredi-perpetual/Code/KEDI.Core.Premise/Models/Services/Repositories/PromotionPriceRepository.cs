using CKBS.AppContext;
using CKBS.Models.Services.Banking;
using CKBS.Models.Services.Promotions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Responsitory
{
    public interface IPromitionPrice
    {
        IQueryable<PromotionPrice> GetPromotionPrice();
        Task AddOrEdit(PromotionPrice promotionPrice);
        Task<int> Delete(int? id);
    }
    public class PromotionPriceRepository :IPromitionPrice
    {
        private readonly DataContext _dataContext;
        public PromotionPriceRepository (DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task AddOrEdit(PromotionPrice promotionPrice)
        {
            if (promotionPrice.ID == 0)
            {
                await _dataContext.PromotionPrice.AddAsync(promotionPrice);
            }
            else
            {
                _dataContext.PromotionPrice.Update(promotionPrice);
            }
            await _dataContext.SaveChangesAsync();
        }

        public async Task<int> Delete(int? id)
        {
            var com = await _dataContext.PromotionPrice.FindAsync(id);
            if (com != null)
            {
                com.Delete = true;
                _dataContext.PromotionPrice.Update(com);
            }
            return await _dataContext.SaveChangesAsync();
        }

        public IQueryable<PromotionPrice> GetPromotionPrice()
        {
            IQueryable<PromotionPrice> List = (from pp in _dataContext.PromotionPrice.Where(x => x.Delete == false)
                                               join cu in _dataContext.Currency.Where(x => x.Delete == false)
                                               on pp.CurrencyID equals cu.ID
                                               where pp.Delete == false
                                               select new PromotionPrice
                                               {
                                                   ID = pp.ID,
                                                   Name = pp.Name,
                                                   Amount = pp.Amount,
                                                   Discount = pp.Discount,
                                                   CurrencyID = pp.CurrencyID,
                                                   TypeDis = pp.TypeDis,
                                                   Currency = new Currency
                                                   {
                                                       Description = cu.Description + " " + cu.Symbol
                                                   }
                                               }
                                               );
            return List;
            
        }
    }
}
