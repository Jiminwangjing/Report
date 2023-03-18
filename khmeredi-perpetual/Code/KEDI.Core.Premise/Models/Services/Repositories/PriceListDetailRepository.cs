
using CKBS.Models.Services.Inventory.PriceList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Responsitory
{
    public interface IPriceListDetail
    {
        IQueryable<PriceListDetail> PriceListDetails { get; }
        IEnumerable<PriceListDetail> PriceListDetail { get; }
        PriceListDetail GetId(int id);
        Task AddOrEdit(PriceListDetail priceListDetail);
        Task Delete(int id);
    }
    public class PriceListDetailRepository : IPriceListDetail
    {
        public IQueryable<PriceListDetail> PriceListDetails => throw new NotImplementedException();

        public IEnumerable<PriceListDetail> PriceListDetail => throw new NotImplementedException();

        public Task AddOrEdit(PriceListDetail priceListDetail)
        {
            throw new NotImplementedException();
        }

        public Task Delete(int id)
        {
            throw new NotImplementedException();
        }

        public PriceListDetail GetId(int id)
        {
            throw new NotImplementedException();
        }
    }
}
