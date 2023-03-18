using Microsoft.EntityFrameworkCore;
using CKBS.AppContext;
using CKBS.Models.ClassCopy;
using CKBS.Models.ReportClass;
using CKBS.Models.Services.Administrator.Inventory;
using CKBS.Models.Services.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using CKBS.Models.Services.Administrator.SystemInitialization;
using CKBS.Models.Services.Financials;
using SetGlAccount = CKBS.Models.Services.Inventory.SetGlAccount;
using CKBS.Models.Services.ChartOfAccounts;
using KEDI.Core.Premise.Models.ServicesClass.SerialBatches.Purchases;
using KEDI.Core.Premise.Repository;

namespace CKBS.Models.Services.Responsitory
{
    public interface IGoodRecepitPo
    {
        void GoodReceiptStockPO(
            int purchaseID,
            string Type,
            List<SerialViewModelPurchase> serialViewModelPurchases,
            List<BatchViewModelPurchase> batchViewModelPurchases);
        IEnumerable<ReportPurchaseOrder> GetAllPruchaseOrder(int BranchID);
        IEnumerable<PurchaseAP_To_PurchaseMemo> GetPurchaseAP_From_PurchaseOrder(int ID, string Invoice);
        IEnumerable<ReportPurchaseAP> GetReportGoodReceiptPO(int BranchID, int WarehouseID, string PostingDate, string DocumentDate, string DeDate, string Check);
    }
    public class GoodReceiptPoResponsitory : IGoodRecepitPo
    {
        private readonly DataContext _context;
        private readonly UtilityModule _utility;

        public GoodReceiptPoResponsitory(DataContext context, UtilityModule utility)
        {
            _context = context;
            _utility = utility;
        }

        public void GoodReceiptStockPO(int purchaseID,
            string Type,
            List<SerialViewModelPurchase> serialViewModelPurchases,
            List<BatchViewModelPurchase> batchViewModelPurchases)
        {
            JournalEntry journalEntry = new();
            SeriesDetail seriesDetail = new();
            List<JournalEntryDetail> journalEntryDetail = new();
            List<AccountBalance> accountBalance = new();
            List<GLAccount> gLAccounts = _context.GLAccounts.Where(i => i.IsActive).ToList();
            var po = _context.GoodsReciptPOs.FirstOrDefault(w => w.ID == purchaseID);
            var series = _context.Series.Find(po.SeriesID);
            var ItemPO = _context.GoodReciptPODetails.Where(w => w.GoodsReciptPOID == purchaseID);
            var douTypeID = _context.DocumentTypes.FirstOrDefault(w => w.Code == "JE");
            var docType = _context.DocumentTypes.FirstOrDefault(w => w.Code == "PD");
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
                journalEntry.Creator = po.UserID;
                 journalEntry.BranchID = po.BranchID;
                journalEntry.TransNo = po.Number;
                journalEntry.PostingDate = po.PostingDate;
                journalEntry.DocumentDate = po.DocumentDate;
                journalEntry.DueDate = po.DueDate;
                journalEntry.SSCID = po.SysCurrencyID;
                journalEntry.LLCID = po.LocalCurID;
                journalEntry.CompanyID = po.CompanyID;
                journalEntry.LocalSetRate = (decimal)po.LocalSetRate;
                journalEntry.SeriesDID = seriesDetail.ID;
                journalEntry.Remarks = series.Name + "-" + po.Number;
                _context.Update(journalEntry);
            }
            _context.SaveChanges();
            var accountPayable = _context.BusinessPartners.FirstOrDefault(w => w.ID == po.VendorID);
            var glAcc = _context.GLAccounts.FirstOrDefault(w => w.ID == accountPayable.GLAccID) ?? new GLAccount();

            //insert inventoryaudit
            foreach (var itemdt in ItemPO)
            {

                var itemmaster = _context.ItemMasterDatas.FirstOrDefault(w => !w.Delete && w.ID == itemdt.ItemID);
                //var po = _context.GoodsReciptPOs.FirstOrDefault(w => w.ID == itemdt.GoodsReciptPOID);
                var gd = _context.GroupDUoMs.FirstOrDefault(W => W.GroupUoMID == itemmaster.GroupUomID && W.AltUOM == itemdt.UomID);
                var orft = _context.GroupDUoMs.FirstOrDefault(w => w.GroupUoMID == itemmaster.GroupUomID);
                var warehouse = _context.WarehouseSummary.FirstOrDefault(w => w.ItemID == itemdt.ItemID && w.WarehouseID == po.WarehouseID);
                ItemAccounting _itemAcc = _context.ItemAccountings.FirstOrDefault(i => i.ItemID == itemdt.ItemID && i.WarehouseID == po.WarehouseID);
                List<ItemAccounting> itemAccs = new();
                double @Qty = itemdt.Qty * gd.Factor;
                double _cost = (itemdt.PurchasPrice / gd.Factor) * po.PurRate;
                InventoryAudit item_inventory_audit = new();
                WarehouseDetail warehousedetail = new();
                //
                int allocationID = 0, inventoryAccID = 0;
                decimal allocationAmount = 0;

                if (itemmaster.SetGlAccount == SetGlAccount.ItemLevel)
                {
                    itemAccs = _context.ItemAccountings.Where(w => w.ItemID == itemdt.ItemID && w.WarehouseID == po.WarehouseID).ToList();
                    var allocationacc = (from ia in itemAccs
                                         join gl in gLAccounts on ia.AllocationAccount equals gl.Code
                                         select gl
                                         ).FirstOrDefault();
                    var inventoryAcc = (from ia in itemAccs
                                        join gl in gLAccounts on ia.InventoryAccount equals gl.Code
                                        select gl
                                         ).FirstOrDefault();
                    if (po.DiscountRate > 0)
                    {
                        decimal disvalue = (decimal)itemdt.TotalSys * (decimal)po.DiscountRate / 100;
                        allocationAmount = (decimal)itemdt.TotalSys - disvalue;
                    }
                    else
                    {
                        allocationAmount = (decimal)itemdt.TotalSys;
                    }
                    allocationID = allocationacc.ID;
                    inventoryAccID = inventoryAcc.ID;
                }
                else if (itemmaster.SetGlAccount == SetGlAccount.ItemGroup)
                {
                    itemAccs = _context.ItemAccountings.Where(w => w.ItemGroupID == itemmaster.ItemGroup1ID).ToList();
                    var allocationacc = (from ia in itemAccs
                                         join gl in gLAccounts on ia.AllocationAccount equals gl.Code
                                         select gl
                                         ).FirstOrDefault();
                    var inventoryAcc = (from ia in itemAccs
                                        join gl in gLAccounts on ia.InventoryAccount equals gl.Code
                                        select gl
                                         ).FirstOrDefault();
                    if (po.DiscountRate > 0)
                    {
                        decimal disvalue = (decimal)itemdt.TotalSys * (decimal)po.DiscountRate / 100;
                        allocationAmount = (decimal)itemdt.TotalSys - disvalue;
                    }
                    else
                    {
                        allocationAmount = (decimal)itemdt.TotalSys;
                    }
                    allocationID = allocationacc.ID;
                    inventoryAccID = inventoryAcc.ID;
                }
                //inventoryAccID
                var glAccInvenfifo = _context.GLAccounts.FirstOrDefault(w => w.ID == inventoryAccID) ?? new GLAccount();
                if (glAccInvenfifo.ID > 0)
                {
                    var journalDetail = journalEntryDetail.FirstOrDefault(w => w.ItemID == glAccInvenfifo.ID) ?? new JournalEntryDetail();
                    if (journalDetail.ItemID > 0)
                    {
                        var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == inventoryAccID);
                        glAccInvenfifo.Balance += allocationAmount;
                        //journalEntryDetail
                        journalDetail.Debit += allocationAmount;
                        //accountBalance
                        accBalance.CumulativeBalance = glAccInvenfifo.Balance;
                        accBalance.Debit += allocationAmount;
                    }
                    else
                    {
                        glAccInvenfifo.Balance += allocationAmount;
                        journalEntryDetail.Add(new JournalEntryDetail
                        {
                            JEID = journalEntry.ID,
                            Type = Financials.Type.GLAcct,
                            ItemID = inventoryAccID,
                            Debit = allocationAmount,
                        });
                        //
                        accountBalance.Add(new AccountBalance
                        {
                            JEID = journalEntry.ID,
                            PostingDate = po.PostingDate,
                            Origin = docType.ID,
                            OriginNo = po.Number,
                            OffsetAccount = glAccInvenfifo.Code,
                            Details = douTypeID.Name + "-" + glAccInvenfifo.Code,
                            CumulativeBalance = glAccInvenfifo.Balance,
                            Debit = allocationAmount,
                            LocalSetRate = (decimal)po.LocalSetRate,
                            GLAID = inventoryAccID,
                            Effective = EffectiveBlance.Debit
                        });
                    }
                    _context.GLAccounts.Update(glAccInvenfifo);
                }
                //AllocationAccount
                var allocation = _context.GLAccounts.FirstOrDefault(w => w.ID == allocationID) ?? new GLAccount();
                if (allocation.ID > 0)
                {
                    var journalDetail = journalEntryDetail.FirstOrDefault(w => w.ItemID == allocation.ID) ?? new JournalEntryDetail();
                    if (journalDetail.ItemID > 0)
                    {
                        var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == allocationID);
                        allocation.Balance -= allocationAmount;
                        //journalEntryDetail
                        journalDetail.Credit += allocationAmount;
                        //accountBalance
                        accBalance.CumulativeBalance = allocation.Balance;
                        accBalance.Credit += allocationAmount;
                    }
                    else
                    {
                        allocation.Balance -= allocationAmount;
                        journalEntryDetail.Add(new JournalEntryDetail
                        {
                            JEID = journalEntry.ID,
                            Type = Financials.Type.GLAcct,
                            ItemID = allocationID,
                            Credit = allocationAmount,
                        });
                        //
                        accountBalance.Add(new AccountBalance
                        {
                            JEID = journalEntry.ID,
                            PostingDate = po.PostingDate,
                            Origin = docType.ID,
                            OriginNo = po.Number,
                            OffsetAccount = allocation.Code,
                            Details = douTypeID.Name + "-" + allocation.Code,
                            CumulativeBalance = allocation.Balance,
                            Credit = allocationAmount,
                            LocalSetRate = (decimal)po.LocalSetRate,
                            GLAID = allocationID,
                            Effective = EffectiveBlance.Credit
                        });
                    }
                    _context.GLAccounts.Update(allocation);
                }
                if (Type != "Add")
                {
                    //update itmemasterdata
                    itemmaster.StockOnHand -= @Qty;
                    itemmaster.StockIn += @Qty;
                    //update warehouse
                    warehouse.Ordered = warehouse.Ordered <= 0 ? 0 : warehouse.Ordered - @Qty;
                    warehouse.InStock += @Qty;
                    _context.ItemMasterDatas.Update(itemmaster);
                    _context.WarehouseSummary.Update(warehouse);
                    _utility.UpdateItemAccounting(_itemAcc, warehouse);
                }
                else
                {
                    // update itmemasterdata                    
                    itemmaster.StockIn += @Qty;
                    //update warehouse                    
                    warehouse.InStock += @Qty;
                    _context.ItemMasterDatas.Update(itemmaster);
                    _context.WarehouseSummary.Update(warehouse);
                    _utility.UpdateItemAccounting(_itemAcc, warehouse);
                }
                if (itemmaster.ManItemBy == ManageItemBy.SerialNumbers && itemmaster.ManMethod == ManagementMethod.OnEveryTransation)
                {
                    var svmp = serialViewModelPurchases.FirstOrDefault(s => s.ItemID == itemdt.ItemID);
                    List<WarehouseDetail> whsDetials = new();
                    List<InventoryAudit> inventoryAudit = new();
                    if (svmp != null)
                    {
                        foreach (var sv in svmp.SerialDetialViewModelPurchase)
                        {
                            whsDetials.Add(new WarehouseDetail
                            {
                                AdmissionDate = sv.AdmissionDate,
                                Cost = _cost,
                                CurrencyID = po.PurCurrencyID,
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
                                WarehouseID = po.WarehouseID,
                                UomID = itemdt.UomID,
                                UserID = po.UserID,
                                ExpireDate = sv.ExpirationDate == null ? default : (DateTime)sv.ExpirationDate,
                                TransType = TransTypeWD.GRPO,
                                BPID = po.VendorID,
                                IsDeleted = true,
                                InStockFrom = po.ID
                            });
                        }
                        //insert inventoryaudit
                        InventoryAudit invAudit = new();
                        var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == itemdt.ItemID && w.UomID == itemdt.UomID && w.WarehouseID == po.WarehouseID);
                        var pri_detial = _context.PriceListDetails.Where(w => w.ItemID == itemdt.ItemID && w.UomID == itemdt.UomID);
                        invAudit.ID = 0;
                        invAudit.WarehouseID = po.WarehouseID;
                        invAudit.BranchID = po.BranchID;
                        invAudit.UserID = po.UserID;
                        invAudit.ItemID = itemdt.ItemID;
                        invAudit.CurrencyID = po.SysCurrencyID;
                        invAudit.UomID = itemdt.UomID;
                        invAudit.InvoiceNo = po.Number;
                        invAudit.Trans_Type = docType.Code;
                        invAudit.Process = itemmaster.Process;
                        invAudit.SystemDate = DateTime.Now;
                        invAudit.PostingDate = po.PostingDate;
                        invAudit.TimeIn = DateTime.Now.ToString("h:mm:ss tt");
                        invAudit.Qty = @Qty;
                        invAudit.Cost = _cost;
                        invAudit.Price = 0;
                        invAudit.CumulativeQty = inventory_audit.Sum(q => q.Qty) + @Qty;
                        invAudit.CumulativeValue = (inventory_audit.Sum(s => s.Trans_Valuse)) + (@Qty * _cost);
                        invAudit.Trans_Valuse = (@Qty * _cost);
                        invAudit.ExpireDate = itemdt.ExpireDate;
                        invAudit.LocalCurID = po.LocalCurID;
                        invAudit.LocalSetRate = po.LocalSetRate;
                        invAudit.DocumentTypeID = po.DocumentTypeID;
                        invAudit.CompanyID = po.CompanyID;
                        invAudit.SeriesID = po.SeriesID;
                        invAudit.SeriesDetailID = po.SeriesDetailID;
                        // update pricelistdetial
                        _utility.CumulativeValue(invAudit.WarehouseID, invAudit.ItemID, invAudit.CumulativeValue, _itemAcc);
                        _context.InventoryAudits.Add(invAudit);
                        _context.WarehouseDetails.AddRange(whsDetials);
                        _context.SaveChanges();
                    }
                }
                else if (itemmaster.ManItemBy == ManageItemBy.Batches && itemmaster.ManMethod == ManagementMethod.OnEveryTransation)
                {
                    var bvmp = batchViewModelPurchases.FirstOrDefault(s => s.ItemID == itemdt.ItemID);
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
                                CurrencyID = po.PurCurrencyID,
                                Details = bv.Detials,
                                ID = 0,
                                InStock = (double)bv.Qty,
                                ItemID = itemdt.ItemID,
                                Location = bv.Location,
                                MfrDate = bv.MfrDate,
                                ProcessItem = ProcessItem.SEBA,
                                SyetemDate = DateTime.Now,
                                SysNum = 0,
                                TimeIn = DateTime.Now,
                                WarehouseID = po.WarehouseID,
                                UomID = itemdt.UomID,
                                UserID = po.UserID,
                                ExpireDate = bv.ExpirationDate == null ? default : (DateTime)bv.ExpirationDate,
                                BatchAttr1 = bv.BatchAttribute1,
                                BatchAttr2 = bv.BatchAttribute2,
                                BatchNo = bv.Batch,
                                TransType = TransTypeWD.GRPO,
                                BPID = po.VendorID,
                                IsDeleted = true,
                                InStockFrom = po.ID
                            });
                        }
                    }
                    //insert inventoryaudit
                    InventoryAudit invAudit = new();
                    var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == itemdt.ItemID && w.UomID == itemdt.UomID && w.WarehouseID == po.WarehouseID);
                    // var pri_detial = _context.PriceListDetails.Where(w => w.ItemID == itemdt.ItemID && w.UomID == itemdt.UomID);
                    invAudit.ID = 0;
                    invAudit.WarehouseID = po.WarehouseID;
                    invAudit.BranchID = po.BranchID;
                    invAudit.UserID = po.UserID;
                    invAudit.ItemID = itemdt.ItemID;
                    invAudit.CurrencyID = po.SysCurrencyID;
                    invAudit.UomID = itemdt.UomID;
                    invAudit.InvoiceNo = po.Number;
                    invAudit.Trans_Type = docType.Code;
                    invAudit.Process = itemmaster.Process;
                    invAudit.SystemDate = DateTime.Now;
                    invAudit.PostingDate = po.PostingDate;
                    invAudit.TimeIn = DateTime.Now.ToString("h:mm:ss tt");
                    invAudit.Qty = @Qty;
                    invAudit.Cost = _cost;
                    invAudit.Price = 0;
                    invAudit.CumulativeQty = inventory_audit.Sum(q => q.Qty) + @Qty;
                    invAudit.CumulativeValue = (inventory_audit.Sum(s => s.Trans_Valuse)) + (@Qty * _cost);
                    invAudit.Trans_Valuse = (@Qty * _cost);
                    invAudit.ExpireDate = itemdt.ExpireDate;
                    invAudit.LocalCurID = po.LocalCurID;
                    invAudit.LocalSetRate = po.LocalSetRate;
                    invAudit.DocumentTypeID = po.DocumentTypeID;
                    invAudit.CompanyID = po.CompanyID;
                    invAudit.SeriesID = po.SeriesID;
                    invAudit.SeriesDetailID = po.SeriesDetailID;
                    // update pricelistdetial
                    _utility.CumulativeValue(invAudit.WarehouseID, invAudit.ItemID, invAudit.CumulativeValue, _itemAcc);
                    _context.InventoryAudits.Add(invAudit);
                    _context.WarehouseDetails.AddRange(whsDetials);
                    _context.SaveChanges();
                }
                else
                {
                    warehousedetail.ID = 0;
                    warehousedetail.WarehouseID = po.WarehouseID;
                    warehousedetail.UserID = po.UserID;
                    warehousedetail.UomID = itemdt.UomID;
                    warehousedetail.SyetemDate = DateTime.Now;
                    warehousedetail.TimeIn = DateTime.Now;
                    warehousedetail.InStock = @Qty;
                    warehousedetail.CurrencyID = po.SysCurrencyID;
                    warehousedetail.ExpireDate = itemdt.ExpireDate;
                    warehousedetail.ItemID = itemdt.ItemID;
                    warehousedetail.Cost = _cost;
                    warehousedetail.IsDeleted = true;
                    warehousedetail.InStockFrom = po.ID;
                    warehousedetail.TransType = TransTypeWD.GRPO;
                    warehousedetail.BPID = po.VendorID;
                    _context.WarehouseDetails.Add(warehousedetail);
                    _context.SaveChanges();
                    if (itemmaster.Process == "FIFO")
                    {
                        //insert inventoryaudit
                        var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == itemdt.ItemID && w.WarehouseID == po.WarehouseID);
                        var pri_detial = _context.PriceListDetails.Where(w => w.ItemID == itemdt.ItemID && w.UomID == itemdt.UomID);
                        item_inventory_audit.ID = 0;
                        item_inventory_audit.WarehouseID = po.WarehouseID;
                        item_inventory_audit.BranchID = po.BranchID;
                        item_inventory_audit.UserID = po.UserID;
                        item_inventory_audit.ItemID = itemdt.ItemID;
                        item_inventory_audit.CurrencyID = po.SysCurrencyID;
                        item_inventory_audit.UomID = orft.BaseUOM;//itemdt.UomID;
                        item_inventory_audit.InvoiceNo = po.Number;
                        item_inventory_audit.Trans_Type = docType.Code;
                        item_inventory_audit.Process = itemmaster.Process;
                        item_inventory_audit.SystemDate = DateTime.Now;
                        item_inventory_audit.PostingDate = po.PostingDate;
                        item_inventory_audit.TimeIn = DateTime.Now.ToString("h:mm:ss tt");
                        item_inventory_audit.Qty = @Qty;
                        item_inventory_audit.Cost = _cost;
                        item_inventory_audit.Price = 0;
                        item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) + @Qty;
                        item_inventory_audit.CumulativeValue = (inventory_audit.Sum(s => s.Trans_Valuse)) + (@Qty * _cost);
                        item_inventory_audit.Trans_Valuse = (@Qty * _cost);
                        item_inventory_audit.ExpireDate = itemdt.ExpireDate;
                        item_inventory_audit.LocalCurID = po.LocalCurID;
                        item_inventory_audit.LocalSetRate = po.LocalSetRate;
                        item_inventory_audit.DocumentTypeID = po.DocumentTypeID;
                        item_inventory_audit.CompanyID = po.CompanyID;
                        item_inventory_audit.SeriesID = po.SeriesID;
                        item_inventory_audit.SeriesDetailID = po.SeriesDetailID;
                        // update pricelistdetial
                        _utility.CumulativeValue(item_inventory_audit.WarehouseID, item_inventory_audit.ItemID, item_inventory_audit.CumulativeValue, _itemAcc);
                        _context.InventoryAudits.Add(item_inventory_audit);
                        _context.SaveChanges();
                    }
                    else if (itemmaster.Process == "Average")
                    {
                        //insert inventoryaudit
                        var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == itemdt.ItemID && w.WarehouseID == po.WarehouseID);
                        var warehouse_sammary = _context.WarehouseSummary.FirstOrDefault(w => w.ItemID == itemdt.ItemID && w.WarehouseID == po.WarehouseID);
                        var pri_detial = _context.PriceListDetails.Where(w => w.ItemID == itemdt.ItemID && w.UomID == itemdt.UomID);
                        double @AvgCost = (inventory_audit.Sum(s => s.Trans_Valuse) + (@Qty * _cost)) / (inventory_audit.Sum(q => q.Qty) + @Qty);
                        @AvgCost = _utility.CheckNaNOrInfinity(@AvgCost);
                        item_inventory_audit.ID = 0;
                        item_inventory_audit.WarehouseID = po.WarehouseID;
                        item_inventory_audit.BranchID = po.BranchID;
                        item_inventory_audit.UserID = po.UserID;
                        item_inventory_audit.ItemID = itemdt.ItemID;
                        item_inventory_audit.CurrencyID = po.SysCurrencyID;
                        item_inventory_audit.UomID = orft.BaseUOM;//itemdt.UomID;
                        item_inventory_audit.InvoiceNo = po.Number;
                        item_inventory_audit.Trans_Type = docType.Code;
                        item_inventory_audit.Process = itemmaster.Process;
                        item_inventory_audit.SystemDate = DateTime.Now;
                        item_inventory_audit.PostingDate = po.PostingDate;
                        item_inventory_audit.TimeIn = DateTime.Now.ToString("h:mm:ss tt");
                        item_inventory_audit.Qty = @Qty;
                        item_inventory_audit.Cost = @AvgCost;
                        item_inventory_audit.Price = 0;
                        item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) + @Qty;
                        item_inventory_audit.CumulativeValue = (inventory_audit.Sum(s => s.Trans_Valuse)) + (Qty * _cost);
                        item_inventory_audit.Trans_Valuse = (@Qty * _cost);
                        item_inventory_audit.ExpireDate = itemdt.ExpireDate;
                        item_inventory_audit.LocalCurID = po.LocalCurID;
                        item_inventory_audit.LocalSetRate = po.LocalSetRate;
                        item_inventory_audit.CompanyID = po.CompanyID;
                        item_inventory_audit.DocumentTypeID = po.DocumentTypeID;
                        item_inventory_audit.SeriesID = po.SeriesID;
                        item_inventory_audit.SeriesDetailID = po.SeriesDetailID;
                        // update_warehouse_summary
                        _utility.UpdateAvgCost(itemdt.ItemID, po.WarehouseID, itemmaster.GroupUomID, item_inventory_audit.Qty, item_inventory_audit.Cost);
                        _utility.UpdateBomCost(itemdt.ItemID, item_inventory_audit.Qty, item_inventory_audit.Cost);
                        _utility.CumulativeValue(item_inventory_audit.WarehouseID, item_inventory_audit.ItemID, item_inventory_audit.CumulativeValue, _itemAcc);
                        _context.InventoryAudits.Add(item_inventory_audit);
                        _context.SaveChanges();
                    }

                }

            }
            //update bom
            foreach (var item in ItemPO)
            {
                var ItemBOMDetail = _context.BOMDetail.Where(w => w.ItemID == item.ItemID);
                foreach (var itembom in ItemBOMDetail)
                {
                    var BOM = _context.BOMaterial.First(w => w.BID == itembom.BID);
                    var DBOM = _context.BOMDetail.Where(w => w.BID == BOM.BID && w.Detele == false);
                    //
                    var Inven = _context.InventoryAudits.Where(w => w.ItemID == itembom.ItemID);
                    double @AvgCost = Inven.Sum(s => s.Trans_Valuse) / Inven.Sum(q => q.Qty);
                    @AvgCost = _utility.CheckNaNOrInfinity(@AvgCost);
                    var Gdoum = _context.GroupDUoMs.Where(w => w.GroupUoMID == itembom.GUomID);
                    var Factor = Gdoum.FirstOrDefault(w => w.AltUOM == itembom.UomID).Factor;
                    itembom.Cost = @AvgCost * Factor;
                    itembom.Amount = itembom.Qty * (@AvgCost * Factor);
                    _context.BOMDetail.UpdateRange(ItemBOMDetail);
                    _context.SaveChanges();
                    //
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
        public IEnumerable<ReportPurchaseOrder> GetAllPruchaseOrder(int BranchID) => _context.ReportPurchaseOrders.FromSql("sp_GetAllPurchaeOrder @BranchID={0}",
           parameters: new[] {
                BranchID.ToString()
           });
        public IEnumerable<PurchaseAP_To_PurchaseMemo> GetPurchaseAP_From_PurchaseOrder(int ID, string Number) => _context.PurchaseAP_To_PurchaseMemos.FromSql("sp_GetPurchaseAP_form_PurchaseOrder @PurchaseOrderID={0},@Number={1}",
           parameters: new[] {
                ID.ToString(),
                Number.ToString()
           });
        public IEnumerable<ReportPurchaseAP> GetReportGoodReceiptPO(int BranchID, int WarehouseID, string PostingDate, string DocumentDate, string DeDate, string Check) => _context.ReportPurchaseAPs.FromSql("sp_ReportGoodReceiptPO @BranchID={0},@warehouseID={1},@PostingDate={2},@DueDate={3},@DocumentDate={4},@Check={5}",
         parameters: new[] {
                BranchID.ToString(),
                WarehouseID.ToString(),
                PostingDate.ToString(),
                DocumentDate.ToString(),
                DeDate.ToString(),
                Check.ToString()
         });
    }
}
