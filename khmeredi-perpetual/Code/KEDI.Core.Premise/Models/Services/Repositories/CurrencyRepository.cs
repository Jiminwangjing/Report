using Microsoft.EntityFrameworkCore;
using CKBS.AppContext;
using CKBS.Models.Services.Banking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Responsitory
{
    public interface ICurrency
    {
        IQueryable<Currency> Cuurencies();
        Currency GetId(int id);
        Task AddorEdit(Currency currency);
        Task<int> Delete(int? id);
    }
    public class CurrencyRepository : ICurrency
    {
        private readonly DataContext _context;
        public CurrencyRepository(DataContext dataContext)
        {
            _context = dataContext;
        }
        public async Task AddorEdit(Currency currency)
        {
            if (currency.ID == 0)
            {
                await _context.Currency.AddAsync(currency);
                await _context.SaveChangesAsync();
                ExchangeRate exchangeRate = new ExchangeRate
                {

                    CurrencyID = currency.ID,
                    Rate = 0
                    
                };
              await  _context.ExchangeRates.AddAsync(exchangeRate);
              await _context.SaveChangesAsync();
            }
            else
            {
                var cur =await _context.Currency.FirstAsync(c => c.ID == currency.ID);
                cur.Symbol = currency.Symbol;
                cur.Description = currency.Description;
                _context.Currency.Update(cur);
                await _context.SaveChangesAsync();
            }
            
        }

        public async Task<int> Delete(int? id)
        {
            var cur = await _context.Currency.FirstAsync(c => c.ID == id);
            cur.Delete = true;
            _context.Currency.Update(cur);
            return await _context.SaveChangesAsync();
        }

        public Currency GetId(int id) => _context.Currency.Find(id);
       
        IQueryable<Currency> ICurrency.Cuurencies()
        {
            IQueryable<Currency> list = _context.Currency.Where(c => c.Delete == false);
            return list;
        }
    }
}
