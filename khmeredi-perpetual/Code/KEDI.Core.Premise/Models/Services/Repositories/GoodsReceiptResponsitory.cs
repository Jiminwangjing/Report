using Microsoft.EntityFrameworkCore;
using CKBS.AppContext;
using CKBS.Models.Services.Administrator.Inventory;
using CKBS.Models.Services.Inventory.Transaction;
using System.Collections.Generic;
using System.Linq;
using CKBS.Models.ServicesClass.GoodsReceipt;
using System;
using CKBS.Models.Services.Inventory;
using CKBS.Models.Services.Administrator.SystemInitialization;
using Microsoft.AspNetCore.Mvc.Rendering;
using CKBS.Models.Services.Financials;
using CKBS.Models.Services.ChartOfAccounts;
using CKBS.Models.ServicesClass.GoodsIssue;
using KEDI.Core.Premise.Models.ServicesClass.SerialBatches.Purchases;
using KEDI.Core.Premise.Repository;

namespace CKBS.Models.Services.Responsitory
{
    public interface IGoodsReceipt
    {
        IEnumerable<Warehouse> GetWarehouse_From(int ID);
        List<GoodReceiptDetail> GetItemByWarehouse_From(int warehouseID, int comId);
        ItemMasterData GetItemFindeBarcode(string barcode);
        List<GoodsReceiptViewModel> GetAllPurItemsByItemID(int wareid, int itemId, int comId);
        void SaveGoodIssues(int receipt, List<SerialViewModelPurchase> serialViewModelPurchases,
            List<BatchViewModelPurchase> batchViewModelPurchases);
        void SaveGoodIssuesBasic(int receipt, List<SerialViewModelPurchase> serialViewModelPurchases,
                List<BatchViewModelPurchase> batchViewModelPurchases);

    }
    public class GoodsReceiptResponsitory : IGoodsReceipt
    {
        private readonly DataContext _context;
        private readonly IDataPropertyRepository _dataProp;
        private readonly UtilityModule _utility;
        public GoodsReceiptResponsitory(DataContext context, IDataPropertyRepository dataProperty, UtilityModule utility)
        {
            _context = context;
            _dataProp = dataProperty;
            _utility = utility;
        }

        public IEnumerable<Warehouse> GetWarehouse_From(int ID) => _context.Warehouses.Where(x => x.Delete == false && x.BranchID == ID).ToList();
        public List<GoodReceiptDetail> GetItemByWarehouse_From(int warehouseID, int comId)
        {
            var items = (from wd in _context.WarehouseDetails
                         join item in _context.ItemMasterDatas on wd.ItemID equals item.ID
                         join uom in _context.UnitofMeasures on item.InventoryUoMID equals uom.ID
                         join cur in _context.Currency on wd.CurrencyID equals cur.ID
                         where wd.WarehouseID == warehouseID && item.Process != "Standard" && !item.Delete && item.Inventory && item.Purchase
                         group new { wd, item, uom, cur } by new { item.ID, UomID = uom.ID } into g
                         let data = g.FirstOrDefault()
                         select new GoodReceiptDetail
                         {
                             BarCode = data.item.Barcode,
                             Code = data.item.Code,
                             Cost = data.wd.Cost,
                             Currency = data.cur.Description,
                             CurrencyID = data.cur.ID,
                             EnglishName = data.item.EnglishName,
                             GLID = 0,
                             ItemID = data.item.ID,
                             KhmerName = data.item.KhmerName,
                             LineID = data.wd.ID,
                             Quantity = g.Sum(i => i.wd.InStock),
                             UomID = data.uom.ID,
                             UomName = data.uom.Name,
                         }).ToList();

            _dataProp.DataProperty(items, comId, "ItemID", "AddictionProps");
            return items;
        }
        public ItemMasterData GetItemFindeBarcode(string barcode)
        {
            var data = _context.ItemMasterDatas.FirstOrDefault(i => i.Barcode == barcode);
            return data;
        }
        #region  SaveGoodIssuesBasic
        public void SaveGoodIssuesBasic(int receipt, List<SerialViewModelPurchase> serialViewModelPurchases,
                   List<BatchViewModelPurchase> batchViewModelPurchases)
        {
            var goodReceipt = _context.GoodsReceipts.FirstOrDefault(i => i.GoodsReceiptID == receipt);
            var goodReceitpDetail = _context.GoodReceiptDetails.Where(i => i.GoodsReceiptID == goodReceipt.GoodsReceiptID).ToList();
            var docType = _context.DocumentTypes.FirstOrDefault(i => i.Code == "GR") ?? new DocumentType();
            var series = _context.Series.Find(goodReceipt.SeriseID) ?? new Series();
            List<WarehouseDetail> whsDetials = new();
            foreach (var itemdt in goodReceitpDetail)
            {
                var itemmaster = _context.ItemMasterDatas.FirstOrDefault(w => !w.Delete && w.ID == itemdt.ItemID);
                var gd = _context.GroupDUoMs.FirstOrDefault(W => W.GroupUoMID == itemmaster.GroupUomID && W.AltUOM == itemdt.UomID);
                var orft = _context.GroupDUoMs.FirstOrDefault(w => w.GroupUoMID == itemmaster.GroupUomID);
                var warehouse = _context.WarehouseSummary.FirstOrDefault(w => w.ItemID == itemdt.ItemID && w.WarehouseID == goodReceipt.WarehouseID);
                var _itemAcc = _context.ItemAccountings
                    .FirstOrDefault(i => i.ItemID == itemdt.ItemID && i.WarehouseID == goodReceipt.WarehouseID);
                double @Qty = itemdt.Quantity * gd.Factor;
                double @Cost = itemdt.Cost / gd.Factor;
                InventoryAudit itemInven = new();
                // update itmemasterdata                    
                itemmaster.StockIn += @Qty;
                //update warehouse                    
                warehouse.InStock += @Qty;
                _context.ItemMasterDatas.Update(itemmaster);
                _context.WarehouseSummary.Update(warehouse);
                _utility.UpdateItemAccounting(_itemAcc, warehouse);

                //insert warehousedetail
                if (itemmaster.ManItemBy == ManageItemBy.SerialNumbers && itemmaster.ManMethod == ManagementMethod.OnEveryTransation)
                {
                    var svmp = serialViewModelPurchases
                        .FirstOrDefault(s => s.ItemID == itemdt.ItemID && s.Cost == (decimal)itemdt.Cost);
                    List<InventoryAudit> inventoryAudit = new();
                    if (svmp != null)
                    {
                        foreach (var sv in svmp.SerialDetialViewModelPurchase)
                        {
                            whsDetials.Add(new WarehouseDetail
                            {
                                AdmissionDate = sv.AdmissionDate,
                                Cost = @Cost,
                                CurrencyID = goodReceipt.SysCurID,
                                Details = sv.Detials,
                                ID = 0,
                                InStock = 1,
                                ItemID = itemdt.ItemID,
                                Location = sv.Location,
                                LotNumber = sv.LotNumber,
                                MfrDate = sv.MfrDate,
                                MfrSerialNumber = sv.MfrSerialNo,
                                MfrWarDateEnd = sv.MfrWarrantyEnd,
                                MfrWarDateStart = sv.MfrWarrantyStart,
                                ProcessItem = ProcessItem.SEBA,
                                SerialNumber = sv.SerialNumber,
                                SyetemDate = DateTime.Now,
                                SysNum = 0,
                                TimeIn = DateTime.Now,
                                WarehouseID = goodReceipt.WarehouseID,
                                UomID = itemdt.UomID,
                                UserID = goodReceipt.UserID,
                                ExpireDate = sv.ExpirationDate == null ? default : (DateTime)sv.ExpirationDate,
                                TransType = TransTypeWD.GoodsReceipt,
                                IsDeleted = true,
                                GRGIID = goodReceipt.GoodsReceiptID,
                                InStockFrom = goodReceipt.GoodsReceiptID
                            });
                        }
                        //insert inventoryaudit
                        InventoryAudit invAudit = new();
                        var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == itemdt.ItemID && w.UomID == itemdt.UomID && w.WarehouseID == goodReceipt.WarehouseID);
                        var pri_detial = _context.PriceListDetails.Where(w => w.ItemID == itemdt.ItemID && w.UomID == itemdt.UomID);
                        invAudit.ID = 0;
                        invAudit.WarehouseID = goodReceipt.WarehouseID;
                        invAudit.BranchID = goodReceipt.BranchID;
                        invAudit.UserID = goodReceipt.UserID;
                        invAudit.ItemID = itemdt.ItemID;
                        invAudit.CurrencyID = goodReceipt.SysCurID;
                        invAudit.UomID = itemdt.UomID;
                        invAudit.InvoiceNo = goodReceipt.Number_No;
                        invAudit.Trans_Type = docType.Code;
                        invAudit.Process = itemmaster.Process;
                        invAudit.SystemDate = DateTime.Now;
                        invAudit.PostingDate = goodReceipt.PostingDate;
                        invAudit.TimeIn = DateTime.Now.ToString("h:mm:ss tt");
                        invAudit.Qty = @Qty;
                        invAudit.Cost = @Cost;
                        invAudit.Price = 0;
                        invAudit.CumulativeQty = inventory_audit.Sum(q => q.Qty) + @Qty;
                        invAudit.CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) + (@Qty * @Cost);
                        invAudit.Trans_Valuse = @Qty * @Cost;
                        invAudit.ExpireDate = itemdt.ExpireDate;
                        invAudit.LocalCurID = goodReceipt.LocalCurID;
                        invAudit.LocalSetRate = goodReceipt.LocalSetRate;
                        invAudit.DocumentTypeID = goodReceipt.DocTypeID;
                        invAudit.CompanyID = goodReceipt.CompanyID;
                        invAudit.SeriesID = goodReceipt.SeriseID;
                        invAudit.SeriesDetailID = goodReceipt.SeriseDID;
                        // update pricelistdetial
                        _utility.CumulativeValue(invAudit.WarehouseID, invAudit.ItemID, invAudit.CumulativeValue, _itemAcc);
                        _context.InventoryAudits.Add(invAudit);
                        _context.WarehouseDetails.AddRange(whsDetials);
                        _context.SaveChanges();
                    }
                }
                else if (itemmaster.ManItemBy == ManageItemBy.Batches && itemmaster.ManMethod == ManagementMethod.OnEveryTransation)
                {
                    var bvmp = batchViewModelPurchases.FirstOrDefault(s => s.ItemID == itemdt.ItemID && s.Cost == (decimal)itemdt.Cost);

                    if (bvmp != null)
                    {
                        var bvs = bvmp.BatchDetialViewModelPurchases.Where(i => !string.IsNullOrEmpty(i.Batch) && i.Qty > 0).ToList();
                        foreach (var bv in bvs)
                        {
                            var _qty = (double)bv.Qty;
                            whsDetials.Add(new WarehouseDetail
                            {
                                AdmissionDate = bv.AdmissionDate,
                                Cost = @Cost,
                                CurrencyID = goodReceipt.SysCurID,
                                Details = bv.Detials,
                                ID = 0,
                                InStock = _qty,
                                ItemID = itemdt.ItemID,
                                Location = bv.Location,
                                MfrDate = bv.MfrDate,
                                ProcessItem = ProcessItem.SEBA,
                                SyetemDate = DateTime.Now,
                                SysNum = 0,
                                TimeIn = DateTime.Now,
                                WarehouseID = goodReceipt.WarehouseID,
                                UomID = itemdt.UomID,
                                UserID = goodReceipt.UserID,
                                ExpireDate = bv.ExpirationDate == null ? default : (DateTime)bv.ExpirationDate,
                                BatchAttr1 = bv.BatchAttribute1,
                                BatchAttr2 = bv.BatchAttribute2,
                                BatchNo = bv.Batch,
                                TransType = TransTypeWD.GoodsReceipt,
                                IsDeleted = true,
                                GRGIID = goodReceipt.GoodsReceiptID,
                                InStockFrom = goodReceipt.GoodsReceiptID
                            });
                        }
                        //insert inventoryaudit
                        InventoryAudit invAudit = new();
                        var inventory_audit = _context.InventoryAudits
                            .Where(w => w.ItemID == itemdt.ItemID && w.UomID == itemdt.UomID && w.WarehouseID == goodReceipt.WarehouseID)
                            .ToList();
                        // var pri_detial = _context.PriceListDetails.Where(w => w.ItemID == itemdt.ItemID && w.UomID == itemdt.UomID);
                        invAudit.ID = 0;
                        invAudit.WarehouseID = goodReceipt.WarehouseID;
                        invAudit.BranchID = goodReceipt.BranchID;
                        invAudit.UserID = goodReceipt.UserID;
                        invAudit.ItemID = itemdt.ItemID;
                        invAudit.CurrencyID = goodReceipt.SysCurID;
                        invAudit.UomID = itemdt.UomID;
                        invAudit.InvoiceNo = goodReceipt.Number_No;
                        invAudit.Trans_Type = docType.Code;
                        invAudit.Process = itemmaster.Process;
                        invAudit.SystemDate = DateTime.Now;
                        invAudit.PostingDate = goodReceipt.PostingDate;
                        invAudit.TimeIn = DateTime.Now.ToString("h:mm:ss tt");
                        invAudit.Qty = @Qty;
                        invAudit.Cost = @Cost;
                        invAudit.Price = 0;
                        invAudit.CumulativeQty = inventory_audit.Sum(q => q.Qty) + @Qty;
                        invAudit.CumulativeValue = (inventory_audit.Sum(s => s.Trans_Valuse)) + (@Qty * @Cost);
                        invAudit.Trans_Valuse = (@Qty * @Cost);
                        invAudit.ExpireDate = itemdt.ExpireDate;
                        invAudit.LocalCurID = goodReceipt.LocalCurID;
                        invAudit.LocalSetRate = goodReceipt.LocalSetRate;
                        invAudit.DocumentTypeID = goodReceipt.DocTypeID;
                        invAudit.CompanyID = goodReceipt.CompanyID;
                        invAudit.SeriesID = goodReceipt.SeriseID;
                        invAudit.SeriesDetailID = goodReceipt.SeriseDID;
                        // update pricelistdetial
                        _utility.CumulativeValue(invAudit.WarehouseID, invAudit.ItemID, invAudit.CumulativeValue, _itemAcc);
                        _context.InventoryAudits.Add(invAudit);
                        _context.WarehouseDetails.AddRange(whsDetials);
                        _context.SaveChanges();
                    }
                }
                else
                {
                    whsDetials.Add(new WarehouseDetail
                    {
                        AdmissionDate = goodReceipt.PostingDate,
                        Cost = @Cost,
                        CurrencyID = goodReceipt.SysCurID,
                        ID = 0,
                        InStock = @Qty,
                        ItemID = itemdt.ItemID,
                        Location = "",
                        ProcessItem = ProcessItem.SEBA,
                        SyetemDate = DateTime.Now,
                        SysNum = 0,
                        TimeIn = DateTime.Now,
                        WarehouseID = goodReceipt.WarehouseID,
                        UomID = itemdt.UomID,
                        UserID = goodReceipt.UserID,
                        TransType = TransTypeWD.GoodsReceipt,
                        IsDeleted = true,
                        GRGIID = goodReceipt.GoodsReceiptID,
                        InStockFrom = goodReceipt.GoodsReceiptID
                    });

                    _context.WarehouseDetails.UpdateRange(whsDetials);
                    _context.SaveChanges();

                }
                if (itemmaster.ManItemBy == ManageItemBy.None)
                {
                    if (itemmaster.Process == "FIFO")
                    {
                        //insert inventoryaudit
                        var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == itemdt.ItemID && w.WarehouseID == goodReceipt.WarehouseID);
                        var pri_detial = _context.PriceListDetails.Where(w => w.ItemID == itemdt.ItemID);
                        itemInven.ID = 0;
                        itemInven.WarehouseID = goodReceipt.WarehouseID;
                        itemInven.BranchID = goodReceipt.BranchID;
                        itemInven.UserID = goodReceipt.UserID;
                        itemInven.ItemID = itemdt.ItemID;
                        itemInven.CurrencyID = goodReceipt.SysCurID;
                        itemInven.UomID = orft.BaseUOM;
                        itemInven.InvoiceNo = goodReceipt.Number_No;
                        itemInven.Trans_Type = docType.Code;
                        itemInven.Process = itemmaster.Process;
                        itemInven.SystemDate = DateTime.Now;
                        itemInven.PostingDate = goodReceipt.PostingDate;
                        itemInven.TimeIn = DateTime.Now.ToShortTimeString();
                        itemInven.Qty = @Qty;
                        itemInven.Cost = @Cost;
                        itemInven.Price = 0;
                        itemInven.CumulativeQty = inventory_audit.Sum(q => q.Qty) + @Qty;
                        itemInven.CumulativeValue = (inventory_audit.Sum(s => s.Trans_Valuse)) + (@Qty * @Cost);
                        itemInven.Trans_Valuse = (@Qty * @Cost);
                        itemInven.ExpireDate = itemdt.ExpireDate;
                        itemInven.LocalCurID = goodReceipt.LocalCurID;
                        itemInven.LocalSetRate = goodReceipt.LocalSetRate;
                        itemInven.SeriesDetailID = goodReceipt.SeriseDID;
                        itemInven.SeriesID = goodReceipt.SeriseID;
                        itemInven.DocumentTypeID = goodReceipt.DocTypeID;
                        itemInven.CompanyID = goodReceipt.CompanyID;
                        //inventoryAccAmount = (decimal)item_inventory_audit.Cost;
                        // update pricelistdetial                   
                        _utility.CumulativeValue(itemInven.WarehouseID, itemInven.ItemID, itemInven.CumulativeValue, _itemAcc);
                    }
                    else
                    {
                        //insert inventoryaudit
                        var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == itemdt.ItemID && w.WarehouseID == goodReceipt.WarehouseID);
                        var warehouse_sammary = _context.WarehouseSummary.FirstOrDefault(w => w.ItemID == itemdt.ItemID && w.WarehouseID == goodReceipt.WarehouseID);
                        var pri_detial = _context.PriceListDetails.Where(w => w.ItemID == itemdt.ItemID);
                        double @AvgCost = (inventory_audit.Sum(s => s.Trans_Valuse) + (@Qty * @Cost)) / (inventory_audit.Sum(q => q.Qty) + @Qty);

                        if (double.IsNaN(@AvgCost) || double.IsInfinity(@AvgCost)) @AvgCost = 0;

                        itemInven.ID = 0;
                        itemInven.WarehouseID = goodReceipt.WarehouseID;
                        itemInven.BranchID = goodReceipt.BranchID;
                        itemInven.UserID = goodReceipt.UserID;
                        itemInven.ItemID = itemdt.ItemID;
                        itemInven.CurrencyID = goodReceipt.SysCurID;
                        itemInven.UomID = orft.BaseUOM;
                        itemInven.InvoiceNo = goodReceipt.Number_No;
                        itemInven.Trans_Type = docType.Code;
                        itemInven.Process = itemmaster.Process;
                        itemInven.SystemDate = DateTime.Now;
                        itemInven.PostingDate = goodReceipt.PostingDate;
                        itemInven.TimeIn = DateTime.Now.ToShortTimeString();
                        itemInven.Qty = @Qty;
                        itemInven.Cost = @AvgCost;
                        itemInven.Price = 0;
                        itemInven.CumulativeQty = inventory_audit.Sum(q => q.Qty) + @Qty;
                        itemInven.CumulativeValue = (inventory_audit.Sum(s => s.Trans_Valuse)) + (Qty * Cost);
                        itemInven.Trans_Valuse = (@Qty * @Cost);
                        itemInven.ExpireDate = itemdt.ExpireDate;
                        itemInven.LocalCurID = goodReceipt.LocalCurID;
                        itemInven.LocalSetRate = goodReceipt.LocalSetRate;
                        itemInven.SeriesDetailID = goodReceipt.SeriseDID;
                        itemInven.SeriesID = goodReceipt.SeriseID;
                        itemInven.DocumentTypeID = goodReceipt.DocTypeID;
                        itemInven.CompanyID = goodReceipt.CompanyID;
                        _utility.UpdateAvgCost(itemdt.ItemID, goodReceipt.WarehouseID, itemmaster.GroupUomID, itemInven.Qty, itemInven.Cost);
                        _utility.CumulativeValue(itemInven.WarehouseID, itemInven.ItemID, itemInven.CumulativeValue, _itemAcc);
                    }
                    _context.InventoryAudits.Add(itemInven);
                    _context.SaveChanges();
                }
            }

            //update bom
            foreach (var item in goodReceitpDetail)
            {
                var ItemBOMDetail = _context.BOMDetail.Where(w => w.ItemID == item.ItemID);
                foreach (var itembom in ItemBOMDetail)
                {
                    var BOM = _context.BOMaterial.First(w => w.BID == itembom.BID);
                    var DBOM = _context.BOMDetail.Where(w => w.BID == BOM.BID && w.Detele == false);
                    var Inven = _context.InventoryAudits.Where(w => w.ItemID == itembom.ItemID);
                    double @AvgCost = (Inven.Sum(s => s.Trans_Valuse)) / (Inven.Sum(q => q.Qty));
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
            }

        }

        #endregion SaveGoodIssuesBasic
        #region  SaveGoodIssues
        public void SaveGoodIssues(int receipt, List<SerialViewModelPurchase> serialViewModelPurchases,
            List<BatchViewModelPurchase> batchViewModelPurchases)
        {
            var goodReceipt = _context.GoodsReceipts.FirstOrDefault(i => i.GoodsReceiptID == receipt);
            var goodReceitpDetail = _context.GoodReceiptDetails.Where(i => i.GoodsReceiptID == goodReceipt.GoodsReceiptID).ToList();
            var docType = _context.DocumentTypes.FirstOrDefault(i => i.Code == "GR") ?? new DocumentType();
            var series = _context.Series.Find(goodReceipt.SeriseID) ?? new Series();
            int inventoryAccID = 0;
            decimal inventoryAccAmount = 0;
            JournalEntry journalEntry = new();
            SeriesDetail seriesDetail = new();
            List<WarehouseDetail> whsDetials = new();
            List<JournalEntryDetail> journalEntryDetail = new();
            List<AccountBalance> accountBalance = new();
            var douTypeID = _context.DocumentTypes.FirstOrDefault(w => w.Code == "JE");
            var defaultJE = _context.Series.FirstOrDefault(w => w.Default == true && w.DocuTypeID == douTypeID.ID && w.NextNo != w.LastNo) ?? new Series();
            if (defaultJE.ID > 0)
            {
                // update series
                string Sno = defaultJE.NextNo;
                long No = long.Parse(Sno);
                defaultJE.NextNo = Convert.ToString(No + 1);
                // update series details
                seriesDetail.SeriesID = defaultJE.ID;
                seriesDetail.Number = Sno;
                _context.Update(defaultJE);
                _context.Update(seriesDetail);
                _context.SaveChanges();
                // Insert Journal Entry
                journalEntry.SeriesID = defaultJE.ID;
                journalEntry.Number = Sno;
                journalEntry.DouTypeID = defaultJE.DocuTypeID;
                journalEntry.Creator = goodReceipt.UserID;
                journalEntry.TransNo = goodReceipt.Number_No;
                journalEntry.PostingDate = goodReceipt.PostingDate;
                journalEntry.DocumentDate = goodReceipt.DocumentDate;
                journalEntry.DueDate = goodReceipt.PostingDate;
                journalEntry.SSCID = goodReceipt.SysCurID;
                journalEntry.LLCID = goodReceipt.LocalCurID;
                journalEntry.CompanyID = goodReceipt.CompanyID;
                journalEntry.LocalSetRate = (decimal)goodReceipt.LocalSetRate;
                journalEntry.SeriesDID = seriesDetail.ID;
                journalEntry.Remarks = series.Name + " " + goodReceipt.Number_No;
                _context.Update(journalEntry);
                _context.SaveChanges();
            }

            decimal sumCost = 0;
            sumCost = (decimal)goodReceitpDetail.Sum(w => w.Cost * w.Quantity);
            // expense account //
            var glAcc = _context.GLAccounts.FirstOrDefault(w => w.ID == goodReceipt.GLID);
            if (glAcc != null)
            {
                journalEntryDetail = new()
                {
                    new JournalEntryDetail
                    {
                        JEID = journalEntry.ID,
                        Type = Financials.Type.GLAcct,
                        ItemID = glAcc.ID,
                        Credit = sumCost,
                    }
                };
                //Insert 
                glAcc.Balance -= sumCost;
                accountBalance = new()
                {
                    new AccountBalance
                    {
                        JEID = journalEntry.ID,
                        PostingDate = goodReceipt.PostingDate,
                        Origin = docType.ID,
                        OriginNo = goodReceipt.Number_No,
                        OffsetAccount = glAcc.Code,
                        Details = douTypeID.Name + "-" + glAcc.Code,
                        CumulativeBalance = glAcc.Balance,
                        Credit = sumCost,
                        LocalSetRate = (decimal)goodReceipt.LocalSetRate,
                        GLAID = glAcc.ID,
                        Effective=EffectiveBlance.Credit
                    }
                };
                //  
                _context.Update(glAcc);
            }
            _context.SaveChanges();
            foreach (var itemdt in goodReceitpDetail)
            {
                var itemmaster = _context.ItemMasterDatas.FirstOrDefault(w => !w.Delete && w.ID == itemdt.ItemID);
                var gd = _context.GroupDUoMs.FirstOrDefault(W => W.GroupUoMID == itemmaster.GroupUomID && W.AltUOM == itemdt.UomID);
                var orft = _context.GroupDUoMs.FirstOrDefault(w => w.GroupUoMID == itemmaster.GroupUomID);
                var warehouse = _context.WarehouseSummary.FirstOrDefault(w => w.ItemID == itemdt.ItemID && w.WarehouseID == goodReceipt.WarehouseID);
                var _itemAcc = _context.ItemAccountings
                    .FirstOrDefault(i => i.ItemID == itemdt.ItemID && i.WarehouseID == goodReceipt.WarehouseID);
                double @Qty = itemdt.Quantity * gd.Factor;
                double @Cost = itemdt.Cost / gd.Factor;
                InventoryAudit itemInven = new();
                // update itmemasterdata                    
                itemmaster.StockIn += @Qty;
                //update warehouse                    
                warehouse.InStock += @Qty;
                _context.ItemMasterDatas.Update(itemmaster);
                _context.WarehouseSummary.Update(warehouse);
                _utility.UpdateItemAccounting(_itemAcc, warehouse);
                //insert warehousedetail
                if (itemmaster.SetGlAccount == Inventory.SetGlAccount.ItemLevel)
                {
                    var inventoryAcc = (from ia in _context.ItemAccountings.Where(w => w.ItemID == itemdt.ItemID && w.WarehouseID == goodReceipt.WarehouseID)
                                        join gl in _context.GLAccounts on ia.InventoryAccount equals gl.Code
                                        select gl
                                         ).FirstOrDefault();
                    inventoryAccID = inventoryAcc.ID;
                    inventoryAccAmount = (decimal)(itemdt.Cost * itemdt.Quantity);
                }
                else if (itemmaster.SetGlAccount == Inventory.SetGlAccount.ItemGroup)
                {
                    var inventoryAcc = (from ia in _context.ItemAccountings.Where(w => w.ItemGroupID == itemmaster.ItemGroup1ID)
                                        join gl in _context.GLAccounts on ia.InventoryAccount equals gl.Code
                                        select gl
                                         ).FirstOrDefault();
                    inventoryAccID = inventoryAcc.ID;
                    inventoryAccAmount = (decimal)(itemdt.Quantity * itemdt.Cost);
                }

                // expence
                var glAccEx = _context.GLAccounts.FirstOrDefault(w => w.ID == itemdt.GLID);
                decimal itemSum = (decimal)(itemdt.Quantity * itemdt.Cost);
                if (glAccEx != null)
                {
                    var journalDetail = journalEntryDetail.FirstOrDefault(w => w.ItemID == itemdt.GLID) ?? new JournalEntryDetail();
                    if (journalDetail.ItemID > 0)
                    {
                        glAccEx.Balance -= itemSum;
                        var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == itemdt.GLID);
                        //journalEntryDetail
                        journalDetail.Credit += itemSum;
                        //accountBalance
                        accBalance.CumulativeBalance = glAccEx.Balance;
                        accBalance.Credit += itemSum;
                    }
                    else
                    {
                        glAccEx.Balance -= itemSum;
                        journalEntryDetail.Add(new JournalEntryDetail
                        {
                            JEID = journalEntry.ID,
                            Type = Financials.Type.GLAcct,
                            ItemID = itemdt.GLID,
                            Credit = itemSum,
                        });
                        //
                        accountBalance.Add(new AccountBalance
                        {
                            JEID = journalEntry.ID,
                            PostingDate = goodReceipt.PostingDate,
                            Origin = docType.ID,
                            OriginNo = goodReceipt.Number_No,
                            OffsetAccount = glAccEx.Code,
                            Details = douTypeID.Name + " - " + glAccEx.Code,
                            CumulativeBalance = glAccEx.Balance,
                            Credit = itemSum,
                            LocalSetRate = (decimal)goodReceipt.LocalSetRate,
                            GLAID = itemdt.GLID,
                            Effective = EffectiveBlance.Credit
                        });
                    }
                    _context.Update(glAccEx);
                }

                //inventoryAccID
                var glAccInven = _context.GLAccounts.FirstOrDefault(w => w.ID == inventoryAccID);
                if (glAccInven != null)
                {
                    var journalDetail = journalEntryDetail.FirstOrDefault(w => w.ItemID == inventoryAccID) ?? new JournalEntryDetail();
                    if (journalDetail.ItemID > 0)
                    {
                        glAccInven.Balance += inventoryAccAmount;
                        var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == inventoryAccID);
                        //journalEntryDetail
                        journalDetail.Debit += inventoryAccAmount;
                        //accountBalance
                        accBalance.CumulativeBalance = glAccInven.Balance;
                        accBalance.Debit += inventoryAccAmount;
                    }
                    else
                    {
                        glAccInven.Balance += inventoryAccAmount;
                        journalEntryDetail.Add(new JournalEntryDetail
                        {
                            JEID = journalEntry.ID,
                            Type = Financials.Type.GLAcct,
                            ItemID = inventoryAccID,
                            Debit = inventoryAccAmount,
                        });
                        //
                        accountBalance.Add(new AccountBalance
                        {
                            JEID = journalEntry.ID,
                            PostingDate = goodReceipt.PostingDate,
                            Origin = docType.ID,
                            OriginNo = goodReceipt.Number_No,
                            OffsetAccount = glAcc == null ? glAccEx.Code : glAcc.Code,
                            Details = douTypeID.Name + " - " + glAccInven.Code,
                            CumulativeBalance = glAccInven.Balance,
                            Debit = inventoryAccAmount,
                            LocalSetRate = (decimal)goodReceipt.LocalSetRate,
                            GLAID = inventoryAccID,
                            Effective = EffectiveBlance.Debit
                        });
                    }
                    _context.Update(glAccInven);
                }

                _context.SaveChanges();
                //insert warehousedetail
                if (itemmaster.ManItemBy == ManageItemBy.SerialNumbers && itemmaster.ManMethod == ManagementMethod.OnEveryTransation)
                {
                    var svmp = serialViewModelPurchases
                        .FirstOrDefault(s => s.ItemID == itemdt.ItemID && s.Cost == (decimal)itemdt.Cost);
                    List<InventoryAudit> inventoryAudit = new();
                    if (svmp != null)
                    {
                        foreach (var sv in svmp.SerialDetialViewModelPurchase)
                        {
                            whsDetials.Add(new WarehouseDetail
                            {
                                AdmissionDate = sv.AdmissionDate,
                                Cost = @Cost,
                                CurrencyID = goodReceipt.SysCurID,
                                Details = sv.Detials,
                                ID = 0,
                                InStock = 1,
                                ItemID = itemdt.ItemID,
                                Location = sv.Location,
                                LotNumber = sv.LotNumber,
                                MfrDate = sv.MfrDate,
                                MfrSerialNumber = sv.MfrSerialNo,
                                MfrWarDateEnd = sv.MfrWarrantyEnd,
                                MfrWarDateStart = sv.MfrWarrantyStart,
                                ProcessItem = ProcessItem.SEBA,
                                SerialNumber = sv.SerialNumber,
                                SyetemDate = DateTime.Now,
                                SysNum = 0,
                                TimeIn = DateTime.Now,
                                WarehouseID = goodReceipt.WarehouseID,
                                UomID = itemdt.UomID,
                                UserID = goodReceipt.UserID,
                                ExpireDate = sv.ExpirationDate == null ? default : (DateTime)sv.ExpirationDate,
                                TransType = TransTypeWD.GoodsReceipt,
                                IsDeleted = true,
                                GRGIID = goodReceipt.GoodsReceiptID,
                                InStockFrom = goodReceipt.GoodsReceiptID
                            });
                        }
                        //insert inventoryaudit
                        InventoryAudit invAudit = new();
                        var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == itemdt.ItemID && w.UomID == itemdt.UomID && w.WarehouseID == goodReceipt.WarehouseID);
                        var pri_detial = _context.PriceListDetails.Where(w => w.ItemID == itemdt.ItemID && w.UomID == itemdt.UomID);
                        invAudit.ID = 0;
                        invAudit.WarehouseID = goodReceipt.WarehouseID;
                        invAudit.BranchID = goodReceipt.BranchID;
                        invAudit.UserID = goodReceipt.UserID;
                        invAudit.ItemID = itemdt.ItemID;
                        invAudit.CurrencyID = goodReceipt.SysCurID;
                        invAudit.UomID = itemdt.UomID;
                        invAudit.InvoiceNo = goodReceipt.Number_No;
                        invAudit.Trans_Type = docType.Code;
                        invAudit.Process = itemmaster.Process;
                        invAudit.SystemDate = DateTime.Now;
                        invAudit.PostingDate = goodReceipt.PostingDate;
                        invAudit.TimeIn = DateTime.Now.ToString("h:mm:ss tt");
                        invAudit.Qty = @Qty;
                        invAudit.Cost = @Cost;
                        invAudit.Price = 0;
                        invAudit.CumulativeQty = inventory_audit.Sum(q => q.Qty) + @Qty;
                        invAudit.CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) + (@Qty * @Cost);
                        invAudit.Trans_Valuse = @Qty * @Cost;
                        invAudit.ExpireDate = itemdt.ExpireDate;
                        invAudit.LocalCurID = goodReceipt.LocalCurID;
                        invAudit.LocalSetRate = goodReceipt.LocalSetRate;
                        invAudit.DocumentTypeID = goodReceipt.DocTypeID;
                        invAudit.CompanyID = goodReceipt.CompanyID;
                        invAudit.SeriesID = goodReceipt.SeriseID;
                        invAudit.SeriesDetailID = goodReceipt.SeriseDID;
                        // update pricelistdetial
                        _utility.CumulativeValue(invAudit.WarehouseID, invAudit.ItemID, invAudit.CumulativeValue, _itemAcc);
                        _context.InventoryAudits.Add(invAudit);
                        _context.WarehouseDetails.AddRange(whsDetials);
                        _context.SaveChanges();
                    }
                }
                else if (itemmaster.ManItemBy == ManageItemBy.Batches && itemmaster.ManMethod == ManagementMethod.OnEveryTransation)
                {
                    var bvmp = batchViewModelPurchases.FirstOrDefault(s => s.ItemID == itemdt.ItemID && s.Cost == (decimal)itemdt.Cost);

                    if (bvmp != null)
                    {
                        var bvs = bvmp.BatchDetialViewModelPurchases.Where(i => !string.IsNullOrEmpty(i.Batch) && i.Qty > 0).ToList();
                        foreach (var bv in bvs)
                        {
                            var _qty = (double)bv.Qty;
                            whsDetials.Add(new WarehouseDetail
                            {
                                AdmissionDate = bv.AdmissionDate,
                                Cost = @Cost,
                                CurrencyID = goodReceipt.SysCurID,
                                Details = bv.Detials,
                                ID = 0,
                                InStock = _qty,
                                ItemID = itemdt.ItemID,
                                Location = bv.Location,
                                MfrDate = bv.MfrDate,
                                ProcessItem = ProcessItem.SEBA,
                                SyetemDate = DateTime.Now,
                                SysNum = 0,
                                TimeIn = DateTime.Now,
                                WarehouseID = goodReceipt.WarehouseID,
                                UomID = itemdt.UomID,
                                UserID = goodReceipt.UserID,
                                ExpireDate = bv.ExpirationDate == null ? default : (DateTime)bv.ExpirationDate,
                                BatchAttr1 = bv.BatchAttribute1,
                                BatchAttr2 = bv.BatchAttribute2,
                                BatchNo = bv.Batch,
                                TransType = TransTypeWD.GoodsReceipt,
                                IsDeleted = true,
                                GRGIID = goodReceipt.GoodsReceiptID,
                                InStockFrom = goodReceipt.GoodsReceiptID
                            });
                        }
                        //insert inventoryaudit
                        InventoryAudit invAudit = new();
                        var inventory_audit = _context.InventoryAudits
                            .Where(w => w.ItemID == itemdt.ItemID && w.UomID == itemdt.UomID && w.WarehouseID == goodReceipt.WarehouseID)
                            .ToList();
                        // var pri_detial = _context.PriceListDetails.Where(w => w.ItemID == itemdt.ItemID && w.UomID == itemdt.UomID);
                        invAudit.ID = 0;
                        invAudit.WarehouseID = goodReceipt.WarehouseID;
                        invAudit.BranchID = goodReceipt.BranchID;
                        invAudit.UserID = goodReceipt.UserID;
                        invAudit.ItemID = itemdt.ItemID;
                        invAudit.CurrencyID = goodReceipt.SysCurID;
                        invAudit.UomID = itemdt.UomID;
                        invAudit.InvoiceNo = goodReceipt.Number_No;
                        invAudit.Trans_Type = docType.Code;
                        invAudit.Process = itemmaster.Process;
                        invAudit.SystemDate = DateTime.Now;
                        invAudit.PostingDate = goodReceipt.PostingDate;
                        invAudit.TimeIn = DateTime.Now.ToString("h:mm:ss tt");
                        invAudit.Qty = @Qty;
                        invAudit.Cost = @Cost;
                        invAudit.Price = 0;
                        invAudit.CumulativeQty = inventory_audit.Sum(q => q.Qty) + @Qty;
                        invAudit.CumulativeValue = (inventory_audit.Sum(s => s.Trans_Valuse)) + (@Qty * @Cost);
                        invAudit.Trans_Valuse = (@Qty * @Cost);
                        invAudit.ExpireDate = itemdt.ExpireDate;
                        invAudit.LocalCurID = goodReceipt.LocalCurID;
                        invAudit.LocalSetRate = goodReceipt.LocalSetRate;
                        invAudit.DocumentTypeID = goodReceipt.DocTypeID;
                        invAudit.CompanyID = goodReceipt.CompanyID;
                        invAudit.SeriesID = goodReceipt.SeriseID;
                        invAudit.SeriesDetailID = goodReceipt.SeriseDID;
                        // update pricelistdetial
                        _utility.CumulativeValue(invAudit.WarehouseID, invAudit.ItemID, invAudit.CumulativeValue, _itemAcc);
                        _context.InventoryAudits.Add(invAudit);
                        _context.WarehouseDetails.AddRange(whsDetials);
                        _context.SaveChanges();
                    }
                }
                else
                {
                    whsDetials.Add(new WarehouseDetail
                    {

                        AdmissionDate = goodReceipt.PostingDate,
                        Cost = @Cost,
                        CurrencyID = goodReceipt.SysCurID,
                        ID = 0,
                        InStock = @Qty,
                        ItemID = itemdt.ItemID,
                        SyetemDate = DateTime.Now,
                        SysNum = 0,
                        TimeIn = DateTime.Now,
                        WarehouseID = goodReceipt.WarehouseID,
                        UomID = itemdt.UomID,
                        UserID = goodReceipt.UserID,
                        TransType = TransTypeWD.GoodsReceipt,
                        IsDeleted = true,
                        GRGIID = goodReceipt.GoodsReceiptID,
                        InStockFrom = goodReceipt.GoodsReceiptID
                    });
                    _context.WarehouseDetails.UpdateRange(whsDetials);
                    _context.SaveChanges();
                }
                if (itemmaster.ManItemBy == ManageItemBy.None)
                {
                    if (itemmaster.Process == "FIFO")
                    {
                        //insert inventoryaudit
                        var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == itemdt.ItemID && w.WarehouseID == goodReceipt.WarehouseID);
                        var pri_detial = _context.PriceListDetails.Where(w => w.ItemID == itemdt.ItemID);
                        itemInven.ID = 0;
                        itemInven.WarehouseID = goodReceipt.WarehouseID;
                        itemInven.BranchID = goodReceipt.BranchID;
                        itemInven.UserID = goodReceipt.UserID;
                        itemInven.ItemID = itemdt.ItemID;
                        itemInven.CurrencyID = goodReceipt.SysCurID;
                        itemInven.UomID = orft.BaseUOM;
                        itemInven.InvoiceNo = goodReceipt.Number_No;
                        itemInven.Trans_Type = docType.Code;
                        itemInven.Process = itemmaster.Process;
                        itemInven.SystemDate = DateTime.Now;
                        itemInven.PostingDate = goodReceipt.PostingDate;
                        itemInven.TimeIn = DateTime.Now.ToShortTimeString();
                        itemInven.Qty = @Qty;
                        itemInven.Cost = @Cost;
                        itemInven.Price = 0;
                        itemInven.CumulativeQty = inventory_audit.Sum(q => q.Qty) + @Qty;
                        itemInven.CumulativeValue = (inventory_audit.Sum(s => s.Trans_Valuse)) + (@Qty * @Cost);
                        itemInven.Trans_Valuse = (@Qty * @Cost);
                        itemInven.ExpireDate = itemdt.ExpireDate;
                        itemInven.LocalCurID = goodReceipt.LocalCurID;
                        itemInven.LocalSetRate = goodReceipt.LocalSetRate;
                        itemInven.SeriesDetailID = goodReceipt.SeriseDID;
                        itemInven.SeriesID = goodReceipt.SeriseID;
                        itemInven.DocumentTypeID = goodReceipt.DocTypeID;
                        itemInven.CompanyID = goodReceipt.CompanyID;
                        //inventoryAccAmount = (decimal)item_inventory_audit.Cost;
                        // update pricelistdetial                   
                        _utility.CumulativeValue(itemInven.WarehouseID, itemInven.ItemID, itemInven.CumulativeValue, _itemAcc);
                    }
                    else
                    {
                        //insert inventoryaudit
                        var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == itemdt.ItemID && w.WarehouseID == goodReceipt.WarehouseID);
                        var warehouse_sammary = _context.WarehouseSummary.FirstOrDefault(w => w.ItemID == itemdt.ItemID && w.WarehouseID == goodReceipt.WarehouseID);
                        var pri_detial = _context.PriceListDetails.Where(w => w.ItemID == itemdt.ItemID);
                        double @AvgCost = (inventory_audit.Sum(s => s.Trans_Valuse) + (@Qty * @Cost)) / (inventory_audit.Sum(q => q.Qty) + @Qty);

                        if (double.IsNaN(@AvgCost) || double.IsInfinity(@AvgCost)) @AvgCost = 0;

                        itemInven.ID = 0;
                        itemInven.WarehouseID = goodReceipt.WarehouseID;
                        itemInven.BranchID = goodReceipt.BranchID;
                        itemInven.UserID = goodReceipt.UserID;
                        itemInven.ItemID = itemdt.ItemID;
                        itemInven.CurrencyID = goodReceipt.SysCurID;
                        itemInven.UomID = orft.BaseUOM;
                        itemInven.InvoiceNo = goodReceipt.Number_No;
                        itemInven.Trans_Type = docType.Code;
                        itemInven.Process = itemmaster.Process;
                        itemInven.SystemDate = DateTime.Now;
                        itemInven.PostingDate = goodReceipt.PostingDate;
                        itemInven.TimeIn = DateTime.Now.ToShortTimeString();
                        itemInven.Qty = @Qty;
                        itemInven.Cost = @AvgCost;
                        itemInven.Price = 0;
                        itemInven.CumulativeQty = inventory_audit.Sum(q => q.Qty) + @Qty;
                        itemInven.CumulativeValue = (inventory_audit.Sum(s => s.Trans_Valuse)) + (Qty * Cost);
                        itemInven.Trans_Valuse = (@Qty * @Cost);
                        itemInven.ExpireDate = itemdt.ExpireDate;
                        itemInven.LocalCurID = goodReceipt.LocalCurID;
                        itemInven.LocalSetRate = goodReceipt.LocalSetRate;
                        itemInven.SeriesDetailID = goodReceipt.SeriseDID;
                        itemInven.SeriesID = goodReceipt.SeriseID;
                        itemInven.DocumentTypeID = goodReceipt.DocTypeID;
                        itemInven.CompanyID = goodReceipt.CompanyID;
                        _utility.UpdateAvgCost(itemdt.ItemID, goodReceipt.WarehouseID, itemmaster.GroupUomID, itemInven.Qty, itemInven.Cost);
                        _utility.CumulativeValue(itemInven.WarehouseID, itemInven.ItemID, itemInven.CumulativeValue, _itemAcc);
                    }
                    _context.InventoryAudits.Add(itemInven);
                    _context.SaveChanges();
                }
            }

            //update bom
            foreach (var item in goodReceitpDetail)
            {
                var ItemBOMDetail = _context.BOMDetail.Where(w => w.ItemID == item.ItemID);
                foreach (var itembom in ItemBOMDetail)
                {
                    var BOM = _context.BOMaterial.First(w => w.BID == itembom.BID);
                    var DBOM = _context.BOMDetail.Where(w => w.BID == BOM.BID && w.Detele == false);
                    var Inven = _context.InventoryAudits.Where(w => w.ItemID == itembom.ItemID);
                    double @AvgCost = (Inven.Sum(s => s.Trans_Valuse)) / (Inven.Sum(q => q.Qty));
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
            }
            var journal = _context.JournalEntries.Find(journalEntry.ID);
            journal.TotalDebit = journalEntryDetail.Sum(s => s.Debit);
            journal.TotalCredit = journalEntryDetail.Sum(s => s.Credit);
            _context.JournalEntries.Update(journal);
            _context.JournalEntryDetails.UpdateRange(journalEntryDetail);
            _context.AccountBalances.UpdateRange(accountBalance);
            _context.SaveChanges();
        }
        #endregion SaveGoodIssues
        public List<GoodsReceiptViewModel> GetAllPurItemsByItemID(int wareid, int itemId, int comId)
        {
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
            WHViewModel _tg = new()
            {
                ID = 0,
                Name = "--Select--",
                Code = "",
                BranchID = 0
            };
            var warehouse = (from wd in _context.Warehouses
                             select new WHViewModel
                             {
                                 Name = wd.Name,
                                 ID = wd.ID,
                                 BranchID = wd.BranchID,
                                 Code = wd.Code
                             }).ToList();
            warehouse.Insert(0, _tg);
            List<GoodsReceiptViewModel> items = new();
            var allPurItems = (from wd in _context.WarehouseDetails.Where(i => i.WarehouseID == wareid && i.ItemID == itemId && i.Cost == 0)
                               join item in _context.ItemMasterDatas.Where(i => !i.Delete) on wd.ItemID equals item.ID
                               join GDU in _context.GroupDUoMs on item.GroupUomID equals GDU.GroupUoMID
                               join uom in _context.UnitofMeasures on item.InventoryUoMID equals uom.ID
                               join cur in _context.Currency on wd.CurrencyID equals cur.ID
                               let ws = _context.WarehouseSummary.Where(i => i.WarehouseID == wd.WarehouseID && i.ItemID == item.ID)

                               select new GoodsReceiptViewModel
                               {
                                   GoodReceitpDetailID = 0,
                                   GoodsReceiptID = 0,
                                   CurrencyID = wd.CurrencyID,
                                   LineID = wd.ID,
                                   ItemID = wd.ItemID,
                                   UomID = (int)item.InventoryUoMID,
                                   Code = item.Code,
                                   KhmerName = item.KhmerName,
                                   EnglishName = item.EnglishName,
                                   Quantity = item.IsLimitOrder ? item.MinOrderQty : 0,
                                   Cost = wd.Cost,
                                   PaymentMeans = "",
                                   CostStore = wd.Cost,
                                   Currency = cur.Description,
                                   UomName = uom.Name,
                                   BarCode = item.Barcode,
                                   Type = item.Process,
                                   ManageExpire = item.ManageExpire,
                                   ExpireDate = wd.ExpireDate,
                                   AvgCost = ws.FirstOrDefault().Cost,
                                   Warehouse = warehouse.Select(c => new SelectListItem
                                   {
                                       Value = c.ID.ToString(),
                                       Text = c.Name,
                                       Selected = c.ID == warehouse.FirstOrDefault().ID
                                   }).ToList(),
                                   UoMs = uoms.Count() < 0 ? new List<SelectListItem>() : uoms.Select(c => new SelectListItem
                                   {
                                       Value = c.ID.ToString(),
                                       Text = c.Name,
                                       Selected = item.InventoryUoMID == c.ID
                                   }).ToList(),
                                   UOMView = uoms.ToList(),
                                   IsLimitOrder = item.IsLimitOrder,
                                   MinOrderQty = item.MinOrderQty,
                                   MaxOrderQty = item.MaxOrderQty,
                               }
                               ).ToList();
            var fifoitem = allPurItems.Where(i => i.Type == "FIFO" || i.Type == "SEBA").GroupBy(i => i.Cost).Select(x => x.First()).ToList();
            var avgitem = allPurItems.FirstOrDefault(i => i.Type == "Average");
            if (fifoitem != null)
                items.AddRange(fifoitem);
            if (avgitem != null)
                items.Add(avgitem);
            foreach (var item in items)
            {
                if (item.Type == "Average")
                {
                    item.Cost = item.AvgCost;
                }
            }
            _dataProp.DataProperty(items, comId, "ItemID", "AddictionProps");
            return items;
        }
    }
}
