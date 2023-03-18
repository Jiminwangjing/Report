
using CKBS.Models.Services.Administrator.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Responsitory
{
    public interface IWarehouseDetail
    {
        IQueryable<WarehouseDetail> WarehouseDetails { get; }
        IEnumerable<WarehouseDetail> WarehouseDetail { get; }
        WarehouseDetail GetId(int id);
        Task AddOrEdit(WarehouseDetail warehouse);
        Task Delete(int id);
    }
    public class WarehouseDetailRepository : IWarehouseDetail
    {
        public IQueryable<WarehouseDetail> WarehouseDetails => throw new NotImplementedException();

        public IEnumerable<WarehouseDetail> WarehouseDetail => throw new NotImplementedException();

        public Task AddOrEdit(WarehouseDetail warehouse)
        {
            throw new NotImplementedException();
        }

        public Task Delete(int id)
        {
            throw new NotImplementedException();
        }

        public WarehouseDetail GetId(int id)
        {
            throw new NotImplementedException();
        }
    }
}
