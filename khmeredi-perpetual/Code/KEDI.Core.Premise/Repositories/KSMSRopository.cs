using CKBS.AppContext;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using KEDI.Core.Premise.Models.ServicesClass.KSMS;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using KEDI.Core.Premise.Models.Services.POS.KSMS;
using CKBS.Models.Services.POS.service;
using KEDI.Core.Premise.Models.Services.Administrator.Inventory;
using CKBS.Models.Services.Inventory;
using CKBS.Models.Services.Administrator.Inventory;
using CKBS.Models.ServicesClass;
using KEDI.Core.Premise.Models.ServicesClass.SerialBatches.Sales;
using CKBS.Models.Services.Production;
using SetGlAccount = CKBS.Models.Services.Inventory.SetGlAccount;
using CKBS.Models.Services.Financials;
using CKBS.Models.Services.ChartOfAccounts;
using CKBS.Models.Services.Administrator.SystemInitialization;
using Type = CKBS.Models.Services.Financials.Type;
using CKBS.Models.Services.Inventory.PriceList;
using CKBS.Models.Services.Administrator.General;
using CKBS.Models.Services.HumanResources;

namespace KEDI.Core.Premise.Repository
{
    public interface IKSMSRopository
    {
        Task<List<KSServiceViewModel>> GetServiceAsync(int plId);
        Task<List<VehicleViewModel>> GetVehiclesAsync(int cusId);
        Task<KSServiceViewModel> GetServiceDetailAsync(int id);
        Task<List<KSServiceViewModel>> GetSoldServiceAsync(int cusid, int plId, string keyword = "");
        Task UpdateKSServiceAsync(KSServiceMaster kSServices, List<SerialNumber> serials, List<BatchNo> batches);
        Task<List<ItemsReturn>> CheckItemOutOfStockAsync(List<KSService> kSServices);
        Task<List<SaleReportKSMS>> GetSaleReportAsync(string fromDate, string toDate, Company com);
    }

    public class KSMSRopository : IKSMSRopository
    {
        private readonly DataContext _context;

        public KSMSRopository(DataContext context)
        {
            _context = context;
        }

        public async Task<List<KSServiceViewModel>> GetServiceAsync(int plId)
        {
            var data = await (from ks in _context.ServiceSetups.Where(i => i.Active && i.PriceListID == plId)
                              join item in _context.ItemMasterDatas on ks.ItemID equals item.ID
                              join uom in _context.UnitofMeasures on ks.UomID equals uom.ID
                              join pl in _context.PriceLists on ks.PriceListID equals pl.ID
                              join _cur in _context.Currency on pl.CurrencyID equals _cur.ID
                              select new KSServiceViewModel
                              {
                                  ItemCode = item.Code,
                                  ItemID = item.ID,
                                  ItemName = item.KhmerName,
                                  LineID = $"{DateTime.Now.Ticks}{ks.ID}",
                                  KSServiceSetupId = ks.ID,
                                  SetupCode = ks.SetupCode,
                                  UoM = uom.Name,
                                  UoMID = ks.UomID,
                                  Cost = ks.Price,
                                  CurName = _cur.Description
                              }).ToListAsync();
            return data;

        }


        public async Task<KSServiceViewModel> GetServiceDetailAsync(int id)
        {
            var data = await (from ks in _context.ServiceSetups.Where(i => i.Active && i.ID == id)
                              join item in _context.ItemMasterDatas on ks.ItemID equals item.ID
                              join uom in _context.UnitofMeasures on ks.UomID equals uom.ID
                              join pl in _context.PriceLists on ks.PriceListID equals pl.ID
                              join _cur in _context.Currency on pl.CurrencyID equals _cur.ID
                              select new KSServiceViewModel
                              {
                                  ItemCode = item.Code,
                                  ItemID = item.ID,
                                  ItemName = item.KhmerName,
                                  LineID = $"{DateTime.Now.Ticks.ToString().Substring(0, 3)}{ks.ID}",
                                  KSServiceSetupId = ks.ID,
                                  SetupCode = ks.SetupCode,
                                  UoM = uom.Name,
                                  UoMID = ks.UomID,
                                  CurName = _cur.Description,
                                  Qty = 1,
                                  Cost = ks.Price,
                                  ServiceSetupDetials = (from sd in _context.ServiceSetupDetials.Where(i => i.ServiceSetupID == ks.ID)
                                                         join itemd in _context.ItemMasterDatas on sd.ItemID equals itemd.ID
                                                         join _uom in _context.UnitofMeasures on sd.UomID equals _uom.ID
                                                         join cur in _context.Currency on sd.CurrencyID equals cur.ID
                                                         select new KSServiceViewModel
                                                         {
                                                             ItemID = sd.ItemID,
                                                             ItemCode = itemd.Code,
                                                             ItemName = itemd.KhmerName,
                                                             KSServiceSetupId = sd.ServiceSetupID,
                                                             LineID = $"{DateTime.Now.Ticks.ToString().Substring(0, 3)}{sd.ID}",
                                                             UoM = _uom.Name,
                                                             UoMID = sd.UomID,
                                                             Qty = sd.Qty,
                                                             Cost = sd.Cost,
                                                             Factor = sd.Factor,
                                                             GUomID = sd.GUomID,
                                                             CurrencyId = sd.CurrencyID,
                                                             CurName = cur.Description
                                                         }).ToList(),
                              }).FirstOrDefaultAsync();
            return data;

        }

        public async Task<List<VehicleViewModel>> GetVehiclesAsync(int cusId)
        {
            var data = await (from cus in _context.BusinessPartners.Where(i => i.ID == cusId)
                              join veh in _context.AutoMobiles on cus.ID equals veh.BusinessPartnerID
                              join type in _context.AutoTypes on veh.TypeID equals type.TypeID
                              join brand in _context.AutoBrands on veh.BrandID equals brand.BrandID
                              join color in _context.AutoColors on veh.ColorID equals color.ColorID
                              join model in _context.AutoModels on veh.ModelID equals model.ModelID
                              select new VehicleViewModel
                              {
                                  ColorID = color.ColorID,
                                  Brand = brand.BrandName,
                                  BrandID = brand.BrandID,
                                  Color = color.ColorName,
                                  Engine = veh.Engine,
                                  Frame = veh.Frame,
                                  ID = veh.AutoMID,
                                  TypeID = type.TypeID,
                                  LineId = DateTime.Now.Ticks.ToString(),
                                  Model = model.ModelName,
                                  ModelID = model.ModelID,
                                  Plate = veh.Plate,
                                  Type = type.TypeName,
                                  Year = veh.Year,
                              }).ToListAsync();
            return data;
        }

        public async Task<List<KSServiceViewModel>> GetSoldServiceAsync(int cusid, int plId, string keyword = "")
        {
            var ksmsService = _context.KSServices.Where(i => i.CusId == cusid && i.MaxCount > i.UsedCount && i.Qty > 0 && i.PriceListID == plId).ToList();
            var data = (from ks in ksmsService
                        join sp in _context.ServiceSetups on ks.KSServiceSetupId equals sp.ID
                        join uom in _context.UnitofMeasures on sp.UomID equals uom.ID
                        join pl in _context.PriceLists on sp.PriceListID equals pl.ID
                        join cur in _context.Currency on pl.CurrencyID equals cur.ID
                        join item in _context.ItemMasterDatas on sp.ItemID equals item.ID
                        select new KSServiceViewModel
                        {
                            UoM = uom.Name,
                            UnitPrice = sp.Price,
                            CurName = cur.Description,
                            CurrencyId = cur.ID,
                            Factor = 0,
                            GUomID = 0,
                            ItemCode = item.Code,
                            ItemID = item.ID,
                            ItemName = item.KhmerName,
                            Qty = (decimal)ks.Qty,
                            KSServiceSetupId = sp.ID,
                            SetupCode = sp.SetupCode,
                            UoMID = uom.ID,
                            MaxCount = ks.MaxCount,
                            UsedCount = 1,
                            UsedCountM = ks.UsedCount,
                            LineID = $"{ks.ID}{item.ID}",
                            ParentLineID = $"{ks.ID}{item.ID}",
                            LinePosition = 1,
                            IsKsmsMaster = true,
                            CusId = ks.CusId,
                            PriceListID = ks.PriceListID,
                            VehicleID = ks.VehicleID,
                            ReceiptDID = ks.ReceiptDID,
                            ID = ks.ID,
                            ReceiptID = ks.ReceiptID,
                            ServiceSetupDetials = (from sd in _context.ServiceSetupDetials.Where(i => i.ServiceSetupID == sp.ID)
                                                   join itemd in _context.ItemMasterDatas on sd.ItemID equals itemd.ID
                                                   join _uom in _context.UnitofMeasures on sd.UomID equals _uom.ID
                                                   join cur in _context.Currency on sd.CurrencyID equals cur.ID
                                                   select new KSServiceViewModel
                                                   {
                                                       IsKsms = true,
                                                       ItemID = sd.ItemID,
                                                       ItemCode = itemd.Code,
                                                       ItemName = itemd.KhmerName,
                                                       KSServiceSetupId = sd.ServiceSetupID,
                                                       LineID = $"{DateTime.Now.Ticks}{sd.ID}",
                                                       UoM = _uom.Name,
                                                       UoMID = sd.UomID,
                                                       Qty = sd.Qty,
                                                       Cost = sd.Cost,
                                                       Factor = sd.Factor,
                                                       GUomID = sd.GUomID,
                                                       CurrencyId = sd.CurrencyID,
                                                       CurName = cur.Description,
                                                       ParentLineID = $"{ks.ID}{item.ID}",
                                                       UnitPrice = 0M,
                                                   }).ToList(),
                        }).ToList();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = RawWord(keyword);
                StringComparison ignoreCase = StringComparison.InvariantCultureIgnoreCase;
                data = data.Where(c =>
                            RawWord(c.SetupCode).Contains(keyword, ignoreCase)
                            || RawWord(c.ItemCode).Contains(keyword, ignoreCase)
                            || RawWord(c.ItemName).Contains(keyword, ignoreCase)
                            || RawWord(c.UoM).Contains(keyword, ignoreCase)
                            || RawWord(c.MaxCount.ToString()).Contains(keyword, ignoreCase)
                            || RawWord(c.UsedCount.ToString()).Contains(keyword, ignoreCase)
                            || RawWord(c.Cost.ToString()).Contains(keyword, ignoreCase)
                            ).ToList();
            }
            return await Task.FromResult(data);
        }
        private static string RawWord(string keyword)
        {
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                return Regex.Replace(keyword, "\\+s", string.Empty);
            }
            return string.Empty;
        }

        public async Task UpdateKSServiceAsync(KSServiceMaster kSServices, List<SerialNumber> serials, List<BatchNo> batches)
        {

            if (kSServices.KsServiceDetials.Any())
            {
                foreach (var kSService in kSServices.KsServiceDetials)
                {
                    // update stock
                    IssuseInStock(kSService.ID, kSServices.ID, serials, batches, kSService.UsedCount);
                    var ks = await _context.KSServices.FindAsync(kSService.ID);
                    if (ks != null)
                    {
                        ks.UsedCount += kSService.UsedCount;
                        ks.Qty -= kSService.UsedCount;
                        _context.KSServices.Update(ks);
                    }
                    KSServiceHistory kSServiceHistory = new()
                    {
                        ID = 0,
                        KSServiceID = ks.ID,
                        KSServiceMasterID = kSServices.ID,
                        Qty = kSService.UsedCount,
                        ReceiptDID = ks.ReceiptDID,
                        ReceiptID = ks.ReceiptID,
                    };
                    _context.KSServiceHistories.Add(kSServiceHistory);
                    await _context.SaveChangesAsync();
                }
            }
        }

        public void IssuseInStock(int ksId, int ksMId, List<SerialNumber> serials, List<BatchNo> batches, double usingQty)
        {
            JournalEntry journalEntry = new();
            SeriesDetail seriesDetail = new();
            string OffsetAcc = "";
            var ks = _context.KSServices.FirstOrDefault(w => w.ID == ksId);
            var ksMaster = _context.KSServiceMaster.Find(ksMId);
            var series = _context.Series.Find(ksMaster.SeriesID) ?? new Series();
            var ksSetup = _context.ServiceSetups.FirstOrDefault(w => w.ID == ks.KSServiceSetupId);
            var ksDetails = _context.ServiceSetupDetials.Where(w => w.ServiceSetupID == ksSetup.ID);
            var Com = _context.Company.FirstOrDefault(c => c.ID == ksMaster.CompanyID);
            var Exr = _context.ExchangeRates.FirstOrDefault(e => e.CurrencyID == Com.LocalCurrencyID);
            var docType = _context.DocumentTypes.FirstOrDefault(w => w.Code == "US");
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
                journalEntry.Creator = ksMaster.UserID;
                journalEntry.TransNo = ksMaster.Number;
                journalEntry.PostingDate = ksMaster.CreatedAt;
                journalEntry.DocumentDate = ksMaster.CreatedAt;
                journalEntry.DueDate = ksMaster.CreatedAt;
                journalEntry.SSCID = ksMaster.SysCurrencyID;
                journalEntry.LLCID = ksMaster.LocalCurrencyID;
                journalEntry.CompanyID = ksMaster.CompanyID;
                journalEntry.LocalSetRate = (decimal)ksMaster.LocalSetRate;
                journalEntry.SeriesDID = seriesDetail.ID;
                journalEntry.Remarks = series.Name + "-" + ksMaster.Number;
                _context.Update(journalEntry);
                _context.SaveChanges();
            }
            var accountReceive = _context.BusinessPartners.FirstOrDefault(w => w.ID == ksMaster.CusId);
            var glAcc = _context.GLAccounts.FirstOrDefault(w => w.ID == accountReceive.GLAccID) ?? new GLAccount();
            OffsetAcc = accountReceive.Code + "-" + glAcc.Code;
            //Debit account receivable  
            List<JournalEntryDetail> journalEntryDetail = new();
            List<AccountBalance> accountBalance = new();
            foreach (var item in ksDetails)
            {
                var itemMaster = _context.ItemMasterDatas.FirstOrDefault(w => w.ID == item.ItemID && !w.Delete);
                int inventoryAccID = 0, COGSAccID = 0;
                decimal inventoryAccAmount = 0, COGSAccAmount = 0;
                if (itemMaster.SetGlAccount == SetGlAccount.ItemLevel)
                {
                    var inventoryAcc = (from ia in _context.ItemAccountings.Where(w => w.ItemID == item.ItemID && w.WarehouseID == ksMaster.WarehouseID)
                                        join gl in _context.GLAccounts on ia.InventoryAccount equals gl.Code
                                        select gl
                                         ).FirstOrDefault() ?? new GLAccount();
                    var COGSAcc = (from ia in _context.ItemAccountings.Where(w => w.ItemID == item.ItemID && w.WarehouseID == ksMaster.WarehouseID)
                                   join gl in _context.GLAccounts on ia.CostofGoodsSoldAccount equals gl.Code
                                   select gl
                                         ).FirstOrDefault() ?? new GLAccount();
                    inventoryAccID = inventoryAcc.ID;
                    COGSAccID = COGSAcc.ID;
                }
                else if (itemMaster.SetGlAccount == SetGlAccount.ItemGroup)
                {
                    var inventoryAcc = (from ia in _context.ItemAccountings.Where(w => w.ItemGroupID == itemMaster.ItemGroup1ID)
                                        join gl in _context.GLAccounts on ia.InventoryAccount equals gl.Code
                                        select gl
                                         ).FirstOrDefault() ?? new GLAccount();
                    var COGSAcc = (from ia in _context.ItemAccountings.Where(w => w.ItemGroupID == itemMaster.ItemGroup1ID).ToList()
                                   join gl in _context.GLAccounts on ia.CostofGoodsSoldAccount equals gl.Code
                                   select gl
                                         ).FirstOrDefault() ?? new GLAccount();
                    inventoryAccID = inventoryAcc.ID;
                    COGSAccID = COGSAcc.ID;
                }
                var orft = _context.GroupDUoMs.FirstOrDefault(w => w.GroupUoMID == itemMaster.GroupUomID && w.AltUOM == item.UomID);
                if (itemMaster.Process != "Standard")
                {
                    double @Check_Stock;
                    double @Remain;
                    double @IssusQty;
                    double @FIFOQty;
                    double @Qty = (double)item.Qty * usingQty * orft.Factor;
                    double Cost = 0;
                    var _itemAcc = _context.ItemAccountings.FirstOrDefault(i => i.WarehouseID == ksMaster.WarehouseID && i.ItemID == item.ItemID);
                    var item_warehouse_summary = _context.WarehouseSummary.FirstOrDefault(w => w.WarehouseID == ksMaster.WarehouseID && w.ItemID == item.ItemID);
                    var wareDetails = _context.WarehouseDetails.Where(w => w.WarehouseID == ksMaster.WarehouseID && w.ItemID == item.ItemID).ToList();
                    if (item_warehouse_summary != null)
                    {
                        //WerehouseSummary
                        item_warehouse_summary.InStock -= @Qty;
                        //Itemmasterdata
                        itemMaster.StockIn -= (double)item.Qty;
                        _context.WarehouseSummary.Update(item_warehouse_summary);
                        UpdateItemAccounting(_itemAcc, item_warehouse_summary);
                        _context.SaveChanges();
                    }

                    //Checking Serial Batch //
                    if (itemMaster.ManItemBy == ManageItemBy.SerialNumbers)
                    {
                        if (serials.Count > 0)
                        {
                            List<WarehouseDetail> wareForAudis = new();
                            foreach (var s in serials)
                            {
                                if (s.SerialNumberSelected != null)
                                {
                                    foreach (var ss in s.SerialNumberSelected.SerialNumberSelectedDetails)
                                    {
                                        var waredetial = wareDetails.FirstOrDefault(i => ss.SerialNumber == i.SerialNumber && i.InStock > 0);
                                        decimal _inventoryAccAmount = 0M;
                                        decimal _COGSAccAmount = 0M;
                                        if (waredetial != null)
                                        {
                                            waredetial.InStock -= 1;
                                            Cost = waredetial.Cost;
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
                                                ProcessItem = ProcessItem.SEBA,
                                                SerialNumber = waredetial.SerialNumber,
                                                SyetemDate = DateTime.Now,
                                                SysNum = 0,
                                                TimeIn = DateTime.Now,
                                                WarehouseID = waredetial.WarehouseID,
                                                UomID = item.UomID,
                                                UserID = ksMaster.UserID,
                                                ExpireDate = waredetial.ExpireDate,
                                                TransType = TransTypeWD.POSService,
                                                FromWareDetialID = waredetial.ID,
                                                BPID = waredetial.BPID,
                                                TransID = ksMaster.ID,
                                                Contract = itemMaster.ContractID,
                                                OutStockFrom = ksMaster.ID,
                                            };
                                            wareForAudis.Add(waredetial);
                                            _inventoryAccAmount = (decimal)waredetial.Cost;
                                            _COGSAccAmount = (decimal)waredetial.Cost;
                                            _context.StockOuts.Add(stockOut);
                                            _context.SaveChanges();
                                        }
                                        InsertFinancial(
                                            inventoryAccID, COGSAccID, _inventoryAccAmount, _COGSAccAmount, journalEntryDetail,
                                            accountBalance, journalEntry, docType, douTypeID, ksMaster, OffsetAcc
                                        );
                                    }
                                }
                            }
                            wareForAudis = wareForAudis.GroupBy(i => i.Cost).Select(i => i.FirstOrDefault()).ToList();
                            if (wareForAudis.Any())
                            {
                                foreach(var i in wareForAudis)
                                {
                                    // Insert to Inventory Audit
                                    var inventory_audit = _context.InventoryAudits
                                        .Where(w => w.ItemID == item.ItemID && w.WarehouseID == ksMaster.WarehouseID);
                                    //var item_IssusStock = wareDetails.FirstOrDefault(w => w.InStock > 0);
                                    var inventory = new InventoryAudit
                                    {
                                        ID = 0,
                                        WarehouseID = ksMaster.WarehouseID,
                                        BranchID = ksMaster.BranchID,
                                        UserID = ksMaster.UserID,
                                        ItemID = item.ItemID,
                                        CurrencyID = ksMaster.SysCurrencyID,
                                        UomID = orft.BaseUOM,
                                        InvoiceNo = ksMaster.Number,
                                        Trans_Type = docType.Code,
                                        Process = itemMaster.Process,
                                        SystemDate = DateTime.Now,
                                        TimeIn = DateTime.Now.ToShortTimeString().ToString(),
                                        Qty = @Qty * -1,
                                        Cost = i.Cost,
                                        Price = 0,
                                        CumulativeQty = inventory_audit.Sum(q => q.Qty) - @Qty,
                                        CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) - (@Qty * i.Cost),
                                        Trans_Valuse = @Qty * i.Cost * -1,
                                        ExpireDate = i.ExpireDate,
                                        LocalCurID = ksMaster.LocalCurrencyID,
                                        LocalSetRate = ksMaster.LocalSetRate,
                                        CompanyID = ksMaster.CompanyID,
                                        DocumentTypeID = docType.ID,
                                        SeriesID = ksMaster.SeriesID,
                                        SeriesDetailID = ksMaster.SeriesDID,
                                    };
                                    CumulativeValue(inventory.WarehouseID, inventory.ItemID, inventory.CumulativeValue, _itemAcc);
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
                            List<WarehouseDetail> wareForAudis = new();
                            foreach (var b in batches)
                            {
                                if (b.BatchNoSelected != null)
                                {
                                    foreach (var sb in b.BatchNoSelected.BatchNoSelectedDetails)
                                    {
                                        //no convert to base qty, its already used base qty
                                        decimal selectedQty = sb.SelectedQty;/* * (decimal)orft.Factor*/
                                        var waredetial = wareDetails.FirstOrDefault(i => sb.BatchNo == i.BatchNo && i.InStock > 0);
                                        decimal _inventoryAccAmount = 0M;
                                        decimal _COGSAccAmount = 0M;
                                        if (waredetial != null)
                                        {
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
                                                ProcessItem = ProcessItem.SEBA,
                                                SyetemDate = DateTime.Now,
                                                SysNum = 0,
                                                TimeIn = DateTime.Now,
                                                WarehouseID = waredetial.WarehouseID,
                                                UomID = item.UomID,
                                                UserID = ksMaster.UserID,
                                                ExpireDate = waredetial.ExpireDate,
                                                BatchAttr1 = waredetial.BatchAttr1,
                                                BatchAttr2 = waredetial.BatchAttr2,
                                                BatchNo = waredetial.BatchNo,
                                                TransType = TransTypeWD.POSService,
                                                BPID = waredetial.BPID,
                                                FromWareDetialID = waredetial.ID,
                                                TransID = ksMaster.ID,
                                                OutStockFrom = ksMaster.ID,
                                            };
                                            _inventoryAccAmount = (decimal)waredetial.Cost * selectedQty;
                                            _COGSAccAmount = (decimal)waredetial.Cost * selectedQty;
                                            _context.StockOuts.Add(stockOut);
                                            _context.SaveChanges();
                                        }
                                        InsertFinancial(
                                            inventoryAccID, COGSAccID, _inventoryAccAmount, _COGSAccAmount, journalEntryDetail,
                                            accountBalance, journalEntry, docType, douTypeID, ksMaster, OffsetAcc
                                        );
                                    }
                                }
                            }

                            wareForAudis = wareForAudis.GroupBy(i => i.Cost).Select(i => i.FirstOrDefault()).ToList();
                            if (wareForAudis.Any())
                            {
                                foreach(var i in wareForAudis)
                                {
                                    // insert to inventory audit
                                    var inventory_audit = _context.InventoryAudits
                                        .Where(w => w.ItemID == item.ItemID && w.WarehouseID == ksMaster.WarehouseID);
                                    var inventory = new InventoryAudit
                                    {
                                        ID = 0,
                                        WarehouseID = ksMaster.WarehouseID,
                                        BranchID = ksMaster.BranchID,
                                        UserID = ksMaster.UserID,
                                        ItemID = item.ItemID,
                                        CurrencyID = ksMaster.SysCurrencyID,
                                        UomID = orft.BaseUOM,
                                        InvoiceNo = ksMaster.Number,
                                        Trans_Type = docType.Code,
                                        Process = itemMaster.Process,
                                        SystemDate = DateTime.Now,
                                        TimeIn = DateTime.Now.ToShortTimeString().ToString(),
                                        Qty = @Qty * -1,
                                        Cost = i.Cost,
                                        Price = 0,
                                        CumulativeQty = inventory_audit.Sum(q => q.Qty) - @Qty,
                                        CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) - (@Qty * i.Cost),
                                        Trans_Valuse = @Qty * i.Cost * -1,
                                        ExpireDate = i.ExpireDate,
                                        LocalCurID = ksMaster.LocalCurrencyID,
                                        LocalSetRate = ksMaster.LocalSetRate,
                                        CompanyID = ksMaster.CompanyID,
                                        DocumentTypeID = docType.ID,
                                        SeriesID = ksMaster.SeriesID,
                                        SeriesDetailID = ksMaster.SeriesDID,
                                    };
                                    CumulativeValue(inventory.WarehouseID, inventory.ItemID, inventory.CumulativeValue, _itemAcc);
                                    _context.InventoryAudits.Add(inventory);
                                    _context.SaveChanges();
                                }
                            }
                            
                        }
                    }
                    else
                    {
                        var iwhd = wareDetails.Where(w => w.InStock > 0).ToList();
                        foreach (var item_warehouse in iwhd)
                        {
                            var item_inventory_audit = new InventoryAudit();
                            var item_IssusStock = wareDetails.FirstOrDefault(w => w.InStock > 0);
                            @Check_Stock = item_warehouse.InStock - @Qty;
                            if (@Check_Stock < 0)
                            {
                                @Remain = (item_warehouse.InStock - @Qty) * (-1);
                                @IssusQty = @Qty - @Remain;
                                if (@Remain <= 0)
                                {
                                    @Qty = 0;
                                }
                                else
                                {
                                    @Qty = @Remain;
                                }
                                if (itemMaster.Process == "FIFO")
                                {
                                    item_IssusStock.InStock = item_IssusStock.InStock -= @IssusQty;
                                    if (@IssusQty > 0)
                                    {
                                        var stockOuts = new StockOut
                                        {
                                            Cost = (decimal)item_warehouse.Cost,
                                            CurrencyID = item_warehouse.CurrencyID,
                                            ID = 0,
                                            InStock = (decimal)@IssusQty,
                                            ItemID = item.ItemID,
                                            ProcessItem = ProcessItem.FIFO,
                                            SyetemDate = DateTime.Now,
                                            TimeIn = DateTime.Now,
                                            WarehouseID = item_warehouse.WarehouseID,
                                            UomID = item.UomID,
                                            UserID = ksMaster.UserID,
                                            ExpireDate = item_IssusStock.ExpireDate,
                                            TransType = TransTypeWD.POSService,
                                            OutStockFrom = ksMaster.ID,
                                            BPID = ksMaster.CusId,
                                            FromWareDetialID = item_IssusStock.ID
                                        };
                                        _context.StockOuts.Add(stockOuts);

                                        var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item.ItemID && w.WarehouseID == ksMaster.WarehouseID);
                                        item_inventory_audit.ID = 0;
                                        item_inventory_audit.WarehouseID = ksMaster.WarehouseID;
                                        item_inventory_audit.BranchID = ksMaster.BranchID;
                                        item_inventory_audit.UserID = ksMaster.UserID;
                                        item_inventory_audit.ItemID = item.ItemID;
                                        item_inventory_audit.CurrencyID = Com.SystemCurrencyID;
                                        item_inventory_audit.UomID = orft.BaseUOM;
                                        item_inventory_audit.InvoiceNo = ksMaster.Number;
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
                                        item_inventory_audit.LocalCurID = ksMaster.LocalCurrencyID;
                                        item_inventory_audit.LocalSetRate = ksMaster.LocalSetRate;
                                        item_inventory_audit.CompanyID = ksMaster.CompanyID;
                                        item_inventory_audit.DocumentTypeID = docType.ID;
                                        item_inventory_audit.SeriesID = ksMaster.SeriesID;
                                        item_inventory_audit.SeriesDetailID = ksMaster.SeriesDID;
                                    }
                                    CumulativeValue(item_inventory_audit.WarehouseID, item_inventory_audit.ItemID, item_inventory_audit.CumulativeValue, _itemAcc);
                                    _context.WarehouseDetails.Update(item_IssusStock);
                                    _context.InventoryAudits.Add(item_inventory_audit);
                                    inventoryAccAmount = (decimal)(item_inventory_audit.Cost * @Qty);
                                    COGSAccAmount += (decimal)(item_inventory_audit.Cost * @Qty);
                                    _context.SaveChanges();
                                }
                                else if (itemMaster.Process == "Average")
                                {
                                    item_IssusStock.InStock = item_IssusStock.InStock -= @IssusQty;
                                    if (@IssusQty > 0)
                                    {
                                        var warehouse_summary = _context.WarehouseSummary.FirstOrDefault(w => w.ItemID == item.ItemID && w.WarehouseID == ksMaster.WarehouseID);
                                        double @sysAvCost = warehouse_summary.Cost;
                                        var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item.ItemID && w.WarehouseID == ksMaster.WarehouseID);
                                        var stockOuts = new StockOut
                                        {
                                            Cost = (decimal)@sysAvCost,
                                            CurrencyID = item_warehouse.CurrencyID,
                                            ID = 0,
                                            InStock = (decimal)@IssusQty,
                                            ItemID = item.ItemID,
                                            ProcessItem = ProcessItem.Average,
                                            SyetemDate = DateTime.Now,
                                            TimeIn = DateTime.Now,
                                            WarehouseID = item_warehouse.WarehouseID,
                                            UomID = item.UomID,
                                            UserID = ksMaster.UserID,
                                            ExpireDate = item_IssusStock.ExpireDate,
                                            TransType = TransTypeWD.POSService,
                                            OutStockFrom = ksMaster.ID,
                                            BPID = ksMaster.CusId,
                                            FromWareDetialID = item_IssusStock.ID
                                        };
                                        _context.StockOuts.Add(stockOuts);

                                        item_inventory_audit.ID = 0;
                                        item_inventory_audit.WarehouseID = ksMaster.WarehouseID;
                                        item_inventory_audit.BranchID = ksMaster.BranchID;
                                        item_inventory_audit.UserID = ksMaster.UserID;
                                        item_inventory_audit.ItemID = item.ItemID;
                                        item_inventory_audit.CurrencyID = Com.SystemCurrencyID;
                                        item_inventory_audit.UomID = orft.BaseUOM;
                                        item_inventory_audit.InvoiceNo = ksMaster.Number;
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
                                        item_inventory_audit.LocalCurID = ksMaster.LocalCurrencyID;
                                        item_inventory_audit.LocalSetRate = ksMaster.LocalSetRate;
                                        item_inventory_audit.CompanyID = ksMaster.CompanyID;
                                        item_inventory_audit.DocumentTypeID = docType.ID;
                                        item_inventory_audit.SeriesID = ksMaster.SeriesID;
                                        item_inventory_audit.SeriesDetailID = ksMaster.SeriesDID;
                                    }
                                    var inventoryAcct = _context.InventoryAudits.Where(w => w.ItemID == item.ItemID);
                                    double @AvgCost = ((inventoryAcct.Sum(s => s.Trans_Valuse) + item_inventory_audit.Cost) / (inventoryAcct.Sum(q => q.Qty) + item_inventory_audit.Qty));
                                    if (double.IsNaN(@AvgCost))
                                    {
                                        @AvgCost = 0;
                                        item_inventory_audit.Cost = 0;
                                    }
                                    UpdateAvgCost(item_warehouse.ItemID, ksMaster.WarehouseID, itemMaster.GroupUomID, item_inventory_audit.Qty, item_inventory_audit.Cost);
                                    UpdateBomCost(item_warehouse.ItemID, item_inventory_audit.Qty, item_inventory_audit.Cost);
                                    CumulativeValue(item_inventory_audit.WarehouseID, item_inventory_audit.ItemID, item_inventory_audit.CumulativeValue, _itemAcc);
                                    _context.WarehouseDetails.Update(item_IssusStock);
                                    _context.InventoryAudits.Add(item_inventory_audit);
                                    inventoryAccAmount = (decimal)(@AvgCost * @Qty);
                                    COGSAccAmount += (decimal)(@AvgCost * @Qty);
                                    _context.SaveChanges();
                                }
                            }
                            else
                            {
                                @FIFOQty = item_IssusStock.InStock - @Qty;
                                @IssusQty = item_IssusStock.InStock - @FIFOQty;
                                if (itemMaster.Process == "FIFO")
                                {
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
                                            ProcessItem = ProcessItem.FIFO,
                                            SyetemDate = DateTime.Now,
                                            TimeIn = DateTime.Now,
                                            WarehouseID = item_warehouse.WarehouseID,
                                            UomID = item.UomID,
                                            UserID = ksMaster.UserID,
                                            ExpireDate = item_IssusStock.ExpireDate,
                                            TransType = TransTypeWD.POSService,
                                            OutStockFrom = ksMaster.ID,
                                            BPID = ksMaster.CusId,
                                            FromWareDetialID = item_IssusStock.ID
                                        };
                                        _context.StockOuts.Add(stockOuts);

                                        var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item.ItemID && w.WarehouseID == ksMaster.WarehouseID);
                                        item_inventory_audit.ID = 0;
                                        item_inventory_audit.WarehouseID = ksMaster.WarehouseID;
                                        item_inventory_audit.BranchID = ksMaster.BranchID;
                                        item_inventory_audit.UserID = ksMaster.UserID;
                                        item_inventory_audit.ItemID = item.ItemID;
                                        item_inventory_audit.CurrencyID = Com.SystemCurrencyID;
                                        item_inventory_audit.UomID = orft.BaseUOM;
                                        item_inventory_audit.InvoiceNo = ksMaster.Number;
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
                                        item_inventory_audit.LocalCurID = ksMaster.LocalCurrencyID;
                                        item_inventory_audit.LocalSetRate = ksMaster.LocalSetRate;
                                        item_inventory_audit.CompanyID = ksMaster.CompanyID;
                                        item_inventory_audit.DocumentTypeID = docType.ID;
                                        item_inventory_audit.SeriesID = ksMaster.SeriesID;
                                        item_inventory_audit.SeriesDetailID = ksMaster.SeriesDID;
                                    }
                                    CumulativeValue(item_inventory_audit.WarehouseID, item_inventory_audit.ItemID, item_inventory_audit.CumulativeValue, _itemAcc);
                                    _context.WarehouseDetails.Update(item_IssusStock);
                                    _context.InventoryAudits.Add(item_inventory_audit);
                                    inventoryAccAmount = (decimal)(item_inventory_audit.Cost * @Qty);
                                    COGSAccAmount += (decimal)(item_inventory_audit.Cost * @Qty);
                                    _context.SaveChanges();
                                }
                                else if (itemMaster.Process == "Average")
                                {
                                    item_IssusStock.InStock = @FIFOQty;
                                    if (@IssusQty > 0)
                                    {
                                        var warehouse_summary = _context.WarehouseSummary.FirstOrDefault(w => w.ItemID == item.ItemID && w.WarehouseID == ksMaster.WarehouseID);
                                        double @sysAvCost = warehouse_summary.Cost;
                                        var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item.ItemID && w.WarehouseID == ksMaster.WarehouseID);

                                        var stockOuts = new StockOut
                                        {
                                            Cost = (decimal)@sysAvCost,
                                            CurrencyID = item_warehouse.CurrencyID,
                                            ID = 0,
                                            InStock = (decimal)@IssusQty,
                                            ItemID = item.ItemID,
                                            ProcessItem = ProcessItem.Average,
                                            SyetemDate = DateTime.Now,
                                            TimeIn = DateTime.Now,
                                            WarehouseID = item_warehouse.WarehouseID,
                                            UomID = item.UomID,
                                            UserID = ksMaster.UserID,
                                            ExpireDate = item_IssusStock.ExpireDate,
                                            TransType = TransTypeWD.POSService,
                                            OutStockFrom = ksMaster.ID,
                                            BPID = ksMaster.CusId,
                                            FromWareDetialID = item_IssusStock.ID
                                        };
                                        _context.StockOuts.Add(stockOuts);

                                        item_inventory_audit.ID = 0;
                                        item_inventory_audit.WarehouseID = ksMaster.WarehouseID;
                                        item_inventory_audit.BranchID = ksMaster.BranchID;
                                        item_inventory_audit.UserID = ksMaster.UserID;
                                        item_inventory_audit.ItemID = item.ItemID;
                                        item_inventory_audit.CurrencyID = Com.SystemCurrencyID;
                                        item_inventory_audit.UomID = orft.BaseUOM;
                                        item_inventory_audit.InvoiceNo = ksMaster.Number;
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
                                        item_inventory_audit.LocalCurID = ksMaster.LocalCurrencyID;
                                        item_inventory_audit.LocalSetRate = ksMaster.LocalSetRate;
                                        item_inventory_audit.CompanyID = ksMaster.CompanyID;
                                        item_inventory_audit.DocumentTypeID = docType.ID;
                                        item_inventory_audit.SeriesID = ksMaster.SeriesID;
                                        item_inventory_audit.SeriesDetailID = ksMaster.SeriesDID;
                                    }
                                    var inventoryAcct = _context.InventoryAudits.Where(w => w.ItemID == item.ItemID);
                                    double @AvgCost = ((inventoryAcct.Sum(s => s.Trans_Valuse) + item_inventory_audit.Cost) / (inventoryAcct.Sum(q => q.Qty) + item_inventory_audit.Qty));
                                    if (double.IsNaN(@AvgCost))
                                    {
                                        @AvgCost = 0;
                                        item_inventory_audit.Cost = 0;
                                    }
                                    UpdateAvgCost(item_warehouse.ItemID, ksMaster.WarehouseID, itemMaster.GroupUomID, item_inventory_audit.Qty, item_inventory_audit.Cost);
                                    UpdateBomCost(item_warehouse.ItemID, item_inventory_audit.Qty, item_inventory_audit.Cost);
                                    CumulativeValue(item_inventory_audit.WarehouseID, item_inventory_audit.ItemID, item_inventory_audit.CumulativeValue, _itemAcc);
                                    _context.WarehouseDetails.Update(item_IssusStock);
                                    _context.InventoryAudits.Add(item_inventory_audit);
                                    inventoryAccAmount = (decimal)(@AvgCost * @Qty);
                                    COGSAccAmount += (decimal)(@AvgCost * @Qty);
                                    _context.SaveChanges();
                                }
                                wareDetails = new List<WarehouseDetail>();
                                break;
                            }
                        }
                    }
                }
                else if (itemMaster.Process == "Standard")
                {
                    var priceListDetail = _context.PriceListDetails.FirstOrDefault(w => w.ItemID == item.ItemID && w.UomID == item.UomID && w.PriceListID == ksMaster.PriceListID) ?? new PriceListDetail();
                    inventoryAccAmount = item.Qty * ksMaster.ExchangeRate * (decimal)priceListDetail.Cost;
                    COGSAccAmount += item.Qty * ksMaster.ExchangeRate * (decimal)priceListDetail.Cost;
                }
                if (itemMaster.ManItemBy == ManageItemBy.None)
                {
                    InsertFinancial(
                        inventoryAccID, COGSAccID, inventoryAccAmount, COGSAccAmount, journalEntryDetail,
                        accountBalance, journalEntry, docType, douTypeID, ksMaster, OffsetAcc
                    );
                }
            }
            #region IssuseInStockMaterial
            List<ItemMaterial> itemMaterials = new();
            foreach (var item in ksDetails.ToList())
            {
                var itemM = _context.ItemMasterDatas.FirstOrDefault(w => w.ID == item.ItemID);
                var orft = _context.GroupDUoMs.FirstOrDefault(w => w.GroupUoMID == itemM.GroupUomID && w.AltUOM == item.UomID);
                var bom = _context.BOMaterial.FirstOrDefault(w => w.ItemID == item.ItemID && w.Active == true) ?? new BOMaterial();
                var items_material = (from bomd in _context.BOMDetail.Where(w => w.BID == bom.BID && w.Detele == false)
                                      join i in _context.ItemMasterDatas.Where(im => !im.Delete) on bomd.ItemID equals i.ID
                                      join gd in _context.GroupDUoMs on bomd.UomID equals gd.AltUOM
                                      join uom in _context.UnitofMeasures on i.InventoryUoMID equals uom.ID
                                      select new ItemMaterial
                                      {
                                          ItemID = bomd.ItemID,
                                          GroupUoMID = gd.GroupUoMID,
                                          GUoMID = i.GroupUomID,
                                          Qty = ((double)item.Qty * orft.Factor) * ((double)bomd.Qty * gd.Factor),
                                          NegativeStock = bomd.NegativeStock,
                                          Process = i.Process,
                                          UomID = uom.ID,
                                          Factor = gd.Factor
                                      }).Where(w => w.GroupUoMID == w.GUoMID).ToList();
                itemMaterials.AddRange(items_material);
            }
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
            if (allMaterials != null)
            {
                foreach (var item_detail in allMaterials.ToList())
                {
                    int inventoryAccIDavg = 0, COGSAccIDavg = 0;
                    decimal inventoryAccAmountavg = 0, COGSAccAmountavg = 0;
                    var item_master_data = _context.ItemMasterDatas.FirstOrDefault(w => w.ID == item_detail.ItemID);
                    var item_warehouse_material = _context.WarehouseSummary.FirstOrDefault(w => w.WarehouseID == ksMaster.WarehouseID && w.ItemID == item_detail.ItemID);
                    var _itemAcc = _context.ItemAccountings.FirstOrDefault(i => i.WarehouseID == ksMaster.WarehouseID && i.ItemID == item_detail.ItemID);
                    var all_item_warehouse_detail = _context.WarehouseDetails.Where(w => w.WarehouseID == ksMaster.WarehouseID && w.ItemID == item_detail.ItemID).ToList();
                    var item_nagative = from wa in _context.WarehouseSummary.Where(w => w.ItemID == item_detail.ItemID)
                                        join na in _context.BOMDetail on wa.ItemID equals na.ItemID
                                        select new
                                        {
                                            NagaStock = wa.InStock
                                        };
                    if (item_master_data.SetGlAccount == SetGlAccount.ItemLevel)
                    {
                        var inventoryAcc = (from ia in _context.ItemAccountings.Where(w => w.ItemID == item_detail.ItemID && w.WarehouseID == ksMaster.WarehouseID)
                                            join gl in _context.GLAccounts on ia.InventoryAccount equals gl.Code
                                            select gl
                                             ).FirstOrDefault() ?? new GLAccount();
                        var COGSAcc = (from ia in _context.ItemAccountings.Where(w => w.ItemID == item_detail.ItemID && w.WarehouseID == ksMaster.WarehouseID)
                                       join gl in _context.GLAccounts on ia.CostofGoodsSoldAccount equals gl.Code
                                       select gl
                                             ).FirstOrDefault() ?? new GLAccount();
                        inventoryAccIDavg = inventoryAcc.ID;
                        COGSAccIDavg = COGSAcc.ID;
                    }
                    else if (item_master_data.SetGlAccount == SetGlAccount.ItemGroup)
                    {
                        var inventoryAcc = (from ia in _context.ItemAccountings.Where(w => w.ItemGroupID == item_master_data.ItemGroup1ID)
                                            join gl in _context.GLAccounts on ia.InventoryAccount equals gl.Code
                                            select gl
                                             ).FirstOrDefault() ?? new GLAccount();
                        var COGSAcc = (from ia in _context.ItemAccountings.Where(w => w.ItemGroupID == item_master_data.ItemGroup1ID)
                                       join gl in _context.GLAccounts on ia.CostofGoodsSoldAccount equals gl.Code
                                       select gl
                                             ).FirstOrDefault() ?? new GLAccount();
                        inventoryAccIDavg = inventoryAcc.ID;
                        COGSAccIDavg = COGSAcc.ID;
                    }
                    var nagative_check = item_nagative.Sum(w => w.NagaStock);
                    //WerehouseSummary
                    item_warehouse_material.Committed -= (double)item_detail.Qty;
                    item_warehouse_material.InStock -= (double)item_detail.Qty;
                    //Itemmasterdata
                    item_master_data.StockIn -= (double)item_detail.Qty;
                    UpdateItemAccounting(_itemAcc, item_warehouse_material);
                    if (item_detail.NegativeStock == true && nagative_check <= 0)
                    {
                        double @IssusQty;
                        double @FIFOQty;
                        double @Qty = item_detail.Qty;
                        var item_inventory_audit = new InventoryAudit();
                        var item_IssusStock = all_item_warehouse_detail.LastOrDefault(w => w.InStock <= 0);
                        @FIFOQty = item_IssusStock.InStock - @Qty;
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
                                    UserID = ksMaster.UserID,
                                    ExpireDate = item_IssusStock.ExpireDate,
                                    TransType = TransTypeWD.POSService,
                                    OutStockFrom = ksMaster.ID,
                                    BPID = ksMaster.CusId,
                                    FromWareDetialID = item_IssusStock.ID
                                };
                                _context.StockOuts.Add(stockOuts);

                                var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item_detail.ItemID && w.WarehouseID == ksMaster.WarehouseID);
                                item_inventory_audit.ID = 0;
                                item_inventory_audit.WarehouseID = ksMaster.WarehouseID;
                                item_inventory_audit.BranchID = ksMaster.BranchID;
                                item_inventory_audit.UserID = ksMaster.UserID;
                                item_inventory_audit.ItemID = item_detail.ItemID;
                                item_inventory_audit.CurrencyID = Com.SystemCurrencyID;
                                item_inventory_audit.UomID = item_detail.UomID;
                                item_inventory_audit.InvoiceNo = ksMaster.Number;
                                item_inventory_audit.Trans_Type = docType.Code;
                                item_inventory_audit.Process = item_detail.Process;
                                item_inventory_audit.SystemDate = DateTime.Now;
                                item_inventory_audit.Qty = @IssusQty * -1;
                                item_inventory_audit.Cost = item_IssusStock.Cost;
                                item_inventory_audit.Price = 0;
                                item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) - @IssusQty;
                                item_inventory_audit.CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) - (@IssusQty * item_IssusStock.Cost);
                                item_inventory_audit.Trans_Valuse = @IssusQty * item_IssusStock.Cost * (-1);
                                item_inventory_audit.ExpireDate = item_IssusStock.ExpireDate;
                                item_inventory_audit.LocalCurID = Com.LocalCurrencyID;
                                item_inventory_audit.LocalSetRate = Exr.SetRate;
                                item_inventory_audit.CompanyID = ksMaster.CompanyID;
                                item_inventory_audit.DocumentTypeID = docType.ID;
                                item_inventory_audit.SeriesID = ksMaster.SeriesID;
                                item_inventory_audit.SeriesDetailID = ksMaster.SeriesDID;
                            }
                            inventoryAccAmountavg += (decimal)(item_inventory_audit.Cost * @Qty);
                            COGSAccAmountavg += (decimal)(item_inventory_audit.Cost * @Qty);
                            CumulativeValue(item_inventory_audit.WarehouseID, item_inventory_audit.ItemID, item_inventory_audit.CumulativeValue, _itemAcc);
                        }
                        else
                        {
                            item_IssusStock.InStock = @FIFOQty;
                            if (@IssusQty > 0)
                            {
                                var warehouse_summary = _context.WarehouseSummary.FirstOrDefault(w => w.ItemID == item_detail.ItemID && w.WarehouseID == ksMaster.WarehouseID);
                                double @sysAvCost = warehouse_summary.Cost;
                                var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item_detail.ItemID && w.WarehouseID == ksMaster.WarehouseID);

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
                                    UserID = ksMaster.UserID,
                                    ExpireDate = item_IssusStock.ExpireDate,
                                    TransType = TransTypeWD.POSService,
                                    OutStockFrom = ksMaster.ID,
                                    BPID = ksMaster.CusId,
                                    FromWareDetialID = item_IssusStock.ID
                                };
                                _context.StockOuts.Add(stockOuts);

                                item_inventory_audit.ID = 0;
                                item_inventory_audit.WarehouseID = ksMaster.WarehouseID;
                                item_inventory_audit.BranchID = ksMaster.BranchID;
                                item_inventory_audit.UserID = ksMaster.UserID;
                                item_inventory_audit.ItemID = item_detail.ItemID;
                                item_inventory_audit.CurrencyID = Com.SystemCurrencyID;
                                item_inventory_audit.UomID = item_detail.UomID;
                                item_inventory_audit.InvoiceNo = ksMaster.Number;
                                item_inventory_audit.Trans_Type = docType.Code;
                                item_inventory_audit.Process = item_detail.Process;
                                item_inventory_audit.SystemDate = DateTime.Now;
                                item_inventory_audit.Qty = @IssusQty * -1;
                                item_inventory_audit.Cost = @sysAvCost;
                                item_inventory_audit.Price = 0;
                                item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) - @IssusQty;
                                item_inventory_audit.CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) - (@IssusQty * @sysAvCost);
                                item_inventory_audit.Trans_Valuse = @IssusQty * @sysAvCost * (-1);
                                item_inventory_audit.ExpireDate = item_IssusStock.ExpireDate;
                                item_inventory_audit.LocalCurID = Com.LocalCurrencyID;
                                item_inventory_audit.LocalSetRate = Exr.SetRate;
                                item_inventory_audit.CompanyID = ksMaster.CompanyID;
                                item_inventory_audit.DocumentTypeID = docType.ID;
                                item_inventory_audit.SeriesID = ksMaster.SeriesID;
                                item_inventory_audit.SeriesDetailID = ksMaster.SeriesDID;
                            }
                            //
                            var inventoryAcct = _context.InventoryAudits.Where(w => w.ItemID == item_detail.ItemID);
                            double @AvgCost = ((inventoryAcct.Sum(s => s.Trans_Valuse) + item_inventory_audit.Cost) / (inventoryAcct.Sum(q => q.Qty) + item_inventory_audit.Qty));
                            if (double.IsNaN(@AvgCost))
                            {
                                @AvgCost = 0;
                                item_inventory_audit.Cost = 0;
                            }
                            UpdateAvgCost(item_detail.ItemID, ksMaster.WarehouseID, item_master_data.GroupUomID, item_inventory_audit.Qty, item_inventory_audit.Cost);
                            UpdateBomCost(item_detail.ItemID, item_inventory_audit.Qty, item_inventory_audit.Cost);
                            CumulativeValue(item_inventory_audit.WarehouseID, item_inventory_audit.ItemID, item_inventory_audit.CumulativeValue, _itemAcc);
                            inventoryAccAmountavg = (decimal)(@AvgCost * @Qty);
                            COGSAccAmountavg += (decimal)(@AvgCost * @Qty);
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
                        double @Qty = item_detail.Qty;
                        foreach (var item_warehouse in all_item_warehouse_detail.Where(w => w.InStock > 0))
                        {
                            InventoryAudit item_inventory_audit = new();
                            var item_IssusStock = all_item_warehouse_detail.FirstOrDefault(w => w.InStock > 0);
                            @Check_Stock = item_warehouse.InStock - @Qty;
                            if (@Check_Stock < 0)
                            {
                                @Remain = (item_warehouse.InStock - @Qty) * (-1);
                                @IssusQty = @Qty - @Remain;
                                if (@Remain <= 0)
                                {
                                    @Qty = 0;
                                }
                                else
                                {
                                    @Qty = @Remain;
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
                                            UserID = ksMaster.UserID,
                                            ExpireDate = item_IssusStock.ExpireDate,
                                            TransType = TransTypeWD.POSService,
                                            OutStockFrom = ksMaster.ID,
                                            BPID = ksMaster.CusId,
                                            FromWareDetialID = item_IssusStock.ID
                                        };
                                        _context.StockOuts.Add(stockOuts);

                                        var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item_detail.ItemID && w.WarehouseID == ksMaster.WarehouseID);
                                        item_inventory_audit.ID = 0;
                                        item_inventory_audit.WarehouseID = ksMaster.WarehouseID;
                                        item_inventory_audit.BranchID = ksMaster.BranchID;
                                        item_inventory_audit.UserID = ksMaster.UserID;
                                        item_inventory_audit.ItemID = item_detail.ItemID;
                                        item_inventory_audit.CurrencyID = Com.SystemCurrencyID;
                                        item_inventory_audit.UomID = item_detail.UomID;
                                        item_inventory_audit.InvoiceNo = ksMaster.Number;
                                        item_inventory_audit.Trans_Type = docType.Code;
                                        item_inventory_audit.Process = item_detail.Process;
                                        item_inventory_audit.SystemDate = DateTime.Now;
                                        item_inventory_audit.Qty = @IssusQty * -1;
                                        item_inventory_audit.Cost = item_IssusStock.Cost;
                                        item_inventory_audit.Price = 0;
                                        item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) - @IssusQty;
                                        item_inventory_audit.CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) - (@IssusQty * item_IssusStock.Cost);
                                        item_inventory_audit.Trans_Valuse = @IssusQty * item_IssusStock.Cost * (-1);
                                        item_inventory_audit.ExpireDate = item_IssusStock.ExpireDate;
                                        item_inventory_audit.LocalCurID = Com.LocalCurrencyID;
                                        item_inventory_audit.LocalSetRate = Exr.SetRate;
                                        item_inventory_audit.CompanyID = ksMaster.CompanyID;
                                        item_inventory_audit.DocumentTypeID = docType.ID;
                                        item_inventory_audit.SeriesID = ksMaster.SeriesID;
                                        item_inventory_audit.SeriesDetailID = ksMaster.SeriesDID;
                                    }
                                    CumulativeValue(item_inventory_audit.WarehouseID, item_inventory_audit.ItemID, item_inventory_audit.CumulativeValue, _itemAcc);
                                    inventoryAccAmountavg += (decimal)(item_inventory_audit.Cost * @Qty);
                                    COGSAccAmountavg += (decimal)(item_inventory_audit.Cost * @Qty);
                                    _context.WarehouseDetails.Update(item_IssusStock);
                                    _context.InventoryAudits.Add(item_inventory_audit);
                                    _context.SaveChanges();
                                }
                                else
                                {
                                    item_IssusStock.InStock = item_IssusStock.InStock -= @IssusQty;
                                    if (@IssusQty > 0)
                                    {
                                        var warehouse_summary = _context.WarehouseSummary.FirstOrDefault(w => w.ItemID == item_detail.ItemID && w.WarehouseID == ksMaster.WarehouseID);
                                        double @sysAvCost = warehouse_summary.Cost;
                                        var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item_detail.ItemID && w.WarehouseID == ksMaster.WarehouseID);

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
                                            UserID = ksMaster.UserID,
                                            ExpireDate = item_IssusStock.ExpireDate,
                                            TransType = TransTypeWD.POSService,
                                            OutStockFrom = ksMaster.ID,
                                            BPID = ksMaster.CusId,
                                            FromWareDetialID = item_IssusStock.ID
                                        };
                                        _context.StockOuts.Add(stockOuts);

                                        item_inventory_audit.ID = 0;
                                        item_inventory_audit.WarehouseID = ksMaster.WarehouseID;
                                        item_inventory_audit.BranchID = ksMaster.BranchID;
                                        item_inventory_audit.UserID = ksMaster.UserID;
                                        item_inventory_audit.ItemID = item_detail.ItemID;
                                        item_inventory_audit.CurrencyID = Com.SystemCurrencyID;
                                        item_inventory_audit.UomID = item_detail.UomID;
                                        item_inventory_audit.InvoiceNo = ksMaster.Number;
                                        item_inventory_audit.Trans_Type = docType.Code;
                                        item_inventory_audit.Process = item_detail.Process;
                                        item_inventory_audit.SystemDate = DateTime.Now;
                                        item_inventory_audit.Qty = @IssusQty * -1;
                                        item_inventory_audit.Cost = @sysAvCost;
                                        item_inventory_audit.Price = 0;
                                        item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) - @IssusQty;
                                        item_inventory_audit.CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) - (@IssusQty * @sysAvCost);
                                        item_inventory_audit.Trans_Valuse = @IssusQty * @sysAvCost * (-1);
                                        item_inventory_audit.ExpireDate = item_IssusStock.ExpireDate;
                                        item_inventory_audit.LocalCurID = Com.LocalCurrencyID;
                                        item_inventory_audit.LocalSetRate = Exr.SetRate;
                                        item_inventory_audit.CompanyID = ksMaster.CompanyID;
                                        item_inventory_audit.DocumentTypeID = docType.ID;
                                        item_inventory_audit.SeriesID = ksMaster.SeriesID;
                                        item_inventory_audit.SeriesDetailID = ksMaster.SeriesDID;
                                    }
                                    //
                                    var inventoryAcct = _context.InventoryAudits.Where(w => w.ItemID == item_detail.ItemID);
                                    double @AvgCost = ((inventoryAcct.Sum(s => s.Trans_Valuse) + item_inventory_audit.Cost) / (inventoryAcct.Sum(q => q.Qty) + item_inventory_audit.Qty));
                                    if (double.IsNaN(@AvgCost) || double.IsInfinity(@AvgCost))
                                    {
                                        @AvgCost = 0;
                                        item_inventory_audit.Cost = 0;
                                    }
                                    inventoryAccAmountavg = (decimal)(@AvgCost * @Qty);
                                    COGSAccAmountavg += (decimal)(@AvgCost * @Qty);
                                    UpdateAvgCost(item_detail.ItemID, ksMaster.WarehouseID, item_master_data.GroupUomID, item_inventory_audit.Qty, item_inventory_audit.Cost);
                                    UpdateBomCost(item_detail.ItemID, item_inventory_audit.Qty, item_inventory_audit.Cost);
                                    CumulativeValue(item_inventory_audit.WarehouseID, item_inventory_audit.ItemID, item_inventory_audit.CumulativeValue, _itemAcc);
                                    _context.WarehouseDetails.Update(item_IssusStock);
                                    _context.InventoryAudits.Add(item_inventory_audit);
                                    _context.SaveChanges();
                                }
                            }
                            else
                            {
                                @FIFOQty = item_IssusStock.InStock - @Qty;
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
                                            UserID = ksMaster.UserID,
                                            ExpireDate = item_IssusStock.ExpireDate,
                                            TransType = TransTypeWD.POSService,
                                            OutStockFrom = ksMaster.ID,
                                            BPID = ksMaster.CusId,
                                            FromWareDetialID = item_IssusStock.ID
                                        };
                                        _context.StockOuts.Add(stockOuts);

                                        var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item_detail.ItemID && w.WarehouseID == ksMaster.WarehouseID);
                                        item_inventory_audit.ID = 0;
                                        item_inventory_audit.WarehouseID = ksMaster.WarehouseID;
                                        item_inventory_audit.BranchID = ksMaster.BranchID;
                                        item_inventory_audit.UserID = ksMaster.UserID;
                                        item_inventory_audit.ItemID = item_detail.ItemID;
                                        item_inventory_audit.CurrencyID = Com.SystemCurrencyID;
                                        item_inventory_audit.UomID = item_detail.UomID;
                                        item_inventory_audit.InvoiceNo = ksMaster.Number;
                                        item_inventory_audit.Trans_Type = docType.Code;
                                        item_inventory_audit.Process = item_detail.Process;
                                        item_inventory_audit.SystemDate = DateTime.Now;
                                        item_inventory_audit.Qty = @IssusQty * -1;
                                        item_inventory_audit.Cost = item_IssusStock.Cost;
                                        item_inventory_audit.Price = 0;
                                        item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) - @IssusQty;
                                        item_inventory_audit.CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) - (@IssusQty * item_IssusStock.Cost);
                                        item_inventory_audit.Trans_Valuse = @IssusQty * item_IssusStock.Cost * (-1);
                                        item_inventory_audit.ExpireDate = item_IssusStock.ExpireDate;
                                        item_inventory_audit.LocalCurID = Com.LocalCurrencyID;
                                        item_inventory_audit.LocalSetRate = Exr.SetRate;
                                        item_inventory_audit.CompanyID = ksMaster.CompanyID;
                                        item_inventory_audit.DocumentTypeID = docType.ID;
                                        item_inventory_audit.SeriesID = ksMaster.SeriesID;
                                        item_inventory_audit.SeriesDetailID = ksMaster.SeriesDID;
                                    }
                                    CumulativeValue(item_inventory_audit.WarehouseID, item_inventory_audit.ItemID, item_inventory_audit.CumulativeValue, _itemAcc);
                                    _context.WarehouseDetails.Update(item_IssusStock);
                                    _context.InventoryAudits.Add(item_inventory_audit);
                                    inventoryAccAmountavg += (decimal)(item_inventory_audit.Cost * @Qty);
                                    COGSAccAmountavg += (decimal)(item_inventory_audit.Cost * @Qty);
                                    _context.SaveChanges();
                                }
                                else
                                {
                                    item_IssusStock.InStock = @FIFOQty;
                                    if (@IssusQty > 0)
                                    {
                                        var warehouse_summary = _context.WarehouseSummary.FirstOrDefault(w => w.ItemID == item_detail.ItemID && w.WarehouseID == ksMaster.WarehouseID);
                                        double @sysAvCost = warehouse_summary.Cost;
                                        var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item_detail.ItemID && w.WarehouseID == ksMaster.WarehouseID);

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
                                            UserID = ksMaster.UserID,
                                            ExpireDate = item_IssusStock.ExpireDate,
                                            TransType = TransTypeWD.POSService,
                                        };
                                        _context.StockOuts.Add(stockOuts);

                                        item_inventory_audit.ID = 0;
                                        item_inventory_audit.WarehouseID = ksMaster.WarehouseID;
                                        item_inventory_audit.BranchID = ksMaster.BranchID;
                                        item_inventory_audit.UserID = ksMaster.UserID;
                                        item_inventory_audit.ItemID = item_detail.ItemID;
                                        item_inventory_audit.CurrencyID = Com.SystemCurrencyID;
                                        item_inventory_audit.UomID = item_detail.UomID;
                                        item_inventory_audit.InvoiceNo = ksMaster.Number;
                                        item_inventory_audit.Trans_Type = docType.Code;
                                        item_inventory_audit.Process = item_detail.Process;
                                        item_inventory_audit.SystemDate = DateTime.Now;
                                        item_inventory_audit.Qty = @IssusQty * -1;
                                        item_inventory_audit.Cost = @sysAvCost;
                                        item_inventory_audit.Price = 0;
                                        item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) - @IssusQty;
                                        item_inventory_audit.CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) - (@IssusQty * @sysAvCost);
                                        item_inventory_audit.Trans_Valuse = @IssusQty * @sysAvCost * (-1);
                                        item_inventory_audit.ExpireDate = item_IssusStock.ExpireDate;
                                        item_inventory_audit.LocalCurID = Com.LocalCurrencyID;
                                        item_inventory_audit.LocalSetRate = Exr.SetRate;
                                        item_inventory_audit.CompanyID = ksMaster.CompanyID;
                                        item_inventory_audit.DocumentTypeID = docType.ID;
                                        item_inventory_audit.SeriesID = ksMaster.SeriesID;
                                        item_inventory_audit.SeriesDetailID = ksMaster.SeriesDID;
                                    }
                                    //
                                    var inventoryAcct = _context.InventoryAudits.Where(w => w.ItemID == item_detail.ItemID);
                                    double @AvgCost = ((inventoryAcct.Sum(s => s.Trans_Valuse) + item_inventory_audit.Cost) / (inventoryAcct.Sum(q => q.Qty) + item_inventory_audit.Qty));
                                    if (double.IsNaN(@AvgCost) || double.IsInfinity(@AvgCost))
                                    {
                                        @AvgCost = 0;
                                        item_inventory_audit.Cost = 0;
                                    }
                                    inventoryAccAmountavg = (decimal)(@AvgCost * @Qty);
                                    COGSAccAmountavg += (decimal)(@AvgCost * @Qty);

                                    UpdateAvgCost(item_detail.ItemID, ksMaster.WarehouseID, item_master_data.GroupUomID, item_inventory_audit.Qty, item_inventory_audit.Cost);
                                    UpdateBomCost(item_detail.ItemID, item_inventory_audit.Qty, item_inventory_audit.Cost);
                                    CumulativeValue(item_inventory_audit.WarehouseID, item_inventory_audit.ItemID, item_inventory_audit.CumulativeValue, _itemAcc);

                                    _context.WarehouseDetails.Update(item_IssusStock);
                                    _context.InventoryAudits.Add(item_inventory_audit);
                                    _context.SaveChanges();
                                }
                                all_item_warehouse_detail = new List<WarehouseDetail>();
                                break;
                            }
                        }
                    }
                    //inventoryAccID
                    var glAccInvenfifo = _context.GLAccounts.FirstOrDefault(w => w.ID == inventoryAccIDavg) ?? new GLAccount();
                    if (glAccInvenfifo.ID > 0)
                    {
                        var journalDetail = journalEntryDetail.FirstOrDefault(w => w.ItemID == glAccInvenfifo.ID) ?? new JournalEntryDetail();
                        if (journalDetail.ItemID > 0)
                        {
                            var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == inventoryAccIDavg);
                            glAccInvenfifo.Balance -= inventoryAccAmountavg;
                            //journalEntryDetail
                            journalDetail.Credit += inventoryAccAmountavg;
                            //accountBalance
                            accBalance.CumulativeBalance = glAccInvenfifo.Balance;
                            accBalance.Credit += inventoryAccAmountavg;
                        }
                        else
                        {
                            glAccInvenfifo.Balance -= inventoryAccAmountavg;
                            journalEntryDetail.Add(new JournalEntryDetail
                            {
                                JEID = journalEntry.ID,
                                Type = Type.GLAcct,
                                ItemID = inventoryAccIDavg,
                                Credit = inventoryAccAmountavg,
                            });
                            //
                            accountBalance.Add(new AccountBalance
                            {
                                PostingDate = ksMaster.CreatedAt,
                                Origin = docType.ID,
                                OriginNo = ksMaster.Number,
                                OffsetAccount = OffsetAcc,
                                Details = douTypeID.Name + " - " + glAccInvenfifo.Code,
                                CumulativeBalance = glAccInvenfifo.Balance,
                                Credit = inventoryAccAmountavg,
                                LocalSetRate = (decimal)ksMaster.LocalSetRate,
                                GLAID = inventoryAccIDavg,
                            });
                        }
                        _context.GLAccounts.Update(glAccInvenfifo);
                    }
                    // COGS
                    var glAccCOGSfifo = _context.GLAccounts.FirstOrDefault(w => w.ID == COGSAccIDavg) ?? new GLAccount();
                    if (glAccCOGSfifo.ID > 0)
                    {
                        var journalDetail = journalEntryDetail.FirstOrDefault(w => w.ItemID == glAccCOGSfifo.ID) ?? new JournalEntryDetail();
                        if (journalDetail.ItemID > 0)
                        {
                            var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == COGSAccIDavg);
                            glAccCOGSfifo.Balance += COGSAccAmountavg;
                            //journalEntryDetail
                            journalDetail.Debit += COGSAccAmountavg;
                            //accountBalance
                            accBalance.CumulativeBalance = glAccCOGSfifo.Balance;
                            accBalance.Debit += COGSAccAmountavg;
                        }
                        else
                        {
                            glAccCOGSfifo.Balance += COGSAccAmountavg;
                            journalEntryDetail.Add(new JournalEntryDetail
                            {
                                JEID = journalEntry.ID,
                                Type = Type.GLAcct,
                                ItemID = COGSAccIDavg,
                                Debit = COGSAccAmountavg,
                            });
                            //
                            accountBalance.Add(new AccountBalance
                            {
                                PostingDate = ksMaster.CreatedAt,
                                Origin = docType.ID,
                                OriginNo = ksMaster.Number,
                                OffsetAccount = OffsetAcc,
                                Details = douTypeID.Name + "-" + glAccCOGSfifo.Code,
                                CumulativeBalance = glAccCOGSfifo.Balance,
                                Debit = COGSAccAmountavg,
                                LocalSetRate = (decimal)ksMaster.LocalSetRate,
                                GLAID = COGSAccIDavg,
                            });
                        }
                        _context.GLAccounts.Update(glAccCOGSfifo);
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

        public async Task<List<ItemsReturn>> CheckItemOutOfStockAsync(List<KSService> kSServices)
        {
            List<ItemsReturn> _itemReturns = new();
            foreach (var kSService in kSServices)
            {
                var ksDetail = await _context.ServiceSetupDetials.Where(i => i.ServiceSetupID == kSService.KSServiceSetupId).ToListAsync();

                // TODO :: check stock

                var itemReturns = (from ksd in ksDetail
                                   join item in _context.ItemMasterDatas on ksd.ItemID equals item.ID
                                   join whd in _context.WarehouseDetails on ksd.ItemID equals whd.ItemID
                                   join ws in _context.WarehouseSummary on item.ID equals ws.ItemID
                                   join uom in _context.UnitofMeasures on item.InventoryUoMID equals uom.ID
                                   group new { ksd, item, whd, ws, uom } by new { item.ID } into g
                                   let data = g.FirstOrDefault()
                                   select new ItemsReturn
                                   {
                                       Code = data.item.Code,
                                       Committed = (decimal)data.ws.Committed,
                                       ItemID = data.item.ID,
                                       InStock = (decimal)g.Sum(i => i.ws.InStock),
                                       KhmerName = $"{data.item.KhmerName} ({data.uom.Name})",
                                       LineID = data.ksd.ID.ToString(),
                                       OrderQty = data.ksd.Qty,
                                   }).Where(i => i.InStock <= 0).ToList();
                if (itemReturns.Any()) _itemReturns.AddRange(itemReturns);
            }
            return await Task.FromResult(_itemReturns);
        }

        private void InsertFinancial(
           int inventoryAccID, int COGSAccID, decimal inventoryAccAmount, decimal COGSAccAmount,
           List<JournalEntryDetail> journalEntryDetail, List<AccountBalance> accountBalance,
           JournalEntry journalEntry, DocumentType docType, DocumentType douTypeID, KSServiceMaster ksMaster,
           string OffsetAcc)
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
                        PostingDate = ksMaster.CreatedAt,
                        Origin = docType.ID,
                        OriginNo = ksMaster.Number,
                        OffsetAccount = OffsetAcc,
                        Details = douTypeID.Name + "-" + glAccInvenfifo.Code,
                        CumulativeBalance = glAccInvenfifo.Balance,
                        Credit = inventoryAccAmount,
                        LocalSetRate = (decimal)ksMaster.LocalSetRate,
                        GLAID = inventoryAccID,
                    });
                }
                _context.Update(glAccInvenfifo);
            }
            //Add COGS
            var glAccCOGSfifo = _context.GLAccounts.FirstOrDefault(w => w.ID == COGSAccID) ?? new GLAccount();
            if (glAccCOGSfifo.ID > 0)
            {
                var journalDetail = journalEntryDetail.FirstOrDefault(w => w.ItemID == glAccCOGSfifo.ID) ?? new JournalEntryDetail();
                if (journalDetail.ItemID > 0)
                {
                    var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == COGSAccID);
                    glAccCOGSfifo.Balance += COGSAccAmount;
                    //journalEntryDetail
                    journalDetail.Debit += COGSAccAmount;
                    //accountBalance
                    accBalance.CumulativeBalance = glAccCOGSfifo.Balance;
                    accBalance.Debit += COGSAccAmount;
                }
                else
                {
                    glAccCOGSfifo.Balance += COGSAccAmount;
                    journalEntryDetail.Add(new JournalEntryDetail
                    {
                        JEID = journalEntry.ID,
                        Type = Type.GLAcct,
                        ItemID = COGSAccID,
                        Debit = COGSAccAmount,
                    });
                    accountBalance.Add(new AccountBalance
                    {
                        PostingDate = ksMaster.CreatedAt,
                        Origin = docType.ID,
                        OriginNo = ksMaster.Number,
                        OffsetAccount = OffsetAcc,
                        Details = douTypeID.Name + "-" + glAccCOGSfifo.Code,
                        CumulativeBalance = glAccCOGSfifo.Balance,
                        Debit = COGSAccAmount,
                        LocalSetRate = (decimal)ksMaster.LocalSetRate,
                        GLAID = COGSAccID,
                    });
                }
                _context.Update(glAccCOGSfifo);
            }
            _context.SaveChanges();
        }

        //update_AvgCost
        public void UpdateAvgCost(int itemid, int whid, int guomid, double qty, double avgcost)
        {
            if (double.IsNaN(avgcost))
            {
                avgcost = 0;
            }
            // update pricelistdetial
            var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == itemid);
            var pri_detial = _context.PriceListDetails.Where(w => w.ItemID == itemid);
            double @AvgCost = ((inventory_audit.Sum(s => s.Trans_Valuse) + avgcost) / (inventory_audit.Sum(q => q.Qty) + qty));
            foreach (var pri in pri_detial)
            {
                var guom = _context.GroupDUoMs.FirstOrDefault(g => g.GroupUoMID == guomid && g.AltUOM == pri.UomID);
                var exp = _context.ExchangeRates.FirstOrDefault(w => w.CurrencyID == pri.CurrencyID);

                pri.Cost = @AvgCost * exp.SetRate * guom.Factor;
                if (double.IsNaN(@AvgCost) || double.IsInfinity(@AvgCost))
                {
                    pri.Cost = 0;
                }
                _context.PriceListDetails.Update(pri);
                _context.SaveChanges();
            }
            //update_waresummary
            var inventory_warehouse = _context.InventoryAudits.Where(w => w.ItemID == itemid && w.WarehouseID == whid);
            var waresummary = _context.WarehouseSummary.FirstOrDefault(w => w.ItemID == itemid && w.WarehouseID == whid);
            double @AvgCostWare = (inventory_warehouse.Sum(s => s.Trans_Valuse) + avgcost) / (inventory_warehouse.Sum(s => s.Qty));
            if (double.IsNaN(@AvgCostWare) || double.IsInfinity(@AvgCostWare))
            {
                @AvgCostWare = 0;
            }
            waresummary.Cost = @AvgCostWare;
            _context.WarehouseSummary.Update(waresummary);
            _context.SaveChanges();
        }

        private void CumulativeValue(int whid, int itemid, double value, ItemAccounting itemAcc)
        {
            if (double.IsNaN(value))
            {
                value = 0;
            }
            var wherehouse = _context.WarehouseSummary.FirstOrDefault(w => w.WarehouseID == whid && w.ItemID == itemid);
            if(wherehouse != null)
            {
                wherehouse.CumulativeValue = (decimal)value;
                _context.WarehouseSummary.Update(wherehouse);
                if (itemAcc != null) itemAcc.CumulativeValue = wherehouse.CumulativeValue;
                _context.SaveChanges();
            }
        }

        //update_bomCost
        public void UpdateBomCost(int itemid, double qty, double avgcost)
        {
            if (double.IsNaN(avgcost))
            {
                avgcost = 0;
            }
            var ItemBOMDetail = _context.BOMDetail.Where(w => w.ItemID == itemid);
            foreach (var itembom in ItemBOMDetail)
            {
                var Inven = _context.InventoryAudits.Where(w => w.ItemID == itembom.ItemID);
                double @AvgCost = (Inven.Sum(s => s.Trans_Valuse) + avgcost) / (Inven.Sum(q => q.Qty) + qty);
                var Gdoum = _context.GroupDUoMs.Where(w => w.GroupUoMID == itembom.GUomID);
                var Factor = Gdoum.FirstOrDefault(w => w.AltUOM == itembom.UomID).Factor;
                itembom.Cost = @AvgCost;
                itembom.Amount = itembom.Qty * @AvgCost;
                if (double.IsNaN(@AvgCost) || double.IsInfinity(@AvgCost))
                {
                    itembom.Cost = 0;
                    itembom.Amount = 0;
                }
                _context.BOMDetail.UpdateRange(itembom);
                _context.SaveChanges();
                // sum 
                var BOM = _context.BOMaterial.FirstOrDefault(w => w.BID == itembom.BID);
                var DBOM = _context.BOMDetail.Where(w => w.BID == BOM.BID && !w.Detele);
                BOM.TotalCost = DBOM.Sum(s => s.Amount);
                _context.BOMaterial.Update(BOM);
                _context.SaveChanges();
            }
        }
        // UpdateStockItemAccounting
        private void UpdateItemAccounting(ItemAccounting itemAcc, WarehouseSummary ws)
        {
            if (itemAcc != null)
            {
                itemAcc.Committed = ws.Committed;
                itemAcc.InStock = ws.InStock;
                itemAcc.Ordered = ws.Ordered;
                itemAcc.Available = ws.Available;
                _context.ItemAccountings.Update(itemAcc);
                _context.SaveChanges();
            }
        }

        // add 
        private string GetCur(int curSysId)
        {
            var cur = _context.Currency.Find(curSysId);
            if (cur != null)
            {
                return cur.Description;
            }
            else
            {
                return "";
            }
        }

        public async Task<List<SaleReportKSMS>> GetSaleReportAsync(string fromDate, string toDate, Company com)
        {
            _ = DateTime.TryParse(toDate, out DateTime _toDate);
            _ = DateTime.TryParse(fromDate, out DateTime _fromDate);
            var curDes = GetCur(com.SystemCurrencyID);
            var data = (from rp in _context.Receipt.Where(i => i.DateIn >= _fromDate & i.DateIn <= _toDate)
                        join branch in _context.Branches on rp.BranchID equals branch.ID
                        join cus in _context.BusinessPartners on rp.CustomerID equals cus.ID
                        join rpd in _context.ReceiptDetail.Where(i => !i.IsKsms) on rp.ReceiptID equals rpd.ReceiptID
                        join uom in _context.UnitofMeasures on rpd.UomID equals uom.ID
                        //join pl in _context.PriceLists on rp.PriceListID equals pl.ID
                        //join cur in _context.Currency on pl.CurrencyID equals cur.ID
                        join item in _context.ItemMasterDatas on rpd.ItemID equals item.ID
                        join user in _context.UserAccounts on rp.UserOrderID equals user.ID
                        join series in _context.Series on rp.SeriesID equals series.ID
                        join mb in _context.AutoMobiles on rp.VehicleID equals mb.AutoMID into gmb
                        from mobile in gmb.DefaultIfEmpty()
                        join md in _context.AutoModels on mobile.ModelID equals md.ModelID into gmd
                        from model in gmd.DefaultIfEmpty()
                        group new { rp, rpd, branch, cus,/* cur, */item, uom, user, series, mobile, model } by new { rp.ReceiptID, rpd.ID } into g
                        let d = g.FirstOrDefault()
                        //let mobile = _context.AutoMobiles.FirstOrDefault(i => i.AutoMID == d.rp.VehicleID) ?? new AutoMobile()
                        //let model = _context.AutoModels.FirstOrDefault(i => i.ModelID == mobile.ModelID) ?? new AutoModel()
                        select new SaleReportKSMS
                        {
                            Invoice = $"{d.series.Name}-{d.rp.ReceiptNo}",
                            RID = d.rp.ReceiptID,
                            BranchName = d.branch.Name,
                            CusName = d.cus.Name,
                            Discount = $"{curDes} {string.Format("{0:#,0.00}", d.rpd.DiscountValue * d.rp.ExchangeRate)}",
                            GrandTotal = $"{curDes} {string.Format("{0:#,0.00}", g.Sum(i => i.rp.GrandTotal) * d.rp.ExchangeRate)}",
                            ItemCode = d.item.Code,
                            ItemName = d.item.KhmerName,
                            LineID = DateTime.Now.Ticks.ToString(),
                            Plate = d.mobile == null ? "" : d.mobile.Plate ?? "",
                            ModelName = d.model == null ? "" : d.model.ModelName ?? "",
                            Qty = d.rpd.Qty,
                            SoldAmount = $"{curDes} {string.Format("{0:#,0.00}", g.Sum(i => i.rp.Sub_Total) * d.rp.ExchangeRate)}",
                            TotalF = $"{curDes} {string.Format("{0:#,0.00}", d.rpd.Total * d.rp.ExchangeRate)}",
                            UnitPrice = $"{curDes} {string.Format("{0:#,0.00}", d.rpd.UnitPrice * d.rp.ExchangeRate)}",
                            Total = d.rpd.Total * d.rp.ExchangeRate,
                            TotalItem = g.Count(),
                            UoM = d.uom.Name,
                            UserName = d.user.Username,
                            Cur = curDes,
                            CusID = d.cus.ID,
                            PostingDate = d.rp.DateIn.ToShortDateString(),
                        }).ToList();
            var _data = data.GroupBy(i => i.CusID).ToList();
            foreach (var i in data)
            {
                var total = _data.FirstOrDefault(d => d.Key == i.CusID).Sum(i => i.Total);
                i.TotalInvoice = $"{i.Cur} {string.Format("{0:#,0.00}", total)}";
                i.TotalAll = $"{i.Cur} {string.Format("{0:#,0.00}", data.Sum(sum => sum.Total))}";
            }
            return await Task.FromResult(data);
        }
    }
}
