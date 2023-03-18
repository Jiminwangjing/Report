using CKBS.AppContext;
using CKBS.Models.Services.Administrator.General;
using CKBS.Models.Services.Administrator.SystemInitialization;
using CKBS.Models.ServicesClass.GoodsIssue;
using KEDI.Core.Models.Validation;
using KEDI.Core.Premise.Models.Services.KSMS;
using KEDI.Core.Premise.Models.ServicesClass.KSMS;
using KEDI.Core.Premise.Models.ServicesClass.Sale;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Repository
{
    public interface IKSServiceSetUpRepository
    {
        Task<List<SeriveItemMasterData>> GetItemsAsync();
        Task<ServiceSetUpDetailModelView> GetItemDetialsAsync(int itemId, int plId, Company com);
        Task UpdateDataAsync(ServiceSetup serviceSetup, ModelStateDictionary ModelState, ModelMessage msg);
        Task<ServiceMasterDataShow> GetServiceAsync(string setupCode);
        Task<List<ServiceSetUpHistory>> GetAllServicesAsync();
    }

    public class KSServiceSetUpRepository : IKSServiceSetUpRepository
    {
        private readonly DataContext _context;

        public KSServiceSetUpRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<List<SeriveItemMasterData>> GetItemsAsync()
        {
            var list = await (from Item in _context.ItemMasterDatas.Where(x => x.Delete == false && x.Sale == true)
                              join IUom in _context.UnitofMeasures on Item.InventoryUoMID equals IUom.ID
                              select new SeriveItemMasterData
                              {
                                  ID = Item.ID,
                                  UomID = (int)Item.InventoryUoMID,
                                  Code = Item.Code,
                                  Barcode = Item.Barcode,
                                  Uom = IUom.Name,
                                  ItemName1 = Item.KhmerName,
                                  ItemName2 = Item.EnglishName
                              }).ToListAsync();
            return list;
        }
        public async Task<ServiceSetUpDetailModelView> GetItemDetialsAsync(int itemId, int plId, Company com)
        {
            ServiceSetUpDetailModelView listItem = new();
            var uoms = from guom in _context.ItemMasterDatas.Where(i => i.ID == itemId)
                       join GDU in _context.GroupDUoMs on guom.GroupUomID equals GDU.GroupUoMID
                       join UNM in _context.UnitofMeasures on GDU.AltUOM equals UNM.ID
                       select new UOMSViewModel
                       {
                           BaseUoMID = GDU.BaseUOM,
                           Factor = GDU.Factor,
                           ID = UNM.ID,
                           Name = UNM.Name
                       };
            var list = (from Item in _context.ItemMasterDatas.Where(x => x.Delete == false && x.ID == itemId)
                        join IUom in _context.UnitofMeasures on Item.InventoryUoMID equals IUom.ID
                        join Guom in _context.GroupUOMs on Item.GroupUomID equals Guom.ID
                        select new
                        {
                            Item.ID,
                            UomID = Item.InventoryUoMID,
                            GuomID = Guom.ID,
                            Item.Code,
                            Item.Barcode,
                            Uom = IUom.Name,
                            Guom = Guom.Name,
                            Item.KhmerName,
                            Item.EnglishName,

                        }).FirstOrDefault();

            var pld = _context.PriceListDetails
                .LastOrDefault(w => w.ItemID == list.ID && w.UomID == list.UomID && w.PriceListID == plId);
            var Gdoum = _context.GroupDUoMs.Where(w => w.GroupUoMID == list.GuomID);
            var Factor = Gdoum.FirstOrDefault(w => w.AltUOM == list.UomID).Factor;
            var uomPriceLists = (from _pld in _context.PriceListDetails.Where(i => i.ItemID == itemId && i.PriceListID == pld.PriceListID)
                                 select new UomPriceList
                                 {
                                     UoMID = (int)_pld.UomID,
                                     UnitPrice = (decimal)_pld.UnitPrice
                                 }).ToList();
            if (pld != null)
            {
                var _item = new ServiceSetUpDetailModelView
                {
                    Cost = (decimal)pld.Cost,
                    CurrencyID = com.SystemCurrencyID,
                    GUomID = list.GuomID,
                    ID = 0,
                    ItemCode = list.Code,
                    ItemID = list.ID,
                    ItemName = list.KhmerName,
                    Qty = 1,
                    ServiceSetupID = 0,
                    UnitPrice = (decimal)pld.UnitPrice,
                    UomID = (int)list.UomID,
                    UomList = uoms.Select(c => new SelectListItem
                    {
                        Value = c.ID.ToString(),
                        Text = c.Name,
                        Selected = c.ID == pld.UomID
                    }).ToList(),
                    Factor = (decimal)Factor,
                    UomPriceLists = uomPriceLists,
                    UoMsList = uoms.ToList(),
                };
                listItem = _item;
            }
            else
            {
                var _item = new ServiceSetUpDetailModelView
                {
                    Cost = 0,
                    CurrencyID = com.SystemCurrencyID,
                    GUomID = list.GuomID,
                    ID = 0,
                    ItemCode = list.Code,
                    ItemID = list.ID,
                    ItemName = list.KhmerName,
                    Qty = 1,
                    ServiceSetupID = 0,
                    UnitPrice = 0,
                    UomID = (int)list.UomID,
                    UomList = uoms.Select(c => new SelectListItem
                    {
                        Value = c.ID.ToString(),
                        Text = c.Name,
                        Selected = c.ID == pld.UomID
                    }).ToList(),
                    UomPriceLists = uomPriceLists,
                    UoMsList = uoms.ToList(),
                    Factor = (decimal)Factor,
                };
                listItem = _item;

            }
            return await Task.FromResult(listItem);
        }

        public async Task UpdateDataAsync(ServiceSetup serviceSetup, ModelStateDictionary ModelState, ModelMessage msg)
        {
            var services = _context.ServiceSetups.AsNoTracking().ToList();
            var setupCode_old = _context.ServiceSetups.AsNoTracking().FirstOrDefault(i=> i.ID == serviceSetup.ID);
            if (serviceSetup.PriceListID <= 0)
            {
                ModelState.AddModelError("PriceList", "Price List is require!");
            }
            if (serviceSetup.ItemID <= 0)
            {
                ModelState.AddModelError("ItemID", "Item is require!");
            }
            if (serviceSetup.Price <= 0)
            {
                ModelState.AddModelError("Price", "Price is require!");
            }
            if (serviceSetup.ServiceSetupDetials == null || serviceSetup.ServiceSetupDetials.Count <= 0)
            {
                ModelState.AddModelError("Detials", "Please choose any items to make it as detials!");
            }
            if(serviceSetup.ID == 0)
            {
                if(services.Any(i=> i.SetupCode == serviceSetup.SetupCode))
                {
                    ModelState.AddModelError("Unuiqe", "Setup Code is already exsited!");
                }
            }
            else
            {
                if (setupCode_old.SetupCode != serviceSetup.SetupCode)
                {
                    var checkBarcode = _context.ServiceSetups.AsNoTracking().FirstOrDefault(w => w.SetupCode == serviceSetup.SetupCode);
                    if (checkBarcode != null)
                    {
                        ModelState.AddModelError("Code", "Code is already existed!");
                    }
                }
            }
            if (ModelState.IsValid)
            {
                if (serviceSetup.ID > 0)
                {
                    ModelState.AddModelError("success", "Service Setup updated successfully!");
                }
                else
                {
                    ModelState.AddModelError("success", "Service Setup created successfully!");
                }
                _context.ServiceSetups.Update(serviceSetup);
                await _context.SaveChangesAsync();
                msg.Approve();
            }
            
        }

        public async Task<ServiceMasterDataShow> GetServiceAsync(string setupCode)
        {
            var master = await (from s in _context.ServiceSetups.Where(i => i.SetupCode == setupCode)
                                join item in _context.ItemMasterDatas on s.ItemID equals item.ID
                                join uom in _context.UnitofMeasures on s.UomID equals uom.ID
                                join user in _context.UserAccounts on s.UserID equals user.ID
                                select new ServiceSetUpViewModel
                                {
                                    Active = s.Active,
                                    Barcode = item.Barcode,
                                    ID = s.ID,
                                    Code = item.Code,
                                    CreatedAt = s.CreatedAt,
                                    ItemName = item.KhmerName,
                                    SetupCode = s.SetupCode,
                                    Price = s.Price,
                                    PriceListID = s.PriceListID,
                                    Remark = s.Remark,
                                    UomName = uom.Name,
                                    UserName = user.Username,
                                    ItemID = s.ItemID,
                                    UomID = uom.ID
                                }).FirstOrDefaultAsync() ?? new ServiceSetUpViewModel();

            var detial = await (from sd in _context.ServiceSetupDetials.Where(i => i.ServiceSetupID == master.ID)
                                join item in _context.ItemMasterDatas on sd.ItemID equals item.ID
                                select new ServiceSetUpDetailModelView
                                {
                                    ID = sd.ID,
                                    ServiceSetupID = sd.ServiceSetupID,
                                    Cost = sd.Cost,
                                    Factor = sd.Factor,
                                    GUomID = sd.GUomID,
                                    ItemCode = item.Code,
                                    ItemID = sd.ItemID,
                                    ItemName = item.KhmerName,
                                    Qty = sd.Qty,
                                    CurrencyID = sd.CurrencyID,
                                    UnitPrice = sd.UnitPrice,
                                    UomID = sd.UomID,
                                    UomList = (from GDU in _context.GroupDUoMs.Where(i => i.GroupUoMID == item.GroupUomID)
                                               join UNM in _context.UnitofMeasures on GDU.AltUOM equals UNM.ID
                                               select new UOMSViewModel
                                               {
                                                   BaseUoMID = GDU.BaseUOM,
                                                   Factor = GDU.Factor,
                                                   ID = UNM.ID,
                                                   Name = UNM.Name
                                               }).Select(c => new SelectListItem
                                               {
                                                   Value = c.ID.ToString(),
                                                   Text = c.Name,
                                                   Selected = c.ID == sd.UomID
                                               }).ToList(),
                                    UomPriceLists = (from _pld in _context.PriceListDetails.Where(i => i.ItemID == sd.ItemID && i.PriceListID == master.PriceListID)
                                                      select new UomPriceList
                                                      {
                                                          UoMID = (int)_pld.UomID,
                                                          UnitPrice = (decimal)_pld.UnitPrice
                                                      }).ToList(),
                                    UoMsList = (from GDU in _context.GroupDUoMs.Where(i => i.GroupUoMID == item.GroupUomID)
                                                join UNM in _context.UnitofMeasures on GDU.AltUOM equals UNM.ID
                                                select new UOMSViewModel
                                                {
                                                    BaseUoMID = GDU.BaseUOM,
                                                    Factor = GDU.Factor,
                                                    ID = UNM.ID,
                                                    Name = UNM.Name
                                                }).ToList(),
                                }).ToListAsync();
            return new ServiceMasterDataShow { 
                ServiceSetUpDetailModels = detial,
                ServiceSetUpView = master
            };
        }
        public async Task<List<ServiceSetUpHistory>> GetAllServicesAsync()
        {
            var master = await (from s in _context.ServiceSetups
                                join item in _context.ItemMasterDatas on s.ItemID equals item.ID
                                join uom in _context.UnitofMeasures on s.UomID equals uom.ID
                                join user in _context.UserAccounts on s.UserID equals user.ID
                                join pl in _context.PriceLists on s.PriceListID equals pl.ID
                                join cur in _context.Currency on pl.CurrencyID equals cur.ID
                                select new ServiceSetUpHistory
                                {
                                    ID = s.ID,
                                    Active = s.Active,
                                    CreationDate = s.CreatedAt.ToShortDateString(),
                                    Currency = cur.Description,
                                    ItemCode = item.Code,
                                    ItemName = item.KhmerName,
                                    SetupCode = s.SetupCode,
                                    Price = $"{cur.Description} {string.Format("{0:#,0.000}", s.Price)}",
                                    PriceList = pl.Name,
                                    Uom = uom.Name,
                                    UserName = user.Username
                                }).ToListAsync();
            return master;
        }
    }
}
