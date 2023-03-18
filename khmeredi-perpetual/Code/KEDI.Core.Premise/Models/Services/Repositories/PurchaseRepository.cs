using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KEDI.Core.Premise.Models.Sale;
using CKBS.AppContext;
using Microsoft.EntityFrameworkCore;
using CKBS.Models.Services.Banking;
using KEDI.Core.Premise.Models.ServicesClass.Purchase;
using CKBS.Models.ServicesClass.GoodsIssue;
using KEDI.Core.Premise.Models.ServicesClass.TaxGroup;
using KEDI.Core.Premise.Models.Services.Banking;
using Microsoft.AspNetCore.Mvc.Rendering;
using CKBS.Models.Services.Administrator.General;
using CKBS.Models.Services.Purchase;
using CKBS.Models.Services.HumanResources;
using CKBS.Models.Services.Inventory;
using CKBS.Models.Services.Administrator.Inventory;
using CKBS.Models.Services.Administrator.SystemInitialization;
using KEDI.Core.Helpers.Enumerations;
using KEDI.Core.Premise.Models.Services.Purchase;
using KEDI.Core.Premise.Models.ServicesClass.SerialBatches.Purchases;
using CKBS.Models.Services.Inventory.Transaction;
using KEDI.Core.Premise.Repository;
using Newtonsoft.Json;

namespace KEDI.Core.Premise.Models.Services.Responsitory
{
    public interface IPurchaseRepository
    {
        Task<IEnumerable<TpExchange>> GetExchange();
        Task<IEnumerable<ExchangeRate>> GetExchangeRates(int id);
        Task<List<PurchaseMasterViewModel>> GetItemMasterDataAsync(int wareid, int comId);
        Task<List<PurchaseMasterViewModel>> GetItemMasterDataPRAsync(int comId);
        Task<List<PurchaseMasterViewModel>> GetItemMasterDataCMAsync(int wareid, int comId);
        PurchaseOrderDetialViewModel GetItemDetails(Company company, int itemId = 0, int curId = 0, string barcode = "", PurCopyType? purCopyType = PurCopyType.None);

        List<PurchaseOrderDetialViewModel> GetItemDetailsCM(Company company, string process, int itemId = 0, int curId = 0, string barcode = "");
        Task<FreightPurchaseViewModel> GetFreightsAsync();
        Task<List<BusinessPartner>> GetBusinessPartnersAsync();
        Task<dynamic> GetCurrencyDefualtAsync();
        Task<List<Warehouse>> GetWarehousesAsync(int id);
        Task<dynamic> GetcurrencyAsync(int sysCurId);
        BusinessPartner Getbp(int id);
        Task<List<PurchaseViewModel>> GetAllPurPOAsync<T>(List<T> purchase);
        Task<List<PurchaseViewModel>> GetAllAPreserveOAsync(List<PurchaseAPReserve> purchase);
        void GetStringCreation(List<SerialDetialViewModelPurchase> serialDetialViewModelPurchase, SerialDetailAutoCreation serialDetailAuto);
        void GetStringCreation(List<BatchDetialViewModelPurchase> batchDetialViewModelPurchase, BatchDetailAutoCreation BatchDetailAuto);
        void CheckItemSerail<T, TD>(T pur, List<TD> purd, List<SerialViewModelPurchase> serialViewModelPurchases);
        void CheckItemBatch<T, TD>(T pur, List<TD> purd, List<BatchViewModelPurchase> batchViewModelPurchases);
        void CheckCanRingItemSerail<T, TD>(T pur, List<TD> purd, List<SerialViewModelPurchase> serialViewModelPurchases);
        void CheckCanRingItemBatch<T, TD>(T pur, List<TD> purd, List<BatchViewModelPurchase> batchViewModelPurchases);

        void CheckItemSerailGoodsReceipt(GoodsReceipt grc, List<SerialViewModelPurchase> serialViewModelPurchases);
        void CheckItemBatchGoodsReceipt(GoodsReceipt grc, List<BatchViewModelPurchase> batchViewModelPurchases);
        //void CheckItemSerail(PurchaseCreditMemo purapc, List<APCSerialNumber> aPCSerialNumber);
        void CheckItemSerail<T, TD>(T purapc, List<TD> detials, List<APCSerialNumber> aPCSerialNumbers, ExchangeRate ex);
        void CheckItemBatch<T, TD>(T purapc, List<TD> detials, List<APCBatchNo> aPCBatchNos, ExchangeRate ex);
        Task<PurchaseOrderUpdateViewModel> FindPurchaseOrderAsync(int seriesID, string number, int comId);
        Task<PurchasePOUpdateViewModel> FindPurchasePOAsync(int seriesID, string number, int comId);
        Task<PurchaseOrderUpdateViewModel> CopyPurchaseOrderAsync(int seriesID, string number, int comId);
        Task<PurchaseAPUpdateViewModel> CopyPurchaseOrder(int seriesID, string number, int comId);
        Task<PurchaseOrderUpdateViewModel> CopyPurchaseRequestAsync(int seriesID, string number, int comId);
        Task<PurchasePOUpdateViewModel> CopyPurchasePOAsync(int seriesID, string number, int comId);
        Task<PurchaseAPUpdateViewModel> FindPurchaseAPAsync(int seriesID, string number, int comId);
        Task<PurchaseAPUpdateViewModel> CopyPurchaseQuote(int seriesID, string number, int comId);
        Task<PurchaseAPUpdateViewModel> CopyPurchaseAPAsync(int seriesID, string number, int comId);
        Task<PurchaseOrderUpdateViewModel> FindAPresverAsync(int seriesID, string number, int comId);
        Task<PurchaseOrderUpdateViewModel> CopyPurchaseQuotationAsync(int seriesID, string number, int comId);
        Task<PurchaseAPUpdateViewModel> FindPurchaseAPReserveAsync(int seriesID, string number, int comId);
        Task<PurchaseAPUpdateViewModel> CopyPurchaseAPReserveAsync(int seriesID, string number, int comId);
        Task<PurchaseAPUpdateViewModel> FindPurchaseQuote(int seriesID, string number, int comId);
        Task<PurchaseAPUpdateViewModel> FindPurchaseOrder(int seriesID, string number, int comId);
        Task<PurchaseCreditMemoUpdateViewModel> FindPurchaseCreditMemoAsync(int seriesID, string number, int comId);
        Task<PurchaseOrderUpdateViewModel> FindPurchaseRequestAsync(int seriesID, string number, int comId);
        Task<PurchaseOrderUpdateViewModel> FindPurchaseQuotationAsync(int seriesID, string number, int comId);
        Task<APCBatchNoUnselect> GetBatchNoDetialsAsync(decimal cost, int bpId, int itemId, int uomID);
        Task<APCSerialNumberDetial> GetSerialDetialsAsync(decimal cost, int bpId, int itemId, int baseOnID, int copyTuype, string apsds, bool isAll = false);
        Task<DraftUpdateViewModel> FindDraftAsync(string draftname, int draftId, int comId);
        Task<int> RemoveDraft(int id);

    }
    public class PurchaseRepository : IPurchaseRepository
    {
        private readonly DataContext _context;
        private readonly IDataPropertyRepository _dataProp;
        public PurchaseRepository(DataContext context, IDataPropertyRepository dataProperty)
        {
            _context = context;
            _dataProp = dataProperty;
        }
        public async Task<DraftUpdateViewModel> FindDraftAsync(string draftname, int draftId, int comId)
        {
            #region
            var taxGroup = GetTaxGroups();
            taxGroup.Insert(0, new TaxGroupViewModel
            {
                ID = 0,
                Name = "-- Select --",
                Code = "",
                Rate = 0,
                Type = 0,
            });
            var test = _context.DraftAPs.Where(s => s.DraftName == draftname && s.CompanyID == comId && s.DraftID == draftId);
            var test1 = _context.FreightPurchases.Where(s => s.PurID == test.FirstOrDefault().DraftID && s.PurType == PurCopyType.PurchaseDraft).FirstOrDefault();
            var purDF = await (from df in _context.DraftAPs.Where(s => s.DraftName == draftname && s.CompanyID == comId && s.DraftID == draftId)
                               join docType in _context.DocumentTypes on df.DocumentTypeID equals docType.ID
                               let fs = _context.FreightPurchases.Where(i => i.PurID == df.DraftID && i.PurType == PurCopyType.PurchaseDraft).FirstOrDefault() ?? new FreightPurchase()
                               select new DraftViewModel
                               {
                                   FrieghtAmount = df.FrieghtAmount,
                                   FrieghtAmountSys = df.FrieghtAmountSys,
                                   BranchID = df.BranchID,
                                   CompanyID = df.CompanyID,
                                   DeliveryDate = df.DueDate,
                                   DueDate = df.DueDate,
                                   DocumentDate = df.DocumentDate,
                                   FreightPurchaseView = new FreightPurchaseViewModel
                                   {
                                       ExpenceAmount = fs.ExpenceAmount,
                                       PurID = df.DraftID,
                                       ID = fs.ID,
                                       PurType = fs.PurType,
                                       TaxSumValue = fs.TaxSumValue,
                                       FreightPurchaseDetailViewModels = (from fsd in _context.FreightPurchaseDetails.Where(i => i.FreightPurchaseID == fs.ID)
                                                                          select new FreightPurchaseDetailViewModel
                                                                          {
                                                                              ID = fsd.ID,
                                                                              FreightPurchaseID = fsd.FreightPurchaseID,
                                                                              Amount = fsd.Amount,
                                                                              AmountWithTax = fsd.AmountWithTax,
                                                                              FreightID = fsd.FreightID,
                                                                              Name = fsd.Name,
                                                                              TaxGroup = fsd.TaxGroup,
                                                                              TaxGroupID = fsd.TaxGroupID,
                                                                              TaxGroups = GetTaxGroups(),
                                                                              TaxGroupSelect = taxGroup.Select(i => new SelectListItem
                                                                              {
                                                                                  Value = i.ID.ToString(),
                                                                                  Selected = fsd.TaxGroupID == i.ID,
                                                                                  Text = $"{i.Code}-{i.Name}"
                                                                              }).ToList(),
                                                                              TaxRate = fsd.TaxRate,
                                                                              TotalTaxAmount = fsd.TotalTaxAmount
                                                                          }).ToList(),
                                   },
                                   InvoiceNo = $"{docType.Code}-{df.Number}",
                                   LocalCurID = df.LocalCurID,
                                   LocalSetRate = (decimal)df.LocalSetRate,
                                   PostingDate = df.PostingDate,
                                   Status = df.Status,
                                   SubTotalAfterDis = df.SubTotalAfterDis,
                                   SubTotalAfterDisSys = df.SubTotalAfterDisSys,
                                   TypeDis = df.TypeDis,
                                   UserID = df.UserID,
                                   WarehouseID = df.WarehouseID,
                                   DraftID = df.DraftID,
                                   BaseOnID = df.DraftID,
                                   AdditionalExpense = (decimal)df.AdditionalExpense,
                                   AdditionalNote = df.AdditionalNote,
                                   AppliedAmount = (decimal)df.AppliedAmount,
                                   AppliedAmountSys = (decimal)df.AppliedAmountSys,
                                   BalanceDue = (decimal)df.BalanceDue,
                                   BalanceDueSys = (decimal)df.BalanceDueSys,
                                   DiscountRate = (decimal)df.DiscountRate,
                                   DiscountValue = (decimal)df.DiscountValue,
                                   DocumentTypeID = df.DocumentTypeID,
                                   DownPayment = (decimal)df.DownPayment,
                                   DownPaymentSys = (decimal)df.DownPaymentSys,
                                   Number = df.Number,
                                   PurCurrencyID = df.PurCurrencyID,
                                   PurRate = (decimal)df.PurRate,
                                   ReffNo = df.ReffNo,
                                   Remark = df.Remark,
                                   ReturnAmount = (decimal)df.ReturnAmount,
                                   SeriesDetailID = df.SeriesDetailID,
                                   SeriesID = df.SeriesID,
                                   SubTotal = (decimal)df.SubTotal,
                                   SubTotalSys = (decimal)df.SubTotalSys,
                                   SysCurrencyID = df.SysCurrencyID,
                                   TaxRate = (decimal)df.TaxRate,
                                   TaxValue = (decimal)df.TaxValue,
                                   VendorID = df.VendorID
                               }).ToListAsync();
            var _tg = new TaxGroupViewModel
            {
                ID = 0,
                Name = "- Select --",
                Code = "",
                Rate = 0,
                Type = 0,
            };
            var tgs = (from t in _context.TaxGroups.Where(i => !i.Delete && i.Active && i.Type == TypeTax.InputTax)
                       let tgds = _context.TaxGroupDefinitions.Where(i => i.TaxGroupID == t.ID).OrderByDescending(i => i.EffectiveFrom).FirstOrDefault() ?? new TaxGroupDefinition()
                       select new TaxGroupViewModel
                       {
                           ID = t.ID,
                           Name = t.Name,
                           Code = t.Code,
                           Effectivefrom = tgds.EffectiveFrom,
                           Rate = tgds.Rate,
                           Type = (int)t.Type,
                       }).ToList();
            tgs.Insert(0, _tg);
            #endregion
            var __DFd = (from df in purDF
                         join dfd in _context.DraftAPDetails on df.DraftID equals dfd.DraftID
                         join item in _context.ItemMasterDatas on dfd.ItemID equals item.ID
                         let cur = _context.Currency.FirstOrDefault(i => i.ID == df.PurCurrencyID) ?? new Currency()
                         select new DraftDetailViewModel
                         {
                             LineIDUN = DateTime.Now.Ticks.ToString(),
                             LineID = dfd.DraftDetailID,
                             ItemID = item.ID,
                             Process = item.Process,
                             Qty = (decimal)dfd.Qty,
                             OpenQty = (decimal)dfd.OpenQty,
                             TypeDis = dfd.TypeDis,
                             UomID = dfd.UomID,
                             TaxGroupID = dfd.TaxGroupID,
                             TaxRate = dfd.TaxRate,
                             TaxGroupSelect = tgs.Select(c => new SelectListItem
                             {
                                 Value = c.ID.ToString(),
                                 Text = $"{c.Code}-{c.Name}",
                                 Selected = c.ID == dfd.TaxGroupID
                             }).ToList(),
                             TotalWTax = (decimal)dfd.TotalWTax,
                             TaxValue = dfd.TaxValue,
                             Total = (decimal)dfd.Total,
                             Remark = dfd.Remark,
                             /// select List UoM ///
                             UoMSelect = (from GDU in _context.GroupDUoMs.Where(i => i.GroupUoMID == item.GroupUomID)
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
                                              Selected = c.ID == dfd.UomID
                                          }).ToList(),
                             /// List UoM ///
                             UoMsList = (from GDU in _context.GroupDUoMs.Where(i => i.GroupUoMID == item.GroupUomID)
                                         join UNM in _context.UnitofMeasures on GDU.AltUOM equals UNM.ID
                                         select new UOMSViewModel
                                         {
                                             BaseUoMID = GDU.BaseUOM,
                                             Factor = GDU.Factor,
                                             ID = UNM.ID,
                                             Name = UNM.Name
                                         }).ToList(),
                             TaxGroups = (from t in _context.TaxGroups.Where(i => !i.Delete && i.Active && i.Type == TypeTax.InputTax)
                                          let tgds = _context.TaxGroupDefinitions.Where(i => i.TaxGroupID == t.ID).OrderByDescending(i => i.EffectiveFrom).FirstOrDefault() ?? new TaxGroupDefinition()
                                          select new TaxGroupViewModel
                                          {
                                              ID = t.ID,
                                              Name = t.Name,
                                              Code = t.Code,
                                              Effectivefrom = tgds.EffectiveFrom,
                                              Rate = tgds.Rate,
                                              Type = (int)t.Type,
                                          }).ToList(),
                             FinDisRate = dfd.FinDisRate,
                             FinDisValue = dfd.FinDisValue,
                             FinTotalValue = dfd.FinTotalValue,
                             TaxOfFinDisValue = dfd.TaxOfFinDisValue,
                             Barcode = item.Barcode,
                             Code = item.Code,
                             CurrencyName = cur.Description,
                             DiscountRate = (decimal)dfd.DiscountRate,
                             AlertStock = (decimal)dfd.AlertStock,
                             Delete = dfd.Delete,
                             DiscountValue = (decimal)dfd.DiscountValue,
                             ExpireDate = dfd.ExpireDate,
                             ItemName = item.KhmerName,
                             LocalCurrencyID = dfd.LocalCurrencyID,
                             DraftDetailID = dfd.DraftDetailID,
                             DraftID = dfd.DraftID,
                             PurchasPrice = (decimal)dfd.PurchasPrice,
                             TotalWTaxSys = (decimal)dfd.TotalWTaxSys,
                             TotalSys = (decimal)dfd.TotalSys,
                         }).GroupBy(i => i.ItemID).Select(i => i.FirstOrDefault()).ToList();
            _dataProp.DataProperty(__DFd, comId, "ItemID", "AddictionProps");
            var data = new DraftUpdateViewModel
            {
                Draft = purDF.FirstOrDefault(),
                DraftDetails = __DFd
            };
            return await Task.FromResult(data);
        }
        public async Task<int> RemoveDraft(int id)
        {
            var tab = await _context.DraftAPs.FirstAsync(t => t.DraftID == id);
            tab.Remove = true;
            _context.DraftAPs.Update(tab);
            return await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<TpExchange>> GetExchange()
        {
            var ex = await (from e in _context.ExchangeRates.Where(x => x.Delete == false)
                            join c in _context.Currency.Where(x => x.Delete == false) on e.CurrencyID equals c.ID
                            select new TpExchange
                            {
                                ID = e.ID,
                                CurID = e.CurrencyID,
                                CurName = c.Description,
                                Rate = e.Rate,
                                SetRate = e.SetRate
                            }).ToListAsync();
            return ex;
        }
        //public async Task<IEnumerable<>> GetCurrency(){

        //}
        public async Task<IEnumerable<ExchangeRate>> GetExchangeRates(int id)
        {
            IEnumerable<ExchangeRate> list = await (from ex in _context.ExchangeRates.Where(x => x.Delete == false)
                                                    join cur in _context.Currency.Where(x => x.Delete == false) on ex.CurrencyID equals cur.ID
                                                    where cur.ID == id
                                                    select new ExchangeRate
                                                    {
                                                        Rate = ex.Rate,
                                                        ID = ex.ID,
                                                        CurrencyID = ex.CurrencyID,
                                                        SetRate = ex.SetRate,
                                                        Currency = new Currency
                                                        {
                                                            Description = cur.Description
                                                        }
                                                    }).ToListAsync();
            return list;
        }
        public async Task<List<PurchaseMasterViewModel>> GetItemMasterDataAsync(int wareid, int comId)
        {
            var _data = await (from _wd in _context.WarehouseDetails
                               join _item in _context.ItemMasterDatas on _wd.ItemID equals _item.ID
                               join _uom in _context.UnitofMeasures on _wd.UomID equals _uom.ID
                               join _cur in _context.Currency on _wd.CurrencyID equals _cur.ID
                               join _pricelD in _context.PriceListDetails on _item.ID equals _pricelD.ItemID
                               where !_item.Delete && _wd.WarehouseID == wareid && _item.Process != "Standard" &&
                                 _item.Inventory && _item.Purchase && _uom.ID == _item.PurchaseUomID
                               group new { _wd, _item, _uom, _cur, _pricelD } by new { ItemID = _item.ID } into g
                               let data = g/*.Where(i=> i._wd.InStock > 0)*/.FirstOrDefault()
                               let item = data._item
                               let uom = data._uom
                               let cur = data._cur
                               let prld = data._pricelD
                               select new PurchaseMasterViewModel
                               {
                                   ID = item.ID,
                                   Barcode = item.Barcode,
                                   Code = item.Code,
                                   ItemName1 = item.KhmerName,
                                   Cost = (decimal)prld.Cost,
                                   Currency = cur.Description,
                                   ItemName2 = item.EnglishName,
                                   UomName = uom.Name,
                                   Process = item.Process,
                                   InStock = (decimal)data._wd.InStock,
                               }).OrderBy(i => i.Code).ToListAsync();
            _dataProp.DataProperty(_data, comId, "ID", "AddictionProps");
            return _data;
        }

        public async Task<List<PurchaseMasterViewModel>> GetItemMasterDataPRAsync(int comId)
        {
            var list = await _context.ItemMasterDatas.Where(i => !i.Delete && i.Process != "Standard").Include(i => i.UnitofMeasurePur).Select(i => new PurchaseMasterViewModel
            {
                ID = i.ID,
                Barcode = i.Barcode,
                Code = i.Code,
                ItemName1 = i.KhmerName,
                ItemName2 = i.EnglishName,
                UomName = i.UnitofMeasurePur.Name,
                Process = i.Process,
            }).OrderBy(o => o.Code).ToListAsync();
            _dataProp.DataProperty(list, comId, "ID", "AddictionProps");
            return list;
        }
        public async Task<List<PurchaseMasterViewModel>> GetItemMasterDataCMAsync(int wareid, int comId)
        {
            var _data = await (from _wd in _context.WarehouseDetails.Where(i => i.InStock > 0)
                               join _item in _context.ItemMasterDatas on _wd.ItemID equals _item.ID
                               join _uom in _context.UnitofMeasures on _wd.UomID equals _uom.ID
                               join _cur in _context.Currency on _wd.CurrencyID equals _cur.ID
                               where !_item.Delete && _wd.WarehouseID == wareid && _item.Process != "Standard" &&
                                 _item.Inventory && _item.Purchase && _uom.ID == _item.PurchaseUomID
                               group new { _wd, _item, _uom, _cur } by new { ItemID = _item.ID } into g
                               let data = g.FirstOrDefault()
                               let item = data._item
                               let uom = data._uom
                               let cur = data._cur
                               select new PurchaseMasterViewModel
                               {
                                   ID = item.ID,
                                   Barcode = item.Barcode,
                                   Code = item.Code,
                                   ItemName1 = item.KhmerName,
                                   Cost = (decimal)item.Cost,
                                   Currency = cur.Description,
                                   ItemName2 = item.EnglishName,
                                   UomName = uom.Name,
                                   Process = item.Process,
                                   InStock = (decimal)data._wd.InStock,
                               }).OrderBy(i => i.Code).ToListAsync();
            _dataProp.DataProperty(_data, comId, "ID", "AddictionProps");
            return _data;
        }
        private List<TaxGroupViewModel> GetTaxGroups()
        {
            var tgs = (from t in _context.TaxGroups.Where(i => !i.Delete && i.Active && i.Type == TypeTax.InputTax)
                       let tgds = _context.TaxGroupDefinitions.Where(i => i.TaxGroupID == t.ID).OrderByDescending(i => i.EffectiveFrom).FirstOrDefault() ?? new TaxGroupDefinition()
                       select new TaxGroupViewModel
                       {
                           ID = t.ID,
                           Name = t.Name,
                           Code = t.Code,
                           Effectivefrom = tgds.EffectiveFrom,
                           Rate = tgds.Rate,
                           Type = (int)t.Type,
                       }).ToList();
            return tgs;
        }
        public PurchaseOrderDetialViewModel GetItemDetails(Company company, int itemId = 0, int curId = 0, string barcode = "", PurCopyType? purCopyType = PurCopyType.None)
        {
            var localCur = _context.Currency.Find(company.LocalCurrencyID) ?? new Currency();
            var _tg = new TaxGroupViewModel
            {
                ID = 0,
                Name = "- Select --",
                Code = "",
                Rate = 0,
                Type = 0,
            };
            ItemMasterData _item = new();
            if (itemId != 0)
            {
                _item = _context.ItemMasterDatas.Find(itemId);
            }
            if (barcode != null)
            {
                _item = _context.ItemMasterDatas.FirstOrDefault(i => i.Barcode == barcode) ?? new ItemMasterData();
            }
            var tgs = GetTaxGroups();
            tgs.Insert(0, _tg);
            var uoms = (from guom in _context.ItemMasterDatas.Where(i => i.ID == _item.ID)
                        join GDU in _context.GroupDUoMs on guom.GroupUomID equals GDU.GroupUoMID
                        join UNM in _context.UnitofMeasures on GDU.AltUOM equals UNM.ID
                        select new UOMSViewModel
                        {
                            BaseUoMID = GDU.BaseUOM,
                            Factor = GDU.Factor,
                            ID = UNM.ID,
                            Name = UNM.Name
                        }).ToList();
            var ex = _context.ExchangeRates.FirstOrDefault(i => i.CurrencyID == curId) ?? new ExchangeRate();
            var cur = _context.Currency.FirstOrDefault(i => i.ID == curId) ?? new Currency();
            var items = purCopyType == PurCopyType.PurRequest ? _context.ItemMasterDatas.Where(i => !i.Delete && i.Purchase && i.Inventory && i.ID == _item.ID).ToList()
                                                         : _context.ItemMasterDatas.Where(i => !i.Delete && i.Purchase && i.Inventory && i.Process != "Starndard" && i.ID == _item.ID).ToList();
            var _data = (from item in items
                         join uom in _context.UnitofMeasures on item.PurchaseUomID equals uom.ID
                         join prilistD in _context.PriceListDetails on item.ID equals prilistD.ItemID
                         let minQty = Convert.ToDecimal(item.MinOrderQty)
                         let minOrderQty = item.IsLimitOrder ? minQty : 1M
                         select new PurchaseOrderDetialViewModel
                         {
                             AlertStock = 0,
                             Barcode = item.Barcode ?? "",
                             Code = item.Code,
                             CurrencyName = cur.Description,
                             Delete = false,
                             DiscountRate = 0M,
                             DiscountValue = 0M,
                             FinDisRate = 0M,
                             FinDisValue = 0M,
                             FinTotalValue = (decimal)item.Cost,
                             ItemID = item.ID,
                             RequiredDate = DateTime.Today,
                             ItemName = item.KhmerName,
                             LineIDUN = DateTime.Now.Ticks.ToString(),
                             LocalCurrencyID = localCur.ID,
                             OldQty = 0,
                             OpenQty = minOrderQty,
                             PurchaseOrderDetailID = 0,
                             PurchaseOrderID = 0,
                             PurchasPrice = (decimal)prilistD.Cost,// (decimal)item.Cost,
                             Qty = minOrderQty,
                             TaxGroupID = 0,
                             TaxGroupSelect = tgs.Select(c => new SelectListItem
                             {
                                 Value = c.ID.ToString(),
                                 Text = $"{c.Code}-{c.Name}",
                             }).ToList(),
                             TaxGroups = tgs,
                             TaxOfFinDisValue = 0,
                             TaxRate = 0M,
                             TaxValue = 0M,
                             Total = (decimal)item.Cost,
                             TotalWTax = (decimal)item.Cost,
                             TotalWTaxSys = (decimal)(item.Cost * ex.Rate),
                             TotalSys = (decimal)(item.Cost * ex.Rate),
                             TypeDis = "Percent",
                             UomID = (int)item.PurchaseUomID,
                             UoMSelect = uoms.Select(c => new SelectListItem
                             {
                                 Value = c.ID.ToString(),
                                 Text = c.Name,
                                 Selected = c.ID == item.PurchaseUomID
                             }).ToList(),
                             UoMsList = uoms.ToList(),
                             Process = item.Process,
                             IsLimitOrder = item.IsLimitOrder,
                             MinOrderQty = item.MinOrderQty,
                             MaxOrderQty = item.MaxOrderQty,
                         }).Take(1).ToList();//.GroupBy(i => i.ItemID).Select(i => i.FirstOrDefault()).ToList();
            _dataProp.DataProperty(_data, company.ID, "ItemID", "AddictionProps");
            return _data.FirstOrDefault();
        }
        public List<PurchaseOrderDetialViewModel> GetItemDetailsCM(Company company, string process, int itemId = 0, int curId = 0, string barcode = "")
        {
            var localCur = _context.Currency.Find(company.LocalCurrencyID) ?? new Currency();
            var _tg = new TaxGroupViewModel
            {
                ID = 0,
                Name = "- Select --",
                Code = "",
                Rate = 0,
                Type = 0,
            };
            ItemMasterData _item = new();
            if (itemId != 0)
            {
                _item = _context.ItemMasterDatas.Find(itemId);
            }
            if (barcode != null)
            {
                _item = _context.ItemMasterDatas.FirstOrDefault(i => i.Barcode == barcode) ?? new ItemMasterData();
            }
            var tgs = GetTaxGroups();
            tgs.Insert(0, _tg);
            var uoms = (from guom in _context.ItemMasterDatas.Where(i => i.ID == _item.ID)
                        join GDU in _context.GroupDUoMs on guom.GroupUomID equals GDU.GroupUoMID
                        join UNM in _context.UnitofMeasures on GDU.AltUOM equals UNM.ID
                        select new UOMSViewModel
                        {
                            BaseUoMID = GDU.BaseUOM,
                            Factor = GDU.Factor,
                            ID = UNM.ID,
                            Name = UNM.Name
                        }).ToList();
            var ex = _context.ExchangeRates.FirstOrDefault(i => i.CurrencyID == curId) ?? new ExchangeRate();
            var cur = _context.Currency.FirstOrDefault(i => i.ID == curId) ?? new Currency();
            List<PurchaseOrderDetialViewModel> items = new();
            var _data = (from item in _context.ItemMasterDatas.Where(i => !i.Delete && i.Purchase && i.Inventory && i.ID == _item.ID)
                         join wd in _context.WarehouseDetails.Where(i => i.InStock > 0) on item.ID equals wd.ItemID
                         join uom in _context.UnitofMeasures on item.PurchaseUomID equals uom.ID
                         let ws = _context.WarehouseSummary.Where(i => i.WarehouseID == wd.WarehouseID && i.ItemID == item.ID)
                         select new PurchaseOrderDetialViewModel
                         {
                             AlertStock = 0,
                             Barcode = item.Barcode ?? "",
                             Code = item.Code,
                             CurrencyName = cur.Description,
                             Delete = false,
                             DiscountRate = 0M,
                             DiscountValue = 0M,
                             FinDisRate = 0M,
                             FinDisValue = 0M,
                             FinTotalValue = (decimal)item.Cost,
                             ItemID = item.ID,
                             ItemName = item.KhmerName,
                             LineIDUN = $"{DateTime.Now.Ticks}{wd.Cost}",
                             LocalCurrencyID = localCur.ID,
                             OldQty = 0,
                             OpenQty = 1,
                             PurchaseOrderDetailID = 0,
                             PurchaseOrderID = 0,
                             Cost = (decimal)item.Cost,
                             PurchasPrice = (decimal)(wd.Cost * ex.SetRate),

                             Qty = 1,
                             TaxGroupID = 0,
                             TaxGroupSelect = tgs.Select(c => new SelectListItem
                             {
                                 Value = c.ID.ToString(),
                                 Text = $"{c.Code}-{c.Name}",
                                 //Selected = c.ID == tg.ID
                             }).ToList(),
                             TaxGroups = tgs,
                             TaxOfFinDisValue = 0,
                             TaxRate = 0M,
                             TaxValue = 0M,
                             Total = (decimal)(item.Cost * ex.SetRate),
                             TotalWTax = (decimal)(item.Cost * ex.SetRate),
                             TotalWTaxSys = (decimal)(item.Cost * ex.Rate),
                             TotalSys = (decimal)(item.Cost * ex.Rate),
                             TypeDis = "Percent",
                             UomID = (int)item.PurchaseUomID,
                             UoMSelect = uoms.Select(c => new SelectListItem
                             {
                                 Value = c.ID.ToString(),
                                 Text = c.Name,
                                 Selected = c.ID == item.PurchaseUomID
                             }).ToList(),
                             UoMsList = uoms.ToList(),
                             Process = item.Process,
                             Avgcost = (decimal)(ws.FirstOrDefault().Cost * ex.SetRate),
                         }).ToList();

            if (process == null) process = _item.Process;
            if (process == "FIFO" || process == "Average" || process == "SEBA")
            {
                var fifoitem = _data.Where(i => i.Process == "FIFO" || i.Process == "SEBA" && i.PurchasPrice > 0).GroupBy(i => i.PurchasPrice).Select(x => x.First()).ToList();
                var avgitem = _data.FirstOrDefault(i => i.Process == "Average");
                if (fifoitem.Count > 0)
                    items.AddRange(fifoitem);
                if (avgitem != null)
                {
                    avgitem.PurchasPrice = avgitem.Avgcost;
                    items.Add(avgitem);
                }
            }
            if (process == "Standard")
            {
                var stdItem = _data.GroupBy(i => i.ItemID).Select(i => i.FirstOrDefault()).ToList();
                items.AddRange(stdItem);
            }
            _dataProp.DataProperty(items, company.ID, "ItemID", "AddictionProps");
            return items;
        }
        public async Task<FreightPurchaseViewModel> GetFreightsAsync()
        {
            var taxGroup = GetTaxGroups();
            taxGroup.Insert(0, new TaxGroupViewModel
            {
                ID = 0,
                Name = "- Select --",
                Code = "",
                Rate = 0,
                Type = 0,
            });
            var freightDetails = await (from fre in _context.Freights
                                        select new FreightPurchaseDetailViewModel
                                        {
                                            Amount = 0,
                                            FreightID = fre.ID,
                                            ID = 0,
                                            FreightPurchaseID = 0,
                                            Name = fre.Name,
                                            TaxGroupSelect = taxGroup.Select(c => new SelectListItem
                                            {
                                                Value = c.ID.ToString(),
                                                Text = $"{c.Code}-{c.Name}",
                                            }).ToList(),
                                            TaxGroups = (from t in _context.TaxGroups.Where(i => !i.Delete && i.Active && i.Type == TypeTax.InputTax)
                                                         let tgds = _context.TaxGroupDefinitions.Where(i => i.TaxGroupID == t.ID).OrderByDescending(i => i.EffectiveFrom).FirstOrDefault() ?? new TaxGroupDefinition()
                                                         select new TaxGroupViewModel
                                                         {
                                                             ID = t.ID,
                                                             Name = t.Name,
                                                             Code = t.Code,
                                                             Effectivefrom = tgds.EffectiveFrom,
                                                             Rate = tgds.Rate,
                                                             Type = (int)t.Type,
                                                         }
                                               ).ToList(),
                                            TaxGroup = "",
                                            TaxGroupID = 0,
                                            TaxRate = 0,
                                            TotalTaxAmount = 0,
                                        }).ToListAsync();
            var freights = new FreightPurchaseViewModel
            {
                ExpenceAmount = 0,
                FreightPurchaseDetailViewModels = freightDetails,
                ID = 0,
                PurID = 0,
                PurType = PurCopyType.None,
                TaxSumValue = 0,
            };
            var data = await Task.FromResult(freights);
            return data;
        }
        public BusinessPartner Getbp(int id)
        {
            var data = _context.BusinessPartners.Find(id) ?? new BusinessPartner();
            return data;
        }
        public async Task<List<BusinessPartner>> GetBusinessPartnersAsync()
        {
            var data = await _context.BusinessPartners.Where(i => !i.Delete && i.Type == "Vendor").ToListAsync();
            return data;
        }
        public async Task<dynamic> GetCurrencyDefualtAsync()
        {
            var list = await (from com in _context.Company.Where(w => !w.Delete)
                              join cur in _context.Currency.Where(w => !w.Delete) on com.SystemCurrencyID equals cur.ID
                              let ex = _context.ExchangeRates.FirstOrDefault(i => i.CurrencyID == cur.ID)
                              select new
                              {
                                  ExchangeRate = ex.Rate,
                                  cur.Description
                              }).ToListAsync();
            return list;
        }
        public async Task<List<Warehouse>> GetWarehousesAsync(int id)
        {
            var list = await _context.Warehouses.Where(x => x.BranchID == id && !x.Delete).ToListAsync();
            return list;
        }
        public async Task<dynamic> GetcurrencyAsync(int sysCurId)
        {
            var sysCur = _context.Currency.FirstOrDefault(i => i.ID == sysCurId);
            var list = await (from c in _context.Currency.Where(i => !i.Delete)
                              select new
                              {
                                  c.ID,
                                  c.Symbol,
                                  c.Description,
                                  SysCur = c.ID == sysCur.ID,
                              }).ToListAsync();

            return list;
        }
        public async Task<List<PurchaseViewModel>> GetAllPurPOAsync<T>(List<T> purchase)
        {
            var data = (from p in purchase
                        let doc = _context.DocumentTypes.FirstOrDefault(i => (int)GetValue(p, "DocumentTypeID") == i.ID) ?? new DocumentType()
                        let cur = _context.Currency.FirstOrDefault(i => (int)GetValue(p, "PurCurrencyID") == i.ID) ?? new Currency()
                        select new PurchaseViewModel
                        {
                            LineID = DateTime.Now.Ticks.ToString(),
                            BalanceDue = GetValue(p, "BalanceDue").ToString(),
                            CurrencyName = cur.Description,
                            DocType = doc.Code,
                            Number = GetValue(p, "Number").ToString(),
                            Invoice = $"{doc.Code}-{GetValue(p, "Number")}",
                            PostingDate = ((DateTime)GetValue(p, "PostingDate")).ToString("MM-dd-yyyy"),
                            Remarks = GetValue(p, "Remark").ToString(),
                            SubTotal = GetValue(p, "SubTotal").ToString(),
                            SeriesID = (int)GetValue(p, "SeriesID"),
                        }).ToList();
            return await Task.FromResult(data);
        }

        // Get AP reserve Do by Mr Bunthorn
        public async Task<List<PurchaseViewModel>> GetAllAPreserveOAsync(List<PurchaseAPReserve> purchase)
        {
            var data = (from p in purchase
                        let doc = _context.DocumentTypes.FirstOrDefault(i => p.DocumentTypeID == i.ID) ?? new DocumentType()
                        let cur = _context.Currency.FirstOrDefault(i => p.PurCurrencyID == i.ID) ?? new Currency()
                        select new PurchaseViewModel
                        {
                            LineID = DateTime.Now.Ticks.ToString(),
                            BalanceDue = p.BalanceDue.ToString(),
                            CurrencyName = cur.Description,
                            DocType = doc.Code,
                            Number = p.Number.ToString(),
                            Invoice = $"{doc.Code}-{p.Number}",
                            PostingDate = (p.PostingDate).ToString("MM-dd-yyyy"),
                            Remarks = p.Remark.ToString(),
                            SubTotal = p.SubTotal.ToString(),
                            SeriesID = p.SeriesID,
                        }).ToList();
            return await Task.FromResult(data);
        }

        private static object GetValue<T>(T obj, string prop)
        {
            var data = typeof(T).GetProperty(prop).GetValue(obj);
            return data ?? "";
        }
        public void GetStringCreation(List<SerialDetialViewModelPurchase> serialDetialViewModelPurchase, SerialDetailAutoCreation serialDetailAuto)
        {
            if (serialDetailAuto.AutomaticStringCreations != null)
            {
                switch (serialDetailAuto.AutomaticStringCreations[0].TypeInt)
                {
                    case TypeAutomaticStringCreation.Number:
                        switch (serialDetailAuto.AutomaticStringCreations[0].OperationInt)
                        {
                            case OperationAutomaticStringCreation.Increase:
                                for (var i = 0; i < serialDetailAuto.Qty; i++)
                                {
                                    string mrfNo;
                                    if (i == 0)
                                    {
                                        mrfNo = $"{serialDetailAuto.AutomaticStringCreations[0].Name}{serialDetailAuto.AutomaticStringCreations[1].Name}";
                                    }
                                    else
                                    {
                                        mrfNo = $"{long.Parse(serialDetailAuto.AutomaticStringCreations[0].Name) + i}{serialDetailAuto.AutomaticStringCreations[1].Name}";
                                    }
                                    if (serialDetialViewModelPurchase.Count < serialDetailAuto.Qty)
                                    {
                                        serialDetialViewModelPurchase.Add(new SerialDetialViewModelPurchase
                                        {
                                            AdmissionDate = serialDetailAuto.AdmissionDate,
                                            Detials = serialDetailAuto.Detials,
                                            ExpirationDate = serialDetailAuto.ExpDate,
                                            LineID = DateTime.Now.Ticks.ToString(),
                                            Location = serialDetailAuto.Location,
                                            LotNumber = serialDetailAuto.LotNumber,
                                            MfrDate = serialDetailAuto.MfrDate,
                                            MfrSerialNo = mrfNo,
                                            MfrWarrantyEnd = serialDetailAuto.MfrWanEndDate,
                                            MfrWarrantyStart = serialDetailAuto.MfrWanStartDate,
                                            SerialNumber = serialDetailAuto.SerailNumber,
                                            PlateNumber = serialDetailAuto.PlateNumber,
                                            Color = serialDetailAuto.Color,
                                            Brand = serialDetailAuto.Brand,
                                            Condition = serialDetailAuto.Condition,
                                            Type = serialDetailAuto.Type,
                                            Power = serialDetailAuto.Power,
                                            Year = serialDetailAuto.Year,

                                        });
                                    }
                                    else
                                    {
                                        serialDetialViewModelPurchase[i].MfrSerialNo = mrfNo;
                                    }
                                }
                                break;
                            case OperationAutomaticStringCreation.Decrease:
                                for (var i = 0; i < serialDetailAuto.Qty; i++)
                                {
                                    string mrfNo;
                                    if (i == 0)
                                    {
                                        mrfNo = $"{serialDetailAuto.AutomaticStringCreations[0].Name}{serialDetailAuto.AutomaticStringCreations[1].Name}";
                                    }
                                    else
                                    {
                                        mrfNo = $"{long.Parse(serialDetailAuto.AutomaticStringCreations[0].Name) - i}{serialDetailAuto.AutomaticStringCreations[1].Name}";
                                    }
                                    if (serialDetialViewModelPurchase.Count > 0)
                                    {
                                        serialDetialViewModelPurchase.Add(new SerialDetialViewModelPurchase
                                        {
                                            AdmissionDate = serialDetailAuto.AdmissionDate,
                                            Detials = serialDetailAuto.Detials,
                                            ExpirationDate = serialDetailAuto.ExpDate,
                                            LineID = DateTime.Now.Ticks.ToString(),
                                            Location = serialDetailAuto.Location,
                                            LotNumber = serialDetailAuto.LotNumber,
                                            MfrDate = serialDetailAuto.MfrDate,
                                            MfrSerialNo = mrfNo,
                                            MfrWarrantyEnd = serialDetailAuto.MfrWanEndDate,
                                            MfrWarrantyStart = serialDetailAuto.MfrWanStartDate,
                                            SerialNumber = serialDetailAuto.SerailNumber,
                                            PlateNumber = serialDetailAuto.PlateNumber,
                                            Color = serialDetailAuto.Color,
                                            Brand = serialDetailAuto.Brand,
                                            Condition = serialDetailAuto.Condition,
                                            Type = serialDetailAuto.Type,
                                            Power = serialDetailAuto.Power,
                                            Year = serialDetailAuto.Year,
                                        });
                                    }
                                    else
                                    {
                                        serialDetialViewModelPurchase[i].MfrSerialNo = mrfNo;
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }
                switch (serialDetailAuto.AutomaticStringCreations[1].TypeInt)
                {
                    case TypeAutomaticStringCreation.Number:
                        switch (serialDetailAuto.AutomaticStringCreations[1].OperationInt)
                        {
                            case OperationAutomaticStringCreation.Increase:
                                for (var i = 0; i < serialDetailAuto.Qty; i++)
                                {
                                    string mfrNo;
                                    if (i == 0)
                                    {
                                        mfrNo = $"{serialDetailAuto.AutomaticStringCreations[0].Name}{serialDetailAuto.AutomaticStringCreations[1].Name}";
                                    }
                                    else
                                    {
                                        mfrNo = $"{serialDetailAuto.AutomaticStringCreations[0].Name}{long.Parse(serialDetailAuto.AutomaticStringCreations[1].Name) + i}";
                                    }
                                    if (serialDetialViewModelPurchase.Count < serialDetailAuto.Qty)
                                    {
                                        serialDetialViewModelPurchase.Add(new SerialDetialViewModelPurchase
                                        {
                                            AdmissionDate = serialDetailAuto.AdmissionDate,
                                            Detials = serialDetailAuto.Detials,
                                            ExpirationDate = serialDetailAuto.ExpDate,
                                            LineID = DateTime.Now.Ticks.ToString(),
                                            Location = serialDetailAuto.Location,
                                            LotNumber = serialDetailAuto.LotNumber,
                                            MfrDate = serialDetailAuto.MfrDate,
                                            MfrSerialNo = mfrNo,
                                            MfrWarrantyEnd = serialDetailAuto.MfrWanEndDate,
                                            MfrWarrantyStart = serialDetailAuto.MfrWanStartDate,
                                            SerialNumber = serialDetailAuto.SerailNumber,
                                            PlateNumber = serialDetailAuto.PlateNumber,
                                            Color = serialDetailAuto.Color,
                                            Brand = serialDetailAuto.Brand,
                                            Condition = serialDetailAuto.Condition,
                                            Type = serialDetailAuto.Type,
                                            Power = serialDetailAuto.Power,
                                            Year = serialDetailAuto.Year,
                                        });
                                    }
                                    else
                                    {
                                        serialDetialViewModelPurchase[i].MfrSerialNo = mfrNo;
                                    }

                                }
                                break;
                            case OperationAutomaticStringCreation.Decrease:
                                for (var i = 0; i < serialDetailAuto.Qty; i++)
                                {
                                    string mrfNo;
                                    if (i == 0)
                                    {
                                        mrfNo = $"{serialDetailAuto.AutomaticStringCreations[0].Name}{serialDetailAuto.AutomaticStringCreations[1].Name}";
                                    }
                                    else
                                    {
                                        mrfNo = $"{serialDetailAuto.AutomaticStringCreations[0].Name}{long.Parse(serialDetailAuto.AutomaticStringCreations[1].Name) - i}";
                                    }
                                    serialDetialViewModelPurchase.Add(new SerialDetialViewModelPurchase
                                    {
                                        AdmissionDate = serialDetailAuto.AdmissionDate,
                                        Detials = serialDetailAuto.Detials,
                                        ExpirationDate = serialDetailAuto.ExpDate,
                                        LineID = DateTime.Now.Ticks.ToString(),
                                        Location = serialDetailAuto.Location,
                                        LotNumber = serialDetailAuto.LotNumber,
                                        MfrDate = serialDetailAuto.MfrDate,
                                        MfrSerialNo = mrfNo,
                                        MfrWarrantyEnd = serialDetailAuto.MfrWanEndDate,
                                        MfrWarrantyStart = serialDetailAuto.MfrWanStartDate,
                                        SerialNumber = serialDetailAuto.SerailNumber,
                                        PlateNumber = serialDetailAuto.PlateNumber,
                                        Color = serialDetailAuto.Color,
                                        Brand = serialDetailAuto.Brand,
                                        Condition = serialDetailAuto.Condition,
                                        Type = serialDetailAuto.Type,
                                        Power = serialDetailAuto.Power,
                                        Year = serialDetailAuto.Year,
                                    });
                                }
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }
            }
            if (serialDetailAuto.SerialAutomaticStringCreations != null)
            {
                switch (serialDetailAuto.SerialAutomaticStringCreations[0].TypeInt)
                {
                    case TypeAutomaticStringCreation.Number:
                        switch (serialDetailAuto.SerialAutomaticStringCreations[0].OperationInt)
                        {
                            case OperationAutomaticStringCreation.Increase:
                                for (var i = 0; i < serialDetailAuto.Qty; i++)
                                {
                                    string sNo;
                                    if (i == 0)
                                    {
                                        sNo = $"{serialDetailAuto.SerialAutomaticStringCreations[0].Name}{serialDetailAuto.SerialAutomaticStringCreations[1].Name}";
                                    }
                                    else
                                    {
                                        sNo = $"{long.Parse(serialDetailAuto.SerialAutomaticStringCreations[0].Name) + i}{serialDetailAuto.SerialAutomaticStringCreations[1].Name}";
                                    }
                                    if (serialDetialViewModelPurchase.Count < serialDetailAuto.Qty)
                                    {
                                        serialDetialViewModelPurchase.Add(new SerialDetialViewModelPurchase
                                        {
                                            AdmissionDate = serialDetailAuto.AdmissionDate,
                                            Detials = serialDetailAuto.Detials,
                                            ExpirationDate = serialDetailAuto.ExpDate,
                                            LineID = DateTime.Now.Ticks.ToString(),
                                            Location = serialDetailAuto.Location,
                                            LotNumber = serialDetailAuto.LotNumber,
                                            MfrDate = serialDetailAuto.MfrDate,
                                            MfrSerialNo = serialDetailAuto.MfrSerialNo,
                                            MfrWarrantyEnd = serialDetailAuto.MfrWanEndDate,
                                            MfrWarrantyStart = serialDetailAuto.MfrWanStartDate,
                                            SerialNumber = sNo,
                                            PlateNumber = serialDetailAuto.PlateNumber,
                                            Color = serialDetailAuto.Color,
                                            Brand = serialDetailAuto.Brand,
                                            Condition = serialDetailAuto.Condition,
                                            Type = serialDetailAuto.Type,
                                            Power = serialDetailAuto.Power,
                                            Year = serialDetailAuto.Year,
                                        });
                                    }
                                    else
                                    {
                                        serialDetialViewModelPurchase[i].SerialNumber = sNo;
                                    }

                                }
                                break;
                            case OperationAutomaticStringCreation.Decrease:
                                for (var i = 0; i < serialDetailAuto.Qty; i++)
                                {
                                    string sNo;
                                    if (i == 0)
                                    {
                                        sNo = $"{serialDetailAuto.SerialAutomaticStringCreations[0].Name}{serialDetailAuto.SerialAutomaticStringCreations[1].Name}";
                                    }
                                    else
                                    {
                                        sNo = $"{long.Parse(serialDetailAuto.SerialAutomaticStringCreations[0].Name) - i}{serialDetailAuto.SerialAutomaticStringCreations[1].Name}";
                                    }
                                    if (serialDetialViewModelPurchase.Count < serialDetailAuto.Qty)
                                    {
                                        serialDetialViewModelPurchase.Add(new SerialDetialViewModelPurchase
                                        {
                                            AdmissionDate = serialDetailAuto.AdmissionDate,
                                            Detials = serialDetailAuto.Detials,
                                            ExpirationDate = serialDetailAuto.ExpDate,
                                            LineID = DateTime.Now.Ticks.ToString(),
                                            Location = serialDetailAuto.Location,
                                            LotNumber = serialDetailAuto.LotNumber,
                                            MfrDate = serialDetailAuto.MfrDate,
                                            MfrSerialNo = serialDetailAuto.MfrSerialNo,
                                            MfrWarrantyEnd = serialDetailAuto.MfrWanEndDate,
                                            MfrWarrantyStart = serialDetailAuto.MfrWanStartDate,
                                            SerialNumber = sNo,
                                            PlateNumber = serialDetailAuto.PlateNumber,
                                            Color = serialDetailAuto.Color,
                                            Brand = serialDetailAuto.Brand,
                                            Condition = serialDetailAuto.Condition,
                                            Type = serialDetailAuto.Type,
                                            Power = serialDetailAuto.Power,
                                            Year = serialDetailAuto.Year,
                                        });
                                    }
                                    else
                                    {
                                        serialDetialViewModelPurchase[i].SerialNumber = sNo;
                                    }

                                }
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }
                switch (serialDetailAuto.SerialAutomaticStringCreations[1].TypeInt)
                {
                    case TypeAutomaticStringCreation.Number:
                        switch (serialDetailAuto.SerialAutomaticStringCreations[1].OperationInt)
                        {
                            case OperationAutomaticStringCreation.Increase:
                                for (var i = 0; i < serialDetailAuto.Qty; i++)
                                {
                                    string sNo;
                                    if (i == 0)
                                    {
                                        sNo = $"{serialDetailAuto.SerialAutomaticStringCreations[0].Name}{serialDetailAuto.SerialAutomaticStringCreations[1].Name}";
                                    }
                                    else
                                    {
                                        sNo = $"{serialDetailAuto.SerialAutomaticStringCreations[0].Name}{long.Parse(serialDetailAuto.SerialAutomaticStringCreations[1].Name) + i}";
                                    }
                                    if (serialDetialViewModelPurchase.Count < serialDetailAuto.Qty)
                                    {
                                        serialDetialViewModelPurchase.Add(new SerialDetialViewModelPurchase
                                        {
                                            AdmissionDate = serialDetailAuto.AdmissionDate,
                                            Detials = serialDetailAuto.Detials,
                                            ExpirationDate = serialDetailAuto.ExpDate,
                                            LineID = DateTime.Now.Ticks.ToString(),
                                            Location = serialDetailAuto.Location,
                                            LotNumber = serialDetailAuto.LotNumber,
                                            MfrDate = serialDetailAuto.MfrDate,
                                            MfrSerialNo = serialDetailAuto.MfrSerialNo,
                                            MfrWarrantyEnd = serialDetailAuto.MfrWanEndDate,
                                            MfrWarrantyStart = serialDetailAuto.MfrWanStartDate,
                                            SerialNumber = sNo,
                                            PlateNumber = serialDetailAuto.PlateNumber,
                                            Color = serialDetailAuto.Color,
                                            Brand = serialDetailAuto.Brand,
                                            Condition = serialDetailAuto.Condition,
                                            Type = serialDetailAuto.Type,
                                            Power = serialDetailAuto.Power,
                                            Year = serialDetailAuto.Year,
                                        });
                                    }
                                    else
                                    {
                                        serialDetialViewModelPurchase[i].SerialNumber = sNo;
                                    }
                                }
                                break;
                            case OperationAutomaticStringCreation.Decrease:
                                for (var i = 0; i < serialDetailAuto.Qty; i++)
                                {
                                    string sNo;
                                    if (i == 0)
                                    {
                                        sNo = $"{serialDetailAuto.SerialAutomaticStringCreations[0].Name}{serialDetailAuto.SerialAutomaticStringCreations[1].Name}";
                                    }
                                    else
                                    {
                                        sNo = $"{serialDetailAuto.SerialAutomaticStringCreations[0].Name}{long.Parse(serialDetailAuto.SerialAutomaticStringCreations[1].Name) - i}";
                                    }
                                    if (serialDetialViewModelPurchase.Count < serialDetailAuto.Qty)
                                    {
                                        serialDetialViewModelPurchase.Add(new SerialDetialViewModelPurchase
                                        {
                                            AdmissionDate = serialDetailAuto.AdmissionDate,
                                            Detials = serialDetailAuto.Detials,
                                            ExpirationDate = serialDetailAuto.ExpDate,
                                            LineID = DateTime.Now.Ticks.ToString(),
                                            Location = serialDetailAuto.Location,
                                            LotNumber = serialDetailAuto.LotNumber,
                                            MfrDate = serialDetailAuto.MfrDate,
                                            MfrSerialNo = serialDetailAuto.MfrSerialNo,
                                            MfrWarrantyEnd = serialDetailAuto.MfrWanEndDate,
                                            MfrWarrantyStart = serialDetailAuto.MfrWanStartDate,
                                            SerialNumber = sNo,
                                            PlateNumber = serialDetailAuto.PlateNumber,
                                            Color = serialDetailAuto.Color,
                                            Brand = serialDetailAuto.Brand,
                                            Condition = serialDetailAuto.Condition,
                                            Type = serialDetailAuto.Type,
                                            Power = serialDetailAuto.Power,
                                            Year = serialDetailAuto.Year,
                                        });
                                    }
                                    else
                                    {
                                        serialDetialViewModelPurchase[i].SerialNumber = sNo;
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }
            }
            if (serialDetailAuto.LotAutomaticStringCreations != null)
            {
                switch (serialDetailAuto.LotAutomaticStringCreations[0].TypeInt)
                {
                    case TypeAutomaticStringCreation.Number:
                        switch (serialDetailAuto.LotAutomaticStringCreations[0].OperationInt)
                        {
                            case OperationAutomaticStringCreation.Increase:
                                for (var i = 0; i < serialDetailAuto.Qty; i++)
                                {
                                    string lotNo;
                                    if (i == 0)
                                    {
                                        lotNo = $"{serialDetailAuto.LotAutomaticStringCreations[0].Name}{serialDetailAuto.LotAutomaticStringCreations[1].Name}";
                                    }
                                    else
                                    {
                                        lotNo = $"{long.Parse(serialDetailAuto.LotAutomaticStringCreations[0].Name) + i}{serialDetailAuto.LotAutomaticStringCreations[1].Name}";
                                    }
                                    if (serialDetialViewModelPurchase.Count < serialDetailAuto.Qty)
                                    {
                                        serialDetialViewModelPurchase.Add(new SerialDetialViewModelPurchase
                                        {
                                            AdmissionDate = serialDetailAuto.AdmissionDate,
                                            Detials = serialDetailAuto.Detials,
                                            ExpirationDate = serialDetailAuto.ExpDate,
                                            LineID = DateTime.Now.Ticks.ToString(),
                                            Location = serialDetailAuto.Location,
                                            LotNumber = lotNo,
                                            MfrDate = serialDetailAuto.MfrDate,
                                            MfrSerialNo = serialDetailAuto.MfrSerialNo,
                                            MfrWarrantyEnd = serialDetailAuto.MfrWanEndDate,
                                            MfrWarrantyStart = serialDetailAuto.MfrWanStartDate,
                                            SerialNumber = serialDetailAuto.SerailNumber,
                                            PlateNumber = serialDetailAuto.PlateNumber,
                                            Color = serialDetailAuto.Color,
                                            Brand = serialDetailAuto.Brand,
                                            Condition = serialDetailAuto.Condition,
                                            Type = serialDetailAuto.Type,
                                            Power = serialDetailAuto.Power,
                                            Year = serialDetailAuto.Year,
                                        });
                                    }
                                    else
                                    {
                                        serialDetialViewModelPurchase[i].LotNumber = lotNo;
                                    }
                                }
                                break;
                            case OperationAutomaticStringCreation.Decrease:
                                for (var i = 0; i < serialDetailAuto.Qty; i++)
                                {
                                    string lotNo;
                                    if (i == 0)
                                    {
                                        lotNo = $"{serialDetailAuto.LotAutomaticStringCreations[0].Name}{serialDetailAuto.LotAutomaticStringCreations[1].Name}";
                                    }
                                    else
                                    {
                                        lotNo = $"{long.Parse(serialDetailAuto.LotAutomaticStringCreations[0].Name) - i}{serialDetailAuto.LotAutomaticStringCreations[1].Name}";
                                    }
                                    if (serialDetialViewModelPurchase.Count < serialDetailAuto.Qty)
                                    {
                                        serialDetialViewModelPurchase.Add(new SerialDetialViewModelPurchase
                                        {
                                            AdmissionDate = serialDetailAuto.AdmissionDate,
                                            Detials = serialDetailAuto.Detials,
                                            ExpirationDate = serialDetailAuto.ExpDate,
                                            LineID = DateTime.Now.Ticks.ToString(),
                                            Location = serialDetailAuto.Location,
                                            LotNumber = lotNo,
                                            MfrDate = serialDetailAuto.MfrDate,
                                            MfrSerialNo = serialDetailAuto.MfrSerialNo,
                                            MfrWarrantyEnd = serialDetailAuto.MfrWanEndDate,
                                            MfrWarrantyStart = serialDetailAuto.MfrWanStartDate,
                                            SerialNumber = serialDetailAuto.SerailNumber,
                                            PlateNumber = serialDetailAuto.PlateNumber,
                                            Color = serialDetailAuto.Color,
                                            Brand = serialDetailAuto.Brand,
                                            Condition = serialDetailAuto.Condition,
                                            Type = serialDetailAuto.Type,
                                            Power = serialDetailAuto.Power,
                                            Year = serialDetailAuto.Year,
                                        });

                                    }
                                    else
                                    {
                                        serialDetialViewModelPurchase[i].LotNumber = lotNo;
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }
                switch (serialDetailAuto.LotAutomaticStringCreations[1].TypeInt)
                {
                    case TypeAutomaticStringCreation.Number:
                        switch (serialDetailAuto.LotAutomaticStringCreations[1].OperationInt)
                        {
                            case OperationAutomaticStringCreation.Increase:
                                for (var i = 0; i < serialDetailAuto.Qty; i++)
                                {
                                    string lotNo;
                                    if (i == 0)
                                    {
                                        lotNo = $"{serialDetailAuto.LotAutomaticStringCreations[0].Name}{serialDetailAuto.LotAutomaticStringCreations[1].Name}";
                                    }
                                    else
                                    {
                                        lotNo = $"{serialDetailAuto.LotAutomaticStringCreations[0].Name}{long.Parse(serialDetailAuto.LotAutomaticStringCreations[1].Name) + i}";
                                    }
                                    if (serialDetialViewModelPurchase.Count < serialDetailAuto.Qty)
                                    {
                                        serialDetialViewModelPurchase.Add(new SerialDetialViewModelPurchase
                                        {
                                            AdmissionDate = serialDetailAuto.AdmissionDate,
                                            Detials = serialDetailAuto.Detials,
                                            ExpirationDate = serialDetailAuto.ExpDate,
                                            LineID = DateTime.Now.Ticks.ToString(),
                                            Location = serialDetailAuto.Location,
                                            LotNumber = lotNo,
                                            MfrDate = serialDetailAuto.MfrDate,
                                            MfrSerialNo = serialDetailAuto.MfrSerialNo,
                                            MfrWarrantyEnd = serialDetailAuto.MfrWanEndDate,
                                            MfrWarrantyStart = serialDetailAuto.MfrWanStartDate,
                                            SerialNumber = serialDetailAuto.SerailNumber,
                                            PlateNumber = serialDetailAuto.PlateNumber,
                                            Color = serialDetailAuto.Color,
                                            Brand = serialDetailAuto.Brand,
                                            Condition = serialDetailAuto.Condition,
                                            Type = serialDetailAuto.Type,
                                            Power = serialDetailAuto.Power,
                                            Year = serialDetailAuto.Year,
                                        });
                                    }
                                    else
                                    {
                                        serialDetialViewModelPurchase[i].LotNumber = lotNo;
                                    }
                                }
                                break;
                            case OperationAutomaticStringCreation.Decrease:
                                for (var i = 0; i < serialDetailAuto.Qty; i++)
                                {
                                    string lotNo;
                                    if (i == 0)
                                    {
                                        lotNo = $"{serialDetailAuto.LotAutomaticStringCreations[0].Name}{serialDetailAuto.LotAutomaticStringCreations[1].Name}";
                                    }
                                    else
                                    {
                                        lotNo = $"{serialDetailAuto.LotAutomaticStringCreations[0].Name}{long.Parse(serialDetailAuto.LotAutomaticStringCreations[1].Name) - i}";
                                    }
                                    if (serialDetialViewModelPurchase.Count < serialDetailAuto.Qty)
                                    {
                                        serialDetialViewModelPurchase.Add(new SerialDetialViewModelPurchase
                                        {
                                            AdmissionDate = serialDetailAuto.AdmissionDate,
                                            Detials = serialDetailAuto.Detials,
                                            ExpirationDate = serialDetailAuto.ExpDate,
                                            LineID = DateTime.Now.Ticks.ToString(),
                                            Location = serialDetailAuto.Location,
                                            LotNumber = lotNo,
                                            MfrDate = serialDetailAuto.MfrDate,
                                            MfrSerialNo = serialDetailAuto.MfrSerialNo,
                                            MfrWarrantyEnd = serialDetailAuto.MfrWanEndDate,
                                            MfrWarrantyStart = serialDetailAuto.MfrWanStartDate,
                                            SerialNumber = serialDetailAuto.SerailNumber,
                                            PlateNumber = serialDetailAuto.PlateNumber,
                                            Color = serialDetailAuto.Color,
                                            Brand = serialDetailAuto.Brand,
                                            Condition = serialDetailAuto.Condition,
                                            Type = serialDetailAuto.Type,
                                            Power = serialDetailAuto.Power,
                                            Year = serialDetailAuto.Year,
                                        });
                                    }
                                    else
                                    {
                                        serialDetialViewModelPurchase[i].LotNumber = lotNo;
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }
            }
        }
        public void CheckItemSerail<T, TD>(T pur, List<TD> purd, List<SerialViewModelPurchase> serialViewModelPurchases)
        {
            List<SerialDetialViewModelPurchase> serialDetialViewModelPurchase = new();
            int docId = (int)GetValue(pur, "DocumentTypeID");
            int wareId = (int)GetValue(pur, "WarehouseID");
            string number = GetValue(pur, "Number").ToString();
            var docType = _context.DocumentTypes.Find(docId) ?? new DocumentType();
            var whs = _context.Warehouses.Find(wareId) ?? new Warehouse();
            if (purd.Count > 0)
            {
                foreach (var grd in purd)
                {
                    int itemId = (int)GetValue(grd, "ItemID");
                    double _qty = (double)GetValue(grd, "Qty");
                    int uomId = (int)GetValue(grd, "UomID");
                    string lineIDUN = GetValue(grd, "LineIDUN").ToString();
                    ItemMasterData item = _context.ItemMasterDatas.Find(itemId) ?? new ItemMasterData();
                    var uom = _context.GroupDUoMs.FirstOrDefault(w => w.GroupUoMID == item.GroupUomID && w.AltUOM == uomId);
                    decimal qty = (decimal)(_qty * uom.Factor);
                    var serial = serialViewModelPurchases.FirstOrDefault(i => i.ItemID == itemId) ?? new SerialViewModelPurchase();
                    if (item.ManItemBy == ManageItemBy.SerialNumbers && item.ManMethod == ManagementMethod.OnEveryTransation)
                    {
                        var listserial = serialViewModelPurchases.Where(s => s.LineID == GetValue(grd, "LineIDUN").ToString()).ToList();
                        if (listserial.Count == 0)
                        {
                            for (var i = 0; i < qty; i++)
                            {

                                serialDetialViewModelPurchase.Add(new SerialDetialViewModelPurchase
                                {
                                    AdmissionDate = DateTime.Now,
                                    Detials = "",
                                    ExpirationDate = null,
                                    LineMID = GetValue(grd, "LineIDUN").ToString(),
                                    LineID = DateTime.Now.Ticks.ToString() + i,
                                    Location = "",
                                    LotNumber = "",
                                    MfrDate = null,
                                    MfrSerialNo = "",
                                    MfrWarrantyEnd = null,
                                    MfrWarrantyStart = null,
                                    SerialNumber = "",
                                    PlateNumber = "",
                                    Color = "",
                                    Brand = "",
                                    Condition = "",
                                    Type = "",
                                    Power = "",
                                    Year = "",
                                });
                            }
                            decimal totalCreated = serialDetialViewModelPurchase.Where(i => !string.IsNullOrEmpty(i.SerialNumber)).Count();
                            serialViewModelPurchases.Add(new SerialViewModelPurchase
                            {
                                BaseOnID = (int)GetValue(grd, "BaseOnID"),
                                PurCopyType = (int)GetValue(grd, "PurCopyType"),
                                DocNo = $"{docType.Code}-{number}",
                                ItemCode = item.Code,
                                ItemName = item.KhmerName,
                                ItemDescription = item.Description,
                                LineID = GetValue(grd, "LineIDUN").ToString(), // DateTime.Now.Ticks.ToString(),
                                OpenQty = qty - totalCreated,
                                TotalCreated = totalCreated,
                                ItemID = itemId,
                                AutomaticStringCreations = GetAutomaticStringCreation(serial.AutomaticStringCreations),
                                SerialAutomaticStringCreations = GetAutomaticStringCreation(serial.SerialAutomaticStringCreations),
                                LotAutomaticStringCreations = GetAutomaticStringCreation(serial.LotAutomaticStringCreations),
                                SerialDetailAutoCreation = GetSerialDetailAutoCreation(serialDetialViewModelPurchase.FirstOrDefault(), item, grd, whs, qty),
                                SerialDetialViewModelPurchase = serialDetialViewModelPurchase,
                                TotalNeeded = qty,
                                WhseCode = whs.Code,
                                WhseName = whs.Name,
                                Newrecord = true,
                            });

                        }
                        else
                        {
                            decimal totalCreate = 0;
                            foreach (var obj in listserial)
                            {
                                totalCreate += obj.SerialDetialViewModelPurchase.Where(i => !string.IsNullOrEmpty(i.SerialNumber)).Count();
                            }
                            serialViewModelPurchases.Where(s => s.LineID == GetValue(grd, "LineIDUN").ToString()).ToList().ForEach(i =>
                            {
                                i.TotalCreated = totalCreate;
                            });

                        }

                        serialDetialViewModelPurchase = new List<SerialDetialViewModelPurchase>();

                    }

                }

            }


        }
        public void CheckItemBatch<T, TD>(T pur, List<TD> purd, List<BatchViewModelPurchase> batchViewModelPurchases)
        {
            List<BatchDetialViewModelPurchase> batchDetialViewModelPurchases = new();
            int docId = (int)GetValue(pur, "DocumentTypeID");
            int wareId = (int)GetValue(pur, "WarehouseID");
            string number = GetValue(pur, "Number").ToString();
            var docType = _context.DocumentTypes.Find(docId) ?? new DocumentType();
            var whs = _context.Warehouses.Find(wareId) ?? new Warehouse();
            if (purd.Count > 0)
            {
                foreach (var grd in purd)
                {
                    int itemId = (int)GetValue(grd, "ItemID");
                    double _qty = (double)GetValue(grd, "Qty");
                    int uomId = (int)GetValue(grd, "UomID");
                    ItemMasterData item = _context.ItemMasterDatas.Find(itemId) ?? new ItemMasterData();
                    var uom = _context.GroupDUoMs.FirstOrDefault(w => w.GroupUoMID == item.GroupUomID && w.AltUOM == uomId);
                    decimal qty = (decimal)(_qty * uom.Factor);
                    double purchasePrice = (double)GetValue(grd, "PurchasPrice");
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
                                    UnitPrice = (decimal)purchasePrice,
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
                            BatchDetailAutoCreation = GetBatchDetailAutoCreation(batchDetialViewModelPurchases.FirstOrDefault(), item, grd, whs, qty),
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

        public void CheckItemSerailGoodsReceipt(GoodsReceipt grc, List<SerialViewModelPurchase> serialViewModelPurchases)
        {
            List<SerialDetialViewModelPurchase> serialDetialViewModelPurchase = new();
            var docType = _context.DocumentTypes.Find(grc.DocTypeID) ?? new DocumentType();
            var whs = _context.Warehouses.Find(grc.WarehouseID) ?? new Warehouse();
            if (grc.GoodReceiptDetails.Count > 0)
            {
                foreach (var grcd in grc.GoodReceiptDetails)
                {
                    ItemMasterData item = _context.ItemMasterDatas.Find(grcd.ItemID) ?? new ItemMasterData();
                    var uom = _context.GroupDUoMs.FirstOrDefault(w => w.GroupUoMID == item.GroupUomID && w.AltUOM == grcd.UomID);
                    decimal qty = (decimal)(grcd.Quantity * uom.Factor);
                    var serial = serialViewModelPurchases.FirstOrDefault(i => i.ItemID == grcd.ItemID && i.Cost == (decimal)grcd.Cost) ?? new SerialViewModelPurchase();
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
                                            PlateNumber = "",
                                            Color = "",
                                            Brand = "",
                                            Condition = "",
                                            Type = "",
                                            Power = "",
                                            Year = "",
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
                                    PlateNumber = "",
                                    Color = "",
                                    Brand = "",
                                    Condition = "",
                                    Type = "",
                                    Power = "",
                                    Year = "",
                                });
                            }
                        }
                        decimal totalCreated = serialDetialViewModelPurchase.Where(i => !String.IsNullOrEmpty(i.SerialNumber)).Count();
                        serialViewModelPurchases.Add(new SerialViewModelPurchase
                        {
                            DocNo = $"{docType.Code}-{grc.Number_No}",
                            ItemCode = item.Code,
                            ItemName = item.KhmerName,
                            ItemDescription = item.Description,
                            LineID = DateTime.Now.Ticks.ToString(),
                            OpenQty = qty - totalCreated,
                            TotalCreated = totalCreated,
                            ItemID = grcd.ItemID,
                            AutomaticStringCreations = GetAutomaticStringCreation(serial.AutomaticStringCreations),
                            SerialAutomaticStringCreations = GetAutomaticStringCreation(serial.SerialAutomaticStringCreations),
                            LotAutomaticStringCreations = GetAutomaticStringCreation(serial.LotAutomaticStringCreations),
                            SerialDetailAutoCreation = GetSerialDetailAutoCreationGoodsReceipt(serialDetialViewModelPurchase.FirstOrDefault(), item, grcd, whs, qty),
                            SerialDetialViewModelPurchase = serialDetialViewModelPurchase,
                            TotalNeeded = qty,
                            WhseCode = whs.Code,
                            WhseName = whs.Name,
                            Cost = (decimal)grcd.Cost,
                        });
                        serialDetialViewModelPurchase = new List<SerialDetialViewModelPurchase>();
                    }

                }
            }

        }
        public void CheckItemBatchGoodsReceipt(GoodsReceipt grc, List<BatchViewModelPurchase> batchViewModelPurchases)
        {
            List<BatchDetialViewModelPurchase> batchDetialViewModelPurchases = new();
            var docType = _context.DocumentTypes.Find(grc.DocTypeID) ?? new DocumentType();
            var whs = _context.Warehouses.Find(grc.WarehouseID) ?? new Warehouse();
            if (grc.GoodReceiptDetails.Count > 0)
            {
                foreach (var grcd in grc.GoodReceiptDetails)
                {
                    ItemMasterData item = _context.ItemMasterDatas.Find(grcd.ItemID) ?? new ItemMasterData();
                    var uom = _context.GroupDUoMs.FirstOrDefault(w => w.GroupUoMID == item.GroupUomID && w.AltUOM == grcd.UomID);
                    decimal qty = (decimal)(grcd.Quantity * uom.Factor);
                    var batch = batchViewModelPurchases
                        .FirstOrDefault(i => i.ItemID == grcd.ItemID && i.Cost == (decimal)grcd.Cost) ?? new BatchViewModelPurchase();
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
                                    UnitPrice = (decimal)grcd.Cost,
                                });
                            }
                        }
                        decimal totalCreated = batchDetialViewModelPurchases.Where(i => !String.IsNullOrEmpty(i.Batch)).Sum(i => i.Qty);
                        batchViewModelPurchases.Add(new BatchViewModelPurchase
                        {
                            DocNo = $"{docType.Code}-{grc.Number_No}",
                            ItemCode = item.Code,
                            ItemName = item.KhmerName,
                            ItemDescription = item.Description,
                            LineID = DateTime.Now.Ticks.ToString(),
                            TotalCreated = totalCreated,
                            ItemID = grcd.ItemID,
                            BatchStringCreations = GetAutomaticStringCreation(batch.BatchStringCreations),
                            Batchtrr1StringCreations = GetAutomaticStringCreation(batch.Batchtrr1StringCreations),
                            Batchtrr2StringCreations = GetAutomaticStringCreation(batch.Batchtrr2StringCreations),
                            BatchDetailAutoCreation = GetBatchDetailAutoCreationGoodsReciept(batchDetialViewModelPurchases.FirstOrDefault(), item, grcd, whs, qty),
                            BatchDetialViewModelPurchases = batchDetialViewModelPurchases,
                            TotalNeeded = qty,
                            WhseCode = whs.Code,
                            WhseName = whs.Name,
                            Cost = (decimal)grcd.Cost,
                        });
                        batchDetialViewModelPurchases = new List<BatchDetialViewModelPurchase>();
                    }

                }
            }
        }




        public void CheckItemSerail<T, TD>(T purapc, List<TD> detials, List<APCSerialNumber> aPCSerialNumber, ExchangeRate ex)
        {
            int wareId = (int)GetValue(purapc, "WarehouseID");
            int vendorId = (int)GetValue(purapc, "VendorID");
            var whs = _context.Warehouses.Find(wareId) ?? new Warehouse();
            if (detials.Count > 0)
            {
                foreach (var purapd in detials)
                {
                    int itemId = (int)GetValue(purapd, "ItemID");
                    int uomId = (int)GetValue(purapd, "UomID");
                    double _qty = (double)GetValue(purapd, "Qty");
                    double purchasPrice = (double)GetValue(purapd, "PurchasPrice");
                    string LineIDUN = GetValue(purapd, "LineIDUN").ToString();
                    ItemMasterData item = _context.ItemMasterDatas.Find(itemId) ?? new ItemMasterData();
                    var uom = _context.GroupDUoMs.FirstOrDefault(w => w.GroupUoMID == item.GroupUomID && w.AltUOM == uomId);
                    decimal qty = (decimal)(_qty * uom.Factor);
                    if (item.ManItemBy == ManageItemBy.SerialNumbers && item.ManMethod == ManagementMethod.OnEveryTransation)
                    {
                        var PCSerial = aPCSerialNumber.FirstOrDefault(i => i.ItemID == itemId && i.LineID == LineIDUN && i.Cost == (decimal)(purchasPrice * ex.Rate));
                        decimal totalCreated = 0;
                        if (PCSerial != null)
                        {
                            totalCreated = PCSerial.APCSNSelected.APCSNDSelectedDetails.Count;
                            aPCSerialNumber.ForEach(i =>
                            {
                                if (i.LineID == LineIDUN)
                                {
                                    i.OpenQty = qty - totalCreated;
                                    i.Qty = qty;
                                    i.TotalSelected = totalCreated;
                                }
                            });

                        }
                        else
                        {
                            aPCSerialNumber.Add(new APCSerialNumber
                            {
                                BaseOnID = (int)GetValue(purapd, "BaseOnID"),
                                PurCopyType = (int)GetValue(purapd, "PurCopyType"),
                                LineID = LineIDUN,//$"{DateTime.Now.Ticks}{purchasPrice}",
                                ItemCode = item.Code,
                                ItemName = item.KhmerName,
                                ItemID = itemId,
                                UomID = uomId,
                                Direction = "Out",
                                OpenQty = qty - totalCreated,
                                Qty = qty,
                                TotalSelected = totalCreated,
                                WhsCode = whs.Code,
                                WhsName = whs.Name,
                                BpId = vendorId,
                                APCSerialNumberDetial = new APCSerialNumberDetial(),
                                APCSNSelected = new APCSNSelected(),
                                Cost = (decimal)(purchasPrice * ex.Rate)
                            });

                        }


                    }

                }
            }
        }
        public void CheckItemBatch<T, TD>(T purapc, List<TD> detials, List<APCBatchNo> aPCBatchNos, ExchangeRate ex)
        {
            int wareId = (int)GetValue(purapc, "WarehouseID");
            int vendorId = (int)GetValue(purapc, "VendorID");
            var whs = _context.Warehouses.Find(wareId) ?? new Warehouse();
            if (detials.Count > 0)
            {
                foreach (var purapd in detials)
                {
                    int itemId = (int)GetValue(purapd, "ItemID");
                    int uomId = (int)GetValue(purapd, "UomID");
                    double _qty = (double)GetValue(purapd, "Qty");
                    double purchasPrice = (double)GetValue(purapd, "PurchasPrice");
                    ItemMasterData item = _context.ItemMasterDatas.Find(itemId) ?? new ItemMasterData();
                    var uom = _context.GroupDUoMs.FirstOrDefault(w => w.GroupUoMID == item.GroupUomID && w.AltUOM == uomId);
                    if (item.ManItemBy == ManageItemBy.Batches && item.ManMethod == ManagementMethod.OnEveryTransation)
                    {
                        var aPCBatchNo = aPCBatchNos.FirstOrDefault(i => i.ItemID == itemId && i.Cost == (decimal)(purchasPrice * ex.Rate)) ?? new APCBatchNo();
                        decimal qty = (decimal)_qty;
                        decimal totalCreated = 0;
                        decimal totalBatches = 0;
                        if (aPCBatchNo.APCBatchNoSelected != null)
                        {
                            if (aPCBatchNo.APCBatchNoSelected.APCBatchNoSelectedDetails != null)
                            {
                                aPCBatchNo.APCBatchNoSelected.APCBatchNoSelectedDetails = aPCBatchNo.APCBatchNoSelected.APCBatchNoSelectedDetails.GroupBy(i => i.BatchNo).Select(i => i.FirstOrDefault()).ToList();
                                totalCreated = aPCBatchNo.APCBatchNoSelected.APCBatchNoSelectedDetails.Sum(i => i.SelectedQty);
                                totalBatches = aPCBatchNo.APCBatchNoSelected.APCBatchNoSelectedDetails.Count;
                            }
                            if (aPCBatchNo.APCBatchNoUnselect != null)
                            {
                                if (aPCBatchNo.APCBatchNoUnselect.APCBatchNoUnselectDetials != null &&
                                    aPCBatchNo.APCBatchNoSelected.APCBatchNoSelectedDetails != null)
                                {
                                    foreach (var i in aPCBatchNo.APCBatchNoUnselect.APCBatchNoUnselectDetials.ToList())
                                    {
                                        if (aPCBatchNo.APCBatchNoSelected.APCBatchNoSelectedDetails
                                            .Any(j => j.BatchNo == i.BatchNo && j.SelectedQty == i.OrigialQty))
                                            aPCBatchNo.APCBatchNoUnselect.APCBatchNoUnselectDetials.Remove(i);
                                    }
                                }
                            }
                        }
                        if (aPCBatchNo.APCBatchNoUnselect == null)
                        {
                            aPCBatchNos.Add(new APCBatchNo
                            {
                                LineID = $"{DateTime.Now.Ticks}{purchasPrice}",
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
                                WhsName = whs.Name,
                                BpId = vendorId,
                                APCBatchNoSelected = new APCBatchNoSelected(),
                                APCBatchNoUnselect = new APCBatchNoUnselect(),
                                Cost = (decimal)(purchasPrice * ex.Rate)
                            });
                        }
                        else
                        {
                            aPCBatchNo.TotalBatches = totalBatches;
                            aPCBatchNo.TotalNeeded = qty - totalCreated;
                            aPCBatchNo.Qty = qty;
                            aPCBatchNo.TotalSelected = totalCreated;
                        }
                    }

                }
            }
        }


        public void CheckCanRingItemSerail<T, TD>(T pur, List<TD> purd, List<SerialViewModelPurchase> serialViewModelPurchases)
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
                    int itemId = (int)GetValue(grd, "ItemID");
                    decimal _qty = (decimal)GetValue(grd, "Qty");
                    int uomId = (int)GetValue(grd, "UomID");
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
                                            PlateNumber = "",
                                            Color = "",
                                            Brand = "",
                                            Condition = "",
                                            Type = "",
                                            Power = "",
                                            Year = "",
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
                                    PlateNumber = "",
                                    Color = "",
                                    Brand = "",
                                    Condition = "",
                                    Type = "",
                                    Power = "",
                                    Year = "",
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
        public void CheckCanRingItemBatch<T, TD>(T pur, List<TD> purd, List<BatchViewModelPurchase> batchViewModelPurchases)
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
                    int itemId = (int)GetValue(grd, "ItemID");
                    decimal _qty = (decimal)GetValue(grd, "Qty");
                    int uomId = (int)GetValue(grd, "UomID");
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
        private static SerialDetailAutoCreation GetSerialDetailAutoCreation<T>(
            SerialDetialViewModelPurchase sdvmp,
            ItemMasterData item,
            T grp,
            Warehouse whs,
            decimal qty
            )
        {
            return new SerialDetailAutoCreation
            {
                AdmissionDate = sdvmp.AdmissionDate,
                Cost = (decimal)((double)GetValue(grp, "PurchasPrice")),
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
                PlateNumber = sdvmp.PlateNumber,
                Color = sdvmp.Color,
                Brand = sdvmp.Brand,
                Condition = sdvmp.Condition,
                Type = sdvmp.Type,
                Power = sdvmp.Power,
                Year = sdvmp.Year,
                SysNo = "",

            };
        }
        private static SerialDetailAutoCreation GetSerialCanRingDetailAutoCreation<T>(
            SerialDetialViewModelPurchase sdvmp,
            ItemMasterData item,
            T grp,
            Warehouse whs,
            decimal qty
            )
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
                PlateNumber = sdvmp.PlateNumber,
                Color = sdvmp.Color,
                Brand = sdvmp.Brand,
                Condition = sdvmp.Condition,
                Type = sdvmp.Type,
                Power = sdvmp.Power,
                Year = sdvmp.Year,
                SysNo = "",

            };
        }
        private static SerialDetailAutoCreation GetSerialDetailAutoCreationGoodsReceipt(
            SerialDetialViewModelPurchase sdvmp,
            ItemMasterData item,
            GoodReceiptDetail grp,
            Warehouse whs,
            decimal qty
            )
        {
            return new SerialDetailAutoCreation
            {
                AdmissionDate = sdvmp.AdmissionDate,
                Cost = (decimal)grp.Cost,
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
                PlateNumber = sdvmp.PlateNumber,
                Color = sdvmp.Color,
                Brand = sdvmp.Brand,
                Condition = sdvmp.Condition,
                Type = sdvmp.Type,
                Power = sdvmp.Power,
                Year = sdvmp.Year,
                SysNo = "",

            };
        }
        private static BatchDetailAutoCreation GetBatchDetailAutoCreation<T>(
            BatchDetialViewModelPurchase bdvmp,
            ItemMasterData item,
            T grp,
            Warehouse whs,
            decimal qty
            )
        {
            return new BatchDetailAutoCreation
            {
                AdmissionDate = bdvmp.AdmissionDate,
                Cost = (decimal)((double)GetValue(grp, "PurchasPrice")),
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
        private static BatchDetailAutoCreation GetBatchCanRingDetailAutoCreation<T>(
            BatchDetialViewModelPurchase bdvmp,
            ItemMasterData item,
            T grp,
            Warehouse whs,
            decimal qty
            )
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
        private static BatchDetailAutoCreation GetBatchDetailAutoCreationGoodsReciept(
            BatchDetialViewModelPurchase bdvmp,
            ItemMasterData item,
            GoodReceiptDetail grp,
            Warehouse whs,
            decimal qty
            )
        {
            return new BatchDetailAutoCreation
            {
                AdmissionDate = bdvmp.AdmissionDate,
                Cost = (decimal)grp.Cost,
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
        public void GetStringCreation(List<BatchDetialViewModelPurchase> batchDetialViewModelPurchase, BatchDetailAutoCreation batchDetailAuto)
        {
            if (batchDetailAuto.BatchStringCreations != null)
            {
                switch (batchDetailAuto.BatchStringCreations[0].TypeInt)
                {
                    case TypeAutomaticStringCreation.Number:
                        switch (batchDetailAuto.BatchStringCreations[0].OperationInt)
                        {
                            case OperationAutomaticStringCreation.Increase:
                                for (var i = 0; i < batchDetailAuto.NoOfBatch; i++)
                                {
                                    string batch;
                                    if (i == 0)
                                    {
                                        batch = $"{batchDetailAuto.BatchStringCreations[0].Name}{batchDetailAuto.BatchStringCreations[1].Name}";
                                    }
                                    else
                                    {
                                        batch = $"{long.Parse(batchDetailAuto.BatchStringCreations[0].Name) + i}{batchDetailAuto.BatchStringCreations[1].Name}";
                                    }
                                    if (batchDetialViewModelPurchase.Sum(k => k.Qty) < batchDetailAuto.Qty)
                                    {
                                        batchDetialViewModelPurchase.Add(new BatchDetialViewModelPurchase
                                        {
                                            AdmissionDate = batchDetailAuto.AdmissionDate,
                                            Detials = batchDetailAuto.Detials,
                                            ExpirationDate = batchDetailAuto.ExpDate,
                                            LineID = DateTime.Now.Ticks.ToString(),
                                            Location = batchDetailAuto.Location,
                                            MfrDate = batchDetailAuto.MfrDate,
                                            Batch = batch,
                                            BatchAttribute1 = batchDetailAuto.BatchAtrr1,
                                            BatchAttribute2 = batchDetailAuto.BatchAtrr2,
                                            Qty = batchDetailAuto.Qty / batchDetailAuto.NoOfBatch,
                                            UnitPrice = batchDetailAuto.Cost
                                        });
                                    }
                                    else
                                    {
                                        batchDetialViewModelPurchase[i].Batch = batch;
                                    }
                                }
                                break;
                            case OperationAutomaticStringCreation.Decrease:
                                for (var i = 0; i < batchDetailAuto.NoOfBatch; i++)
                                {
                                    string batch;
                                    if (i == 0)
                                    {
                                        batch = $"{batchDetailAuto.BatchStringCreations[0].Name}{batchDetailAuto.BatchStringCreations[1].Name}";
                                    }
                                    else
                                    {
                                        batch = $"{long.Parse(batchDetailAuto.BatchStringCreations[0].Name) - i}{batchDetailAuto.BatchStringCreations[1].Name}";
                                    }
                                    if (batchDetialViewModelPurchase.Sum(k => k.Qty) < batchDetailAuto.Qty)
                                    {
                                        batchDetialViewModelPurchase.Add(new BatchDetialViewModelPurchase
                                        {
                                            AdmissionDate = batchDetailAuto.AdmissionDate,
                                            Detials = batchDetailAuto.Detials,
                                            ExpirationDate = batchDetailAuto.ExpDate,
                                            LineID = DateTime.Now.Ticks.ToString(),
                                            Location = batchDetailAuto.Location,
                                            MfrDate = batchDetailAuto.MfrDate,
                                            Batch = batch,
                                            BatchAttribute1 = batchDetailAuto.BatchAtrr1,
                                            BatchAttribute2 = batchDetailAuto.BatchAtrr2,
                                            Qty = batchDetailAuto.Qty / batchDetailAuto.NoOfBatch,
                                            UnitPrice = batchDetailAuto.Cost
                                        });
                                    }
                                    else
                                    {
                                        batchDetialViewModelPurchase[i].Batch = batch;
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }
                switch (batchDetailAuto.BatchStringCreations[1].TypeInt)
                {
                    case TypeAutomaticStringCreation.Number:
                        switch (batchDetailAuto.BatchStringCreations[1].OperationInt)
                        {
                            case OperationAutomaticStringCreation.Increase:
                                for (var i = 0; i < batchDetailAuto.NoOfBatch; i++)
                                {
                                    string batch;
                                    if (i == 0)
                                    {
                                        batch = $"{batchDetailAuto.BatchStringCreations[0].Name}{batchDetailAuto.BatchStringCreations[1].Name}";
                                    }
                                    else
                                    {
                                        batch = $"{batchDetailAuto.BatchStringCreations[0].Name}{long.Parse(batchDetailAuto.BatchStringCreations[1].Name) + i}";
                                    }
                                    if (batchDetialViewModelPurchase.Sum(k => k.Qty) < batchDetailAuto.Qty)
                                    {
                                        batchDetialViewModelPurchase.Add(new BatchDetialViewModelPurchase
                                        {
                                            AdmissionDate = batchDetailAuto.AdmissionDate,
                                            Detials = batchDetailAuto.Detials,
                                            ExpirationDate = batchDetailAuto.ExpDate,
                                            LineID = DateTime.Now.Ticks.ToString(),
                                            Location = batchDetailAuto.Location,
                                            MfrDate = batchDetailAuto.MfrDate,
                                            Batch = batch,
                                            BatchAttribute1 = batchDetailAuto.BatchAtrr1,
                                            BatchAttribute2 = batchDetailAuto.BatchAtrr2,
                                            Qty = batchDetailAuto.Qty / batchDetailAuto.NoOfBatch,
                                            UnitPrice = batchDetailAuto.Cost
                                        });
                                    }
                                    else
                                    {
                                        batchDetialViewModelPurchase[i].Batch = batch;
                                    }

                                }
                                break;
                            case OperationAutomaticStringCreation.Decrease:
                                for (var i = 0; i < batchDetailAuto.NoOfBatch; i++)
                                {
                                    string batch;
                                    if (i == 0)
                                    {
                                        batch = $"{batchDetailAuto.BatchStringCreations[0].Name}{batchDetailAuto.BatchStringCreations[1].Name}";
                                    }
                                    else
                                    {
                                        batch = $"{batchDetailAuto.BatchStringCreations[0].Name}{long.Parse(batchDetailAuto.BatchStringCreations[1].Name) - i}";
                                    }
                                    if (batchDetialViewModelPurchase.Sum(k => k.Qty) < batchDetailAuto.Qty)
                                    {
                                        batchDetialViewModelPurchase.Add(new BatchDetialViewModelPurchase
                                        {
                                            AdmissionDate = batchDetailAuto.AdmissionDate,
                                            Detials = batchDetailAuto.Detials,
                                            ExpirationDate = batchDetailAuto.ExpDate,
                                            LineID = DateTime.Now.Ticks.ToString(),
                                            Location = batchDetailAuto.Location,
                                            MfrDate = batchDetailAuto.MfrDate,
                                            Batch = batch,
                                            BatchAttribute1 = batchDetailAuto.BatchAtrr1,
                                            BatchAttribute2 = batchDetailAuto.BatchAtrr2,
                                            Qty = batchDetailAuto.Qty / batchDetailAuto.NoOfBatch,
                                            UnitPrice = batchDetailAuto.Cost
                                        });
                                    }
                                    else
                                    {
                                        batchDetialViewModelPurchase[i].Batch = batch;
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }
            }
            if (batchDetailAuto.Batchtrr1StringCreations != null)
            {
                switch (batchDetailAuto.Batchtrr1StringCreations[0].TypeInt)
                {
                    case TypeAutomaticStringCreation.Number:
                        switch (batchDetailAuto.Batchtrr1StringCreations[0].OperationInt)
                        {
                            case OperationAutomaticStringCreation.Increase:
                                for (var i = 0; i < batchDetailAuto.NoOfBatch; i++)
                                {
                                    string batchattr1;
                                    if (i == 0)
                                    {
                                        batchattr1 = $"{batchDetailAuto.Batchtrr1StringCreations[0].Name}{batchDetailAuto.Batchtrr1StringCreations[1].Name}";
                                    }
                                    else
                                    {
                                        batchattr1 = $"{long.Parse(batchDetailAuto.Batchtrr1StringCreations[0].Name) + i}{batchDetailAuto.Batchtrr1StringCreations[1].Name}";
                                    }
                                    if (batchDetialViewModelPurchase.Sum(k => k.Qty) < batchDetailAuto.Qty)
                                    {
                                        batchDetialViewModelPurchase.Add(new BatchDetialViewModelPurchase
                                        {
                                            AdmissionDate = batchDetailAuto.AdmissionDate,
                                            Detials = batchDetailAuto.Detials,
                                            ExpirationDate = batchDetailAuto.ExpDate,
                                            LineID = DateTime.Now.Ticks.ToString(),
                                            Location = batchDetailAuto.Location,
                                            MfrDate = batchDetailAuto.MfrDate,
                                            Batch = batchDetailAuto.Batch,
                                            BatchAttribute1 = batchattr1,
                                            BatchAttribute2 = batchDetailAuto.BatchAtrr2,
                                            Qty = batchDetailAuto.Qty / batchDetailAuto.NoOfBatch,
                                            UnitPrice = batchDetailAuto.Cost
                                        });
                                    }
                                    else
                                    {
                                        batchDetialViewModelPurchase[i].BatchAttribute1 = batchattr1;
                                    }

                                }
                                break;
                            case OperationAutomaticStringCreation.Decrease:
                                for (var i = 0; i < batchDetailAuto.NoOfBatch; i++)
                                {
                                    string batchattr1;
                                    if (i == 0)
                                    {
                                        batchattr1 = $"{batchDetailAuto.Batchtrr1StringCreations[0].Name}{batchDetailAuto.Batchtrr1StringCreations[1].Name}";
                                    }
                                    else
                                    {
                                        batchattr1 = $"{long.Parse(batchDetailAuto.Batchtrr1StringCreations[0].Name) - i}{batchDetailAuto.Batchtrr1StringCreations[1].Name}";
                                    }
                                    if (batchDetialViewModelPurchase.Sum(k => k.Qty) < batchDetailAuto.Qty)
                                    {
                                        batchDetialViewModelPurchase.Add(new BatchDetialViewModelPurchase
                                        {
                                            AdmissionDate = batchDetailAuto.AdmissionDate,
                                            Detials = batchDetailAuto.Detials,
                                            ExpirationDate = batchDetailAuto.ExpDate,
                                            LineID = DateTime.Now.Ticks.ToString(),
                                            Location = batchDetailAuto.Location,
                                            MfrDate = batchDetailAuto.MfrDate,
                                            Batch = batchDetailAuto.Batch,
                                            BatchAttribute1 = batchattr1,
                                            BatchAttribute2 = batchDetailAuto.BatchAtrr2,
                                            Qty = batchDetailAuto.Qty / batchDetailAuto.NoOfBatch,
                                            UnitPrice = batchDetailAuto.Cost
                                        });
                                    }
                                    else
                                    {
                                        batchDetialViewModelPurchase[i].BatchAttribute1 = batchattr1;
                                    }

                                }
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }
                switch (batchDetailAuto.Batchtrr1StringCreations[1].TypeInt)
                {
                    case TypeAutomaticStringCreation.Number:
                        switch (batchDetailAuto.Batchtrr1StringCreations[1].OperationInt)
                        {
                            case OperationAutomaticStringCreation.Increase:
                                for (var i = 0; i < batchDetailAuto.NoOfBatch; i++)
                                {
                                    string batchattr1;
                                    if (i == 0)
                                    {
                                        batchattr1 = $"{batchDetailAuto.Batchtrr1StringCreations[0].Name}{batchDetailAuto.Batchtrr1StringCreations[1].Name}";
                                    }
                                    else
                                    {
                                        batchattr1 = $"{batchDetailAuto.Batchtrr1StringCreations[0].Name}{long.Parse(batchDetailAuto.Batchtrr1StringCreations[1].Name) + i}";
                                    }
                                    if (batchDetialViewModelPurchase.Sum(k => k.Qty) < batchDetailAuto.Qty)
                                    {
                                        batchDetialViewModelPurchase.Add(new BatchDetialViewModelPurchase
                                        {
                                            AdmissionDate = batchDetailAuto.AdmissionDate,
                                            Detials = batchDetailAuto.Detials,
                                            ExpirationDate = batchDetailAuto.ExpDate,
                                            LineID = DateTime.Now.Ticks.ToString(),
                                            Location = batchDetailAuto.Location,
                                            MfrDate = batchDetailAuto.MfrDate,
                                            Batch = batchDetailAuto.Batch,
                                            BatchAttribute1 = batchattr1,
                                            BatchAttribute2 = batchDetailAuto.BatchAtrr2,
                                            Qty = batchDetailAuto.Qty / batchDetailAuto.NoOfBatch,
                                            UnitPrice = batchDetailAuto.Cost
                                        });
                                    }
                                    else
                                    {
                                        batchDetialViewModelPurchase[i].BatchAttribute1 = batchattr1;
                                    }
                                }
                                break;
                            case OperationAutomaticStringCreation.Decrease:
                                for (var i = 0; i < batchDetailAuto.NoOfBatch; i++)
                                {
                                    string batchattr1;
                                    if (i == 0)
                                    {
                                        batchattr1 = $"{batchDetailAuto.Batchtrr1StringCreations[0].Name}{batchDetailAuto.Batchtrr1StringCreations[1].Name}";
                                    }
                                    else
                                    {
                                        batchattr1 = $"{batchDetailAuto.Batchtrr1StringCreations[0].Name}{long.Parse(batchDetailAuto.Batchtrr1StringCreations[1].Name) - i}";
                                    }
                                    if (batchDetialViewModelPurchase.Sum(k => k.Qty) < batchDetailAuto.Qty)
                                    {
                                        batchDetialViewModelPurchase.Add(new BatchDetialViewModelPurchase
                                        {
                                            AdmissionDate = batchDetailAuto.AdmissionDate,
                                            Detials = batchDetailAuto.Detials,
                                            ExpirationDate = batchDetailAuto.ExpDate,
                                            LineID = DateTime.Now.Ticks.ToString(),
                                            Location = batchDetailAuto.Location,
                                            MfrDate = batchDetailAuto.MfrDate,
                                            Batch = batchDetailAuto.Batch,
                                            BatchAttribute1 = batchattr1,
                                            BatchAttribute2 = batchDetailAuto.BatchAtrr2,
                                            Qty = batchDetailAuto.Qty / batchDetailAuto.NoOfBatch,
                                            UnitPrice = batchDetailAuto.Cost
                                        });
                                    }
                                    else
                                    {
                                        batchDetialViewModelPurchase[i].BatchAttribute1 = batchattr1;
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }
            }
            if (batchDetailAuto.Batchtrr2StringCreations != null)
            {
                switch (batchDetailAuto.Batchtrr2StringCreations[0].TypeInt)
                {
                    case TypeAutomaticStringCreation.Number:
                        switch (batchDetailAuto.Batchtrr2StringCreations[0].OperationInt)
                        {
                            case OperationAutomaticStringCreation.Increase:
                                for (var i = 0; i < batchDetailAuto.NoOfBatch; i++)
                                {
                                    string batchattr2;
                                    if (i == 0)
                                    {
                                        batchattr2 = $"{batchDetailAuto.Batchtrr2StringCreations[0].Name}{batchDetailAuto.Batchtrr2StringCreations[1].Name}";
                                    }
                                    else
                                    {
                                        batchattr2 = $"{long.Parse(batchDetailAuto.Batchtrr2StringCreations[0].Name) + i}{batchDetailAuto.Batchtrr2StringCreations[1].Name}";
                                    }
                                    if (batchDetialViewModelPurchase.Sum(k => k.Qty) < batchDetailAuto.Qty)
                                    {
                                        batchDetialViewModelPurchase.Add(new BatchDetialViewModelPurchase
                                        {
                                            AdmissionDate = batchDetailAuto.AdmissionDate,
                                            Detials = batchDetailAuto.Detials,
                                            ExpirationDate = batchDetailAuto.ExpDate,
                                            LineID = DateTime.Now.Ticks.ToString(),
                                            Location = batchDetailAuto.Location,
                                            MfrDate = batchDetailAuto.MfrDate,
                                            Batch = batchDetailAuto.Batch,
                                            BatchAttribute1 = batchDetailAuto.BatchAtrr1,
                                            BatchAttribute2 = batchattr2,
                                            Qty = batchDetailAuto.Qty / batchDetailAuto.NoOfBatch,
                                            UnitPrice = batchDetailAuto.Cost
                                        });
                                    }
                                    else
                                    {
                                        batchDetialViewModelPurchase[i].BatchAttribute2 = batchattr2;
                                    }
                                }
                                break;
                            case OperationAutomaticStringCreation.Decrease:
                                for (var i = 0; i < batchDetailAuto.NoOfBatch; i++)
                                {
                                    string batchattr2;
                                    if (i == 0)
                                    {
                                        batchattr2 = $"{batchDetailAuto.Batchtrr2StringCreations[0].Name}{batchDetailAuto.Batchtrr2StringCreations[1].Name}";
                                    }
                                    else
                                    {
                                        batchattr2 = $"{long.Parse(batchDetailAuto.Batchtrr2StringCreations[0].Name) - i}{batchDetailAuto.Batchtrr2StringCreations[1].Name}";
                                    }
                                    if (batchDetialViewModelPurchase.Sum(k => k.Qty) < batchDetailAuto.Qty)
                                    {
                                        batchDetialViewModelPurchase.Add(new BatchDetialViewModelPurchase
                                        {
                                            AdmissionDate = batchDetailAuto.AdmissionDate,
                                            Detials = batchDetailAuto.Detials,
                                            ExpirationDate = batchDetailAuto.ExpDate,
                                            LineID = DateTime.Now.Ticks.ToString(),
                                            Location = batchDetailAuto.Location,
                                            MfrDate = batchDetailAuto.MfrDate,
                                            Batch = batchDetailAuto.Batch,
                                            BatchAttribute1 = batchDetailAuto.BatchAtrr1,
                                            BatchAttribute2 = batchattr2,
                                            Qty = batchDetailAuto.Qty / batchDetailAuto.NoOfBatch,
                                            UnitPrice = batchDetailAuto.Cost
                                        });
                                    }
                                    else
                                    {
                                        batchDetialViewModelPurchase[i].BatchAttribute2 = batchattr2;
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }
                switch (batchDetailAuto.Batchtrr2StringCreations[1].TypeInt)
                {
                    case TypeAutomaticStringCreation.Number:
                        switch (batchDetailAuto.Batchtrr2StringCreations[1].OperationInt)
                        {
                            case OperationAutomaticStringCreation.Increase:
                                for (var i = 0; i < batchDetailAuto.NoOfBatch; i++)
                                {
                                    string batchattr2;
                                    if (i == 0)
                                    {
                                        batchattr2 = $"{batchDetailAuto.Batchtrr2StringCreations[0].Name}{batchDetailAuto.Batchtrr2StringCreations[1].Name}";
                                    }
                                    else
                                    {
                                        batchattr2 = $"{batchDetailAuto.Batchtrr2StringCreations[0].Name}{long.Parse(batchDetailAuto.Batchtrr2StringCreations[1].Name) + i}";
                                    }
                                    if (batchDetialViewModelPurchase.Sum(k => k.Qty) < batchDetailAuto.Qty)
                                    {
                                        batchDetialViewModelPurchase.Add(new BatchDetialViewModelPurchase
                                        {
                                            AdmissionDate = batchDetailAuto.AdmissionDate,
                                            Detials = batchDetailAuto.Detials,
                                            ExpirationDate = batchDetailAuto.ExpDate,
                                            LineID = DateTime.Now.Ticks.ToString(),
                                            Location = batchDetailAuto.Location,
                                            MfrDate = batchDetailAuto.MfrDate,
                                            Batch = batchDetailAuto.Batch,
                                            BatchAttribute1 = batchDetailAuto.BatchAtrr1,
                                            BatchAttribute2 = batchattr2,
                                            Qty = batchDetailAuto.Qty / batchDetailAuto.NoOfBatch,
                                            UnitPrice = batchDetailAuto.Cost
                                        });
                                    }
                                    else
                                    {
                                        batchDetialViewModelPurchase[i].BatchAttribute2 = batchattr2;
                                    }
                                }
                                break;
                            case OperationAutomaticStringCreation.Decrease:
                                for (var i = 0; i < batchDetailAuto.NoOfBatch; i++)
                                {
                                    string batchattr2;
                                    if (i == 0)
                                    {
                                        batchattr2 = $"{batchDetailAuto.Batchtrr2StringCreations[0].Name}{batchDetailAuto.Batchtrr2StringCreations[1].Name}";
                                    }
                                    else
                                    {
                                        batchattr2 = $"{batchDetailAuto.Batchtrr2StringCreations[0].Name}{long.Parse(batchDetailAuto.Batchtrr2StringCreations[1].Name) - i}";
                                    }
                                    if (batchDetialViewModelPurchase.Sum(k => k.Qty) < batchDetailAuto.Qty)
                                    {
                                        batchDetialViewModelPurchase.Add(new BatchDetialViewModelPurchase
                                        {
                                            AdmissionDate = batchDetailAuto.AdmissionDate,
                                            Detials = batchDetailAuto.Detials,
                                            ExpirationDate = batchDetailAuto.ExpDate,
                                            LineID = DateTime.Now.Ticks.ToString(),
                                            Location = batchDetailAuto.Location,
                                            MfrDate = batchDetailAuto.MfrDate,
                                            Batch = batchDetailAuto.Batch,
                                            BatchAttribute1 = batchDetailAuto.BatchAtrr1,
                                            BatchAttribute2 = batchattr2,
                                            Qty = batchDetailAuto.Qty / batchDetailAuto.NoOfBatch,
                                            UnitPrice = batchDetailAuto.Cost
                                        });
                                    }
                                    else
                                    {
                                        batchDetialViewModelPurchase[i].BatchAttribute2 = batchattr2;
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        public async Task<PurchaseOrderUpdateViewModel> FindPurchaseOrderAsync(int seriesID, string number, int comId)
        {
            #region
            var taxGroup = GetTaxGroups();
            taxGroup.Insert(0, new TaxGroupViewModel
            {
                ID = 0,
                Name = "- Select --",
                Code = "",
                Rate = 0,
                Type = 0,
            });
            var purOrder = await (from pod in _context.PurchaseOrders.Where(x => x.Number == number && x.SeriesID == seriesID && x.CompanyID == comId)
                                  join docType in _context.DocumentTypes on pod.DocumentTypeID equals docType.ID
                                  let fs = _context.FreightPurchases.Where(i => i.PurID == pod.PurchaseOrderID && i.PurType == PurCopyType.PurOrder).FirstOrDefault() ?? new FreightPurchase()
                                  select new PurchaseOrderViewModel
                                  {
                                      FrieghtAmount = pod.FrieghtAmount,
                                      FrieghtAmountSys = pod.FrieghtAmountSys,
                                      BranchID = pod.BranchID,
                                      CompanyID = pod.CompanyID,
                                      BaseOnID = pod.PurchaseOrderID,
                                      DeliveryDate = pod.DeliveryDate,
                                      DocumentDate = pod.DocumentDate,
                                      FreightPurchaseView = new FreightPurchaseViewModel
                                      {
                                          ExpenceAmount = fs.ExpenceAmount,
                                          PurID = pod.PurchaseOrderID,
                                          ID = fs.ID,
                                          PurType = fs.PurType,
                                          TaxSumValue = fs.TaxSumValue,
                                          FreightPurchaseDetailViewModels = (from fsd in _context.FreightPurchaseDetails.Where(i => i.FreightPurchaseID == fs.ID)
                                                                             select new FreightPurchaseDetailViewModel
                                                                             {
                                                                                 ID = fsd.ID,
                                                                                 FreightPurchaseID = fsd.FreightPurchaseID,
                                                                                 Amount = fsd.Amount,
                                                                                 AmountWithTax = fsd.AmountWithTax,
                                                                                 FreightID = fsd.FreightID,
                                                                                 Name = fsd.Name,
                                                                                 TaxGroup = fsd.TaxGroup,
                                                                                 TaxGroupID = fsd.TaxGroupID,
                                                                                 TaxGroups = GetTaxGroups(),
                                                                                 TaxGroupSelect = taxGroup.Select(i => new SelectListItem
                                                                                 {
                                                                                     Value = i.ID.ToString(),
                                                                                     Selected = fsd.TaxGroupID == i.ID,
                                                                                     Text = $"{i.Code}-{i.Name}"
                                                                                 }).ToList(),
                                                                                 TaxRate = fsd.TaxRate,
                                                                                 TotalTaxAmount = fsd.TotalTaxAmount
                                                                             }).ToList(),
                                      },
                                      InvoiceNo = $"{docType.Code}-{pod.Number}",
                                      LocalCurID = pod.LocalCurID,
                                      LocalSetRate = (decimal)pod.LocalSetRate,
                                      PostingDate = pod.PostingDate,
                                      Status = pod.Status,
                                      SubTotalAfterDis = pod.SubTotalAfterDis,
                                      SubTotalAfterDisSys = pod.SubTotalAfterDisSys,
                                      TypeDis = pod.TypeDis,
                                      UserID = pod.UserID,
                                      WarehouseID = pod.WarehouseID,
                                      PurchaseOrderID = pod.PurchaseOrderID,
                                      AdditionalExpense = (decimal)pod.AdditionalExpense,
                                      AdditionalNote = pod.AdditionalNote,
                                      AppliedAmount = (decimal)pod.AppliedAmount,
                                      AppliedAmountSys = (decimal)pod.AppliedAmountSys,
                                      BalanceDue = (decimal)pod.BalanceDue,
                                      BalanceDueSys = (decimal)pod.BalanceDueSys,
                                      DiscountRate = (decimal)pod.DiscountRate,
                                      DiscountValue = (decimal)pod.DiscountValue,
                                      DocumentTypeID = pod.DocumentTypeID,
                                      DownPayment = (decimal)pod.DownPayment,
                                      DownPaymentSys = (decimal)pod.DownPaymentSys,
                                      Number = pod.Number,
                                      PurCurrencyID = pod.PurCurrencyID,
                                      PurRate = (decimal)pod.PurRate,
                                      ReffNo = pod.ReffNo,
                                      Remark = pod.Remark,
                                      ReturnAmount = (decimal)pod.ReturnAmount,
                                      SeriesDetailID = pod.SeriesDetailID,
                                      SeriesID = pod.SeriesID,
                                      SubTotal = (decimal)pod.SubTotal,
                                      SubTotalSys = (decimal)pod.SubTotalSys,
                                      SysCurrencyID = pod.SysCurrencyID,
                                      TaxRate = (decimal)pod.TaxRate,
                                      TaxValue = (decimal)pod.TaxValue,
                                      VendorID = pod.VendorID
                                  }).ToListAsync();

            var _tg = new TaxGroupViewModel
            {
                ID = 0,
                Name = "- Select --",
                Code = "",
                Rate = 0,
                Type = 0,
            };
            var tgs = (from t in _context.TaxGroups.Where(i => !i.Delete && i.Active && i.Type == TypeTax.InputTax)
                       let tgds = _context.TaxGroupDefinitions.Where(i => i.TaxGroupID == t.ID).OrderByDescending(i => i.EffectiveFrom).FirstOrDefault() ?? new TaxGroupDefinition()
                       select new TaxGroupViewModel
                       {
                           ID = t.ID,
                           Name = t.Name,
                           Code = t.Code,
                           Effectivefrom = tgds.EffectiveFrom,
                           Rate = tgds.Rate,
                           Type = (int)t.Type,
                       }).ToList();
            tgs.Insert(0, _tg);
            #endregion
            var _purOrderDetails = (from purOr in purOrder
                                    join podd in _context.PurchaseOrderDetails on purOr.PurchaseOrderID equals podd.PurchaseOrderID
                                    join item in _context.ItemMasterDatas on podd.ItemID equals item.ID
                                    let cur = _context.Currency.FirstOrDefault(i => i.ID == purOr.PurCurrencyID) ?? new Currency()
                                    select new PurchaseOrderDetialViewModel
                                    {
                                        LineIDUN = DateTime.Now.Ticks.ToString(),
                                        LineID = podd.PurchaseOrderDetailID,
                                        ItemID = item.ID,
                                        OrderID = item.ID,
                                        Process = item.Process,
                                        Qty = (decimal)podd.Qty,
                                        OpenQty = (decimal)podd.OpenQty,
                                        TypeDis = podd.TypeDis,
                                        UomID = podd.UomID,
                                        TaxGroupID = podd.TaxGroupID,
                                        TaxRate = podd.TaxRate,
                                        TaxGroupSelect = tgs.Select(c => new SelectListItem
                                        {
                                            Value = c.ID.ToString(),
                                            Text = $"{c.Code}-{c.Name}",
                                            Selected = c.ID == podd.TaxGroupID
                                        }).ToList(),
                                        TotalWTax = (decimal)podd.TotalWTax,
                                        TaxValue = podd.TaxValue,
                                        Total = (decimal)podd.Total,
                                        Remark = podd.Remark,
                                        /// select List UoM ///
                                        UoMSelect = (from GDU in _context.GroupDUoMs.Where(i => i.GroupUoMID == item.GroupUomID)
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
                                                         Selected = c.ID == podd.UomID
                                                     }).ToList(),
                                        /// List UoM ///
                                        UoMsList = (from GDU in _context.GroupDUoMs.Where(i => i.GroupUoMID == item.GroupUomID)
                                                    join UNM in _context.UnitofMeasures on GDU.AltUOM equals UNM.ID
                                                    select new UOMSViewModel
                                                    {
                                                        BaseUoMID = GDU.BaseUOM,
                                                        Factor = GDU.Factor,
                                                        ID = UNM.ID,
                                                        Name = UNM.Name
                                                    }).ToList(),
                                        TaxGroups = (from t in _context.TaxGroups.Where(i => !i.Delete && i.Active && i.Type == TypeTax.InputTax)
                                                     let tgds = _context.TaxGroupDefinitions.Where(i => i.TaxGroupID == t.ID).OrderByDescending(i => i.EffectiveFrom).FirstOrDefault() ?? new TaxGroupDefinition()
                                                     select new TaxGroupViewModel
                                                     {
                                                         ID = t.ID,
                                                         Name = t.Name,
                                                         Code = t.Code,
                                                         Effectivefrom = tgds.EffectiveFrom,
                                                         Rate = tgds.Rate,
                                                         Type = (int)t.Type,
                                                     }
                                                     ).ToList(),
                                        FinDisRate = podd.FinDisRate,
                                        FinDisValue = podd.FinDisValue,
                                        FinTotalValue = podd.FinTotalValue,
                                        TaxOfFinDisValue = podd.TaxOfFinDisValue,
                                        Barcode = item.Barcode,
                                        Code = item.Code,
                                        CurrencyName = cur.Description,
                                        DiscountRate = (decimal)podd.DiscountRate,
                                        AlertStock = (decimal)podd.AlertStock,
                                        Delete = podd.Delete,
                                        DiscountValue = (decimal)podd.DiscountValue,
                                        ExpireDate = podd.ExpireDate,
                                        ItemName = item.KhmerName,
                                        LocalCurrencyID = podd.LocalCurrencyID,
                                        OldQty = (decimal)podd.OldQty,
                                        PurchaseOrderDetailID = podd.PurchaseOrderDetailID,
                                        PurchaseOrderID = podd.PurchaseOrderID,
                                        PurchasPrice = (decimal)podd.PurchasPrice,
                                        QuotationID = podd.QuotationID,
                                        TotalWTaxSys = (decimal)podd.TotalWTaxSys,
                                        TotalSys = (decimal)podd.TotalSys,
                                    }).ToList();
            _dataProp.DataProperty(_purOrderDetails, comId, "ItemID", "AddictionProps");
            var data = new PurchaseOrderUpdateViewModel
            {
                PurchaseOrder = purOrder.FirstOrDefault(),
                PurchaseOrderDetials = _purOrderDetails,
            };
            return await Task.FromResult(data);
        }
        // Get APReserve do by Mr Bunthorn
        public async Task<PurchaseOrderUpdateViewModel> FindAPresverAsync(int seriesID, string number, int comId)
        {
            #region
            var taxGroup = GetTaxGroups();
            taxGroup.Insert(0, new TaxGroupViewModel
            {
                ID = 0,
                Name = "- Select --",
                Code = "",
                Rate = 0,
                Type = 0,
            });
            var purOrder = await (from pod in _context.PurchaseAPReserves.Where(x => x.Number == number && x.SeriesID == seriesID && x.CompanyID == comId)
                                  join docType in _context.DocumentTypes on pod.DocumentTypeID equals docType.ID
                                  let fs = _context.FreightPurchases.Where(i => i.PurID == pod.ID && i.PurType == PurCopyType.PurReserve).FirstOrDefault() ?? new FreightPurchase()
                                  select new PurchaseOrderViewModel
                                  {
                                      FrieghtAmount = pod.FrieghtAmount,
                                      FrieghtAmountSys = pod.FrieghtAmountSys,
                                      BranchID = pod.BranchID,
                                      CompanyID = pod.CompanyID,
                                      BaseOnID = pod.ID,
                                      DeliveryDate = pod.DueDate,
                                      DocumentDate = pod.DocumentDate,
                                      FreightPurchaseView = new FreightPurchaseViewModel
                                      {
                                          ExpenceAmount = fs.ExpenceAmount,
                                          PurID = pod.ID,
                                          ID = fs.ID,
                                          PurType = fs.PurType,
                                          TaxSumValue = fs.TaxSumValue,
                                          FreightPurchaseDetailViewModels = (from fsd in _context.FreightPurchaseDetails.Where(i => i.FreightPurchaseID == fs.ID)
                                                                             select new FreightPurchaseDetailViewModel
                                                                             {
                                                                                 ID = fsd.ID,
                                                                                 FreightPurchaseID = fsd.FreightPurchaseID,
                                                                                 Amount = fsd.Amount,
                                                                                 AmountWithTax = fsd.AmountWithTax,
                                                                                 FreightID = fsd.FreightID,
                                                                                 Name = fsd.Name,
                                                                                 TaxGroup = fsd.TaxGroup,
                                                                                 TaxGroupID = fsd.TaxGroupID,
                                                                                 TaxGroups = GetTaxGroups(),
                                                                                 TaxGroupSelect = taxGroup.Select(i => new SelectListItem
                                                                                 {
                                                                                     Value = i.ID.ToString(),
                                                                                     Selected = fsd.TaxGroupID == i.ID,
                                                                                     Text = $"{i.Code}-{i.Name}"
                                                                                 }).ToList(),
                                                                                 TaxRate = fsd.TaxRate,
                                                                                 TotalTaxAmount = fsd.TotalTaxAmount
                                                                             }).ToList(),
                                      },
                                      InvoiceNo = $"{docType.Code}-{pod.Number}",
                                      LocalCurID = pod.LocalCurID,
                                      LocalSetRate = (decimal)pod.LocalSetRate,
                                      PostingDate = pod.PostingDate,
                                      Status = pod.Status,
                                      SubTotalAfterDis = pod.SubTotalAfterDis,
                                      SubTotalAfterDisSys = pod.SubTotalAfterDisSys,
                                      TypeDis = pod.TypeDis,
                                      UserID = pod.UserID,
                                      WarehouseID = pod.WarehouseID,
                                      PurchaseOrderID = pod.ID,
                                      AdditionalExpense = (decimal)pod.AdditionalExpense,
                                      AdditionalNote = pod.AdditionalNote,
                                      AppliedAmount = (decimal)pod.AppliedAmount,
                                      AppliedAmountSys = (decimal)pod.AppliedAmountSys,
                                      BalanceDue = (decimal)pod.BalanceDue,
                                      BalanceDueSys = (decimal)pod.BalanceDueSys,
                                      DiscountRate = (decimal)pod.DiscountRate,
                                      DiscountValue = (decimal)pod.DiscountValue,
                                      DocumentTypeID = pod.DocumentTypeID,
                                      DownPayment = (decimal)pod.DownPayment,
                                      DownPaymentSys = (decimal)pod.DownPaymentSys,
                                      Number = pod.Number,
                                      PurCurrencyID = pod.PurCurrencyID,
                                      PurRate = (decimal)pod.PurRate,
                                      ReffNo = pod.ReffNo,
                                      Remark = pod.Remark,
                                      ReturnAmount = (decimal)pod.ReturnAmount,
                                      SeriesDetailID = pod.SeriesDetailID,
                                      SeriesID = pod.SeriesID,
                                      SubTotal = (decimal)pod.SubTotal,
                                      SubTotalSys = (decimal)pod.SubTotalSys,
                                      SysCurrencyID = pod.SysCurrencyID,
                                      TaxRate = (decimal)pod.TaxRate,
                                      TaxValue = (decimal)pod.TaxValue,
                                      VendorID = pod.VendorID,

                                  }).ToListAsync();

            var _tg = new TaxGroupViewModel
            {
                ID = 0,
                Name = "- Select --",
                Code = "",
                Rate = 0,
                Type = 0,
            };
            var tgs = (from t in _context.TaxGroups.Where(i => !i.Delete && i.Active && i.Type == TypeTax.InputTax)
                       let tgds = _context.TaxGroupDefinitions.Where(i => i.TaxGroupID == t.ID).OrderByDescending(i => i.EffectiveFrom).FirstOrDefault() ?? new TaxGroupDefinition()
                       select new TaxGroupViewModel
                       {
                           ID = t.ID,
                           Name = t.Name,
                           Code = t.Code,
                           Effectivefrom = tgds.EffectiveFrom,
                           Rate = tgds.Rate,
                           Type = (int)t.Type,
                       }).ToList();
            tgs.Insert(0, _tg);
            #endregion
            var _purOrderDetails = (from purOr in purOrder
                                    join podd in _context.PurchaseAPReserveDetails on purOr.PurchaseOrderID equals podd.PurchaseAPReserveID
                                    join item in _context.ItemMasterDatas on podd.ItemID equals item.ID
                                    let cur = _context.Currency.FirstOrDefault(i => i.ID == purOr.PurCurrencyID) ?? new Currency()
                                    select new PurchaseOrderDetialViewModel
                                    {
                                        LineIDUN = DateTime.Now.Ticks.ToString(),
                                        LineID = podd.ID,
                                        ItemID = item.ID,
                                        OrderID = item.ID,
                                        Process = item.Process,
                                        Qty = (decimal)podd.OpenQty,
                                        OpenQty = (decimal)podd.OpenQty,
                                        TypeDis = podd.TypeDis,
                                        UomID = podd.UomID,
                                        TaxGroupID = podd.TaxGroupID,
                                        TaxRate = podd.TaxRate,
                                        TaxGroupSelect = tgs.Select(c => new SelectListItem
                                        {
                                            Value = c.ID.ToString(),
                                            Text = $"{c.Code}-{c.Name}",
                                            Selected = c.ID == podd.TaxGroupID
                                        }).ToList(),
                                        TotalWTax = (decimal)podd.TotalWTax,
                                        TaxValue = podd.TaxValue,
                                        Total = (decimal)podd.Total,
                                        Remark = podd.Remark,
                                        /// select List UoM ///
                                        UoMSelect = (from GDU in _context.GroupDUoMs.Where(i => i.GroupUoMID == item.GroupUomID)
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
                                                         Selected = c.ID == podd.UomID
                                                     }).ToList(),
                                        /// List UoM ///
                                        UoMsList = (from GDU in _context.GroupDUoMs.Where(i => i.GroupUoMID == item.GroupUomID)
                                                    join UNM in _context.UnitofMeasures on GDU.AltUOM equals UNM.ID
                                                    select new UOMSViewModel
                                                    {
                                                        BaseUoMID = GDU.BaseUOM,
                                                        Factor = GDU.Factor,
                                                        ID = UNM.ID,
                                                        Name = UNM.Name
                                                    }).ToList(),
                                        TaxGroups = (from t in _context.TaxGroups.Where(i => !i.Delete && i.Active && i.Type == TypeTax.InputTax)
                                                     let tgds = _context.TaxGroupDefinitions.Where(i => i.TaxGroupID == t.ID).OrderByDescending(i => i.EffectiveFrom).FirstOrDefault() ?? new TaxGroupDefinition()
                                                     select new TaxGroupViewModel
                                                     {
                                                         ID = t.ID,
                                                         Name = t.Name,
                                                         Code = t.Code,
                                                         Effectivefrom = tgds.EffectiveFrom,
                                                         Rate = tgds.Rate,
                                                         Type = (int)t.Type,
                                                     }
                                                     ).ToList(),
                                        FinDisRate = podd.FinDisRate,
                                        FinDisValue = podd.FinDisValue,
                                        FinTotalValue = podd.FinTotalValue,
                                        TaxOfFinDisValue = podd.TaxOfFinDisValue,
                                        Barcode = item.Barcode,
                                        Code = item.Code,
                                        CurrencyName = cur.Description,
                                        DiscountRate = (decimal)podd.DiscountRate,
                                        AlertStock = (decimal)podd.AlertStock,
                                        Delete = podd.Delete,
                                        DiscountValue = (decimal)podd.DiscountValue,
                                        ExpireDate = podd.ExpireDate,
                                        ItemName = item.KhmerName,
                                        LocalCurrencyID = podd.LocalCurrencyID,
                                        OldQty = (decimal)podd.Qty,
                                        PurchaseOrderDetailID = podd.ID,
                                        PurchaseOrderID = podd.PurchaseAPReserveID,
                                        PurchasPrice = (decimal)podd.PurchasPrice,
                                        // QuotationID = podd.QuotationID,
                                        TotalWTaxSys = (decimal)podd.TotalWTaxSys,
                                        TotalSys = (decimal)podd.TotalSys,
                                    }).ToList();
            _dataProp.DataProperty(_purOrderDetails, comId, "ItemID", "AddictionProps");
            var data = new PurchaseOrderUpdateViewModel
            {
                PurchaseOrder = purOrder.FirstOrDefault(),
                PurchaseOrderDetials = _purOrderDetails,
            };
            return await Task.FromResult(data);
        }
        public async Task<PurchasePOUpdateViewModel> FindPurchasePOAsync(int seriesID, string number, int comId)
        {
            #region
            var taxGroup = GetTaxGroups();
            taxGroup.Insert(0, new TaxGroupViewModel
            {
                ID = 0,
                Name = "- Select --",
                Code = "",
                Rate = 0,
                Type = 0,
            });
            var purPO = await (from po in _context.GoodsReciptPOs.Where(x => x.Number == number && x.SeriesID == seriesID && x.CompanyID == comId)
                               join docType in _context.DocumentTypes on po.DocumentTypeID equals docType.ID
                               let fs = _context.FreightPurchases.Where(i => i.PurID == po.ID && i.PurType == PurCopyType.GRPO).FirstOrDefault() ?? new FreightPurchase()
                               select new PurchasePOViewModel
                               {
                                   FrieghtAmount = po.FrieghtAmount,
                                   FrieghtAmountSys = po.FrieghtAmountSys,
                                   BranchID = po.BranchID,
                                   CompanyID = po.CompanyID,
                                   DeliveryDate = po.DueDate,
                                   DueDate = po.DueDate,
                                   DocumentDate = po.DocumentDate,
                                   FreightPurchaseView = new FreightPurchaseViewModel
                                   {
                                       ExpenceAmount = fs.ExpenceAmount,
                                       PurID = po.ID,
                                       ID = fs.ID,
                                       PurType = fs.PurType,
                                       TaxSumValue = fs.TaxSumValue,
                                       FreightPurchaseDetailViewModels = (from fsd in _context.FreightPurchaseDetails.Where(i => i.FreightPurchaseID == fs.ID)
                                                                          select new FreightPurchaseDetailViewModel
                                                                          {
                                                                              ID = fsd.ID,
                                                                              FreightPurchaseID = fsd.FreightPurchaseID,
                                                                              Amount = fsd.Amount,
                                                                              AmountWithTax = fsd.AmountWithTax,
                                                                              FreightID = fsd.FreightID,
                                                                              Name = fsd.Name,
                                                                              TaxGroup = fsd.TaxGroup,
                                                                              TaxGroupID = fsd.TaxGroupID,
                                                                              TaxGroups = GetTaxGroups(),
                                                                              TaxGroupSelect = taxGroup.Select(i => new SelectListItem
                                                                              {
                                                                                  Value = i.ID.ToString(),
                                                                                  Selected = fsd.TaxGroupID == i.ID,
                                                                                  Text = $"{i.Code}-{i.Name}"
                                                                              }).ToList(),
                                                                              TaxRate = fsd.TaxRate,
                                                                              TotalTaxAmount = fsd.TotalTaxAmount
                                                                          }).ToList(),
                                   },
                                   InvoiceNo = $"{docType.Code}-{po.Number}",
                                   LocalCurID = po.LocalCurID,
                                   LocalSetRate = (decimal)po.LocalSetRate,
                                   PostingDate = po.PostingDate,
                                   Status = po.Status,
                                   SubTotalAfterDis = po.SubTotalAfterDis,
                                   SubTotalAfterDisSys = po.SubTotalAfterDisSys,
                                   TypeDis = po.TypeDis,
                                   UserID = po.UserID,
                                   WarehouseID = po.WarehouseID,
                                   ID = po.ID,
                                   BaseOnID = po.BaseOnID,
                                   AdditionalExpense = (decimal)po.AdditionalExpense,
                                   AdditionalNote = po.AdditionalNote,
                                   AppliedAmount = (decimal)po.AppliedAmount,
                                   AppliedAmountSys = (decimal)po.AppliedAmountSys,
                                   BalanceDue = (decimal)po.BalanceDue,
                                   BalanceDueSys = (decimal)po.BalanceDueSys,
                                   DiscountRate = (decimal)po.DiscountRate,
                                   DiscountValue = (decimal)po.DiscountValue,
                                   DocumentTypeID = po.DocumentTypeID,
                                   DownPayment = (decimal)po.DownPayment,
                                   DownPaymentSys = (decimal)po.DownPaymentSys,
                                   Number = po.Number,
                                   PurCurrencyID = po.PurCurrencyID,
                                   PurRate = (decimal)po.PurRate,
                                   ReffNo = po.ReffNo,
                                   Remark = po.Remark,
                                   ReturnAmount = (decimal)po.ReturnAmount,
                                   SeriesDetailID = po.SeriesDetailID,
                                   SeriesID = po.SeriesID,
                                   SubTotal = (decimal)po.SubTotal,
                                   SubTotalSys = (decimal)po.SubTotalSys,
                                   SysCurrencyID = po.SysCurrencyID,
                                   TaxRate = (decimal)po.TaxRate,
                                   TaxValue = (decimal)po.TaxValue,
                                   VendorID = po.VendorID
                               }).ToListAsync();

            var _tg = new TaxGroupViewModel
            {
                ID = 0,
                Name = "- Select --",
                Code = "",
                Rate = 0,
                Type = 0,
            };
            var tgs = (from t in _context.TaxGroups.Where(i => !i.Delete && i.Active && i.Type == TypeTax.InputTax)
                       let tgds = _context.TaxGroupDefinitions.Where(i => i.TaxGroupID == t.ID).OrderByDescending(i => i.EffectiveFrom).FirstOrDefault() ?? new TaxGroupDefinition()
                       select new TaxGroupViewModel
                       {
                           ID = t.ID,
                           Name = t.Name,
                           Code = t.Code,
                           Effectivefrom = tgds.EffectiveFrom,
                           Rate = tgds.Rate,
                           Type = (int)t.Type,
                       }).ToList();
            tgs.Insert(0, _tg);
            #endregion
            var _grdd = (from po in purPO
                         join grd in _context.GoodReciptPODetails on po.ID equals grd.GoodsReciptPOID
                         join item in _context.ItemMasterDatas on grd.ItemID equals item.ID
                         let cur = _context.Currency.FirstOrDefault(i => i.ID == po.PurCurrencyID) ?? new Currency()
                         select new PurchasePODetialViewModel
                         {
                             LineIDUN = DateTime.Now.Ticks.ToString(),
                             LineID = grd.ID,
                             ItemID = item.ID,
                             Process = item.Process,
                             Qty = (decimal)grd.Qty,
                             OpenQty = (decimal)grd.OpenQty,
                             TypeDis = grd.TypeDis,
                             UomID = grd.UomID,
                             TaxGroupID = grd.TaxGroupID,
                             TaxRate = grd.TaxRate,
                             TaxGroupSelect = tgs.Select(c => new SelectListItem
                             {
                                 Value = c.ID.ToString(),
                                 Text = $"{c.Code}-{c.Name}",
                                 Selected = c.ID == grd.TaxGroupID
                             }).ToList(),
                             TotalWTax = (decimal)grd.TotalWTax,
                             TaxValue = grd.TaxValue,
                             Total = (decimal)grd.Total,
                             Remark = grd.Remark,
                             /// select List UoM ///
                             UoMSelect = (from GDU in _context.GroupDUoMs.Where(i => i.GroupUoMID == item.GroupUomID)
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
                                              Selected = c.ID == grd.UomID
                                          }).ToList(),
                             /// List UoM ///
                             UoMsList = (from GDU in _context.GroupDUoMs.Where(i => i.GroupUoMID == item.GroupUomID)
                                         join UNM in _context.UnitofMeasures on GDU.AltUOM equals UNM.ID
                                         select new UOMSViewModel
                                         {
                                             BaseUoMID = GDU.BaseUOM,
                                             Factor = GDU.Factor,
                                             ID = UNM.ID,
                                             Name = UNM.Name
                                         }).ToList(),
                             TaxGroups = (from t in _context.TaxGroups.Where(i => !i.Delete && i.Active && i.Type == TypeTax.InputTax)
                                          let tgds = _context.TaxGroupDefinitions.Where(i => i.TaxGroupID == t.ID).OrderByDescending(i => i.EffectiveFrom).FirstOrDefault() ?? new TaxGroupDefinition()
                                          select new TaxGroupViewModel
                                          {
                                              ID = t.ID,
                                              Name = t.Name,
                                              Code = t.Code,
                                              Effectivefrom = tgds.EffectiveFrom,
                                              Rate = tgds.Rate,
                                              Type = (int)t.Type,
                                          }
                                          ).ToList(),
                             FinDisRate = grd.FinDisRate,
                             FinDisValue = grd.FinDisValue,
                             FinTotalValue = grd.FinTotalValue,
                             TaxOfFinDisValue = grd.TaxOfFinDisValue,
                             Barcode = item.Barcode,
                             Code = item.Code,
                             CurrencyName = cur.Description,
                             DiscountRate = (decimal)grd.DiscountRate,
                             AlertStock = (decimal)grd.AlertStock,
                             Delete = grd.Delete,
                             DiscountValue = (decimal)grd.DiscountValue,
                             ExpireDate = grd.ExpireDate,
                             ItemName = item.KhmerName,
                             LocalCurrencyID = grd.LocalCurrencyID,
                             ID = grd.ID,
                             GoodsReciptPOID = grd.GoodsReciptPOID,
                             PurchasPrice = (decimal)grd.PurchasPrice,
                             TotalWTaxSys = (decimal)grd.TotalWTaxSys,
                             TotalSys = (decimal)grd.TotalSys,
                             PurCopyType = grd.PurCopyType,
                         }).ToList();//GroupBy(i => i.ItemID).Select(i => i.FirstOrDefault()).ToList();
            _dataProp.DataProperty(_grdd, comId, "ItemID", "AddictionProps");
            var data = new PurchasePOUpdateViewModel
            {
                PurchasePO = purPO.FirstOrDefault(),
                PurchasePODetials = _grdd
            };
            return await Task.FromResult(data);
        }

        public async Task<PurchaseAPUpdateViewModel> FindPurchaseAPAsync(int seriesID, string number, int comId)
        {
            #region
            var taxGroup = GetTaxGroups();
            taxGroup.Insert(0, new TaxGroupViewModel
            {
                ID = 0,
                Name = "- Select --",
                Code = "",
                Rate = 0,
                Type = 0,
            });
            var purAP = await (from ap in _context.Purchase_APs.Where(x => x.Number == number && x.SeriesID == seriesID && x.CompanyID == comId)
                               join docType in _context.DocumentTypes on ap.DocumentTypeID equals docType.ID
                               let fs = _context.FreightPurchases.Where(i => i.PurID == ap.PurchaseAPID && i.PurType == PurCopyType.PurAP).FirstOrDefault() ?? new FreightPurchase()
                               select new PurchaseAPViewModel
                               {
                                   FrieghtAmount = ap.FrieghtAmount,
                                   FrieghtAmountSys = ap.FrieghtAmountSys,
                                   BranchID = ap.BranchID,
                                   CompanyID = ap.CompanyID,
                                   DeliveryDate = ap.DueDate,
                                   DueDate = ap.DueDate,
                                   DocumentDate = ap.DocumentDate,
                                   FreightPurchaseView = new FreightPurchaseViewModel
                                   {
                                       ExpenceAmount = fs.ExpenceAmount,
                                       PurID = ap.PurchaseAPID,
                                       ID = fs.ID,
                                       PurType = fs.PurType,
                                       TaxSumValue = fs.TaxSumValue,
                                       FreightPurchaseDetailViewModels = (from fsd in _context.FreightPurchaseDetails.Where(i => i.FreightPurchaseID == fs.ID)
                                                                          select new FreightPurchaseDetailViewModel
                                                                          {
                                                                              ID = fsd.ID,
                                                                              FreightPurchaseID = fsd.FreightPurchaseID,
                                                                              Amount = fsd.Amount,
                                                                              AmountWithTax = fsd.AmountWithTax,
                                                                              FreightID = fsd.FreightID,
                                                                              Name = fsd.Name,
                                                                              TaxGroup = fsd.TaxGroup,
                                                                              TaxGroupID = fsd.TaxGroupID,
                                                                              TaxGroups = GetTaxGroups(),
                                                                              TaxGroupSelect = taxGroup.Select(i => new SelectListItem
                                                                              {
                                                                                  Value = i.ID.ToString(),
                                                                                  Selected = fsd.TaxGroupID == i.ID,
                                                                                  Text = $"{i.Code}-{i.Name}"
                                                                              }).ToList(),
                                                                              TaxRate = fsd.TaxRate,
                                                                              TotalTaxAmount = fsd.TotalTaxAmount
                                                                          }).ToList(),
                                   },
                                   InvoiceNo = $"{docType.Code}-{ap.Number}",
                                   LocalCurID = ap.LocalCurID,
                                   LocalSetRate = (decimal)ap.LocalSetRate,
                                   PostingDate = ap.PostingDate,
                                   Status = ap.Status,
                                   SubTotalAfterDis = ap.SubTotalAfterDis,
                                   SubTotalAfterDisSys = ap.SubTotalAfterDisSys,
                                   TypeDis = ap.TypeDis,
                                   UserID = ap.UserID,
                                   WarehouseID = ap.WarehouseID,
                                   PurchaseAPID = ap.PurchaseAPID,
                                   BaseOnID = ap.PurchaseAPID,
                                   AdditionalExpense = (decimal)ap.AdditionalExpense,
                                   AdditionalNote = ap.AdditionalNote,
                                   AppliedAmount = (decimal)ap.AppliedAmount,
                                   AppliedAmountSys = (decimal)ap.AppliedAmountSys,
                                   BalanceDue = (decimal)ap.BalanceDue,
                                   BalanceDueSys = (decimal)ap.BalanceDueSys,
                                   DiscountRate = (decimal)ap.DiscountRate,
                                   DiscountValue = (decimal)ap.DiscountValue,
                                   DocumentTypeID = ap.DocumentTypeID,
                                   DownPayment = (decimal)ap.DownPayment,
                                   DownPaymentSys = (decimal)ap.DownPaymentSys,
                                   Number = ap.Number,
                                   PurCurrencyID = ap.PurCurrencyID,
                                   PurRate = (decimal)ap.PurRate,
                                   ReffNo = ap.ReffNo,
                                   Remark = ap.Remark,
                                   ReturnAmount = (decimal)ap.ReturnAmount,
                                   SeriesDetailID = ap.SeriesDetailID,
                                   SeriesID = ap.SeriesID,
                                   SubTotal = (decimal)ap.SubTotal,
                                   SubTotalSys = (decimal)ap.SubTotalSys,
                                   SysCurrencyID = ap.SysCurrencyID,
                                   TaxRate = (decimal)ap.TaxRate,
                                   TaxValue = (decimal)ap.TaxValue,
                                   VendorID = ap.VendorID
                               }).ToListAsync();

            var _tg = new TaxGroupViewModel
            {
                ID = 0,
                Name = "- Select --",
                Code = "",
                Rate = 0,
                Type = 0,
            };
            var tgs = (from t in _context.TaxGroups.Where(i => !i.Delete && i.Active && i.Type == TypeTax.InputTax)
                       let tgds = _context.TaxGroupDefinitions.Where(i => i.TaxGroupID == t.ID).OrderByDescending(i => i.EffectiveFrom).FirstOrDefault() ?? new TaxGroupDefinition()
                       select new TaxGroupViewModel
                       {
                           ID = t.ID,
                           Name = t.Name,
                           Code = t.Code,
                           Effectivefrom = tgds.EffectiveFrom,
                           Rate = tgds.Rate,
                           Type = (int)t.Type,
                       }).ToList();
            tgs.Insert(0, _tg);
            #endregion
            var __APd = (from ap in purAP
                         join apd in _context.PurchaseAPDetail on ap.PurchaseAPID equals apd.PurchaseAPID
                         join item in _context.ItemMasterDatas on apd.ItemID equals item.ID
                         let cur = _context.Currency.FirstOrDefault(i => i.ID == ap.PurCurrencyID) ?? new Currency()
                         select new PurchaseAPDetialViewModel
                         {
                             LineIDUN = DateTime.Now.Ticks.ToString(),
                             BaseOnID = apd.PurchaseDetailAPID,
                             LineID = apd.LineID,
                             PurCopyType = apd.PurCopyType,
                             ItemID = item.ID,
                             Process = item.Process,
                             Qty = (decimal)apd.Qty,
                             OpenQty = (decimal)apd.OpenQty,
                             TypeDis = apd.TypeDis,
                             UomID = apd.UomID,
                             TaxGroupID = apd.TaxGroupID,
                             TaxRate = apd.TaxRate,
                             TaxGroupSelect = tgs.Select(c => new SelectListItem
                             {
                                 Value = c.ID.ToString(),
                                 Text = $"{c.Code}-{c.Name}",
                                 Selected = c.ID == apd.TaxGroupID
                             }).ToList(),
                             TotalWTax = (decimal)apd.TotalWTax,
                             TaxValue = apd.TaxValue,
                             Total = (decimal)apd.Total,
                             Remark = apd.Remark,
                             /// select List UoM ///
                             UoMSelect = (from GDU in _context.GroupDUoMs.Where(i => i.GroupUoMID == item.GroupUomID)
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
                                              Selected = c.ID == apd.UomID
                                          }).ToList(),
                             /// List UoM ///
                             UoMsList = (from GDU in _context.GroupDUoMs.Where(i => i.GroupUoMID == item.GroupUomID)
                                         join UNM in _context.UnitofMeasures on GDU.AltUOM equals UNM.ID
                                         select new UOMSViewModel
                                         {
                                             BaseUoMID = GDU.BaseUOM,
                                             Factor = GDU.Factor,
                                             ID = UNM.ID,
                                             Name = UNM.Name
                                         }).ToList(),
                             TaxGroups = (from t in _context.TaxGroups.Where(i => !i.Delete && i.Active && i.Type == TypeTax.InputTax)
                                          let tgds = _context.TaxGroupDefinitions.Where(i => i.TaxGroupID == t.ID).OrderByDescending(i => i.EffectiveFrom).FirstOrDefault() ?? new TaxGroupDefinition()
                                          select new TaxGroupViewModel
                                          {
                                              ID = t.ID,
                                              Name = t.Name,
                                              Code = t.Code,
                                              Effectivefrom = tgds.EffectiveFrom,
                                              Rate = tgds.Rate,
                                              Type = (int)t.Type,
                                          }
                                          ).ToList(),
                             FinDisRate = apd.FinDisRate,
                             FinDisValue = apd.FinDisValue,
                             FinTotalValue = apd.FinTotalValue,
                             TaxOfFinDisValue = apd.TaxOfFinDisValue,
                             Barcode = item.Barcode,
                             Code = item.Code,
                             CurrencyName = cur.Description,
                             DiscountRate = (decimal)apd.DiscountRate,
                             AlertStock = (decimal)apd.AlertStock,
                             Delete = apd.Delete,
                             DiscountValue = (decimal)apd.DiscountValue,
                             ExpireDate = apd.ExpireDate,
                             ItemName = item.KhmerName,
                             LocalCurrencyID = apd.LocalCurrencyID,
                             PurchaseDetailAPID = apd.PurchaseDetailAPID,
                             PurchaseAPID = apd.PurchaseAPID,
                             PurchasPrice = (decimal)apd.PurchasPrice,
                             TotalWTaxSys = (decimal)apd.TotalWTaxSys,
                             TotalSys = (decimal)apd.TotalSys,
                         }).ToList();
            _dataProp.DataProperty(__APd, comId, "ItemID", "AddictionProps");
            var data = new PurchaseAPUpdateViewModel
            {
                PurchaseAP = purAP.FirstOrDefault(),
                PurchaseAPDetials = __APd
            };
            return await Task.FromResult(data);
        }
        public async Task<PurchaseCreditMemoUpdateViewModel> FindPurchaseCreditMemoAsync(int seriesID, string number, int comId)
        {
            #region
            var taxGroup = GetTaxGroups();
            taxGroup.Insert(0, new TaxGroupViewModel
            {
                ID = 0,
                Name = "- Select --",
                Code = "",
                Rate = 0,
                Type = 0,
            });
            var purAPMemo = await (from ap in _context.PurchaseCreditMemos.Where(x => x.Number == number && x.SeriesID == seriesID && x.CompanyID == comId)
                                   join docType in _context.DocumentTypes on ap.DocumentTypeID equals docType.ID
                                   let fs = _context.FreightPurchases.Where(i => i.PurID == ap.PurchaseMemoID && i.PurType == PurCopyType.PurCreditMemo).FirstOrDefault() ?? new FreightPurchase()
                                   select new PurchaseCreditMemoViewModel
                                   {
                                       FrieghtAmount = ap.FrieghtAmount,
                                       FrieghtAmountSys = ap.FrieghtAmountSys,
                                       BranchID = ap.BranchID,
                                       CompanyID = ap.CompanyID,
                                       DeliveryDate = ap.DueDate,
                                       DueDate = ap.DueDate,
                                       DocumentDate = ap.DocumentDate,
                                       FreightPurchaseView = new FreightPurchaseViewModel
                                       {
                                           ExpenceAmount = fs.ExpenceAmount,
                                           PurID = ap.PurchaseMemoID,
                                           ID = fs.ID,
                                           PurType = fs.PurType,
                                           TaxSumValue = fs.TaxSumValue,
                                           FreightPurchaseDetailViewModels = (from fsd in _context.FreightPurchaseDetails.Where(i => i.FreightPurchaseID == fs.ID)
                                                                              select new FreightPurchaseDetailViewModel
                                                                              {
                                                                                  ID = fsd.ID,
                                                                                  FreightPurchaseID = fsd.FreightPurchaseID,
                                                                                  Amount = fsd.Amount,
                                                                                  AmountWithTax = fsd.AmountWithTax,
                                                                                  FreightID = fsd.FreightID,
                                                                                  Name = fsd.Name,
                                                                                  TaxGroup = fsd.TaxGroup,
                                                                                  TaxGroupID = fsd.TaxGroupID,
                                                                                  TaxGroups = GetTaxGroups(),
                                                                                  TaxGroupSelect = taxGroup.Select(i => new SelectListItem
                                                                                  {
                                                                                      Value = i.ID.ToString(),
                                                                                      Selected = fsd.TaxGroupID == i.ID,
                                                                                      Text = $"{i.Code}-{i.Name}"
                                                                                  }).ToList(),
                                                                                  TaxRate = fsd.TaxRate,
                                                                                  TotalTaxAmount = fsd.TotalTaxAmount
                                                                              }).ToList(),
                                       },
                                       InvoiceNo = $"{docType.Code}-{ap.Number}",
                                       LocalCurID = ap.LocalCurID,
                                       LocalSetRate = (decimal)ap.LocalSetRate,
                                       PostingDate = ap.PostingDate,
                                       Status = ap.Status,
                                       SubTotalAfterDis = ap.SubTotalAfterDis,
                                       SubTotalAfterDisSys = ap.SubTotalAfterDisSys,
                                       TypeDis = ap.TypeDis,
                                       UserID = ap.UserID,
                                       WarehouseID = ap.WarehouseID,
                                       PurchaseMemoID = ap.PurchaseMemoID,
                                       BaseOnID = ap.PurchaseMemoID,
                                       AdditionalExpense = (decimal)ap.AdditionalExpense,
                                       AdditionalNote = ap.AdditionalNote,
                                       AppliedAmount = (decimal)ap.AppliedAmount,
                                       AppliedAmountSys = (decimal)ap.AppliedAmountSys,
                                       BalanceDue = (decimal)ap.BalanceDue,
                                       BalanceDueSys = (decimal)ap.BalanceDueSys,
                                       DiscountRate = (decimal)ap.DiscountRate,
                                       DiscountValue = (decimal)ap.DiscountValue,
                                       DocumentTypeID = ap.DocumentTypeID,
                                       DownPayment = (decimal)ap.DownPayment,
                                       DownPaymentSys = (decimal)ap.DownPaymentSys,
                                       Number = ap.Number,
                                       PurCurrencyID = ap.PurCurrencyID,
                                       PurRate = (decimal)ap.PurRate,
                                       ReffNo = ap.ReffNo,
                                       Remark = ap.Remark,
                                       ReturnAmount = (decimal)ap.ReturnAmount,
                                       SeriesDetailID = ap.SeriesDetailID,
                                       SeriesID = ap.SeriesID,
                                       SubTotal = (decimal)ap.SubTotal,
                                       SubTotalSys = (decimal)ap.SubTotalSys,
                                       SysCurrencyID = ap.SysCurrencyID,
                                       TaxRate = (decimal)ap.TaxRate,
                                       TaxValue = (decimal)ap.TaxValue,
                                       VendorID = ap.VendorID
                                   }).ToListAsync();

            var _tg = new TaxGroupViewModel
            {
                ID = 0,
                Name = "- Select --",
                Code = "",
                Rate = 0,
                Type = 0,
            };
            var tgs = (from t in _context.TaxGroups.Where(i => !i.Delete && i.Active && i.Type == TypeTax.InputTax)
                       let tgds = _context.TaxGroupDefinitions.Where(i => i.TaxGroupID == t.ID).OrderByDescending(i => i.EffectiveFrom).FirstOrDefault() ?? new TaxGroupDefinition()
                       select new TaxGroupViewModel
                       {
                           ID = t.ID,
                           Name = t.Name,
                           Code = t.Code,
                           Effectivefrom = tgds.EffectiveFrom,
                           Rate = tgds.Rate,
                           Type = (int)t.Type,
                       }).ToList();
            tgs.Insert(0, _tg);
            #endregion
            var __APmemod = (from ap in purAPMemo
                             join apd in _context.PurchaseCreditMemoDetails on ap.PurchaseMemoID equals apd.PurchaseCreditMemoID
                             join item in _context.ItemMasterDatas on apd.ItemID equals item.ID
                             let cur = _context.Currency.FirstOrDefault(i => i.ID == ap.PurCurrencyID) ?? new Currency()
                             select new PurchaseCreditMemoDetialViewModel
                             {
                                 LineIDUN = DateTime.Now.Ticks.ToString(),
                                 LineID = apd.PurchaseCreditMemoID,
                                 ItemID = item.ID,
                                 Process = item.Process,
                                 Qty = (decimal)apd.Qty,
                                 //OpenQty = (decimal)apd.OpenQty,
                                 TypeDis = apd.TypeDis,
                                 UomID = apd.UomID,
                                 TaxGroupID = apd.TaxGroupID,
                                 TaxRate = apd.TaxRate,
                                 TaxGroupSelect = tgs.Select(c => new SelectListItem
                                 {
                                     Value = c.ID.ToString(),
                                     Text = $"{c.Code}-{c.Name}",
                                     Selected = c.ID == apd.TaxGroupID
                                 }).ToList(),
                                 TotalWTax = (decimal)apd.TotalWTax,
                                 TaxValue = apd.TaxValue,
                                 Total = (decimal)apd.Total,
                                 Remark = apd.Remark,
                                 /// select List UoM ///
                                 UoMSelect = (from GDU in _context.GroupDUoMs.Where(i => i.GroupUoMID == item.GroupUomID)
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
                                                  Selected = c.ID == apd.UomID
                                              }).ToList(),
                                 /// List UoM ///
                                 UoMsList = (from GDU in _context.GroupDUoMs.Where(i => i.GroupUoMID == item.GroupUomID)
                                             join UNM in _context.UnitofMeasures on GDU.AltUOM equals UNM.ID
                                             select new UOMSViewModel
                                             {
                                                 BaseUoMID = GDU.BaseUOM,
                                                 Factor = GDU.Factor,
                                                 ID = UNM.ID,
                                                 Name = UNM.Name
                                             }).ToList(),
                                 TaxGroups = (from t in _context.TaxGroups.Where(i => !i.Delete && i.Active && i.Type == TypeTax.InputTax)
                                              let tgds = _context.TaxGroupDefinitions.Where(i => i.TaxGroupID == t.ID).OrderByDescending(i => i.EffectiveFrom).FirstOrDefault() ?? new TaxGroupDefinition()
                                              select new TaxGroupViewModel
                                              {
                                                  ID = t.ID,
                                                  Name = t.Name,
                                                  Code = t.Code,
                                                  Effectivefrom = tgds.EffectiveFrom,
                                                  Rate = tgds.Rate,
                                                  Type = (int)t.Type,
                                              }
                                              ).ToList(),
                                 FinDisRate = apd.FinDisRate,
                                 FinDisValue = apd.FinDisValue,
                                 FinTotalValue = apd.FinTotalValue,
                                 TaxOfFinDisValue = apd.TaxOfFinDisValue,
                                 Barcode = item.Barcode,
                                 Code = item.Code,
                                 CurrencyName = cur.Description,
                                 DiscountRate = (decimal)apd.DiscountRate,
                                 AlertStock = (decimal)apd.AlertStock,
                                 //Delete = apd.Delete,
                                 DiscountValue = (decimal)apd.DiscountValue,
                                 ExpireDate = apd.ExpireDate,
                                 ItemName = item.KhmerName,
                                 LocalCurrencyID = apd.LocalCurrencyID,
                                 PurchaseMemoDetailID = apd.PurchaseMemoDetailID,
                                 PurchaseCreditMemoID = apd.PurchaseCreditMemoID,
                                 PurchasPrice = (decimal)apd.PurchasPrice,
                                 TotalWTaxSys = (decimal)apd.TotalWTaxSys,
                                 TotalSys = (decimal)apd.TotalSys,
                             }).ToList();//GroupBy(i => i.ItemID).Select(i => i.FirstOrDefault()).ToList();
            _dataProp.DataProperty(__APmemod, comId, "ItemID", "AddictionProps");
            var data = new PurchaseCreditMemoUpdateViewModel
            {
                PurchaseCreditMemo = purAPMemo.FirstOrDefault(),
                PurchaseCreditMemoDetials = __APmemod
            };
            return await Task.FromResult(data);
        }
        public async Task<PurchaseOrderUpdateViewModel> FindPurchaseRequestAsync(int seriesID, string number, int comId)
        {
            #region
            var taxGroup = GetTaxGroups();
            taxGroup.Insert(0, new TaxGroupViewModel
            {
                ID = 0,
                Name = "- Select --",
                Code = "",
                Rate = 0,
                Type = 0,
            });
            var purOrder = await (from pod in _context.PurchaseRequests.Where(x => x.Number == number && x.SeriesID == seriesID && x.CompanyID == comId)
                                  join docType in _context.DocumentTypes on pod.DocumentTypeID equals docType.ID
                                  join req in _context.UserAccounts.Include(i => i.Employee) on pod.RequesterID equals req.ID
                                  let fs = _context.FreightPurchases.Where(i => i.PurID == pod.ID && i.PurType == PurCopyType.PurRequest).FirstOrDefault() ?? new FreightPurchase()
                                  select new PurchaseOrderViewModel
                                  {
                                      ID = pod.ID,
                                      FrieghtAmount = pod.FrieghtAmount,
                                      FrieghtAmountSys = pod.FrieghtAmountSys,
                                      BranchID = pod.BranchID,
                                      CompanyID = pod.CompanyID,
                                      BaseOnID = pod.ID,
                                      DeliveryDate = pod.DeliveryDate,
                                      DocumentDate = pod.DocumentDate,
                                      RequesterCode = req.Employee.Code,
                                      RequesterName = req.Employee.Name,
                                      RequesterUsername = req.Username,
                                      FreightPurchaseView = new FreightPurchaseViewModel
                                      {
                                          ExpenceAmount = fs.ExpenceAmount,
                                          PurID = pod.ID,
                                          ID = fs.ID,
                                          PurType = fs.PurType,
                                          TaxSumValue = fs.TaxSumValue,
                                          FreightPurchaseDetailViewModels = (from fsd in _context.FreightPurchaseDetails.Where(i => i.FreightPurchaseID == fs.ID)
                                                                             select new FreightPurchaseDetailViewModel
                                                                             {
                                                                                 ID = fsd.ID,
                                                                                 FreightPurchaseID = fsd.FreightPurchaseID,
                                                                                 Amount = fsd.Amount,
                                                                                 AmountWithTax = fsd.AmountWithTax,
                                                                                 FreightID = fsd.FreightID,
                                                                                 Name = fsd.Name,
                                                                                 TaxGroup = fsd.TaxGroup,
                                                                                 TaxGroupID = fsd.TaxGroupID,
                                                                                 TaxGroups = GetTaxGroups(),
                                                                                 TaxGroupSelect = taxGroup.Select(i => new SelectListItem
                                                                                 {
                                                                                     Value = i.ID.ToString(),
                                                                                     Selected = fsd.TaxGroupID == i.ID,
                                                                                     Text = $"{i.Code}-{i.Name}"
                                                                                 }).ToList(),
                                                                                 TaxRate = fsd.TaxRate,
                                                                                 TotalTaxAmount = fsd.TotalTaxAmount
                                                                             }).ToList(),
                                      },
                                      InvoiceNo = $"{docType.Code}-{pod.Number}",
                                      LocalCurID = pod.LocalCurID,
                                      LocalSetRate = (decimal)pod.LocalSetRate,
                                      PostingDate = pod.PostingDate,
                                      Status = pod.Status,
                                      SubTotalAfterDis = pod.SubTotalAfterDis,
                                      SubTotalAfterDisSys = pod.SubTotalAfterDisSys,
                                      UserID = pod.UserID,
                                      WarehouseID = 0,
                                      RequesterID = pod.RequesterID,
                                      PurchaseOrderID = pod.ID,
                                      AdditionalExpense = (decimal)pod.AdditionalExpense,
                                      AdditionalNote = pod.AdditionalNote,
                                      AppliedAmount = (decimal)pod.AppliedAmount,
                                      AppliedAmountSys = (decimal)pod.AppliedAmountSys,
                                      BalanceDue = (decimal)pod.BalanceDue,
                                      BalanceDueSys = (decimal)pod.BalanceDueSys,
                                      DiscountRate = (decimal)pod.DiscountRate,
                                      DiscountValue = (decimal)pod.DiscountValue,
                                      DocumentTypeID = pod.DocumentTypeID,
                                      DownPayment = (decimal)pod.DownPayment,
                                      DownPaymentSys = (decimal)pod.DownPaymentSys,
                                      Number = pod.Number,
                                      PurCurrencyID = pod.PurCurrencyID,
                                      PurRate = (decimal)pod.PurRate,
                                      ReffNo = pod.ReffNo,
                                      Remark = pod.Remark,
                                      ReturnAmount = (decimal)pod.ReturnAmount,
                                      SeriesDetailID = pod.SeriesDetailID,
                                      SeriesID = pod.SeriesID,
                                      SubTotal = (decimal)pod.SubTotal,
                                      SubTotalSys = (decimal)pod.SubTotalSys,
                                      SysCurrencyID = pod.SysCurrencyID,
                                      TaxRate = (decimal)pod.TaxRate,
                                      TaxValue = (decimal)pod.TaxValue,
                                      VendorID = pod.RequesterID
                                  }).FirstOrDefaultAsync() ?? new PurchaseOrderViewModel();

            var _tg = new TaxGroupViewModel
            {
                ID = 0,
                Name = "- Select --",
                Code = "",
                Rate = 0,
                Type = 0,
            };
            var tgs = (from t in _context.TaxGroups.Where(i => !i.Delete && i.Active && i.Type == TypeTax.InputTax)
                       let tgds = _context.TaxGroupDefinitions.Where(i => i.TaxGroupID == t.ID).OrderByDescending(i => i.EffectiveFrom).FirstOrDefault() ?? new TaxGroupDefinition()
                       select new TaxGroupViewModel
                       {
                           ID = t.ID,
                           Name = t.Name,
                           Code = t.Code,
                           Effectivefrom = tgds.EffectiveFrom,
                           Rate = tgds.Rate,
                           Type = (int)t.Type,
                       }).ToList();
            tgs.Insert(0, _tg);
            #endregion
            var _purOrderDetails = (from podd in _context.PurchaseRequestDetails.Where(s => s.PurchaseRequestID == purOrder.PurchaseOrderID) //on purOr.PurchaseOrderID equals podd.ID
                                    join item in _context.ItemMasterDatas on podd.ItemID equals item.ID
                                    let vendor = _context.BusinessPartners.FirstOrDefault(i => i.ID == podd.VendorID) ?? new BusinessPartner()
                                    let cur = _context.Currency.FirstOrDefault(i => i.ID == purOrder.PurCurrencyID) ?? new Currency()
                                    select new PurchaseOrderDetialViewModel
                                    {
                                        LineIDUN = podd.ID + "" + DateTime.Now.Ticks.ToString(),
                                        LineID = podd.ID,
                                        ItemID = item.ID,
                                        OrderID = item.ID,
                                        Process = item.Process,
                                        Qty = (decimal)podd.Qty,
                                        OpenQty = (decimal)podd.OpenQty,
                                        UomID = podd.UomID,
                                        TaxGroupID = podd.TaxGroupID,
                                        TaxRate = podd.TaxRate,
                                        TaxGroupSelect = tgs.Select(c => new SelectListItem
                                        {
                                            Value = c.ID.ToString(),
                                            Text = $"{c.Code}-{c.Name}",
                                            Selected = c.ID == podd.TaxGroupID
                                        }).ToList(),
                                        TotalWTax = (decimal)podd.TotalWTax,
                                        TaxValue = podd.TaxValue,
                                        Total = (decimal)podd.Total,
                                        Remark = podd.Remark,
                                        /// select List UoM ///
                                        UoMSelect = (from GDU in _context.GroupDUoMs.Where(i => i.GroupUoMID == item.GroupUomID)
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
                                                         Selected = c.ID == podd.UomID
                                                     }).ToList(),
                                        /// List UoM ///
                                        UoMsList = (from GDU in _context.GroupDUoMs.Where(i => i.GroupUoMID == item.GroupUomID)
                                                    join UNM in _context.UnitofMeasures on GDU.AltUOM equals UNM.ID
                                                    select new UOMSViewModel
                                                    {
                                                        BaseUoMID = GDU.BaseUOM,
                                                        Factor = GDU.Factor,
                                                        ID = UNM.ID,
                                                        Name = UNM.Name
                                                    }).ToList(),
                                        TaxGroups = (from t in _context.TaxGroups.Where(i => !i.Delete && i.Active && i.Type == TypeTax.InputTax)
                                                     let tgds = _context.TaxGroupDefinitions.Where(i => i.TaxGroupID == t.ID).OrderByDescending(i => i.EffectiveFrom).FirstOrDefault() ?? new TaxGroupDefinition()
                                                     select new TaxGroupViewModel
                                                     {
                                                         ID = t.ID,
                                                         Name = t.Name,
                                                         Code = t.Code,
                                                         Effectivefrom = tgds.EffectiveFrom,
                                                         Rate = tgds.Rate,
                                                         Type = (int)t.Type,
                                                     }
                                                     ).ToList(),
                                        PurchaseOrderDetailID = podd.ID,
                                        FinDisRate = podd.FinDisRate,
                                        FinDisValue = podd.FinDisValue,
                                        FinTotalValue = podd.FinTotalValue,
                                        TaxOfFinDisValue = podd.TaxOfFinDisValue,
                                        Barcode = item.Barcode,
                                        Code = item.Code,
                                        CurrencyName = cur.Description,
                                        DiscountRate = (decimal)podd.DiscountRate,
                                        AlertStock = (decimal)podd.AlertStock,
                                        Delete = podd.Delete,
                                        DiscountValue = (decimal)podd.DiscountValue,
                                        ExpireDate = podd.ExpireDate,
                                        ItemName = item.KhmerName,
                                        LocalCurrencyID = podd.LocalCurrencyID,
                                        OldQty = (decimal)podd.OldQty,
                                        ID = podd.ID,
                                        VendorCode = vendor.Code,
                                        VendorName = vendor.Name,
                                        VendorID = vendor.ID,
                                        PurchaseOrderID = podd.PurchaseRequestID,
                                        PurchasPrice = (decimal)podd.PurchasPrice,
                                        QuotationID = podd.QuotationID,
                                        TotalWTaxSys = (decimal)podd.TotalWTaxSys,
                                        TotalSys = (decimal)podd.TotalSys,
                                    }).ToList() ?? new List<PurchaseOrderDetialViewModel>();
            _dataProp.DataProperty(_purOrderDetails, comId, "ItemID", "AddictionProps");
            var data = new PurchaseOrderUpdateViewModel
            {
                PurchaseOrder = purOrder,
                PurchaseOrderDetials = _purOrderDetails,
            };
            return await Task.FromResult(data);
        }

        public async Task<PurchaseOrderUpdateViewModel> FindPurchaseQuotationAsync(int seriesID, string number, int comId)
        {
            #region
            var taxGroup = GetTaxGroups();
            taxGroup.Insert(0, new TaxGroupViewModel
            {
                ID = 0,
                Name = "- Select --",
                Code = "",
                Rate = 0,
                Type = 0,
            });
            var purOrder = await (from pod in _context.PurchaseQuotations.Where(x => x.Number == number && x.SeriesID == seriesID && x.CompanyID == comId)
                                  join docType in _context.DocumentTypes on pod.DocumentTypeID equals docType.ID
                                  let fs = _context.FreightPurchases.Where(i => i.PurID == pod.ID && i.PurType == PurCopyType.PurQuote).FirstOrDefault() ?? new FreightPurchase()
                                  select new PurchaseOrderViewModel
                                  {
                                      ID = pod.ID,
                                      FrieghtAmount = pod.FrieghtAmount,
                                      FrieghtAmountSys = pod.FrieghtAmountSys,
                                      BranchID = pod.BranchID,
                                      CompanyID = pod.CompanyID,
                                      BaseOnID = pod.BaseOnID,

                                      DeliveryDate = pod.DeliveryDate,
                                      DocumentDate = pod.DocumentDate,
                                      FreightPurchaseView = new FreightPurchaseViewModel
                                      {
                                          ExpenceAmount = fs.ExpenceAmount,
                                          PurID = pod.ID,
                                          ID = fs.ID,
                                          PurType = fs.PurType,
                                          TaxSumValue = fs.TaxSumValue,
                                          FreightPurchaseDetailViewModels = (from fsd in _context.FreightPurchaseDetails.Where(i => i.FreightPurchaseID == fs.ID)
                                                                             select new FreightPurchaseDetailViewModel
                                                                             {
                                                                                 ID = fsd.ID,
                                                                                 FreightPurchaseID = fsd.FreightPurchaseID,
                                                                                 Amount = fsd.Amount,
                                                                                 AmountWithTax = fsd.AmountWithTax,
                                                                                 FreightID = fsd.FreightID,
                                                                                 Name = fsd.Name,
                                                                                 TaxGroup = fsd.TaxGroup,
                                                                                 TaxGroupID = fsd.TaxGroupID,
                                                                                 TaxGroups = GetTaxGroups(),
                                                                                 TaxGroupSelect = taxGroup.Select(i => new SelectListItem
                                                                                 {
                                                                                     Value = i.ID.ToString(),
                                                                                     Selected = fsd.TaxGroupID == i.ID,
                                                                                     Text = $"{i.Code}-{i.Name}"
                                                                                 }).ToList(),
                                                                                 TaxRate = fsd.TaxRate,
                                                                                 TotalTaxAmount = fsd.TotalTaxAmount
                                                                             }).ToList(),
                                      },
                                      InvoiceNo = $"{docType.Code}-{pod.Number}",
                                      LocalCurID = pod.LocalCurID,
                                      LocalSetRate = (decimal)pod.LocalSetRate,
                                      PostingDate = pod.PostingDate,
                                      Status = pod.Status,
                                      SubTotalAfterDis = pod.SubTotalAfterDis,
                                      SubTotalAfterDisSys = pod.SubTotalAfterDisSys,
                                      TypeDis = pod.TypeDis,
                                      UserID = pod.UserID,
                                      WarehouseID = pod.WarehouseID,
                                      PurchaseOrderID = pod.ID,
                                      AdditionalExpense = (decimal)pod.AdditionalExpense,
                                      AdditionalNote = pod.AdditionalNote,
                                      AppliedAmount = (decimal)pod.AppliedAmount,
                                      AppliedAmountSys = (decimal)pod.AppliedAmountSys,
                                      BalanceDue = (decimal)pod.BalanceDue,
                                      BalanceDueSys = (decimal)pod.BalanceDueSys,
                                      DiscountRate = (decimal)pod.DiscountRate,
                                      DiscountValue = (decimal)pod.DiscountValue,
                                      DocumentTypeID = pod.DocumentTypeID,
                                      DownPayment = (decimal)pod.DownPayment,
                                      DownPaymentSys = (decimal)pod.DownPaymentSys,
                                      Number = pod.Number,
                                      PurCurrencyID = pod.PurCurrencyID,
                                      PurRate = (decimal)pod.PurRate,
                                      ReffNo = pod.ReffNo,
                                      Remark = pod.Remark,
                                      ReturnAmount = (decimal)pod.ReturnAmount,
                                      SeriesDetailID = pod.SeriesDetailID,
                                      SeriesID = pod.SeriesID,
                                      SubTotal = (decimal)pod.SubTotal,
                                      SubTotalSys = (decimal)pod.SubTotalSys,
                                      SysCurrencyID = pod.SysCurrencyID,
                                      TaxRate = (decimal)pod.TaxRate,
                                      TaxValue = (decimal)pod.TaxValue,
                                      VendorID = pod.VendorID
                                  }).FirstOrDefaultAsync() ?? new PurchaseOrderViewModel();

            var _tg = new TaxGroupViewModel
            {
                ID = 0,
                Name = "- Select --",
                Code = "",
                Rate = 0,
                Type = 0,
            };
            var tgs = (from t in _context.TaxGroups.Where(i => !i.Delete && i.Active && i.Type == TypeTax.InputTax)
                       let tgds = _context.TaxGroupDefinitions.Where(i => i.TaxGroupID == t.ID).OrderByDescending(i => i.EffectiveFrom).FirstOrDefault() ?? new TaxGroupDefinition()
                       select new TaxGroupViewModel
                       {
                           ID = t.ID,
                           Name = t.Name,
                           Code = t.Code,
                           Effectivefrom = tgds.EffectiveFrom,
                           Rate = tgds.Rate,
                           Type = (int)t.Type,
                       }).ToList();
            tgs.Insert(0, _tg);
            #endregion
            var _purOrderDetails = (from podd in _context.PurchaseQuotationDetails.Where(s => s.PurchaseQuotationID == purOrder.ID) //on purOr.PurchaseOrderID equals podd.ID
                                    join item in _context.ItemMasterDatas on podd.ItemID equals item.ID
                                    let cur = _context.Currency.FirstOrDefault(i => i.ID == purOrder.PurCurrencyID) ?? new Currency()
                                    select new PurchaseOrderDetialViewModel
                                    {
                                        LineIDUN = podd.ID + "" + DateTime.Now.Ticks.ToString(),
                                        LineID = podd.ID,
                                        ItemID = item.ID,
                                        OrderID = item.ID,
                                        Process = item.Process,
                                        Qty = (decimal)podd.Qty,
                                        OpenQty = (decimal)podd.OpenQty,
                                        TypeDis = podd.TypeDis,
                                        UomID = podd.UomID,
                                        TaxGroupID = podd.TaxGroupID,
                                        TaxRate = podd.TaxRate,
                                        TaxGroupSelect = tgs.Select(c => new SelectListItem
                                        {
                                            Value = c.ID.ToString(),
                                            Text = $"{c.Code}-{c.Name}",
                                            Selected = c.ID == podd.TaxGroupID
                                        }).ToList(),
                                        TotalWTax = (decimal)podd.TotalWTax,
                                        TaxValue = podd.TaxValue,
                                        Total = (decimal)podd.Total,
                                        Remark = podd.Remark,
                                        /// select List UoM ///
                                        UoMSelect = (from GDU in _context.GroupDUoMs.Where(i => i.GroupUoMID == item.GroupUomID)
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
                                                         Selected = c.ID == podd.UomID
                                                     }).ToList(),
                                        /// List UoM ///
                                        UoMsList = (from GDU in _context.GroupDUoMs.Where(i => i.GroupUoMID == item.GroupUomID)
                                                    join UNM in _context.UnitofMeasures on GDU.AltUOM equals UNM.ID
                                                    select new UOMSViewModel
                                                    {
                                                        BaseUoMID = GDU.BaseUOM,
                                                        Factor = GDU.Factor,
                                                        ID = UNM.ID,
                                                        Name = UNM.Name
                                                    }).ToList(),
                                        TaxGroups = (from t in _context.TaxGroups.Where(i => !i.Delete && i.Active && i.Type == TypeTax.InputTax)
                                                     let tgds = _context.TaxGroupDefinitions.Where(i => i.TaxGroupID == t.ID).OrderByDescending(i => i.EffectiveFrom).FirstOrDefault() ?? new TaxGroupDefinition()
                                                     select new TaxGroupViewModel
                                                     {
                                                         ID = t.ID,
                                                         Name = t.Name,
                                                         Code = t.Code,
                                                         Effectivefrom = tgds.EffectiveFrom,
                                                         Rate = tgds.Rate,
                                                         Type = (int)t.Type,
                                                     }
                                                     ).ToList(),
                                        PurchaseOrderDetailID = podd.ID,
                                        FinDisRate = podd.FinDisRate,
                                        FinDisValue = podd.FinDisValue,
                                        FinTotalValue = podd.FinTotalValue,
                                        TaxOfFinDisValue = podd.TaxOfFinDisValue,
                                        Barcode = item.Barcode,
                                        Code = item.Code,
                                        CurrencyName = cur.Description,
                                        DiscountRate = (decimal)podd.DiscountRate,
                                        AlertStock = (decimal)podd.AlertStock,
                                        Delete = podd.Delete,
                                        DiscountValue = (decimal)podd.DiscountValue,
                                        ExpireDate = podd.ExpireDate,
                                        ItemName = item.KhmerName,
                                        LocalCurrencyID = podd.LocalCurrencyID,
                                        OldQty = (decimal)podd.OldQty,
                                        ID = podd.ID,

                                        PurchaseOrderID = podd.PurchaseQuotationID,
                                        PurchasPrice = (decimal)podd.PurchasPrice,
                                        QuotationID = podd.QuotationID,
                                        TotalWTaxSys = (decimal)podd.TotalWTaxSys,
                                        TotalSys = (decimal)podd.TotalSys,
                                    }).ToList() ?? new List<PurchaseOrderDetialViewModel>();
            _dataProp.DataProperty(_purOrderDetails, comId, "ItemID", "AddictionProps");
            var data = new PurchaseOrderUpdateViewModel
            {
                PurchaseOrder = purOrder,
                PurchaseOrderDetials = _purOrderDetails,
            };
            return await Task.FromResult(data);
        }
        // 
        public async Task<PurchaseAPUpdateViewModel> FindPurchaseAPReserveAsync(int seriesID, string number, int comId)
        {
            #region
            var taxGroup = GetTaxGroups();
            taxGroup.Insert(0, new TaxGroupViewModel
            {
                ID = 0,
                Name = "- Select --",
                Code = "",
                Rate = 0,
                Type = 0,
            });
            var purAP = await (from ap in _context.PurchaseAPReserves.Where(x => x.Number == number && x.SeriesID == seriesID && x.CompanyID == comId)
                               join docType in _context.DocumentTypes on ap.DocumentTypeID equals docType.ID
                               let fs = _context.FreightPurchases.Where(i => i.PurID == ap.ID && i.PurType == PurCopyType.PurReserve).FirstOrDefault() ?? new FreightPurchase()
                               select new PurchaseAPViewModel
                               {
                                   FrieghtAmount = ap.FrieghtAmount,
                                   FrieghtAmountSys = ap.FrieghtAmountSys,
                                   BranchID = ap.BranchID,
                                   CompanyID = ap.CompanyID,
                                   DeliveryDate = ap.DueDate,
                                   DueDate = ap.DueDate,
                                   DocumentDate = ap.DocumentDate,
                                   FreightPurchaseView = new FreightPurchaseViewModel
                                   {
                                       ExpenceAmount = fs.ExpenceAmount,
                                       PurID = ap.ID,
                                       ID = fs.ID,
                                       PurType = fs.PurType,
                                       TaxSumValue = fs.TaxSumValue,
                                       FreightPurchaseDetailViewModels = (from fsd in _context.FreightPurchaseDetails.Where(i => i.FreightPurchaseID == fs.ID)
                                                                          select new FreightPurchaseDetailViewModel
                                                                          {
                                                                              ID = fsd.ID,
                                                                              FreightPurchaseID = fsd.FreightPurchaseID,
                                                                              Amount = fsd.Amount,
                                                                              AmountWithTax = fsd.AmountWithTax,
                                                                              FreightID = fsd.FreightID,
                                                                              Name = fsd.Name,
                                                                              TaxGroup = fsd.TaxGroup,
                                                                              TaxGroupID = fsd.TaxGroupID,
                                                                              TaxGroups = GetTaxGroups(),
                                                                              TaxGroupSelect = taxGroup.Select(i => new SelectListItem
                                                                              {
                                                                                  Value = i.ID.ToString(),
                                                                                  Selected = fsd.TaxGroupID == i.ID,
                                                                                  Text = $"{i.Code}-{i.Name}"
                                                                              }).ToList(),
                                                                              TaxRate = fsd.TaxRate,
                                                                              TotalTaxAmount = fsd.TotalTaxAmount
                                                                          }).ToList(),
                                   },
                                   InvoiceNo = $"{docType.Code}-{ap.Number}",
                                   LocalCurID = ap.LocalCurID,
                                   LocalSetRate = (decimal)ap.LocalSetRate,
                                   PostingDate = ap.PostingDate,
                                   Status = ap.Status,
                                   SubTotalAfterDis = ap.SubTotalAfterDis,
                                   SubTotalAfterDisSys = ap.SubTotalAfterDisSys,
                                   TypeDis = ap.TypeDis,
                                   UserID = ap.UserID,
                                   WarehouseID = ap.WarehouseID,
                                   PurchaseAPID = ap.ID,
                                   BaseOnID = ap.ID,
                                   AdditionalExpense = (decimal)ap.AdditionalExpense,
                                   AdditionalNote = ap.AdditionalNote,
                                   AppliedAmount = (decimal)ap.AppliedAmount,
                                   AppliedAmountSys = (decimal)ap.AppliedAmountSys,
                                   BalanceDue = (decimal)ap.BalanceDue,
                                   BalanceDueSys = (decimal)ap.BalanceDueSys,
                                   DiscountRate = (decimal)ap.DiscountRate,
                                   DiscountValue = (decimal)ap.DiscountValue,
                                   DocumentTypeID = ap.DocumentTypeID,
                                   DownPayment = (decimal)ap.DownPayment,
                                   DownPaymentSys = (decimal)ap.DownPaymentSys,
                                   Number = ap.Number,
                                   PurCurrencyID = ap.PurCurrencyID,
                                   PurRate = (decimal)ap.PurRate,
                                   ReffNo = ap.ReffNo,
                                   Remark = ap.Remark,
                                   ReturnAmount = (decimal)ap.ReturnAmount,
                                   SeriesDetailID = ap.SeriesDetailID,
                                   SeriesID = ap.SeriesID,
                                   SubTotal = (decimal)ap.SubTotal,
                                   SubTotalSys = (decimal)ap.SubTotalSys,
                                   SysCurrencyID = ap.SysCurrencyID,
                                   TaxRate = (decimal)ap.TaxRate,
                                   TaxValue = (decimal)ap.TaxValue,
                                   VendorID = ap.VendorID
                               }).ToListAsync();

            var _tg = new TaxGroupViewModel
            {
                ID = 0,
                Name = "- Select --",
                Code = "",
                Rate = 0,
                Type = 0,
            };
            var tgs = (from t in _context.TaxGroups.Where(i => !i.Delete && i.Active && i.Type == TypeTax.InputTax)
                       let tgds = _context.TaxGroupDefinitions.Where(i => i.TaxGroupID == t.ID).OrderByDescending(i => i.EffectiveFrom).FirstOrDefault() ?? new TaxGroupDefinition()
                       select new TaxGroupViewModel
                       {
                           ID = t.ID,
                           Name = t.Name,
                           Code = t.Code,
                           Effectivefrom = tgds.EffectiveFrom,
                           Rate = tgds.Rate,
                           Type = (int)t.Type,
                       }).ToList();
            tgs.Insert(0, _tg);
            #endregion
            var __APd = (from ap in purAP
                         join apd in _context.PurchaseAPReserveDetails on ap.PurchaseAPID equals apd.PurchaseAPReserveID
                         join item in _context.ItemMasterDatas on apd.ItemID equals item.ID
                         let cur = _context.Currency.FirstOrDefault(i => i.ID == ap.PurCurrencyID) ?? new Currency()
                         select new PurchaseAPDetialViewModel
                         {
                             LineIDUN = DateTime.Now.Ticks.ToString(),
                             LineID = apd.ID,
                             ItemID = item.ID,
                             Process = item.Process,
                             Qty = (decimal)apd.Qty,
                             OpenQty = (decimal)apd.OpenQty,
                             TypeDis = apd.TypeDis,
                             UomID = apd.UomID,
                             TaxGroupID = apd.TaxGroupID,
                             TaxRate = apd.TaxRate,
                             TaxGroupSelect = tgs.Select(c => new SelectListItem
                             {
                                 Value = c.ID.ToString(),
                                 Text = $"{c.Code}-{c.Name}",
                                 Selected = c.ID == apd.TaxGroupID
                             }).ToList(),
                             TotalWTax = (decimal)apd.TotalWTax,
                             TaxValue = apd.TaxValue,
                             Total = (decimal)apd.Total,
                             Remark = apd.Remark,
                             /// select List UoM ///
                             UoMSelect = (from GDU in _context.GroupDUoMs.Where(i => i.GroupUoMID == item.GroupUomID)
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
                                              Selected = c.ID == apd.UomID
                                          }).ToList(),
                             /// List UoM ///
                             UoMsList = (from GDU in _context.GroupDUoMs.Where(i => i.GroupUoMID == item.GroupUomID)
                                         join UNM in _context.UnitofMeasures on GDU.AltUOM equals UNM.ID
                                         select new UOMSViewModel
                                         {
                                             BaseUoMID = GDU.BaseUOM,
                                             Factor = GDU.Factor,
                                             ID = UNM.ID,
                                             Name = UNM.Name
                                         }).ToList(),
                             TaxGroups = (from t in _context.TaxGroups.Where(i => !i.Delete && i.Active && i.Type == TypeTax.InputTax)
                                          let tgds = _context.TaxGroupDefinitions.Where(i => i.TaxGroupID == t.ID).OrderByDescending(i => i.EffectiveFrom).FirstOrDefault() ?? new TaxGroupDefinition()
                                          select new TaxGroupViewModel
                                          {
                                              ID = t.ID,
                                              Name = t.Name,
                                              Code = t.Code,
                                              Effectivefrom = tgds.EffectiveFrom,
                                              Rate = tgds.Rate,
                                              Type = (int)t.Type,
                                          }
                                          ).ToList(),
                             FinDisRate = apd.FinDisRate,
                             FinDisValue = apd.FinDisValue,
                             FinTotalValue = apd.FinTotalValue,
                             TaxOfFinDisValue = apd.TaxOfFinDisValue,
                             Barcode = item.Barcode,
                             Code = item.Code,
                             CurrencyName = cur.Description,
                             DiscountRate = (decimal)apd.DiscountRate,
                             AlertStock = (decimal)apd.AlertStock,
                             Delete = apd.Delete,
                             DiscountValue = (decimal)apd.DiscountValue,
                             ExpireDate = apd.ExpireDate,
                             ItemName = item.KhmerName,
                             LocalCurrencyID = apd.LocalCurrencyID,
                             PurchaseDetailAPID = apd.ID,
                             PurchaseAPID = apd.ID,
                             PurchasPrice = (decimal)apd.PurchasPrice,
                             TotalWTaxSys = (decimal)apd.TotalWTaxSys,
                             TotalSys = (decimal)apd.TotalSys,
                         }).ToList();//GroupBy(i => i.ItemID).Select(i => i.FirstOrDefault()).ToList();
            _dataProp.DataProperty(__APd, comId, "ItemID", "AddictionProps");
            var data = new PurchaseAPUpdateViewModel
            {
                PurchaseAP = purAP.FirstOrDefault(),
                PurchaseAPDetials = __APd
            };
            return await Task.FromResult(data);
        }


        public async Task<PurchaseAPUpdateViewModel> FindPurchaseQuote(int seriesID, string number, int comId)
        {
            #region
            var taxGroup = GetTaxGroups();
            taxGroup.Insert(0, new TaxGroupViewModel
            {
                ID = 0,
                Name = "- Select --",
                Code = "",
                Rate = 0,
                Type = 0,
            });
            var purAP = await (from ap in _context.PurchaseQuotations.Where(x => x.Number == number && x.SeriesID == seriesID && x.CompanyID == comId)
                               join docType in _context.DocumentTypes on ap.DocumentTypeID equals docType.ID
                               let fs = _context.FreightPurchases.Where(i => i.PurID == ap.ID && i.PurType == PurCopyType.PurQuote).FirstOrDefault() ?? new FreightPurchase()
                               select new PurchaseAPViewModel
                               {
                                   FrieghtAmount = ap.FrieghtAmount,
                                   FrieghtAmountSys = ap.FrieghtAmountSys,
                                   BranchID = ap.BranchID,
                                   CompanyID = ap.CompanyID,
                                   DeliveryDate = ap.DeliveryDate,
                                   DueDate = ap.DeliveryDate,
                                   DocumentDate = ap.DocumentDate,
                                   FreightPurchaseView = new FreightPurchaseViewModel
                                   {
                                       ExpenceAmount = fs.ExpenceAmount,
                                       PurID = ap.ID,
                                       ID = fs.ID,
                                       PurType = fs.PurType,
                                       TaxSumValue = fs.TaxSumValue,
                                       FreightPurchaseDetailViewModels = (from fsd in _context.FreightPurchaseDetails.Where(i => i.FreightPurchaseID == fs.ID)
                                                                          select new FreightPurchaseDetailViewModel
                                                                          {
                                                                              ID = fsd.ID,
                                                                              FreightPurchaseID = fsd.FreightPurchaseID,
                                                                              Amount = fsd.Amount,
                                                                              AmountWithTax = fsd.AmountWithTax,
                                                                              FreightID = fsd.FreightID,
                                                                              Name = fsd.Name,
                                                                              TaxGroup = fsd.TaxGroup,
                                                                              TaxGroupID = fsd.TaxGroupID,
                                                                              TaxGroups = GetTaxGroups(),
                                                                              TaxGroupSelect = taxGroup.Select(i => new SelectListItem
                                                                              {
                                                                                  Value = i.ID.ToString(),
                                                                                  Selected = fsd.TaxGroupID == i.ID,
                                                                                  Text = $"{i.Code}-{i.Name}"
                                                                              }).ToList(),
                                                                              TaxRate = fsd.TaxRate,
                                                                              TotalTaxAmount = fsd.TotalTaxAmount
                                                                          }).ToList(),
                                   },
                                   InvoiceNo = $"{docType.Code}-{ap.Number}",
                                   LocalCurID = ap.LocalCurID,
                                   LocalSetRate = (decimal)ap.LocalSetRate,
                                   PostingDate = ap.PostingDate,
                                   Status = ap.Status,
                                   SubTotalAfterDis = ap.SubTotalAfterDis,
                                   SubTotalAfterDisSys = ap.SubTotalAfterDisSys,
                                   TypeDis = ap.TypeDis,
                                   UserID = ap.UserID,
                                   WarehouseID = ap.WarehouseID,
                                   PurchaseAPID = ap.ID,
                                   BaseOnID = ap.BaseOnID,
                                   AdditionalExpense = (decimal)ap.AdditionalExpense,
                                   AdditionalNote = ap.AdditionalNote,
                                   AppliedAmount = (decimal)ap.AppliedAmount,
                                   AppliedAmountSys = (decimal)ap.AppliedAmountSys,
                                   BalanceDue = (decimal)ap.BalanceDue,
                                   BalanceDueSys = (decimal)ap.BalanceDueSys,
                                   DiscountRate = (decimal)ap.DiscountRate,
                                   DiscountValue = (decimal)ap.DiscountValue,
                                   DocumentTypeID = ap.DocumentTypeID,
                                   DownPayment = (decimal)ap.DownPayment,
                                   DownPaymentSys = (decimal)ap.DownPaymentSys,
                                   Number = ap.Number,
                                   PurCurrencyID = ap.PurCurrencyID,
                                   PurRate = (decimal)ap.PurRate,
                                   ReffNo = ap.ReffNo,
                                   Remark = ap.Remark,
                                   ReturnAmount = (decimal)ap.ReturnAmount,
                                   SeriesDetailID = ap.SeriesDetailID,
                                   SeriesID = ap.SeriesID,
                                   SubTotal = (decimal)ap.SubTotal,
                                   SubTotalSys = (decimal)ap.SubTotalSys,
                                   SysCurrencyID = ap.SysCurrencyID,
                                   TaxRate = (decimal)ap.TaxRate,
                                   TaxValue = (decimal)ap.TaxValue,
                                   VendorID = ap.VendorID
                               }).ToListAsync();

            var _tg = new TaxGroupViewModel
            {
                ID = 0,
                Name = "- Select --",
                Code = "",
                Rate = 0,
                Type = 0,
            };
            var tgs = (from t in _context.TaxGroups.Where(i => !i.Delete && i.Active && i.Type == TypeTax.InputTax)
                       let tgds = _context.TaxGroupDefinitions.Where(i => i.TaxGroupID == t.ID).OrderByDescending(i => i.EffectiveFrom).FirstOrDefault() ?? new TaxGroupDefinition()
                       select new TaxGroupViewModel
                       {
                           ID = t.ID,
                           Name = t.Name,
                           Code = t.Code,
                           Effectivefrom = tgds.EffectiveFrom,
                           Rate = tgds.Rate,
                           Type = (int)t.Type,
                       }).ToList();
            tgs.Insert(0, _tg);
            #endregion
            var __APd = (from ap in purAP
                         join apd in _context.PurchaseQuotationDetails on ap.PurchaseAPID equals apd.PurchaseQuotationID
                         join item in _context.ItemMasterDatas on apd.ItemID equals item.ID
                         let cur = _context.Currency.FirstOrDefault(i => i.ID == ap.PurCurrencyID) ?? new Currency()
                         select new PurchaseAPDetialViewModel
                         {
                             LineIDUN = DateTime.Now.Ticks.ToString(),
                             LineID = apd.ID,
                             ItemID = item.ID,
                             Process = item.Process,
                             Qty = (decimal)apd.Qty,
                             OpenQty = (decimal)apd.OpenQty,
                             TypeDis = apd.TypeDis,
                             UomID = apd.UomID,
                             TaxGroupID = apd.TaxGroupID,
                             TaxRate = apd.TaxRate,
                             TaxGroupSelect = tgs.Select(c => new SelectListItem
                             {
                                 Value = c.ID.ToString(),
                                 Text = $"{c.Code}-{c.Name}",
                                 Selected = c.ID == apd.TaxGroupID
                             }).ToList(),
                             TotalWTax = (decimal)apd.TotalWTax,
                             TaxValue = apd.TaxValue,
                             Total = (decimal)apd.Total,
                             Remark = apd.Remark,
                             /// select List UoM ///
                             UoMSelect = (from GDU in _context.GroupDUoMs.Where(i => i.GroupUoMID == item.GroupUomID)
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
                                              Selected = c.ID == apd.UomID
                                          }).ToList(),
                             /// List UoM ///
                             UoMsList = (from GDU in _context.GroupDUoMs.Where(i => i.GroupUoMID == item.GroupUomID)
                                         join UNM in _context.UnitofMeasures on GDU.AltUOM equals UNM.ID
                                         select new UOMSViewModel
                                         {
                                             BaseUoMID = GDU.BaseUOM,
                                             Factor = GDU.Factor,
                                             ID = UNM.ID,
                                             Name = UNM.Name
                                         }).ToList(),
                             TaxGroups = (from t in _context.TaxGroups.Where(i => !i.Delete && i.Active && i.Type == TypeTax.InputTax)
                                          let tgds = _context.TaxGroupDefinitions.Where(i => i.TaxGroupID == t.ID).OrderByDescending(i => i.EffectiveFrom).FirstOrDefault() ?? new TaxGroupDefinition()
                                          select new TaxGroupViewModel
                                          {
                                              ID = t.ID,
                                              Name = t.Name,
                                              Code = t.Code,
                                              Effectivefrom = tgds.EffectiveFrom,
                                              Rate = tgds.Rate,
                                              Type = (int)t.Type,
                                          }
                                          ).ToList(),
                             FinDisRate = apd.FinDisRate,
                             FinDisValue = apd.FinDisValue,
                             FinTotalValue = apd.FinTotalValue,
                             TaxOfFinDisValue = apd.TaxOfFinDisValue,
                             Barcode = item.Barcode,
                             Code = item.Code,
                             CurrencyName = cur.Description,
                             DiscountRate = (decimal)apd.DiscountRate,
                             AlertStock = (decimal)apd.AlertStock,
                             Delete = apd.Delete,
                             DiscountValue = (decimal)apd.DiscountValue,
                             ExpireDate = apd.ExpireDate,
                             ItemName = item.KhmerName,
                             LocalCurrencyID = apd.LocalCurrencyID,
                             PurchaseDetailAPID = apd.ID,
                             PurchaseAPID = apd.ID,
                             PurchasPrice = (decimal)apd.PurchasPrice,
                             TotalWTaxSys = (decimal)apd.TotalWTaxSys,
                             TotalSys = (decimal)apd.TotalSys,
                         }).ToList();//GroupBy(i => i.ItemID).Select(i => i.FirstOrDefault()).ToList();
            _dataProp.DataProperty(__APd, comId, "ItemID", "AddictionProps");
            var data = new PurchaseAPUpdateViewModel
            {
                PurchaseAP = purAP.FirstOrDefault(),
                PurchaseAPDetials = __APd
            };
            return await Task.FromResult(data);
        }

        public async Task<PurchaseAPUpdateViewModel> FindPurchaseOrder(int seriesID, string number, int comId)
        {
            #region
            var taxGroup = GetTaxGroups();
            taxGroup.Insert(0, new TaxGroupViewModel
            {
                ID = 0,
                Name = "- Select --",
                Code = "",
                Rate = 0,
                Type = 0,
            });
            var purAP = await (from ap in _context.PurchaseOrders.Where(x => x.Number == number && x.SeriesID == seriesID && x.CompanyID == comId)
                               join docType in _context.DocumentTypes on ap.DocumentTypeID equals docType.ID
                               let fs = _context.FreightPurchases.Where(i => i.PurID == ap.PurchaseOrderID && i.PurType == PurCopyType.PurOrder).FirstOrDefault() ?? new FreightPurchase()
                               select new PurchaseAPViewModel
                               {
                                   FrieghtAmount = ap.FrieghtAmount,
                                   FrieghtAmountSys = ap.FrieghtAmountSys,
                                   BranchID = ap.BranchID,
                                   CompanyID = ap.CompanyID,
                                   DeliveryDate = ap.DeliveryDate,
                                   DueDate = ap.DeliveryDate,
                                   DocumentDate = ap.DocumentDate,
                                   FreightPurchaseView = new FreightPurchaseViewModel
                                   {
                                       ExpenceAmount = fs.ExpenceAmount,
                                       PurID = ap.PurchaseOrderID,
                                       ID = fs.ID,
                                       PurType = fs.PurType,
                                       TaxSumValue = fs.TaxSumValue,
                                       FreightPurchaseDetailViewModels = (from fsd in _context.FreightPurchaseDetails.Where(i => i.FreightPurchaseID == fs.ID)
                                                                          select new FreightPurchaseDetailViewModel
                                                                          {
                                                                              ID = fsd.ID,
                                                                              FreightPurchaseID = fsd.FreightPurchaseID,
                                                                              Amount = fsd.Amount,
                                                                              AmountWithTax = fsd.AmountWithTax,
                                                                              FreightID = fsd.FreightID,
                                                                              Name = fsd.Name,
                                                                              TaxGroup = fsd.TaxGroup,
                                                                              TaxGroupID = fsd.TaxGroupID,
                                                                              TaxGroups = GetTaxGroups(),
                                                                              TaxGroupSelect = taxGroup.Select(i => new SelectListItem
                                                                              {
                                                                                  Value = i.ID.ToString(),
                                                                                  Selected = fsd.TaxGroupID == i.ID,
                                                                                  Text = $"{i.Code}-{i.Name}"
                                                                              }).ToList(),
                                                                              TaxRate = fsd.TaxRate,
                                                                              TotalTaxAmount = fsd.TotalTaxAmount
                                                                          }).ToList(),
                                   },
                                   InvoiceNo = $"{docType.Code}-{ap.Number}",
                                   LocalCurID = ap.LocalCurID,
                                   LocalSetRate = (decimal)ap.LocalSetRate,
                                   PostingDate = ap.PostingDate,
                                   Status = ap.Status,
                                   SubTotalAfterDis = ap.SubTotalAfterDis,
                                   SubTotalAfterDisSys = ap.SubTotalAfterDisSys,
                                   TypeDis = ap.TypeDis,
                                   UserID = ap.UserID,
                                   WarehouseID = ap.WarehouseID,
                                   PurchaseAPID = ap.PurchaseOrderID,
                                   BaseOnID = ap.BaseOnID,
                                   AdditionalExpense = (decimal)ap.AdditionalExpense,
                                   AdditionalNote = ap.AdditionalNote,
                                   AppliedAmount = (decimal)ap.AppliedAmount,
                                   AppliedAmountSys = (decimal)ap.AppliedAmountSys,
                                   BalanceDue = (decimal)ap.BalanceDue,
                                   BalanceDueSys = (decimal)ap.BalanceDueSys,
                                   DiscountRate = (decimal)ap.DiscountRate,
                                   DiscountValue = (decimal)ap.DiscountValue,
                                   DocumentTypeID = ap.DocumentTypeID,
                                   DownPayment = (decimal)ap.DownPayment,
                                   DownPaymentSys = (decimal)ap.DownPaymentSys,
                                   Number = ap.Number,
                                   PurCurrencyID = ap.PurCurrencyID,
                                   PurRate = (decimal)ap.PurRate,
                                   ReffNo = ap.ReffNo,
                                   Remark = ap.Remark,
                                   ReturnAmount = (decimal)ap.ReturnAmount,
                                   SeriesDetailID = ap.SeriesDetailID,
                                   SeriesID = ap.SeriesID,
                                   SubTotal = (decimal)ap.SubTotal,
                                   SubTotalSys = (decimal)ap.SubTotalSys,
                                   SysCurrencyID = ap.SysCurrencyID,
                                   TaxRate = (decimal)ap.TaxRate,
                                   TaxValue = (decimal)ap.TaxValue,
                                   VendorID = ap.VendorID
                               }).ToListAsync();

            var _tg = new TaxGroupViewModel
            {
                ID = 0,
                Name = "- Select --",
                Code = "",
                Rate = 0,
                Type = 0,
            };
            var tgs = (from t in _context.TaxGroups.Where(i => !i.Delete && i.Active && i.Type == TypeTax.InputTax)
                       let tgds = _context.TaxGroupDefinitions.Where(i => i.TaxGroupID == t.ID).OrderByDescending(i => i.EffectiveFrom).FirstOrDefault() ?? new TaxGroupDefinition()
                       select new TaxGroupViewModel
                       {
                           ID = t.ID,
                           Name = t.Name,
                           Code = t.Code,
                           Effectivefrom = tgds.EffectiveFrom,
                           Rate = tgds.Rate,
                           Type = (int)t.Type,
                       }).ToList();
            tgs.Insert(0, _tg);
            #endregion
            var __APd = (from ap in purAP
                         join apd in _context.PurchaseOrderDetails on ap.PurchaseAPID equals apd.PurchaseOrderID
                         join item in _context.ItemMasterDatas on apd.ItemID equals item.ID
                         let cur = _context.Currency.FirstOrDefault(i => i.ID == ap.PurCurrencyID) ?? new Currency()
                         select new PurchaseAPDetialViewModel
                         {
                             LineIDUN = DateTime.Now.Ticks.ToString(),
                             LineID = apd.PurchaseOrderDetailID,
                             ItemID = item.ID,
                             Process = item.Process,
                             Qty = (decimal)apd.Qty,
                             OpenQty = (decimal)apd.OpenQty,
                             TypeDis = apd.TypeDis,
                             UomID = apd.UomID,
                             TaxGroupID = apd.TaxGroupID,
                             TaxRate = apd.TaxRate,
                             TaxGroupSelect = tgs.Select(c => new SelectListItem
                             {
                                 Value = c.ID.ToString(),
                                 Text = $"{c.Code}-{c.Name}",
                                 Selected = c.ID == apd.TaxGroupID
                             }).ToList(),
                             TotalWTax = (decimal)apd.TotalWTax,
                             TaxValue = apd.TaxValue,
                             Total = (decimal)apd.Total,
                             Remark = apd.Remark,
                             /// select List UoM ///
                             UoMSelect = (from GDU in _context.GroupDUoMs.Where(i => i.GroupUoMID == item.GroupUomID)
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
                                              Selected = c.ID == apd.UomID
                                          }).ToList(),
                             /// List UoM ///
                             UoMsList = (from GDU in _context.GroupDUoMs.Where(i => i.GroupUoMID == item.GroupUomID)
                                         join UNM in _context.UnitofMeasures on GDU.AltUOM equals UNM.ID
                                         select new UOMSViewModel
                                         {
                                             BaseUoMID = GDU.BaseUOM,
                                             Factor = GDU.Factor,
                                             ID = UNM.ID,
                                             Name = UNM.Name
                                         }).ToList(),
                             TaxGroups = (from t in _context.TaxGroups.Where(i => !i.Delete && i.Active && i.Type == TypeTax.InputTax)
                                          let tgds = _context.TaxGroupDefinitions.Where(i => i.TaxGroupID == t.ID).OrderByDescending(i => i.EffectiveFrom).FirstOrDefault() ?? new TaxGroupDefinition()
                                          select new TaxGroupViewModel
                                          {
                                              ID = t.ID,
                                              Name = t.Name,
                                              Code = t.Code,
                                              Effectivefrom = tgds.EffectiveFrom,
                                              Rate = tgds.Rate,
                                              Type = (int)t.Type,
                                          }
                                          ).ToList(),
                             FinDisRate = apd.FinDisRate,
                             FinDisValue = apd.FinDisValue,
                             FinTotalValue = apd.FinTotalValue,
                             TaxOfFinDisValue = apd.TaxOfFinDisValue,
                             Barcode = item.Barcode,
                             Code = item.Code,
                             CurrencyName = cur.Description,
                             DiscountRate = (decimal)apd.DiscountRate,
                             AlertStock = (decimal)apd.AlertStock,
                             Delete = apd.Delete,
                             DiscountValue = (decimal)apd.DiscountValue,
                             ExpireDate = apd.ExpireDate,
                             ItemName = item.KhmerName,
                             LocalCurrencyID = apd.LocalCurrencyID,
                             PurchaseDetailAPID = apd.PurchaseOrderDetailID,
                             PurchaseAPID = apd.PurchaseOrderID,
                             PurchasPrice = (decimal)apd.PurchasPrice,
                             TotalWTaxSys = (decimal)apd.TotalWTaxSys,
                             TotalSys = (decimal)apd.TotalSys,
                         }).ToList();//GroupBy(i => i.ItemID).Select(i => i.FirstOrDefault()).ToList();
            _dataProp.DataProperty(__APd, comId, "ItemID", "AddictionProps");
            var data = new PurchaseAPUpdateViewModel
            {
                PurchaseAP = purAP.FirstOrDefault(),
                PurchaseAPDetials = __APd
            };
            return await Task.FromResult(data);
        }
        public async Task<APCSerialNumberDetial> GetSerialDetialsAsync(decimal cost, int bpId, int itemId, int baseOnID, int copyTuype, string apsds, bool isAll = false)
        {
            List<APCSNDUnselectDetial> aPCSNDDetials = new();
            var list = baseOnID == 0 ? await _context.WarehouseDetails.Where(i => i.InStock > 0 && (decimal)i.Cost == cost && i.ItemID == itemId && !string.IsNullOrEmpty(i.SerialNumber)).ToListAsync()
                             : await _context.WarehouseDetails.Where(i => i.InStock > 0 && (decimal)i.Cost == cost && i.ItemID == itemId && i.BaseOnID == baseOnID && (int)i.PurCopyType == copyTuype && !string.IsNullOrEmpty(i.SerialNumber)).ToListAsync();
            aPCSNDDetials = (from i in list
                             select new APCSNDUnselectDetial
                             {
                                 // Qty = (decimal)i.InStock,
                                 // SerialNumber = i.SerialNumber,
                                 // LotNumber = i.LotNumber,
                                 // MfrSerialNo = i.MfrSerialNumber,
                                 // UnitCost = Convert.ToDecimal(i.Cost),
                                 // BPID = i.BPID,
                                 UnitCost = Convert.ToDecimal(i.Cost),
                                 BPID = i.BPID,
                                 BaseOnID = i.BaseOnID,
                                 PurCopyType = (int)i.PurCopyType,
                                 AdmissionDate = !i.AdmissionDate.HasValue || i.AdmissionDate == default ? "" : i.AdmissionDate.GetValueOrDefault().ToString("dd-MM-yyyy"),
                                 Detials = i.Details,
                                 ExpirationDate = i.ExpireDate == default ? string.Empty : i.ExpireDate.ToString("dd-MM-yyyy"),
                                 LineID = DateTime.Now.Ticks.ToString() + i.ID,
                                 Location = i.Location,

                                 MfrDate = !i.MfrDate.HasValue || i.MfrDate == default ? "" : i.MfrDate.GetValueOrDefault().ToString("dd-MM-yyyy"),

                                 MfrWarrantyEnd = !i.MfrWarDateEnd.HasValue || i.MfrWarDateEnd == default ? "" : i.MfrWarDateEnd.GetValueOrDefault().ToString("dd-MM-yyyy"),
                                 MfrWarrantyStart = !i.MfrWarDateStart.HasValue || i.MfrWarDateStart == default ? "" : i.MfrWarDateStart.GetValueOrDefault().ToString("dd-MM-yyyy"),

                                 PlateNumber = i.PlateNumber,
                                 Color = i.Color,
                                 Brand = i.Brand,
                                 Condition = i.Condition,
                                 Type = i.Type,
                                 Power = i.Power,
                                 Year = i.Year,


                                 Qty = (decimal)i.InStock,
                                 SerialNumber = i.SerialNumber,
                                 LotNumber = i.LotNumber,
                                 MfrSerialNo = i.MfrSerialNumber == "NULL" ? "" : i.MfrSerialNumber,

                             }).ToList();
            if (!isAll)
            {
                aPCSNDDetials = aPCSNDDetials.Where(i => i.BPID == bpId).ToList();
            }
            List<APCSNDSelectedDetail> aPCSNDs = JsonConvert.DeserializeObject<List<APCSNDSelectedDetail>>(apsds, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            if (aPCSNDs.Count > 0)
            {
                foreach (var ap in aPCSNDDetials.ToList())
                {
                    if (aPCSNDs.Any(i => i.SerialNumber == ap.SerialNumber)) aPCSNDDetials.Remove(ap);
                }

            }
            APCSerialNumberDetial data = new()
            {
                APCSNDDetials = aPCSNDDetials.OrderBy(i => i.SerialNumber).ToList(),
                TotalAvailableQty = aPCSNDDetials.Sum(i => i.Qty),
            };
            return await Task.FromResult(data);
        }
        public async Task<APCBatchNoUnselect> GetBatchNoDetialsAsync(decimal cost, int bpId, int itemId, int uomID)
        {
            var item = _context.ItemMasterDatas.Find(itemId) ?? new ItemMasterData();
            var uom = _context.GroupDUoMs.FirstOrDefault(w => w.GroupUoMID == item.GroupUomID && w.AltUOM == uomID) ?? new GroupDUoM();
            List<APCBatchNoUnselectDetial> aPCBatchNoUnselectDetials = await _context.WarehouseDetails
                .Where(i => i.InStock > 0 && (decimal)i.Cost == cost && i.BPID == bpId && i.ItemID == itemId && !String.IsNullOrEmpty(i.BatchNo))
                .Select(i => new APCBatchNoUnselectDetial
                {
                    AvailableQty = (decimal)(i.InStock / uom.Factor),
                    BatchNo = i.BatchNo,
                    SelectedQty = 0,
                    UnitCost = (decimal)(i.Cost * uom.Factor),
                    BPID = i.BPID,
                    OrigialQty = (decimal)(i.InStock / uom.Factor),
                }).ToListAsync();
            APCBatchNoUnselect data = new()
            {
                APCBatchNoUnselectDetials = aPCBatchNoUnselectDetials.OrderBy(i => i.BatchNo).ToList(),
                TotalAvailableQty = aPCBatchNoUnselectDetials.Sum(i => i.AvailableQty),
            };
            return await Task.FromResult(data);
        }
        public async Task<PurchaseOrderUpdateViewModel> CopyPurchaseQuotationAsync(int seriesID, string number, int comId)
        {
            bool copied = false;
            int count1, count2 = 0;
            var data = await FindPurchaseQuotationAsync(seriesID, number, comId);
            count1 = data.PurchaseOrderDetials.Count;
            data.PurchaseOrderDetials = data.PurchaseOrderDetials.Where(s => s.OpenQty > 0).ToList();
            count2 = data.PurchaseOrderDetials.Count;
            copied = count1 != count2 ? true : false;
            data.PurchaseOrder.BaseOnID = data.PurchaseOrder.ID;
            data.PurchaseOrder.FreightPurchaseView.ID = 0;
            data.PurchaseOrder.FreightPurchaseView.PurID = 0;
            data.PurchaseOrder.FreightPurchaseView.FreightPurchaseDetailViewModels.ToList().ForEach(j =>
                {
                    j.ID = 0;
                    j.FreightPurchaseID = 0;
                });

            data.PurchaseOrderDetials.ForEach(i =>
            {
                if (i.Qty != i.OpenQty)
                {
                    copied = true;
                }
                i.Qty = i.OpenQty;
                i.DiscountValue = i.DiscountRate == 0 ? 0 : (i.DiscountRate / 100) * (i.OpenQty * i.PurchasPrice);
                i.Total = (i.OpenQty * i.PurchasPrice) - i.DiscountValue;
                i.TotalSys = i.Total * data.PurchaseOrder.PurRate;

                i.TaxValue = i.TaxRate == 0 ? 0 : i.Total * (i.TaxRate / 100);
                i.TotalWTax = i.Total + i.TaxValue;
                i.TotalWTaxSys = (i.TotalWTax * data.PurchaseOrder.PurRate);
                i.FinDisValue = i.FinDisRate == 0 ? 0 : (i.FinDisRate / 100) * i.Total;
                i.FinTotalValue = i.Total - i.FinDisValue;
                i.TaxOfFinDisValue = i.TaxRate == 0 ? 0 : i.FinTotalValue * (i.TaxRate / 100);
                i.LineID = i.LineID;
                i.PurCopyType = PurCopyType.PurQuote;
                i.PurchaseOrderDetailID = 0;
                i.ID = 0;

            });
            if (copied)
            {
                data.PurchaseOrder.FreightPurchaseView.ExpenceAmount = 0;
                data.PurchaseOrder.FreightPurchaseView.OpenExpenceAmount = 0;
                data.PurchaseOrder.FreightPurchaseView.TaxSumValue = 0;
                data.PurchaseOrder.FrieghtAmount = 0;
                data.PurchaseOrder.FrieghtAmountSys = 0;
                data.PurchaseOrder.FreightPurchaseView.FreightPurchaseDetailViewModels.ToList().ForEach(j =>
                {
                    j.ID = 0;
                    j.TaxGroupID = 0;
                    j.FreightPurchaseID = 0;
                    j.TaxRate = 0;
                    j.Amount = 0;
                    j.AmountWithTax = 0;
                    j.TotalTaxAmount = 0;
                    j.TaxGroupSelect.ForEach(k =>
                    {
                        k.Selected = false;
                    });
                });
            }
            data.PurchaseOrder.SubTotal = data.PurchaseOrderDetials.Sum(s => s.Total);
            data.PurchaseOrder.SubTotalSys = data.PurchaseOrder.PurRate;

            data.PurchaseOrder.DiscountValue = data.PurchaseOrder.DiscountRate == 0 ? 0 : (data.PurchaseOrder.DiscountRate / 100) * data.PurchaseOrder.SubTotal;
            data.PurchaseOrder.SubTotalAfterDis = data.PurchaseOrder.SubTotal - data.PurchaseOrder.DiscountValue;
            data.PurchaseOrder.SubTotalAfterDisSys = data.PurchaseOrder.SubTotalAfterDis * data.PurchaseOrder.PurRate;
            data.PurchaseOrder.TaxValue = data.PurchaseOrderDetials.Sum(s => s.TaxOfFinDisValue) + data.PurchaseOrder.FreightPurchaseView.FreightPurchaseDetailViewModels.Sum(s => s.TotalTaxAmount);
            data.PurchaseOrder.TaxRate = data.PurchaseOrder.TaxValue == 0 ? 0 : (data.PurchaseOrder.TaxValue / (data.PurchaseOrder.SubTotalAfterDis + data.PurchaseOrder.FrieghtAmount)) * 100;
            data.PurchaseOrder.BalanceDue = data.PurchaseOrder.TaxValue + data.PurchaseOrder.SubTotalAfterDis + data.PurchaseOrder.FrieghtAmount;
            data.PurchaseOrder.PurchaseOrderID = 0;
            return data;
        }

        public async Task<PurchaseOrderUpdateViewModel> CopyPurchaseRequestAsync(int seriesID, string number, int comId)
        {
            bool copied = false;
            int count1, count2 = 0;
            var data = await FindPurchaseRequestAsync(seriesID, number, comId);
            count1 = data.PurchaseOrderDetials.Count;
            data.PurchaseOrderDetials = data.PurchaseOrderDetials.Where(s => s.OpenQty > 0).ToList();
            count2 = data.PurchaseOrderDetials.Count;
            copied = count1 != count2 ? true : false;
            data.PurchaseOrder.BaseOnID = data.PurchaseOrder.ID;
            data.PurchaseOrder.VendorID = 0;
            data.PurchaseOrder.FreightPurchaseView.ID = 0;
            data.PurchaseOrder.FreightPurchaseView.PurID = 0;
            data.PurchaseOrder.FreightPurchaseView.FreightPurchaseDetailViewModels.ToList().ForEach(j =>
                {
                    j.ID = 0;
                    j.FreightPurchaseID = 0;
                });

            data.PurchaseOrderDetials.ForEach(i =>
            {
                if (i.Qty != i.OpenQty)
                {
                    copied = true;
                }
                i.Qty = i.OpenQty;
                i.DiscountValue = i.DiscountRate == 0 ? 0 : (i.DiscountRate / 100) * (i.OpenQty * i.PurchasPrice);
                i.Total = (i.OpenQty * i.PurchasPrice) - i.DiscountValue;
                i.TotalSys = i.Total * data.PurchaseOrder.PurRate;

                i.TaxValue = i.TaxRate == 0 ? 0 : i.Total * (i.TaxRate / 100);
                i.TotalWTax = i.Total + i.TaxValue;
                i.TotalWTaxSys = (i.TotalWTax * data.PurchaseOrder.PurRate);
                i.FinDisValue = i.FinDisRate == 0 ? 0 : (i.FinDisRate / 100) * i.Total;
                i.FinTotalValue = i.Total - i.FinDisValue;
                i.TaxOfFinDisValue = i.TaxRate == 0 ? 0 : i.FinTotalValue * (i.TaxRate / 100);
                i.LineID = i.LineID;
                i.PurCopyType = PurCopyType.PurRequest;
                i.PurchaseOrderDetailID = 0;
                i.ID = 0;

            });
            if (copied)
            {
                data.PurchaseOrder.FreightPurchaseView.ExpenceAmount = 0;
                data.PurchaseOrder.FreightPurchaseView.OpenExpenceAmount = 0;
                data.PurchaseOrder.FreightPurchaseView.TaxSumValue = 0;
                data.PurchaseOrder.FrieghtAmount = 0;
                data.PurchaseOrder.FrieghtAmountSys = 0;
                data.PurchaseOrder.FreightPurchaseView.FreightPurchaseDetailViewModels.ToList().ForEach(j =>
                {
                    j.ID = 0;
                    j.TaxGroupID = 0;
                    j.FreightPurchaseID = 0;
                    j.TaxRate = 0;
                    j.Amount = 0;
                    j.AmountWithTax = 0;
                    j.TotalTaxAmount = 0;
                    j.TaxGroupSelect.ForEach(k =>
                    {
                        k.Selected = false;
                    });
                });
            }
            data.PurchaseOrder.SubTotal = data.PurchaseOrderDetials.Sum(s => s.Total);
            data.PurchaseOrder.SubTotalSys = data.PurchaseOrder.PurRate;

            data.PurchaseOrder.DiscountValue = data.PurchaseOrder.DiscountRate == 0 ? 0 : (data.PurchaseOrder.DiscountRate / 100) * data.PurchaseOrder.SubTotal;
            data.PurchaseOrder.SubTotalAfterDis = data.PurchaseOrder.SubTotal - data.PurchaseOrder.DiscountValue;
            data.PurchaseOrder.SubTotalAfterDisSys = data.PurchaseOrder.SubTotalAfterDis * data.PurchaseOrder.PurRate;
            data.PurchaseOrder.TaxValue = data.PurchaseOrderDetials.Sum(s => s.TaxOfFinDisValue) + data.PurchaseOrder.FreightPurchaseView.FreightPurchaseDetailViewModels.Sum(s => s.TotalTaxAmount);
            data.PurchaseOrder.TaxRate = data.PurchaseOrder.TaxValue == 0 ? 0 : (data.PurchaseOrder.TaxValue / (data.PurchaseOrder.SubTotalAfterDis + data.PurchaseOrder.FrieghtAmount)) * 100;
            data.PurchaseOrder.BalanceDue = data.PurchaseOrder.TaxValue + data.PurchaseOrder.SubTotalAfterDis + data.PurchaseOrder.FrieghtAmount;
            data.PurchaseOrder.PurchaseOrderID = 0;
            return data;
        }
        public async Task<PurchaseOrderUpdateViewModel> CopyPurchaseOrderAsync(int seriesID, string number, int comId)
        {

            bool copied = false;
            int count1 = 0;
            int count2 = 0;
            var data = await FindPurchaseOrderAsync(seriesID, number, comId);
            count1 = data.PurchaseOrderDetials.Count;
            data.PurchaseOrderDetials = data.PurchaseOrderDetials.Where(s => s.OpenQty > 0).ToList();
            count2 = data.PurchaseOrderDetials.Count;
            if (count1 != count2)
            {
                copied = true;
            }
            data.PurchaseOrder.BaseOnID = data.PurchaseOrder.PurchaseOrderID;
            data.PurchaseOrder.FreightPurchaseView.ID = 0;
            data.PurchaseOrder.FreightPurchaseView.PurID = 0;
            data.PurchaseOrder.FreightPurchaseView.FreightPurchaseDetailViewModels.ToList().ForEach(j =>
                {
                    j.ID = 0;
                    j.FreightPurchaseID = 0;
                });

            data.PurchaseOrderDetials.ForEach(i =>
            {
                if (i.Qty != i.OpenQty)
                {
                    copied = true;
                }
                i.Qty = i.OpenQty;
                i.DiscountValue = i.DiscountRate == 0 ? 0 : (i.DiscountRate / 100) * (i.OpenQty * i.PurchasPrice);
                i.Total = (i.OpenQty * i.PurchasPrice) - i.DiscountValue;
                i.TotalSys = i.Total * data.PurchaseOrder.PurRate;

                i.TaxValue = i.TaxRate == 0 ? 0 : i.Total * (i.TaxRate / 100);
                i.TotalWTax = i.Total + i.TaxValue;
                i.TotalWTaxSys = (i.TotalWTax * data.PurchaseOrder.PurRate);
                i.FinDisValue = (i.FinDisRate / 100) * i.Total;
                i.FinTotalValue = i.Total - i.FinDisValue;
                i.TaxOfFinDisValue = i.TaxRate == 0 ? 0 : i.FinTotalValue * (i.TaxRate / 100);
                i.LineID = i.LineID;
                i.PurCopyType = PurCopyType.PurOrder;
                i.ID = 0;
                i.PurchaseOrderDetailID = 0;

            });
            if (copied)
            {
                data.PurchaseOrder.FreightPurchaseView.ExpenceAmount = 0;
                data.PurchaseOrder.FreightPurchaseView.OpenExpenceAmount = 0;
                data.PurchaseOrder.FreightPurchaseView.TaxSumValue = 0;
                data.PurchaseOrder.FrieghtAmount = 0;
                data.PurchaseOrder.FrieghtAmountSys = 0;
                data.PurchaseOrder.FreightPurchaseView.FreightPurchaseDetailViewModels.ToList().ForEach(j =>
                {
                    j.ID = 0;
                    j.TaxGroupID = 0;
                    j.FreightPurchaseID = 0;
                    j.TaxRate = 0;
                    j.Amount = 0;
                    j.AmountWithTax = 0;
                    j.TotalTaxAmount = 0;
                    j.TaxGroupSelect.ForEach(k =>
                    {
                        k.Selected = false;
                    });
                });
            }
            data.PurchaseOrder.SubTotal = data.PurchaseOrderDetials.Sum(s => s.Total);
            data.PurchaseOrder.SubTotalSys = data.PurchaseOrder.PurRate;

            data.PurchaseOrder.DiscountValue = data.PurchaseOrder.DiscountRate == 0 ? 0 : (data.PurchaseOrder.DiscountRate / 100) * data.PurchaseOrder.SubTotal;
            data.PurchaseOrder.SubTotalAfterDis = data.PurchaseOrder.SubTotal - data.PurchaseOrder.DiscountValue;
            data.PurchaseOrder.SubTotalAfterDisSys = data.PurchaseOrder.SubTotalAfterDis * data.PurchaseOrder.PurRate;
            data.PurchaseOrder.TaxValue = data.PurchaseOrderDetials.Sum(s => s.TaxOfFinDisValue) + data.PurchaseOrder.FreightPurchaseView.FreightPurchaseDetailViewModels.Sum(s => s.TotalTaxAmount);
            data.PurchaseOrder.TaxRate = data.PurchaseOrder.TaxValue == 0 ? 0 : (data.PurchaseOrder.TaxValue / (data.PurchaseOrder.SubTotalAfterDis + data.PurchaseOrder.FrieghtAmount)) * 100;
            data.PurchaseOrder.BalanceDue = data.PurchaseOrder.TaxValue + data.PurchaseOrder.SubTotalAfterDis + data.PurchaseOrder.FrieghtAmount;
            data.PurchaseOrder.PurchaseOrderID = 0;
            return data;
        }

        public async Task<PurchaseAPUpdateViewModel> CopyPurchaseOrder(int seriesID, string number, int comId)
        {
            bool copied = false;
            int count1 = 0;
            int count2 = 0;
            var data = await FindPurchaseOrder(seriesID, number, comId);
            count1 = data.PurchaseAPDetials.Count;
            data.PurchaseAPDetials = data.PurchaseAPDetials.Where(s => s.OpenQty > 0).ToList();
            count2 = data.PurchaseAPDetials.Count;
            if (count1 != count2)
            {
                copied = true;
            }
            data.PurchaseAP.BaseOnID = data.PurchaseAP.PurchaseAPID;
            data.PurchaseAP.FreightPurchaseView.ID = 0;
            data.PurchaseAP.FreightPurchaseView.PurID = 0;
            data.PurchaseAP.FreightPurchaseView.FreightPurchaseDetailViewModels.ToList().ForEach(j =>
                {
                    j.ID = 0;
                    j.FreightPurchaseID = 0;
                });

            data.PurchaseAPDetials.ForEach(i =>
            {

                if (i.Qty != i.OpenQty)
                {
                    copied = true;
                }
                i.Qty = i.OpenQty;
                i.DiscountValue = i.DiscountRate == 0 ? 0 : (i.DiscountRate / 100) * (i.OpenQty * i.PurchasPrice);
                i.Total = (i.OpenQty * i.PurchasPrice) - i.DiscountValue;
                i.TotalSys = i.Total * data.PurchaseAP.PurRate;

                i.TaxValue = i.TaxRate == 0 ? 0 : i.Total * (i.TaxRate / 100);
                i.TotalWTax = i.Total + i.TaxValue;
                i.TotalWTaxSys = (i.TotalWTax * data.PurchaseAP.PurRate);
                i.FinDisValue = i.FinDisRate == 0 ? 0 : (i.FinDisRate / 100) * i.Total;
                i.FinTotalValue = i.Total - i.FinDisValue;
                i.TaxOfFinDisValue = i.TaxRate == 0 ? 0 : i.FinTotalValue * (i.TaxRate / 100);
                i.LineID = i.LineID;
                i.PurCopyType = PurCopyType.PurOrder;
                i.PurchaseDetailAPID = 0;


            });
            if (copied)
            {
                data.PurchaseAP.FreightPurchaseView.ExpenceAmount = 0;
                data.PurchaseAP.FreightPurchaseView.OpenExpenceAmount = 0;
                data.PurchaseAP.FreightPurchaseView.TaxSumValue = 0;
                data.PurchaseAP.FrieghtAmount = 0;
                data.PurchaseAP.FrieghtAmountSys = 0;
                data.PurchaseAP.FreightPurchaseView.FreightPurchaseDetailViewModels.ToList().ForEach(j =>
                {
                    j.ID = 0;
                    j.TaxGroupID = 0;
                    j.FreightPurchaseID = 0;
                    j.TaxRate = 0;
                    j.Amount = 0;
                    j.AmountWithTax = 0;
                    j.TotalTaxAmount = 0;
                    j.TaxGroupSelect.ForEach(k =>
                    {
                        k.Selected = false;
                    });
                });
            }
            data.PurchaseAP.SubTotal = data.PurchaseAPDetials.Sum(s => s.Total);
            data.PurchaseAP.SubTotalSys = data.PurchaseAP.PurRate;

            data.PurchaseAP.DiscountValue = data.PurchaseAP.DiscountRate == 0 ? 0 : (data.PurchaseAP.DiscountRate / 100) * data.PurchaseAP.SubTotal;
            data.PurchaseAP.SubTotalAfterDis = data.PurchaseAP.SubTotal - data.PurchaseAP.DiscountValue;
            data.PurchaseAP.SubTotalAfterDisSys = data.PurchaseAP.SubTotalAfterDis * data.PurchaseAP.PurRate;
            data.PurchaseAP.TaxValue = data.PurchaseAPDetials.Sum(s => s.TaxOfFinDisValue) + data.PurchaseAP.FreightPurchaseView.FreightPurchaseDetailViewModels.Sum(s => s.TotalTaxAmount);
            data.PurchaseAP.TaxRate = data.PurchaseAP.TaxValue == 0 ? 0 : (data.PurchaseAP.TaxValue / (data.PurchaseAP.SubTotalAfterDis + data.PurchaseAP.FrieghtAmount)) * 100;
            data.PurchaseAP.BalanceDue = data.PurchaseAP.TaxValue + data.PurchaseAP.SubTotalAfterDis + data.PurchaseAP.FrieghtAmount;
            data.PurchaseAP.PurchaseAPID = 0;
            return data;
        }
        public async Task<PurchasePOUpdateViewModel> CopyPurchasePOAsync(int seriesID, string number, int comId)
        {
            bool copied = false;
            int count1 = 0;
            int count2 = 0;
            var data = await FindPurchasePOAsync(seriesID, number, comId);
            count1 = data.PurchasePODetials.Count;
            data.PurchasePODetials = data.PurchasePODetials.Where(s => s.OpenQty > 0).ToList();
            count2 = data.PurchasePODetials.Count;
            if (count1 != count2)
            {
                copied = true;
            }
            data.PurchasePO.BaseOnID = data.PurchasePO.ID;
            data.PurchasePO.FreightPurchaseView.ID = 0;
            data.PurchasePO.FreightPurchaseView.PurID = 0;
            data.PurchasePO.FreightPurchaseView.FreightPurchaseDetailViewModels.ToList().ForEach(j =>
                {
                    j.ID = 0;
                    j.FreightPurchaseID = 0;
                });

            data.PurchasePODetials.ForEach(i =>
            {
                if (i.Qty != i.OpenQty)
                {
                    copied = true;
                }
                i.Qty = i.OpenQty;
                i.DiscountValue = i.DiscountRate == 0 ? 0 : (i.DiscountRate / 100) * (i.OpenQty * i.PurchasPrice);
                i.Total = (i.OpenQty * i.PurchasPrice) - i.DiscountValue;
                i.TotalSys = i.Total * data.PurchasePO.PurRate;

                i.TaxValue = i.DiscountRate == 0 ? 0 : i.Total * (i.DiscountRate / 100);
                i.TotalWTax = i.Total + i.TaxValue;
                i.TotalWTaxSys = (i.TotalWTax * data.PurchasePO.PurRate);
                i.FinDisValue = i.FinDisRate == 0 ? 0 : (i.FinDisRate / 100) * i.Total;
                i.FinTotalValue = i.Total - i.FinDisValue;
                i.TaxOfFinDisValue = i.TaxRate == 0 ? 0 : i.FinTotalValue * (i.TaxRate / 100);
                i.LineID = i.LineID;
                i.PurCopyType = PurCopyType.GRPO;
                i.ID = 0;

            });
            if (copied)
            {
                data.PurchasePO.FreightPurchaseView.ExpenceAmount = 0;
                data.PurchasePO.FreightPurchaseView.OpenExpenceAmount = 0;
                data.PurchasePO.FreightPurchaseView.TaxSumValue = 0;
                data.PurchasePO.FrieghtAmount = 0;
                data.PurchasePO.FrieghtAmountSys = 0;
                data.PurchasePO.FreightPurchaseView.FreightPurchaseDetailViewModels.ToList().ForEach(j =>
                {
                    j.ID = 0;
                    j.TaxGroupID = 0;
                    j.FreightPurchaseID = 0;
                    j.TaxRate = 0;
                    j.Amount = 0;
                    j.AmountWithTax = 0;
                    j.TotalTaxAmount = 0;
                    j.TaxGroupSelect.ForEach(k =>
                    {
                        k.Selected = false;
                    });
                });
            }
            data.PurchasePO.SubTotal = data.PurchasePODetials.Sum(s => s.Total);
            data.PurchasePO.SubTotalSys = data.PurchasePO.PurRate;

            data.PurchasePO.DiscountValue = data.PurchasePO.DiscountRate == 0 ? 0 : (data.PurchasePO.DiscountRate / 100) * data.PurchasePO.SubTotal;
            data.PurchasePO.SubTotalAfterDis = data.PurchasePO.SubTotal - data.PurchasePO.DiscountValue;
            data.PurchasePO.SubTotalAfterDisSys = data.PurchasePO.SubTotalAfterDis * data.PurchasePO.PurRate;
            data.PurchasePO.TaxValue = data.PurchasePODetials.Sum(s => s.TaxOfFinDisValue) + data.PurchasePO.FreightPurchaseView.FreightPurchaseDetailViewModels.Sum(s => s.TotalTaxAmount);
            data.PurchasePO.TaxRate = data.PurchasePO.TaxValue == 0 ? 0 : (data.PurchasePO.TaxValue / (data.PurchasePO.SubTotalAfterDis + data.PurchasePO.FrieghtAmount)) * 100;
            data.PurchasePO.BalanceDue = data.PurchasePO.TaxValue + data.PurchasePO.SubTotalAfterDis + data.PurchasePO.FrieghtAmount;
            data.PurchasePO.ID = 0;
            return data;
        }
        public async Task<PurchaseAPUpdateViewModel> CopyPurchaseQuote(int seriesID, string number, int comId)
        {
            bool copied = false;
            int count1 = 0;
            int count2 = 0;
            var data = await FindPurchaseQuote(seriesID, number, comId);
            count1 = data.PurchaseAPDetials.Count;
            data.PurchaseAPDetials = data.PurchaseAPDetials.Where(s => s.OpenQty > 0).ToList();
            count2 = data.PurchaseAPDetials.Count;
            if (count1 != count2)
            {
                copied = true;
            }
            data.PurchaseAP.BaseOnID = data.PurchaseAP.PurchaseAPID;
            data.PurchaseAP.FreightPurchaseView.ID = 0;
            data.PurchaseAP.FreightPurchaseView.PurID = 0;
            data.PurchaseAP.FreightPurchaseView.FreightPurchaseDetailViewModels.ToList().ForEach(j =>
                {
                    j.ID = 0;
                    j.FreightPurchaseID = 0;
                });

            data.PurchaseAPDetials.ForEach(i =>
            {
                if (i.Qty != i.OpenQty)
                {
                    copied = true;
                }
                i.Qty = i.OpenQty;
                i.DiscountValue = i.DiscountRate == 0 ? 0 : (i.DiscountRate / 100) * (i.OpenQty * i.PurchasPrice);
                i.Total = (i.OpenQty * i.PurchasPrice) - i.DiscountValue;
                i.TotalSys = i.Total * data.PurchaseAP.PurRate;

                i.TaxValue = i.TaxRate == 0 ? 0 : i.Total * (i.TaxRate / 100);
                i.TotalWTax = i.Total + i.TaxValue;
                i.TotalWTaxSys = (i.TotalWTax * data.PurchaseAP.PurRate);
                i.FinDisValue = i.FinDisRate == 0 ? 0 : (i.FinDisRate / 100) * i.Total;
                i.FinTotalValue = i.Total - i.FinDisValue;
                i.TaxOfFinDisValue = i.TaxRate == 0 ? 0 : i.FinTotalValue * (i.TaxRate / 100);
                i.LineID = i.LineID;
                i.PurCopyType = PurCopyType.PurQuote;
                i.PurchaseDetailAPID = 0;


            });
            if (copied)
            {
                data.PurchaseAP.FreightPurchaseView.ExpenceAmount = 0;
                data.PurchaseAP.FreightPurchaseView.OpenExpenceAmount = 0;
                data.PurchaseAP.FreightPurchaseView.TaxSumValue = 0;
                data.PurchaseAP.FrieghtAmount = 0;
                data.PurchaseAP.FrieghtAmountSys = 0;
                data.PurchaseAP.FreightPurchaseView.FreightPurchaseDetailViewModels.ToList().ForEach(j =>
                {
                    j.ID = 0;
                    j.TaxGroupID = 0;
                    j.FreightPurchaseID = 0;
                    j.TaxRate = 0;
                    j.Amount = 0;
                    j.AmountWithTax = 0;
                    j.TotalTaxAmount = 0;
                    j.TaxGroupSelect.ForEach(k =>
                    {
                        k.Selected = false;
                    });
                });
            }
            data.PurchaseAP.SubTotal = data.PurchaseAPDetials.Sum(s => s.Total);
            data.PurchaseAP.SubTotalSys = data.PurchaseAP.PurRate;

            data.PurchaseAP.DiscountValue = data.PurchaseAP.DiscountRate == 0 ? 0 : (data.PurchaseAP.DiscountRate / 100) * data.PurchaseAP.SubTotal;
            data.PurchaseAP.SubTotalAfterDis = data.PurchaseAP.SubTotal - data.PurchaseAP.DiscountValue;
            data.PurchaseAP.SubTotalAfterDisSys = data.PurchaseAP.SubTotalAfterDis * data.PurchaseAP.PurRate;
            data.PurchaseAP.TaxValue = data.PurchaseAPDetials.Sum(s => s.TaxOfFinDisValue) + data.PurchaseAP.FreightPurchaseView.FreightPurchaseDetailViewModels.Sum(s => s.TotalTaxAmount);
            data.PurchaseAP.TaxRate = data.PurchaseAP.TaxValue == 0 ? 0 : (data.PurchaseAP.TaxValue / (data.PurchaseAP.SubTotalAfterDis + data.PurchaseAP.FrieghtAmount)) * 100;
            data.PurchaseAP.BalanceDue = data.PurchaseAP.TaxValue + data.PurchaseAP.SubTotalAfterDis + data.PurchaseAP.FrieghtAmount;
            data.PurchaseAP.PurchaseAPID = 0;
            return data;

        }
        public async Task<PurchaseAPUpdateViewModel> CopyPurchaseAPAsync(int seriesID, string number, int comId)
        {
            bool copied = false;
            int count1, count2 = 0;
            var data = await FindPurchaseAPAsync(seriesID, number, comId);
            count1 = data.PurchaseAPDetials.Count;
            data.PurchaseAPDetials = data.PurchaseAPDetials.Where(i => i.OpenQty > 0).ToList();
            count2 = data.PurchaseAPDetials.Count;
            copied = count1 != count2 ? true : false;
            data.PurchaseAP.FreightPurchaseView.FreightPurchaseDetailViewModels.ToList().ForEach(j =>
                            {
                                j.ID = 0;
                                j.FreightPurchaseID = 0;
                            });
            data.PurchaseAP.FreightPurchaseView.ID = 0;
            data.PurchaseAP.FreightPurchaseView.PurID = 0;


            data.PurchaseAPDetials.ForEach(i =>
            {
                if (i.Qty != i.OpenQty)
                {
                    copied = true;
                }
                i.Qty = i.OpenQty;
                i.DiscountValue = i.DiscountRate == 0 ? 0 : (i.DiscountRate / 100) * (i.OpenQty * i.PurchasPrice);
                i.Total = (i.OpenQty * i.PurchasPrice) - i.DiscountValue;
                i.TotalSys = i.Total * data.PurchaseAP.PurRate;

                i.TaxValue = i.TaxRate == 0 ? 0 : i.Total * (i.TaxRate / 100);
                i.TotalWTax = i.Total + i.TaxValue;
                i.TotalWTaxSys = (i.TotalWTax * data.PurchaseAP.PurRate);
                i.FinDisValue = i.FinDisRate == 0 ? 0 : (i.FinDisRate / 100) * i.Total;
                i.FinTotalValue = i.Total - i.FinDisValue;
                i.TaxOfFinDisValue = i.TaxRate == 0 ? 0 : i.FinTotalValue * (i.TaxRate / 100);
                i.LineID = i.PurchaseDetailAPID;
                i.PurCopyType = PurCopyType.PurAP;
                i.PurchaseDetailAPID = 0;


            });
            if (copied)
            {
                data.PurchaseAP.FreightPurchaseView.ExpenceAmount = 0;
                data.PurchaseAP.FreightPurchaseView.OpenExpenceAmount = 0;
                data.PurchaseAP.FreightPurchaseView.TaxSumValue = 0;
                data.PurchaseAP.FrieghtAmount = 0;
                data.PurchaseAP.FrieghtAmountSys = 0;
                data.PurchaseAP.FreightPurchaseView.FreightPurchaseDetailViewModels.ToList().ForEach(j =>
                {
                    j.ID = 0;
                    j.TaxGroupID = 0;
                    j.FreightPurchaseID = 0;
                    j.TaxRate = 0;
                    j.Amount = 0;
                    j.AmountWithTax = 0;
                    j.TotalTaxAmount = 0;
                    j.TaxGroupSelect.ForEach(k =>
                    {
                        k.Selected = false;
                    });
                });
            }
            data.PurchaseAP.SubTotal = data.PurchaseAPDetials.Sum(s => s.Total);
            data.PurchaseAP.SubTotalSys = data.PurchaseAP.PurRate * data.PurchaseAP.SubTotal;
            // data.PurchaseAP.SubTotalAfDis  =data.PurchaseAP.SubTotal;
            // data.PurchaseAP.SubTotalBefDisSys = data.PurchaseAP.SubTotalSys;
            data.PurchaseAP.DiscountValue = data.PurchaseAP.DiscountRate == 0 ? 0 : (data.PurchaseAP.DiscountRate / 100) * data.PurchaseAP.SubTotal;
            data.PurchaseAP.SubTotalAfterDis = data.PurchaseAP.SubTotal - data.PurchaseAP.DiscountValue;
            data.PurchaseAP.SubTotalAfterDisSys = data.PurchaseAP.SubTotalAfterDis * data.PurchaseAP.PurRate;
            data.PurchaseAP.TaxValue = data.PurchaseAPDetials.Sum(s => s.TaxOfFinDisValue) + data.PurchaseAP.FreightPurchaseView.FreightPurchaseDetailViewModels.Sum(s => s.TotalTaxAmount);
            data.PurchaseAP.TaxRate = data.PurchaseAP.TaxValue == 0 ? 0 : (data.PurchaseAP.TaxValue / (data.PurchaseAP.SubTotalAfterDis + data.PurchaseAP.FrieghtAmount)) * 100;
            data.PurchaseAP.BalanceDue = data.PurchaseAP.TaxValue + data.PurchaseAP.SubTotalAfterDis + data.PurchaseAP.FrieghtAmount;
            data.PurchaseAP.PurchaseAPID = 0;
            return data;

        }
        public async Task<PurchaseAPUpdateViewModel> CopyPurchaseAPReserveAsync(int seriesID, string number, int comId)
        {
            bool copied = false;
            int count1, count2 = 0;
            var data = await FindPurchaseAPReserveAsync(seriesID, number, comId);
            count1 = data.PurchaseAPDetials.Count;
            data.PurchaseAPDetials = data.PurchaseAPDetials.Where(i => i.OpenQty > 0).ToList();
            count2 = data.PurchaseAPDetials.Count;
            copied = count1 != count2 ? true : false;    // PurchaseAP = purAP.FirstOrDefault(),
            data.PurchaseAP.FreightPurchaseView.FreightPurchaseDetailViewModels.ToList().ForEach(j =>
                {
                    j.ID = 0;
                    j.FreightPurchaseID = 0;
                });
            data.PurchaseAP.FreightPurchaseView.ID = 0;
            data.PurchaseAP.FreightPurchaseView.PurID = 0;
            data.PurchaseAPDetials.ForEach(i =>
            {
                if (i.Qty != i.OpenQty)
                {
                    copied = true;
                }
                i.Qty = i.OpenQty;

                i.DiscountValue = i.DiscountRate == 0 ? 0 : (i.DiscountRate / 100) * (i.OpenQty * i.PurchasPrice);
                i.Total = (i.OpenQty * i.PurchasPrice) - i.DiscountValue;
                i.TotalSys = i.Total * data.PurchaseAP.PurRate;

                i.TaxValue = i.TaxRate == 0 ? 0 : i.Total * (i.TaxRate / 100);
                i.TotalWTax = i.Total + i.TaxValue;
                i.TotalWTaxSys = (i.TotalWTax * data.PurchaseAP.PurRate);
                i.FinDisValue = i.FinDisRate == 0 ? 0 : (i.FinDisRate / 100) * i.Total;
                i.FinTotalValue = i.Total - i.FinDisValue;
                i.TaxOfFinDisValue = i.TaxRate == 0 ? 0 : i.FinTotalValue * (i.TaxRate / 100);
                i.LineID = i.LineID;
                i.PurCopyType = PurCopyType.PurReserve;
                i.PurchaseDetailAPID = 0;


            });
            if (copied)
            {
                data.PurchaseAP.FreightPurchaseView.ExpenceAmount = 0;
                data.PurchaseAP.FreightPurchaseView.OpenExpenceAmount = 0;
                data.PurchaseAP.FreightPurchaseView.TaxSumValue = 0;
                data.PurchaseAP.FrieghtAmount = 0;
                data.PurchaseAP.FrieghtAmountSys = 0;
                data.PurchaseAP.FreightPurchaseView.FreightPurchaseDetailViewModels.ToList().ForEach(j =>
                {
                    j.ID = 0;
                    j.TaxGroupID = 0;
                    j.FreightPurchaseID = 0;
                    j.TaxRate = 0;
                    j.Amount = 0;
                    j.AmountWithTax = 0;
                    j.TotalTaxAmount = 0;
                    j.TaxGroupSelect.ForEach(k =>
                    {
                        k.Selected = false;
                    });
                });
            }
            data.PurchaseAP.SubTotal = data.PurchaseAPDetials.Sum(s => s.Total);
            data.PurchaseAP.SubTotalSys = data.PurchaseAP.PurRate;
            // data.PurchaseAP.SubTotalAfDis  =data.PurchaseAP.SubTotal;
            // data.PurchaseAP.SubTotalBefDisSys = data.PurchaseAP.SubTotalSys;
            data.PurchaseAP.DiscountValue = data.PurchaseAP.DiscountRate == 0 ? 0 : (data.PurchaseAP.DiscountRate / 100) * data.PurchaseAP.SubTotal;
            data.PurchaseAP.SubTotalAfterDis = data.PurchaseAP.SubTotal - data.PurchaseAP.DiscountValue;
            data.PurchaseAP.SubTotalAfterDisSys = data.PurchaseAP.SubTotalAfterDis * data.PurchaseAP.PurRate;
            data.PurchaseAP.TaxValue = data.PurchaseAPDetials.Sum(s => s.TaxOfFinDisValue) + data.PurchaseAP.FreightPurchaseView.FreightPurchaseDetailViewModels.Sum(s => s.TotalTaxAmount);
            data.PurchaseAP.TaxRate = data.PurchaseAP.TaxValue == 0 ? 0 : (data.PurchaseAP.TaxValue / (data.PurchaseAP.SubTotalAfterDis + data.PurchaseAP.FrieghtAmount)) * 100;
            data.PurchaseAP.BalanceDue = data.PurchaseAP.TaxValue + data.PurchaseAP.SubTotalAfterDis + data.PurchaseAP.FrieghtAmount;
            data.PurchaseAP.PurchaseAPID = 0;
            return data;
        }


    }
}
