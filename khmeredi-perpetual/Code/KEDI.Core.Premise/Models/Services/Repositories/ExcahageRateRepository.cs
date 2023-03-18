
using Microsoft.EntityFrameworkCore;
using CKBS.AppContext;
using CKBS.Models.Services.Banking;
using CKBS.Models.SeverClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Responsitory
{
    public interface IExchangeRate
    {
        IEnumerable<ExchangeRate> GetListCurrency();
        IEnumerable<Currency> GetBaseCurrencyName();
        void UpdateExchangeRate(ExchangeRateService data);
    }
    public class ExcahageRateRepository : IExchangeRate
    {
        private readonly DataContext _context;
        public ExcahageRateRepository(DataContext dataContext)
        {
            _context = dataContext;
        }

        public IEnumerable<ExchangeRate> GetListCurrency()
        {
            var exchanges = _context.ExchangeRates.Include(e => e.Currency).Where(e => !e.Delete);
            return exchanges;
        }

        public IEnumerable<Currency> GetBaseCurrencyName()
        {
            IEnumerable<Currency> name = from company in _context.Company.Where(d => d.Delete == false)                      
                       join SysCur in _context.Currency.Where(d => d.Delete == false) on company.SystemCurrencyID equals SysCur.ID
                       select new Currency
                       {
                           ID = company.ID,
                           Description = SysCur.Description + " ( " + SysCur.Symbol + " )"
                       };

            return name;
        }

        public void UpdateExchangeRate(ExchangeRateService data)
        {
            foreach (var item in data.ExchangeRates)
            {
                if (item.SetRate >0)
                {
                    _context.Update(item);
                    _context.SaveChanges();
                }
                
            }
        }
    }
}
