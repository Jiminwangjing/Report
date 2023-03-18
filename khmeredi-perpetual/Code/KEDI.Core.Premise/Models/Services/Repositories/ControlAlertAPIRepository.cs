using CKBS.AppContext;
using CKBS.Models.Services.AlertManagement;
using CKBS.Models.Services.Inventory;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Responsitory
{
    public interface IControlAlertAPIRepository
    {
        Task<List<StockAlert>> GetStockAlertByCodeAsync(string code);
        Task<List<StockAlert>> GetStockAlertByNameAsync(string name);
        Task<List<ItemMasterData>> GetItemsAsync(bool active, bool inactive);
    }

    public class ControlAlertAPIRepository : IControlAlertAPIRepository
    {
        private readonly DataContext _context;

        public ControlAlertAPIRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<List<StockAlert>> GetStockAlertByCodeAsync(string code)
        {
            var data = await (from wh in _context.Warehouses.Where(i => !i.Delete)
                              join ia in _context.ItemAccountings on wh.ID equals ia.WarehouseID
                              join item in _context.ItemMasterDatas.Where(i => !i.Delete && i.Code.ToLower() == code.ToLower()) on ia.ItemID equals item.ID
                              select new StockAlert
                              {
                                  ID = 0,
                                  ItemID = item.ID,
                                  InStock = ia.InStock,
                                  IsRead = false,
                                  ItemName = item.EnglishName,
                                  MaxInv = ia.MaximunInventory,
                                  MinInv = ia.MinimunInventory,
                                  WarehouseID = (int)ia.WarehouseID,
                                  WarehouseName = wh.Name,
                                  Image = item.Image ?? "no-image.jpg",
                              }).ToListAsync();
            return data;
        }
        public async Task<List<StockAlert>> GetStockAlertByNameAsync(string name)
        {
            var data = await (from wh in _context.Warehouses.Where(i => !i.Delete)
                              join ia in _context.ItemAccountings on wh.ID equals ia.WarehouseID
                              join item in _context.ItemMasterDatas.Where(i => !i.Delete && i.EnglishName.ToLower() == name.ToLower()) on ia.ItemID equals item.ID
                              select new StockAlert
                              {
                                  ID = 0,
                                  ItemID = item.ID,
                                  InStock = ia.InStock,
                                  IsRead = false,
                                  ItemName = item.EnglishName,
                                  MaxInv = ia.MaximunInventory,
                                  MinInv = ia.MinimunInventory,
                                  WarehouseID = (int)ia.WarehouseID,
                                  WarehouseName = wh.Name,
                                  Image = item.Image ?? "no-image.jpg",
                              }).ToListAsync();
            return data;
        }
        public async Task<List<ItemMasterData>> GetItemsAsync(bool active, bool inactive)
        {
            if (active) return await _context.ItemMasterDatas.Where(i => !i.Delete && i.Inventory).ToListAsync();
            else if (inactive) return await _context.ItemMasterDatas.Where(i => i.Delete && i.Inventory).ToListAsync();
            else return await _context.ItemMasterDatas.Where(w=>w.Inventory).ToListAsync();
        }                                                                               
    }
}
