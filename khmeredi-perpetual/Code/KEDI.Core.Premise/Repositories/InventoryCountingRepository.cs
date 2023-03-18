using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CKBS.AppContext;
using CKBS.Models.Services.Administrator.Inventory;
using CKBS.Models.Services.HumanResources;
using KEDI.Core.Premise.Models.Services.Inventory.Transaction;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace KEDI.Core.Premise.Models.Services.Repositories
{
    public interface IinventoryCounting
    {
        InventoryCountingDetail CreateDefualtRow();
        Task<List<InventoryCountingDetail>> BindRows();
        Task<List<ViewItemMasterData>> GetItemMaster(string barcode="");
        ViewItemMasterData GetStockFromWarehouse(int itemID, int wID,int uomID);
         Task<List<Employee>> GetEmployee();
        Task<InventoryCounting> FindInventoryCounting(string number);
    }
    public class InventoryCountingRepository:IinventoryCounting 
    {
        private readonly DataContext _context;
        public InventoryCountingRepository(DataContext context)
        {
            _context=context;
        }

        public async Task<List<InventoryCountingDetail>> BindRows()
        {
            List<InventoryCountingDetail> list=new List<InventoryCountingDetail>();
            for(int i= 0;i<10;i++)
            {
                list.Add(CreateDefualtRow());
            }
           return await Task.FromResult(list);
        }

        public InventoryCountingDetail CreateDefualtRow()
        {
               
            var warehouse= (from w in _context.Warehouses.Where(s=> !s.Delete)
                            select new
                            {
                                ID      =   w.ID,
                                Name    =   w.Name,
                            }).ToList();
                 warehouse.Insert(0,new {
                    ID=0,
                    Name="--- select item ---",
                 }) ;
                 
            var obj= new InventoryCountingDetail
            {
                LineID              =   DateTime.Now.Ticks.ToString(),
                ID                  =   0,
                InventoryCountingID =   0,
                EmployeeID          =   0,
                ItemID              =   0,
                WarehouseID         =   0,
                Barcode             =   "",
                ItemNo              =   "",
                ItemName            =   "",
                Warehouse           =   warehouse.Select(i=> new SelectListItem{
                                            Text = i.Name,
                                            Value   = i.ID.ToString(),
                                        }).ToList(),
                       
                InstockQty          =   0,
                Counted             =   false,
                UomCountQty         =   0,
                CountedQty          =   0,
                Varaince            =   0,
                UomName             =   "",
                UomID               =   0,
                EmName              =   "",
                Delete              =   false,

            };
            return obj;
        }

        public async Task<InventoryCounting> FindInventoryCounting(string number)
        {
            var warehouse= (from w in _context.Warehouses.Where(s=> !s.Delete)
                            select new
                            {
                                ID      =   w.ID,
                                Name    =   w.Name,
                            }).ToList();
                 warehouse.Insert(0,new {
                    ID=0,
                    Name="--- select item ---",
                 }) ;
            var obj= await(from inv in _context.InventoryCountings.Where(s=> s.InvioceNumber==number)
                        select new InventoryCounting
                        {
                            ID              = inv.ID,
                            BranchID        = inv.BranchID,
                            Date            = inv.Date,
                            Time            = inv.Time,
                            InvioceNumber   = inv.InvioceNumber,
                            Status          = inv.Status,
                            DocTypeID       = inv.DocTypeID,
                            Ref_No          = inv.Ref_No,
                            SeriesID        = inv.SeriesID,
                            Remark          = inv.Remark,
                            SeriesDetailID  = inv.SeriesDetailID,
                            InventoryCountingDetails    = (from invd in _context.InventoryCountingDetails.Where(s=> s.InventoryCountingID== inv.ID)
                                                            join item in _context.ItemMasterDatas on invd.ItemID equals item.ID
                                                            select new InventoryCountingDetail
                                                            {
                                                                ID          = invd.ID,
                                                                InventoryCountingID = invd.InventoryCountingID,
                                                                LineID      = invd.ID.ToString(),
                                                                Barcode     = item.Barcode,
                                                                ItemID      = invd.ItemID,
                                                                ItemNo      = item.Code,
                                                                ItemName    = string.IsNullOrWhiteSpace(item.KhmerName)?item.EnglishName:item.KhmerName,
                                                                WarehouseID = invd.WarehouseID,
                                                                InstockQty  = invd.InstockQty,
                                                                Counted     = invd.Counted,
                                                                UomCountQty = invd.CountedQty,
                                                                CountedQty  = invd.CountedQty,
                                                                Varaince    = invd.Varaince,
                                                                UomID       = invd.UomID,
                                                                UomName     = invd.UomName,
                                                                EmName      = invd.EmName,
                                                                EmployeeID  = invd.EmployeeID,
                                                                                                                                                                                
                                                                Warehouse   =   warehouse.Select(i=> new SelectListItem{
                                                                                                Text        = i.Name,
                                                                                                Value       = i.ID.ToString(),
                                                                                                Selected    = i.ID==invd.WarehouseID,
                                                                                            }).ToList(), 
                                                            }).ToList(),
                        }).FirstOrDefaultAsync();
                    for(int i=0;i<=4;i++)
                    {
                        obj.InventoryCountingDetails.Add(CreateDefualtRow());
                    }
            return obj;
        }

       

        public async Task<List<Employee>> GetEmployee()
        {
            var list=await(from e in _context.Employees.Where(s=> !s.Delete)
            select new Employee
            {
                ID              =   e.ID,
                Name            =   e.Name,
                GenderDisplay   =   e.Gender.ToString(),
                Address         =   e.Address,
                Phone           =   e.Phone,
                Position        =   e.Position,

            }).ToListAsync();
            return list;
        }

        // get Item Master Data
        public async Task<List<ViewItemMasterData>>GetItemMaster(string barcode="")
        {
                var itemmaster= string.IsNullOrWhiteSpace(barcode)? _context.ItemMasterDatas.Where(s=> !s.Delete)
                                                                  :  _context.ItemMasterDatas.Where(s=> s.Barcode==barcode && !s.Delete);
            var list=await(from i in  itemmaster
                            join guom in _context.GroupUOMs on i.GroupUomID equals guom.ID
                             let  ws= _context.WarehouseSummary.Where(s=> s.WarehouseID==i.WarehouseID&& s.UomID==guom.ID&& s.ItemID==i.ID)
                            select new ViewItemMasterData
                            {
                                ID          =   i.ID,
                                BarCode     =   i.Barcode,
                                GuomID      =   guom.ID,
                                GuomName    =   guom.Name,
                                ItemNo      =   i.Code,
                                WarehouseID =   i.WarehouseID,
                                InStock     =   ws.Sum(x=> x.InStock),
                                ItemName    =   string.IsNullOrWhiteSpace(i.KhmerName)?i.EnglishName:i.KhmerName,
                                Batch       =   (int)i.ManItemBy==1?"Y":"N",
                                Serial      =   (int)i.ManItemBy==2?"Y":"N"
                            }
                        ).ToListAsync();
            return  list;
        }

        public  ViewItemMasterData GetStockFromWarehouse(int itemID, int wID,int uomID)
        {
            var obj=(from i in _context.ItemMasterDatas.Where(s=> s.ID== itemID)
                    join ws in _context.WarehouseSummary.Where(s=> s.WarehouseID==wID&& s.UomID==uomID) on i.ID equals ws.ItemID
                    
                    select new ViewItemMasterData
                    {
                        ID          =   i.ID,
                        InStock     =   ws.InStock,
                        CountedQty  =   ws.InStock,
                        Varaince    =   0,



                    }).FirstOrDefault()?? new ViewItemMasterData();
            return obj;
        }
    }
}