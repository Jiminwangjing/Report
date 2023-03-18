
using CKBS.AppContext;
using CKBS.Models.Services.Banking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Responsitory
{
    public interface ITax
    {
        IQueryable<Tax> GetTax { get; }
        IEnumerable<Tax> Tax { get; }
        Tax GetID(int id);
        Task AddOrEdit(Tax tax);
        Task<int> DeleteTax(int? id);
    }

    public class TaxRepository : ITax
    {
        private readonly DataContext _Context;
        public TaxRepository(DataContext dataContext)
        {
            _Context = dataContext;
        }
        public IQueryable<Tax> GetTax => _Context.Tax.Where( t=> t.Delete== false);

        public IEnumerable<Tax> Tax => throw new NotImplementedException();

        public Task AddOrEdit(Tax tax)
        {
            throw new NotImplementedException();
        }

        public async Task <int> DeleteTax(int? id)
        {
            var com = await _Context.Tax.FindAsync(id);
            if (com != null)
            {
                com.Delete = true;
                _Context.Tax.Update(com);
            }
            return await _Context.SaveChangesAsync();
        }

        public Tax GetID(int id)
        {
            throw new NotImplementedException();
        }
    }
}
