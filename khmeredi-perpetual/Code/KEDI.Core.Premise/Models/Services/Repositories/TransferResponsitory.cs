using CKBS.AppContext;
using CKBS.Models.Services.Administrator.General;
using CKBS.Models.Services.Administrator.Inventory;
using CKBS.Models.Services.Administrator.SystemInitialization;
using CKBS.Models.Services.Financials;
using CKBS.Models.Services.Inventory;
using CKBS.Models.Services.Inventory.Transaction;
using CKBS.Models.ServicesClass.GoodsIssue;
using CKBS.Models.ServicesClass.Transfer;
using KEDI.Core.Premise.Models.Services.Inventory.Transaction;
using KEDI.Core.Premise.Models.ServicesClass.SerialBatches.Sales;
using KEDI.Core.Premise.Repository;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Responsitory
{

    public interface ITransfer
    {
        IEnumerable<Warehouse> GetFromWarehouse(int BranchID);
        IEnumerable<Warehouse> GetToWarehouse { get; }
        IEnumerable<Branch> GetBranches { get; }
        IEnumerable<Warehouse> GetWarehouse_filter_Branch(int BranchID);
        List<TransferDetail> GetItemMasterBy_Warehouse(int warehouseID, int comId);
        IEnumerable<TransferDetail> GetItemFindBarcode(int warehouseID, string Barcode);
        List<TransferViewModel> GetAllPurItemsByItemID(int wareid, int itemId, int comId);
        void SaveTrasfers(int TransferID, List<SerialNumber> serials, List<BatchNo> batchNos);
        Task<TransferRequest> FindTransferRequest(string number);
        void UpdateTransferRequest(List<TransferDetail> lis, int trrqid);

    }
    public class TransferResponsitory : ITransfer
    {
        private readonly DataContext _context;
        private readonly IDataPropertyRepository _dataProp;
        private readonly UtilityModule _utility;
        public TransferResponsitory(DataContext context, IDataPropertyRepository dataProperty, UtilityModule utility)
        {
            _context = context;
            _dataProp = dataProperty;
            _utility = utility;
        }
        public IEnumerable<Warehouse> GetFromWarehouse(int BranchID) => _context.Warehouses.Where(x => x.Delete == false && x.BranchID == BranchID);

        public IEnumerable<Warehouse> GetWarehouse_filter_Branch(int BranchID) => _context.Warehouses.Where(x => x.Delete == false && x.BranchID == BranchID);

        public List<TransferDetail> GetItemMasterBy_Warehouse(int warehouseID, int comId)
        {
            var data = (from wd in _context.WarehouseDetails
                        join item in _context.ItemMasterDatas on wd.ItemID equals item.ID
                        join uom in _context.UnitofMeasures on wd.UomID equals uom.ID
                        join cur in _context.Currency on wd.CurrencyID equals cur.ID
                        where wd.WarehouseID == warehouseID && !item.Delete && item.Process != "Standard" && wd.InStock > 0
                        && item.Inventory && item.Purchase && item.InventoryUoMID == wd.UomID
                        group new {wd, item, uom, cur} by new {uom.ID, ItemID = item.ID} into g
                        let d = g.FirstOrDefault()
                        select new TransferDetail
                        {
                            LineID = d.wd.ID,
                            ItemID = d.item.ID,
                            CurrencyID = d.cur.ID,
                            Code = d.item.Code,
                            KhmerName = d.item.KhmerName,
                            EnglishName = d.item.EnglishName,
                            Qty = g.Sum(i=> i.wd.InStock),
                            Cost = d.wd.Cost,
                            Currency = d.cur.Description,
                            UomName = d.uom.Name,
                            Barcode = d.item.Barcode,
                            ExpireDate = d.wd.ExpireDate,
                        }).ToList();
            _dataProp.DataProperty(data, comId, "ItemID", "AddictionProps");
            return data;
        }
        public void SaveTrasfers(int TransferID, List<SerialNumber> serials, List<BatchNo> batchNos)
        {
            var transfer = _context.Transfers.Find(TransferID) ?? new Transfer();
            var Com = _context.Company.FirstOrDefault(c => !c.Delete && c.ID == transfer.CompanyID);
            foreach (var item in transfer.TransferDetails)
            {
                WarehouseSummary ws = new();
                var item_master_data = _context.ItemMasterDatas.FirstOrDefault(w => w.ID == item.ItemID);
                var baseUOM = _context.GroupDUoMs.FirstOrDefault(w => w.UoMID == item.UomID);
                if (item_master_data.Process != "Standard")
                {
                    double cost = item.Cost / baseUOM.Factor;
                    var item_warehouse_summaryForm = _context.WarehouseSummary
                        .FirstOrDefault(w => w.WarehouseID == transfer.WarehouseFromID && w.ItemID == item.ItemID/* && w.Cost == ((double)item.Cost / baseUOM.Factor)*/);
                    var item_warehouse_summaryTo = _context.WarehouseSummary.FirstOrDefault(w => w.WarehouseID == transfer.WarehouseToID && w.ItemID == item.ItemID /*&& w.Cost == ((double)item.Cost / baseUOM.Factor)*/);
                    var item_warehouse_detailFrom = _context.WarehouseDetails.FirstOrDefault(w => w.WarehouseID == transfer.WarehouseFromID && w.ID == item.LineID && w.Cost == cost);
                    var item_warehouse_detailTo = _context.WarehouseDetails.FirstOrDefault(w => w.WarehouseID == transfer.WarehouseToID && w.ItemID == item.ItemID && w.Cost == cost);
                    var _itemAccFrom = _context.ItemAccountings.FirstOrDefault(i => i.ItemID == item.ItemID && i.WarehouseID == transfer.WarehouseFromID);
                    var _itemAccTo = _context.ItemAccountings.FirstOrDefault(i => i.ItemID == item.ItemID && i.WarehouseID == transfer.WarehouseToID);
                    if (item_warehouse_summaryForm != null)
                    {
                        //WerehouseSummary
                        item_warehouse_summaryForm.InStock -= (double)item.Qty * baseUOM.Factor;
                        _context.WarehouseSummary.Update(item_warehouse_summaryForm);
                        _context.SaveChanges();
                        _utility.UpdateItemAccounting(_itemAccFrom, item_warehouse_summaryForm);
                        if (item_warehouse_summaryTo != null)
                        {
                            //WerehouseSummary
                            item_warehouse_summaryTo.InStock += (double)item.Qty * baseUOM.Factor;
                            _context.WarehouseSummary.Update(item_warehouse_summaryTo);
                            _context.SaveChanges();
                            _utility.UpdateItemAccounting(_itemAccTo, item_warehouse_summaryTo);
                        }
                        else
                        {
                            ws.Available = 0;
                            ws.Committed = 0;
                            ws.Cost = item_master_data.Process == "Average" ? item.Cost : 0;
                            ws.CumulativeValue = (decimal)(item.Qty * item.Cost);
                            ws.CurrencyID = item_warehouse_summaryForm.CurrencyID;
                            ws.ExpireDate = item_warehouse_summaryForm.ExpireDate;
                            ws.Factor = 1;
                            ws.ID = 0;
                            ws.InStock = (double)item.Qty * baseUOM.Factor;
                            ws.ItemID = item_warehouse_summaryForm.ItemID;
                            ws.Ordered = 0;
                            ws.SyetemDate = item_warehouse_summaryForm.SyetemDate;
                            ws.TimeIn = DateTime.Parse(DateTime.Now.ToShortTimeString());
                            ws.UomID = item_warehouse_summaryForm.UomID;
                            ws.UserID = item_warehouse_summaryForm.UserID;
                            ws.WarehouseID = transfer.WarehouseToID;
                            _context.WarehouseSummary.Update(ws);
                            _context.SaveChanges();
                            _utility.UpdateItemAccounting(_itemAccTo, ws);
                        }
                    }
                    if (item_master_data.ManItemBy == ManageItemBy.None)
                    {
                        if (item_warehouse_detailTo != null)
                        {
                            item_warehouse_detailFrom.InStock -= (double)item.Qty * baseUOM.Factor;
                            item_warehouse_detailTo.InStock += (double)item.Qty * baseUOM.Factor;
                            _context.WarehouseDetails.Update(item_warehouse_detailFrom);
                            _context.WarehouseDetails.Update(item_warehouse_detailTo);
                            _context.SaveChanges();
                        }
                        else
                        {
                            item_warehouse_detailFrom.InStock -= (double)item.Qty * baseUOM.Factor;
                            var wd = new WarehouseDetail
                            {
                                Cost = item.Cost,
                                CurrencyID = item.CurrencyID,
                                ExpireDate = item.ExpireDate,
                                ID = 0,
                                InStock = item.Qty * baseUOM.Factor,
                                ItemID = item.ItemID,
                                SyetemDate = item_warehouse_summaryTo == null ? ws.SyetemDate : item_warehouse_summaryTo.SyetemDate,
                                TimeIn = DateTime.Parse(DateTime.Now.ToShortTimeString()),
                                UomID = item.UomID,
                                UserID = item_warehouse_summaryTo == null ? ws.UserID : item_warehouse_summaryTo.UserID,
                                WarehouseID = transfer.WarehouseToID,
                                AutoCreate = true,
                            };
                            _context.WarehouseDetails.Update(item_warehouse_detailFrom);
                            _context.WarehouseDetails.Update(wd);
                            _context.SaveChanges();
                        }
                    }
                    CheckinWarehouseFrom(Com, transfer, item_master_data, baseUOM, item, _itemAccFrom, serials, batchNos);
                    //// Warehouse to ////
                    CheckinWarehouseTo(Com, transfer, item_master_data, baseUOM, item, _itemAccTo, serials, batchNos);
                }
            }
        }
        private void CheckinWarehouseFrom(Company Com, Transfer transfer, ItemMasterData item_master_data, GroupDUoM baseUOM, TransferDetail item, ItemAccounting itemAccounting, List<SerialNumber> serials, List<BatchNo> batches)
        {
            double @Qty = item.Qty * baseUOM.Factor;
            double @Cost = item.Cost / baseUOM.Factor;
            var item_inventory_audit = new InventoryAudit();
            var docType = _context.DocumentTypes.Find(transfer.DocTypeID) ?? new DocumentType();
            //var wdStock = itemwd.FirstOrDefault(w => w.InStock > 0 && w.Cost == item.Cost)
            if (item_master_data.ManItemBy == ManageItemBy.SerialNumbers && item_master_data.ManMethod == ManagementMethod.OnEveryTransation)
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
                                if (waredetial != null)
                                {
                                    waredetial.InStock -= 1;
                                    _context.SaveChanges();

                                    var waredetialTo = _context.WarehouseDetails.FirstOrDefault(i => ss.SerialNumber == i.SerialNumber && i.WarehouseID == transfer.WarehouseToID);
                                    if (waredetialTo != null)
                                    {
                                        waredetialTo.InStock += 1;
                                        _context.SaveChanges();
                                    }
                                    else
                                    {
                                        WarehouseDetail waredetialToNew = new()
                                        {
                                            AdmissionDate = DateTime.Today,
                                            InStock = 1,
                                            BPID = 0,
                                            Cost = waredetial.Cost,
                                            CurrencyID = waredetial.CurrencyID,
                                            Details = waredetial.Details,
                                            Direction = waredetial.Direction,
                                            ExpireDate = waredetial.ExpireDate,
                                            GRGIID = transfer.TarmsferID,
                                            ID = 0,
                                            InStockFrom = transfer.TarmsferID,
                                            IsDeleted = waredetial.IsDeleted,
                                            ItemID = waredetial.ItemID,
                                            Location = waredetial.Location,
                                            LotNumber = waredetial.LotNumber,
                                            MfrDate = waredetial.MfrDate,
                                            MfrSerialNumber = waredetial.MfrSerialNumber,
                                            MfrWarDateEnd = waredetial.MfrWarDateEnd,
                                            MfrWarDateStart = waredetial.MfrWarDateStart,
                                            ProcessItem = waredetial.ProcessItem,
                                            SerialNumber = waredetial.SerialNumber,
                                            SyetemDate = DateTime.Today,
                                            SysNum = waredetial.SysNum,
                                            TimeIn = DateTime.Now,
                                            TransType = TransTypeWD.Transfer,
                                            UomID = waredetial.UomID,
                                            UserID = transfer.UserID,
                                            WarehouseID = transfer.WarehouseToID
                                        };
                                        _context.WarehouseDetails.Update(waredetialToNew);
                                        _context.SaveChanges();
                                    }
                                }
                            }
                        }
                    }
                    // Insert to Inventory Audit Warehouse From
                    var inventory_audit = _context.InventoryAudits
                        .Where(w => w.ItemID == item.ItemID && w.WarehouseID == transfer.WarehouseFromID);
                    InventoryAudit inventory = new()
                    {
                        ID = 0,
                        WarehouseID = transfer.WarehouseFromID,
                        BranchID = transfer.BranchID,
                        UserID = transfer.UserID,
                        ItemID = item.ItemID,
                        CurrencyID = transfer.LocalCurID,
                        UomID = baseUOM.BaseUOM,
                        InvoiceNo = transfer.Number,
                        Trans_Type = docType.Code,
                        Process = item_master_data.Process,
                        SystemDate = DateTime.Now,
                        TimeIn = DateTime.Now.ToShortTimeString().ToString(),
                        Qty = @Qty * -1,
                        Cost = @Cost,
                        Price = 0,
                        CumulativeQty = inventory_audit.Sum(q => q.Qty) - @Qty,
                        CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) - (@Qty * @Cost),
                        Trans_Valuse = @Qty * @Cost * -1,
                        ExpireDate = item.ExpireDate,
                        LocalCurID = transfer.LocalCurID,
                        LocalSetRate = transfer.LocalSetRate,
                        CompanyID = transfer.CompanyID,
                        DocumentTypeID = docType.ID,
                        SeriesID = transfer.SeriseID,
                        SeriesDetailID = transfer.SeriseDID,
                    };

                    // Insert to Inventory Audit Warehouse To
                    var inventory_audit_to = _context.InventoryAudits
                        .Where(w => w.ItemID == item.ItemID && w.WarehouseID == transfer.WarehouseToID);
                    InventoryAudit inventoryTo = new()
                    {
                        ID = 0,
                        WarehouseID = transfer.WarehouseToID,
                        BranchID = transfer.BranchID,
                        UserID = transfer.UserID,
                        ItemID = item.ItemID,
                        CurrencyID = transfer.LocalCurID,
                        UomID = baseUOM.BaseUOM,
                        InvoiceNo = transfer.Number,
                        Trans_Type = docType.Code,
                        Process = item_master_data.Process,
                        SystemDate = DateTime.Now,
                        TimeIn = DateTime.Now.ToShortTimeString().ToString(),
                        Qty = @Qty,
                        Cost = @Cost,
                        Price = 0,
                        CumulativeQty = inventory_audit_to.Sum(q => q.Qty) + @Qty,
                        CumulativeValue = inventory_audit_to.Sum(s => s.Trans_Valuse) + (@Qty * @Cost),
                        Trans_Valuse = @Qty * @Cost,
                        ExpireDate = item.ExpireDate,
                        LocalCurID = transfer.LocalCurID,
                        LocalSetRate = transfer.LocalSetRate,
                        CompanyID = transfer.CompanyID,
                        DocumentTypeID = docType.ID,
                        SeriesID = transfer.SeriseID,
                        SeriesDetailID = transfer.SeriseDID,
                    };



                    _utility.CumulativeValue(inventory.WarehouseID, inventory.ItemID, inventory.CumulativeValue, itemAccounting);
                    _utility.CumulativeValue(inventoryTo.WarehouseID, inventoryTo.ItemID, inventoryTo.CumulativeValue, itemAccounting);
                    _context.InventoryAudits.Add(inventory);
                    _context.InventoryAudits.Add(inventoryTo);
                    _context.SaveChanges();
                }
            }
            else if (item_master_data.ManItemBy == ManageItemBy.Batches && item_master_data.ManMethod == ManagementMethod.OnEveryTransation)
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
                                if (waredetial != null)
                                {
                                    waredetial.InStock -= (double)selectedQty;
                                    _context.SaveChanges();
                                    var waredetialTo = _context.WarehouseDetails.FirstOrDefault(i => sb.BatchNo == i.BatchNo && i.WarehouseID == transfer.WarehouseToID);
                                    if (waredetialTo != null)
                                    {
                                        waredetialTo.InStock += 1;
                                        _context.SaveChanges();
                                    }
                                    else
                                    {
                                        WarehouseDetail waredetialToNew = new()
                                        {
                                            AdmissionDate = DateTime.Today,
                                            BatchNo = waredetial.BatchNo,
                                            BatchAttr1 = waredetial.BatchAttr1,
                                            BatchAttr2 = waredetial.BatchAttr2,
                                            BPID = 0,
                                            InStock = (double)selectedQty,
                                            Cost = waredetial.Cost,
                                            CurrencyID = waredetial.CurrencyID,
                                            Details = waredetial.Details,
                                            Direction = waredetial.Direction,
                                            ExpireDate = waredetial.ExpireDate,
                                            GRGIID = transfer.TarmsferID,
                                            ID = 0,
                                            InStockFrom = transfer.TarmsferID,
                                            IsDeleted = waredetial.IsDeleted,
                                            ItemID = waredetial.ItemID,
                                            Location = waredetial.Location,
                                            LotNumber = waredetial.LotNumber,
                                            MfrDate = waredetial.MfrDate,
                                            MfrSerialNumber = waredetial.MfrSerialNumber,
                                            MfrWarDateEnd = waredetial.MfrWarDateEnd,
                                            MfrWarDateStart = waredetial.MfrWarDateStart,
                                            ProcessItem = waredetial.ProcessItem,
                                            SerialNumber = waredetial.SerialNumber,
                                            SyetemDate = DateTime.Today,
                                            SysNum = waredetial.SysNum,
                                            TimeIn = DateTime.Now,
                                            TransType = TransTypeWD.Transfer,
                                            UomID = waredetial.UomID,
                                            UserID = transfer.UserID,
                                            WarehouseID = transfer.WarehouseToID
                                        };
                                        _context.WarehouseDetails.Update(waredetialToNew);
                                        _context.SaveChanges();
                                    }
                                }
                            }
                        }
                    }
                    // insert to inventory audit warehouse from
                    var inventory_audit = _context.InventoryAudits
                        .Where(w => w.ItemID == item.ItemID && w.WarehouseID == transfer.WarehouseFromID);
                    InventoryAudit inventory = new()
                    {
                        ID = 0,
                        WarehouseID = transfer.WarehouseFromID,
                        BranchID = transfer.BranchID,
                        UserID = transfer.UserID,
                        ItemID = item.ItemID,
                        CurrencyID = transfer.LocalCurID,
                        UomID = baseUOM.BaseUOM,
                        InvoiceNo = transfer.Number,
                        Trans_Type = docType.Code,
                        Process = item_master_data.Process,
                        SystemDate = DateTime.Now,
                        TimeIn = DateTime.Now.ToShortTimeString().ToString(),
                        Qty = @Qty * -1,
                        Cost = @Cost,
                        Price = 0,
                        CumulativeQty = inventory_audit.Sum(q => q.Qty) - @Qty,
                        CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) - (@Qty * @Cost),
                        Trans_Valuse = @Qty * @Cost * -1,
                        ExpireDate = item.ExpireDate,
                        LocalCurID = transfer.LocalCurID,
                        LocalSetRate = transfer.LocalSetRate,
                        CompanyID = transfer.CompanyID,
                        DocumentTypeID = docType.ID,
                        SeriesID = transfer.SeriseID,
                        SeriesDetailID = transfer.SeriseDID,
                    };

                    // Insert to Inventory Audit Warehouse To
                    var inventory_audit_to = _context.InventoryAudits
                        .Where(w => w.ItemID == item.ItemID && w.WarehouseID == transfer.WarehouseToID);
                    InventoryAudit inventoryTo = new()
                    {
                        ID = 0,
                        WarehouseID = transfer.WarehouseToID,
                        BranchID = transfer.BranchID,
                        UserID = transfer.UserID,
                        ItemID = item.ItemID,
                        CurrencyID = transfer.LocalCurID,
                        UomID = baseUOM.BaseUOM,
                        InvoiceNo = transfer.Number,
                        Trans_Type = docType.Code,
                        Process = item_master_data.Process,
                        SystemDate = DateTime.Now,
                        TimeIn = DateTime.Now.ToShortTimeString().ToString(),
                        Qty = @Qty,
                        Cost = @Cost,
                        Price = 0,
                        CumulativeQty = inventory_audit_to.Sum(q => q.Qty) + @Qty,
                        CumulativeValue = inventory_audit_to.Sum(s => s.Trans_Valuse) + (@Qty * @Cost),
                        Trans_Valuse = @Qty * @Cost,
                        ExpireDate = item.ExpireDate,
                        LocalCurID = transfer.LocalCurID,
                        LocalSetRate = transfer.LocalSetRate,
                        CompanyID = transfer.CompanyID,
                        DocumentTypeID = docType.ID,
                        SeriesID = transfer.SeriseID,
                        SeriesDetailID = transfer.SeriseDID,
                    };
                    _utility.CumulativeValue(inventory.WarehouseID, inventory.ItemID, inventory.CumulativeValue, itemAccounting);
                    _utility.CumulativeValue(inventoryTo.WarehouseID, inventoryTo.ItemID, inventoryTo.CumulativeValue, itemAccounting);
                    _context.InventoryAudits.Add(inventory);
                    _context.InventoryAudits.Add(inventoryTo);
                    _context.SaveChanges();
                }
            }
            else
            {
                if (item_master_data.Process == "FIFO")
                {
                    var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item.ItemID && w.WarehouseID == transfer.WarehouseFromID);
                    item_inventory_audit.ID = 0;
                    item_inventory_audit.WarehouseID = transfer.WarehouseFromID;
                    item_inventory_audit.BranchID = transfer.BranchID;
                    item_inventory_audit.UserID = transfer.UserID;
                    item_inventory_audit.ItemID = item.ItemID;
                    item_inventory_audit.CurrencyID = Com.SystemCurrencyID;
                    item_inventory_audit.UomID = baseUOM.BaseUOM;
                    item_inventory_audit.InvoiceNo = transfer.Number;
                    item_inventory_audit.Trans_Type = "ST";
                    item_inventory_audit.Process = item_master_data.Process;
                    item_inventory_audit.SystemDate = DateTime.Now;
                    item_inventory_audit.TimeIn = DateTime.Now.ToShortTimeString().ToString();
                    item_inventory_audit.Qty = @Qty * -1;
                    item_inventory_audit.Cost = @Cost;
                    item_inventory_audit.Price = 0;
                    item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) - @Qty;
                    item_inventory_audit.CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) - @Qty * @Cost;
                    item_inventory_audit.Trans_Valuse = @Qty * @Cost * -1;
                    item_inventory_audit.ExpireDate = item.ExpireDate;
                    item_inventory_audit.LocalCurID = transfer.LocalCurID;
                    item_inventory_audit.LocalSetRate = transfer.LocalSetRate;
                    item_inventory_audit.CompanyID = Com.ID;
                    item_inventory_audit.SeriesDetailID = transfer.SeriseDID;
                    item_inventory_audit.SeriesID = transfer.SeriseID;
                    item_inventory_audit.DocumentTypeID = transfer.DocTypeID;
                    _context.InventoryAudits.Update(item_inventory_audit);
                    _utility.CumulativeValue(item_inventory_audit.WarehouseID, item_inventory_audit.ItemID, item_inventory_audit.CumulativeValue, itemAccounting);
                    _context.SaveChanges();
                }
                else if (item_master_data.Process == "Average")
                {
                    var warehouse_summary = _context.WarehouseSummary.FirstOrDefault(w => w.ItemID == item.ItemID &&
                    w.WarehouseID == transfer.WarehouseFromID);
                    double @sysAvCost = warehouse_summary.Cost;
                    var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item.ItemID && w.UomID == item.UomID &&
                    w.WarehouseID == transfer.WarehouseFromID);
                    item_inventory_audit.ID = 0;
                    item_inventory_audit.WarehouseID = transfer.WarehouseFromID;
                    item_inventory_audit.BranchID = transfer.BranchID;
                    item_inventory_audit.UserID = transfer.UserID;
                    item_inventory_audit.ItemID = item.ItemID;
                    item_inventory_audit.CurrencyID = Com.SystemCurrencyID;
                    item_inventory_audit.UomID = baseUOM.BaseUOM;
                    item_inventory_audit.InvoiceNo = transfer.Number;
                    item_inventory_audit.Trans_Type = "ST";
                    item_inventory_audit.Process = item_master_data.Process;
                    item_inventory_audit.SystemDate = DateTime.Now;
                    item_inventory_audit.TimeIn = DateTime.Now.ToShortTimeString().ToString();
                    item_inventory_audit.Qty = @Qty * -1;
                    item_inventory_audit.Cost = @sysAvCost;
                    item_inventory_audit.Price = 0;
                    item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) - @Qty;
                    item_inventory_audit.CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) - @Qty * @sysAvCost;
                    item_inventory_audit.Trans_Valuse = @Qty * @sysAvCost * -1;
                    item_inventory_audit.ExpireDate = item.ExpireDate;
                    item_inventory_audit.LocalCurID = transfer.LocalCurID;
                    item_inventory_audit.LocalSetRate = transfer.LocalSetRate;
                    item_inventory_audit.CompanyID = Com.ID;
                    item_inventory_audit.SeriesDetailID = transfer.SeriseDID;
                    item_inventory_audit.SeriesID = transfer.SeriseID;
                    item_inventory_audit.DocumentTypeID = transfer.DocTypeID;
                    _context.InventoryAudits.Update(item_inventory_audit);
                    _context.SaveChanges();
                    _utility.CumulativeValue(item_inventory_audit.WarehouseID, item_inventory_audit.ItemID, item_inventory_audit.CumulativeValue, itemAccounting);
                    _utility.UpdateAvgCost(item.ItemID, item_inventory_audit.WarehouseID, item_master_data.GroupUomID, item_inventory_audit.Qty, item_inventory_audit.Cost);
                    _utility.UpdateBomCost(item.ItemID, item_inventory_audit.Qty, item_inventory_audit.Cost);
                }
            }
        }
        private void CheckinWarehouseTo(Company Com, Transfer transfer, ItemMasterData item_master_data, GroupDUoM baseUOM, TransferDetail item, ItemAccounting itemAccounting, List<SerialNumber> serials, List<BatchNo> batches)
        {
            double @Qty = item.Qty * baseUOM.Factor;
            double @Cost = item.Cost / baseUOM.Factor;
            var item_inventory_audit = new InventoryAudit();
            var docType = _context.DocumentTypes.Find(transfer.DocTypeID) ?? new DocumentType();
            if (item_master_data.ManItemBy == ManageItemBy.None)
            {
                if (item_master_data.Process == "FIFO")
                {
                    var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item.ItemID &&
                    w.UomID == item.UomID && w.WarehouseID == transfer.WarehouseToID);
                    item_inventory_audit.ID = 0;
                    item_inventory_audit.WarehouseID = transfer.WarehouseToID;
                    item_inventory_audit.BranchID = transfer.BranchID;
                    item_inventory_audit.UserID = transfer.UserID;
                    item_inventory_audit.ItemID = item.ItemID;
                    item_inventory_audit.CurrencyID = Com.SystemCurrencyID;
                    item_inventory_audit.UomID = baseUOM.BaseUOM;
                    item_inventory_audit.InvoiceNo = transfer.Number;
                    item_inventory_audit.Trans_Type = "ST";
                    item_inventory_audit.Process = item_master_data.Process;
                    item_inventory_audit.SystemDate = DateTime.Now;
                    item_inventory_audit.TimeIn = DateTime.Now.ToShortTimeString().ToString();
                    item_inventory_audit.Qty = @Qty;
                    item_inventory_audit.Cost = @Cost;
                    item_inventory_audit.Price = 0;
                    item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) + @Qty;
                    item_inventory_audit.CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) + @Qty * @Cost;
                    item_inventory_audit.Trans_Valuse = @Qty * @Cost;
                    item_inventory_audit.ExpireDate = item.ExpireDate;
                    item_inventory_audit.LocalCurID = transfer.LocalCurID;
                    item_inventory_audit.LocalSetRate = transfer.LocalSetRate;
                    item_inventory_audit.CompanyID = Com.ID;
                    item_inventory_audit.SeriesDetailID = transfer.SeriseDID;
                    item_inventory_audit.SeriesID = transfer.SeriseID;
                    item_inventory_audit.DocumentTypeID = transfer.DocTypeID;
                    _context.InventoryAudits.Update(item_inventory_audit);
                    _utility.CumulativeValue(item_inventory_audit.WarehouseID, item_inventory_audit.ItemID, item_inventory_audit.CumulativeValue, itemAccounting);
                    _context.SaveChanges();
                }
                else if (item_master_data.Process == "Average")
                {
                    var warehouse_summary = _context.WarehouseSummary.FirstOrDefault(w => w.ItemID == item.ItemID &&
                    w.WarehouseID == transfer.WarehouseToID);
                    double @sysAvCost = warehouse_summary.Cost;
                    var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item.ItemID && w.WarehouseID == transfer.WarehouseToID);
                    item_inventory_audit.ID = 0;
                    item_inventory_audit.WarehouseID = transfer.WarehouseToID;
                    item_inventory_audit.BranchID = transfer.BranchID;
                    item_inventory_audit.UserID = transfer.UserID;
                    item_inventory_audit.ItemID = item.ItemID;
                    item_inventory_audit.CurrencyID = Com.SystemCurrencyID;
                    item_inventory_audit.UomID = baseUOM.BaseUOM;
                    item_inventory_audit.InvoiceNo = transfer.Number;
                    item_inventory_audit.Trans_Type = "ST";
                    item_inventory_audit.Process = item_master_data.Process;
                    item_inventory_audit.SystemDate = DateTime.Now;
                    item_inventory_audit.TimeIn = DateTime.Now.ToShortTimeString().ToString();
                    item_inventory_audit.Qty = @Qty;
                    item_inventory_audit.Cost = @sysAvCost;
                    item_inventory_audit.Price = 0;
                    item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) + @Qty;
                    item_inventory_audit.CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) + @Qty * @sysAvCost;
                    item_inventory_audit.Trans_Valuse = @Qty * @sysAvCost;
                    item_inventory_audit.ExpireDate = item.ExpireDate;
                    item_inventory_audit.LocalCurID = transfer.LocalCurID;
                    item_inventory_audit.LocalSetRate = transfer.LocalSetRate;
                    item_inventory_audit.CompanyID = Com.ID;
                    item_inventory_audit.SeriesDetailID = transfer.SeriseDID;
                    item_inventory_audit.SeriesID = transfer.SeriseID;
                    item_inventory_audit.DocumentTypeID = transfer.DocTypeID;
                    _context.InventoryAudits.Update(item_inventory_audit);
                    _context.SaveChanges();
                    _utility.CumulativeValue(item_inventory_audit.WarehouseID, item_inventory_audit.ItemID, item_inventory_audit.CumulativeValue, itemAccounting);
                    _utility.UpdateAvgCost(item.ItemID, item_inventory_audit.WarehouseID, item_master_data.GroupUomID, item_inventory_audit.Qty, item_inventory_audit.Cost);
                    _utility.UpdateBomCost(item.ItemID, item_inventory_audit.Qty, item_inventory_audit.Cost);
                }
            }
        }
        public IEnumerable<TransferDetail> GetItemFindBarcode(int warehouseID, string Barcode) => _context.TransferDetails.FromSql("sp_FindBarcodeTransfer @WarehouseID={0},@Barcode={1}",
            parameters: new[] {
                warehouseID.ToString(),
                Barcode.ToString()
            });
        public IEnumerable<Warehouse> GetToWarehouse => _context.Warehouses.Where(x => x.Delete == false);

        public IEnumerable<Branch> GetBranches => _context.Branches.Where(x => x.Delete == false);
        public List<TransferViewModel> GetAllPurItemsByItemID(int wareid, int itemId, int comId)
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
            List<TransferViewModel> items = new();
            List<WarehouseDetail> warehouseDetails = _context.WarehouseDetails.Where(i => i.WarehouseID == wareid && i.ItemID == itemId && i.InStock > 0).ToList();
            var allPurItems = (from wd in warehouseDetails
                               join item in _context.ItemMasterDatas on wd.ItemID equals item.ID
                               join GDU in _context.GroupDUoMs on item.GroupUomID equals GDU.GroupUoMID
                               join uom in _context.UnitofMeasures on item.SaleUomID equals uom.ID
                               join cur in _context.Currency on wd.CurrencyID equals cur.ID
                               where !item.Delete
                               let ws = _context.WarehouseSummary.FirstOrDefault(i => i.WarehouseID == wd.WarehouseID && i.ItemID == item.ID)
                               let inStock = warehouseDetails.Sum(i => i.InStock)
                               select new TransferViewModel
                               {
                                   TarnsferDetailID = 0,
                                   TransferID = 0,
                                   CurrencyID = wd.CurrencyID,
                                   LineID = wd.ID,
                                   ItemID = wd.ItemID,
                                   UomID = wd.UomID,
                                   Code = item.Code,
                                   KhmerName = item.KhmerName,
                                   EnglishName = item.EnglishName,
                                   Qty = 0,
                                   QuantitySum = item.ManItemBy != ManageItemBy.None ? inStock : wd.InStock,
                                   Cost = wd.Cost,
                                   CostStore = wd.Cost,
                                   Currency = cur.Description,
                                   UomName = uom.Name,
                                   Barcode = item.Barcode,
                                   Type = item.Process,
                                   ManageExpire = item.ManageExpire,
                                   ExpireDate = wd.ExpireDate,
                                   UoMsList = uoms.ToList(),
                                   AvgCost = ws.Cost,
                                   FWarehouse = warehouse.Select(c => new SelectListItem
                                   {
                                       Value = c.ID.ToString(),
                                       Text = c.Name,
                                       Selected = c.ID == warehouse.FirstOrDefault().ID
                                   }).ToList(),
                                   TWarehouse = warehouse.Select(c => new SelectListItem
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
                               }
                               ).ToList();
            var itemGroup = allPurItems.Where(i => i.Type == "FIFO" || i.Type == "SEBA").GroupBy(i => i.Cost).ToList();
            if (itemGroup.Count > 0)
            {
                itemGroup.ForEach(i =>
                {
                    var qty = i.Where(q => q.QuantitySum > 0).ToList();
                    itemGroup.Select(x => x.FirstOrDefault()).ToList().ForEach(item =>
                    {
                        if (i.Key == item.Cost)
                        {
                            item.InStock = item.QuantitySum;
                            items.Add(item);
                        }
                    });

                });
            }
            var itemAvs = allPurItems.Where(i => i.Type == "Average").ToList();
            if (itemAvs.Count > 0)
            {
                double sumQtyAvg = itemAvs.Sum(i => i.QuantitySum);
                var itemAv = itemAvs.FirstOrDefault();
                itemAv.InStock = sumQtyAvg;
                itemAv.Cost = itemAv.AvgCost;
                items.Add(itemAv);
            }
            _dataProp.DataProperty(items, comId, "ItemID", "AddictionProps");
            return items;
        }
        public async Task<TransferRequest> FindTransferRequest(string number)
        {
            var obj1 = await _context.TransferRequests.FirstOrDefaultAsync(s => s.Number == number) ?? new TransferRequest();
            var list = await _context.TransferRequestDetails.Where(s => s.TransferRequestID == obj1.ID).ToListAsync() ?? new List<TransferRequestDetail>();
            var uoms = (from trd in list
                        join guom in _context.ItemMasterDatas on trd.ItemID equals guom.ID
                        join GDU in _context.GroupDUoMs on guom.GroupUomID equals GDU.GroupUoMID
                        join UNM in _context.UnitofMeasures on GDU.AltUOM equals UNM.ID
                        select new UOMSViewModel
                        {
                            BaseUoMID = GDU.BaseUOM,
                            Factor = GDU.Factor,
                            ID = UNM.ID,
                            Name = UNM.Name
                        }).ToList();
            WHViewModel _tg = new()
            {
                ID = 0,
                Name = "--Select--",
                Code = "",
                BranchID = 0
            };
            var warehouse = (from wd in _context.Warehouses.Where(s => s.Delete == false)
                             select new WHViewModel
                             {
                                 Name = wd.Name,
                                 ID = wd.ID,
                                 BranchID = wd.BranchID,
                                 Code = wd.Code
                             }).ToList();
            warehouse.Insert(0, _tg);
            var obj = await (from tr in _context.TransferRequests.Where(tr => tr.Number == number)
                             select new TransferRequest
                             {
                                 ID = tr.ID,
                                 WarehouseFromID = tr.WarehouseFromID,
                                 WarehouseToID = tr.WarehouseToID,
                                 BranchID = tr.BranchID,
                                 BranchToID = tr.BranchToID,

                                 UserID = tr.UserID,
                                 UserRequestID = tr.UserRequestID,
                                 Number = tr.Number,
                                 PostingDate = tr.PostingDate,
                                 DocumentDate = tr.DocumentDate,
                                 Remark = tr.Remark,
                                 Time = tr.Time,
                                 SysCurID = tr.SysCurID,
                                 LocalCurID = tr.LocalCurID,
                                 LocalSetRate = tr.LocalSetRate,
                                 CompanyID = tr.CompanyID,
                                 SeriseID = tr.SeriseID,
                                 SeriseDID = tr.SeriseDID,
                                 DocTypeID = tr.DocTypeID,
                                 TransferViewModels = (from trd in _context.TransferRequestDetails.Where(s => s.TransferRequestID == tr.ID)
                                                       let item = _context.ItemMasterDatas.FirstOrDefault(s => s.ID == trd.ItemID)
                                                       let ward = _context.WarehouseDetails.Where(s => s.WarehouseID == obj1.WarehouseFromID && s.ItemID == item.ID && s.InStock > 0)
                                                       select new TransferViewModel
                                                       {
                                                           ID = trd.ID,
                                                           TransferRequestID = trd.TransferRequestID,
                                                           TarnsferDetailID = 0,
                                                           TransferID = 0,
                                                           CurrencyID = trd.CurrencyID,
                                                           LineID = trd.LineID,
                                                           ItemID = trd.ItemID,
                                                           UomID = trd.UomID,
                                                           Code = trd.Code,
                                                           KhmerName = trd.KhmerName,
                                                           EnglishName = trd.EnglishName,
                                                           Qty = trd.Qty,
                                                           QuantitySum = 0,//item.ManItemBy != ManageItemBy.None ? inStock : wd.InStock,
                                                           Cost = trd.Cost,
                                                           CostStore = trd.Cost,
                                                           Currency = trd.Currency,
                                                           UomName = trd.UomName,
                                                           Barcode = trd.Barcode,
                                                           Type = item.Process,
                                                           ManageExpire = trd.ManageExpire,
                                                           ExpireDate = trd.ExpireDate,
                                                           UoMsList = uoms.ToList(),
                                                           AvgCost = trd.Cost,
                                                           InStock = ward.Sum(s => s.InStock),
                                                           FWarehouseID = trd.FWarehouseID,
                                                           TWarehouseID = trd.TWarehouseID,
                                                           BaseOnID = trd.ID,
                                                           OpenQty  = trd.OPenQty,
                                                           FWarehouse = warehouse.Select(c => new SelectListItem
                                                           {
                                                               Value = c.ID.ToString(),
                                                               Text = c.Name,
                                                               Selected = c.ID == trd.FWarehouseID
                                                           }).ToList(),
                                                           TWarehouse = warehouse.Select(c => new SelectListItem
                                                           {
                                                               Value = c.ID.ToString(),
                                                               Text = c.Name,
                                                               Selected = c.ID == trd.TWarehouseID
                                                           }).ToList(),
                                                           UoMs = uoms.Count() < 0 ? new List<SelectListItem>() : uoms.Select(c => new SelectListItem
                                                           {
                                                               Value = c.ID.ToString(),
                                                               Text = c.Name,
                                                               Selected = trd.UomID == c.ID
                                                           }).ToList(),
                                                       }).ToList(),
                             }).FirstOrDefaultAsync();
            return obj;
        }

        public void UpdateTransferRequest(List<TransferDetail> lis, int trrqid)
        {
            lis.ForEach(i => {
                if (i.BaseOnID > 0)
                {
                    var obj = _context.TransferRequestDetails.FirstOrDefault(s => s.ID == i.BaseOnID) ?? new TransferRequestDetail();
                    if (i.Qty > obj.OPenQty)
                    {
                        obj.OPenQty = 0;
                    }
                    else
                    {
                        obj.OPenQty = obj.OPenQty - i.Qty;
                    }
                    _context.TransferRequestDetails.Update(obj);
                    _context.SaveChanges();
                }
            });
            var list = _context.TransferRequestDetails.Where(s => s.TransferRequestID == trrqid && s.OPenQty > 0).ToList();
            if (list.Count == 0)
            {
                var obj = _context.TransferRequests.FirstOrDefault(s => s.ID == trrqid) ?? new TransferRequest();
                if (obj.ID > 0)
                {
                    obj.TranRequStatus = TranRequStatus.Colse;
                    _context.TransferRequests.Update(obj);
                    _context.SaveChanges();
                }
            }
        }
    }
}
