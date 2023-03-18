using CKBS.AppContext;
using CKBS.Models.Services.Account;
using CKBS.Models.Services.Administrator.Inventory;
using CKBS.Models.Services.Administrator.SystemInitialization;
using CKBS.Models.Services.Banking;
using CKBS.Models.Services.ChartOfAccounts;
using CKBS.Models.Services.Financials;
using CKBS.Models.Services.HumanResources;
using CKBS.Models.Services.Inventory;
using CKBS.Models.Services.Inventory.PriceList;
using CKBS.Models.Services.POS.service;
using CKBS.Models.Services.Production;
using CKBS.Models.ServicesClass;
using KEDI.Core.Helpers.Enumerations;
using KEDI.Core.Premise.Models.GeneralSettingAdmin;
using KEDI.Core.Premise.Models.Services.Administrator.CanRingSetup;
using KEDI.Core.Premise.Models.Services.Administrator.Inventory;
using KEDI.Core.Premise.Models.Services.CanRingExchangeAdmin;
using KEDI.Core.Premise.Models.ServicesClass.SerialBatches.Purchases;
using KEDI.Core.Premise.Models.ServicesClass.SerialBatches.Sales;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Type = CKBS.Models.Services.Financials.Type;
using SetGlAccount = CKBS.Models.Services.Inventory.SetGlAccount;
using CKBS.Models.Services.Administrator.General;
using KEDI.Core.Premise.Models.Services.POS.CanRing;

namespace KEDI.Core.Premise.Repository
{
    public interface ICanRingRepository
    {
        Task<List<CanRing>> GetCanRingsSetupDefaultAsync();
        Task<List<CanRing>> GetCanRingsSetupAsync(int plid, ActiveCanRingType active);
        Task<List<CanRing>> GetCanRingSetupAsync(int id, string name);
        Task<CanRing> GetCanRingSetupDefaultAsync();
        Task<bool> CreateUpdateAsync(List<CanRing> data, ModelStateDictionary modelState);
        Task<List<ServiceItemSales>> GetItemMasterDataAsync(int plid);
        Display Display { get; }
        Task<dynamic> Currency(int plid);
        List<Warehouse> Warehouses { get; }
        List<PriceLists> PriceLists { get; }
        List<PaymentMeans> PaymentMeans { get; }
        List<BusinessPartner> BusinessPartners { get; }
        List<BusinessPartner> Customers { get; }
        List<Branch> Branches { get; }
        List<UserAccount> Users { get; }
        List<ItemsReturn> CheckStockExchangeCanring(ExchangeCanRingMaster crm, Warehouse wh);
        Task<ExchangeCanRingMaster> GetExchangeCanRingMasterAsync(int seriesId, string invoiceNumber);
        Task<List<ExchangeCanRingMaster>> GetExchangeCanRingHistoryAsync(HistoryExchangeCanRingParamFilter param);
        Task<ExchangeCanRingMaster> PrintExchangeCanRingHistoryAsync(int id);
        Task<List<CanRingReport>> GetCanRingReportAsync(string dateFrom, string dateTo, int customerId, int paymentMeansId, int userId);
        void CheckCanRingItemSerailOut<T, TD>(T sale, List<TD> detials, List<SerialNumber> serialNumbers, string saleID);
        void CheckCanRingItemBatchOut<T, TD>(T sale, List<TD> detials, List<BatchNo> batchNoes, string saleID);
        void CheckCanRingItemSerailIn<T, TD>(T pur, List<TD> purd, List<SerialViewModelPurchase> serialViewModelPurchases);
        void CheckCanRingItemBatchIn<T, TD>(T pur, List<TD> purd, List<BatchViewModelPurchase> batchViewModelPurchases);
        void IssuseStockExchangeCanRing(ExchangeCanRingMaster crm, List<SerialNumber> serials, List<BatchNo> batches, List<SerialViewModelPurchase> serialViewModelPurchases, List<BatchViewModelPurchase> batchViewModelPurchases);
    }

    public class CanRingRepository : ICanRingRepository
    {
        private readonly UserManager _userModule;
        private readonly IDataPropertyRepository _dataProp;
        private readonly DataContext _context;
        private readonly UtilityModule _utility;

        public CanRingRepository(UserManager userModule, DataContext context, IDataPropertyRepository dataProp, UtilityModule utility)
        {
            _context = context;
            _userModule = userModule;
            _dataProp = dataProp;
            _utility = utility;
        }
        private UserAccount CurrentUser => _userModule.CurrentUser;
        private ExchangeRate Rate => _context.ExchangeRates.Include(i => i.Currency).FirstOrDefault(i => i.CurrencyID == CurrentUser.Company.SystemCurrencyID);
        public Display Display => _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == CurrentUser.Company.SystemCurrencyID) ?? new Display();
        public Display DisplayLocal => _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == CurrentUser.Company.LocalCurrencyID) ?? new Display();
        public async Task<dynamic> Currency(int plid)
        {
            var pl = PriceLists.FirstOrDefault(i => i.ID == plid) ?? new PriceLists();
            var display = await _context.Displays.FirstOrDefaultAsync(i => pl.CurrencyID == i.DisplayCurrencyID) ?? new Display();
            var cur = await _context.Currency.FirstOrDefaultAsync(i => i.ID == pl.CurrencyID) ?? new Currency();
            return new { Display = display, Currency = cur };
        }

        public List<Warehouse> Warehouses => _context.Warehouses.Where(i => !i.Delete).ToList();

        public List<PriceLists> PriceLists => _context.PriceLists.Where(i => !i.Delete).ToList();

        public List<PaymentMeans> PaymentMeans => _context.PaymentMeans.Where(i => !i.Delete).ToList();
        public List<BusinessPartner> BusinessPartners => _context.BusinessPartners.Where(i => !i.Delete && i.Type == "Vendor").ToList();
        public List<BusinessPartner> Customers => _context.BusinessPartners.Where(i => !i.Delete && i.Type == "Customer").ToList();

        public List<Branch> Branches => _context.Branches.Where(i => !i.Delete).ToList();

        public List<UserAccount> Users => _context.UserAccounts.Where(i => !i.Delete).Include(i => i.Employee).Select(i => new UserAccount
        {
            BranchID = i.BranchID,
            ID = i.ID,
            Username = i.Employee.Name,
        }).ToList();

        private static List<SelectListItem> GetSelectLists<T>(List<T> data, string propIdSelected, string textProp, int id = 0)
        {
            if (data.Count > 0)
            {
                int _id = id > 0 ? id : (int)GetValue(data.FirstOrDefault(), propIdSelected);
                var lists = data.Select(i => new SelectListItem
                {
                    Selected = (int)GetValue(i, propIdSelected) == _id,
                    Value = GetValue(i, propIdSelected).ToString(),
                    Text = GetValue(i, textProp).ToString(),
                }).ToList();
                return lists;
            }
            return new List<SelectListItem>();
        }
        public async Task<List<CanRing>> GetCanRingsSetupDefaultAsync()
        {
            List<CanRing> canRings = new();
            for (var i = 0; i <= 10; i++)
            {
                canRings.Add(NewCanRing());
            }
            return await Task.FromResult(canRings);
        }
        public async Task<CanRing> GetCanRingSetupDefaultAsync()
        {
            CanRing canring = NewCanRing();
            return await Task.FromResult(canring);
        }
        public async Task<ExchangeCanRingMaster> GetExchangeCanRingMasterAsync(int seriesId, string invoiceNumber)
        {
            var data = await (from ec in _context.ExchangeCanRingMasters.Where(i => i.SeriesID == seriesId && i.Number == invoiceNumber)
                              join pl in _context.PriceLists on ec.PriceListID equals pl.ID
                              join cur in _context.Currency on pl.CurrencyID equals cur.ID
                              join vendor in _context.BusinessPartners on ec.CusId equals vendor.ID
                              select new ExchangeCanRingMaster
                              {
                                  BranchID = ec.BranchID,
                                  CompanyID = ec.CompanyID,
                                  CreatedAt = ec.CreatedAt,
                                  CusId = ec.CusId,
                                  DocTypeID = ec.DocTypeID,
                                  ExchangeRate = ec.ExchangeRate,
                                  ID = ec.ID,
                                  LocalCurrencyID = ec.LocalCurrencyID,
                                  LocalSetRate = ec.LocalSetRate,
                                  Number = ec.Number,
                                  PaymentMeanID = ec.PaymentMeanID,
                                  PriceListID = ec.PriceListID,
                                  SeriesDID = ec.SeriesDID,
                                  SeriesID = ec.SeriesID,
                                  SysCurrencyID = ec.SysCurrencyID,
                                  Total = ec.Total,
                                  TotalDis = _utility.ToCurrency(ec.Total, Display.Amounts),
                                  TotalSystem = ec.TotalSystem,
                                  UserID = ec.UserID,
                                  WarehouseID = ec.WarehouseID,
                                  VendorName = vendor.Name,
                                  CurrencyName = cur.Description,
                              }).FirstOrDefaultAsync();
            if (data != null)
            {
                var exchangeCanRingDetails = (from ecd in _context.ExchangeCanRingDetails.Where(i => i.ExchangeCanRingMasterID == data.ID)
                                              join item in _context.ItemMasterDatas on ecd.ItemID equals item.ID
                                              join itemChange in _context.ItemMasterDatas on ecd.ItemChangeID equals itemChange.ID
                                              join uom in _context.UnitofMeasures on ecd.UomID equals uom.ID
                                              join uomChange in _context.UnitofMeasures on ecd.UomChangeID equals uomChange.ID
                                              select new ExchangeCanRingDetail
                                              {
                                                  ID = ecd.ID,
                                                  ChangeQty = ecd.ChangeQty,
                                                  ChargePrice = ecd.ChargePrice,
                                                  ItemChangeName = itemChange.KhmerName,
                                                  ItemName = item.KhmerName,
                                                  LineID = $"{DateTime.Now.Ticks}{ecd.ID}",
                                                  Name = ecd.Name,
                                                  Qty = ecd.Qty,
                                                  UomName = uom.Name,
                                                  UomChangeName = uomChange.Name,
                                                  UserID = ecd.UserID,
                                                  CreatedAt = ecd.CreatedAt,
                                                  CreatedAtDis = ecd.CreatedAtDis,
                                                  ItemChangeID = ecd.ItemChangeID,
                                                  ItemID = ecd.ItemID,
                                                  PriceListID = ecd.PriceListID,
                                                  UomChangeID = ecd.UomChangeID,
                                                  UomID = ecd.UomID,
                                                  UomLists = (from GDU in _context.GroupDUoMs.Where(i => i.GroupUoMID == item.GroupUomID)
                                                              join UNM in _context.UnitofMeasures on GDU.AltUOM equals UNM.ID
                                                              select new SelectListItem
                                                              {
                                                                  Selected = UNM.ID == ecd.UomID,
                                                                  Text = UNM.Name,
                                                                  Value = UNM.ID.ToString()
                                                              }).ToList(),
                                                  UomChangeLists = (from GDU in _context.GroupDUoMs.Where(i => i.GroupUoMID == itemChange.GroupUomID)
                                                                    join UNM in _context.UnitofMeasures on GDU.AltUOM equals UNM.ID
                                                                    select new SelectListItem
                                                                    {
                                                                        Selected = UNM.ID == ecd.UomChangeID,
                                                                        Text = UNM.Name,
                                                                        Value = UNM.ID.ToString()
                                                                    }).ToList(),
                                                  IsActive = ecd.IsActive,
                                                  CanRingID = ecd.CanRingID,
                                                  ExchangeCanRingMasterID = ecd.ExchangeCanRingMasterID
                                              }).ToList();
                data.ExchangeCanRingDetails = exchangeCanRingDetails;
            }
            return data;
        }
        CanRing NewCanRing()
        {
            var priceLists = GetSelectLists(_context.PriceLists.Where(i => !i.Delete).ToList(), "ID", "Name");
            CanRing canring = new()
            {
                ChargePrice = 0,
                ChangeQty = 1,
                ItemChangeID = 0,
                ItemChangeName = "",
                ID = 0,
                ItemID = 0,
                ItemName = "",
                Name = "",
                PriceListID = Convert.ToInt32(priceLists.FirstOrDefault().Value),
                PriceLists = priceLists,
                Qty = 22,
                Total = "",
                UomChangeID = 0,
                UomChangeLists = new List<SelectListItem>(),
                UomID = 0,
                UomLists = new List<SelectListItem>(),
                UserID = CurrentUser.ID,
                ExchangRate = (decimal)Rate.Rate,
                Currency = Rate.Currency?.Description,
                ExchangeRates = _context.ExchangeRates.ToList(),
                ListPriceLists = _context.PriceLists.ToList(),
            };
            return canring;
        }
        public async Task<bool> CreateUpdateAsync(List<CanRing> data, ModelStateDictionary modelState)
        {
            data = data.Where(i => !string.IsNullOrEmpty(i.Name)).ToList();
            Validation(data, modelState);
            if (modelState.IsValid)
            {
                _context.CanRings.UpdateRange(data);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
        public async Task<List<ServiceItemSales>> GetItemMasterDataAsync(int plid)
        {
            var data = (from pld in _context.PriceListDetails.Where(i => i.PriceListID == plid)
                        join item in _context.ItemMasterDatas on pld.ItemID equals item.ID
                        join uom in _context.UnitofMeasures on pld.UomID equals uom.ID
                        join cur in _context.Currency on pld.CurrencyID equals cur.ID
                        where uom.ID == item.SaleUomID && item.Sale && !item.Delete
                        select new ServiceItemSales
                        {
                            ID = pld.ID,
                            Barcode = item.Barcode,
                            Code = item.Code,
                            Cost = pld.Cost,
                            Currency = cur.Description,
                            CurrencyID = cur.ID,
                            EnglishName = item.EnglishName,
                            Image = item.Image,
                            ItemID = item.ID,
                            ItemType = item.Type,
                            KhmerName = item.KhmerName,
                            PricListID = pld.PriceListID,
                            UnitPrice = pld.UnitPrice,
                            UoM = uom.Name,
                            UomID = uom.ID,
                            Process = item.Process,
                            UomLists = (from GDU in _context.GroupDUoMs.Where(i => i.GroupUoMID == item.GroupUomID)
                                        join UNM in _context.UnitofMeasures on GDU.AltUOM equals UNM.ID
                                        select new SelectListItem
                                        {
                                            Selected = UNM.ID == uom.ID,
                                            Text = UNM.Name,
                                            Value = UNM.ID.ToString()
                                        }).ToList(),
                            UomChangeLists = (from GDU in _context.GroupDUoMs.Where(i => i.GroupUoMID == item.GroupUomID)
                                              join UNM in _context.UnitofMeasures on GDU.AltUOM equals UNM.ID
                                              select new SelectListItem
                                              {
                                                  Selected = UNM.ID == uom.ID,
                                                  Text = UNM.Name,
                                                  Value = UNM.ID.ToString()
                                              }).ToList(),
                        }).GroupBy(i => i.ItemID).Select(i => i.FirstOrDefault()).ToList();
            _dataProp.DataProperty(data, CurrentUser.ID, "ItemID", "AddictionProps");
            return await Task.FromResult(data);
        }
        public async Task<List<CanRing>> GetCanRingsSetupAsync(int plid, ActiveCanRingType active)
        {
            List<CanRing> canrings = _context.CanRings.ToList();
            if (plid > 0) canrings = canrings.Where(i => i.PriceListID == plid).ToList();
            if (active == ActiveCanRingType.Active) canrings = canrings.Where(i => i.IsActive).ToList();
            if (active == ActiveCanRingType.InActive) canrings = canrings.Where(i => !i.IsActive).ToList();
            var data = (from cr in canrings
                        join item in _context.ItemMasterDatas on cr.ItemID equals item.ID
                        join itemChange in _context.ItemMasterDatas on cr.ItemChangeID equals itemChange.ID
                        join pl in _context.PriceLists on cr.PriceListID equals pl.ID
                        join cur in _context.Currency on pl.CurrencyID equals cur.ID
                        join uom in _context.UnitofMeasures on cr.UomID equals uom.ID
                        join uomChange in _context.UnitofMeasures on cr.UomChangeID equals uomChange.ID
                        select new CanRing
                        {
                            ID = cr.ID,
                            CanRingID = cr.ID,
                            ChangeQty = cr.ChangeQty,
                            ChargePrice = cr.ChargePrice,
                            Currency = cur.Description,
                            ItemChangeName = itemChange.KhmerName,
                            ItemName = item.KhmerName,
                            LineID = $"{DateTime.Now.Ticks}{cr.ID}",
                            Name = cr.Name,
                            Qty = cr.Qty,
                            UomName = uom.Name,
                            UomChangeName = uomChange.Name,
                            PriceList = pl.Name,
                            IsActive = cr.IsActive,
                        }).ToList();
            return await Task.FromResult(data);
        }
        public async Task<List<CanRing>> GetCanRingSetupAsync(int id, string name)
        {
            var canrings = _context.CanRings.Where(i => i.ID == id && i.IsActive);
            if (!string.IsNullOrEmpty(name)) canrings = _context.CanRings.Where(i => i.Name == name);
            var data = (from cr in canrings
                        join item in _context.ItemMasterDatas on cr.ItemID equals item.ID
                        join itemChange in _context.ItemMasterDatas on cr.ItemChangeID equals itemChange.ID
                        join pl in _context.PriceLists on cr.PriceListID equals pl.ID
                        join cur in _context.Currency on pl.CurrencyID equals cur.ID
                        join uom in _context.UnitofMeasures on cr.UomID equals uom.ID
                        join uomChange in _context.UnitofMeasures on cr.UomChangeID equals uomChange.ID
                        join rate in _context.ExchangeRates on cur.ID equals rate.CurrencyID
                        select new CanRing
                        {
                            ID = cr.ID,
                            CanRingID = cr.ID,
                            ChangeQty = cr.ChangeQty,
                            ChargePrice = cr.ChargePrice,
                            Currency = cur.Description,
                            ItemChangeName = itemChange.KhmerName,
                            ItemName = item.KhmerName,
                            LineID = $"{DateTime.Now.Ticks}{cr.ID}",
                            Name = cr.Name,
                            Qty = cr.Qty,
                            UomName = uom.Name,
                            UomChangeName = uomChange.Name,
                            UserID = cr.UserID,
                            CreatedAt = cr.CreatedAt,
                            CreatedAtDis = cr.CreatedAtDis,
                            ExchangRate = (decimal)rate.Rate,
                            ItemChangeID = cr.ItemChangeID,
                            ItemID = cr.ItemID,
                            PriceListID = cr.PriceListID,
                            Total = $"{cur.Description} {_utility.ToCurrency(cr.ChargePrice, Display.Amounts)}",
                            UomChangeID = cr.UomChangeID,
                            UomID = cr.UomID,
                            UomLists = (from GDU in _context.GroupDUoMs.Where(i => i.GroupUoMID == item.GroupUomID)
                                        join UNM in _context.UnitofMeasures on GDU.AltUOM equals UNM.ID
                                        select new SelectListItem
                                        {
                                            Selected = UNM.ID == cr.UomID,
                                            Text = UNM.Name,
                                            Value = UNM.ID.ToString()
                                        }).ToList(),
                            UomChangeLists = (from GDU in _context.GroupDUoMs.Where(i => i.GroupUoMID == itemChange.GroupUomID)
                                              join UNM in _context.UnitofMeasures on GDU.AltUOM equals UNM.ID
                                              select new SelectListItem
                                              {
                                                  Selected = UNM.ID == cr.UomChangeID,
                                                  Text = UNM.Name,
                                                  Value = UNM.ID.ToString()
                                              }).ToList(),
                            PriceLists = GetSelectLists(_context.PriceLists.Where(i => !i.Delete).ToList(), "ID", "Name", cr.PriceListID),
                            PriceList = pl.Name,
                            IsActive = cr.IsActive,
                        }).ToList();
            return await Task.FromResult(data);
        }
        public List<ItemsReturn> CheckStockExchangeCanring(ExchangeCanRingMaster crm, Warehouse wh)
        {
            List<ItemsReturn> list = new();
            List<ItemsReturn> list_group = new();
            var sbReturn = (from od in crm.ExchangeCanRingDetails
                            join item in _context.ItemMasterDatas on od.ItemID equals item.ID
                            join ws in _context.WarehouseSummary on item.ID equals ws.ItemID
                            join uom in _context.UnitofMeasures on item.InventoryUoMID equals uom.ID
                            group new { od, item, ws, uom } by new { item.ID } into g
                            let data = g.FirstOrDefault()
                            let uom_defined = _context.GroupDUoMs.FirstOrDefault(w => w.GroupUoMID == data.item.GroupUomID && w.AltUOM == data.od.UomID) ?? new GroupDUoM()
                            let instock = (decimal)g.Where(i => i.ws.WarehouseID == crm.WarehouseID).Sum(i => i.ws.InStock)
                            select new ItemsReturn
                            {
                                Code = data.item.Code,
                                Committed = (decimal)data.ws.Committed,
                                ItemID = data.item.ID,
                                InStock = instock,
                                TotalStock = instock - data.od.Qty * (decimal)uom_defined.Factor,
                                KhmerName = $"{data.item.KhmerName} ({data.uom.Name})",
                                LineID = data.od.LineID,
                                OrderQty = data.od.Qty * (decimal)uom_defined.Factor,//(decimal)data.od.Qty,
                                IsSerailBatch = data.item.ManItemBy == ManageItemBy.Batches || data.item.ManItemBy == ManageItemBy.SerialNumbers
                            }).ToList();
            foreach (var item in crm.ExchangeCanRingDetails)
            {
                var itemChange = _context.ItemMasterDatas.Find(item.ItemID) ?? new ItemMasterData();
                var check = list_group.Find(w => w.ItemID == item.ItemID);
                var item_group_uom = _context.ItemMasterDatas.Include(gu => gu.GroupUOM).Include(uom => uom.UnitofMeasureInv).FirstOrDefault(w => w.ID == item.ItemID);
                var uom_defined = _context.GroupDUoMs.FirstOrDefault(w => w.GroupUoMID == item_group_uom.GroupUomID && w.AltUOM == item.UomID);
                var bom = _context.BOMaterial.FirstOrDefault(w => w.ItemID == item.ItemID && w.Active == true);
                var item_material = (from bomd in _context.BOMDetail.Where(w => w.BID == bom.BID && w.NegativeStock == false && w.Detele == false)
                                     join i in _context.ItemMasterDatas.Where(im => !im.Delete) on bomd.ItemID equals i.ID
                                     join gd in _context.GroupDUoMs on bomd.UomID equals gd.AltUOM
                                     join uom in _context.UnitofMeasures on i.InventoryUoMID equals uom.ID
                                     select new
                                     {
                                         bomd.ItemID,
                                         i.Code,
                                         i.KhmerName,
                                         Uom = uom.Name,
                                         gd.Factor,
                                         gd.GroupUoMID,
                                         GUoMID = i.GroupUomID,
                                         bomd.Qty,
                                         i.Process,
                                     }).Where(w => w.GroupUoMID == w.GUoMID);
                if (check == null)
                {
                    if (bom != null)
                    {
                        foreach (var items in item_material.ToList())
                        {
                            var item_warehouse_material = _context.WarehouseSummary.FirstOrDefault(w => w.WarehouseID == crm.WarehouseID && w.ItemID == items.ItemID);
                            if (item_warehouse_material != null)
                            {
                                ItemsReturn item_group = new()
                                {
                                    LineID = item.LineID,
                                    Code = items.Code,
                                    ItemID = items.ItemID,
                                    KhmerName = items.KhmerName + " (" + items.Uom + ")",
                                    InStock = (decimal)item_warehouse_material.InStock,
                                    TotalStock = (decimal)item_warehouse_material.InStock - item.Qty * (decimal)(uom_defined.Factor * items.Qty * items.Factor),
                                    OrderQty = item.Qty * (decimal)(uom_defined.Factor * items.Qty * items.Factor),
                                    Committed = (decimal)item_warehouse_material.Committed,
                                    IsBOM = true,
                                };
                                list_group.Add(item_group);
                            }
                            else if (items.Process != "Standard" && item_warehouse_material == null)
                            {
                                ItemsReturn item_group = new()
                                {
                                    LineID = item.LineID,
                                    Code = itemChange.Code,
                                    ItemID = item.ItemID,
                                    KhmerName = item_group_uom.KhmerName + " (" + item_group_uom.UnitofMeasureInv.Name + ")",
                                    InStock = 0,
                                    OrderQty = item.Qty * (decimal)uom_defined.Factor,
                                    Committed = 0,
                                    IsBOM = true,
                                };
                                list_group.Add(item_group);
                            }
                        }
                    }
                }
                else
                {
                    check.OrderQty += item.Qty * (decimal)uom_defined.Factor;
                }
            }

            foreach (var item in list_group)
            {
                if (wh.IsAllowNegativeStock)
                {
                    if (item.IsSerailBatch)
                    {
                        ItemsReturn item_return = new()
                        {
                            LineID = item.LineID,
                            Code = item.Code,
                            ItemID = item.ItemID,
                            KhmerName = item.KhmerName,
                            InStock = item.InStock,
                            OrderQty = item.OrderQty,
                            Committed = item.Committed,
                            IsBOM = item.IsBOM,
                        };
                        list.Add(item_return);
                    }
                }
                else
                {
                    if (item.OrderQty > item.InStock)
                    {
                        ItemsReturn item_return = new()
                        {
                            LineID = item.LineID,
                            Code = item.Code,
                            ItemID = item.ItemID,
                            KhmerName = item.KhmerName,
                            InStock = item.InStock,
                            OrderQty = item.OrderQty,
                            Committed = item.Committed,
                            IsBOM = item.IsBOM,
                        };
                        list.Add(item_return);
                    }
                }
            }
            List<ItemsReturn> itemsReturn = new(list.Count + sbReturn.Count);
            itemsReturn.AddRange(list);
            itemsReturn.AddRange(sbReturn);
            return itemsReturn;
        }
        public async Task<List<ExchangeCanRingMaster>> GetExchangeCanRingHistoryAsync(HistoryExchangeCanRingParamFilter param)
        {
            List<ExchangeCanRingMaster> canRingMasters = new();
            DateTime dateFrom = Convert.ToDateTime(param.DateFrom);
            DateTime dateTo = Convert.ToDateTime(param.DateTo);
            #region a big filtering
            if (param.DateFrom != null && param.DateTo != null && param.BranchID == 0 && param.PaymentMeansID == 0 && param.PriceListID == 0 && param.VendorID == 0)
            {
                canRingMasters = _context.ExchangeCanRingMasters.Where(i => i.CreatedAt >= dateFrom && i.CreatedAt <= dateTo).ToList();
            }
            else if (param.DateFrom != null && param.DateTo != null && param.BranchID != 0 && param.UserID == 0 && param.WarehouseID == 0 && param.PaymentMeansID == 0 && param.PriceListID == 0 && param.VendorID == 0)
            {
                canRingMasters = _context.ExchangeCanRingMasters
                    .Where(i => i.CreatedAt >= dateFrom && i.CreatedAt <= dateTo && i.BranchID == param.BranchID).ToList();
            }
            else if (param.DateFrom != null && param.DateTo != null && param.BranchID != 0 && param.UserID != 0 && param.WarehouseID == 0 && param.PaymentMeansID == 0 && param.PriceListID == 0 && param.VendorID == 0)
            {
                canRingMasters = _context.ExchangeCanRingMasters
                    .Where(i => i.CreatedAt >= dateFrom && i.CreatedAt <= dateTo && i.BranchID == param.BranchID && i.UserID == param.UserID).ToList();
            }
            else if (param.DateFrom != null && param.DateTo != null && param.BranchID != 0 && param.UserID == 0 && param.WarehouseID != 0 && param.PaymentMeansID == 0 && param.PriceListID == 0 && param.VendorID == 0)
            {
                canRingMasters = _context.ExchangeCanRingMasters
                    .Where(i => i.CreatedAt >= dateFrom && i.CreatedAt <= dateTo && i.BranchID == param.BranchID && i.WarehouseID == param.WarehouseID).ToList();
            }
            else if (param.DateFrom != null && param.DateTo != null && param.BranchID != 0 && param.UserID != 0 && param.WarehouseID != 0 && param.PaymentMeansID == 0 && param.PriceListID == 0 && param.VendorID == 0)
            {
                canRingMasters = _context.ExchangeCanRingMasters
                    .Where(i => i.CreatedAt >= dateFrom && i.CreatedAt <= dateTo && i.BranchID == param.BranchID && i.WarehouseID == param.WarehouseID && i.UserID == param.UserID).ToList();
            }
            else if (param.DateFrom != null && param.DateTo != null && param.BranchID != 0 && param.UserID != 0 && param.WarehouseID != 0 && param.PaymentMeansID != 0 && param.PriceListID == 0 && param.VendorID == 0)
            {
                canRingMasters = _context.ExchangeCanRingMasters
                    .Where(i => i.CreatedAt >= dateFrom && i.CreatedAt <= dateTo && i.BranchID == param.BranchID && i.WarehouseID == param.WarehouseID && i.UserID == param.UserID && i.PaymentMeanID == param.PaymentMeansID).ToList();
            }
            else if (param.DateFrom != null && param.DateTo != null && param.BranchID == 0 && param.PaymentMeansID != 0 && param.PriceListID == 0 && param.VendorID == 0)
            {
                canRingMasters = _context.ExchangeCanRingMasters
                    .Where(i => i.CreatedAt >= dateFrom && i.CreatedAt <= dateTo && i.PaymentMeanID == param.PaymentMeansID).ToList();
            }
            else if (param.DateFrom != null && param.DateTo != null && param.BranchID != 0 && param.UserID == 0 && param.WarehouseID != 0 && param.PaymentMeansID != 0 && param.PriceListID == 0 && param.VendorID == 0)
            {
                canRingMasters = _context.ExchangeCanRingMasters
                    .Where(i => i.CreatedAt >= dateFrom && i.CreatedAt <= dateTo && i.WarehouseID == param.WarehouseID && i.PaymentMeanID == param.PaymentMeansID).ToList();
            }
            else if (param.DateFrom != null && param.DateTo != null && param.BranchID != 0 && param.UserID != 0 && param.WarehouseID == 0 && param.PaymentMeansID != 0 && param.PriceListID == 0 && param.VendorID == 0)
            {
                canRingMasters = _context.ExchangeCanRingMasters
                    .Where(i => i.CreatedAt >= dateFrom && i.CreatedAt <= dateTo && i.UserID == param.UserID && i.PaymentMeanID == param.PaymentMeansID).ToList();
            }
            else if (param.DateFrom != null && param.DateTo != null && param.BranchID != 0 && param.UserID == 0 && param.WarehouseID == 0 && param.PaymentMeansID != 0 && param.PriceListID == 0 && param.VendorID == 0)
            {
                canRingMasters = _context.ExchangeCanRingMasters
                    .Where(i => i.CreatedAt >= dateFrom && i.CreatedAt <= dateTo && i.PaymentMeanID == param.PaymentMeansID).ToList();
            }
            else if (param.DateFrom != null && param.DateTo != null && param.BranchID != 0 && param.UserID != 0 && param.WarehouseID != 0 && param.PaymentMeansID != 0 && param.PriceListID != 0 && param.VendorID == 0)
            {
                canRingMasters = _context.ExchangeCanRingMasters
                    .Where(i => i.CreatedAt >= dateFrom && i.CreatedAt <= dateTo && i.BranchID == param.BranchID && i.WarehouseID == param.WarehouseID && i.UserID == param.UserID && i.PaymentMeanID == param.PaymentMeansID && i.PriceListID == param.PriceListID).ToList();
            }
            else if (param.DateFrom != null && param.DateTo != null && param.BranchID != 0 && param.UserID == 0 && param.WarehouseID != 0 && param.PaymentMeansID != 0 && param.PriceListID != 0 && param.VendorID == 0)
            {
                canRingMasters = _context.ExchangeCanRingMasters
                    .Where(i => i.CreatedAt >= dateFrom && i.CreatedAt <= dateTo && i.BranchID == param.BranchID && i.WarehouseID == param.WarehouseID && i.PaymentMeanID == param.PaymentMeansID && i.PriceListID == param.PriceListID).ToList();
            }
            else if (param.DateFrom != null && param.DateTo != null && param.BranchID != 0 && param.UserID != 0 && param.WarehouseID == 0 && param.PaymentMeansID != 0 && param.PriceListID != 0 && param.VendorID == 0)
            {
                canRingMasters = _context.ExchangeCanRingMasters
                    .Where(i => i.CreatedAt >= dateFrom && i.CreatedAt <= dateTo && i.BranchID == param.BranchID && i.UserID == param.UserID && i.PaymentMeanID == param.PaymentMeansID && i.PriceListID == param.PriceListID).ToList();
            }
            else if (param.DateFrom != null && param.DateTo != null && param.BranchID != 0 && param.UserID == 0 && param.WarehouseID != 0 && param.PaymentMeansID == 0 && param.PriceListID != 0 && param.VendorID == 0)
            {
                canRingMasters = _context.ExchangeCanRingMasters
                    .Where(i => i.CreatedAt >= dateFrom && i.CreatedAt <= dateTo && i.BranchID == param.BranchID && i.WarehouseID == param.WarehouseID && i.PriceListID == param.PriceListID).ToList();
            }
            else if (param.DateFrom != null && param.DateTo != null && param.BranchID != 0 && param.UserID != 0 && param.WarehouseID == 0 && param.PaymentMeansID == 0 && param.PriceListID != 0 && param.VendorID == 0)
            {
                canRingMasters = _context.ExchangeCanRingMasters
                    .Where(i => i.CreatedAt >= dateFrom && i.CreatedAt <= dateTo && i.BranchID == param.BranchID && i.UserID == param.UserID && i.PriceListID == param.PriceListID).ToList();
            }
            else if (param.DateFrom != null && param.DateTo != null && param.BranchID != 0 && param.UserID != 0 && param.WarehouseID != 0 && param.PaymentMeansID != 0 && param.PriceListID != 0 && param.VendorID != 0)
            {
                canRingMasters = _context.ExchangeCanRingMasters
                    .Where(i => i.CreatedAt >= dateFrom && i.CreatedAt <= dateTo && i.BranchID == param.BranchID && i.UserID == param.UserID && i.WarehouseID == param.WarehouseID && i.PriceListID == param.PriceListID && i.CusId == param.VendorID).ToList();
            }
            else if (param.DateFrom != null && param.DateTo != null && param.BranchID == 0 && param.PaymentMeansID != 0 && param.PriceListID != 0 && param.VendorID != 0)
            {
                canRingMasters = _context.ExchangeCanRingMasters
                    .Where(i => i.CreatedAt >= dateFrom && i.CreatedAt <= dateTo && i.PaymentMeanID == param.PaymentMeansID && i.PriceListID == param.PriceListID && i.CusId == param.VendorID).ToList();
            }
            else if (param.DateFrom != null && param.DateTo != null && param.BranchID == 0 && param.PaymentMeansID == 0 && param.PriceListID != 0 && param.VendorID != 0)
            {
                canRingMasters = _context.ExchangeCanRingMasters
                    .Where(i => i.CreatedAt >= dateFrom && i.CreatedAt <= dateTo && i.PriceListID == param.PriceListID && i.CusId == param.VendorID).ToList();
            }
            else if (param.DateFrom != null && param.DateTo != null && param.BranchID == 0 && param.PaymentMeansID == 0 && param.PriceListID == 0 && param.VendorID != 0)
            {
                canRingMasters = _context.ExchangeCanRingMasters
                    .Where(i => i.CreatedAt >= dateFrom && i.CreatedAt <= dateTo && i.CusId == param.VendorID).ToList();
            }
            else if (param.DateFrom != null && param.DateTo != null && param.BranchID == 0 && param.PaymentMeansID != 0 && param.PriceListID == 0 && param.VendorID != 0)
            {
                canRingMasters = _context.ExchangeCanRingMasters
                    .Where(i => i.CreatedAt >= dateFrom && i.CreatedAt <= dateTo && param.PaymentMeansID == i.PaymentMeanID && i.CusId == param.VendorID).ToList();
            }
            else if (param.DateFrom != null && param.DateTo != null && param.BranchID != 0 && param.UserID != 0 && param.WarehouseID == 0 && param.PaymentMeansID != 0 && param.PriceListID == 0 && param.VendorID != 0)
            {
                canRingMasters = _context.ExchangeCanRingMasters
                    .Where(i => i.CreatedAt >= dateFrom && i.BranchID == param.BranchID && i.UserID == param.UserID && i.CreatedAt <= dateTo && param.PaymentMeansID == i.PaymentMeanID && i.CusId == param.VendorID).ToList();
            }
            else if (param.DateFrom != null && param.DateTo != null && param.BranchID != 0 && param.UserID == 0 && param.WarehouseID != 0 && param.PaymentMeansID != 0 && param.PriceListID == 0 && param.VendorID != 0)
            {
                canRingMasters = _context.ExchangeCanRingMasters
                    .Where(i => i.CreatedAt >= dateFrom && i.BranchID == param.BranchID && i.WarehouseID == param.WarehouseID && i.CreatedAt <= dateTo && param.PaymentMeansID == i.PaymentMeanID && i.CusId == param.VendorID).ToList();
            }
            else if (param.DateFrom != null && param.DateTo != null && param.BranchID != 0 && param.UserID == 0 && param.WarehouseID != 0 && param.PaymentMeansID == 0 && param.PriceListID != 0 && param.VendorID != 0)
            {
                canRingMasters = _context.ExchangeCanRingMasters
                    .Where(i => i.CreatedAt >= dateFrom && i.BranchID == param.BranchID && i.WarehouseID == param.WarehouseID && i.CreatedAt <= dateTo && param.PriceListID == i.PriceListID && i.CusId == param.VendorID).ToList();
            }
            else if (param.DateFrom != null && param.DateTo != null && param.BranchID != 0 && param.UserID != 0 && param.WarehouseID == 0 && param.PaymentMeansID == 0 && param.PriceListID != 0 && param.VendorID != 0)
            {
                canRingMasters = _context.ExchangeCanRingMasters
                    .Where(i => i.CreatedAt >= dateFrom && i.BranchID == param.BranchID && i.UserID == param.UserID && i.CreatedAt <= dateTo && param.PriceListID == i.PriceListID && i.CusId == param.VendorID).ToList();
            }
            else if (param.DateFrom != null && param.DateTo != null && param.BranchID == 0 && param.PaymentMeansID != 0 && param.PriceListID != 0 && param.VendorID == 0)
            {
                canRingMasters = _context.ExchangeCanRingMasters
                    .Where(i => i.CreatedAt >= dateFrom && i.CreatedAt <= dateTo && param.PriceListID == i.PriceListID && i.PaymentMeanID == param.PaymentMeansID).ToList();
            }
            else if (param.DateFrom != null && param.DateTo != null && param.BranchID == 0 && param.PaymentMeansID == 0 && param.PriceListID != 0 && param.VendorID == 0)
            {
                canRingMasters = _context.ExchangeCanRingMasters
                    .Where(i => i.CreatedAt >= dateFrom && i.CreatedAt <= dateTo && param.PriceListID == i.PriceListID).ToList();
            }
            else
            {
                canRingMasters = _context.ExchangeCanRingMasters.ToList();
            }
            #endregion
            var data = (from ec in canRingMasters
                              join doc in _context.DocumentTypes on ec.DocTypeID equals doc.ID
                              join series in _context.Series on ec.SeriesID equals series.ID
                              join pl in _context.PriceLists on ec.PriceListID equals pl.ID
                              join cur in _context.Currency on pl.CurrencyID equals cur.ID
                              join vendor in _context.BusinessPartners on ec.CusId equals vendor.ID
                              join pm in _context.PaymentMeans on ec.PaymentMeanID equals pm.ID
                              join user in _context.UserAccounts.Include(i=> i.Employee) on ec.UserID equals user.ID
                              join wh in _context.Warehouses on ec.WarehouseID equals wh.ID
                              select new ExchangeCanRingMaster
                              {
                                  CreatedAt = ec.CreatedAt,
                                  DocCode = doc.Code,
                                  ID = ec.ID,
                                  CurrencyName = cur.Description,
                                  ExchangeRate = ec.ExchangeRate,
                                  Number = $"{series.Name}-{ec.Number}",
                                  TotalDis = $"{cur.Description} {_utility.ToCurrency(ec.Total, Display.Amounts)}",
                                  VendorName = vendor.Name,
                                  PaymentMeans = pm.Type,
                                  PriceList = pl.Name,
                                  UserName = user.Employee.Name,
                                  WarehouseName = wh.Name,
                              }).OrderBy(i => i.CreatedAt).ToList();
            return await Task.FromResult(data);
        }
        public async Task<ExchangeCanRingMaster> PrintExchangeCanRingHistoryAsync(int id)
        {
            var data = (from ec in _context.ExchangeCanRingMasters.Where(i=> i.ID == id)
                        join doc in _context.DocumentTypes on ec.DocTypeID equals doc.ID
                        join series in _context.Series on ec.SeriesID equals series.ID
                        join pl in _context.PriceLists on ec.PriceListID equals pl.ID
                        join cur in _context.Currency on pl.CurrencyID equals cur.ID
                        join vendor in _context.BusinessPartners on ec.CusId equals vendor.ID
                        join pm in _context.PaymentMeans on ec.PaymentMeanID equals pm.ID
                        join user in _context.UserAccounts.Include(i => i.Employee).Include(i=> i.Company) on ec.UserID equals user.ID
                        join curSys in _context.Currency on user.Company.SystemCurrencyID equals curSys.ID
                        join curLocal in _context.Currency on user.Company.LocalCurrencyID equals curLocal.ID
                        join wh in _context.Warehouses on ec.WarehouseID equals wh.ID
                        let totalLocal = (double)ec.TotalSystem * ec.LocalSetRate
                        select new ExchangeCanRingMaster
                        {
                            CreatedAt = ec.CreatedAt,
                            DocCode = doc.Code,
                            ID = ec.ID,
                            CurrencyName = cur.Description,
                            ExchangeRate = ec.ExchangeRate,
                            Number = $"{series.Name}-{ec.Number}",
                            TotalDis = $"{cur.Description} {_utility.ToCurrency(ec.Total, Display.Amounts)}",
                            TotalSystemDis = $"{curSys.Description} {_utility.ToCurrency(ec.TotalSystem, Display.Amounts)}",
                            TotalLocal = $"{curLocal.Description} {_utility.ToCurrency(totalLocal, DisplayLocal.Amounts)}",
                            VendorName = vendor.Name,
                            PaymentMeans = pm.Type,
                            PriceList = pl.Name,
                            UserName = user.Employee.Name,
                            WarehouseName = wh.Name,
                            CampanyLogo = user.Company == null ? "" : user.Company.Logo ?? "",
                        }).FirstOrDefault();
            if (data != null)
            {
                var exchangeCanRingDetails = (from ecd in _context.ExchangeCanRingDetails.Where(i => i.ExchangeCanRingMasterID == data.ID)
                                              join item in _context.ItemMasterDatas on ecd.ItemID equals item.ID
                                              join itemChange in _context.ItemMasterDatas on ecd.ItemChangeID equals itemChange.ID
                                              join uom in _context.UnitofMeasures on ecd.UomID equals uom.ID
                                              join uomChange in _context.UnitofMeasures on ecd.UomChangeID equals uomChange.ID
                                              select new ExchangeCanRingDetail
                                              {
                                                  ID = ecd.ID,
                                                  ChangeQtyDis = $"{uomChange.Name} {ecd.ChangeQty}",
                                                  Total = $"{data.CurrencyName} {_utility.ToCurrency(ecd.ChargePrice, Display.Amounts)}",
                                                  ItemChangeName = itemChange.KhmerName,
                                                  ItemName = item.KhmerName,
                                                  LineID = $"{DateTime.Now.Ticks}{ecd.ID}",
                                                  Name = ecd.Name,
                                                  QtyDis = $"{uom.Name} {ecd.Qty}",
                                              }).ToList();
                data.ExchangeCanRingDetails = exchangeCanRingDetails;
            }
            return await Task.FromResult(data);
        }
        public async Task<List<CanRingReport>> GetCanRingReportAsync(string dateFrom, string dateTo, int customerId, int paymentMeansId, int userId)
        {
            List<CanRingMaster> canRings = new();
            DateTime _dateFrom = Convert.ToDateTime(dateFrom);
            DateTime _dateTo = Convert.ToDateTime(dateTo);
            if(dateFrom != null && dateTo != null && customerId == 0 && paymentMeansId == 0)
            {
                canRings = _context.CanRingMasters.Where(i => i.CreatedAt >= _dateFrom && i.CreatedAt <= _dateTo && i.UserID == userId).ToList();
            }
            else if (dateFrom != null && dateTo != null && customerId != 0 && paymentMeansId == 0)
            {
                canRings = _context.CanRingMasters.Where(i => i.CreatedAt >= _dateFrom && i.CreatedAt <= _dateTo && i.UserID == userId && i.CusId == customerId).ToList();
            }
            else if (dateFrom != null && dateTo != null && customerId == 0 && paymentMeansId != 0)
            {
                canRings = _context.CanRingMasters.Where(i => i.CreatedAt >= _dateFrom && i.CreatedAt <= _dateTo && i.UserID == userId && i.PaymentMeanID == paymentMeansId).ToList();
            }
            else
            {
                canRings = _context.CanRingMasters.ToList();
            }
            var data = (from crm in canRings
                        join crmd in _context.CanRingDetails on crm.ID equals crmd.CanRingMasterID
                        join doc in _context.DocumentTypes on crm.DocTypeID equals doc.ID
                        join series in _context.Series on crm.SeriesID equals series.ID
                        join pl in _context.PriceLists on crm.PriceListID equals pl.ID
                        join cur in _context.Currency on pl.CurrencyID equals cur.ID
                        join cus in _context.BusinessPartners on crm.CusId equals cus.ID
                        join pm in _context.PaymentMeans on crm.PaymentMeanID equals pm.ID
                        join user in _context.UserAccounts.Include(i => i.Employee).Include(i=> i.Company) on crm.UserID equals user.ID
                        join wh in _context.Warehouses on crm.WarehouseID equals wh.ID
                        join itemChange in _context.ItemMasterDatas on crmd.ItemChangeID equals itemChange.ID
                        join item in _context.ItemMasterDatas on crmd.ItemID equals item.ID
                        join uom in _context.UnitofMeasures on crmd.UomID equals uom.ID
                        join uomChange in _context.UnitofMeasures on crmd.UomChangeID equals uomChange.ID
                        join canring in _context.CanRings on crmd.CanRingID equals canring.ID
                        join curSys in _context.Currency on user.Company.SystemCurrencyID equals curSys.ID
                        join curLocal in _context.Currency on user.Company.LocalCurrencyID equals curLocal.ID
                        select new CanRingReport
                        {
                            // Master
                            CreatedAt = crm.CreatedAt.ToShortDateString(),
                            DocCode = doc.Code,
                            ID = crm.ID,
                            ExchangeRate = crm.ExchangeRate.ToString(),
                            Number = $"{series.Name}-{crm.Number}",
                            TotalDis = $"{cur.Description} {_utility.ToCurrency(canRings.Sum(i=> i.Total), Display.Amounts)}",
                            TotalSystemDis = $"{curSys.Description} {_utility.ToCurrency(canRings.Sum(i => i.TotalSystem), Display.Amounts)}",
                            TotalLocal = $"{curLocal.Description} {_utility.ToCurrency(canRings.Sum(i => i.TotalSystem * (decimal)i.LocalSetRate), DisplayLocal.Amounts)}",
                            CustomerName = cus.Name,
                            PaymentMeans = pm.Type,
                            PriceList = pl.Name,
                            UserName = user.Employee.Name,
                            WarehouseName = wh.Name,
                            // Detial
                            ChangeQty = $"{uomChange.Name} {crmd.ChangeQty}",
                            ItemChangeName = itemChange.KhmerName,
                            Name = canring.Name,
                            ItemName = item.KhmerName,
                            Qty = $"{uom.Name} {crmd.Qty}",
                            Total = $"{cur.Description} {_utility.ToCurrency(crmd.ChargePrice, Display.Amounts)}",
                        }).OrderBy(i => i.CreatedAt).ToList();
            return await Task.FromResult(data);
        }
        public void IssuseStockExchangeCanRing(ExchangeCanRingMaster crm, List<SerialNumber> serials, List<BatchNo> batches, List<SerialViewModelPurchase> serialViewModelPurchases, List<BatchViewModelPurchase> batchViewModelPurchases)
        {
            var Com = _context.Company.FirstOrDefault(c => c.ID == crm.CompanyID);
            var Exr = _context.ExchangeRates.FirstOrDefault(e => e.CurrencyID == Com.LocalCurrencyID);
            var docType = _context.DocumentTypes.FirstOrDefault(w => w.Code == "EC");
            var glAccs = _context.GLAccounts.Where(i => i.IsActive);
            var series = _context.Series.Find(crm.SeriesID) ?? new Series();
            var warehouse = _context.Warehouses.Find(crm.WarehouseID) ?? new Warehouse();
            JournalEntry journalEntry = new();
            SeriesDetail seriesDetail = new();
            string OffsetAcc = "";
            var douTypeID = _context.DocumentTypes.FirstOrDefault(w => w.Code == "JE");
            var defaultJE = _context.Series.FirstOrDefault(w => w.Default == true && w.DocuTypeID == douTypeID.ID && w.NextNo != w.LastNo) ?? new Series();
            if (defaultJE.ID > 0)
            {
                // update series
                string Sno = defaultJE.NextNo;
                long No = long.Parse(Sno);
                defaultJE.NextNo = Convert.ToString(No + 1);
                // update series details
                seriesDetail.SeriesID = journalEntry.SeriesID;
                seriesDetail.Number = Sno;
                _context.Update(defaultJE);
                _context.Update(seriesDetail);
                _context.SaveChanges();
                // Insert Journal Entry
                journalEntry.SeriesID = defaultJE.ID;
                journalEntry.Number = defaultJE.PreFix + "-" + Sno;
                journalEntry.DouTypeID = defaultJE.DocuTypeID;
                journalEntry.Creator = crm.UserID;
                journalEntry.TransNo = crm.Number;
                journalEntry.PostingDate = crm.CreatedAt;
                journalEntry.DocumentDate = crm.CreatedAt;
                journalEntry.DueDate = crm.CreatedAt;
                journalEntry.SSCID = crm.SysCurrencyID;
                journalEntry.LLCID = crm.LocalCurrencyID;
                journalEntry.CompanyID = crm.CompanyID;
                journalEntry.LocalSetRate = (decimal)crm.LocalSetRate;
                journalEntry.SeriesDID = seriesDetail.ID;
                journalEntry.Remarks = series.Name + "-" + crm.Number;
                _context.Update(journalEntry);
            }
            _context.SaveChanges();
            //Debit account receivable  
            List<JournalEntryDetail> journalEntryDetail = new();
            List<AccountBalance> accountBalance = new();
            var accountReceive = _context.BusinessPartners.FirstOrDefault(w => w.ID == crm.CusId);
            var glAccCus = _context.GLAccounts.FirstOrDefault(w => w.ID == accountReceive.GLAccID) ?? new GLAccount();
            OffsetAcc = accountReceive.Code + "-" + glAccCus.Code;
            if (glAccCus.ID > 0)
            {
                journalEntryDetail.Add(new JournalEntryDetail
                {
                    JEID = journalEntry.ID,
                    Type = Type.BPCode,
                    ItemID = accountReceive.GLAccID,
                    Credit = crm.TotalSystem,
                    BPAcctID = crm.CusId,
                });
                //Insert             
                glAccCus.Balance -= crm.TotalSystem;
                accountBalance.Add(new AccountBalance
                {
                    PostingDate = crm.CreatedAt,
                    Origin = docType.ID,
                    OriginNo = crm.Number,
                    OffsetAccount = OffsetAcc,
                    Details = douTypeID.Name + "-" + glAccCus.Code,
                    CumulativeBalance = glAccCus.Balance,
                    Credit = crm.TotalSystem,
                    LocalSetRate = (decimal)crm.LocalSetRate,
                    GLAID = accountReceive.GLAccID,
                    BPAcctID = crm.CusId,
                    Creator = crm.UserID
                });
            }
            var paymentmean = _context.PaymentMeans.Find(crm.PaymentMeanID) ?? new PaymentMeans();
            var glAccCashBank = _context.GLAccounts.FirstOrDefault(w => w.ID == paymentmean.AccountID) ?? new GLAccount();
            if (glAccCashBank.ID > 0)
            {
                journalEntryDetail.Add(new JournalEntryDetail
                {
                    JEID = journalEntry.ID,
                    Type = Type.BPCode,
                    ItemID = paymentmean.AccountID,
                    Credit = crm.TotalSystem,
                });
                //Insert             
                glAccCashBank.Balance -= crm.TotalSystem;
                accountBalance.Add(new AccountBalance
                {
                    PostingDate = crm.CreatedAt,
                    Origin = docType.ID,
                    OriginNo = crm.Number,
                    OffsetAccount = OffsetAcc,
                    Details = douTypeID.Name + "-" + glAccCashBank.Code,
                    CumulativeBalance = glAccCashBank.Balance,
                    Credit = crm.TotalSystem,
                    LocalSetRate = (decimal)crm.LocalSetRate,
                    GLAID = paymentmean.AccountID,
                });
            }

            if (glAccCus.ID > 0)
            {
                journalEntryDetail.Add(new JournalEntryDetail
                {
                    JEID = journalEntry.ID,
                    Type = Type.BPCode,
                    ItemID = accountReceive.GLAccID,
                    Debit = crm.TotalSystem,
                    BPAcctID = crm.CusId,
                });
                //Insert             
                glAccCus.Balance += crm.TotalSystem;
                accountBalance.Add(new AccountBalance
                {
                    PostingDate = crm.CreatedAt,
                    Origin = docType.ID,
                    OriginNo = crm.Number,
                    OffsetAccount = OffsetAcc,
                    Details = douTypeID.Name + "-" + glAccCus.Code,
                    CumulativeBalance = glAccCus.Balance,
                    Debit = crm.TotalSystem,
                    LocalSetRate = (decimal)crm.LocalSetRate,
                    GLAID = accountReceive.GLAccID,
                    BPAcctID = crm.CusId,
                    Creator = crm.UserID
                });
            }
            _context.AccountBalances.UpdateRange(accountBalance);
            _context.GLAccounts.Update(glAccCus);
            _context.SaveChanges();

            foreach (var item in crm.ExchangeCanRingDetails)
            {
                int expenceAccID = 0, inventoryAccID = 0, inventoryAccIncreaseID = 0;
                decimal expenceAccAmount = 0, inventoryAccAmount = 0;
                var itemMaster = _context.ItemMasterDatas.FirstOrDefault(w => w.ID == item.ItemID && !w.Delete);
                var orft = _context.GroupDUoMs.FirstOrDefault(w => w.GroupUoMID == itemMaster.GroupUomID && w.AltUOM == item.UomID);
                double qty = (double)item.Qty * orft.Factor;
                var wareDetails = _context.WarehouseDetails.Where(w => w.WarehouseID == crm.WarehouseID && w.ItemID == item.ItemID).ToList();
                WarehouseDetail lastItemWh = wareDetails.OrderByDescending(i => i.SyetemDate).FirstOrDefault(w => w.InStock > 0) ?? new WarehouseDetail();
                if (itemMaster.SetGlAccount == SetGlAccount.ItemLevel)
                {
                    var itemAccs = _context.ItemAccountings.Where(w => w.ItemID == item.ItemID && w.WarehouseID == crm.WarehouseID);
                    var expenceAcc = (from ia in itemAccs
                                      join gl in glAccs on ia.ExpenseAccount equals gl.Code
                                      select gl
                                         ).FirstOrDefault() ?? new GLAccount();
                    var inventoryAcc = (from ia in itemAccs
                                        join gl in glAccs on ia.InventoryAccount equals gl.Code
                                        select gl
                                         ).FirstOrDefault() ?? new GLAccount();
                    inventoryAccID = inventoryAcc.ID;
                    expenceAccID = expenceAcc.ID;
                }
                else if (itemMaster.SetGlAccount == SetGlAccount.ItemGroup)
                {
                    var itemAccs = _context.ItemAccountings.Where(w => w.ItemGroupID == itemMaster.ItemGroup1ID);
                    var expenceAcc = (from ia in itemAccs
                                      join gl in glAccs on ia.ExpenseAccount equals gl.Code
                                      select gl
                                         ).FirstOrDefault() ?? new GLAccount();
                    var inventoryAcc = (from ia in itemAccs
                                        join gl in glAccs on ia.InventoryAccount equals gl.Code
                                        select gl
                                         ).FirstOrDefault() ?? new GLAccount();
                    inventoryAccID = inventoryAcc.ID;
                    expenceAccID = expenceAcc.ID;
                }
                expenceAccAmount = item.ChargePrice * crm.ExchangeRate;

                #region normal item increase stock
                var itemMasterChange = _context.ItemMasterDatas.FirstOrDefault(w => w.ID == item.ItemChangeID && !w.Delete);
                if (itemMasterChange.SetGlAccount == SetGlAccount.ItemLevel)
                {
                    var itemAccs = _context.ItemAccountings.Where(w => w.ItemID == item.ItemChangeID && w.WarehouseID == crm.WarehouseID);
                    var inventoryAcc = (from ia in itemAccs
                                        join gl in glAccs on ia.InventoryAccount equals gl.Code
                                        select gl
                                         ).FirstOrDefault() ?? new GLAccount();
                    inventoryAccIncreaseID = inventoryAcc.ID;
                }
                else if (itemMasterChange.SetGlAccount == SetGlAccount.ItemGroup)
                {
                    var itemAccs = _context.ItemAccountings.Where(w => w.ItemGroupID == itemMasterChange.ItemGroup1ID);
                    var inventoryAcc = (from ia in itemAccs
                                        join gl in glAccs on ia.InventoryAccount equals gl.Code
                                        select gl
                                         ).FirstOrDefault() ?? new GLAccount();
                    inventoryAccIncreaseID = inventoryAcc.ID;
                }
                if (itemMasterChange.Process != "Standard")
                {
                    InventoryAudit item_inventory_audit = new();
                    WarehouseDetail warehousedetail = new();
                    double _cost = lastItemWh.Cost;
                    var orftChange = _context.GroupDUoMs.FirstOrDefault(w => w.GroupUoMID == itemMasterChange.GroupUomID && w.AltUOM == item.UomChangeID);
                    double qtyChange = (double)item.ChangeQty * orftChange.Factor;
                    var _itemAccChange = _context.ItemAccountings.FirstOrDefault(i => i.WarehouseID == crm.WarehouseID && i.ItemID == item.ItemChangeID);
                    var item_warehouse_summary = _context.WarehouseSummary.FirstOrDefault(w => w.WarehouseID == crm.WarehouseID && w.ItemID == item.ItemChangeID);
                    if (item_warehouse_summary != null)
                    {
                        item_warehouse_summary.InStock += qtyChange;
                        _context.WarehouseSummary.Update(item_warehouse_summary);
                        _utility.UpdateItemAccounting(_itemAccChange, item_warehouse_summary);
                        _context.SaveChanges();
                    }

                    //insert warehousedetail
                    if (itemMasterChange.ManItemBy == ManageItemBy.SerialNumbers && itemMasterChange.ManMethod == ManagementMethod.OnEveryTransation)
                    {

                        var svmp = serialViewModelPurchases.FirstOrDefault(s => s.ItemID == item.ItemChangeID);
                        List<WarehouseDetail> whsDetials = new();
                        List<InventoryAudit> inventoryAudit = new();
                        if (svmp != null)
                        {
                            foreach (var sv in svmp.SerialDetialViewModelPurchase.Where(i => !string.IsNullOrEmpty(i.SerialNumber)).ToList())
                            {
                                whsDetials.Add(new WarehouseDetail
                                {
                                    AdmissionDate = sv.AdmissionDate,
                                    Cost = _cost,
                                    CurrencyID = crm.SysCurrencyID,
                                    Details = sv.Detials,
                                    ID = 0,
                                    InStock = 1,
                                    ItemID = item.ItemChangeID,
                                    Location = sv.Location,
                                    LotNumber = sv.LotNumber,
                                    MfrDate = sv.MfrDate,
                                    MfrSerialNumber = sv.MfrSerialNo,
                                    MfrWarDateEnd = sv.MfrWarrantyEnd,
                                    MfrWarDateStart = sv.MfrWarrantyStart,
                                    ProcessItem = _utility.CheckProcessItem(itemMasterChange.Process),
                                    SerialNumber = sv.SerialNumber,
                                    SyetemDate = DateTime.Now,
                                    SysNum = 0,
                                    TimeIn = DateTime.Now,
                                    WarehouseID = crm.WarehouseID,
                                    UomID = item.UomChangeID,
                                    UserID = crm.UserID,
                                    ExpireDate = sv.ExpirationDate == null ? default : (DateTime)sv.ExpirationDate,
                                    TransType = TransTypeWD.ExchangeCanRing,
                                    BPID = crm.CusId,
                                    IsDeleted = true,
                                    InStockFrom = crm.ID
                                });
                            }
                            //insert inventoryaudit
                            InventoryAudit invAudit = new();
                            var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item.ItemChangeID && w.UomID == item.UomChangeID && w.WarehouseID == crm.WarehouseID);
                            var pri_detial = _context.PriceListDetails.Where(w => w.ItemID == item.ItemChangeID && w.UomID == item.UomChangeID);
                            invAudit.ID = 0;
                            invAudit.WarehouseID = crm.WarehouseID;
                            invAudit.BranchID = crm.BranchID;
                            invAudit.UserID = crm.UserID;
                            invAudit.ItemID = item.ItemChangeID;
                            invAudit.CurrencyID = crm.SysCurrencyID;
                            invAudit.UomID = item.UomChangeID;
                            invAudit.InvoiceNo = crm.Number;
                            invAudit.Trans_Type = docType.Code;
                            invAudit.Process = itemMasterChange.Process;
                            invAudit.SystemDate = DateTime.Now;
                            invAudit.TimeIn = DateTime.Now.ToString("h:mm:ss tt");
                            invAudit.Qty = qtyChange;
                            invAudit.Cost = _cost;
                            invAudit.Price = 0;
                            invAudit.CumulativeQty = inventory_audit.Sum(q => q.Qty) + qtyChange;
                            invAudit.CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) + qtyChange * _cost;
                            invAudit.Trans_Valuse = qtyChange * _cost;
                            invAudit.LocalCurID = crm.LocalCurrencyID;
                            invAudit.LocalSetRate = crm.LocalSetRate;
                            invAudit.DocumentTypeID = crm.DocTypeID;
                            invAudit.CompanyID = crm.CompanyID;
                            invAudit.SeriesID = crm.SeriesID;
                            invAudit.SeriesDetailID = crm.SeriesDID;
                            // update pricelistdetial
                            _utility.CumulativeValue(invAudit.WarehouseID, invAudit.ItemID, invAudit.CumulativeValue, _itemAccChange);
                            _context.InventoryAudits.Add(invAudit);
                            _context.WarehouseDetails.AddRange(whsDetials);
                            _context.SaveChanges();
                        }
                    }
                    else if (itemMasterChange.ManItemBy == ManageItemBy.Batches && itemMasterChange.ManMethod == ManagementMethod.OnEveryTransation)
                    {
                        var bvmp = batchViewModelPurchases.FirstOrDefault(s => s.ItemID == item.ItemChangeID);
                        List<WarehouseDetail> whsDetials = new();

                        if (bvmp != null)
                        {
                            var bvs = bvmp.BatchDetialViewModelPurchases.Where(i => !string.IsNullOrEmpty(i.Batch) && i.Qty > 0).ToList();
                            foreach (var bv in bvs)
                            {
                                var _qty = (double)bv.Qty;
                                whsDetials.Add(new WarehouseDetail
                                {
                                    AdmissionDate = bv.AdmissionDate,
                                    Cost = _cost,
                                    CurrencyID = crm.SysCurrencyID,
                                    Details = bv.Detials,
                                    ID = 0,
                                    InStock = _qty,
                                    ItemID = item.ItemChangeID,
                                    Location = bv.Location,
                                    MfrDate = bv.MfrDate,
                                    ProcessItem = _utility.CheckProcessItem(itemMasterChange.Process),
                                    SyetemDate = DateTime.Now,
                                    SysNum = 0,
                                    TimeIn = DateTime.Now,
                                    WarehouseID = crm.WarehouseID,
                                    UomID = item.UomChangeID,
                                    UserID = crm.UserID,
                                    ExpireDate = bv.ExpirationDate == null ? default : (DateTime)bv.ExpirationDate,
                                    BatchAttr1 = bv.BatchAttribute1,
                                    BatchAttr2 = bv.BatchAttribute2,
                                    BatchNo = bv.Batch,
                                    TransType = TransTypeWD.ExchangeCanRing,
                                    BPID = crm.CusId,
                                    InStockFrom = crm.ID,
                                    IsDeleted = true,
                                });
                            }
                            //insert inventoryaudit
                            InventoryAudit invAudit = new();
                            var inventory_audit = _context.InventoryAudits
                                .Where(w => w.ItemID == item.ItemChangeID && w.UomID == item.UomChangeID && w.WarehouseID == crm.WarehouseID)
                                .ToList();
                            // var pri_detial = _context.PriceListDetails.Where(w => w.ItemID == item.ItemID && w.UomID == item.UomID);
                            invAudit.ID = 0;
                            invAudit.WarehouseID = crm.WarehouseID;
                            invAudit.BranchID = crm.BranchID;
                            invAudit.UserID = crm.UserID;
                            invAudit.ItemID = item.ItemChangeID;
                            invAudit.CurrencyID = crm.SysCurrencyID;
                            invAudit.UomID = item.UomChangeID;
                            invAudit.InvoiceNo = crm.Number;
                            invAudit.Trans_Type = docType.Code;
                            invAudit.Process = itemMasterChange.Process;
                            invAudit.SystemDate = DateTime.Now;
                            invAudit.TimeIn = DateTime.Now.ToString("h:mm:ss tt");
                            invAudit.Qty = qtyChange;
                            invAudit.Cost = _cost;
                            invAudit.Price = 0;
                            invAudit.CumulativeQty = inventory_audit.Sum(q => q.Qty) + qtyChange;
                            invAudit.CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) + qtyChange * _cost;
                            invAudit.Trans_Valuse = qtyChange * _cost;
                            invAudit.LocalCurID = crm.LocalCurrencyID;
                            invAudit.LocalSetRate = crm.LocalSetRate;
                            invAudit.DocumentTypeID = crm.DocTypeID;
                            invAudit.CompanyID = crm.CompanyID;
                            invAudit.SeriesID = crm.SeriesID;
                            invAudit.SeriesDetailID = crm.SeriesDID;
                            // update pricelistdetial
                            _utility.CumulativeValue(invAudit.WarehouseID, invAudit.ItemID, invAudit.CumulativeValue, _itemAccChange);
                            _context.InventoryAudits.Add(invAudit);
                            _context.WarehouseDetails.AddRange(whsDetials);
                            _context.SaveChanges();
                        }
                    }
                    else
                    {
                        warehousedetail.ID = 0;
                        warehousedetail.WarehouseID = crm.WarehouseID;
                        warehousedetail.UserID = crm.UserID;
                        warehousedetail.UomID = item.UomChangeID;
                        warehousedetail.SyetemDate = DateTime.Now;
                        warehousedetail.TimeIn = DateTime.Now;
                        warehousedetail.InStock = qtyChange;
                        warehousedetail.CurrencyID = crm.SysCurrencyID;
                        warehousedetail.ItemID = item.ItemChangeID;
                        warehousedetail.Cost = _cost;
                        warehousedetail.IsDeleted = true;
                        warehousedetail.BPID = crm.CusId;
                        warehousedetail.TransType = TransTypeWD.ExchangeCanRing;
                        warehousedetail.InStockFrom = crm.ID;
                        _context.WarehouseDetails.Add(warehousedetail);
                        _context.SaveChanges();
                        if (itemMasterChange.Process == "FIFO")
                        {
                            //insert inventoryaudit
                            var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item.ItemChangeID && w.UomID == item.UomChangeID && w.WarehouseID == crm.WarehouseID);
                            var pri_detial = _context.PriceListDetails.Where(w => w.ItemID == item.ItemChangeID);
                            item_inventory_audit.ID = 0;
                            item_inventory_audit.WarehouseID = crm.WarehouseID;
                            item_inventory_audit.BranchID = crm.BranchID;
                            item_inventory_audit.UserID = crm.UserID;
                            item_inventory_audit.ItemID = item.ItemChangeID;
                            item_inventory_audit.CurrencyID = crm.SysCurrencyID;
                            item_inventory_audit.UomID = orftChange.BaseUOM;
                            item_inventory_audit.InvoiceNo = crm.Number;
                            item_inventory_audit.Trans_Type = docType.Code;
                            item_inventory_audit.Process = itemMasterChange.Process;
                            item_inventory_audit.SystemDate = DateTime.Now;
                            item_inventory_audit.TimeIn = DateTime.Now.ToShortTimeString();
                            item_inventory_audit.Qty = qtyChange;
                            item_inventory_audit.Cost = _cost;
                            item_inventory_audit.Price = 0;
                            item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) + qtyChange;
                            item_inventory_audit.CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) + qtyChange * _cost;
                            item_inventory_audit.Trans_Valuse = qtyChange * _cost;
                            //item_inventory_audit.ExpireDate = item.ExpireDate;
                            item_inventory_audit.LocalCurID = crm.LocalCurrencyID;
                            item_inventory_audit.LocalSetRate = crm.LocalSetRate;
                            item_inventory_audit.SeriesDetailID = crm.SeriesDID;
                            item_inventory_audit.SeriesID = crm.SeriesID;
                            item_inventory_audit.DocumentTypeID = crm.DocTypeID;
                            item_inventory_audit.CompanyID = crm.CompanyID;
                            //inventoryAccAmount = (decimal)item_inventory_audit.Cost;
                            // update pricelistdetial
                            foreach (var pri in pri_detial)
                            {
                                var guom = _context.GroupDUoMs.FirstOrDefault(g => g.GroupUoMID == itemMasterChange.GroupUomID && g.AltUOM == pri.UomID);
                                var exp = _context.ExchangeRates.FirstOrDefault(w => w.CurrencyID == pri.CurrencyID);
                                pri.Cost = _cost * exp.SetRate * guom.Factor;
                                _context.PriceListDetails.Update(pri);
                            }
                            _utility.CumulativeValue(item_inventory_audit.WarehouseID, item_inventory_audit.ItemID, item_inventory_audit.CumulativeValue, _itemAccChange);
                            _context.InventoryAudits.Add(item_inventory_audit);
                            _context.SaveChanges();
                        }
                        else if (itemMasterChange.Process == "Average")
                        {
                            //insert inventoryaudit
                            var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item.ItemChangeID && w.WarehouseID == crm.WarehouseID);
                            var warehouse_sammary = _context.WarehouseSummary.FirstOrDefault(w => w.ItemID == item.ItemChangeID && w.WarehouseID == crm.WarehouseID);
                            var pri_detial = _context.PriceListDetails.Where(w => w.ItemID == item.ItemChangeID);
                            InventoryAudit avgInAudit = new() { Qty = qtyChange, Cost = _cost };
                            double @AvgCost = _utility.CalAVGCost(item.ItemChangeID, crm.WarehouseID, avgInAudit);
                            @AvgCost = _utility.CheckNaNOrInfinity(@AvgCost);
                            item_inventory_audit.ID = 0;
                            item_inventory_audit.WarehouseID = crm.WarehouseID;
                            item_inventory_audit.BranchID = crm.BranchID;
                            item_inventory_audit.UserID = crm.UserID;
                            item_inventory_audit.ItemID = item.ItemChangeID;
                            item_inventory_audit.CurrencyID = crm.SysCurrencyID;
                            item_inventory_audit.UomID = orftChange.BaseUOM;
                            item_inventory_audit.InvoiceNo = crm.Number;
                            item_inventory_audit.Trans_Type = docType.Code;
                            item_inventory_audit.Process = itemMasterChange.Process;
                            item_inventory_audit.SystemDate = DateTime.Now;
                            item_inventory_audit.TimeIn = DateTime.Now.ToShortTimeString();
                            item_inventory_audit.Qty = qtyChange;
                            item_inventory_audit.Cost = @AvgCost;
                            item_inventory_audit.Price = 0;
                            item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) + qtyChange;
                            item_inventory_audit.CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) + qtyChange * _cost;
                            item_inventory_audit.Trans_Valuse = qtyChange * _cost;
                            //item_inventory_audit.ExpireDate = item.ExpireDate;
                            item_inventory_audit.LocalCurID = crm.LocalCurrencyID;
                            item_inventory_audit.LocalSetRate = crm.LocalSetRate;
                            item_inventory_audit.SeriesDetailID = crm.SeriesDID;
                            item_inventory_audit.SeriesID = crm.SeriesID;
                            item_inventory_audit.DocumentTypeID = crm.DocTypeID;
                            item_inventory_audit.CompanyID = crm.CompanyID;
                            //inventoryAccAmount = (decimal)item_inventory_audit.Cost;
                            // update_warehouse_summary
                            warehouse_sammary.Cost = @AvgCost;
                            _context.WarehouseSummary.Update(warehouse_sammary);
                            // update_pricelistdetial
                            var inventory_pricelist = _context.InventoryAudits.Where(w => w.ItemID == item.ItemChangeID && w.WarehouseID == crm.WarehouseID).ToList();
                            double @AvgCostPL = inventory_pricelist.Sum(s => s.Trans_Valuse) / inventory_pricelist.Sum(q => q.Qty);
                            @AvgCostPL = _utility.CheckNaNOrInfinity(@AvgCostPL);
                            foreach (var pri in pri_detial)
                            {
                                var guom = _context.GroupDUoMs.Where(g => g.GroupUoMID == itemMasterChange.GroupUomID && g.AltUOM == pri.UomID);
                                var exp = _context.ExchangeRates.FirstOrDefault(w => w.CurrencyID == pri.CurrencyID);
                                foreach (var g in guom)
                                {
                                    pri.Cost = @AvgCostPL * exp.SetRate * g.Factor;
                                }
                                _context.PriceListDetails.Update(pri);
                            }
                            _utility.CumulativeValue(item_inventory_audit.WarehouseID, item_inventory_audit.ItemID, item_inventory_audit.CumulativeValue, _itemAccChange);
                            _context.InventoryAudits.Add(item_inventory_audit);
                            _context.SaveChanges();
                        }
                    }
                }
                #endregion

                #region item change redue stock
                if (itemMaster.Process != "Standard")
                {
                    double @Check_Stock;
                    double @Remain;
                    double @IssusQty;
                    double @FIFOQty;
                    double Cost = 0;

                    var _itemAcc = _context.ItemAccountings.FirstOrDefault(i => i.WarehouseID == crm.WarehouseID && i.ItemID == item.ItemID);
                    var item_warehouse_summary = _context.WarehouseSummary.FirstOrDefault(w => w.WarehouseID == crm.WarehouseID && w.ItemID == item.ItemID);
                    if (item_warehouse_summary != null)
                    {
                        item_warehouse_summary.InStock -= qty;
                        _context.WarehouseSummary.Update(item_warehouse_summary);
                        _utility.UpdateItemAccounting(_itemAcc, item_warehouse_summary);
                        _context.SaveChanges();
                    }

                    //Checking Serial Batch //
                    if (itemMaster.ManItemBy == ManageItemBy.SerialNumbers)
                    {
                        if (serials.Count > 0)
                        {
                            List<WareForAudiView> wareForAudis = new();

                            foreach (var s in serials)
                            {
                                if (s.SerialNumberSelected != null)
                                {
                                    foreach (var ss in s.SerialNumberSelected.SerialNumberSelectedDetails)
                                    {
                                        var waredetial = wareDetails.FirstOrDefault(i => ss.SerialNumber == i.SerialNumber && i.InStock > 0);
                                        decimal _inventoryAccAmount = 0M;
                                        if (waredetial != null)
                                        {
                                            Cost = waredetial.Cost;
                                            wareForAudis.Add(new WareForAudiView
                                            {
                                                Cost = waredetial.Cost,
                                                Qty = waredetial.InStock,
                                                ExpireDate = waredetial.ExpireDate,
                                            });
                                            waredetial.InStock -= 1;
                                            // insert to warehouse detail
                                            var stockOut = new StockOut
                                            {
                                                AdmissionDate = waredetial.AdmissionDate,
                                                Cost = (decimal)waredetial.Cost,
                                                CurrencyID = waredetial.CurrencyID,
                                                Details = waredetial.Details,
                                                ID = 0,
                                                InStock = 1,
                                                ItemID = waredetial.ItemID,
                                                Location = waredetial.Location,
                                                LotNumber = waredetial.LotNumber,
                                                MfrDate = waredetial.MfrDate,
                                                MfrSerialNumber = waredetial.MfrSerialNumber,
                                                MfrWarDateEnd = waredetial.MfrWarDateEnd,
                                                MfrWarDateStart = waredetial.MfrWarDateStart,
                                                ProcessItem = _utility.CheckProcessItem(itemMaster.Process),
                                                SerialNumber = waredetial.SerialNumber,
                                                SyetemDate = DateTime.Now,
                                                SysNum = 0,
                                                TimeIn = DateTime.Now,
                                                WarehouseID = waredetial.WarehouseID,
                                                UomID = item.UomChangeID,
                                                UserID = crm.UserID,
                                                ExpireDate = waredetial.ExpireDate,
                                                TransType = TransTypeWD.ExchangeCanRing,
                                                TransID = crm.ID,
                                                Contract = itemMaster.ContractID,
                                                OutStockFrom = crm.ID,
                                                FromWareDetialID = waredetial.ID,
                                                BPID = crm.CusId
                                            };
                                            _inventoryAccAmount = (decimal)waredetial.Cost;
                                            inventoryAccAmount += _inventoryAccAmount;
                                            _context.StockOuts.Add(stockOut);
                                            _context.SaveChanges();
                                        }
                                        InsertFinancialCanRingInventory(inventoryAccID, _inventoryAccAmount, journalEntryDetail, accountBalance, journalEntry, docType, douTypeID, crm, OffsetAcc, false);
                                    }
                                }
                            }
                            wareForAudis = (from wa in wareForAudis
                                            group wa by wa.Cost into g
                                            let wha = g.FirstOrDefault()
                                            select new WareForAudiView
                                            {
                                                Qty = g.Sum(i => i.Qty),
                                                Cost = wha.Cost,
                                                ExpireDate = wha.ExpireDate,
                                            }).ToList();
                            if (wareForAudis.Any())
                            {
                                foreach (var i in wareForAudis)
                                {
                                    // Insert to Inventory Audit
                                    var inventory_audit = _context.InventoryAudits
                                        .Where(w => w.ItemID == item.ItemID && w.WarehouseID == crm.WarehouseID && w.Cost == i.Cost).ToList();
                                    //var item_IssusStock = wareDetails.FirstOrDefault(w => w.InStock > 0);
                                    var inventory = new InventoryAudit
                                    {
                                        ID = 0,
                                        WarehouseID = crm.WarehouseID,
                                        BranchID = crm.BranchID,
                                        UserID = crm.UserID,
                                        ItemID = item.ItemID,
                                        CurrencyID = crm.SysCurrencyID,
                                        UomID = orft.BaseUOM,
                                        InvoiceNo = crm.Number,
                                        Trans_Type = docType.Code,
                                        Process = itemMaster.Process,
                                        SystemDate = DateTime.Now,
                                        TimeIn = DateTime.Now.ToShortTimeString().ToString(),
                                        Qty = i.Qty * -1,
                                        Cost = i.Cost,
                                        Price = 0,
                                        CumulativeQty = inventory_audit.Sum(q => q.Qty) - i.Qty,
                                        CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) - (i.Qty * i.Cost),
                                        Trans_Valuse = i.Qty * i.Cost * -1,
                                        ExpireDate = i.ExpireDate,
                                        LocalCurID = crm.LocalCurrencyID,
                                        LocalSetRate = crm.LocalSetRate,
                                        CompanyID = crm.CompanyID,
                                        DocumentTypeID = docType.ID,
                                        SeriesID = crm.SeriesID,
                                        SeriesDetailID = crm.SeriesDID,
                                    };
                                    _utility.CumulativeValue(inventory.WarehouseID, inventory.ItemID, inventory.CumulativeValue, _itemAcc);
                                    _context.InventoryAudits.Add(inventory);
                                    _context.SaveChanges();
                                }
                            }
                        }
                    }
                    else if (itemMaster.ManItemBy == ManageItemBy.Batches)
                    {
                        if (batches.Count > 0)
                        {
                            List<WareForAudiView> wareForAudis = new();
                            foreach (var b in batches)
                            {
                                if (b.BatchNoSelected != null)
                                {
                                    foreach (var sb in b.BatchNoSelected.BatchNoSelectedDetails)
                                    {
                                        decimal _inventoryAccAmount = 0M;
                                        decimal selectedQty = sb.SelectedQty * (decimal)orft.Factor;
                                        var waredetial = wareDetails.FirstOrDefault(i => sb.BatchNo == i.BatchNo && i.InStock > 0);
                                        if (waredetial != null)
                                        {
                                            wareForAudis.Add(new WareForAudiView
                                            {
                                                Cost = waredetial.Cost,
                                                Qty = (double)selectedQty,
                                                ExpireDate = waredetial.ExpireDate,
                                            });
                                            waredetial.InStock -= (double)selectedQty;
                                            Cost = waredetial.Cost;
                                            // insert to waredetial
                                            var stockOut = new StockOut
                                            {
                                                AdmissionDate = waredetial.AdmissionDate,
                                                Cost = (decimal)waredetial.Cost,
                                                CurrencyID = waredetial.CurrencyID,
                                                Details = waredetial.Details,
                                                ID = 0,
                                                InStock = selectedQty,
                                                ItemID = item.ItemID,
                                                Location = waredetial.Location,
                                                MfrDate = waredetial.MfrDate,
                                                ProcessItem = _utility.CheckProcessItem(itemMaster.Process),
                                                SyetemDate = DateTime.Now,
                                                SysNum = 0,
                                                TimeIn = DateTime.Now,
                                                WarehouseID = waredetial.WarehouseID,
                                                UomID = item.UomID,
                                                UserID = crm.UserID,
                                                ExpireDate = waredetial.ExpireDate,
                                                BatchAttr1 = waredetial.BatchAttr1,
                                                BatchAttr2 = waredetial.BatchAttr2,
                                                BatchNo = waredetial.BatchNo,
                                                TransType = TransTypeWD.ExchangeCanRing,
                                                TransID = crm.ID,
                                                OutStockFrom = crm.ID,
                                                FromWareDetialID = waredetial.ID,
                                                BPID = crm.CusId
                                            };
                                            _inventoryAccAmount = (decimal)waredetial.Cost;
                                            inventoryAccAmount += _inventoryAccAmount;
                                            _context.StockOuts.Add(stockOut);
                                            _context.SaveChanges();
                                        }
                                        InsertFinancialCanRingInventory(inventoryAccID, _inventoryAccAmount, journalEntryDetail, accountBalance, journalEntry, docType, douTypeID, crm, OffsetAcc, false);
                                    }
                                }
                            }
                            wareForAudis = (from wa in wareForAudis
                                            group wa by wa.Cost into g
                                            let wha = g.FirstOrDefault()
                                            select new WareForAudiView
                                            {
                                                Qty = g.Sum(i => i.Qty),
                                                Cost = wha.Cost,
                                                ExpireDate = wha.ExpireDate
                                            }).ToList();

                            if (wareForAudis.Any())
                            {
                                foreach (var i in wareForAudis)
                                {
                                    // insert to inventory audit
                                    var inventory_audit = _context.InventoryAudits
                                        .Where(w => w.ItemID == item.ItemID && w.WarehouseID == crm.WarehouseID && w.Cost == i.Cost).ToList();
                                    var inventory = new InventoryAudit
                                    {
                                        ID = 0,
                                        WarehouseID = crm.WarehouseID,
                                        BranchID = crm.BranchID,
                                        UserID = crm.UserID,
                                        ItemID = item.ItemID,
                                        CurrencyID = crm.SysCurrencyID,
                                        UomID = orft.BaseUOM,
                                        InvoiceNo = crm.Number,
                                        Trans_Type = docType.Code,
                                        Process = itemMaster.Process,
                                        SystemDate = DateTime.Now,
                                        TimeIn = DateTime.Now.ToShortTimeString().ToString(),
                                        Qty = i.Qty * -1,
                                        Cost = i.Cost,
                                        Price = 0,
                                        CumulativeQty = inventory_audit.Sum(q => q.Qty) - i.Qty,
                                        CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) - (i.Qty * i.Cost),
                                        Trans_Valuse = i.Qty * i.Cost * -1,
                                        ExpireDate = i.ExpireDate,
                                        LocalCurID = crm.LocalCurrencyID,
                                        LocalSetRate = crm.LocalSetRate,
                                        CompanyID = crm.CompanyID,
                                        DocumentTypeID = docType.ID,
                                        SeriesID = crm.SeriesID,
                                        SeriesDetailID = crm.SeriesDID,
                                    };
                                    _utility.CumulativeValue(inventory.WarehouseID, inventory.ItemID, inventory.CumulativeValue, _itemAcc);
                                    _context.InventoryAudits.Add(inventory);
                                    _context.SaveChanges();
                                }
                            }
                        }
                    }
                    else
                    {
                        List<WarehouseDetail> _whlists = wareDetails.Where(w => w.InStock > 0).OrderBy(i => i.SyetemDate).ToList();
                        if (warehouse.IsAllowNegativeStock && _whlists.Count == 0)
                        {
                            var wh = wareDetails.LastOrDefault();
                            _whlists.Add(wh);
                        }
                        foreach (var (item_warehouse, index) in _whlists.Select((value, i) => (value, i)))
                        {
                            InventoryAudit item_inventory_audit = new();
                            WarehouseDetail item_IssusStock = new();
                            @Check_Stock = item_warehouse.InStock - qty;
                            if (@Check_Stock < 0)
                            {
                                @Remain = @Check_Stock * (-1);
                                @IssusQty = qty - @Remain;
                                if (@Remain <= 0)
                                {
                                    qty = 0;
                                }
                                else if (qty > 0 && index == _whlists.Count - 1 && warehouse.IsAllowNegativeStock)
                                {
                                    @IssusQty = qty;
                                }
                                else
                                {
                                    qty = @Remain;
                                }

                                if (itemMaster.Process == "FIFO")
                                {
                                    item_IssusStock = item_warehouse;
                                    double _cost = item_IssusStock.Cost;
                                    item_IssusStock.InStock -= @IssusQty;
                                    if (@IssusQty > 0)
                                    {
                                        var stockOuts = new StockOut
                                        {
                                            Cost = (decimal)item_warehouse.Cost,
                                            CurrencyID = item_warehouse.CurrencyID,
                                            ID = 0,
                                            InStock = (decimal)@IssusQty,
                                            ItemID = item.ItemID,
                                            ProcessItem = _utility.CheckProcessItem(itemMaster.Process),
                                            SyetemDate = DateTime.Now,
                                            TimeIn = DateTime.Now,
                                            WarehouseID = item_warehouse.WarehouseID,
                                            UomID = item.UomID,
                                            UserID = crm.UserID,
                                            ExpireDate = item_IssusStock.ExpireDate,
                                            TransType = TransTypeWD.ExchangeCanRing,
                                            TransID = crm.ID,
                                            OutStockFrom = crm.ID,
                                            BPID = crm.CusId,
                                            FromWareDetialID = item_IssusStock.ID,
                                        };
                                        _context.StockOuts.Add(stockOuts);
                                        var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item.ItemChangeID && w.WarehouseID == crm.WarehouseID).ToList();
                                        item_inventory_audit.ID = 0;
                                        item_inventory_audit.WarehouseID = crm.WarehouseID;
                                        item_inventory_audit.BranchID = crm.BranchID;
                                        item_inventory_audit.UserID = crm.UserID;
                                        item_inventory_audit.ItemID = item.ItemChangeID;
                                        item_inventory_audit.CurrencyID = Com.SystemCurrencyID;
                                        item_inventory_audit.UomID = orft.BaseUOM;
                                        item_inventory_audit.InvoiceNo = crm.Number;
                                        item_inventory_audit.Trans_Type = docType.Code;
                                        item_inventory_audit.Process = itemMaster.Process;
                                        item_inventory_audit.SystemDate = DateTime.Now;
                                        item_inventory_audit.TimeIn = DateTime.Now.ToShortTimeString().ToString();
                                        item_inventory_audit.Qty = @IssusQty * -1;
                                        item_inventory_audit.Cost = _cost;
                                        item_inventory_audit.Price = 0;
                                        item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) - @IssusQty;
                                        item_inventory_audit.CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) - (@IssusQty * _cost);
                                        item_inventory_audit.Trans_Valuse = @IssusQty * item_IssusStock.Cost * -1;
                                        item_inventory_audit.ExpireDate = item_IssusStock.ExpireDate;
                                        item_inventory_audit.LocalCurID = crm.LocalCurrencyID;
                                        item_inventory_audit.LocalSetRate = crm.LocalSetRate;
                                        item_inventory_audit.CompanyID = crm.CompanyID;
                                        item_inventory_audit.DocumentTypeID = docType.ID;
                                        item_inventory_audit.SeriesID = crm.SeriesID;
                                        item_inventory_audit.SeriesDetailID = crm.SeriesDID;
                                    }
                                    inventoryAccAmount += (decimal)(item_inventory_audit.Cost * @IssusQty);
                                    _utility.CumulativeValue(item_inventory_audit.WarehouseID, item_inventory_audit.ItemID, item_inventory_audit.CumulativeValue, _itemAcc);
                                    _context.WarehouseDetails.Update(item_IssusStock);
                                    _context.InventoryAudits.Add(item_inventory_audit);
                                    _context.SaveChanges();
                                }
                                else if (itemMaster.Process == "Average")
                                {
                                    item_IssusStock = wareDetails.OrderByDescending(i => i.SyetemDate).FirstOrDefault();
                                    item_IssusStock.InStock -= @IssusQty;
                                    if (@IssusQty > 0)
                                    {
                                        var warehouse_summary = _context.WarehouseSummary.FirstOrDefault(w => w.ItemID == item.ItemChangeID && w.WarehouseID == crm.WarehouseID);
                                        double @sysAvCost = warehouse_summary.Cost;
                                        var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item.ItemChangeID && w.WarehouseID == crm.WarehouseID).ToList();
                                        var stockOuts = new StockOut
                                        {
                                            Cost = (decimal)@sysAvCost,
                                            CurrencyID = item_warehouse.CurrencyID,
                                            ID = 0,
                                            InStock = (decimal)@IssusQty,
                                            ItemID = item.ItemChangeID,
                                            ProcessItem = _utility.CheckProcessItem(itemMaster.Process),
                                            SyetemDate = DateTime.Now,
                                            TimeIn = DateTime.Now,
                                            WarehouseID = item_warehouse.WarehouseID,
                                            UomID = item.UomChangeID,
                                            UserID = crm.UserID,
                                            ExpireDate = item_IssusStock.ExpireDate,
                                            TransType = TransTypeWD.ExchangeCanRing,
                                            TransID = crm.ID,
                                            OutStockFrom = crm.ID,
                                            BPID = crm.CusId,
                                            FromWareDetialID = item_IssusStock.ID,
                                        };
                                        _context.StockOuts.Add(stockOuts);

                                        item_inventory_audit.ID = 0;
                                        item_inventory_audit.WarehouseID = crm.WarehouseID;
                                        item_inventory_audit.BranchID = crm.BranchID;
                                        item_inventory_audit.UserID = crm.UserID;
                                        item_inventory_audit.ItemID = item.ItemChangeID;
                                        item_inventory_audit.CurrencyID = Com.SystemCurrencyID;
                                        item_inventory_audit.UomID = orft.BaseUOM;
                                        item_inventory_audit.InvoiceNo = crm.Number;
                                        item_inventory_audit.Trans_Type = docType.Code;
                                        item_inventory_audit.Process = itemMaster.Process;
                                        item_inventory_audit.SystemDate = DateTime.Now;
                                        item_inventory_audit.TimeIn = DateTime.Now.ToShortTimeString().ToString();
                                        item_inventory_audit.Qty = @IssusQty * -1;
                                        item_inventory_audit.Cost = @sysAvCost;
                                        item_inventory_audit.Price = 0;
                                        item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) - @IssusQty;
                                        item_inventory_audit.CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) - (@IssusQty * @sysAvCost);
                                        item_inventory_audit.Trans_Valuse = @IssusQty * @sysAvCost * -1;
                                        item_inventory_audit.ExpireDate = item_IssusStock.ExpireDate;
                                        item_inventory_audit.LocalCurID = crm.LocalCurrencyID;
                                        item_inventory_audit.LocalSetRate = crm.LocalSetRate;
                                        item_inventory_audit.CompanyID = crm.CompanyID;
                                        item_inventory_audit.DocumentTypeID = docType.ID;
                                        item_inventory_audit.SeriesID = crm.SeriesID;
                                        item_inventory_audit.SeriesDetailID = crm.SeriesDID;
                                    }
                                    double @AvgCost = _utility.CalAVGCost(item.ItemChangeID, crm.WarehouseID, item_inventory_audit);
                                    inventoryAccAmount += (decimal)(@AvgCost * @IssusQty);
                                    _utility.UpdateAvgCost(item_warehouse.ItemID, crm.WarehouseID, itemMaster.GroupUomID, item_inventory_audit.Qty, item_inventory_audit.Cost);
                                    _utility.UpdateBomCost(item_warehouse.ItemID, item_inventory_audit.Qty, item_inventory_audit.Cost);
                                    _utility.CumulativeValue(item_inventory_audit.WarehouseID, item_inventory_audit.ItemID, item_inventory_audit.CumulativeValue, _itemAcc);
                                    _context.WarehouseDetails.Update(item_IssusStock);
                                    _context.InventoryAudits.Add(item_inventory_audit);
                                    _context.SaveChanges();
                                }
                            }
                            else
                            {

                                if (itemMaster.Process == "FIFO")
                                {
                                    item_IssusStock = item_warehouse;
                                    @FIFOQty = item_IssusStock.InStock - qty;
                                    @IssusQty = item_IssusStock.InStock - @FIFOQty;
                                    item_IssusStock.InStock = @FIFOQty;
                                    if (@IssusQty > 0)
                                    {
                                        var stockOuts = new StockOut
                                        {
                                            Cost = (decimal)item_warehouse.Cost,
                                            CurrencyID = item_warehouse.CurrencyID,
                                            ID = 0,
                                            InStock = (decimal)@IssusQty,
                                            ItemID = item.ItemID,
                                            ProcessItem = _utility.CheckProcessItem(itemMaster.Process),
                                            SyetemDate = DateTime.Now,
                                            TimeIn = DateTime.Now,
                                            WarehouseID = item_warehouse.WarehouseID,
                                            UomID = item.UomID,
                                            UserID = crm.UserID,
                                            ExpireDate = item_IssusStock.ExpireDate,
                                            TransType = TransTypeWD.ExchangeCanRing,
                                            TransID = crm.ID,
                                            OutStockFrom = crm.ID,
                                            BPID = crm.CusId,
                                            FromWareDetialID = item_IssusStock.ID,
                                        };
                                        _context.StockOuts.Add(stockOuts);
                                        var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item.ItemID && w.WarehouseID == crm.WarehouseID).ToList();
                                        item_inventory_audit.ID = 0;
                                        item_inventory_audit.WarehouseID = crm.WarehouseID;
                                        item_inventory_audit.BranchID = crm.BranchID;
                                        item_inventory_audit.UserID = crm.UserID;
                                        item_inventory_audit.ItemID = item.ItemID;
                                        item_inventory_audit.CurrencyID = Com.SystemCurrencyID;
                                        item_inventory_audit.UomID = orft.BaseUOM;
                                        item_inventory_audit.InvoiceNo = crm.Number;
                                        item_inventory_audit.Trans_Type = docType.Code;
                                        item_inventory_audit.Process = itemMaster.Process;
                                        item_inventory_audit.SystemDate = DateTime.Now;
                                        item_inventory_audit.TimeIn = DateTime.Now.ToShortTimeString().ToString();
                                        item_inventory_audit.Qty = @IssusQty * -1;
                                        item_inventory_audit.Cost = item_IssusStock.Cost;
                                        item_inventory_audit.Price = 0;
                                        item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) - @IssusQty;
                                        item_inventory_audit.CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) - (@IssusQty * item_IssusStock.Cost);
                                        item_inventory_audit.Trans_Valuse = @IssusQty * item_IssusStock.Cost * -1;
                                        item_inventory_audit.ExpireDate = item_IssusStock.ExpireDate;
                                        item_inventory_audit.LocalCurID = crm.LocalCurrencyID;
                                        item_inventory_audit.LocalSetRate = crm.LocalSetRate;
                                        item_inventory_audit.CompanyID = crm.CompanyID;
                                        item_inventory_audit.DocumentTypeID = docType.ID;
                                        item_inventory_audit.SeriesID = crm.SeriesID;
                                        item_inventory_audit.SeriesDetailID = crm.SeriesDID;
                                    }
                                    inventoryAccAmount += (decimal)item_inventory_audit.Cost * (decimal)@IssusQty;
                                    _utility.CumulativeValue(item_inventory_audit.WarehouseID, item_inventory_audit.ItemID, item_inventory_audit.CumulativeValue, _itemAcc);
                                    _context.WarehouseDetails.Update(item_IssusStock);
                                    _context.InventoryAudits.Add(item_inventory_audit);
                                    _context.SaveChanges();
                                }
                                else if (itemMaster.Process == "Average")
                                {
                                    item_IssusStock = wareDetails.OrderByDescending(i => i.SyetemDate).FirstOrDefault();
                                    @FIFOQty = item_IssusStock.InStock - qty;
                                    @IssusQty = item_IssusStock.InStock - @FIFOQty;
                                    item_IssusStock.InStock = @FIFOQty;
                                    if (@IssusQty > 0)
                                    {
                                        var warehouse_summary = _context.WarehouseSummary.FirstOrDefault(w => w.ItemID == item.ItemID && w.WarehouseID == crm.WarehouseID);
                                        double @sysAvCost = warehouse_summary.Cost;
                                        var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item.ItemID && w.WarehouseID == crm.WarehouseID).ToList();

                                        var stockOuts = new StockOut
                                        {
                                            Cost = (decimal)@sysAvCost,
                                            CurrencyID = item_warehouse.CurrencyID,
                                            ID = 0,
                                            InStock = (decimal)@IssusQty,
                                            ItemID = item.ItemID,
                                            ProcessItem = _utility.CheckProcessItem(itemMaster.Process),
                                            SyetemDate = DateTime.Now,
                                            TimeIn = DateTime.Now,
                                            WarehouseID = item_warehouse.WarehouseID,
                                            UomID = item.UomID,
                                            UserID = crm.UserID,
                                            ExpireDate = item_IssusStock.ExpireDate,
                                            TransType = TransTypeWD.ExchangeCanRing,
                                            TransID = crm.ID,
                                            OutStockFrom = crm.ID,
                                            FromWareDetialID = item_IssusStock.ID,
                                            BPID = crm.CusId
                                        };
                                        _context.StockOuts.Add(stockOuts);
                                        item_inventory_audit.ID = 0;
                                        item_inventory_audit.WarehouseID = crm.WarehouseID;
                                        item_inventory_audit.BranchID = crm.BranchID;
                                        item_inventory_audit.UserID = crm.UserID;
                                        item_inventory_audit.ItemID = item.ItemID;
                                        item_inventory_audit.CurrencyID = Com.SystemCurrencyID;
                                        item_inventory_audit.UomID = orft.BaseUOM;
                                        item_inventory_audit.InvoiceNo = crm.Number;
                                        item_inventory_audit.Trans_Type = docType.Code;
                                        item_inventory_audit.Process = itemMaster.Process;
                                        item_inventory_audit.SystemDate = DateTime.Now;
                                        item_inventory_audit.TimeIn = DateTime.Now.ToShortTimeString().ToString();
                                        item_inventory_audit.Qty = @IssusQty * -1;
                                        item_inventory_audit.Cost = @sysAvCost;
                                        item_inventory_audit.Price = 0;
                                        item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) - @IssusQty;
                                        item_inventory_audit.CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) - (@IssusQty * @sysAvCost);
                                        item_inventory_audit.Trans_Valuse = @IssusQty * @sysAvCost * -1;
                                        item_inventory_audit.ExpireDate = item_IssusStock.ExpireDate;
                                        item_inventory_audit.LocalCurID = crm.LocalCurrencyID;
                                        item_inventory_audit.LocalSetRate = crm.LocalSetRate;
                                        item_inventory_audit.CompanyID = crm.CompanyID;
                                        item_inventory_audit.DocumentTypeID = docType.ID;
                                        item_inventory_audit.SeriesID = crm.SeriesID;
                                        item_inventory_audit.SeriesDetailID = crm.SeriesDID;
                                    }
                                    double @AvgCost = _utility.CalAVGCost(item.ItemChangeID, crm.WarehouseID, item_inventory_audit);
                                    inventoryAccAmount += (decimal)(@AvgCost * @IssusQty);
                                    _utility.UpdateAvgCost(item_warehouse.ItemID, crm.WarehouseID, itemMaster.GroupUomID, item_inventory_audit.Qty, item_inventory_audit.Cost);
                                    _utility.UpdateBomCost(item_warehouse.ItemID, item_inventory_audit.Qty, item_inventory_audit.Cost);
                                    _utility.CumulativeValue(item_inventory_audit.WarehouseID, item_inventory_audit.ItemID, item_inventory_audit.CumulativeValue, _itemAcc);
                                    _context.WarehouseDetails.Update(item_IssusStock);
                                    _context.InventoryAudits.Add(item_inventory_audit);
                                    _context.SaveChanges();
                                }
                                wareDetails = new List<WarehouseDetail>();
                                break;
                            }
                        }
                    }
                }
                else
                {
                    var priceListDetail = _context.PriceListDetails.FirstOrDefault(w => w.ItemID == item.ItemID && w.UomID == item.UomID && w.PriceListID == crm.PriceListID) ?? new PriceListDetail();
                    var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item.ItemID && w.WarehouseID == crm.WarehouseID).ToList();
                    inventoryAccAmount += (decimal)priceListDetail.Cost * item.ChangeQty * crm.ExchangeRate;
                    InventoryAudit item_inventory_audit = new()
                    {
                        ID = 0,
                        WarehouseID = crm.WarehouseID,
                        BranchID = crm.BranchID,
                        UserID = crm.UserID,
                        ItemID = item.ItemID,
                        CurrencyID = Com.SystemCurrencyID,
                        UomID = orft.BaseUOM,
                        InvoiceNo = crm.Number,
                        Trans_Type = docType.Code,
                        Process = itemMaster.Process,
                        SystemDate = DateTime.Now,
                        TimeIn = DateTime.Now.ToShortTimeString().ToString(),
                        Qty = qty * -1,
                        Cost = priceListDetail.Cost,
                        Price = 0,
                        CumulativeQty = inventory_audit.Sum(q => q.Qty) - qty,
                        CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) - (qty * priceListDetail.Cost),
                        Trans_Valuse = qty * priceListDetail.Cost * -1,
                        //ExpireDate = item_IssusStock.ExpireDate,
                        LocalCurID = crm.LocalCurrencyID,
                        LocalSetRate = crm.LocalSetRate,
                        CompanyID = crm.CompanyID,
                        DocumentTypeID = docType.ID,
                        SeriesID = crm.SeriesID,
                        SeriesDetailID = crm.SeriesDID,
                        TypeItem = "Standard",
                    };
                    _context.InventoryAudits.Update(item_inventory_audit);
                    _context.SaveChanges();
                }
                #endregion
                var glAccExpfifo = _context.GLAccounts.FirstOrDefault(w => w.ID == expenceAccID) ?? new GLAccount();
                if (glAccExpfifo.ID > 0)
                {
                    var list = journalEntryDetail.FirstOrDefault(w => w.ItemID == glAccExpfifo.ID) ?? new JournalEntryDetail();
                    if (list.ItemID > 0)
                    {
                        var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == expenceAccID);
                        glAccExpfifo.Balance += expenceAccAmount;
                        //journalEntryDetail
                        list.Debit += expenceAccAmount;
                        //accountBalance
                        accBalance.CumulativeBalance = glAccExpfifo.Balance;
                        accBalance.Debit += expenceAccAmount;
                    }
                    else
                    {
                        glAccExpfifo.Balance += expenceAccAmount;
                        journalEntryDetail.Add(new JournalEntryDetail
                        {
                            JEID = journalEntry.ID,
                            Type = Type.GLAcct,
                            ItemID = expenceAccID,
                            Debit = expenceAccAmount,
                        });
                        //
                        accountBalance.Add(new AccountBalance
                        {
                            PostingDate = crm.CreatedAt,
                            Origin = docType.ID,
                            OriginNo = crm.Number,
                            OffsetAccount = OffsetAcc,
                            Details = douTypeID.Name + "-" + glAccExpfifo.Code,
                            CumulativeBalance = glAccExpfifo.Balance,
                            Debit = expenceAccAmount,
                            LocalSetRate = (decimal)crm.LocalSetRate,
                            GLAID = expenceAccID,
                        });
                    }
                    _context.Update(glAccExpfifo);
                }
                InsertFinancialCanRingInventory(inventoryAccIncreaseID, inventoryAccAmount, journalEntryDetail, accountBalance, journalEntry, docType, douTypeID, crm, OffsetAcc);
                if (itemMasterChange.ManItemBy == ManageItemBy.None)
                {
                    InsertFinancialCanRingInventory(inventoryAccID, inventoryAccAmount, journalEntryDetail, accountBalance, journalEntry, docType, douTypeID, crm, OffsetAcc, false);
                }
            }
            //IssuseInStockMaterial
            List<ItemMaterial> itemMaterials = new();
            foreach (var item in crm.ExchangeCanRingDetails)
            {
                var itemM = _context.ItemMasterDatas.FirstOrDefault(w => w.ID == item.ItemChangeID);
                var orft = _context.GroupDUoMs.FirstOrDefault(w => w.GroupUoMID == itemM.GroupUomID && w.AltUOM == item.UomChangeID);
                var bom = _context.BOMaterial.FirstOrDefault(w => w.ItemID == item.ItemChangeID && w.Active == true) ?? new BOMaterial();
                var items_material = (from bomd in _context.BOMDetail.Where(w => w.BID == bom.BID && w.Detele == false)
                                      join i in _context.ItemMasterDatas.Where(im => !im.Delete) on bomd.ItemID equals i.ID
                                      join gd in _context.GroupDUoMs on bomd.UomID equals gd.AltUOM
                                      join uom in _context.UnitofMeasures on i.InventoryUoMID equals uom.ID
                                      select new ItemMaterial
                                      {
                                          ItemID = bomd.ItemID,
                                          GroupUoMID = gd.GroupUoMID,
                                          GUoMID = i.GroupUomID,
                                          Qty = (double)item.ChangeQty * orft.Factor * ((double)bomd.Qty * gd.Factor),
                                          NegativeStock = bomd.NegativeStock,
                                          Process = i.Process,
                                          UomID = uom.ID,
                                          Factor = gd.Factor,
                                      }).Where(w => w.GroupUoMID == w.GUoMID).ToList();
                itemMaterials.AddRange(items_material);
                #region bom normal item increase stock
                var ItemBOMDetail = _context.BOMDetail.Where(w => w.ItemID == item.ItemID);
                foreach (var itembom in ItemBOMDetail)
                {
                    var BOM = _context.BOMaterial.First(w => w.BID == itembom.BID);
                    var DBOM = _context.BOMDetail.Where(w => w.BID == BOM.BID && w.Detele == false);
                    var Inven = _context.InventoryAudits.Where(w => w.ItemID == itembom.ItemID);
                    double @AvgCost = Inven.Sum(s => s.Trans_Valuse) / Inven.Sum(q => q.Qty);
                    @AvgCost = _utility.CheckNaNOrInfinity(@AvgCost);
                    var Gdoum = _context.GroupDUoMs.Where(w => w.GroupUoMID == itembom.GUomID);
                    var Factor = Gdoum.FirstOrDefault(w => w.AltUOM == itembom.UomID).Factor;
                    itembom.Cost = @AvgCost * Factor;
                    itembom.Amount = itembom.Qty * (@AvgCost * Factor);
                    _context.BOMDetail.UpdateRange(ItemBOMDetail);
                    _context.SaveChanges();
                    BOM.TotalCost = DBOM.Sum(w => w.Amount);
                    _context.BOMaterial.Update(BOM);
                    _context.SaveChanges();
                }
                #endregion
            }

            #region bom change item reduce stock
            var allMaterials = (from all in itemMaterials
                                group new { all } by new { all.ItemID, all.NegativeStock } into g
                                let data = g.FirstOrDefault()
                                select new
                                {
                                    data.all.ItemID,
                                    data.all.GroupUoMID,
                                    data.all.GUoMID,
                                    Qty = g.Sum(s => s.all.Qty),
                                    data.all.NegativeStock,
                                    data.all.Process,
                                    data.all.UomID,
                                    data.all.Factor,
                                }).ToList();
            if (allMaterials.Count > 0)
            {
                foreach (var item_detail in allMaterials.ToList())
                {
                    var item_master_data = _context.ItemMasterDatas.FirstOrDefault(w => w.ID == item_detail.ItemID);
                    var item_warehouse_material = _context.WarehouseSummary.FirstOrDefault(w => w.WarehouseID == crm.WarehouseID && w.ItemID == item_detail.ItemID);
                    var _itemAcc = _context.ItemAccountings.FirstOrDefault(i => i.WarehouseID == crm.WarehouseID && i.ItemID == item_detail.ItemID);
                    var all_item_warehouse_detail = _context.WarehouseDetails.Where(w => w.WarehouseID == crm.WarehouseID && w.ItemID == item_detail.ItemID).ToList();
                    var item_nagative = from wa in _context.WarehouseSummary.Where(w => w.ItemID == item_detail.ItemID)
                                        join na in _context.BOMDetail on wa.ItemID equals na.ItemID
                                        select new
                                        {
                                            NagaStock = wa.InStock
                                        };
                    var nagative_check = item_nagative.Sum(w => w.NagaStock);
                    //WerehouseSummary
                    item_warehouse_material.Committed -= (double)item_detail.Qty;
                    item_warehouse_material.InStock -= (double)item_detail.Qty;
                    //Itemmasterdata
                    item_master_data.StockIn -= (double)item_detail.Qty;
                    _utility.UpdateItemAccounting(_itemAcc, item_warehouse_material);
                    if (item_detail.NegativeStock == true && nagative_check <= 0)
                    {
                        double @IssusQty;
                        double @FIFOQty;
                        double qty = item_detail.Qty;
                        var item_inventory_audit = new InventoryAudit();
                        var item_IssusStock = all_item_warehouse_detail.LastOrDefault(w => w.InStock <= 0);
                        @FIFOQty = item_IssusStock.InStock - qty;
                        @IssusQty = item_IssusStock.InStock - @FIFOQty;
                        if (item_detail.Process == "FIFO")
                        {
                            item_IssusStock.InStock = @FIFOQty;
                            if (@IssusQty > 0)
                            {
                                var stockOuts = new StockOut
                                {
                                    Cost = (decimal)item_IssusStock.Cost,
                                    CurrencyID = item_IssusStock.CurrencyID,
                                    ID = 0,
                                    InStock = (decimal)@IssusQty,
                                    ItemID = item_detail.ItemID,
                                    ProcessItem = ProcessItem.FIFO,
                                    SyetemDate = DateTime.Now,
                                    TimeIn = DateTime.Now,
                                    WarehouseID = item_IssusStock.WarehouseID,
                                    UomID = item_detail.UomID,
                                    UserID = crm.UserID,
                                    ExpireDate = item_IssusStock.ExpireDate,
                                    TransType = TransTypeWD.CanRing,
                                    TransID = crm.ID,
                                    OutStockFrom = crm.ID,
                                    BPID = crm.CusId,
                                    FromWareDetialID = item_IssusStock.ID
                                };
                                _context.StockOuts.Add(stockOuts);

                                var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item_detail.ItemID && w.WarehouseID == crm.WarehouseID).ToList();
                                item_inventory_audit.ID = 0;
                                item_inventory_audit.WarehouseID = crm.WarehouseID;
                                item_inventory_audit.BranchID = crm.BranchID;
                                item_inventory_audit.UserID = crm.UserID;
                                item_inventory_audit.ItemID = item_detail.ItemID;
                                item_inventory_audit.CurrencyID = Com.SystemCurrencyID;
                                item_inventory_audit.UomID = item_detail.UomID;
                                item_inventory_audit.InvoiceNo = crm.Number;
                                item_inventory_audit.Trans_Type = docType.Code;
                                item_inventory_audit.Process = item_detail.Process;
                                item_inventory_audit.SystemDate = DateTime.Now;
                                item_inventory_audit.TimeIn = DateTime.Now.ToShortTimeString();
                                item_inventory_audit.Qty = @IssusQty * -1;
                                item_inventory_audit.Cost = item_IssusStock.Cost;
                                item_inventory_audit.Price = 0;
                                item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) - @IssusQty;
                                item_inventory_audit.CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) - (@IssusQty * item_IssusStock.Cost);
                                item_inventory_audit.Trans_Valuse = @IssusQty * item_IssusStock.Cost * (-1);
                                item_inventory_audit.ExpireDate = item_IssusStock.ExpireDate;
                                item_inventory_audit.LocalCurID = Com.LocalCurrencyID;
                                item_inventory_audit.LocalSetRate = Exr.SetRate;
                                item_inventory_audit.CompanyID = crm.CompanyID;
                                item_inventory_audit.DocumentTypeID = docType.ID;
                                item_inventory_audit.SeriesID = crm.SeriesID;
                                item_inventory_audit.SeriesDetailID = crm.SeriesDID;
                            }
                            _utility.CumulativeValue(item_inventory_audit.WarehouseID, item_inventory_audit.ItemID, item_inventory_audit.CumulativeValue, _itemAcc);
                        }
                        else if (item_detail.Process == "Average")
                        {
                            item_IssusStock.InStock = @FIFOQty;
                            if (@IssusQty > 0)
                            {
                                var warehouse_summary = _context.WarehouseSummary.FirstOrDefault(w => w.ItemID == item_detail.ItemID && w.WarehouseID == crm.WarehouseID);
                                double @sysAvCost = warehouse_summary.Cost;
                                var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item_detail.ItemID && w.WarehouseID == crm.WarehouseID).ToList();

                                var stockOuts = new StockOut
                                {
                                    Cost = (decimal)@sysAvCost,
                                    CurrencyID = item_IssusStock.CurrencyID,
                                    ID = 0,
                                    InStock = (decimal)@IssusQty,
                                    ItemID = item_detail.ItemID,
                                    ProcessItem = ProcessItem.Average,
                                    SyetemDate = DateTime.Now,
                                    TimeIn = DateTime.Now,
                                    WarehouseID = item_IssusStock.WarehouseID,
                                    UomID = item_detail.UomID,
                                    UserID = crm.UserID,
                                    ExpireDate = item_IssusStock.ExpireDate,
                                    TransType = TransTypeWD.CanRing,
                                    TransID = crm.ID,
                                    OutStockFrom = crm.ID,
                                    BPID = crm.CusId,
                                    FromWareDetialID = item_IssusStock.ID
                                };
                                _context.StockOuts.Add(stockOuts);
                                item_inventory_audit.ID = 0;
                                item_inventory_audit.WarehouseID = crm.WarehouseID;
                                item_inventory_audit.BranchID = crm.BranchID;
                                item_inventory_audit.UserID = crm.UserID;
                                item_inventory_audit.ItemID = item_detail.ItemID;
                                item_inventory_audit.CurrencyID = Com.SystemCurrencyID;
                                item_inventory_audit.UomID = item_detail.UomID;
                                item_inventory_audit.InvoiceNo = crm.Number;
                                item_inventory_audit.Trans_Type = docType.Code;
                                item_inventory_audit.Process = item_detail.Process;
                                item_inventory_audit.SystemDate = DateTime.Now;
                                item_inventory_audit.TimeIn = DateTime.Now.ToShortTimeString();
                                item_inventory_audit.Qty = @IssusQty * -1;
                                item_inventory_audit.Cost = @sysAvCost;
                                item_inventory_audit.Price = 0;
                                item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) - @IssusQty;
                                item_inventory_audit.CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) - (@IssusQty * @sysAvCost);
                                item_inventory_audit.Trans_Valuse = @IssusQty * @sysAvCost * (-1);
                                item_inventory_audit.ExpireDate = item_IssusStock.ExpireDate;
                                item_inventory_audit.LocalCurID = Com.LocalCurrencyID;
                                item_inventory_audit.LocalSetRate = Exr.SetRate;
                                item_inventory_audit.CompanyID = crm.CompanyID;
                                item_inventory_audit.DocumentTypeID = docType.ID;
                                item_inventory_audit.SeriesID = crm.SeriesID;
                                item_inventory_audit.SeriesDetailID = crm.SeriesDID;
                            }
                            _utility.UpdateAvgCost(item_detail.ItemID, crm.WarehouseID, item_master_data.GroupUomID, item_inventory_audit.Qty, item_inventory_audit.Cost);
                            _utility.UpdateBomCost(item_detail.ItemID, item_inventory_audit.Qty, item_inventory_audit.Cost);
                            _utility.CumulativeValue(item_inventory_audit.WarehouseID, item_inventory_audit.ItemID, item_inventory_audit.CumulativeValue, _itemAcc);
                        }
                        _context.WarehouseDetails.Update(item_IssusStock);
                        _context.InventoryAudits.Add(item_inventory_audit);
                        _context.SaveChanges();
                    }
                    else
                    {
                        double @Check_Stock;
                        double @Remain;
                        double @IssusQty;
                        double @FIFOQty;
                        double qty = item_detail.Qty;
                        foreach (var item_warehouse in all_item_warehouse_detail.Where(w => w.InStock > 0))
                        {
                            InventoryAudit item_inventory_audit = new();
                            var item_IssusStock = all_item_warehouse_detail.FirstOrDefault(w => w.InStock > 0);
                            @Check_Stock = item_warehouse.InStock - qty;
                            if (@Check_Stock < 0)
                            {
                                @Remain = (item_warehouse.InStock - qty) * (-1);
                                @IssusQty = qty - @Remain;
                                if (@Remain <= 0)
                                {
                                    qty = 0;
                                }
                                else
                                {
                                    qty = @Remain;
                                }
                                if (item_detail.Process == "FIFO")
                                {
                                    item_IssusStock.InStock = item_IssusStock.InStock -= @IssusQty;
                                    if (@IssusQty > 0)
                                    {
                                        var stockOuts = new StockOut
                                        {
                                            Cost = (decimal)item_IssusStock.Cost,
                                            CurrencyID = item_IssusStock.CurrencyID,
                                            ID = 0,
                                            InStock = (decimal)@IssusQty,
                                            ItemID = item_detail.ItemID,
                                            ProcessItem = ProcessItem.FIFO,
                                            SyetemDate = DateTime.Now,
                                            TimeIn = DateTime.Now,
                                            WarehouseID = item_IssusStock.WarehouseID,
                                            UomID = item_detail.UomID,
                                            UserID = crm.UserID,
                                            ExpireDate = item_IssusStock.ExpireDate,
                                            TransType = TransTypeWD.CanRing,
                                            TransID = crm.ID,
                                            OutStockFrom = crm.ID,
                                            BPID = crm.CusId,
                                            FromWareDetialID = item_IssusStock.ID
                                        };
                                        _context.StockOuts.Add(stockOuts);
                                        var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item_detail.ItemID && w.WarehouseID == crm.WarehouseID).ToList();
                                        item_inventory_audit.ID = 0;
                                        item_inventory_audit.WarehouseID = crm.WarehouseID;
                                        item_inventory_audit.BranchID = crm.BranchID;
                                        item_inventory_audit.UserID = crm.UserID;
                                        item_inventory_audit.ItemID = item_detail.ItemID;
                                        item_inventory_audit.CurrencyID = Com.SystemCurrencyID;
                                        item_inventory_audit.UomID = item_detail.UomID;
                                        item_inventory_audit.InvoiceNo = crm.Number;
                                        item_inventory_audit.Trans_Type = docType.Code;
                                        item_inventory_audit.Process = item_detail.Process;
                                        item_inventory_audit.SystemDate = DateTime.Now;
                                        item_inventory_audit.TimeIn = DateTime.Now.ToShortTimeString();
                                        item_inventory_audit.Qty = @IssusQty * -1;
                                        item_inventory_audit.Cost = item_IssusStock.Cost;
                                        item_inventory_audit.Price = 0;
                                        item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) + (@IssusQty * (-1));
                                        item_inventory_audit.CumulativeValue = (inventory_audit.Sum(s => s.Trans_Valuse)) + (@IssusQty * item_IssusStock.Cost) * (-1);
                                        item_inventory_audit.Trans_Valuse = (@IssusQty * item_IssusStock.Cost) * (-1);
                                        item_inventory_audit.ExpireDate = item_IssusStock.ExpireDate;
                                        item_inventory_audit.LocalCurID = Com.LocalCurrencyID;
                                        item_inventory_audit.LocalSetRate = Exr.SetRate;
                                        item_inventory_audit.CompanyID = crm.CompanyID;
                                        item_inventory_audit.DocumentTypeID = docType.ID;
                                        item_inventory_audit.SeriesID = crm.SeriesID;
                                        item_inventory_audit.SeriesDetailID = crm.SeriesDID;
                                    }
                                    _utility.CumulativeValue(item_inventory_audit.WarehouseID, item_inventory_audit.ItemID, item_inventory_audit.CumulativeValue, _itemAcc);
                                    _context.WarehouseDetails.Update(item_IssusStock);
                                    _context.InventoryAudits.Add(item_inventory_audit);
                                    _context.SaveChanges();
                                }
                                else if (item_detail.Process == "Average")
                                {
                                    item_IssusStock.InStock = item_IssusStock.InStock -= @IssusQty;
                                    if (@IssusQty > 0)
                                    {
                                        var warehouse_summary = _context.WarehouseSummary.FirstOrDefault(w => w.ItemID == item_detail.ItemID && w.WarehouseID == crm.WarehouseID);
                                        double @sysAvCost = warehouse_summary.Cost;
                                        var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item_detail.ItemID && w.WarehouseID == crm.WarehouseID).ToList();

                                        var stockOuts = new StockOut
                                        {
                                            Cost = (decimal)item_IssusStock.Cost,
                                            CurrencyID = item_IssusStock.CurrencyID,
                                            ID = 0,
                                            InStock = (decimal)@IssusQty,
                                            ItemID = item_detail.ItemID,
                                            ProcessItem = ProcessItem.FIFO,
                                            SyetemDate = DateTime.Now,
                                            TimeIn = DateTime.Now,
                                            WarehouseID = item_IssusStock.WarehouseID,
                                            UomID = item_detail.UomID,
                                            UserID = crm.UserID,
                                            ExpireDate = item_IssusStock.ExpireDate,
                                            TransType = TransTypeWD.CanRing,
                                            TransID = crm.ID,
                                            OutStockFrom = crm.ID,
                                            BPID = crm.CusId,
                                            FromWareDetialID = item_IssusStock.ID
                                        };
                                        _context.StockOuts.Add(stockOuts);
                                        item_inventory_audit.ID = 0;
                                        item_inventory_audit.WarehouseID = crm.WarehouseID;
                                        item_inventory_audit.BranchID = crm.BranchID;
                                        item_inventory_audit.UserID = crm.UserID;
                                        item_inventory_audit.ItemID = item_detail.ItemID;
                                        item_inventory_audit.CurrencyID = Com.SystemCurrencyID;
                                        item_inventory_audit.UomID = item_detail.UomID;
                                        item_inventory_audit.InvoiceNo = crm.Number;
                                        item_inventory_audit.Trans_Type = docType.Code;
                                        item_inventory_audit.Process = item_detail.Process;
                                        item_inventory_audit.SystemDate = DateTime.Now;
                                        item_inventory_audit.TimeIn = DateTime.Now.ToShortTimeString();
                                        item_inventory_audit.Qty = @IssusQty * -1;
                                        item_inventory_audit.Cost = @sysAvCost;
                                        item_inventory_audit.Price = 0;
                                        item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) + (@IssusQty * (-1));
                                        item_inventory_audit.CumulativeValue = (inventory_audit.Sum(s => s.Trans_Valuse)) + (@IssusQty * @sysAvCost) * (-1);
                                        item_inventory_audit.Trans_Valuse = (@IssusQty * @sysAvCost) * (-1);
                                        item_inventory_audit.ExpireDate = item_IssusStock.ExpireDate;
                                        item_inventory_audit.LocalCurID = Com.LocalCurrencyID;
                                        item_inventory_audit.LocalSetRate = Exr.SetRate;
                                        item_inventory_audit.CompanyID = crm.CompanyID;
                                        item_inventory_audit.DocumentTypeID = docType.ID;
                                        item_inventory_audit.SeriesID = crm.SeriesID;
                                        item_inventory_audit.SeriesDetailID = crm.SeriesDID;
                                        item_inventory_audit.DocumentTypeID = docType.ID;
                                        item_inventory_audit.SeriesID = crm.SeriesID;
                                        item_inventory_audit.SeriesDetailID = crm.SeriesDID;
                                    }
                                    _utility.UpdateAvgCost(item_detail.ItemID, crm.WarehouseID, item_master_data.GroupUomID, item_inventory_audit.Qty, item_inventory_audit.Cost);
                                    _utility.UpdateBomCost(item_detail.ItemID, item_inventory_audit.Qty, item_inventory_audit.Cost);
                                    _utility.CumulativeValue(item_inventory_audit.WarehouseID, item_inventory_audit.ItemID, item_inventory_audit.CumulativeValue, _itemAcc);
                                    _context.WarehouseDetails.Update(item_IssusStock);
                                    _context.InventoryAudits.Add(item_inventory_audit);
                                    _context.SaveChanges();
                                }
                            }
                            else
                            {
                                @FIFOQty = item_IssusStock.InStock - qty;
                                @IssusQty = item_IssusStock.InStock - @FIFOQty;
                                if (item_detail.Process == "FIFO")
                                {
                                    item_IssusStock.InStock = @FIFOQty;
                                    if (@IssusQty > 0)
                                    {

                                        var stockOuts = new StockOut
                                        {
                                            Cost = (decimal)item_IssusStock.Cost,
                                            CurrencyID = item_IssusStock.CurrencyID,
                                            ID = 0,
                                            InStock = (decimal)@IssusQty,
                                            ItemID = item_detail.ItemID,
                                            ProcessItem = ProcessItem.FIFO,
                                            SyetemDate = DateTime.Now,
                                            TimeIn = DateTime.Now,
                                            WarehouseID = item_IssusStock.WarehouseID,
                                            UomID = item_detail.UomID,
                                            UserID = crm.UserID,
                                            ExpireDate = item_IssusStock.ExpireDate,
                                            TransType = TransTypeWD.CanRing,
                                            TransID = crm.ID,
                                            OutStockFrom = crm.ID,
                                            BPID = crm.CusId,
                                            FromWareDetialID = item_IssusStock.ID
                                        };
                                        _context.StockOuts.Add(stockOuts);
                                        var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item_detail.ItemID && w.WarehouseID == crm.WarehouseID).ToList();
                                        item_inventory_audit.ID = 0;
                                        item_inventory_audit.WarehouseID = crm.WarehouseID;
                                        item_inventory_audit.BranchID = crm.BranchID;
                                        item_inventory_audit.UserID = crm.UserID;
                                        item_inventory_audit.ItemID = item_detail.ItemID;
                                        item_inventory_audit.CurrencyID = Com.SystemCurrencyID;
                                        item_inventory_audit.UomID = item_detail.UomID;
                                        item_inventory_audit.InvoiceNo = crm.Number;
                                        item_inventory_audit.Trans_Type = docType.Code;
                                        item_inventory_audit.Process = item_detail.Process;
                                        item_inventory_audit.SystemDate = DateTime.Now;
                                        item_inventory_audit.TimeIn = DateTime.Now.ToShortTimeString();
                                        item_inventory_audit.Qty = @IssusQty * -1;
                                        item_inventory_audit.Cost = item_IssusStock.Cost;
                                        item_inventory_audit.Price = 0;
                                        item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) - @IssusQty;
                                        item_inventory_audit.CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) - @IssusQty * item_IssusStock.Cost;
                                        item_inventory_audit.Trans_Valuse = @IssusQty * item_IssusStock.Cost * -1;
                                        item_inventory_audit.ExpireDate = item_IssusStock.ExpireDate;
                                        item_inventory_audit.LocalCurID = Com.LocalCurrencyID;
                                        item_inventory_audit.LocalSetRate = Exr.SetRate;
                                        item_inventory_audit.CompanyID = crm.CompanyID;
                                        item_inventory_audit.DocumentTypeID = docType.ID;
                                        item_inventory_audit.SeriesID = crm.SeriesID;
                                        item_inventory_audit.SeriesDetailID = crm.SeriesDID;
                                        item_inventory_audit.DocumentTypeID = docType.ID;
                                        item_inventory_audit.SeriesID = crm.SeriesID;
                                        item_inventory_audit.SeriesDetailID = crm.SeriesDID;
                                    }
                                    _utility.CumulativeValue(item_inventory_audit.WarehouseID, item_inventory_audit.ItemID, item_inventory_audit.CumulativeValue, _itemAcc);
                                    _context.WarehouseDetails.Update(item_IssusStock);
                                    _context.InventoryAudits.Add(item_inventory_audit);
                                    _context.SaveChanges();
                                }
                                else if (item_detail.Process == "Average")
                                {
                                    item_IssusStock.InStock = @FIFOQty;
                                    if (@IssusQty > 0)
                                    {
                                        var warehouse_summary = _context.WarehouseSummary.FirstOrDefault(w => w.ItemID == item_detail.ItemID && w.WarehouseID == crm.WarehouseID);
                                        double @sysAvCost = warehouse_summary.Cost;
                                        var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item_detail.ItemID && w.WarehouseID == crm.WarehouseID).ToList();

                                        var stockOuts = new StockOut
                                        {
                                            Cost = (decimal)item_IssusStock.Cost,
                                            CurrencyID = item_IssusStock.CurrencyID,
                                            ID = 0,
                                            InStock = (decimal)@IssusQty,
                                            ItemID = item_detail.ItemID,
                                            ProcessItem = ProcessItem.FIFO,
                                            SyetemDate = DateTime.Now,
                                            TimeIn = DateTime.Now,
                                            WarehouseID = item_IssusStock.WarehouseID,
                                            UomID = item_detail.UomID,
                                            UserID = crm.UserID,
                                            ExpireDate = item_IssusStock.ExpireDate,
                                            TransType = TransTypeWD.CanRing,
                                            TransID = crm.ID,
                                            OutStockFrom = crm.ID,
                                            BPID = crm.CusId,
                                            FromWareDetialID = item_IssusStock.ID
                                        };
                                        _context.StockOuts.Add(stockOuts);
                                        item_inventory_audit.ID = 0;
                                        item_inventory_audit.WarehouseID = crm.WarehouseID;
                                        item_inventory_audit.BranchID = crm.BranchID;
                                        item_inventory_audit.UserID = crm.UserID;
                                        item_inventory_audit.ItemID = item_detail.ItemID;
                                        item_inventory_audit.CurrencyID = Com.SystemCurrencyID;
                                        item_inventory_audit.UomID = item_detail.UomID;
                                        item_inventory_audit.InvoiceNo = crm.Number;
                                        item_inventory_audit.Trans_Type = docType.Code;
                                        item_inventory_audit.Process = item_detail.Process;
                                        item_inventory_audit.SystemDate = DateTime.Now;
                                        item_inventory_audit.TimeIn = DateTime.Now.ToShortTimeString();
                                        item_inventory_audit.Qty = @IssusQty * -1;
                                        item_inventory_audit.Cost = @sysAvCost;
                                        item_inventory_audit.Price = 0;
                                        item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) - @IssusQty;
                                        item_inventory_audit.CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) - (@IssusQty * @sysAvCost);
                                        item_inventory_audit.Trans_Valuse = @IssusQty * @sysAvCost * -1;
                                        item_inventory_audit.ExpireDate = item_IssusStock.ExpireDate;
                                        item_inventory_audit.LocalCurID = Com.LocalCurrencyID;
                                        item_inventory_audit.LocalSetRate = Exr.SetRate;
                                        item_inventory_audit.CompanyID = crm.CompanyID;
                                        item_inventory_audit.DocumentTypeID = docType.ID;
                                        item_inventory_audit.SeriesID = crm.SeriesID;
                                        item_inventory_audit.SeriesDetailID = crm.SeriesDID;
                                    }
                                    _utility.UpdateAvgCost(item_detail.ItemID, crm.WarehouseID, item_master_data.GroupUomID, item_inventory_audit.Qty, item_inventory_audit.Cost);
                                    _utility.UpdateBomCost(item_detail.ItemID, item_inventory_audit.Qty, item_inventory_audit.Cost);
                                    _utility.CumulativeValue(item_inventory_audit.WarehouseID, item_inventory_audit.ItemID, item_inventory_audit.CumulativeValue, _itemAcc);

                                    _context.WarehouseDetails.Update(item_IssusStock);
                                    _context.InventoryAudits.Add(item_inventory_audit);
                                    _context.SaveChanges();
                                }
                                all_item_warehouse_detail = new List<WarehouseDetail>();
                                break;
                            }
                        }
                    }
                    _context.WarehouseSummary.Update(item_warehouse_material);
                    _context.ItemMasterDatas.Update(item_master_data);
                    _context.SaveChanges();
                }
            }
            #endregion
            if (journalEntry.ID > 0)
            {
                journalEntry.TotalDebit = journalEntryDetail.Sum(s => s.Debit);
                journalEntry.TotalCredit = journalEntryDetail.Sum(s => s.Credit);
                _context.JournalEntries.Update(journalEntry);
            }
            _context.JournalEntryDetails.UpdateRange(journalEntryDetail);
            _context.AccountBalances.UpdateRange(accountBalance);
            _context.SaveChanges();
        }
        private void InsertFinancialCanRingInventory(
            int inventoryAccID, decimal inventoryAccAmount, List<JournalEntryDetail> journalEntryDetail, List<AccountBalance> accountBalance,
            JournalEntry journalEntry, DocumentType docType, DocumentType douTypeID, ExchangeCanRingMaster crm,
            string OffsetAcc, bool itemChange = true)
        {
            if (itemChange)
            {
                //inventoryAccID
                var glAccInvenfifo = _context.GLAccounts.FirstOrDefault(w => w.ID == inventoryAccID) ?? new GLAccount();
                if (glAccInvenfifo.ID > 0)
                {
                    var journalDetail = journalEntryDetail.FirstOrDefault(w => w.ItemID == glAccInvenfifo.ID) ?? new JournalEntryDetail();
                    if (journalDetail.ItemID > 0)
                    {
                        var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == inventoryAccID);
                        glAccInvenfifo.Balance += inventoryAccAmount;
                        //journalEntryDetail
                        journalDetail.Debit += inventoryAccAmount;
                        //accountBalance
                        accBalance.CumulativeBalance = glAccInvenfifo.Balance;
                        accBalance.Debit += inventoryAccAmount;
                    }
                    else
                    {
                        glAccInvenfifo.Balance += inventoryAccAmount;
                        journalEntryDetail.Add(new JournalEntryDetail
                        {
                            JEID = journalEntry.ID,
                            Type = Type.GLAcct,
                            ItemID = inventoryAccID,
                            Debit = inventoryAccAmount,
                        });
                        //
                        accountBalance.Add(new AccountBalance
                        {
                            PostingDate = crm.CreatedAt,
                            Origin = docType.ID,
                            OriginNo = crm.Number,
                            OffsetAccount = OffsetAcc,
                            Details = douTypeID.Name + "-" + glAccInvenfifo.Code,
                            CumulativeBalance = glAccInvenfifo.Balance,
                            Debit = inventoryAccAmount,
                            LocalSetRate = (decimal)crm.LocalSetRate,
                            GLAID = inventoryAccID,
                        });
                    }
                    _context.Update(glAccInvenfifo);
                }
            }
            else
            {
                //inventoryAccID
                var glAccInvenfifo = _context.GLAccounts.FirstOrDefault(w => w.ID == inventoryAccID) ?? new GLAccount();
                if (glAccInvenfifo.ID > 0)
                {
                    var journalDetail = journalEntryDetail.FirstOrDefault(w => w.ItemID == glAccInvenfifo.ID) ?? new JournalEntryDetail();
                    if (journalDetail.ItemID > 0)
                    {
                        var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == inventoryAccID);
                        glAccInvenfifo.Balance -= inventoryAccAmount;
                        //journalEntryDetail
                        journalDetail.Credit += inventoryAccAmount;
                        //accountBalance
                        accBalance.CumulativeBalance = glAccInvenfifo.Balance;
                        accBalance.Credit += inventoryAccAmount;
                    }
                    else
                    {
                        glAccInvenfifo.Balance -= inventoryAccAmount;
                        journalEntryDetail.Add(new JournalEntryDetail
                        {
                            JEID = journalEntry.ID,
                            Type = Type.GLAcct,
                            ItemID = inventoryAccID,
                            Credit = inventoryAccAmount,
                        });
                        //
                        accountBalance.Add(new AccountBalance
                        {
                            PostingDate = crm.CreatedAt,
                            Origin = docType.ID,
                            OriginNo = crm.Number,
                            OffsetAccount = OffsetAcc,
                            Details = douTypeID.Name + "-" + glAccInvenfifo.Code,
                            CumulativeBalance = glAccInvenfifo.Balance,
                            Credit = inventoryAccAmount,
                            LocalSetRate = (decimal)crm.LocalSetRate,
                            GLAID = inventoryAccID,
                        });
                    }
                    _context.Update(glAccInvenfifo);
                }
            }
            _context.SaveChanges();
        }

        public void CheckCanRingItemSerailOut<T, TD>(T sale, List<TD> detials, List<SerialNumber> serialNumbers, string saleID)
        {
            int wareId = (int)GetValue(sale, "WarehouseID");
            var whs = _context.Warehouses.Find(wareId) ?? new Warehouse();
            if (detials.Count > 0)
            {
                foreach (var sd in detials)
                {
                    int itemId = (int)GetValue(sd, "ItemID");
                    int uomId = (int)GetValue(sd, "UomID");
                    ItemMasterData item = _context.ItemMasterDatas.Find(itemId) ?? new ItemMasterData();
                    var uom = _context.GroupDUoMs.FirstOrDefault(w => w.GroupUoMID == item.GroupUomID && w.AltUOM == uomId);
                    decimal qty = (decimal)GetValue(sd, "Qty") * (decimal)uom.Factor;
                    if (item.ManItemBy == ManageItemBy.SerialNumbers)
                    {
                        var serialNumber = serialNumbers.FirstOrDefault(i => i.ItemID == itemId) ?? new SerialNumber();
                        decimal totalCreated = 0;
                        if (serialNumber.SerialNumberSelected != null)
                        {
                            if (serialNumber.SerialNumberSelected.SerialNumberSelectedDetails != null)
                            {
                                serialNumber.SerialNumberSelected.SerialNumberSelectedDetails = serialNumber.SerialNumberSelected.SerialNumberSelectedDetails.GroupBy(i => i.SerialNumber).Select(i => i.FirstOrDefault()).ToList();
                                totalCreated = serialNumber.SerialNumberSelected.SerialNumberSelectedDetails.Count;
                            }
                            if (serialNumber.SerialNumberUnselected != null)
                            {
                                if (serialNumber.SerialNumberUnselected.SerialNumberUnselectedDetials != null &&
                                    serialNumber.SerialNumberSelected.SerialNumberSelectedDetails != null)
                                {
                                    foreach (var i in serialNumber.SerialNumberUnselected.SerialNumberUnselectedDetials.ToList())
                                    {
                                        if (serialNumber.SerialNumberSelected.SerialNumberSelectedDetails.Any(j => j.SerialNumber == i.SerialNumber)) serialNumber.SerialNumberUnselected.SerialNumberUnselectedDetials.Remove(i);
                                    }
                                }
                            }
                        }
                        if (serialNumber.SerialNumberSelected == null)
                        {
                            serialNumbers.Add(new SerialNumber
                            {
                                ItemCode = item.Code,
                                ItemName = item.KhmerName,
                                ItemID = itemId,
                                UomID = uomId,
                                Direction = "Out",
                                OpenQty = qty - totalCreated,
                                Qty = qty,
                                TotalSelected = totalCreated,
                                WhsCode = whs.Code,
                                BpId = (int)GetValue(sale, "CusId"),
                                SaleID = (int)GetValue(sale, saleID),
                                SerialNumberSelected = new SerialNumberSelected(),
                                SerialNumberUnselected = new SerialNumberUnselected(),
                                WareId = wareId,
                            });
                        }
                        else
                        {
                            serialNumber.OpenQty = qty - totalCreated;
                            serialNumber.Qty = qty;
                            serialNumber.TotalSelected = totalCreated;
                        }
                    }

                }
            }
        }
        public void CheckCanRingItemBatchOut<T, TD>(T sale, List<TD> detials, List<BatchNo> batchNoes, string saleID)
        {
            int wareid = (int)GetValue(sale, "WarehouseID");
            var whs = _context.Warehouses.Find(wareid) ?? new Warehouse();
            if (detials.Count > 0)
            {
                foreach (var sd in detials)
                {
                    int itemId = (int)GetValue(sd, "ItemID");
                    int uomId = (int)GetValue(sd, "UomID");
                    ItemMasterData item = _context.ItemMasterDatas.Find(itemId) ?? new ItemMasterData();
                    var uom = _context.GroupDUoMs.FirstOrDefault(w => w.GroupUoMID == item.GroupUomID && w.AltUOM == uomId);
                    decimal qty = (decimal)GetValue(sd, "Qty");
                    if (item.ManItemBy == ManageItemBy.Batches &&
                        item.ManMethod == ManagementMethod.OnEveryTransation
                        )
                    {
                        var batchNo = batchNoes.FirstOrDefault(i => i.ItemID == itemId) ?? new BatchNo();
                        decimal totalCreated = 0;
                        decimal totalBatches = 0;
                        if (batchNo.BatchNoSelected != null)
                        {
                            if (batchNo.BatchNoSelected.BatchNoSelectedDetails != null)
                            {
                                batchNo.BatchNoSelected.BatchNoSelectedDetails = batchNo.BatchNoSelected.BatchNoSelectedDetails.GroupBy(i => i.BatchNo).Select(i => i.FirstOrDefault()).ToList();
                                totalCreated = batchNo.BatchNoSelected.BatchNoSelectedDetails.Sum(i => i.SelectedQty);
                                totalBatches = batchNo.BatchNoSelected.BatchNoSelectedDetails.Count;
                            }
                            if (batchNo.BatchNoUnselect != null)
                            {
                                if (batchNo.BatchNoUnselect.BatchNoUnselectDetails != null &&
                                    batchNo.BatchNoSelected.BatchNoSelectedDetails != null)
                                {
                                    foreach (var i in batchNo.BatchNoUnselect.BatchNoUnselectDetails.ToList())
                                    {
                                        if (batchNo.BatchNoSelected.BatchNoSelectedDetails
                                            .Any(j => j.BatchNo == i.BatchNo && j.SelectedQty == i.OrigialQty))
                                            batchNo.BatchNoUnselect.BatchNoUnselectDetails.Remove(i);
                                    }
                                }
                            }
                        }
                        if (batchNo.BatchNoUnselect == null)
                        {
                            batchNoes.Add(new BatchNo
                            {
                                ItemCode = item.Code,
                                ItemName = item.KhmerName,
                                ItemID = itemId,
                                UomID = uomId,
                                Direction = "Out",
                                TotalBatches = totalBatches,
                                TotalNeeded = qty - totalCreated,
                                Qty = qty,
                                TotalSelected = totalCreated,
                                WhsCode = whs.Code,
                                SaleID = (int)GetValue(sale, saleID),
                                BpId = (int)GetValue(sale, "CusId"),
                                BatchNoSelected = new BatchNoSelected(),
                                BatchNoUnselect = new BatchNoUnselect(),
                                WareId = wareid,
                            });
                        }
                        else
                        {
                            batchNo.TotalBatches = totalBatches;
                            batchNo.TotalNeeded = qty - totalCreated;
                            batchNo.Qty = qty;
                            batchNo.TotalSelected = totalCreated;
                        }
                    }

                }
            }
        }
        public void CheckCanRingItemSerailIn<T, TD>(T pur, List<TD> purd, List<SerialViewModelPurchase> serialViewModelPurchases)
        {
            List<SerialDetialViewModelPurchase> serialDetialViewModelPurchase = new();
            int docId = (int)GetValue(pur, "DocTypeID");
            int wareId = (int)GetValue(pur, "WarehouseID");
            string number = GetValue(pur, "Number").ToString();
            var docType = _context.DocumentTypes.Find(docId) ?? new DocumentType();
            var whs = _context.Warehouses.Find(wareId) ?? new Warehouse();
            if (purd.Count > 0)
            {
                foreach (var grd in purd)
                {
                    int itemId = (int)GetValue(grd, "ItemChangeID");
                    decimal _qty = (decimal)GetValue(grd, "ChangeQty");
                    int uomId = (int)GetValue(grd, "UomChangeID");
                    ItemMasterData item = _context.ItemMasterDatas.Find(itemId) ?? new ItemMasterData();
                    var uom = _context.GroupDUoMs.FirstOrDefault(w => w.GroupUoMID == item.GroupUomID && w.AltUOM == uomId);
                    decimal qty = _qty * (decimal)uom.Factor;
                    var serial = serialViewModelPurchases.FirstOrDefault(i => i.ItemID == itemId) ?? new SerialViewModelPurchase();
                    if (item.ManItemBy == ManageItemBy.SerialNumbers &&
                        item.ManMethod == ManagementMethod.OnEveryTransation
                        )
                    {
                        if (serial.SerialDetialViewModelPurchase != null)
                        {
                            if (serial.SerialDetialViewModelPurchase.Count > qty)
                            {
                                serialDetialViewModelPurchase = serial.SerialDetialViewModelPurchase.Take((int)qty).ToList();
                            }
                            else if (serial.SerialDetialViewModelPurchase.Count == qty)
                            {
                                serialDetialViewModelPurchase = serial.SerialDetialViewModelPurchase;
                            }
                            else
                            {
                                serialDetialViewModelPurchase = serial.SerialDetialViewModelPurchase;
                                for (var i = 0; i < qty; i++)
                                {
                                    if (serial.SerialDetialViewModelPurchase.Count < i)
                                    {
                                        serialDetialViewModelPurchase.Add(new SerialDetialViewModelPurchase
                                        {
                                            AdmissionDate = DateTime.Now,
                                            Detials = "",
                                            ExpirationDate = null,
                                            LineID = DateTime.Now.Ticks.ToString(),
                                            Location = "",
                                            LotNumber = "",
                                            MfrDate = null,
                                            MfrSerialNo = "",
                                            MfrWarrantyEnd = null,
                                            MfrWarrantyStart = null,
                                            SerialNumber = "",
                                        });
                                    }
                                }
                            }
                        }
                        else
                        {
                            for (var i = 0; i < qty; i++)
                            {

                                serialDetialViewModelPurchase.Add(new SerialDetialViewModelPurchase
                                {
                                    AdmissionDate = DateTime.Now,
                                    Detials = "",
                                    ExpirationDate = null,
                                    LineID = DateTime.Now.Ticks.ToString(),
                                    Location = "",
                                    LotNumber = "",
                                    MfrDate = null,
                                    MfrSerialNo = "",
                                    MfrWarrantyEnd = null,
                                    MfrWarrantyStart = null,
                                    SerialNumber = "",
                                });
                            }
                        }
                        decimal totalCreated = serialDetialViewModelPurchase.Where(i => !string.IsNullOrEmpty(i.SerialNumber)).Count();
                        serialViewModelPurchases.Add(new SerialViewModelPurchase
                        {
                            DocNo = $"{docType.Code}-{number}",
                            ItemCode = item.Code,
                            ItemName = item.KhmerName,
                            ItemDescription = item.Description,
                            LineID = DateTime.Now.Ticks.ToString(),
                            OpenQty = qty - totalCreated,
                            TotalCreated = totalCreated,
                            ItemID = itemId,
                            AutomaticStringCreations = GetAutomaticStringCreation(serial.AutomaticStringCreations),
                            SerialAutomaticStringCreations = GetAutomaticStringCreation(serial.SerialAutomaticStringCreations),
                            LotAutomaticStringCreations = GetAutomaticStringCreation(serial.LotAutomaticStringCreations),
                            SerialDetailAutoCreation = GetSerialCanRingDetailAutoCreation(serialDetialViewModelPurchase.FirstOrDefault(), item, grd, whs, qty),
                            SerialDetialViewModelPurchase = serialDetialViewModelPurchase,
                            TotalNeeded = qty,
                            WhseCode = whs.Code,
                            WhseName = whs.Name
                        });
                        serialDetialViewModelPurchase = new List<SerialDetialViewModelPurchase>();
                    }

                }
            }

        }
        public void CheckCanRingItemBatchIn<T, TD>(T pur, List<TD> purd, List<BatchViewModelPurchase> batchViewModelPurchases)
        {
            List<BatchDetialViewModelPurchase> batchDetialViewModelPurchases = new();
            int docId = (int)GetValue(pur, "DocTypeID");
            int wareId = (int)GetValue(pur, "WarehouseID");
            string number = GetValue(pur, "Number").ToString();
            var docType = _context.DocumentTypes.Find(docId) ?? new DocumentType();
            var whs = _context.Warehouses.Find(wareId) ?? new Warehouse();
            if (purd.Count > 0)
            {
                foreach (var grd in purd)
                {
                    int itemId = (int)GetValue(grd, "ItemChangeID");
                    decimal _qty = (decimal)GetValue(grd, "ChangeQty");
                    int uomId = (int)GetValue(grd, "UomChangeID");
                    ItemMasterData item = _context.ItemMasterDatas.Find(itemId) ?? new ItemMasterData();
                    var uom = _context.GroupDUoMs.FirstOrDefault(w => w.GroupUoMID == item.GroupUomID && w.AltUOM == uomId);
                    decimal qty = _qty * (decimal)uom.Factor;
                    decimal purchasePrice = (decimal)GetValue(grd, "ChargePrice");
                    var batch = batchViewModelPurchases.FirstOrDefault(i => i.ItemID == itemId) ?? new BatchViewModelPurchase();
                    if (item.ManItemBy == ManageItemBy.Batches &&
                        item.ManMethod == ManagementMethod.OnEveryTransation
                        )
                    {
                        if (batch.BatchDetialViewModelPurchases != null)
                        {
                            batchDetialViewModelPurchases = batch.BatchDetialViewModelPurchases;
                        }
                        else
                        {
                            for (var i = 0; i < 10; i++)
                            {

                                batchDetialViewModelPurchases.Add(new BatchDetialViewModelPurchase
                                {
                                    AdmissionDate = DateTime.Now,
                                    Detials = "",
                                    ExpirationDate = null,
                                    LineID = DateTime.Now.Ticks.ToString(),
                                    Location = "",
                                    MfrDate = null,
                                    Batch = "",
                                    BatchAttribute1 = "",
                                    BatchAttribute2 = "",
                                    Qty = 0M,
                                    UnitPrice = purchasePrice,
                                });
                            }
                        }
                        decimal totalCreated = batchDetialViewModelPurchases.Where(i => !String.IsNullOrEmpty(i.Batch)).Sum(i => i.Qty);
                        batchViewModelPurchases.Add(new BatchViewModelPurchase
                        {
                            DocNo = $"{docType.Code}-{number}",
                            ItemCode = item.Code,
                            ItemName = item.KhmerName,
                            ItemDescription = item.Description,
                            LineID = DateTime.Now.Ticks.ToString(),
                            //OpenQty = (decimal)grd.OpenQty - totalCreated,
                            TotalCreated = totalCreated,
                            ItemID = itemId,
                            BatchStringCreations = GetAutomaticStringCreation(batch.BatchStringCreations),
                            Batchtrr1StringCreations = GetAutomaticStringCreation(batch.Batchtrr1StringCreations),
                            Batchtrr2StringCreations = GetAutomaticStringCreation(batch.Batchtrr2StringCreations),
                            BatchDetailAutoCreation = GetBatchCanRingDetailAutoCreation(batchDetialViewModelPurchases.FirstOrDefault(), item, grd, whs, qty),
                            BatchDetialViewModelPurchases = batchDetialViewModelPurchases,
                            TotalNeeded = qty,
                            WhseCode = whs.Code,
                            WhseName = whs.Name
                        });
                        batchDetialViewModelPurchases = new List<BatchDetialViewModelPurchase>();
                    }

                }
            }
        }
        private static BatchDetailAutoCreation GetBatchCanRingDetailAutoCreation<T>(BatchDetialViewModelPurchase bdvmp, ItemMasterData item, T grp, Warehouse whs, decimal qty)
        {
            return new BatchDetailAutoCreation
            {
                AdmissionDate = bdvmp.AdmissionDate,
                Cost = (decimal)GetValue(grp, "ChargePrice"),
                ItemCode = item.Code,
                ItemName = item.KhmerName,
                Qty = qty,
                WhsCode = whs.Code,
                Detials = "",
                ExpDate = bdvmp.ExpirationDate,
                Location = "",
                MfrDate = bdvmp.MfrDate,
                SysNo = "",
                Batch = "",
                BatchAtrr1 = "",
                BatchAtrr2 = "",
                NoOfBatch = 1M,

            };
        }
        private static List<AutomaticStringCreation> GetAutomaticStringCreation(List<AutomaticStringCreation> stringCreation)
        {
            if (stringCreation != null)
            {
                return stringCreation;
            }
            else
            {
                var oas = EnumHelper.ToDictionary(typeof(OperationAutomaticStringCreation));
                var tasc = EnumHelper.ToDictionary(typeof(TypeAutomaticStringCreation));
                List<SelectListItem> oasitems = oas.Select(i => new SelectListItem
                {
                    Value = i.Key.ToString(),
                    Text = i.Value
                }).ToList();
                List<SelectListItem> tascitems = tasc.Select(i => new SelectListItem
                {
                    Value = i.Key.ToString(),
                    Text = i.Value
                }).ToList();
                List<AutomaticStringCreation> items = new();
                for (var i = 0; i < 2; i++)
                {
                    items.Add(new AutomaticStringCreation
                    {
                        Name = "",
                        Operation = oasitems,
                        Type = tascitems,
                        OperationInt = OperationAutomaticStringCreation.NoOperation,
                        TypeInt = TypeAutomaticStringCreation.String
                    });
                }
                return items;
            }
        }
        private static SerialDetailAutoCreation GetSerialCanRingDetailAutoCreation<T>(SerialDetialViewModelPurchase sdvmp, ItemMasterData item, T grp, Warehouse whs, decimal qty)
        {
            return new SerialDetailAutoCreation
            {
                AdmissionDate = sdvmp.AdmissionDate,
                Cost = (decimal)GetValue(grp, "ChargePrice"),
                ItemCode = item.Code,
                ItemName = item.KhmerName,
                Qty = qty,
                WhsCode = whs.Code,
                Detials = "",
                ExpDate = sdvmp.ExpirationDate,
                Location = "",
                LotNumber = sdvmp.LotNumber,
                MfrDate = sdvmp.MfrDate,
                MfrSerialNo = sdvmp.MfrSerialNo,
                MfrWanEndDate = sdvmp.MfrWarrantyEnd,
                MfrWanStartDate = sdvmp.MfrWarrantyStart,
                SerailNumber = sdvmp.SerialNumber,
                SysNo = "",

            };
        }
        void Validation(List<CanRing> data, ModelStateDictionary modelState)
        {
            if (data.Count > 0)
            {
                var existedCanRingSetup = _context.CanRings.AsNoTracking().ToList();
                foreach (var (item, index) in data.Select((item, index) => (item, index)))
                {
                    var existedItem = data.Where(i => i.Name == item.Name).ToList();
                    if (item.ID == 0)
                    {
                        if (existedItem.Count > 1)
                        {
                            modelState.AddModelError($"ItemName{index + 1}", $"At row {index + 1} Name \"{item.Name}\" has duplicate name");
                        }
                        else if (existedCanRingSetup.Any(i => i.Name == item.Name))
                        {
                            modelState.AddModelError($"ItemName{index + 1}", $"At row {index + 1} Name \"{item.Name}\" has already existed");
                        }
                    }
                    else
                    {
                        var canringUpdate = existedCanRingSetup.FirstOrDefault(i => i.Name == item.Name && i.ID == item.ID);
                        if (existedCanRingSetup.Any(i => i.Name == item.Name) && canringUpdate == null)
                        {
                            modelState.AddModelError($"ItemName{index + 1}", $"At row {index + 1} Name \"{item.Name}\" has already existed");
                        }
                    }
                    if (item.ItemID == 0) modelState.AddModelError($"ItemName{index + 1}", $"At row {index + 1} require Item");
                    if (item.ItemChangeID == 0) modelState.AddModelError($"ItemChangeName{index + 1}", $"At row {index + 1} require Item Change");
                    if (item.Qty == 0) modelState.AddModelError($"Qty{index + 1}", $"At row {index + 1} require Qty");
                    if (item.ChangeQty == 0) modelState.AddModelError($"ChangeQty{index + 1}", $"At row {index + 1} require Qty Change");
                    if (item.UomID == 0) modelState.AddModelError($"UomID{index + 1}", $"At row {index + 1} require Uom");
                    if (item.UomChangeID == 0) modelState.AddModelError($"UomChangeID{index + 1}", $"At row {index + 1} require Uom Change");
                    if (item.PriceListID == 0) modelState.AddModelError($"PriceListID{index + 1}", $"At row {index + 1} require Price List");
                }
            }
        }
        private static object GetValue<T>(T obj, string prop)
        {
            var data = typeof(T).GetProperty(prop).GetValue(obj);
            return data ?? "";
        }
    }
}
