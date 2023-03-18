using Microsoft.EntityFrameworkCore;
using CKBS.AppContext;
using CKBS.Models.Services.Administrator.Inventory;
using CKBS.Models.Services.Inventory.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using CKBS.Models.Services.Inventory;
using Microsoft.AspNetCore.Mvc.Rendering;
using CKBS.Models.ServicesClass.GoodsIssue;
using CKBS.Models.Services.Financials;
using CKBS.Models.Services.Administrator.SystemInitialization;
using CKBS.Models.Services.ChartOfAccounts;
using CKBS.Models.Services.Banking;
using KEDI.Core.Premise.Models.ServicesClass.SerialBatches.Sales;
using KEDI.Core.Premise.Models.Services.Administrator.Inventory;
using KEDI.Core.Premise.Repository;
using CKBS.Models.ServicesClass.Property;

namespace CKBS.Models.Services.Responsitory
{
    public interface IGoodIssuse
    {
        IEnumerable<Warehouse> GetWarehouse_From(int ID);
        IEnumerable<Warehouse> GetWarehouse_To(int ID);
        List<GoodIssuesDetail> GetItemByWarehouse_From(int warehouseID, int comId);
        ItemMasterData GetItemFindBarcode(string barcode);
        void SaveGoodIssues(GoodIssues issues, List<SerialNumber> serials, List<BatchNo> batches);
        void IssuesStock(int IssuesID, int ComID, List<SerialNumber> serials, List<BatchNo> batches);
        void IssuesStockBasic(int IssuesID, int ComID, List<SerialNumber> serials, List<BatchNo> batches);


        List<GoodsIssueViewModel> GetAllPurItemsByItemID(int wareid, int itemId, int comId);
    }
    public class GoodIssuseResponsitory : IGoodIssuse
    {
        private readonly DataContext _context;
        private readonly IDataPropertyRepository _dataProp;
        private readonly UtilityModule _utility;
        public GoodIssuseResponsitory(DataContext context, IDataPropertyRepository dataProperty, UtilityModule utility)
        {
            _context = context;
            _dataProp = dataProperty;
            _utility = utility;
        }

        public IEnumerable<Warehouse> GetWarehouse_From(int ID) => _context.Warehouses.Where(x => x.Delete == false && x.BranchID == ID).ToList();
        public IEnumerable<Warehouse> GetWarehouse_To(int ID) => _context.Warehouses.Where(x => x.Delete == false && x.BranchID == ID).ToList();
        public List<GoodIssuesDetail> GetItemByWarehouse_From(int warehouseID, int comId)
        {
            var items = (from wd in _context.WarehouseDetails
                         join item in _context.ItemMasterDatas on wd.ItemID equals item.ID
                         join uom in _context.UnitofMeasures on item.InventoryUoMID equals uom.ID
                         join cur in _context.Currency on wd.CurrencyID equals cur.ID
                         where wd.WarehouseID == warehouseID && !item.Delete && item.Process != "Standard" && wd.InStock != 0 && item.Inventory && item.Purchase
                         group new { wd, item, uom, cur } by new { item.ID } into g
                         let data = g.FirstOrDefault()
                         select new GoodIssuesDetail
                         {
                             CurrencyID = data.cur.ID,
                             BarCode = data.item.Barcode,
                             Code = data.item.Code,
                             Cost = data.wd.Cost,
                             Currency = data.cur.Description,
                             EnglishName = data.item.EnglishName,
                             KhmerName = data.item.KhmerName,
                             ItemID = data.item.ID,
                             LineID = data.wd.ID,
                             Quantity = g.Sum(i => i.wd.InStock),
                             UomID = data.uom.ID,
                             UomName = data.uom.Name,
                         }).ToList();
            _dataProp.DataProperty(items, comId, "ItemID", "AddictionProps");
            return items;
        }
        public void SaveGoodIssues(GoodIssues issues, List<SerialNumber> serials, List<BatchNo> batches)
        {
            var user = _context.UserAccounts.FirstOrDefault(w => w.ID == issues.UserID);
            var com = _context.Company.Where(w => !w.Delete && w.ID == user.CompanyID);
            var lc = _context.ExchangeRates.Where(w => w.CurrencyID == com.FirstOrDefault().LocalCurrencyID);
            issues.SysCurID = com.FirstOrDefault().SystemCurrencyID;
            issues.LocalCurID = com.FirstOrDefault().LocalCurrencyID;
            issues.LocalSetRate = lc.FirstOrDefault().SetRate;
            _context.GoodIssues.Add(issues);
            _context.SaveChanges();
            var IssuesID = issues.GoodIssuesID;
            IssuesStock(IssuesID, com.FirstOrDefault().ID, serials, batches);
        }
        public ItemMasterData GetItemFindBarcode(string barcode)
        {
            var data = _context.ItemMasterDatas.FirstOrDefault(i => i.Barcode == barcode);
            return data;
        }

        #region IssuesStockBasic
        public void IssuesStockBasic(int IssuesID, int comID, List<SerialNumber> serials, List<BatchNo> batches)
        {
            var Com = _context.Company.FirstOrDefault(c => !c.Delete && c.ID == comID);
            var Order = _context.GoodIssues.First(w => w.GoodIssuesID == IssuesID);
            var OrderDetails = _context.GoodIssuesDetails.Where(w => w.GoodIssuesID == IssuesID).ToList();
            var docType = _context.DocumentTypes.Find(Order.DocTypeID) ?? new DocumentType();
            var series = _context.Series.Find(Order.SeriseID) ?? new Series();
            var glAccs = _context.GLAccounts.Where(i => i.IsActive).ToList();
            SeriesDetail seriesDetail = new();
            decimal sumCost = 0;
            sumCost = (decimal)OrderDetails.Sum(s => s.Cost * s.Quantity);
            foreach (var item in OrderDetails)
            {
                var itemMaster = _context.ItemMasterDatas.FirstOrDefault(w => w.ID == item.ItemID);
                var baseUOM = _context.GroupDUoMs.FirstOrDefault(w => w.UoMID == item.UomID);
                var warehouse = _context.WarehouseSummary.FirstOrDefault(w => w.ItemID == item.ItemID && w.WarehouseID == Order.WarehouseID);
                var warehouseDetail = _context.WarehouseDetails.FirstOrDefault(w => w.ID == item.LineID) ?? new WarehouseDetail();
                var gd = _context.GroupDUoMs.FirstOrDefault(W => W.GroupUoMID == itemMaster.GroupUomID && W.AltUOM == item.UomID);
                var _itemAcc = _context.ItemAccountings
                    .FirstOrDefault(i => i.ItemID == item.ItemID && i.WarehouseID == Order.WarehouseID);
                double @Qty = item.Quantity * gd.Factor;
                double @Cost = item.Cost / gd.Factor;
                var item_inventory_audit = new InventoryAudit();
                //var warehousedetail = new WarehouseDetail();
                // update itmemasterdata                    
                itemMaster.StockIn -= @Qty;
                //update warehouse                    
                warehouse.InStock -= @Qty;
                warehouseDetail.InStock -= @Qty;
                _context.WarehouseDetails.Update(warehouseDetail);
                _context.ItemMasterDatas.Update(itemMaster);
                _context.WarehouseSummary.Update(warehouse);
                _utility.UpdateItemAccounting(_itemAcc, warehouse);

                if (itemMaster.ManItemBy == ManageItemBy.SerialNumbers && itemMaster.ManMethod == ManagementMethod.OnEveryTransation)
                {
                    if (serials.Count > 0)
                    {
                        foreach (var s in serials)
                        {
                            if (s.SerialNumberSelected != null)
                            {
                                foreach (var ss in s.SerialNumberSelected.SerialNumberSelectedDetails)
                                {
                                    var waredetial = _context.WarehouseDetails.FirstOrDefault(i => ss.SerialNumber == i.SerialNumber && i.InStock > 0);
                                    decimal _inventoryAccAmount = 0M;
                                    decimal _COGSAccAmount = 0M;
                                    if (waredetial != null)
                                    {
                                        waredetial.InStock -= 1;
                                        // insert to warehouse detail
                                        var ware = new StockOut
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
                                            UserID = Order.UserID,
                                            ExpireDate = item.ExpireDate,
                                            TransType = TransTypeWD.GoodsIssue,
                                            TransID = Order.GoodIssuesID,
                                            FromWareDetialID = waredetial.ID,
                                            OutStockFrom = Order.GoodIssuesID,
                                        };
                                        _inventoryAccAmount = (decimal)waredetial.Cost;
                                        _COGSAccAmount = (decimal)waredetial.Cost;
                                        _context.StockOuts.Add(ware);
                                        _context.SaveChanges();
                                    }
                                }
                            }
                        }
                        // Insert to Inventory Audit
                        var inventory_audit = _context.InventoryAudits
                            .Where(w => w.ItemID == item.ItemID && w.WarehouseID == Order.WarehouseID);
                        var inventory = new InventoryAudit
                        {
                            ID = 0,
                            WarehouseID = Order.WarehouseID,
                            BranchID = Order.BranchID,
                            UserID = Order.UserID,
                            ItemID = item.ItemID,
                            CurrencyID = Order.LocalCurID,
                            UomID = baseUOM.BaseUOM,
                            InvoiceNo = Order.Number_No,
                            Trans_Type = docType.Code,
                            Process = itemMaster.Process,
                            SystemDate = DateTime.Now,
                            PostingDate = Order.PostingDate,
                            TimeIn = DateTime.Now.ToShortTimeString().ToString(),
                            Qty = @Qty * -1,
                            Cost = @Cost,
                            Price = 0,
                            CumulativeQty = inventory_audit.Sum(q => q.Qty) - @Qty,
                            CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) - (@Qty * @Cost),
                            Trans_Valuse = @Qty * @Cost * -1,
                            ExpireDate = item.ExpireDate,
                            LocalCurID = Order.LocalCurID,
                            LocalSetRate = Order.LocalSetRate,
                            CompanyID = Order.CompanyID,
                            DocumentTypeID = docType.ID,
                            SeriesID = Order.SeriseID,
                            SeriesDetailID = Order.SeriseDID,
                        };
                        _utility.CumulativeValue(inventory.WarehouseID, inventory.ItemID, inventory.CumulativeValue, _itemAcc);
                        _context.InventoryAudits.Add(inventory);
                        _context.SaveChanges();
                    }
                }
                else if (itemMaster.ManItemBy == ManageItemBy.Batches && itemMaster.ManMethod == ManagementMethod.OnEveryTransation)
                {
                    if (batches.Count > 0)
                    {
                        foreach (var b in batches)
                        {
                            if (b.BatchNoSelected != null)
                            {
                                foreach (var sb in b.BatchNoSelected.BatchNoSelectedDetails)
                                {
                                    decimal selectedQty = sb.SelectedQty * (decimal)baseUOM.Factor;
                                    var waredetial = _context.WarehouseDetails.FirstOrDefault(i => sb.BatchNo == i.BatchNo && i.InStock > 0);
                                    decimal _inventoryAccAmount = 0M;
                                    decimal _COGSAccAmount = 0M;
                                    if (waredetial != null)
                                    {
                                        waredetial.InStock -= (double)selectedQty;

                                        // insert to waredetial
                                        var ware = new StockOut
                                        {
                                            AdmissionDate = waredetial.AdmissionDate,
                                            Cost = (decimal)waredetial.Cost,
                                            CurrencyID = waredetial.CurrencyID,
                                            Details = waredetial.Details,
                                            ID = 0,
                                            InStock = selectedQty * -1,
                                            ItemID = item.ItemID,
                                            Location = waredetial.Location,
                                            MfrDate = waredetial.MfrDate,
                                            ProcessItem = ProcessItem.SEBA,
                                            SyetemDate = DateTime.Now,
                                            SysNum = 0,
                                            TimeIn = DateTime.Now,
                                            WarehouseID = waredetial.WarehouseID,
                                            UomID = item.UomID,
                                            UserID = Order.UserID,
                                            ExpireDate = item.ExpireDate,
                                            BatchAttr1 = waredetial.BatchAttr1,
                                            BatchAttr2 = waredetial.BatchAttr2,
                                            BatchNo = waredetial.BatchNo,
                                            TransType = TransTypeWD.GoodsIssue,
                                            TransID = Order.GoodIssuesID,
                                            FromWareDetialID = waredetial.ID,
                                            OutStockFrom = Order.GoodIssuesID,
                                        };

                                        _inventoryAccAmount = (decimal)waredetial.Cost * selectedQty;
                                        _COGSAccAmount = (decimal)waredetial.Cost * selectedQty;
                                        _context.StockOuts.Add(ware);
                                        _context.SaveChanges();
                                    }
                                }
                            }
                        }
                        // insert to inventory audit
                        var inventory_audit = _context.InventoryAudits
                            .Where(w => w.ItemID == item.ItemID && w.WarehouseID == Order.WarehouseID);
                        var inventory = new InventoryAudit
                        {
                            ID = 0,
                            WarehouseID = Order.WarehouseID,
                            BranchID = Order.BranchID,
                            UserID = Order.UserID,
                            ItemID = item.ItemID,
                            CurrencyID = Order.LocalCurID,
                            UomID = baseUOM.BaseUOM,
                            InvoiceNo = Order.Number_No,
                            Trans_Type = docType.Code,
                            Process = itemMaster.Process,
                            SystemDate = DateTime.Now,
                            PostingDate = Order.PostingDate,
                            TimeIn = DateTime.Now.ToShortTimeString().ToString(),
                            Qty = @Qty * -1,
                            Cost = @Cost,
                            Price = 0,
                            CumulativeQty = inventory_audit.Sum(q => q.Qty) - @Qty,
                            CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) - (@Qty * @Cost),
                            Trans_Valuse = @Qty * @Cost * -1,
                            ExpireDate = item.ExpireDate,
                            LocalCurID = Order.LocalCurID,
                            LocalSetRate = Order.LocalSetRate,
                            CompanyID = Order.CompanyID,
                            DocumentTypeID = docType.ID,
                            SeriesID = Order.SeriseID,
                            SeriesDetailID = Order.SeriseDID,
                        };
                        _utility.CumulativeValue(inventory.WarehouseID, inventory.ItemID, inventory.CumulativeValue, _itemAcc);
                        _context.InventoryAudits.Add(inventory);
                    }
                }
                else
                {
                    if (itemMaster.Process == "FIFO")
                    {

                        var stockOuts = new StockOut
                        {
                            Cost = (decimal)warehouseDetail.Cost,
                            CurrencyID = warehouseDetail.CurrencyID,
                            ID = 0,
                            InStock = (decimal)@Qty,
                            ItemID = item.ItemID,
                            ProcessItem = ProcessItem.FIFO,
                            SyetemDate = DateTime.Now,
                            TimeIn = DateTime.Now,
                            WarehouseID = warehouseDetail.WarehouseID,
                            UomID = item.UomID,
                            UserID = Order.UserID,
                            ExpireDate = item.ExpireDate,
                            TransType = TransTypeWD.PurCreditMemo,
                            FromWareDetialID = warehouseDetail.ID,
                            OutStockFrom = Order.GoodIssuesID,
                        };
                        _context.StockOuts.Add(stockOuts);
                        //insert inventoryaudit
                        var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item.ItemID && w.WarehouseID == Order.WarehouseID);
                        var pri_detial = _context.PriceListDetails.Where(w => w.ItemID == item.ItemID);
                        item_inventory_audit.ID = 0;
                        item_inventory_audit.WarehouseID = Order.WarehouseID;
                        item_inventory_audit.BranchID = Order.BranchID;
                        item_inventory_audit.UserID = Order.UserID;
                        item_inventory_audit.ItemID = item.ItemID;
                        item_inventory_audit.CurrencyID = Order.SysCurID;
                        item_inventory_audit.UomID = (int)itemMaster.InventoryUoMID;
                        item_inventory_audit.InvoiceNo = Order.Number_No;
                        item_inventory_audit.Trans_Type = docType.Code;
                        item_inventory_audit.Process = itemMaster.Process;
                        item_inventory_audit.SystemDate = DateTime.Now;
                        item_inventory_audit.PostingDate = Order.PostingDate;
                        item_inventory_audit.TimeIn = DateTime.Now.ToShortTimeString();
                        item_inventory_audit.Qty = @Qty * -1;
                        item_inventory_audit.Cost = @Cost;
                        item_inventory_audit.Price = 0;
                        item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) - @Qty;
                        item_inventory_audit.CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) - (@Qty * @Cost);
                        item_inventory_audit.Trans_Valuse = @Qty * @Cost * -1;
                        item_inventory_audit.ExpireDate = item.ExpireDate;
                        item_inventory_audit.LocalCurID = Order.LocalCurID;
                        item_inventory_audit.LocalSetRate = Order.LocalSetRate;
                        item_inventory_audit.SeriesDetailID = Order.SeriseDID;
                        item_inventory_audit.SeriesID = Order.SeriseID;
                        item_inventory_audit.DocumentTypeID = Order.DocTypeID;
                        item_inventory_audit.CompanyID = Order.CompanyID;
                        _utility.CumulativeValue(item_inventory_audit.WarehouseID, item_inventory_audit.ItemID, item_inventory_audit.CumulativeValue, _itemAcc);
                    }
                    else
                    {
                        //insert inventoryaudit
                        var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item.ItemID && w.WarehouseID == Order.WarehouseID);
                        var warehouse_sammary = _context.WarehouseSummary.FirstOrDefault(w => w.ItemID == item.ItemID && w.WarehouseID == Order.WarehouseID);
                        var pri_detial = _context.PriceListDetails.Where(w => w.ItemID == item.ItemID);
                        InventoryAudit inventoryAudit = new() { Qty = @Qty, Cost = @Cost };
                        double @AvgCost = _utility.CalAVGCost(item.ItemID, Order.WarehouseID, inventoryAudit);
                        var stockOuts = new StockOut
                        {
                            Cost = (decimal)@AvgCost,
                            CurrencyID = warehouseDetail.CurrencyID,
                            ID = 0,
                            InStock = (decimal)@Qty,
                            ItemID = item.ItemID,
                            ProcessItem = ProcessItem.Average,
                            SyetemDate = DateTime.Now,
                            TimeIn = DateTime.Now,
                            WarehouseID = warehouseDetail.WarehouseID,
                            UomID = item.UomID,
                            UserID = Order.UserID,
                            ExpireDate = item.ExpireDate,
                            TransType = TransTypeWD.PurCreditMemo,
                            FromWareDetialID = warehouseDetail.ID,
                            OutStockFrom = Order.GoodIssuesID,
                        };
                        _context.StockOuts.Add(stockOuts);

                        item_inventory_audit.ID = 0;
                        item_inventory_audit.WarehouseID = Order.WarehouseID;
                        item_inventory_audit.BranchID = Order.BranchID;
                        item_inventory_audit.UserID = Order.UserID;
                        item_inventory_audit.ItemID = item.ItemID;
                        item_inventory_audit.CurrencyID = Order.SysCurID;
                        item_inventory_audit.UomID = (int)itemMaster.InventoryUoMID;
                        item_inventory_audit.InvoiceNo = Order.Number_No;
                        item_inventory_audit.Trans_Type = docType.Code;
                        item_inventory_audit.Process = itemMaster.Process;
                        item_inventory_audit.SystemDate = DateTime.Now;
                        item_inventory_audit.PostingDate = Order.PostingDate;
                        item_inventory_audit.TimeIn = DateTime.Now.ToShortTimeString();
                        item_inventory_audit.Qty = @Qty * -1;
                        item_inventory_audit.Cost = @AvgCost;
                        item_inventory_audit.Price = 0;
                        item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) - @Qty;
                        item_inventory_audit.CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) - (Qty * Cost);
                        item_inventory_audit.Trans_Valuse = @Qty * @Cost * -1;
                        item_inventory_audit.ExpireDate = item.ExpireDate;
                        item_inventory_audit.LocalCurID = Order.LocalCurID;
                        item_inventory_audit.LocalSetRate = Order.LocalSetRate;
                        item_inventory_audit.SeriesDetailID = Order.SeriseDID;
                        item_inventory_audit.SeriesID = Order.SeriseID;
                        item_inventory_audit.DocumentTypeID = Order.DocTypeID;
                        item_inventory_audit.CompanyID = Order.CompanyID;
                        _utility.UpdateAvgCost(item.ItemID, Order.WarehouseID, itemMaster.GroupUomID, item_inventory_audit.Qty, item_inventory_audit.Cost);
                        _utility.CumulativeValue(item_inventory_audit.WarehouseID, item_inventory_audit.ItemID, item_inventory_audit.CumulativeValue, _itemAcc);
                    }
                    _context.InventoryAudits.Add(item_inventory_audit);
                    _context.SaveChanges();
                }
            }
        }

        #endregion IssuesStockBasic
        #region  IssuesStock
        public void IssuesStock(int IssuesID, int comID, List<SerialNumber> serials, List<BatchNo> batches)
        {
            var Com = _context.Company.FirstOrDefault(c => !c.Delete && c.ID == comID);
            var Order = _context.GoodIssues.First(w => w.GoodIssuesID == IssuesID);
            var OrderDetails = _context.GoodIssuesDetails.Where(w => w.GoodIssuesID == IssuesID).ToList();
            var docType = _context.DocumentTypes.Find(Order.DocTypeID) ?? new DocumentType();
            var series = _context.Series.Find(Order.SeriseID) ?? new Series();
            var glAccs = _context.GLAccounts.Where(i => i.IsActive).ToList();
            int inventoryAccID = 0;
            decimal inventoryAccAmount = 0;
            JournalEntry journalEntry = new();
            SeriesDetail seriesDetail = new();
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
                journalEntry.Creator = Order.UserID;
                journalEntry.TransNo = Order.Number_No;
                journalEntry.PostingDate = Order.PostingDate;
                journalEntry.DocumentDate = Order.DocumentDate;
                journalEntry.DueDate = Order.PostingDate;
                journalEntry.SSCID = Order.SysCurID;
                journalEntry.LLCID = Order.LocalCurID;
                journalEntry.CompanyID = Order.CompanyID;
                journalEntry.LocalSetRate = (decimal)Order.LocalSetRate;
                journalEntry.SeriesDID = seriesDetail.ID;
                journalEntry.Remarks = series.Name + " " + Order.Number_No;
                _context.Update(journalEntry);
                _context.SaveChanges();
            }

            decimal sumCost = 0;
            sumCost = (decimal)OrderDetails.Sum(s => s.Cost * s.Quantity);
            // expense account //
            var glAcc = _context.GLAccounts.FirstOrDefault(w => w.ID == Order.GLID);
            if (glAcc != null)
            {
                journalEntryDetail = new()
                {
                    new JournalEntryDetail
                    {
                        JEID = journalEntry.ID,
                        Type = Financials.Type.GLAcct,
                        ItemID = glAcc.ID,
                        Debit = sumCost,
                    }
                };
                //Insert 
                glAcc.Balance += sumCost;
                accountBalance = new()
                {
                    new AccountBalance
                    {
                        JEID = journalEntry.ID,
                        PostingDate = Order.PostingDate,
                        Origin = docType.ID,
                        OriginNo = Order.Number_No,
                        OffsetAccount = glAcc.Code,
                        Details = douTypeID.Name + "-" + glAcc.Code,
                        CumulativeBalance = glAcc.Balance,
                        Debit = sumCost,
                        LocalSetRate = (decimal)Order.LocalSetRate,
                        GLAID = glAcc.ID,
                        Effective=EffectiveBlance.Debit
                    }
                };
                //  
                _context.Update(glAcc);
            }
            _context.SaveChanges();
            foreach (var item in OrderDetails)
            {
                var itemMaster = _context.ItemMasterDatas.FirstOrDefault(w => w.ID == item.ItemID);
                var baseUOM = _context.GroupDUoMs.FirstOrDefault(w => w.UoMID == item.UomID);
                var warehouse = _context.WarehouseSummary.FirstOrDefault(w => w.ItemID == item.ItemID && w.WarehouseID == Order.WarehouseID);
                var warehouseDetail = _context.WarehouseDetails.FirstOrDefault(w => w.ID == item.LineID) ?? new WarehouseDetail();
                var gd = _context.GroupDUoMs.FirstOrDefault(W => W.GroupUoMID == itemMaster.GroupUomID && W.AltUOM == item.UomID);
                var _itemAcc = _context.ItemAccountings
                    .FirstOrDefault(i => i.ItemID == item.ItemID && i.WarehouseID == Order.WarehouseID);
                double @Qty = item.Quantity * gd.Factor;
                double @Cost = item.Cost / gd.Factor;
                var item_inventory_audit = new InventoryAudit();
                //var warehousedetail = new WarehouseDetail();
                // update itmemasterdata                    
                itemMaster.StockIn -= @Qty;
                //update warehouse                    
                warehouse.InStock -= @Qty;
                warehouseDetail.InStock -= @Qty;
                _context.WarehouseDetails.Update(warehouseDetail);
                _context.ItemMasterDatas.Update(itemMaster);
                _context.WarehouseSummary.Update(warehouse);
                _utility.UpdateItemAccounting(_itemAcc, warehouse);
                if (itemMaster.SetGlAccount == Inventory.SetGlAccount.ItemLevel)
                {
                    var itemAccs = _context.ItemAccountings.Where(w => w.ItemID == item.ItemID && w.WarehouseID == Order.WarehouseID).ToList();
                    var inventoryAcc = (from ia in itemAccs
                                        join gl in glAccs on ia.InventoryAccount equals gl.Code
                                        select gl
                                         ).FirstOrDefault();
                    inventoryAccID = inventoryAcc.ID;
                    inventoryAccAmount = (decimal)(item.Cost * item.Quantity);
                }
                else if (itemMaster.SetGlAccount == Inventory.SetGlAccount.ItemGroup)
                {
                    var itemAccs = _context.ItemAccountings.Where(w => w.ItemGroupID == itemMaster.ItemGroup1ID).ToList();
                    var inventoryAcc = (from ia in itemAccs
                                        join gl in glAccs on ia.InventoryAccount equals gl.Code
                                        select gl
                                         ).FirstOrDefault();
                    inventoryAccID = inventoryAcc.ID;
                    inventoryAccAmount = (decimal)(item.Quantity * item.Cost);
                }

                // expense account //
                var glAccExItem = _context.GLAccounts.FirstOrDefault(w => w.ID == item.GLID);
                decimal itemSum = (decimal)(item.Quantity * item.Cost);
                if (glAccExItem != null)
                {
                    var journalDetailEx = journalEntryDetail.FirstOrDefault(w => w.ItemID == item.GLID) ?? new JournalEntryDetail();
                    if (journalDetailEx.ItemID > 0)
                    {
                        glAccExItem.Balance += itemSum;
                        var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == item.GLID);
                        //journalEntryDetail
                        journalDetailEx.Debit += itemSum;
                        //accountBalance
                        accBalance.CumulativeBalance = glAccExItem.Balance;
                        accBalance.Debit += itemSum;
                    }
                    else
                    {
                        glAccExItem.Balance -= itemSum;
                        journalEntryDetail.Add(new JournalEntryDetail
                        {
                            JEID = journalEntry.ID,
                            Type = Financials.Type.GLAcct,
                            ItemID = glAccExItem.ID,
                            Debit = itemSum,
                        });
                        //
                        accountBalance.Add(new AccountBalance
                        {
                            JEID = journalEntry.ID,
                            PostingDate = Order.PostingDate,
                            Origin = docType.ID,
                            OriginNo = Order.Number_No,
                            OffsetAccount = glAccExItem.Code,
                            Details = douTypeID.Name + "-" + glAccExItem.Code,
                            CumulativeBalance = glAccExItem.Balance,
                            Debit = itemSum,
                            LocalSetRate = (decimal)Order.LocalSetRate,
                            GLAID = glAccExItem.ID,
                            Effective = EffectiveBlance.Debit

                        });
                    }
                    _context.Update(glAccExItem);
                }

                //inventoryAccID
                var glAccInven = _context.GLAccounts.FirstOrDefault(w => w.ID == inventoryAccID) ?? new GLAccount();
                var journalDetail = journalEntryDetail.FirstOrDefault(w => w.ItemID == inventoryAccID) ?? new JournalEntryDetail();
                if (glAccInven.ID > 0)
                {
                    if (journalDetail.ItemID > 0)
                    {
                        glAccInven.Balance -= inventoryAccAmount;
                        var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == inventoryAccID);
                        //journalEntryDetail
                        journalDetail.Credit += inventoryAccAmount;
                        //accountBalance
                        accBalance.CumulativeBalance = glAccInven.Balance;
                        accBalance.Credit += inventoryAccAmount;
                    }
                    else
                    {
                        glAccInven.Balance -= inventoryAccAmount;
                        journalEntryDetail.Add(new JournalEntryDetail
                        {
                            JEID = journalEntry.ID,
                            Type = Financials.Type.GLAcct,
                            ItemID = inventoryAccID,
                            Credit = inventoryAccAmount,
                        });
                        //
                        accountBalance.Add(new AccountBalance
                        {
                            JEID = journalEntry.ID,

                            PostingDate = Order.PostingDate,
                            Origin = docType.ID,
                            OriginNo = Order.Number_No,
                            OffsetAccount = glAcc == null ? glAccExItem.Code : glAcc.Code,
                            Details = douTypeID.Name + " - " + glAccInven.Code,
                            CumulativeBalance = glAccInven.Balance,
                            Credit = inventoryAccAmount,
                            LocalSetRate = (decimal)Order.LocalSetRate,
                            GLAID = inventoryAccID,
                            Effective = EffectiveBlance.Credit

                        });
                    }
                    _context.Update(glAccInven);
                }
                _context.SaveChanges();
                if (itemMaster.ManItemBy == ManageItemBy.SerialNumbers && itemMaster.ManMethod == ManagementMethod.OnEveryTransation)
                {
                    if (serials.Count > 0)
                    {
                        foreach (var s in serials)
                        {
                            if (s.SerialNumberSelected != null)
                            {
                                foreach (var ss in s.SerialNumberSelected.SerialNumberSelectedDetails)
                                {
                                    var waredetial = _context.WarehouseDetails.FirstOrDefault(i => ss.SerialNumber == i.SerialNumber && i.InStock > 0);
                                    decimal _inventoryAccAmount = 0M;
                                    decimal _COGSAccAmount = 0M;
                                    if (waredetial != null)
                                    {
                                        waredetial.InStock -= 1;
                                        // insert to warehouse detail
                                        var ware = new StockOut
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
                                            UserID = Order.UserID,
                                            ExpireDate = item.ExpireDate,
                                            TransType = TransTypeWD.GoodsIssue,
                                            TransID = Order.GoodIssuesID,
                                            FromWareDetialID = waredetial.ID,
                                            OutStockFrom = Order.GoodIssuesID,
                                        };
                                        _inventoryAccAmount = (decimal)waredetial.Cost;
                                        _COGSAccAmount = (decimal)waredetial.Cost;
                                        _context.StockOuts.Add(ware);
                                        _context.SaveChanges();
                                    }
                                }
                            }
                        }
                        // Insert to Inventory Audit
                        var inventory_audit = _context.InventoryAudits
                            .Where(w => w.ItemID == item.ItemID && w.WarehouseID == Order.WarehouseID);
                        var inventory = new InventoryAudit
                        {
                            ID = 0,
                            WarehouseID = Order.WarehouseID,
                            BranchID = Order.BranchID,
                            UserID = Order.UserID,
                            ItemID = item.ItemID,
                            CurrencyID = Order.LocalCurID,
                            UomID = baseUOM.BaseUOM,
                            InvoiceNo = Order.Number_No,
                            Trans_Type = docType.Code,
                            Process = itemMaster.Process,
                            SystemDate = DateTime.Now,
                            PostingDate = Order.PostingDate,
                            TimeIn = DateTime.Now.ToShortTimeString().ToString(),
                            Qty = @Qty * -1,
                            Cost = @Cost,
                            Price = 0,
                            CumulativeQty = inventory_audit.Sum(q => q.Qty) - @Qty,
                            CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) - (@Qty * @Cost),
                            Trans_Valuse = @Qty * @Cost * -1,
                            ExpireDate = item.ExpireDate,
                            LocalCurID = Order.LocalCurID,
                            LocalSetRate = Order.LocalSetRate,
                            CompanyID = Order.CompanyID,
                            DocumentTypeID = docType.ID,
                            SeriesID = Order.SeriseID,
                            SeriesDetailID = Order.SeriseDID,
                        };
                        _utility.CumulativeValue(inventory.WarehouseID, inventory.ItemID, inventory.CumulativeValue, _itemAcc);
                        _context.InventoryAudits.Add(inventory);
                        _context.SaveChanges();
                    }
                }
                else if (itemMaster.ManItemBy == ManageItemBy.Batches && itemMaster.ManMethod == ManagementMethod.OnEveryTransation)
                {
                    if (batches.Count > 0)
                    {
                        foreach (var b in batches)
                        {
                            if (b.BatchNoSelected != null)
                            {
                                foreach (var sb in b.BatchNoSelected.BatchNoSelectedDetails)
                                {
                                    decimal selectedQty = sb.SelectedQty * (decimal)baseUOM.Factor;
                                    var waredetial = _context.WarehouseDetails.FirstOrDefault(i => sb.BatchNo == i.BatchNo && i.InStock > 0);
                                    decimal _inventoryAccAmount = 0M;
                                    decimal _COGSAccAmount = 0M;
                                    if (waredetial != null)
                                    {
                                        waredetial.InStock -= (double)selectedQty;

                                        // insert to waredetial
                                        var ware = new StockOut
                                        {
                                            AdmissionDate = waredetial.AdmissionDate,
                                            Cost = (decimal)waredetial.Cost,
                                            CurrencyID = waredetial.CurrencyID,
                                            Details = waredetial.Details,
                                            ID = 0,
                                            InStock = selectedQty * -1,
                                            ItemID = item.ItemID,
                                            Location = waredetial.Location,
                                            MfrDate = waredetial.MfrDate,
                                            ProcessItem = ProcessItem.SEBA,
                                            SyetemDate = DateTime.Now,
                                            SysNum = 0,
                                            TimeIn = DateTime.Now,
                                            WarehouseID = waredetial.WarehouseID,
                                            UomID = item.UomID,
                                            UserID = Order.UserID,
                                            ExpireDate = item.ExpireDate,
                                            BatchAttr1 = waredetial.BatchAttr1,
                                            BatchAttr2 = waredetial.BatchAttr2,
                                            BatchNo = waredetial.BatchNo,
                                            TransType = TransTypeWD.GoodsIssue,
                                            TransID = Order.GoodIssuesID,
                                            FromWareDetialID = waredetial.ID,
                                            OutStockFrom = Order.GoodIssuesID,
                                        };

                                        _inventoryAccAmount = (decimal)waredetial.Cost * selectedQty;
                                        _COGSAccAmount = (decimal)waredetial.Cost * selectedQty;
                                        _context.StockOuts.Add(ware);
                                        _context.SaveChanges();
                                    }
                                }
                            }
                        }
                        // insert to inventory audit
                        var inventory_audit = _context.InventoryAudits
                            .Where(w => w.ItemID == item.ItemID && w.WarehouseID == Order.WarehouseID);
                        var inventory = new InventoryAudit
                        {
                            ID = 0,
                            WarehouseID = Order.WarehouseID,
                            BranchID = Order.BranchID,
                            UserID = Order.UserID,
                            ItemID = item.ItemID,
                            CurrencyID = Order.LocalCurID,
                            UomID = baseUOM.BaseUOM,
                            InvoiceNo = Order.Number_No,
                            Trans_Type = docType.Code,
                            Process = itemMaster.Process,
                            SystemDate = DateTime.Now,
                            PostingDate = Order.PostingDate,
                            TimeIn = DateTime.Now.ToShortTimeString().ToString(),
                            Qty = @Qty * -1,
                            Cost = @Cost,
                            Price = 0,
                            CumulativeQty = inventory_audit.Sum(q => q.Qty) - @Qty,
                            CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) - (@Qty * @Cost),
                            Trans_Valuse = @Qty * @Cost * -1,
                            ExpireDate = item.ExpireDate,
                            LocalCurID = Order.LocalCurID,
                            LocalSetRate = Order.LocalSetRate,
                            CompanyID = Order.CompanyID,
                            DocumentTypeID = docType.ID,
                            SeriesID = Order.SeriseID,
                            SeriesDetailID = Order.SeriseDID,
                        };
                        _utility.CumulativeValue(inventory.WarehouseID, inventory.ItemID, inventory.CumulativeValue, _itemAcc);
                        _context.InventoryAudits.Add(inventory);
                    }
                }
                else
                {
                    if (itemMaster.Process == "FIFO")
                    {

                        var stockOuts = new StockOut
                        {
                            Cost = (decimal)warehouseDetail.Cost,
                            CurrencyID = warehouseDetail.CurrencyID,
                            ID = 0,
                            InStock = (decimal)@Qty,
                            ItemID = item.ItemID,
                            ProcessItem = ProcessItem.FIFO,
                            SyetemDate = DateTime.Now,
                            TimeIn = DateTime.Now,
                            WarehouseID = warehouseDetail.WarehouseID,
                            UomID = item.UomID,
                            UserID = Order.UserID,
                            ExpireDate = item.ExpireDate,
                            TransType = TransTypeWD.PurCreditMemo,
                            FromWareDetialID = warehouseDetail.ID,
                            OutStockFrom = Order.GoodIssuesID,
                        };
                        _context.StockOuts.Add(stockOuts);

                        //insert inventoryaudit
                        var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item.ItemID && w.WarehouseID == Order.WarehouseID);
                        var pri_detial = _context.PriceListDetails.Where(w => w.ItemID == item.ItemID);
                        item_inventory_audit.ID = 0;
                        item_inventory_audit.WarehouseID = Order.WarehouseID;
                        item_inventory_audit.BranchID = Order.BranchID;
                        item_inventory_audit.UserID = Order.UserID;
                        item_inventory_audit.ItemID = item.ItemID;
                        item_inventory_audit.CurrencyID = Order.SysCurID;
                        item_inventory_audit.UomID = (int)itemMaster.InventoryUoMID;
                        item_inventory_audit.InvoiceNo = Order.Number_No;
                        item_inventory_audit.Trans_Type = docType.Code;
                        item_inventory_audit.Process = itemMaster.Process;
                        item_inventory_audit.SystemDate = DateTime.Now;
                        item_inventory_audit.PostingDate = Order.PostingDate;
                        item_inventory_audit.TimeIn = DateTime.Now.ToShortTimeString();
                        item_inventory_audit.Qty = @Qty * -1;
                        item_inventory_audit.Cost = @Cost;
                        item_inventory_audit.Price = 0;
                        item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) - @Qty;
                        item_inventory_audit.CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) - (@Qty * @Cost);
                        item_inventory_audit.Trans_Valuse = @Qty * @Cost * -1;
                        item_inventory_audit.ExpireDate = item.ExpireDate;
                        item_inventory_audit.LocalCurID = Order.LocalCurID;
                        item_inventory_audit.LocalSetRate = Order.LocalSetRate;
                        item_inventory_audit.SeriesDetailID = Order.SeriseDID;
                        item_inventory_audit.SeriesID = Order.SeriseID;
                        item_inventory_audit.DocumentTypeID = Order.DocTypeID;
                        item_inventory_audit.CompanyID = Order.CompanyID;
                        _utility.CumulativeValue(item_inventory_audit.WarehouseID, item_inventory_audit.ItemID, item_inventory_audit.CumulativeValue, _itemAcc);
                    }
                    else
                    {
                        //insert inventoryaudit
                        var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item.ItemID && w.WarehouseID == Order.WarehouseID);
                        var warehouse_sammary = _context.WarehouseSummary.FirstOrDefault(w => w.ItemID == item.ItemID && w.WarehouseID == Order.WarehouseID);
                        var pri_detial = _context.PriceListDetails.Where(w => w.ItemID == item.ItemID);
                        InventoryAudit inventoryAudit = new() { Qty = @Qty, Cost = @Cost };
                        double @AvgCost = _utility.CalAVGCost(item.ItemID, Order.WarehouseID, inventoryAudit);
                        var stockOuts = new StockOut
                        {
                            Cost = (decimal)@AvgCost,
                            CurrencyID = warehouseDetail.CurrencyID,
                            ID = 0,
                            InStock = (decimal)@Qty,
                            ItemID = item.ItemID,
                            ProcessItem = ProcessItem.Average,
                            SyetemDate = DateTime.Now,
                            TimeIn = DateTime.Now,
                            WarehouseID = warehouseDetail.WarehouseID,
                            UomID = item.UomID,
                            UserID = Order.UserID,
                            ExpireDate = item.ExpireDate,
                            TransType = TransTypeWD.PurCreditMemo,
                            FromWareDetialID = warehouseDetail.ID,
                            OutStockFrom = Order.GoodIssuesID,
                        };
                        _context.StockOuts.Add(stockOuts);

                        item_inventory_audit.ID = 0;
                        item_inventory_audit.WarehouseID = Order.WarehouseID;
                        item_inventory_audit.BranchID = Order.BranchID;
                        item_inventory_audit.UserID = Order.UserID;
                        item_inventory_audit.ItemID = item.ItemID;
                        item_inventory_audit.CurrencyID = Order.SysCurID;
                        item_inventory_audit.UomID = (int)itemMaster.InventoryUoMID;
                        item_inventory_audit.InvoiceNo = Order.Number_No;
                        item_inventory_audit.Trans_Type = docType.Code;
                        item_inventory_audit.Process = itemMaster.Process;
                        item_inventory_audit.SystemDate = DateTime.Now;
                        item_inventory_audit.PostingDate = Order.PostingDate;
                        item_inventory_audit.TimeIn = DateTime.Now.ToShortTimeString();
                        item_inventory_audit.Qty = @Qty * -1;
                        item_inventory_audit.Cost = @AvgCost;
                        item_inventory_audit.Price = 0;
                        item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) - @Qty;
                        item_inventory_audit.CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) - (Qty * Cost);
                        item_inventory_audit.Trans_Valuse = @Qty * @Cost * -1;
                        item_inventory_audit.ExpireDate = item.ExpireDate;
                        item_inventory_audit.LocalCurID = Order.LocalCurID;
                        item_inventory_audit.LocalSetRate = Order.LocalSetRate;
                        item_inventory_audit.SeriesDetailID = Order.SeriseDID;
                        item_inventory_audit.SeriesID = Order.SeriseID;
                        item_inventory_audit.DocumentTypeID = Order.DocTypeID;
                        item_inventory_audit.CompanyID = Order.CompanyID;
                        _utility.UpdateAvgCost(item.ItemID, Order.WarehouseID, itemMaster.GroupUomID, item_inventory_audit.Qty, item_inventory_audit.Cost);
                        _utility.CumulativeValue(item_inventory_audit.WarehouseID, item_inventory_audit.ItemID, item_inventory_audit.CumulativeValue, _itemAcc);
                    }
                    _context.InventoryAudits.Add(item_inventory_audit);
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
        #endregion IssuesStock
        public List<GoodsIssueViewModel> GetAllPurItemsByItemID(int wareid, int itemId, int comId)
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
            List<GoodsIssueViewModel> items = new();
            var allPurItems = (from wd in _context.WarehouseDetails.Where(i => i.WarehouseID == wareid && i.ItemID == itemId && i.InStock != 0)
                               join item in _context.ItemMasterDatas.Where(i => !i.Delete) on wd.ItemID equals item.ID
                               let uom = _context.UnitofMeasures.FirstOrDefault(i => i.ID == item.SaleUomID) ?? new UnitofMeasure()
                               let cur = _context.Currency.FirstOrDefault(i => i.ID == wd.CurrencyID) ?? new Currency()
                               let ws = _context.WarehouseSummary.Where(i => i.WarehouseID == wd.WarehouseID && i.ItemID == item.ID)
                               select new GoodsIssueViewModel
                               {
                                   GoodIssuesDetailID = 0,
                                   GoodIssuesID = 0,
                                   CurrencyID = wd.CurrencyID,
                                   LineID = wd.ID,
                                   ItemID = wd.ItemID,
                                   UomID = Convert.ToInt32(item.InventoryUoMID),
                                   Code = item.Code,
                                   KhmerName = item.KhmerName,
                                   EnglishName = item.EnglishName,
                                   Quantity = 1,
                                   QuantitySum = wd.InStock,
                                   Cost = wd.Cost,
                                   CostStore = wd.Cost,
                                   Currency = cur.Description,
                                   UomName = uom.Name,
                                   BarCode = item.Barcode,
                                   Type = item.Process,
                                   ManageExpire = item.ManageExpire,
                                   ExpireDate = wd.ExpireDate,
                                   UoMsList = uoms.ToList(),
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
                                       Selected = Convert.ToInt32(item.InventoryUoMID) == c.ID
                                   }).ToList(),
                                   PaymentMeans = "",
                               }
                               ).ToList();
            var itemGroup = allPurItems.Where(i => i.Type == "FIFO" || i.Type == "SEBA").GroupBy(i => i.Cost).ToList();
            if (itemGroup.Count > 0)
            {
                foreach (var i in itemGroup)
                {
                    var qty = i.Sum(q => q.QuantitySum);
                    foreach (var item in itemGroup.Select(x => x.FirstOrDefault()).ToList())
                    {
                        if (i.Key == item.Cost)
                        {
                            item.InStock = qty;
                            item.QuantitySum = qty;
                            items.Add(item);
                        }
                    };

                };
            }
            var itemAvs = allPurItems.Where(i => i.Type == "Average").ToList();
            if (itemAvs.Count > 0)
            {
                double sumQtyAvg = itemAvs.Sum(i => i.QuantitySum);
                var itemAv = itemAvs.FirstOrDefault();
                itemAv.InStock = sumQtyAvg;
                items.Add(itemAv);
            }
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
