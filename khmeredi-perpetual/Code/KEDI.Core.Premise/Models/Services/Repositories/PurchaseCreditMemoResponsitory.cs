using Microsoft.EntityFrameworkCore;
using CKBS.AppContext;
using CKBS.Models.ReportClass;
using CKBS.Models.Services.Banking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CKBS.Models.Services.Financials;
using CKBS.Models.Services.Administrator.SystemInitialization;
using CKBS.Models.Services.ChartOfAccounts;
using CKBS.Models.Services.Inventory;
using CKBS.Models.Services.Administrator.Inventory;
using Newtonsoft.Json;
using KEDI.Core.Premise.Models.Services.Purchase;
using CKBS.Models.Services.Administrator.Setup;
using KEDI.Core.Premise.Models.Services.Banking;
using KEDI.Core.Premise.Models.ServicesClass.SerialBatches.Purchases;
using KEDI.Core.Premise.Models.Services.Administrator.Inventory;
using KEDI.Core.Premise.Repository;

namespace CKBS.Models.Services.Responsitory
{
    public interface IPurchaseCreditMemo
    {
        void GoodIssuesStock(int PurchaseMemoID, string Type, List<APCSerialNumber> serials, List<APCBatchNo> batches, FreightPurchase freight);
        void GoodIssuesStockBasic(int PurchaseMemoID, string Type, List<APCSerialNumber> serials, List<APCBatchNo> batches, FreightPurchase freight);

        void GoodIssuesStockAPReserve(int copynote, int PurchaseMemoID, string Type, List<APCSerialNumber> serials, List<APCBatchNo> batches, FreightPurchase freight);
        IEnumerable<ReportPurchasCreditMemo> ReportPurchaseCreditMemo(int BranchID, int WarehoseID, string PostingDate, string DocumentDate, string DeliveryDate, string Check);
    }
    public class PurchaseCreditMemoResponsitory : IPurchaseCreditMemo
    {
        private readonly DataContext _context;
        private readonly UtilityModule _utility;
        public PurchaseCreditMemoResponsitory(DataContext context, UtilityModule utility)
        {
            _context = context;
            _utility = utility;
        }
        #region  GoodIssuesStockBasic

        public void GoodIssuesStockBasic(int purchaseMemoID, string Type, List<APCSerialNumber> serials, List<APCBatchNo> batches, FreightPurchase freight)
        {
            var purchaseMemo = _context.PurchaseCreditMemos.Find(purchaseMemoID);
            var memomaster = _context.PurchaseCreditMemos.FirstOrDefault(w => w.PurchaseMemoID == purchaseMemoID);
            var memodetail = _context.PurchaseCreditMemoDetails.Where(w => w.PurchaseCreditMemoID == purchaseMemoID).ToList();
            var docType = _context.DocumentTypes.Find(purchaseMemo.DocumentTypeID);
            var series = _context.Series.Find(purchaseMemo.SeriesID);
            JournalEntry journalEntry = new();
            SeriesDetail seriesDetail = new();
            //IssuseInstock
            foreach (var item in memodetail)
            {
                var itemMaster = _context.ItemMasterDatas.FirstOrDefault(w => w.ID == item.ItemID);
                ItemAccounting _itemAcc = _context.ItemAccountings.FirstOrDefault(i => i.ItemID == item.ItemID && purchaseMemo.WarehouseID == i.WarehouseID);

                var orft = _context.GroupDUoMs.FirstOrDefault(w => w.GroupUoMID == itemMaster.GroupUomID && w.AltUOM == item.UomID);
                var itemWareSum = _context.WarehouseSummary.FirstOrDefault(w => w.WarehouseID == purchaseMemo.WarehouseID && w.ItemID == item.ItemID);
                var Cost = item.PurchasPrice * purchaseMemo.PurRate;
                var waredetials = _context.WarehouseDetails.Where(w => w.WarehouseID == purchaseMemo.WarehouseID && w.ItemID == item.ItemID).ToList();
                if (itemWareSum != null)
                {
                    //WerehouseSummary
                    itemWareSum.InStock -= (double)item.Qty;
                    //Itemmasterdata
                    itemMaster.StockIn = itemWareSum.InStock - (double)item.Qty;
                    _context.WarehouseSummary.Update(itemWareSum);
                    _context.ItemMasterDatas.Update(itemMaster);
                    _utility.UpdateItemAccounting(_itemAcc, itemWareSum);
                }
                double @Check_Stock;
                double @Remain;
                double @IssusQty;
                double @FIFOQty;
                double @Qty = item.Qty * orft.Factor;
                if (itemMaster.ManItemBy == ManageItemBy.SerialNumbers
                    && itemMaster.ManMethod == ManagementMethod.OnEveryTransation)
                {
                    if (serials.Count > 0)
                    {
                        List<InventoryAudit> invens = new();
                        List<StockOut> stockOuts = new();
                        foreach (var s in serials)
                        {
                            if (s.APCSNSelected != null)
                            {
                                foreach (var ss in s.APCSNSelected.APCSNDSelectedDetails)
                                {
                                    var waredetial = waredetials
                                        .FirstOrDefault(i => ss.SerialNumber == i.SerialNumber && i.InStock > 0 && i.Cost == item.PurchasPrice * purchaseMemo.PurRate);
                                    if (waredetial != null)
                                    {
                                        waredetial.InStock -= 1;
                                        // Insert to warehouse detial //
                                        stockOuts.Add(new StockOut
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
                                            UserID = purchaseMemo.UserID,
                                            ExpireDate = item.ExpireDate,
                                            TransType = TransTypeWD.PurCreditMemo,
                                            TransID = purchaseMemo.PurchaseMemoID,
                                            OutStockFrom = purchaseMemo.PurchaseMemoID,
                                            FromWareDetialID = waredetial.ID,
                                        });
                                    }
                                }
                            }
                        }
                        // Insert to InventoryAudit //
                        var inventory_audit = _context.InventoryAudits
                            .Where(w => w.ItemID == item.ItemID && w.WarehouseID == purchaseMemo.WarehouseID);
                        var inven = new InventoryAudit
                        {
                            ID = 0,
                            WarehouseID = purchaseMemo.WarehouseID,
                            BranchID = purchaseMemo.BranchID,
                            UserID = purchaseMemo.UserID,
                            ItemID = item.ItemID,
                            CurrencyID = purchaseMemo.CompanyID,
                            UomID = orft.BaseUOM,
                            InvoiceNo = purchaseMemo.Number,
                            Trans_Type = docType.Code,
                            Process = itemMaster.Process,
                            SystemDate = DateTime.Now,
                            PostingDate = purchaseMemo.PostingDate,
                            TimeIn = DateTime.Now.ToShortTimeString().ToString(),
                            Qty = @Qty * -1,
                            Cost = Cost,
                            Price = 0,
                            CumulativeQty = (inventory_audit.Sum(q => q.Qty) - @Qty),
                            CumulativeValue = (inventory_audit.Sum(s => s.Trans_Valuse)) - (@Qty * Cost),
                            Trans_Valuse = @Qty * Cost * -1,
                            ExpireDate = item.ExpireDate,
                            LocalCurID = purchaseMemo.LocalCurID,
                            LocalSetRate = purchaseMemo.LocalSetRate,
                            CompanyID = purchaseMemo.CompanyID,
                            DocumentTypeID = docType.ID,
                            SeriesID = purchaseMemo.SeriesID,
                            SeriesDetailID = purchaseMemo.SeriesDetailID,
                        };
                        _context.InventoryAudits.Add(inven);
                        _context.StockOuts.AddRange(stockOuts);
                        _context.SaveChanges();
                    }
                }
                else if (itemMaster.ManItemBy == ManageItemBy.Batches
                    && itemMaster.ManMethod == ManagementMethod.OnEveryTransation)
                {
                    if (batches.Count > 0)
                    {
                        foreach (var b in batches)
                        {
                            if (b.APCBatchNoSelected != null)
                            {
                                foreach (var sb in b.APCBatchNoSelected.APCBatchNoSelectedDetails)
                                {
                                    var waredetial = waredetials
                                        .FirstOrDefault(i =>
                                            sb.BatchNo == i.BatchNo
                                            && i.InStock > 0
                                            && i.Cost == item.PurchasPrice * purchaseMemo.PurRate);
                                    if (waredetial != null)
                                    {
                                        waredetial.InStock -= (double)sb.SelectedQty;
                                        _context.SaveChanges();
                                        // insert to waredetial
                                        var stockOuts = new StockOut
                                        {
                                            AdmissionDate = waredetial.AdmissionDate,
                                            Cost = (decimal)waredetial.Cost,
                                            CurrencyID = waredetial.CurrencyID,
                                            Details = waredetial.Details,
                                            ID = 0,
                                            InStock = sb.SelectedQty,
                                            ItemID = item.ItemID,
                                            Location = waredetial.Location,
                                            MfrDate = waredetial.MfrDate,
                                            ProcessItem = ProcessItem.SEBA,
                                            SyetemDate = DateTime.Now,
                                            SysNum = 0,
                                            TimeIn = DateTime.Now,
                                            WarehouseID = waredetial.WarehouseID,
                                            UomID = item.UomID,
                                            UserID = purchaseMemo.UserID,
                                            ExpireDate = item.ExpireDate,
                                            BatchAttr1 = waredetial.BatchAttr1,
                                            BatchAttr2 = waredetial.BatchAttr2,
                                            BatchNo = waredetial.BatchNo,
                                            TransType = TransTypeWD.PurCreditMemo,
                                            TransID = purchaseMemo.PurchaseMemoID,
                                            FromWareDetialID = waredetial.ID,
                                            OutStockFrom = purchaseMemo.PurCurrencyID,
                                        };
                                        _context.StockOuts.Add(stockOuts);
                                        _context.SaveChanges();
                                    }
                                }
                            }
                        }
                        // insert to inventory audit
                        var inventory_audit = _context.InventoryAudits
                            .Where(w => w.ItemID == item.ItemID && w.WarehouseID == purchaseMemo.WarehouseID);
                        var invens = new InventoryAudit
                        {
                            ID = 0,
                            WarehouseID = purchaseMemo.WarehouseID,
                            BranchID = purchaseMemo.BranchID,
                            UserID = purchaseMemo.UserID,
                            ItemID = item.ItemID,
                            CurrencyID = purchaseMemo.CompanyID,
                            UomID = orft.BaseUOM,
                            InvoiceNo = purchaseMemo.Number,
                            Trans_Type = docType.Code,
                            Process = itemMaster.Process,
                            SystemDate = DateTime.Now,
                            PostingDate = purchaseMemo.PostingDate,
                            TimeIn = DateTime.Now.ToShortTimeString().ToString(),
                            Qty = @Qty * -1,
                            Cost = Cost,
                            Price = 0,
                            CumulativeQty = inventory_audit.Sum(q => q.Qty) - @Qty,
                            CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) - (@Qty * Cost),
                            Trans_Valuse = @Qty * Cost * -1,
                            ExpireDate = item.ExpireDate,
                            LocalCurID = purchaseMemo.LocalCurID,
                            LocalSetRate = purchaseMemo.LocalSetRate,
                            CompanyID = purchaseMemo.CompanyID,
                            DocumentTypeID = docType.ID,
                            SeriesID = purchaseMemo.SeriesID,
                            SeriesDetailID = purchaseMemo.SeriesDetailID,
                        };
                        _context.InventoryAudits.Add(invens);
                        _context.SaveChanges();
                    }
                }
                else
                {
                    foreach (var item_warehouse in waredetials.Where(w => w.InStock > 0))
                    {
                        var item_inventory_audit = new InventoryAudit();
                        var item_IssusStock = new WarehouseDetail();
                        var itmewhd = waredetials.FirstOrDefault(w => w.InStock >= @Qty && w.Cost == Cost) ?? new WarehouseDetail();
                        if (itmewhd.ID > 0)
                        {
                            item_IssusStock = itmewhd;
                        }
                        else
                        {
                            item_IssusStock = waredetials.FirstOrDefault(w => w.InStock > 0);
                        }
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
                                        UserID = purchaseMemo.UserID,
                                        ExpireDate = item.ExpireDate,
                                        TransType = TransTypeWD.PurCreditMemo,
                                    };
                                    _context.StockOuts.Add(stockOuts);
                                    var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item.ItemID && w.WarehouseID == purchaseMemo.WarehouseID);
                                    item_inventory_audit.ID = 0;
                                    item_inventory_audit.WarehouseID = purchaseMemo.WarehouseID;
                                    item_inventory_audit.BranchID = purchaseMemo.BranchID;
                                    item_inventory_audit.UserID = purchaseMemo.UserID;
                                    item_inventory_audit.ItemID = item.ItemID;
                                    item_inventory_audit.CurrencyID = purchaseMemo.CompanyID;
                                    item_inventory_audit.UomID = orft.BaseUOM;
                                    item_inventory_audit.InvoiceNo = purchaseMemo.Number;
                                    item_inventory_audit.Trans_Type = docType.Code;
                                    item_inventory_audit.Process = itemMaster.Process;
                                    item_inventory_audit.SystemDate = DateTime.Now;
                                    item_inventory_audit.PostingDate = purchaseMemo.PostingDate;
                                    item_inventory_audit.TimeIn = DateTime.Now.ToShortTimeString().ToString();
                                    item_inventory_audit.Qty = @IssusQty * -1;
                                    item_inventory_audit.Cost = item_IssusStock.Cost;
                                    item_inventory_audit.Price = 0;
                                    item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) - @IssusQty;
                                    item_inventory_audit.CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) - (@IssusQty * item_IssusStock.Cost);
                                    item_inventory_audit.Trans_Valuse = @IssusQty * item_IssusStock.Cost * -1;
                                    item_inventory_audit.ExpireDate = item_IssusStock.ExpireDate;
                                    item_inventory_audit.LocalCurID = purchaseMemo.LocalCurID;
                                    item_inventory_audit.LocalSetRate = purchaseMemo.LocalSetRate;
                                    item_inventory_audit.CompanyID = purchaseMemo.CompanyID;
                                    item_inventory_audit.DocumentTypeID = docType.ID;
                                    item_inventory_audit.SeriesID = purchaseMemo.SeriesID;
                                    item_inventory_audit.SeriesDetailID = purchaseMemo.SeriesDetailID;
                                }
                                _utility.CumulativeValue(item_inventory_audit.WarehouseID, item_inventory_audit.ItemID, item_inventory_audit.CumulativeValue, _itemAcc);
                                _context.WarehouseDetails.Update(item_IssusStock);
                                _context.InventoryAudits.Add(item_inventory_audit);
                                _context.SaveChanges();
                            }
                            else if (itemMaster.Process == "Average")
                            {
                                item_IssusStock.InStock = item_IssusStock.InStock -= @IssusQty;
                                if (@IssusQty > 0)
                                {
                                    var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item.ItemID && w.WarehouseID == purchaseMemo.WarehouseID);
                                    var avgInven = new InventoryAudit { Qty = @IssusQty, Cost = Cost };
                                    double @sysAvCost = _utility.CalAVGCost(item.ItemID, item_warehouse.ID, avgInven, false);
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
                                        UserID = purchaseMemo.UserID,
                                        ExpireDate = item.ExpireDate,
                                        TransType = TransTypeWD.PurCreditMemo,
                                    };
                                    _context.StockOuts.Add(stockOuts);

                                    item_inventory_audit.ID = 0;
                                    item_inventory_audit.WarehouseID = purchaseMemo.WarehouseID;
                                    item_inventory_audit.BranchID = purchaseMemo.BranchID;
                                    item_inventory_audit.UserID = purchaseMemo.UserID;
                                    item_inventory_audit.ItemID = item.ItemID;
                                    item_inventory_audit.CurrencyID = purchaseMemo.CompanyID;
                                    item_inventory_audit.UomID = orft.BaseUOM;
                                    item_inventory_audit.InvoiceNo = purchaseMemo.Number;
                                    item_inventory_audit.Trans_Type = docType.Code;
                                    item_inventory_audit.Process = itemMaster.Process;
                                    item_inventory_audit.SystemDate = DateTime.Now;
                                    item_inventory_audit.PostingDate = purchaseMemo.PostingDate;
                                    item_inventory_audit.TimeIn = DateTime.Now.ToShortTimeString().ToString();
                                    item_inventory_audit.Qty = @IssusQty * -1;
                                    item_inventory_audit.Cost = @sysAvCost;
                                    item_inventory_audit.Price = 0;
                                    item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) - @IssusQty;
                                    item_inventory_audit.CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) - (@IssusQty * @sysAvCost);
                                    item_inventory_audit.Trans_Valuse = @IssusQty * @sysAvCost * (-1);
                                    item_inventory_audit.ExpireDate = item_IssusStock.ExpireDate;
                                    item_inventory_audit.LocalCurID = purchaseMemo.LocalCurID;
                                    item_inventory_audit.LocalSetRate = purchaseMemo.LocalSetRate;
                                    item_inventory_audit.CompanyID = purchaseMemo.CompanyID;
                                    item_inventory_audit.DocumentTypeID = docType.ID;
                                    item_inventory_audit.SeriesID = purchaseMemo.SeriesID;
                                    item_inventory_audit.SeriesDetailID = purchaseMemo.SeriesDetailID;
                                }
                                var inventoryAcct = _context.InventoryAudits.Where(w => w.ItemID == item.ItemID);
                                double @AvgCost = ((inventoryAcct.Sum(s => s.Trans_Valuse) + item_inventory_audit.Cost) / (inventoryAcct.Sum(q => q.Qty) + item_inventory_audit.Qty));
                                @AvgCost = _utility.CheckNaNOrInfinity(@AvgCost);
                                _utility.UpdateAvgCost(item_warehouse.ItemID, purchaseMemo.WarehouseID, itemMaster.GroupUomID, item_inventory_audit.Qty, item_inventory_audit.Cost);
                                _utility.UpdateBomCost(item_warehouse.ItemID, item_inventory_audit.Qty, item_inventory_audit.Cost);
                                _utility.CumulativeValue(item_inventory_audit.WarehouseID, item_inventory_audit.ItemID, item_inventory_audit.CumulativeValue, _itemAcc);
                                _context.WarehouseDetails.Update(item_IssusStock);
                                _context.InventoryAudits.Add(item_inventory_audit);
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
                                        UserID = purchaseMemo.UserID,
                                        ExpireDate = item.ExpireDate,
                                        TransType = TransTypeWD.PurCreditMemo,
                                    };
                                    _context.StockOuts.Add(stockOuts);

                                    var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item.ItemID && w.WarehouseID == purchaseMemo.WarehouseID);
                                    item_inventory_audit.ID = 0;
                                    item_inventory_audit.WarehouseID = purchaseMemo.WarehouseID;
                                    item_inventory_audit.BranchID = purchaseMemo.BranchID;
                                    item_inventory_audit.UserID = purchaseMemo.UserID;
                                    item_inventory_audit.ItemID = item.ItemID;
                                    item_inventory_audit.CurrencyID = purchaseMemo.CompanyID;
                                    item_inventory_audit.UomID = orft.BaseUOM;
                                    item_inventory_audit.InvoiceNo = purchaseMemo.Number;
                                    item_inventory_audit.Trans_Type = docType.Code;
                                    item_inventory_audit.Process = itemMaster.Process;
                                    item_inventory_audit.SystemDate = DateTime.Now;
                                    item_inventory_audit.PostingDate = purchaseMemo.PostingDate;
                                    item_inventory_audit.TimeIn = DateTime.Now.ToShortTimeString().ToString();
                                    item_inventory_audit.Qty = @IssusQty * -1;
                                    item_inventory_audit.Cost = item_IssusStock.Cost;
                                    item_inventory_audit.Price = 0;
                                    item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) - @IssusQty;
                                    item_inventory_audit.CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) - (@IssusQty * item_IssusStock.Cost);
                                    item_inventory_audit.Trans_Valuse = @IssusQty * item_IssusStock.Cost * (-1);
                                    item_inventory_audit.ExpireDate = item_IssusStock.ExpireDate;
                                    item_inventory_audit.LocalCurID = purchaseMemo.LocalCurID;
                                    item_inventory_audit.LocalSetRate = purchaseMemo.LocalSetRate;
                                    item_inventory_audit.CompanyID = purchaseMemo.CompanyID;
                                    item_inventory_audit.DocumentTypeID = docType.ID;
                                    item_inventory_audit.SeriesID = purchaseMemo.SeriesID;
                                    item_inventory_audit.SeriesDetailID = purchaseMemo.SeriesDetailID;
                                }
                                _utility.CumulativeValue(item_inventory_audit.WarehouseID, item_inventory_audit.ItemID, item_inventory_audit.CumulativeValue, _itemAcc);
                                _context.WarehouseDetails.Update(item_IssusStock);
                                _context.InventoryAudits.Add(item_inventory_audit);
                                _context.SaveChanges();
                            }
                            else if (itemMaster.Process == "Average")
                            {
                                item_IssusStock.InStock = @FIFOQty;
                                if (@IssusQty > 0)
                                {
                                    var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item.ItemID && w.WarehouseID == purchaseMemo.WarehouseID);
                                    var avgInven = new InventoryAudit { Qty = @IssusQty, Cost = Cost };
                                    double @sysAvCost = _utility.CalAVGCost(item.ItemID, item_warehouse.ID, avgInven, false);
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
                                        UserID = purchaseMemo.UserID,
                                        ExpireDate = item.ExpireDate,
                                        TransType = TransTypeWD.PurCreditMemo,
                                    };
                                    _context.StockOuts.Add(stockOuts);
                                    item_inventory_audit.ID = 0;
                                    item_inventory_audit.WarehouseID = purchaseMemo.WarehouseID;
                                    item_inventory_audit.BranchID = purchaseMemo.BranchID;
                                    item_inventory_audit.UserID = purchaseMemo.UserID;
                                    item_inventory_audit.ItemID = item.ItemID;
                                    item_inventory_audit.CurrencyID = purchaseMemo.CompanyID;
                                    item_inventory_audit.UomID = orft.BaseUOM;
                                    item_inventory_audit.InvoiceNo = purchaseMemo.Number;
                                    item_inventory_audit.Trans_Type = docType.Code;
                                    item_inventory_audit.Process = itemMaster.Process;
                                    item_inventory_audit.SystemDate = DateTime.Now;
                                    item_inventory_audit.PostingDate = purchaseMemo.PostingDate;
                                    item_inventory_audit.TimeIn = DateTime.Now.ToShortTimeString().ToString();
                                    item_inventory_audit.Qty = @IssusQty * -1;
                                    item_inventory_audit.Cost = @sysAvCost;
                                    item_inventory_audit.Price = 0;
                                    item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) - @IssusQty;
                                    item_inventory_audit.CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) - (@IssusQty * @sysAvCost);
                                    item_inventory_audit.Trans_Valuse = @IssusQty * @sysAvCost * -1;
                                    item_inventory_audit.ExpireDate = item_IssusStock.ExpireDate;
                                    item_inventory_audit.LocalCurID = purchaseMemo.LocalCurID;
                                    item_inventory_audit.LocalSetRate = purchaseMemo.LocalSetRate;
                                    item_inventory_audit.CompanyID = purchaseMemo.CompanyID;
                                    item_inventory_audit.DocumentTypeID = docType.ID;
                                    item_inventory_audit.SeriesID = purchaseMemo.SeriesID;
                                    item_inventory_audit.SeriesDetailID = purchaseMemo.SeriesDetailID;
                                }
                                var inventoryAcct = _context.InventoryAudits.Where(w => w.ItemID == item.ItemID);
                                _utility.UpdateAvgCost(item_warehouse.ItemID, purchaseMemo.WarehouseID, itemMaster.GroupUomID, item_inventory_audit.Qty, item_inventory_audit.Cost);
                                _utility.UpdateBomCost(item_warehouse.ItemID, item_inventory_audit.Qty, item_inventory_audit.Cost);
                                _utility.CumulativeValue(item_inventory_audit.WarehouseID, item_inventory_audit.ItemID, item_inventory_audit.CumulativeValue, _itemAcc);
                                _context.WarehouseDetails.Update(item_IssusStock);
                                _context.InventoryAudits.Add(item_inventory_audit);
                                _context.SaveChanges();
                            }
                            waredetials = new List<WarehouseDetail>();
                            break;
                        }
                    }
                }
            }
            if (Type == "Add")
            {
                var outgoingVendor = new OutgoingPaymentVendor();
                //insert outgoingpaymentvendor
                outgoingVendor.TypePurchase = TypePurchase.CreditMemo;
                outgoingVendor.OutgoingPaymentVendorID = 0;
                outgoingVendor.VendorID = purchaseMemo.VendorID;
                outgoingVendor.BranchID = purchaseMemo.BranchID;
                outgoingVendor.WarehouseID = purchaseMemo.WarehouseID;
                outgoingVendor.CurrencyID = purchaseMemo.PurCurrencyID;
                outgoingVendor.PostingDate = purchaseMemo.PostingDate;
                outgoingVendor.Date = DateTime.Now;
                outgoingVendor.OverdueDays = 0;
                outgoingVendor.Total = purchaseMemo.BalanceDue;
                outgoingVendor.BalanceDue = purchaseMemo.BalanceDue;
                outgoingVendor.TotalPayment = purchaseMemo.BalanceDue;
                outgoingVendor.Applied_Amount = 0;
                outgoingVendor.CurrencyName = "";
                outgoingVendor.SysName = "";
                outgoingVendor.Status = "open";
                outgoingVendor.CashDiscount = 0;
                outgoingVendor.TotalDiscount = 0;
                outgoingVendor.ExchangeRate = purchaseMemo.PurRate;
                outgoingVendor.SysCurrency = purchaseMemo.SysCurrencyID;
                outgoingVendor.LocalCurID = purchaseMemo.LocalCurID;
                outgoingVendor.LocalSetRate = purchaseMemo.LocalSetRate;
                outgoingVendor.CompanyID = purchaseMemo.CompanyID;
                outgoingVendor.SeriesDetailID = purchaseMemo.SeriesDetailID;
                outgoingVendor.DocumentID = purchaseMemo.DocumentTypeID;
                outgoingVendor.Number = purchaseMemo.Number;
                _context.OutgoingPaymentVendors.Update(outgoingVendor);
                _context.SaveChanges();
            }

        }
        #endregion GoodIssuesStockBasic
        #region  GoodIssuesStock
        public void GoodIssuesStock(int purchaseMemoID, string Type, List<APCSerialNumber> serials, List<APCBatchNo> batches, FreightPurchase freight)
        {
            var purchaseMemo = _context.PurchaseCreditMemos.Find(purchaseMemoID);
            var memomaster = _context.PurchaseCreditMemos.FirstOrDefault(w => w.PurchaseMemoID == purchaseMemoID);
            var memodetail = _context.PurchaseCreditMemoDetails.Where(w => w.PurchaseCreditMemoID == purchaseMemoID).ToList();
            var docType = _context.DocumentTypes.Find(purchaseMemo.DocumentTypeID);
            var series = _context.Series.Find(purchaseMemo.SeriesID);
            int inventoryAccID = 0;
            decimal inventoryAccAmount = 0;
            JournalEntry journalEntry = new();
            SeriesDetail seriesDetail = new();
            List<JournalEntryDetail> journalEntryDetail = new();
            List<AccountBalance> accountBalance = new();
            var douTypeID = _context.DocumentTypes.FirstOrDefault(w => w.Code == "JE");
            var defaultJE = _context.Series.FirstOrDefault(w => w.Default == true && w.DocuTypeID == douTypeID.ID && w.NextNo != w.LastNo);
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
                journalEntry.Creator = purchaseMemo.UserID;
                journalEntry.BranchID = purchaseMemo.BranchID;
                journalEntry.TransNo = purchaseMemo.Number;
                journalEntry.PostingDate = purchaseMemo.PostingDate;
                journalEntry.DocumentDate = purchaseMemo.DocumentDate;
                journalEntry.DueDate = purchaseMemo.DueDate;
                journalEntry.SSCID = purchaseMemo.SysCurrencyID;
                journalEntry.LLCID = purchaseMemo.LocalCurID;
                journalEntry.CompanyID = purchaseMemo.CompanyID;
                journalEntry.LocalSetRate = (decimal)purchaseMemo.LocalSetRate;
                journalEntry.SeriesDID = seriesDetail.ID;
                journalEntry.Remarks = series.Name + " " + Sno;
                _context.Update(journalEntry);
                _context.SaveChanges();
            }
            //IssuseInstock
            // AccountReceice
            var accountReceive = _context.BusinessPartners.FirstOrDefault(w => w.ID == purchaseMemo.VendorID);
            var glAcc = _context.GLAccounts.FirstOrDefault(w => w.ID == accountReceive.GLAccID);
            if (glAcc.ID > 0)
            {
                journalEntryDetail.Add(new JournalEntryDetail
                {
                    JEID = journalEntry.ID,
                    Type = Financials.Type.BPCode,
                    ItemID = accountReceive.GLAccID,
                    Debit = purchaseMemo.SubTotalAfterDisSys,
                    BPAcctID = purchaseMemo.VendorID,
                });
                //Insert 
                glAcc.Balance += purchaseMemo.SubTotalAfterDisSys;
                accountBalance.Add(new AccountBalance
                {
                    JEID = journalEntry.ID,
                    PostingDate = purchaseMemo.PostingDate,
                    Origin = docType.ID,
                    OriginNo = purchaseMemo.Number,
                    OffsetAccount = glAcc.Code,
                    Details = douTypeID.Name + "-" + glAcc.Code,
                    CumulativeBalance = glAcc.Balance,
                    Debit = purchaseMemo.SubTotalAfterDisSys,
                    LocalSetRate = (decimal)purchaseMemo.LocalSetRate,
                    GLAID = accountReceive.GLAccID,
                    BPAcctID = purchaseMemo.VendorID,
                    Creator = purchaseMemo.UserID,
                    Effective = EffectiveBlance.Debit
                });
                _context.Update(glAcc);
            }
            #region
            // Freight //
            if (freight != null)
            {
                if (freight.FreightPurchaseDetials.Any())
                {
                    foreach (var fr in freight.FreightPurchaseDetials.Where(i => i.Amount > 0).ToList())
                    {
                        var freightOg = _context.Freights.Find(fr.FreightID) ?? new Freight();
                        var frgl = _context.GLAccounts.Find(freightOg.ExpenAcctID) ?? new GLAccount();
                        var taxfr = _context.TaxGroups.Find(fr.TaxGroupID) ?? new TaxGroup();
                        var taxgacc = _context.GLAccounts.Find(taxfr.GLID) ?? new GLAccount();
                        if (frgl.ID > 0)
                        {
                            var frgljur = journalEntryDetail.FirstOrDefault(w => w.ItemID == frgl.ID) ?? new JournalEntryDetail();
                            var _framount = fr.Amount * (decimal)purchaseMemo.PurRate;
                            if (frgljur.ItemID > 0)
                            {
                                var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == frgl.ID);
                                frgl.Balance -= _framount;
                                //journalEntryDetail
                                frgljur.Credit += _framount;
                                //accountBalance
                                accBalance.CumulativeBalance = frgl.Balance;
                                accBalance.Credit += _framount;
                            }
                            else
                            {
                                frgl.Balance -= _framount;
                                journalEntryDetail.Add(new JournalEntryDetail
                                {
                                    JEID = journalEntry.ID,
                                    Type = Financials.Type.GLAcct,
                                    ItemID = frgl.ID,
                                    Credit = _framount,
                                });
                                //
                                accountBalance.Add(new AccountBalance
                                {
                                    JEID = journalEntry.ID,
                                    PostingDate = purchaseMemo.PostingDate,
                                    Origin = docType.ID,
                                    OriginNo = purchaseMemo.Number,
                                    OffsetAccount = glAcc.Code,
                                    Details = douTypeID.Name + " - " + frgl.Code,
                                    CumulativeBalance = frgl.Balance,
                                    Credit = _framount,
                                    LocalSetRate = (decimal)purchaseMemo.LocalSetRate,
                                    GLAID = frgl.ID,
                                    Effective = EffectiveBlance.Credit
                                });
                            }
                            _context.Update(frgl);
                            _context.SaveChanges();
                        }
                        if (taxgacc.ID > 0)
                        {
                            var frtaxgljur = journalEntryDetail.FirstOrDefault(w => w.ItemID == taxgacc.ID) ?? new JournalEntryDetail();
                            var _frtaxamount = fr.TotalTaxAmount * (decimal)purchaseMemo.PurRate;
                            if (frtaxgljur.ItemID > 0)
                            {
                                var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == taxgacc.ID) ?? new AccountBalance();
                                taxgacc.Balance -= _frtaxamount;
                                //journalEntryDetail
                                frtaxgljur.Credit += _frtaxamount;
                                //accountBalance
                                accBalance.CumulativeBalance = frgl.Balance;
                                accBalance.Credit += _frtaxamount;
                            }
                            else
                            {
                                taxgacc.Balance -= _frtaxamount;
                                journalEntryDetail.Add(new JournalEntryDetail
                                {
                                    JEID = journalEntry.ID,
                                    Type = Financials.Type.GLAcct,
                                    ItemID = taxgacc.ID,
                                    Credit = _frtaxamount,
                                });
                                //
                                accountBalance.Add(new AccountBalance
                                {
                                    JEID = journalEntry.ID,

                                    PostingDate = purchaseMemo.PostingDate,
                                    Origin = docType.ID,
                                    OriginNo = purchaseMemo.Number,
                                    OffsetAccount = glAcc.Code,
                                    Details = douTypeID.Name + " - " + taxgacc.Code,
                                    CumulativeBalance = taxgacc.Balance,
                                    Credit = _frtaxamount,
                                    LocalSetRate = (decimal)purchaseMemo.LocalSetRate,
                                    GLAID = taxgacc.ID,
                                    Effective = EffectiveBlance.Credit
                                });
                            }
                            _context.Update(taxgacc);
                            _context.SaveChanges();
                        }
                    }
                }
            }
            #endregion
            foreach (var item in memodetail)
            {
                var itemMaster = _context.ItemMasterDatas.FirstOrDefault(w => w.ID == item.ItemID);
                ItemAccounting _itemAcc = _context.ItemAccountings.FirstOrDefault(i => i.ItemID == item.ItemID && purchaseMemo.WarehouseID == i.WarehouseID);
                if (itemMaster.SetGlAccount == Inventory.SetGlAccount.ItemLevel)
                {
                    var inventoryAcc = (from ia in _context.ItemAccountings.Where(w => w.ItemID == item.ItemID && w.WarehouseID == purchaseMemo.WarehouseID)
                                        join gl in _context.GLAccounts on ia.InventoryAccount equals gl.Code
                                        select gl
                                         ).FirstOrDefault();
                    if (inventoryAcc != null)
                    {
                        inventoryAccID = inventoryAcc.ID;
                        if (purchaseMemo.DiscountRate > 0)
                        {
                            decimal disvalue = (decimal)item.TotalSys * (decimal)purchaseMemo.DiscountRate / 100;
                            inventoryAccAmount = (decimal)item.TotalSys - disvalue;
                        }
                        else
                        {
                            inventoryAccAmount = (decimal)item.TotalSys;
                        }
                    }
                }
                else if (itemMaster.SetGlAccount == Inventory.SetGlAccount.ItemGroup)
                {
                    var inventoryAcc = (from ia in _context.ItemAccountings.Where(w => w.ItemGroupID == itemMaster.ItemGroup1ID)
                                        join gl in _context.GLAccounts on ia.InventoryAccount equals gl.Code
                                        select gl
                                         ).FirstOrDefault();
                    if (inventoryAcc != null)
                    {
                        inventoryAccID = inventoryAcc.ID;
                        if (purchaseMemo.DiscountRate > 0)
                        {
                            decimal disvalue = (decimal)item.TotalSys * (decimal)purchaseMemo.DiscountRate / 100;
                            inventoryAccAmount = (decimal)item.TotalSys - disvalue;
                        }
                        else
                        {
                            inventoryAccAmount = (decimal)item.TotalSys;
                        }
                    }
                }
                #region
                //// Tax Account ///
                var taxg = _context.TaxGroups.Find(item.TaxGroupID) ?? new TaxGroup();
                var taxAcc = _context.GLAccounts.Find(taxg.GLID) ?? new GLAccount();
                decimal taxValue = item.TaxOfFinDisValue * (decimal)purchaseMemo.PurRate;
                if (taxAcc.ID > 0)
                {
                    var taxjur = journalEntryDetail.FirstOrDefault(w => w.ItemID == taxAcc.ID) ?? new JournalEntryDetail();
                    if (taxjur.ItemID > 0)
                    {
                        var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == taxAcc.ID);
                        taxAcc.Balance -= taxValue;
                        //journalEntryDetail
                        taxjur.Credit += taxValue;
                        //accountBalance
                        accBalance.CumulativeBalance = taxAcc.Balance;
                        accBalance.Credit += taxValue;
                    }
                    else
                    {
                        taxAcc.Balance -= taxValue;
                        journalEntryDetail.Add(new JournalEntryDetail
                        {
                            JEID = journalEntry.ID,
                            Type = Financials.Type.GLAcct,
                            ItemID = taxAcc.ID,
                            Credit = taxValue,
                        });
                        //
                        accountBalance.Add(new AccountBalance
                        {
                            JEID = journalEntry.ID,
                            PostingDate = purchaseMemo.PostingDate,
                            Origin = docType.ID,
                            OriginNo = purchaseMemo.Number,
                            OffsetAccount = taxAcc.Code,
                            Details = douTypeID.Name + " - " + taxAcc.Code,
                            CumulativeBalance = taxAcc.Balance,
                            Credit = taxValue,
                            LocalSetRate = (decimal)purchaseMemo.LocalSetRate,
                            GLAID = taxAcc.ID,
                            Effective = EffectiveBlance.Credit
                        });
                    }
                    _context.Update(taxAcc);
                }
                #endregion
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
                            Type = Financials.Type.GLAcct,
                            ItemID = inventoryAccID,
                            Credit = inventoryAccAmount,
                        });
                        //
                        accountBalance.Add(new AccountBalance
                        {
                            JEID = journalEntry.ID,
                            PostingDate = purchaseMemo.PostingDate,
                            Origin = docType.ID,
                            OriginNo = purchaseMemo.Number,
                            OffsetAccount = glAcc.Code,
                            Details = douTypeID.Name + "-" + glAccInvenfifo.Code,
                            CumulativeBalance = glAccInvenfifo.Balance,
                            Credit = inventoryAccAmount,
                            LocalSetRate = (decimal)purchaseMemo.LocalSetRate,
                            GLAID = inventoryAccID,
                            Effective = EffectiveBlance.Credit
                        });
                    }
                    _context.GLAccounts.Update(glAccInvenfifo);
                }
                var orft = _context.GroupDUoMs.FirstOrDefault(w => w.GroupUoMID == itemMaster.GroupUomID && w.AltUOM == item.UomID);
                var itemWareSum = _context.WarehouseSummary.FirstOrDefault(w => w.WarehouseID == purchaseMemo.WarehouseID && w.ItemID == item.ItemID);
                var Cost = item.PurchasPrice * purchaseMemo.PurRate;
                var waredetials = _context.WarehouseDetails.Where(w => w.WarehouseID == purchaseMemo.WarehouseID && w.ItemID == item.ItemID).ToList();
                if (itemWareSum != null)
                {
                    //WerehouseSummary
                    itemWareSum.InStock -= (double)item.Qty;
                    //Itemmasterdata
                    itemMaster.StockIn = itemWareSum.InStock - (double)item.Qty;
                    _context.WarehouseSummary.Update(itemWareSum);
                    _context.ItemMasterDatas.Update(itemMaster);
                    _utility.UpdateItemAccounting(_itemAcc, itemWareSum);
                }
                double @Check_Stock;
                double @Remain;
                double @IssusQty;
                double @FIFOQty;
                double @Qty = item.Qty * orft.Factor;
                if (itemMaster.ManItemBy == ManageItemBy.SerialNumbers
                    && itemMaster.ManMethod == ManagementMethod.OnEveryTransation)
                {
                    if (serials.Count > 0)
                    {
                        List<InventoryAudit> invens = new();
                        List<StockOut> stockOuts = new();
                        foreach (var s in serials)
                        {
                            if (s.APCSNSelected != null)
                            {
                                foreach (var ss in s.APCSNSelected.APCSNDSelectedDetails)
                                {
                                    var waredetial = waredetials
                                        .FirstOrDefault(i => ss.SerialNumber == i.SerialNumber && i.InStock > 0 && i.Cost == item.PurchasPrice * purchaseMemo.PurRate);
                                    if (waredetial != null)
                                    {
                                        waredetial.InStock -= 1;
                                        // Insert to warehouse detial //
                                        stockOuts.Add(new StockOut
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
                                            UserID = purchaseMemo.UserID,
                                            ExpireDate = item.ExpireDate,
                                            TransType = TransTypeWD.PurCreditMemo,
                                            TransID = purchaseMemo.PurchaseMemoID,
                                            OutStockFrom = purchaseMemo.PurchaseMemoID,
                                            FromWareDetialID = waredetial.ID,
                                        });
                                    }
                                }
                            }
                        }
                        // Insert to InventoryAudit //
                        var inventory_audit = _context.InventoryAudits
                            .Where(w => w.ItemID == item.ItemID && w.WarehouseID == purchaseMemo.WarehouseID);
                        var inven = new InventoryAudit
                        {
                            ID = 0,
                            WarehouseID = purchaseMemo.WarehouseID,
                            BranchID = purchaseMemo.BranchID,
                            UserID = purchaseMemo.UserID,
                            ItemID = item.ItemID,
                            CurrencyID = purchaseMemo.CompanyID,
                            UomID = orft.BaseUOM,
                            InvoiceNo = purchaseMemo.Number,
                            Trans_Type = docType.Code,
                            Process = itemMaster.Process,
                            SystemDate = DateTime.Now,
                            PostingDate = purchaseMemo.PostingDate,
                            TimeIn = DateTime.Now.ToShortTimeString().ToString(),
                            Qty = @Qty * -1,
                            Cost = Cost,
                            Price = 0,
                            CumulativeQty = (inventory_audit.Sum(q => q.Qty) - @Qty),
                            CumulativeValue = (inventory_audit.Sum(s => s.Trans_Valuse)) - (@Qty * Cost),
                            Trans_Valuse = @Qty * Cost * -1,
                            ExpireDate = item.ExpireDate,
                            LocalCurID = purchaseMemo.LocalCurID,
                            LocalSetRate = purchaseMemo.LocalSetRate,
                            CompanyID = purchaseMemo.CompanyID,
                            DocumentTypeID = docType.ID,
                            SeriesID = purchaseMemo.SeriesID,
                            SeriesDetailID = purchaseMemo.SeriesDetailID,
                        };
                        _context.InventoryAudits.Add(inven);
                        _context.StockOuts.AddRange(stockOuts);
                        _context.SaveChanges();
                    }
                }
                else if (itemMaster.ManItemBy == ManageItemBy.Batches
                    && itemMaster.ManMethod == ManagementMethod.OnEveryTransation)
                {
                    if (batches.Count > 0)
                    {
                        foreach (var b in batches)
                        {
                            if (b.APCBatchNoSelected != null)
                            {
                                foreach (var sb in b.APCBatchNoSelected.APCBatchNoSelectedDetails)
                                {
                                    var waredetial = waredetials
                                        .FirstOrDefault(i =>
                                            sb.BatchNo == i.BatchNo
                                            && i.InStock > 0
                                            && i.Cost == item.PurchasPrice * purchaseMemo.PurRate);
                                    if (waredetial != null)
                                    {
                                        waredetial.InStock -= (double)sb.SelectedQty;
                                        _context.SaveChanges();
                                        // insert to waredetial
                                        var stockOuts = new StockOut
                                        {
                                            AdmissionDate = waredetial.AdmissionDate,
                                            Cost = (decimal)waredetial.Cost,
                                            CurrencyID = waredetial.CurrencyID,
                                            Details = waredetial.Details,
                                            ID = 0,
                                            InStock = sb.SelectedQty,
                                            ItemID = item.ItemID,
                                            Location = waredetial.Location,
                                            MfrDate = waredetial.MfrDate,
                                            ProcessItem = ProcessItem.SEBA,
                                            SyetemDate = DateTime.Now,
                                            SysNum = 0,
                                            TimeIn = DateTime.Now,
                                            WarehouseID = waredetial.WarehouseID,
                                            UomID = item.UomID,
                                            UserID = purchaseMemo.UserID,
                                            ExpireDate = item.ExpireDate,
                                            BatchAttr1 = waredetial.BatchAttr1,
                                            BatchAttr2 = waredetial.BatchAttr2,
                                            BatchNo = waredetial.BatchNo,
                                            TransType = TransTypeWD.PurCreditMemo,
                                            TransID = purchaseMemo.PurchaseMemoID,
                                            FromWareDetialID = waredetial.ID,
                                            OutStockFrom = purchaseMemo.PurCurrencyID,
                                        };
                                        _context.StockOuts.Add(stockOuts);
                                        _context.SaveChanges();
                                    }
                                }
                            }
                        }
                        // insert to inventory audit
                        var inventory_audit = _context.InventoryAudits
                            .Where(w => w.ItemID == item.ItemID && w.WarehouseID == purchaseMemo.WarehouseID);
                        var invens = new InventoryAudit
                        {
                            ID = 0,
                            WarehouseID = purchaseMemo.WarehouseID,
                            BranchID = purchaseMemo.BranchID,
                            UserID = purchaseMemo.UserID,
                            ItemID = item.ItemID,
                            CurrencyID = purchaseMemo.CompanyID,
                            UomID = orft.BaseUOM,
                            InvoiceNo = purchaseMemo.Number,
                            Trans_Type = docType.Code,
                            Process = itemMaster.Process,
                            SystemDate = DateTime.Now,
                            PostingDate = purchaseMemo.PostingDate,
                            TimeIn = DateTime.Now.ToShortTimeString().ToString(),
                            Qty = @Qty * -1,
                            Cost = Cost,
                            Price = 0,
                            CumulativeQty = inventory_audit.Sum(q => q.Qty) - @Qty,
                            CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) - (@Qty * Cost),
                            Trans_Valuse = @Qty * Cost * -1,
                            ExpireDate = item.ExpireDate,
                            LocalCurID = purchaseMemo.LocalCurID,
                            LocalSetRate = purchaseMemo.LocalSetRate,
                            CompanyID = purchaseMemo.CompanyID,
                            DocumentTypeID = docType.ID,
                            SeriesID = purchaseMemo.SeriesID,
                            SeriesDetailID = purchaseMemo.SeriesDetailID,
                        };
                        _context.InventoryAudits.Add(invens);
                        _context.SaveChanges();
                    }
                }
                else
                {
                    foreach (var item_warehouse in waredetials.Where(w => w.InStock > 0))
                    {
                        var item_inventory_audit = new InventoryAudit();
                        var item_IssusStock = new WarehouseDetail();
                        var itmewhd = waredetials.FirstOrDefault(w => w.InStock >= @Qty && w.Cost == Cost) ?? new WarehouseDetail();
                        if (itmewhd.ID > 0)
                        {
                            item_IssusStock = itmewhd;
                        }
                        else
                        {
                            item_IssusStock = waredetials.FirstOrDefault(w => w.InStock > 0);
                        }
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
                                        UserID = purchaseMemo.UserID,
                                        ExpireDate = item.ExpireDate,
                                        TransType = TransTypeWD.PurCreditMemo,
                                    };
                                    _context.StockOuts.Add(stockOuts);
                                    var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item.ItemID && w.WarehouseID == purchaseMemo.WarehouseID);
                                    item_inventory_audit.ID = 0;
                                    item_inventory_audit.WarehouseID = purchaseMemo.WarehouseID;
                                    item_inventory_audit.BranchID = purchaseMemo.BranchID;
                                    item_inventory_audit.UserID = purchaseMemo.UserID;
                                    item_inventory_audit.ItemID = item.ItemID;
                                    item_inventory_audit.CurrencyID = purchaseMemo.CompanyID;
                                    item_inventory_audit.UomID = orft.BaseUOM;
                                    item_inventory_audit.InvoiceNo = purchaseMemo.Number;
                                    item_inventory_audit.Trans_Type = docType.Code;
                                    item_inventory_audit.Process = itemMaster.Process;
                                    item_inventory_audit.SystemDate = DateTime.Now;
                                    item_inventory_audit.PostingDate = purchaseMemo.PostingDate;
                                    item_inventory_audit.TimeIn = DateTime.Now.ToShortTimeString().ToString();
                                    item_inventory_audit.Qty = @IssusQty * -1;
                                    item_inventory_audit.Cost = item_IssusStock.Cost;
                                    item_inventory_audit.Price = 0;
                                    item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) - @IssusQty;
                                    item_inventory_audit.CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) - (@IssusQty * item_IssusStock.Cost);
                                    item_inventory_audit.Trans_Valuse = @IssusQty * item_IssusStock.Cost * -1;
                                    item_inventory_audit.ExpireDate = item_IssusStock.ExpireDate;
                                    item_inventory_audit.LocalCurID = purchaseMemo.LocalCurID;
                                    item_inventory_audit.LocalSetRate = purchaseMemo.LocalSetRate;
                                    item_inventory_audit.CompanyID = purchaseMemo.CompanyID;
                                    item_inventory_audit.DocumentTypeID = docType.ID;
                                    item_inventory_audit.SeriesID = purchaseMemo.SeriesID;
                                    item_inventory_audit.SeriesDetailID = purchaseMemo.SeriesDetailID;
                                }
                                _utility.CumulativeValue(item_inventory_audit.WarehouseID, item_inventory_audit.ItemID, item_inventory_audit.CumulativeValue, _itemAcc);
                                _context.WarehouseDetails.Update(item_IssusStock);
                                _context.InventoryAudits.Add(item_inventory_audit);
                                _context.SaveChanges();
                            }
                            else if (itemMaster.Process == "Average")
                            {
                                item_IssusStock.InStock = item_IssusStock.InStock -= @IssusQty;
                                if (@IssusQty > 0)
                                {
                                    var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item.ItemID && w.WarehouseID == purchaseMemo.WarehouseID);
                                    var avgInven = new InventoryAudit { Qty = @IssusQty, Cost = Cost };
                                    double @sysAvCost = _utility.CalAVGCost(item.ItemID, item_warehouse.ID, avgInven, false);
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
                                        UserID = purchaseMemo.UserID,
                                        ExpireDate = item.ExpireDate,
                                        TransType = TransTypeWD.PurCreditMemo,
                                    };
                                    _context.StockOuts.Add(stockOuts);

                                    item_inventory_audit.ID = 0;
                                    item_inventory_audit.WarehouseID = purchaseMemo.WarehouseID;
                                    item_inventory_audit.BranchID = purchaseMemo.BranchID;
                                    item_inventory_audit.UserID = purchaseMemo.UserID;
                                    item_inventory_audit.ItemID = item.ItemID;
                                    item_inventory_audit.CurrencyID = purchaseMemo.CompanyID;
                                    item_inventory_audit.UomID = orft.BaseUOM;
                                    item_inventory_audit.InvoiceNo = purchaseMemo.Number;
                                    item_inventory_audit.Trans_Type = docType.Code;
                                    item_inventory_audit.Process = itemMaster.Process;
                                    item_inventory_audit.SystemDate = DateTime.Now;
                                    item_inventory_audit.PostingDate = purchaseMemo.PostingDate;
                                    item_inventory_audit.TimeIn = DateTime.Now.ToShortTimeString().ToString();
                                    item_inventory_audit.Qty = @IssusQty * -1;
                                    item_inventory_audit.Cost = @sysAvCost;
                                    item_inventory_audit.Price = 0;
                                    item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) - @IssusQty;
                                    item_inventory_audit.CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) - (@IssusQty * @sysAvCost);
                                    item_inventory_audit.Trans_Valuse = @IssusQty * @sysAvCost * (-1);
                                    item_inventory_audit.ExpireDate = item_IssusStock.ExpireDate;
                                    item_inventory_audit.LocalCurID = purchaseMemo.LocalCurID;
                                    item_inventory_audit.LocalSetRate = purchaseMemo.LocalSetRate;
                                    item_inventory_audit.CompanyID = purchaseMemo.CompanyID;
                                    item_inventory_audit.DocumentTypeID = docType.ID;
                                    item_inventory_audit.SeriesID = purchaseMemo.SeriesID;
                                    item_inventory_audit.SeriesDetailID = purchaseMemo.SeriesDetailID;
                                }
                                var inventoryAcct = _context.InventoryAudits.Where(w => w.ItemID == item.ItemID);
                                double @AvgCost = ((inventoryAcct.Sum(s => s.Trans_Valuse) + item_inventory_audit.Cost) / (inventoryAcct.Sum(q => q.Qty) + item_inventory_audit.Qty));
                                @AvgCost = _utility.CheckNaNOrInfinity(@AvgCost);
                                _utility.UpdateAvgCost(item_warehouse.ItemID, purchaseMemo.WarehouseID, itemMaster.GroupUomID, item_inventory_audit.Qty, item_inventory_audit.Cost);
                                _utility.UpdateBomCost(item_warehouse.ItemID, item_inventory_audit.Qty, item_inventory_audit.Cost);
                                _utility.CumulativeValue(item_inventory_audit.WarehouseID, item_inventory_audit.ItemID, item_inventory_audit.CumulativeValue, _itemAcc);
                                _context.WarehouseDetails.Update(item_IssusStock);
                                _context.InventoryAudits.Add(item_inventory_audit);
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
                                        UserID = purchaseMemo.UserID,
                                        ExpireDate = item.ExpireDate,
                                        TransType = TransTypeWD.PurCreditMemo,
                                    };
                                    _context.StockOuts.Add(stockOuts);

                                    var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item.ItemID && w.WarehouseID == purchaseMemo.WarehouseID);
                                    item_inventory_audit.ID = 0;
                                    item_inventory_audit.WarehouseID = purchaseMemo.WarehouseID;
                                    item_inventory_audit.BranchID = purchaseMemo.BranchID;
                                    item_inventory_audit.UserID = purchaseMemo.UserID;
                                    item_inventory_audit.ItemID = item.ItemID;
                                    item_inventory_audit.CurrencyID = purchaseMemo.CompanyID;
                                    item_inventory_audit.UomID = orft.BaseUOM;
                                    item_inventory_audit.InvoiceNo = purchaseMemo.Number;
                                    item_inventory_audit.Trans_Type = docType.Code;
                                    item_inventory_audit.Process = itemMaster.Process;
                                    item_inventory_audit.SystemDate = DateTime.Now;
                                    item_inventory_audit.PostingDate = purchaseMemo.PostingDate;
                                    item_inventory_audit.TimeIn = DateTime.Now.ToShortTimeString().ToString();
                                    item_inventory_audit.Qty = @IssusQty * -1;
                                    item_inventory_audit.Cost = item_IssusStock.Cost;
                                    item_inventory_audit.Price = 0;
                                    item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) - @IssusQty;
                                    item_inventory_audit.CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) - (@IssusQty * item_IssusStock.Cost);
                                    item_inventory_audit.Trans_Valuse = @IssusQty * item_IssusStock.Cost * (-1);
                                    item_inventory_audit.ExpireDate = item_IssusStock.ExpireDate;
                                    item_inventory_audit.LocalCurID = purchaseMemo.LocalCurID;
                                    item_inventory_audit.LocalSetRate = purchaseMemo.LocalSetRate;
                                    item_inventory_audit.CompanyID = purchaseMemo.CompanyID;
                                    item_inventory_audit.DocumentTypeID = docType.ID;
                                    item_inventory_audit.SeriesID = purchaseMemo.SeriesID;
                                    item_inventory_audit.SeriesDetailID = purchaseMemo.SeriesDetailID;
                                }
                                _utility.CumulativeValue(item_inventory_audit.WarehouseID, item_inventory_audit.ItemID, item_inventory_audit.CumulativeValue, _itemAcc);
                                _context.WarehouseDetails.Update(item_IssusStock);
                                _context.InventoryAudits.Add(item_inventory_audit);
                                _context.SaveChanges();
                            }
                            else if (itemMaster.Process == "Average")
                            {
                                item_IssusStock.InStock = @FIFOQty;
                                if (@IssusQty > 0)
                                {
                                    var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item.ItemID && w.WarehouseID == purchaseMemo.WarehouseID);
                                    var avgInven = new InventoryAudit { Qty = @IssusQty, Cost = Cost };
                                    double @sysAvCost = _utility.CalAVGCost(item.ItemID, item_warehouse.ID, avgInven, false);
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
                                        UserID = purchaseMemo.UserID,
                                        ExpireDate = item.ExpireDate,
                                        TransType = TransTypeWD.PurCreditMemo,
                                    };
                                    _context.StockOuts.Add(stockOuts);
                                    item_inventory_audit.ID = 0;
                                    item_inventory_audit.WarehouseID = purchaseMemo.WarehouseID;
                                    item_inventory_audit.BranchID = purchaseMemo.BranchID;
                                    item_inventory_audit.UserID = purchaseMemo.UserID;
                                    item_inventory_audit.ItemID = item.ItemID;
                                    item_inventory_audit.CurrencyID = purchaseMemo.CompanyID;
                                    item_inventory_audit.UomID = orft.BaseUOM;
                                    item_inventory_audit.InvoiceNo = purchaseMemo.Number;
                                    item_inventory_audit.Trans_Type = docType.Code;
                                    item_inventory_audit.Process = itemMaster.Process;
                                    item_inventory_audit.SystemDate = DateTime.Now;
                                    item_inventory_audit.PostingDate = purchaseMemo.PostingDate;
                                    item_inventory_audit.TimeIn = DateTime.Now.ToShortTimeString().ToString();
                                    item_inventory_audit.Qty = @IssusQty * -1;
                                    item_inventory_audit.Cost = @sysAvCost;
                                    item_inventory_audit.Price = 0;
                                    item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) - @IssusQty;
                                    item_inventory_audit.CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) - (@IssusQty * @sysAvCost);
                                    item_inventory_audit.Trans_Valuse = @IssusQty * @sysAvCost * -1;
                                    item_inventory_audit.ExpireDate = item_IssusStock.ExpireDate;
                                    item_inventory_audit.LocalCurID = purchaseMemo.LocalCurID;
                                    item_inventory_audit.LocalSetRate = purchaseMemo.LocalSetRate;
                                    item_inventory_audit.CompanyID = purchaseMemo.CompanyID;
                                    item_inventory_audit.DocumentTypeID = docType.ID;
                                    item_inventory_audit.SeriesID = purchaseMemo.SeriesID;
                                    item_inventory_audit.SeriesDetailID = purchaseMemo.SeriesDetailID;
                                }
                                var inventoryAcct = _context.InventoryAudits.Where(w => w.ItemID == item.ItemID);
                                _utility.UpdateAvgCost(item_warehouse.ItemID, purchaseMemo.WarehouseID, itemMaster.GroupUomID, item_inventory_audit.Qty, item_inventory_audit.Cost);
                                _utility.UpdateBomCost(item_warehouse.ItemID, item_inventory_audit.Qty, item_inventory_audit.Cost);
                                _utility.CumulativeValue(item_inventory_audit.WarehouseID, item_inventory_audit.ItemID, item_inventory_audit.CumulativeValue, _itemAcc);
                                _context.WarehouseDetails.Update(item_IssusStock);
                                _context.InventoryAudits.Add(item_inventory_audit);
                                _context.SaveChanges();
                            }
                            waredetials = new List<WarehouseDetail>();
                            break;
                        }
                    }
                }
            }
            var journal = _context.JournalEntries.Find(journalEntry.ID);
            journal.TotalDebit = journalEntryDetail.Sum(s => s.Debit);
            journal.TotalCredit = journalEntryDetail.Sum(s => s.Credit);
            _context.JournalEntries.Update(journal);
            _context.JournalEntryDetails.UpdateRange(journalEntryDetail);
            _context.AccountBalances.UpdateRange(accountBalance);
            _context.SaveChanges();
            if (Type == "Add")
            {
                var outgoingVendor = new OutgoingPaymentVendor();
                //insert outgoingpaymentvendor
                outgoingVendor.TypePurchase = TypePurchase.CreditMemo;
                outgoingVendor.OutgoingPaymentVendorID = 0;
                outgoingVendor.VendorID = purchaseMemo.VendorID;
                outgoingVendor.BranchID = purchaseMemo.BranchID;
                outgoingVendor.WarehouseID = purchaseMemo.WarehouseID;
                outgoingVendor.CurrencyID = purchaseMemo.PurCurrencyID;
                outgoingVendor.PostingDate = purchaseMemo.PostingDate;
                outgoingVendor.Date = DateTime.Now;
                outgoingVendor.OverdueDays = 0;
                outgoingVendor.Total = purchaseMemo.BalanceDue;
                outgoingVendor.BalanceDue = purchaseMemo.BalanceDue;
                outgoingVendor.TotalPayment = purchaseMemo.BalanceDue;
                outgoingVendor.Applied_Amount = 0;
                outgoingVendor.CurrencyName = "";
                outgoingVendor.SysName = "";
                outgoingVendor.Status = "open";
                outgoingVendor.CashDiscount = 0;
                outgoingVendor.TotalDiscount = 0;
                outgoingVendor.ExchangeRate = purchaseMemo.PurRate;
                outgoingVendor.SysCurrency = purchaseMemo.SysCurrencyID;
                outgoingVendor.LocalCurID = purchaseMemo.LocalCurID;
                outgoingVendor.LocalSetRate = purchaseMemo.LocalSetRate;
                outgoingVendor.CompanyID = purchaseMemo.CompanyID;
                outgoingVendor.SeriesDetailID = purchaseMemo.SeriesDetailID;
                outgoingVendor.DocumentID = purchaseMemo.DocumentTypeID;
                outgoingVendor.Number = purchaseMemo.Number;
                _context.OutgoingPaymentVendors.Update(outgoingVendor);
                _context.SaveChanges();
            }

        }
        #endregion GoodIssuesStock
        public void GoodIssuesStockAPReserve(int copynote, int purchaseMemoID, string Type, List<APCSerialNumber> serials, List<APCBatchNo> batches, FreightPurchase freight)
        {
            var purchaseMemo = _context.PurchaseCreditMemos.Find(purchaseMemoID);
            var memomaster = _context.PurchaseCreditMemos.FirstOrDefault(w => w.PurchaseMemoID == purchaseMemoID);
            var memodetail = _context.PurchaseCreditMemoDetails.Where(w => w.PurchaseCreditMemoID == purchaseMemoID).ToList();

            var docType = _context.DocumentTypes.Find(purchaseMemo.DocumentTypeID);
            var series = _context.SeriesDetails.Find(purchaseMemo.SeriesDetailID);

            List<GLAccount> gLAccounts = _context.GLAccounts.Where(i => i.IsActive).ToList();
            int allocateAccID = 0;
            decimal inventoryAccAmount = 0, allocateAccAmount = 0; ;
            JournalEntry journalEntry = new();
            SeriesDetail seriesDetail = new();
            List<JournalEntryDetail> journalEntryDetail = new();
            List<AccountBalance> accountBalance = new();
            var douTypeID = _context.DocumentTypes.FirstOrDefault(w => w.Code == "JE");
            var defaultJE = _context.Series.FirstOrDefault(w => w.Default == true && w.DocuTypeID == douTypeID.ID && w.NextNo != w.LastNo);
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
                journalEntry.Creator = purchaseMemo.UserID;
                  journalEntry.BranchID = purchaseMemo.BranchID;
                journalEntry.TransNo = Sno;
                journalEntry.PostingDate = purchaseMemo.PostingDate;
                journalEntry.DocumentDate = purchaseMemo.DocumentDate;
                journalEntry.DueDate = purchaseMemo.DueDate;
                journalEntry.SSCID = purchaseMemo.SysCurrencyID;
                journalEntry.LLCID = purchaseMemo.LocalCurID;
                journalEntry.CompanyID = purchaseMemo.CompanyID;
                journalEntry.LocalSetRate = (decimal)purchaseMemo.LocalSetRate;
                journalEntry.SeriesDID = seriesDetail.ID;
                journalEntry.Remarks = defaultJE.Name + " " + Sno;
                _context.Update(journalEntry);
                _context.SaveChanges();
            }
            //IssuseInstock
            // AccountReceice
            var accountReceive = _context.BusinessPartners.FirstOrDefault(w => w.ID == purchaseMemo.VendorID);
            var glAcc = _context.GLAccounts.FirstOrDefault(w => w.ID == accountReceive.GLAccID);
            if (glAcc.ID > 0)
            {
                journalEntryDetail.Add(new JournalEntryDetail
                {
                    JEID = journalEntry.ID,
                    Type = Financials.Type.BPCode,
                    ItemID = accountReceive.GLAccID,
                    Credit = purchaseMemo.SubTotalAfterDisSys * -1,
                    BPAcctID = purchaseMemo.VendorID,
                });
                //Insert 
                glAcc.Balance += purchaseMemo.SubTotalAfterDisSys * -1;
                accountBalance.Add(new AccountBalance
                {
                    JEID = journalEntry.ID,
                    PostingDate = purchaseMemo.PostingDate,
                    Origin = docType.ID,
                    OriginNo = purchaseMemo.Number,
                    OffsetAccount = glAcc.Code,
                    Details = douTypeID.Name + "-" + glAcc.Code,
                    CumulativeBalance = glAcc.Balance,
                    Credit = purchaseMemo.SubTotalAfterDisSys * -1,
                    LocalSetRate = (decimal)purchaseMemo.LocalSetRate,
                    GLAID = accountReceive.GLAccID,
                    BPAcctID = purchaseMemo.VendorID,
                    Creator = purchaseMemo.UserID,
                    Effective = EffectiveBlance.Credit
                });
                _context.Update(glAcc);
            }
            #region
            // Freight //
            if (freight != null)
            {
                if (freight.FreightPurchaseDetials.Any())
                {
                    foreach (var fr in freight.FreightPurchaseDetials.Where(i => i.Amount > 0).ToList())
                    {
                        var freightOg = _context.Freights.Find(fr.FreightID) ?? new Freight();
                        var frgl = _context.GLAccounts.Find(freightOg.ExpenAcctID) ?? new GLAccount();
                        var taxfr = _context.TaxGroups.Find(fr.TaxGroupID) ?? new TaxGroup();
                        var taxgacc = _context.GLAccounts.Find(taxfr.GLID) ?? new GLAccount();
                        if (frgl.ID > 0)
                        {
                            var frgljur = journalEntryDetail.FirstOrDefault(w => w.ItemID == frgl.ID) ?? new JournalEntryDetail();
                            var _framount = fr.Amount * (decimal)purchaseMemo.PurRate;
                            if (frgljur.ItemID > 0)
                            {
                                var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == frgl.ID);
                                frgl.Balance -= _framount;
                                //journalEntryDetail
                                frgljur.Credit += _framount;
                                //accountBalance
                                accBalance.CumulativeBalance = frgl.Balance;
                                accBalance.Credit += _framount;
                            }
                            else
                            {
                                frgl.Balance -= _framount;
                                journalEntryDetail.Add(new JournalEntryDetail
                                {
                                    JEID = journalEntry.ID,
                                    Type = Financials.Type.GLAcct,
                                    ItemID = frgl.ID,
                                    Credit = _framount,
                                });
                                //
                                accountBalance.Add(new AccountBalance
                                {
                                    JEID = journalEntry.ID,
                                    PostingDate = purchaseMemo.PostingDate,
                                    Origin = docType.ID,
                                    OriginNo = purchaseMemo.Number,
                                    OffsetAccount = glAcc.Code,
                                    Details = douTypeID.Name + " - " + frgl.Code,
                                    CumulativeBalance = frgl.Balance,
                                    Credit = _framount,
                                    LocalSetRate = (decimal)purchaseMemo.LocalSetRate,
                                    GLAID = frgl.ID,
                                    Effective = EffectiveBlance.Credit
                                });
                            }
                            _context.Update(frgl);
                            _context.SaveChanges();
                        }
                        if (taxgacc.ID > 0)
                        {
                            var frtaxgljur = journalEntryDetail.FirstOrDefault(w => w.ItemID == taxgacc.ID) ?? new JournalEntryDetail();
                            var _frtaxamount = fr.TotalTaxAmount * (decimal)purchaseMemo.PurRate;
                            if (frtaxgljur.ItemID > 0)
                            {
                                var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == taxgacc.ID) ?? new AccountBalance();
                                taxgacc.Balance -= _frtaxamount;
                                //journalEntryDetail
                                frtaxgljur.Credit += _frtaxamount;
                                //accountBalance
                                accBalance.CumulativeBalance = frgl.Balance;
                                accBalance.Credit += _frtaxamount;
                            }
                            else
                            {
                                taxgacc.Balance -= _frtaxamount;
                                journalEntryDetail.Add(new JournalEntryDetail
                                {
                                    JEID = journalEntry.ID,
                                    Type = Financials.Type.GLAcct,
                                    ItemID = taxgacc.ID,
                                    Credit = _frtaxamount,
                                });
                                //
                                accountBalance.Add(new AccountBalance
                                {
                                    JEID = journalEntry.ID,
                                    PostingDate = purchaseMemo.PostingDate,
                                    Origin = docType.ID,
                                    OriginNo = purchaseMemo.Number,
                                    OffsetAccount = glAcc.Code,
                                    Details = douTypeID.Name + " - " + taxgacc.Code,
                                    CumulativeBalance = taxgacc.Balance,
                                    Credit = _frtaxamount,
                                    LocalSetRate = (decimal)purchaseMemo.LocalSetRate,
                                    GLAID = taxgacc.ID,
                                    Effective = EffectiveBlance.Credit
                                });
                            }
                            _context.Update(taxgacc);
                            _context.SaveChanges();
                        }
                    }
                }
            }
            #endregion
            if (copynote == 3)
            {
                foreach (var itemdt in memodetail)
                {
                    //var itemmaster = _context.ItemMasterDatas.FirstOrDefault(w => !w.Delete && w.ID == itemdt.ItemID);
                    //var ap = _context.Purchase_APs.FirstOrDefault(w => w.PurchaseAPID == itemdt.PurchaseAPID);                
                    var _itemAcc = _context.ItemAccountings.FirstOrDefault(i => i.ItemID == itemdt.ItemID && i.WarehouseID == purchaseMemo.WarehouseID);
                    List<ItemAccounting> itemAccs = new();
                    var item_master_data = _context.ItemMasterDatas.FirstOrDefault(w => w.ID == itemdt.ItemID);
                    if (item_master_data.SetGlAccount == Inventory.SetGlAccount.ItemLevel)
                    {
                        itemAccs = _context.ItemAccountings.Where(w => w.ItemID == itemdt.ItemID && w.WarehouseID == purchaseMemo.WarehouseID).ToList();
                        var invebtery = (from ia in itemAccs
                                         join gl in gLAccounts on ia.InventoryAccount equals gl.Code
                                         select gl).FirstOrDefault();
                        if (invebtery != null)
                        {
                            allocateAccID = invebtery.ID;
                            if (purchaseMemo.DiscountRate > 0)
                            {
                                decimal disvalue = (decimal)itemdt.TotalSys * (decimal)purchaseMemo.DiscountRate / 100;
                                inventoryAccAmount = (decimal)itemdt.TotalSys - disvalue;
                            }
                            else
                            {
                                inventoryAccAmount = (decimal)itemdt.TotalSys;
                            }
                        }
                    }
                    else if (item_master_data.SetGlAccount == Inventory.SetGlAccount.ItemGroup)
                    {
                        itemAccs = _context.ItemAccountings.Where(w => w.ItemGroupID == item_master_data.ItemGroup1ID).ToList();
                        var inventery = (from ia in itemAccs
                                         join gl in gLAccounts on ia.InventoryAccount equals gl.Code
                                         select gl
                                             ).FirstOrDefault();
                        if (inventery != null)
                        {
                            allocateAccID = inventery.ID;
                            if (purchaseMemo.DiscountRate > 0)
                            {
                                decimal disvalue = (decimal)itemdt.TotalSys * (decimal)purchaseMemo.DiscountRate / 100;
                                inventoryAccAmount = (decimal)itemdt.TotalSys - disvalue;
                            }
                            else
                            {
                                inventoryAccAmount = (decimal)itemdt.TotalSys;
                            }
                        }
                    }
                    //inventoryAccID
                    var glAccAllocate = _context.GLAccounts.FirstOrDefault(w => w.ID == allocateAccID);
                    var journalDetail = journalEntryDetail.FirstOrDefault(w => w.ItemID == allocateAccID) ?? new JournalEntryDetail();
                    if (journalDetail.ItemID > 0)
                    {
                        glAccAllocate.Balance += inventoryAccAmount * -1;
                        var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == allocateAccID);
                        //journalEntryDetail
                        journalDetail.Debit += inventoryAccAmount * -1;
                        //accountBalance
                        accBalance.CumulativeBalance = glAccAllocate.Balance;
                        accBalance.Debit += inventoryAccAmount * -1;
                    }
                    else
                    {
                        glAccAllocate.Balance += inventoryAccAmount;
                        journalEntryDetail.Add(new JournalEntryDetail
                        {
                            JEID = journalEntry.ID,
                            Type = Financials.Type.GLAcct,
                            ItemID = allocateAccID,
                            Debit = inventoryAccAmount * -1,
                        });
                        //
                        accountBalance.Add(new AccountBalance
                        {
                            JEID = journalEntry.ID,
                            PostingDate = purchaseMemo.PostingDate,
                            Origin = docType.ID,
                            OriginNo = purchaseMemo.Number,
                            OffsetAccount = glAcc.Code,
                            Details = douTypeID.Name + " - " + glAccAllocate.Code,
                            CumulativeBalance = glAccAllocate.Balance,
                            Debit = inventoryAccAmount * -1,
                            LocalSetRate = (decimal)purchaseMemo.LocalSetRate,
                            GLAID = allocateAccID,
                            Effective = EffectiveBlance.Debit
                        });
                        _context.Update(glAccAllocate);
                        _context.SaveChanges();
                    }
                    #region
                    //// Tax Account ///
                    var taxg = _context.TaxGroups.Find(itemdt.TaxGroupID) ?? new TaxGroup();
                    var taxAcc = _context.GLAccounts.Find(taxg.GLID) ?? new GLAccount();
                    decimal taxValue = itemdt.TaxOfFinDisValue * (decimal)purchaseMemo.PurRate;
                    if (taxAcc.ID > 0)
                    {
                        var taxjur = journalEntryDetail.FirstOrDefault(w => w.ItemID == taxAcc.ID) ?? new JournalEntryDetail();
                        if (taxjur.ItemID > 0)
                        {
                            var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == taxAcc.ID);
                            taxAcc.Balance += taxValue;
                            //journalEntryDetail
                            taxjur.Debit += taxValue;
                            //accountBalance
                            accBalance.CumulativeBalance = taxAcc.Balance;
                            accBalance.Debit += taxValue;
                        }
                        else
                        {
                            taxAcc.Balance += taxValue;
                            journalEntryDetail.Add(new JournalEntryDetail
                            {
                                JEID = journalEntry.ID,
                                Type = Financials.Type.GLAcct,
                                ItemID = taxAcc.ID,
                                Debit = taxValue,
                            });
                            //
                            accountBalance.Add(new AccountBalance
                            {
                                JEID = journalEntry.ID,
                                PostingDate = purchaseMemo.PostingDate,
                                Origin = docType.ID,
                                OriginNo = purchaseMemo.Number,
                                OffsetAccount = taxAcc.Code,
                                Details = douTypeID.Name + " - " + taxAcc.Code,
                                CumulativeBalance = taxAcc.Balance,
                                Debit = taxValue,
                                LocalSetRate = (decimal)purchaseMemo.LocalSetRate,
                                GLAID = taxAcc.ID,
                                Effective = EffectiveBlance.Debit
                            });
                        }
                        _context.Update(taxAcc);
                    }
                    #endregion
                    var orft = _context.GroupDUoMs.FirstOrDefault(w => w.GroupUoMID == item_master_data.GroupUomID && w.AltUOM == itemdt.UomID);
                    var itemWareSum = _context.WarehouseSummary.FirstOrDefault(w => w.WarehouseID == purchaseMemo.WarehouseID && w.ItemID == itemdt.ItemID);
                    var Cost = itemdt.PurchasPrice * purchaseMemo.PurRate;
                    var waredetials = _context.WarehouseDetails.Where(w => w.WarehouseID == purchaseMemo.WarehouseID && w.ItemID == itemdt.ItemID && w.Cost == Cost).ToList();
                    if (itemWareSum != null)
                    {
                        //WerehouseSummary
                        itemWareSum.InStock -= (double)itemdt.Qty * orft.Factor;
                        //Itemmasterdata
                        item_master_data.StockIn = itemWareSum.InStock - (double)itemdt.Qty;
                        _context.WarehouseSummary.Update(itemWareSum);
                        _context.ItemMasterDatas.Update(item_master_data);
                        _utility.UpdateItemAccounting(_itemAcc, itemWareSum);
                    }
                    double @Check_Stock;
                    double @Remain;
                    double @IssusQty;
                    double @FIFOQty;
                    double @Qty = itemdt.Qty * orft.Factor;
                    if (item_master_data.ManItemBy == ManageItemBy.SerialNumbers
                        && item_master_data.ManMethod == ManagementMethod.OnEveryTransation)
                    {
                        if (serials.Count > 0)
                        {
                            List<InventoryAudit> invens = new();
                            List<StockOut> stockOuts = new();
                            foreach (var s in serials)
                            {
                                if (s.APCSNSelected != null)
                                {
                                    foreach (var ss in s.APCSNSelected.APCSNDSelectedDetails)
                                    {
                                        var waredetial = waredetials
                                            .FirstOrDefault(i => ss.SerialNumber == i.SerialNumber && i.InStock > 0
                                                && i.Cost == itemdt.PurchasPrice * purchaseMemo.PurRate);
                                        if (waredetial != null)
                                        {
                                            waredetial.InStock -= 1;
                                            // Insert to warehouse detial //
                                            stockOuts.Add(new StockOut
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
                                                UomID = itemdt.UomID,
                                                UserID = purchaseMemo.UserID,
                                                ExpireDate = itemdt.ExpireDate,
                                                TransType = TransTypeWD.PurCreditMemo,
                                                TransID = purchaseMemo.PurchaseMemoID,
                                                OutStockFrom = purchaseMemo.PurchaseMemoID,
                                                FromWareDetialID = waredetial.ID,
                                            });
                                        }
                                    }
                                }
                            }
                            // Insert to InventoryAudit //
                            var inventory_audit = _context.InventoryAudits
                                .Where(w => w.ItemID == itemdt.ItemID && w.WarehouseID == purchaseMemo.WarehouseID);
                            var inven = new InventoryAudit
                            {
                                ID = 0,
                                WarehouseID = purchaseMemo.WarehouseID,
                                BranchID = purchaseMemo.BranchID,
                                UserID = purchaseMemo.UserID,
                                ItemID = itemdt.ItemID,
                                CurrencyID = purchaseMemo.CompanyID,
                                UomID = orft.BaseUOM,
                                InvoiceNo = purchaseMemo.Number,
                                Trans_Type = docType.Code,
                                Process = item_master_data.Process,
                                SystemDate = DateTime.Now,
                                PostingDate = purchaseMemo.PostingDate,
                                TimeIn = DateTime.Now.ToShortTimeString().ToString(),
                                Qty = @Qty * -1,
                                Cost = Cost,
                                Price = 0,
                                CumulativeQty = (inventory_audit.Sum(q => q.Qty) - @Qty),
                                CumulativeValue = (inventory_audit.Sum(s => s.Trans_Valuse)) - (@Qty * Cost),
                                Trans_Valuse = @Qty * Cost * -1,
                                ExpireDate = itemdt.ExpireDate,
                                LocalCurID = purchaseMemo.LocalCurID,
                                LocalSetRate = purchaseMemo.LocalSetRate,
                                CompanyID = purchaseMemo.CompanyID,
                                DocumentTypeID = docType.ID,
                                SeriesID = purchaseMemo.SeriesID,
                                SeriesDetailID = purchaseMemo.SeriesDetailID,
                            };
                            _context.InventoryAudits.Add(inven);
                            _context.StockOuts.AddRange(stockOuts);
                            _context.SaveChanges();
                        }
                    }
                    else if (item_master_data.ManItemBy == ManageItemBy.Batches
                        && item_master_data.ManMethod == ManagementMethod.OnEveryTransation)
                    {
                        if (batches.Count > 0)
                        {
                            foreach (var b in batches)
                            {
                                if (b.APCBatchNoSelected != null)
                                {
                                    foreach (var sb in b.APCBatchNoSelected.APCBatchNoSelectedDetails)
                                    {
                                        var waredetial = waredetials
                                            .FirstOrDefault(i =>
                                                sb.BatchNo == i.BatchNo
                                                && i.InStock > 0
                                                && i.Cost == itemdt.PurchasPrice * purchaseMemo.PurRate);
                                        if (waredetial != null)
                                        {
                                            waredetial.InStock -= (double)sb.SelectedQty;
                                            _context.SaveChanges();
                                            // insert to waredetial
                                            var stockOuts = new StockOut
                                            {
                                                AdmissionDate = waredetial.AdmissionDate,
                                                Cost = (decimal)waredetial.Cost,
                                                CurrencyID = waredetial.CurrencyID,
                                                Details = waredetial.Details,
                                                ID = 0,
                                                InStock = sb.SelectedQty,
                                                ItemID = itemdt.ItemID,
                                                Location = waredetial.Location,
                                                MfrDate = waredetial.MfrDate,
                                                ProcessItem = ProcessItem.SEBA,
                                                SyetemDate = DateTime.Now,
                                                SysNum = 0,
                                                TimeIn = DateTime.Now,
                                                WarehouseID = waredetial.WarehouseID,
                                                UomID = itemdt.UomID,
                                                UserID = purchaseMemo.UserID,
                                                ExpireDate = itemdt.ExpireDate,
                                                BatchAttr1 = waredetial.BatchAttr1,
                                                BatchAttr2 = waredetial.BatchAttr2,
                                                BatchNo = waredetial.BatchNo,
                                                TransType = TransTypeWD.PurCreditMemo,
                                                TransID = purchaseMemo.PurchaseMemoID,
                                                FromWareDetialID = waredetial.ID,
                                                OutStockFrom = purchaseMemo.PurCurrencyID,
                                            };
                                            _context.StockOuts.Add(stockOuts);
                                            _context.SaveChanges();
                                        }
                                    }
                                }
                            }
                            // insert to inventory audit
                            var inventory_audit = _context.InventoryAudits
                                .Where(w => w.ItemID == itemdt.ItemID && w.WarehouseID == purchaseMemo.WarehouseID);
                            var invens = new InventoryAudit
                            {
                                ID = 0,
                                WarehouseID = purchaseMemo.WarehouseID,
                                BranchID = purchaseMemo.BranchID,
                                UserID = purchaseMemo.UserID,
                                ItemID = itemdt.ItemID,
                                CurrencyID = purchaseMemo.CompanyID,
                                UomID = orft.BaseUOM,
                                InvoiceNo = purchaseMemo.Number,
                                Trans_Type = docType.Code,
                                Process = item_master_data.Process,
                                SystemDate = DateTime.Now,
                                PostingDate = purchaseMemo.PostingDate,
                                TimeIn = DateTime.Now.ToShortTimeString().ToString(),
                                Qty = @Qty * -1,
                                Cost = Cost,
                                Price = 0,
                                CumulativeQty = inventory_audit.Sum(q => q.Qty) - @Qty,
                                CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) - (@Qty * Cost),
                                Trans_Valuse = @Qty * Cost * -1,
                                ExpireDate = itemdt.ExpireDate,
                                LocalCurID = purchaseMemo.LocalCurID,
                                LocalSetRate = purchaseMemo.LocalSetRate,
                                CompanyID = purchaseMemo.CompanyID,
                                DocumentTypeID = docType.ID,
                                SeriesID = purchaseMemo.SeriesID,
                                SeriesDetailID = purchaseMemo.SeriesDetailID,
                            };
                            _context.InventoryAudits.Add(invens);
                            _context.SaveChanges();
                        }
                    }
                    else
                    {
                        foreach (var item_warehouse in waredetials.Where(w => w.InStock > 0))
                        {
                            var item_inventory_audit = new InventoryAudit();
                            var item_IssusStock = waredetials.FirstOrDefault(w => w.InStock > 0);
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
                                if (item_master_data.Process == "FIFO")
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
                                            ItemID = itemdt.ItemID,
                                            ProcessItem = ProcessItem.FIFO,
                                            SyetemDate = DateTime.Now,
                                            TimeIn = DateTime.Now,
                                            WarehouseID = item_warehouse.WarehouseID,
                                            UomID = itemdt.UomID,
                                            UserID = purchaseMemo.UserID,
                                            ExpireDate = itemdt.ExpireDate,
                                            TransType = TransTypeWD.PurCreditMemo,
                                        };
                                        _context.StockOuts.Add(stockOuts);
                                        var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == itemdt.ItemID && w.WarehouseID == purchaseMemo.WarehouseID);
                                        item_inventory_audit.ID = 0;
                                        item_inventory_audit.WarehouseID = purchaseMemo.WarehouseID;
                                        item_inventory_audit.BranchID = purchaseMemo.BranchID;
                                        item_inventory_audit.UserID = purchaseMemo.UserID;
                                        item_inventory_audit.ItemID = itemdt.ItemID;
                                        item_inventory_audit.CurrencyID = purchaseMemo.CompanyID;
                                        item_inventory_audit.UomID = orft.BaseUOM;
                                        item_inventory_audit.InvoiceNo = purchaseMemo.Number;
                                        item_inventory_audit.Trans_Type = docType.Code;
                                        item_inventory_audit.Process = item_master_data.Process;
                                        item_inventory_audit.SystemDate = DateTime.Now;
                                        item_inventory_audit.PostingDate = purchaseMemo.PostingDate;
                                        item_inventory_audit.TimeIn = DateTime.Now.ToShortTimeString().ToString();
                                        item_inventory_audit.Qty = @IssusQty * -1;
                                        item_inventory_audit.Cost = item_IssusStock.Cost;
                                        item_inventory_audit.Price = 0;
                                        item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) - @IssusQty;
                                        item_inventory_audit.CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) - (@IssusQty * item_IssusStock.Cost);
                                        item_inventory_audit.Trans_Valuse = @IssusQty * item_IssusStock.Cost * -1;
                                        item_inventory_audit.ExpireDate = item_IssusStock.ExpireDate;
                                        item_inventory_audit.LocalCurID = purchaseMemo.LocalCurID;
                                        item_inventory_audit.LocalSetRate = purchaseMemo.LocalSetRate;
                                        item_inventory_audit.CompanyID = purchaseMemo.CompanyID;
                                        item_inventory_audit.DocumentTypeID = docType.ID;
                                        item_inventory_audit.SeriesID = purchaseMemo.SeriesID;
                                        item_inventory_audit.SeriesDetailID = purchaseMemo.SeriesDetailID;
                                    }
                                    _utility.CumulativeValue(item_inventory_audit.WarehouseID, item_inventory_audit.ItemID, item_inventory_audit.CumulativeValue, _itemAcc);
                                    _context.WarehouseDetails.Update(item_IssusStock);
                                    _context.InventoryAudits.Add(item_inventory_audit);
                                    _context.SaveChanges();
                                }
                                else if (item_master_data.Process == "Average")
                                {
                                    item_IssusStock.InStock = item_IssusStock.InStock -= @IssusQty;
                                    if (@IssusQty > 0)
                                    {
                                        var warehouse_summary = _context.WarehouseSummary.FirstOrDefault(w => w.ItemID == itemdt.ItemID && w.WarehouseID == purchaseMemo.WarehouseID);
                                        double @sysAvCost = warehouse_summary.Cost;
                                        var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == itemdt.ItemID && w.UomID == itemdt.UomID && w.WarehouseID == purchaseMemo.WarehouseID);
                                        double cost = (inventory_audit.Sum(s => s.Trans_Valuse) - @IssusQty * @sysAvCost / (inventory_audit.Sum(q => q.Qty) - @IssusQty));
                                        cost = _utility.CheckNaNOrInfinity(cost);
                                        var stockOuts = new StockOut
                                        {
                                            Cost = (decimal)cost,
                                            CurrencyID = item_warehouse.CurrencyID,
                                            ID = 0,
                                            InStock = (decimal)@IssusQty,
                                            ItemID = itemdt.ItemID,
                                            ProcessItem = ProcessItem.Average,
                                            SyetemDate = DateTime.Now,
                                            TimeIn = DateTime.Now,
                                            WarehouseID = item_warehouse.WarehouseID,
                                            UomID = itemdt.UomID,
                                            UserID = purchaseMemo.UserID,
                                            ExpireDate = itemdt.ExpireDate,
                                            TransType = TransTypeWD.PurCreditMemo,
                                        };
                                        _context.StockOuts.Add(stockOuts);

                                        item_inventory_audit.ID = 0;
                                        item_inventory_audit.WarehouseID = purchaseMemo.WarehouseID;
                                        item_inventory_audit.BranchID = purchaseMemo.BranchID;
                                        item_inventory_audit.UserID = purchaseMemo.UserID;
                                        item_inventory_audit.ItemID = itemdt.ItemID;
                                        item_inventory_audit.CurrencyID = purchaseMemo.CompanyID;
                                        item_inventory_audit.UomID = orft.BaseUOM;
                                        item_inventory_audit.InvoiceNo = purchaseMemo.Number;
                                        item_inventory_audit.Trans_Type = docType.Code;
                                        item_inventory_audit.Process = item_master_data.Process;
                                        item_inventory_audit.SystemDate = DateTime.Now;
                                        item_inventory_audit.PostingDate = purchaseMemo.PostingDate;
                                        item_inventory_audit.TimeIn = DateTime.Now.ToShortTimeString().ToString();
                                        item_inventory_audit.Qty = @IssusQty * -1;
                                        item_inventory_audit.Cost = cost;
                                        item_inventory_audit.Price = 0;
                                        item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) - @IssusQty;
                                        item_inventory_audit.CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) - (@IssusQty * @sysAvCost);
                                        item_inventory_audit.Trans_Valuse = @IssusQty * @sysAvCost * (-1);
                                        item_inventory_audit.ExpireDate = item_IssusStock.ExpireDate;
                                        item_inventory_audit.LocalCurID = purchaseMemo.LocalCurID;
                                        item_inventory_audit.LocalSetRate = purchaseMemo.LocalSetRate;
                                        item_inventory_audit.CompanyID = purchaseMemo.CompanyID;
                                        item_inventory_audit.DocumentTypeID = docType.ID;
                                        item_inventory_audit.SeriesID = purchaseMemo.SeriesID;
                                        item_inventory_audit.SeriesDetailID = purchaseMemo.SeriesDetailID;
                                    }
                                    var inventoryAcct = _context.InventoryAudits.Where(w => w.ItemID == itemdt.ItemID);
                                    double @AvgCost = ((inventoryAcct.Sum(s => s.Trans_Valuse) + item_inventory_audit.Cost) / (inventoryAcct.Sum(q => q.Qty) + item_inventory_audit.Qty));
                                    @AvgCost = _utility.CheckNaNOrInfinity(@AvgCost);
                                    _utility.UpdateAvgCost(item_warehouse.ItemID, purchaseMemo.WarehouseID, item_master_data.GroupUomID, item_inventory_audit.Qty, item_inventory_audit.Cost);
                                    _utility.UpdateBomCost(item_warehouse.ItemID, item_inventory_audit.Qty, item_inventory_audit.Cost);
                                    _utility.CumulativeValue(item_inventory_audit.WarehouseID, item_inventory_audit.ItemID, item_inventory_audit.CumulativeValue, _itemAcc);
                                    _context.WarehouseDetails.Update(item_IssusStock);
                                    _context.InventoryAudits.Add(item_inventory_audit);
                                    _context.SaveChanges();
                                }
                            }
                            else
                            {
                                @FIFOQty = item_IssusStock.InStock - @Qty;
                                @IssusQty = item_IssusStock.InStock - @FIFOQty;
                                if (item_master_data.Process == "FIFO")
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
                                            ItemID = itemdt.ItemID,
                                            ProcessItem = ProcessItem.FIFO,
                                            SyetemDate = DateTime.Now,
                                            TimeIn = DateTime.Now,
                                            WarehouseID = item_warehouse.WarehouseID,
                                            UomID = itemdt.UomID,
                                            UserID = purchaseMemo.UserID,
                                            ExpireDate = itemdt.ExpireDate,
                                            TransType = TransTypeWD.PurCreditMemo,
                                        };
                                        _context.StockOuts.Add(stockOuts);

                                        var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == itemdt.ItemID && w.UomID == itemdt.UomID && w.WarehouseID == purchaseMemo.WarehouseID);
                                        item_inventory_audit.ID = 0;
                                        item_inventory_audit.WarehouseID = purchaseMemo.WarehouseID;
                                        item_inventory_audit.BranchID = purchaseMemo.BranchID;
                                        item_inventory_audit.UserID = purchaseMemo.UserID;
                                        item_inventory_audit.ItemID = itemdt.ItemID;
                                        item_inventory_audit.CurrencyID = purchaseMemo.CompanyID;
                                        item_inventory_audit.UomID = orft.BaseUOM;
                                        item_inventory_audit.InvoiceNo = purchaseMemo.Number;
                                        item_inventory_audit.Trans_Type = docType.Code;
                                        item_inventory_audit.Process = item_master_data.Process;
                                        item_inventory_audit.SystemDate = DateTime.Now;
                                        item_inventory_audit.PostingDate = purchaseMemo.PostingDate;
                                        item_inventory_audit.TimeIn = DateTime.Now.ToShortTimeString().ToString();
                                        item_inventory_audit.Qty = @IssusQty * -1;
                                        item_inventory_audit.Cost = item_IssusStock.Cost;
                                        item_inventory_audit.Price = 0;
                                        item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) - @IssusQty;
                                        item_inventory_audit.CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) - (@IssusQty * item_IssusStock.Cost);
                                        item_inventory_audit.Trans_Valuse = @IssusQty * item_IssusStock.Cost * (-1);
                                        item_inventory_audit.ExpireDate = item_IssusStock.ExpireDate;
                                        item_inventory_audit.LocalCurID = purchaseMemo.LocalCurID;
                                        item_inventory_audit.LocalSetRate = purchaseMemo.LocalSetRate;
                                        item_inventory_audit.CompanyID = purchaseMemo.CompanyID;
                                        item_inventory_audit.DocumentTypeID = docType.ID;
                                        item_inventory_audit.SeriesID = purchaseMemo.SeriesID;
                                        item_inventory_audit.SeriesDetailID = purchaseMemo.SeriesDetailID;
                                    }
                                    _utility.CumulativeValue(item_inventory_audit.WarehouseID, item_inventory_audit.ItemID, item_inventory_audit.CumulativeValue, _itemAcc);
                                    _context.WarehouseDetails.Update(item_IssusStock);
                                    _context.InventoryAudits.Add(item_inventory_audit);
                                    _context.SaveChanges();
                                }
                                else if (item_master_data.Process == "Average")
                                {
                                    item_IssusStock.InStock = @FIFOQty;
                                    if (@IssusQty > 0)
                                    {
                                        var warehouse_summary = _context.WarehouseSummary.FirstOrDefault(w => w.ItemID == itemdt.ItemID && w.WarehouseID == purchaseMemo.WarehouseID);
                                        double @sysAvCost = warehouse_summary.Cost;
                                        var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == itemdt.ItemID && w.UomID == itemdt.UomID && w.WarehouseID == purchaseMemo.WarehouseID);
                                        double cost = (inventory_audit.Sum(s => s.Trans_Valuse) + @IssusQty * @sysAvCost * (-1)) / (inventory_audit.Sum(q => q.Qty) + (@IssusQty * (-1)));
                                        cost = _utility.CheckNaNOrInfinity(cost);
                                        var stockOuts = new StockOut
                                        {
                                            Cost = (decimal)cost,
                                            CurrencyID = item_warehouse.CurrencyID,
                                            ID = 0,
                                            InStock = (decimal)@IssusQty,
                                            ItemID = itemdt.ItemID,
                                            ProcessItem = ProcessItem.Average,
                                            SyetemDate = DateTime.Now,
                                            TimeIn = DateTime.Now,
                                            WarehouseID = item_warehouse.WarehouseID,
                                            UomID = itemdt.UomID,
                                            UserID = purchaseMemo.UserID,
                                            ExpireDate = itemdt.ExpireDate,
                                            TransType = TransTypeWD.PurCreditMemo,
                                        };
                                        _context.StockOuts.Add(stockOuts);
                                        item_inventory_audit.ID = 0;
                                        item_inventory_audit.WarehouseID = purchaseMemo.WarehouseID;
                                        item_inventory_audit.BranchID = purchaseMemo.BranchID;
                                        item_inventory_audit.UserID = purchaseMemo.UserID;
                                        item_inventory_audit.ItemID = itemdt.ItemID;
                                        item_inventory_audit.CurrencyID = purchaseMemo.CompanyID;
                                        item_inventory_audit.UomID = orft.BaseUOM;
                                        item_inventory_audit.InvoiceNo = purchaseMemo.Number;
                                        item_inventory_audit.Trans_Type = docType.Code;
                                        item_inventory_audit.Process = item_master_data.Process;
                                        item_inventory_audit.SystemDate = DateTime.Now;
                                        item_inventory_audit.PostingDate = purchaseMemo.PostingDate;
                                        item_inventory_audit.TimeIn = DateTime.Now.ToShortTimeString().ToString();
                                        item_inventory_audit.Qty = @IssusQty * -1;
                                        item_inventory_audit.Cost = cost;
                                        item_inventory_audit.Price = 0;
                                        item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) - @IssusQty;
                                        item_inventory_audit.CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) - (@IssusQty * @sysAvCost);
                                        item_inventory_audit.Trans_Valuse = @IssusQty * @sysAvCost * -1;
                                        item_inventory_audit.ExpireDate = item_IssusStock.ExpireDate;
                                        item_inventory_audit.LocalCurID = purchaseMemo.LocalCurID;
                                        item_inventory_audit.LocalSetRate = purchaseMemo.LocalSetRate;
                                        item_inventory_audit.CompanyID = purchaseMemo.CompanyID;
                                        item_inventory_audit.DocumentTypeID = docType.ID;
                                        item_inventory_audit.SeriesID = purchaseMemo.SeriesID;
                                        item_inventory_audit.SeriesDetailID = purchaseMemo.SeriesDetailID;
                                    }
                                    var inventoryAcct = _context.InventoryAudits.Where(w => w.ItemID == itemdt.ItemID);
                                    _utility.UpdateAvgCost(item_warehouse.ItemID, purchaseMemo.WarehouseID, item_master_data.GroupUomID, item_inventory_audit.Qty, item_inventory_audit.Cost);
                                    _utility.UpdateBomCost(item_warehouse.ItemID, item_inventory_audit.Qty, item_inventory_audit.Cost);
                                    _utility.CumulativeValue(item_inventory_audit.WarehouseID, item_inventory_audit.ItemID, item_inventory_audit.CumulativeValue, _itemAcc);
                                    _context.WarehouseDetails.Update(item_IssusStock);
                                    _context.InventoryAudits.Add(item_inventory_audit);
                                    _context.SaveChanges();
                                }
                                waredetials = new List<WarehouseDetail>();
                                break;
                            }
                        }
                    }
                }
            }

            if (copynote != 3)
            {
                foreach (var itemdt in memodetail)
                {
                    var itemmaster = _context.ItemMasterDatas.FirstOrDefault(w => !w.Delete && w.ID == itemdt.ItemID);
                    //var ap = _context.Purchase_APs.FirstOrDefault(w => w.PurchaseAPID == itemdt.PurchaseAPID);                
                    var _itemAcc = _context.ItemAccountings.FirstOrDefault(i => i.ItemID == itemdt.ItemID && i.WarehouseID == purchaseMemo.WarehouseID);
                    List<ItemAccounting> itemAccs = new();
                    var item_master_data = _context.ItemMasterDatas.FirstOrDefault(w => w.ID == itemdt.ItemID);
                    if (item_master_data.SetGlAccount == Inventory.SetGlAccount.ItemLevel)
                    {
                        itemAccs = _context.ItemAccountings.Where(w => w.ItemID == itemdt.ItemID && w.WarehouseID == purchaseMemo.WarehouseID).ToList();
                        var allocateAcc = (from ia in itemAccs
                                           join gl in gLAccounts on ia.AllocationAccount equals gl.Code
                                           select gl).FirstOrDefault();
                        if (allocateAcc != null)
                        {
                            allocateAccID = allocateAcc.ID;
                            if (purchaseMemo.DiscountRate > 0)
                            {
                                decimal disvalue = (decimal)itemdt.TotalSys * (decimal)purchaseMemo.DiscountRate / 100;
                                allocateAccAmount = (decimal)itemdt.TotalSys - disvalue;
                            }
                            else
                            {
                                allocateAccAmount = (decimal)itemdt.TotalSys;
                            }
                        }
                    }
                    else if (item_master_data.SetGlAccount == Inventory.SetGlAccount.ItemGroup)
                    {
                        itemAccs = _context.ItemAccountings.Where(w => w.ItemGroupID == item_master_data.ItemGroup1ID).ToList();
                        var allocateAcc = (from ia in itemAccs
                                           join gl in gLAccounts on ia.AllocationAccount equals gl.Code
                                           select gl
                                             ).FirstOrDefault();
                        if (allocateAcc != null)
                        {
                            allocateAccID = allocateAcc.ID;
                            if (purchaseMemo.DiscountRate > 0)
                            {
                                decimal disvalue = (decimal)itemdt.TotalSys * (decimal)purchaseMemo.DiscountRate / 100;
                                allocateAccAmount = (decimal)itemdt.TotalSys - disvalue;
                            }
                            else
                            {
                                allocateAccAmount = (decimal)itemdt.TotalSys;
                            }
                        }
                    }
                    //inventoryAccID
                    var glAccAllocate = _context.GLAccounts.FirstOrDefault(w => w.ID == allocateAccID);
                    var journalDetail = journalEntryDetail.FirstOrDefault(w => w.ItemID == allocateAccID) ?? new JournalEntryDetail();
                    if (journalDetail.ItemID > 0)
                    {
                        glAccAllocate.Balance += allocateAccAmount * -1;
                        var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == allocateAccID);
                        //journalEntryDetail
                        journalDetail.Debit += allocateAccAmount * -1;
                        //accountBalance
                        accBalance.CumulativeBalance = glAccAllocate.Balance;
                        accBalance.Debit += allocateAccAmount * -1;
                    }
                    else
                    {
                        glAccAllocate.Balance += allocateAccAmount;
                        journalEntryDetail.Add(new JournalEntryDetail
                        {
                            JEID = journalEntry.ID,
                            Type = Financials.Type.GLAcct,
                            ItemID = allocateAccID,
                            Debit = allocateAccAmount * -1,
                        });
                        //
                        accountBalance.Add(new AccountBalance
                        {
                            JEID = journalEntry.ID,
                            PostingDate = purchaseMemo.PostingDate,
                            Origin = docType.ID,
                            OriginNo = purchaseMemo.Number,
                            OffsetAccount = glAcc.Code,
                            Details = douTypeID.Name + " - " + glAccAllocate.Code,
                            CumulativeBalance = glAccAllocate.Balance,
                            Debit = allocateAccAmount * -1,
                            LocalSetRate = (decimal)purchaseMemo.LocalSetRate,
                            GLAID = allocateAccID,
                            Effective = EffectiveBlance.Debit
                        });
                        _context.Update(glAccAllocate);
                        _context.SaveChanges();
                    }

                    #region
                    //// Tax Account ///
                    var taxg = _context.TaxGroups.Find(itemdt.TaxGroupID) ?? new TaxGroup();
                    var taxAcc = _context.GLAccounts.Find(taxg.GLID) ?? new GLAccount();
                    decimal taxValue = itemdt.TaxOfFinDisValue * (decimal)purchaseMemo.PurRate;
                    if (taxAcc.ID > 0)
                    {
                        var taxjur = journalEntryDetail.FirstOrDefault(w => w.ItemID == taxAcc.ID) ?? new JournalEntryDetail();
                        if (taxjur.ItemID > 0)
                        {
                            var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == taxAcc.ID);
                            taxAcc.Balance += taxValue;
                            //journalEntryDetail
                            taxjur.Debit += taxValue;
                            //accountBalance
                            accBalance.CumulativeBalance = taxAcc.Balance;
                            accBalance.Debit += taxValue;
                        }
                        else
                        {
                            taxAcc.Balance += taxValue;
                            journalEntryDetail.Add(new JournalEntryDetail
                            {
                                JEID = journalEntry.ID,
                                Type = Financials.Type.GLAcct,
                                ItemID = taxAcc.ID,
                                Debit = taxValue,
                            });
                            //
                            accountBalance.Add(new AccountBalance
                            {
                                JEID = journalEntry.ID,
                                PostingDate = purchaseMemo.PostingDate,
                                Origin = docType.ID,
                                OriginNo = purchaseMemo.Number,
                                OffsetAccount = taxAcc.Code,
                                Details = douTypeID.Name + " - " + taxAcc.Code,
                                CumulativeBalance = taxAcc.Balance,
                                Debit = taxValue,
                                LocalSetRate = (decimal)purchaseMemo.LocalSetRate,
                                GLAID = taxAcc.ID,
                                Effective = EffectiveBlance.Debit
                            });
                        }
                        _context.Update(taxAcc);
                    }
                    #endregion

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
        public IEnumerable<ReportPurchasCreditMemo> ReportPurchaseCreditMemo(int BranchID, int WarehoseID, string PostingDate, string DocumentDate, string DeliveryDate, string Check) => _context.ReportPurchasCreditMemos.FromSql("sp_ReportPurchaseCreditMemo @BranchID={0},@WarehouseID={1},@PostingDate={2},@DocumentDate={3},@DeliveryDate={4},@Check={5}",
            parameters: new[] {
                BranchID.ToString(),
                WarehoseID.ToString(),
                PostingDate.ToString(),
                DocumentDate.ToString(),
                DeliveryDate.ToString(),
                Check.ToString()
            });
    }
}
