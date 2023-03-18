using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using CKBS.AppContext;
using CKBS.Models.Services.Administrator.General;
using CKBS.Models.Services.Administrator.Inventory;
using CKBS.Models.ServicesClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Responsitory
{
    public interface IWarehouse
    { 
        IEnumerable <Warehouse> Warehouse { get; }
        IQueryable<Warehouse> Warehouses();
       
        Warehouse GetId(int id);
        Task AddOrEdit(Warehouse warehouse);
        Task Delete(int id);
        Task SetDefault(int id);
        IEnumerable<ServicePriceListCopyItem> GetItemMasterToCopy(int from_whid,int to_whid, int group1, int group2, int group3);
        void InsertIntoPricelist(ItemCopyToWH itemCopyToPriceList, int userid);
    }
    public class WarehouseRepository : IWarehouse
    {
        private readonly DataContext _context;
        public WarehouseRepository(DataContext context)
        {
            _context = context;
        }
        public IEnumerable<Warehouse> Warehouse => _context.Warehouses.Where(d=>d.Delete==false);

        public IQueryable<Warehouse> Warehouses()
        {
            IQueryable<Warehouse> list = (from w in _context.Warehouses.Where(x => x.Delete == false)
                        join b in _context.Branches.Where(x => x.Delete == false) on
                        w.BranchID equals b.ID
                        where w.Delete == false
                        select new Warehouse
                        {
                            ID = w.ID,
                            Code = w.Code,
                            Name = w.Name,
                            BranchID = w.BranchID,
                            StockIn = w.StockIn,
                            Address = w.Address,
                            Location = w.Location,
                            Branch = new Branch
                            {
                                ID = b.ID,
                                Name = b.Name
                            }

                        }
                      );
            return list;
        }

        public async Task AddOrEdit(Warehouse warehouse)
        {
            if (warehouse.ID == 0)
            {
               
                await _context.Warehouses.AddAsync(warehouse);
                await _context.SaveChangesAsync();
            }
            else
            {
                 _context.Warehouses.Update(warehouse);
                await _context.SaveChangesAsync();
            }
        }

        public async Task Delete(int id)
        {
            Warehouse warehouses = await _context.Warehouses.FirstAsync(w=>w.ID==id);
            warehouses.Delete = true;
            _context.Update(warehouses);
            await _context.SaveChangesAsync();
        }

        public Warehouse GetId(int id)
        {
            Warehouse warehouses = _context.Warehouses.Find(id);
            return  warehouses;
        }

        public async Task SetDefault(int id)
        {
            List<Warehouse> warehouses = _context.Warehouses.Where(w=>w.Delete==false).ToList();
            foreach (var item in warehouses)
            {
               
                _context.Update(item);
                await _context.SaveChangesAsync();
            }
           
        }
        public IEnumerable<ServicePriceListCopyItem> GetItemMasterToCopy(int from_whid,int to_whid, int group1, int group2, int group3) =>

          _context.ServicePriceListCopyItem.FromSql("sp_GetItemMasterToCopyWH @FromWhID={0},@ToWhID={1},@Group1={2},@Group2={3},@Group3={4}",
             parameters: new[] {
                    from_whid.ToString(),
                    to_whid.ToString(),
                    group1.ToString(),
                    group2.ToString(),
                    group3.ToString()
             });
        public void InsertIntoPricelist(ItemCopyToWH itemCopyToWH, int userid)
        {
            if (itemCopyToWH.ItemCopyToWHDetail.Count() == 0)
            {
                _context.Database.ExecuteSqlCommand("sp_InsertWarehouseDatail @Process={0},@ToWHID={1},@UserID={2}",
                parameters: new[] {
                    "A", //insert all item
                    itemCopyToWH.ToWHID.ToString(),
                    userid.ToString()

                });
                _context.SaveChanges();
            }
            else
            {
                _context.ItemCopyToWH.Add(itemCopyToWH);
                _context.SaveChanges();

                _context.Database.ExecuteSqlCommand("sp_InsertWarehouseDatail @Process={0},@ToWHID={1},@UserID={2}",
               parameters: new[] {
                    "S", //insert spcific item
                    itemCopyToWH.ToWHID.ToString(),
                     userid.ToString()

                });

            }
        }
    }
}
