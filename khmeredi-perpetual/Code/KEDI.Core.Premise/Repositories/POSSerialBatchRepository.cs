using CKBS.AppContext;
using CKBS.Models.Services.Administrator.Inventory;
using CKBS.Models.Services.Inventory;
using KEDI.Core.Premise.Models.ServicesClass.SerialBatches.Sales;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Repository
{
    public interface IPOSSerialBatchRepository
    {
        void CheckItemBatch<T, TD>(T sale, List<TD> detials, List<BatchNo> batchNoes, string saleID);
        void CheckItemSerail<T, TD>(T sale, List<TD> detials, List<SerialNumber> serialNumbers, string saleID);
        void CheckItemSerailKSMS<T, TD>(T sale, List<TD> detials, List<SerialNumber> serialNumbers, int saleID, int wareId);
        void CheckItemBatchKSMS<T, TD>(T sale, List<TD> detials, List<BatchNo> batchNoes, int saleID, int wareId);
        Task<BatchNoUnselect> GetBatchNoDetialsAsync(int itemId, int uomID, int wareId);
        Task<SerialNumberUnselected> GetSerialDetialsAsync(int itemId, int wareId);
        Task<SerialNumberSelected> GetSerialDetialsReturnAsync(int itemId, int saleId, int wareId, TransTypeWD transType);
        Task<BatchNoSelected> GetBatchNoDetialsReturnAsync(int itemId, int uomID, int saleId, int wareId, TransTypeWD transType);
        Task<SerialNumberUnselected> GetSerialDetialsReturnItemAsync(int itemId, int saleId, int wareId);
        Task<BatchNoUnselect> GetBatchNoDetialsReturnItemAsync(int itemId, int uomID, int saleId, int wareId);
        void CheckItemBatchReturn<T, TD>(T sale, List<TD> detials, List<BatchNo> batchNoes, string direction, TransTypeWD transTypeWD);
        void CheckItemSerailReturn<T, TD>(T sale, List<TD> detials, List<SerialNumber> serialNumbers, string direction);
        void CheckCanRingItemSerail<T, TD>(T sale, List<TD> detials, List<SerialNumber> serialNumbers, string saleID);
        void CheckCanRingItemBatch<T, TD>(T sale, List<TD> detials, List<BatchNo> batchNoes, string saleID);

    }

    public class POSSerialBatchRepository : IPOSSerialBatchRepository
    {
        private readonly DataContext _context;

        public POSSerialBatchRepository(DataContext context)
        {
            _context = context;
        }
        public void CheckItemSerail<T, TD>(T sale, List<TD> detials, List<SerialNumber> serialNumbers, string saleID)
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
                    decimal qty = (decimal)((double)GetValue(sd, "Qty") * uom.Factor);
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
                                BpId = (int)GetValue(sale, "CustomerID"),
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
        public void CheckItemBatch<T, TD>(T sale, List<TD> detials, List<BatchNo> batchNoes, string saleID)
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
                    double _qty = (double)GetValue(sd, "Qty");
                    if (item.ManItemBy == ManageItemBy.Batches &&
                        item.ManMethod == ManagementMethod.OnEveryTransation
                        )
                    {
                        var batchNo = batchNoes.FirstOrDefault(i => i.ItemID == itemId) ?? new BatchNo();
                        decimal qty = (decimal)_qty;
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
                                BpId = (int)GetValue(sale, "CustomerID"),
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


        /// <summary>
        /// KSMS Block
        /// </summary>
        public void CheckItemSerailKSMS<T, TD>(T sale, List<TD> detials, List<SerialNumber> serialNumbers, int saleID, int wareId)
        {
            var whs = _context.Warehouses.Find(wareId) ?? new Warehouse();
            if (detials.Count > 0)
            {
                foreach (var sd in detials)
                {
                    int itemId = (int)GetValue(sd, "ItemID");
                    int uomId = (int)GetValue(sd, "UomID");
                    decimal _qty = (decimal)GetValue(sd, "Qty");
                    ItemMasterData item = _context.ItemMasterDatas.Find(itemId) ?? new ItemMasterData();
                    var uom = _context.GroupDUoMs.FirstOrDefault(w => w.GroupUoMID == item.GroupUomID && w.AltUOM == uomId);
                    decimal qty = _qty * (decimal)uom.Factor * (decimal)(double)GetValue(sale, "UsedCount");
                    if (item.ManItemBy == ManageItemBy.SerialNumbers)
                    {
                        var serialNumber = serialNumbers.FirstOrDefault(i => i.ItemID == itemId) ?? new SerialNumber();
                        decimal totalCreated = 0;
                        if (serialNumber.SerialNumberSelected != null)
                        {
                            if (serialNumber.SerialNumberSelected.SerialNumberSelectedDetails != null)
                            {
                                serialNumber.SerialNumberSelected.SerialNumberSelectedDetails = serialNumber.SerialNumberSelected.SerialNumberSelectedDetails
                                    .GroupBy(i => i.SerialNumber).Select(i => i.FirstOrDefault()).ToList();
                                totalCreated = serialNumber.SerialNumberSelected.SerialNumberSelectedDetails.Count;
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
                                BaseQty = _qty,
                                OpenQty = qty - totalCreated,
                                Qty = qty,
                                TotalSelected = totalCreated,
                                WhsCode = whs.Code,
                                BpId = (int)GetValue(sale, "CusId"),
                                SaleID = saleID,
                                SerialNumberSelected = new SerialNumberSelected(),
                                SerialNumberUnselected = new SerialNumberUnselected(),
                                WareId = wareId,
                            });
                        }
                        else if (serialNumber.SerialNumberSelected.SerialNumberSelectedDetails == null)
                        {
                            serialNumbers.Add(new SerialNumber
                            {
                                ItemCode = item.Code,
                                ItemName = item.KhmerName,
                                ItemID = itemId,
                                UomID = uomId,
                                Direction = "Out",
                                BaseQty = _qty,
                                OpenQty = qty - totalCreated,
                                Qty = qty,
                                TotalSelected = totalCreated,
                                WhsCode = whs.Code,
                                BpId = (int)GetValue(sale, "CusId"),
                                SaleID = saleID,
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
                            serialNumber.BaseQty = _qty;
                        }
                    }
                }
            }
        }
        public void CheckItemBatchKSMS<T, TD>(T sale, List<TD> detials, List<BatchNo> batchNoes, int saleID, int wareId)
        {
            var whs = _context.Warehouses.Find(wareId) ?? new Warehouse();
            if (detials.Count > 0)
            {
                foreach (var sd in detials)
                {
                    int itemId = (int)GetValue(sd, "ItemID");
                    int uomId = (int)GetValue(sd, "UomID");
                    ItemMasterData item = _context.ItemMasterDatas.Find(itemId) ?? new ItemMasterData();
                    var uom = _context.GroupDUoMs.FirstOrDefault(w => w.GroupUoMID == item.GroupUomID && w.AltUOM == uomId);
                    decimal qty = (decimal)GetValue(sd, "Qty");
                    decimal _qty = qty * (decimal)(uom.Factor * (double)GetValue(sale, "UsedCount"));
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
                                batchNo.BatchNoSelected.BatchNoSelectedDetails = batchNo.BatchNoSelected.BatchNoSelectedDetails
                                    .GroupBy(i => i.BatchNo).Select(i => i.FirstOrDefault()).ToList();
                                totalCreated = batchNo.BatchNoSelected.BatchNoSelectedDetails.Sum(i => i.SelectedQty);
                                totalBatches = batchNo.BatchNoSelected.BatchNoSelectedDetails.Count;

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
                                TotalNeeded = _qty - totalCreated,
                                Qty = _qty,
                                BaseQty = qty,
                                TotalSelected = totalCreated,
                                WhsCode = whs.Code,
                                SaleID = saleID,
                                BpId = (int)GetValue(sale, "CusId"),
                                BatchNoSelected = new BatchNoSelected(),
                                BatchNoUnselect = new BatchNoUnselect(),
                                WareId = wareId,
                            });
                        }
                        else if (batchNo.BatchNoUnselect.BatchNoUnselectDetails == null)
                        {
                            batchNoes.Add(new BatchNo
                            {
                                ItemCode = item.Code,
                                ItemName = item.KhmerName,
                                ItemID = itemId,
                                UomID = uomId,
                                Direction = "Out",
                                TotalBatches = totalBatches,
                                TotalNeeded = _qty - totalCreated,
                                Qty = _qty,
                                BaseQty = qty,
                                TotalSelected = totalCreated,
                                WhsCode = whs.Code,
                                SaleID = saleID,
                                BpId = (int)GetValue(sale, "CusId"),
                                BatchNoSelected = new BatchNoSelected(),
                                BatchNoUnselect = new BatchNoUnselect(),
                                WareId = wareId,
                            });
                        }
                        else
                        {
                            batchNo.TotalBatches = totalBatches;
                            batchNo.TotalNeeded = _qty - totalCreated;
                            batchNo.Qty = _qty;
                            batchNo.BaseQty = qty;
                            batchNo.TotalSelected = totalCreated;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// End KSMS Block
        /// </summary>

        public async Task<SerialNumberUnselected> GetSerialDetialsAsync(int itemId, int wareId)
        {
           
            List<SerialNumberUnselectedDetial> serialNumberUnselectedDetials = await _context.WarehouseDetails
                .Where(i => i.InStock > 0 && i.ItemID == itemId && !string.IsNullOrEmpty(i.SerialNumber) && i.WarehouseID == wareId)
                .Select(i => new SerialNumberUnselectedDetial
                {
                    // Qty = (decimal)i.InStock,
                    // SerialNumber = i.SerialNumber,
                    // LotNumber = i.LotNumber,
                    // MfrSerialNo = i.MfrSerialNumber,
                    // UnitCost = Convert.ToDecimal(i.Cost),
                    // ExpireDate = i.ExpireDate
                    Qty = (decimal)i.InStock,
                    SerialNumber = i.SerialNumber,
                    LotNumber = i.LotNumber,
                    MfrSerialNo = i.MfrSerialNumber=="NULL"?"":i.MfrSerialNumber,
                    UnitCost = Convert.ToDecimal(i.Cost),
                    ExpireDate = i.ExpireDate == default? string.Empty : i.ExpireDate.ToString("dd-MM-yyyy"),

                    PlateNumber =i.PlateNumber=="NULL"? "":i.PlateNumber,
                    Color =i.Color=="NULL"?"":i.Color,
                    Brand =i.Brand=="NULL"?"":i.Brand,
                    Condition = i.Condition=="NULL"?"":i.Condition,
                    Type =i.Type=="NULL"?"":i.Type,
                    Power = i.Power=="NULL"?"":i.Power,
                    Year =i.Year=="NULL"?"":i.Year,                 
                      MfrDate = !i.MfrDate.HasValue || i.MfrDate == default ? "" : i.MfrDate.GetValueOrDefault().ToString("dd-MM-yyyy"),
                      AdmissionDate =!i.MfrDate.HasValue||i.AdmissionDate == default ? "" : i.AdmissionDate.GetValueOrDefault().ToString("dd-MM-yyyy"),
                      MfrWarDateStart = !i.MfrWarDateStart.HasValue ||i.MfrWarDateStart == default ? "" : i.MfrWarDateStart.GetValueOrDefault().ToString("dd-MM-yyyy"),
                      MfrWarDateEnd =!i.MfrWarDateEnd.HasValue||i.MfrWarDateEnd == default ? "" : i.MfrWarDateEnd.GetValueOrDefault().ToString("dd-MM-yyyy"),
                     Location =i.Location=="NULL"?"":i.Location,
                     Details =i.Details=="NULL"?"":i.Details,
                }).ToListAsync();
            SerialNumberUnselected data = new()
            {
                SerialNumberUnselectedDetials = serialNumberUnselectedDetials.OrderBy(i => i.SerialNumber).ToList(),
                TotalAvailableQty = serialNumberUnselectedDetials.Sum(i => i.Qty),
            };
            return await Task.FromResult(data);
        }
        public async Task<SerialNumberUnselected> GetSerialDetialsReturnItemAsync(int itemId, int saleId, int wareId)
        {
            List<SerialNumberUnselectedDetial> serialNumberUnselectedDetials = await _context.StockOuts
                .Where(i =>
                    i.WarehouseID == wareId
                    && i.TransID == saleId
                    && i.InStock > 0
                    && i.ItemID == itemId
                    && i.TransType == TransTypeWD.POS
                    && !string.IsNullOrEmpty(i.SerialNumber))
                .Select(i => new SerialNumberUnselectedDetial
                {
                    // Qty = i.InStock,
                    // SerialNumber = i.SerialNumber,
                    // LotNumber = i.LotNumber,
                    // MfrSerialNo = i.MfrSerialNumber,
                    // UnitCost = i.Cost,
                    // ExpireDate = (DateTime)i.ExpireDate,
                     Qty = (decimal)i.InStock,
                    SerialNumber = i.SerialNumber,
                    LotNumber = i.LotNumber,
                    MfrSerialNo = i.MfrSerialNumber=="NULL"?"":i.MfrSerialNumber,
                    UnitCost = Convert.ToDecimal(i.Cost),
                    ExpireDate = !i.ExpireDate.HasValue||i.ExpireDate == default? string.Empty : i.ExpireDate.GetValueOrDefault().ToString("dd-MM-yyyy"),

                    PlateNumber =i.PlateNumber=="NULL"?"":i.PlateNumber,
                    Color =i.Color=="NULL"?"":i.Color,
                    Brand =i.Brand=="NULL"?"":i.Brand,
                    Condition = i.Condition=="NULL"?"":i.Condition,
                    Type =i.Type=="NULL"?"":i.Type,
                    Power = i.Power=="NULL"?"":i.Power,
                    Year =i.Year=="NULL"?"":i.Year,                 
                      MfrDate = !i.MfrDate.HasValue || i.MfrDate == default ? "" : i.MfrDate.GetValueOrDefault().ToString("dd-MM-yyyy"),
                      AdmissionDate =!i.MfrDate.HasValue||i.AdmissionDate == default ? "" : i.AdmissionDate.GetValueOrDefault().ToString("dd-MM-yyyy"),
                      MfrWarDateStart = !i.MfrWarDateStart.HasValue || i.MfrWarDateStart == default ? "" : i.MfrWarDateStart.GetValueOrDefault().ToString("dd-MM-yyyy"),
                      MfrWarDateEnd = !i.MfrWarDateEnd.HasValue || i.MfrWarDateEnd == default ? "" : i.MfrWarDateEnd.GetValueOrDefault().ToString("dd-MM-yyyy"),
                     Location =i.Location=="NULL"?"":i.Location,
                     Details =i.Details=="NULL"?"":i.Details,
                }).ToListAsync();
            SerialNumberUnselected data = new()
            {
                SerialNumberUnselectedDetials = serialNumberUnselectedDetials.OrderBy(i => i.SerialNumber).ToList(),
                TotalAvailableQty = serialNumberUnselectedDetials.Sum(i => i.Qty),
            };
            return await Task.FromResult(data);
        }
        public async Task<BatchNoUnselect> GetBatchNoDetialsAsync(int itemId, int uomID, int wareId)
        {
            var item = _context.ItemMasterDatas.Find(itemId) ?? new ItemMasterData();
            var uom = _context.GroupDUoMs.FirstOrDefault(w => w.GroupUoMID == item.GroupUomID && w.AltUOM == uomID) ?? new GroupDUoM();
            double factor = uom.Factor;
            var batchNoUnselectDetials = await (from whd in _context.WarehouseDetails
                                                 .Where(i =>
                                                    i.WarehouseID == wareId
                                                    && i.InStock > 0
                                                    && i.ItemID == itemId
                                                    && !string.IsNullOrEmpty(i.BatchNo))
                                                group whd by whd.BatchNo into g
                                                let _data = g.FirstOrDefault()
                                                select new BatchNoUnselectDetail
                                                {
                                                    AvailableQty = (decimal)(g.Sum(i => i.InStock) / factor),
                                                    BatchNo = _data.BatchNo,
                                                    SelectedQty = 0,
                                                    UnitCost = (decimal)(_data.Cost * factor),
                                                    BPID = _data.BPID,
                                                    OrigialQty = (decimal)(g.Sum(i => i.InStock) / factor),
                                                    ExpireDate = _data.ExpireDate
                                                }).ToListAsync();


            BatchNoUnselect data = new()
            {
                BatchNoUnselectDetails = batchNoUnselectDetials.OrderBy(i => i.BatchNo).ToList(),
                TotalAvailableQty = batchNoUnselectDetials.Sum(i => i.AvailableQty),
            };
            return await Task.FromResult(data);
        }
        public async Task<BatchNoUnselect> GetBatchNoDetialsReturnItemAsync(int itemId, int uomID, int saleId, int wareId)
        {
            var item = _context.ItemMasterDatas.Find(itemId) ?? new ItemMasterData();
            var uom = _context.GroupDUoMs.FirstOrDefault(w => w.GroupUoMID == item.GroupUomID && w.AltUOM == uomID) ?? new GroupDUoM();
            decimal factor = (decimal)uom.Factor;
            List<BatchNoUnselectDetail> batchNoUnselectDetials = await _context.StockOuts
                .Where(i =>
                    i.WarehouseID == wareId
                    && i.TransID == saleId
                    && i.InStock > 0
                    && i.ItemID == itemId
                    && i.TransType == TransTypeWD.POS
                    && !string.IsNullOrEmpty(i.BatchNo))
                .Select(i => new BatchNoUnselectDetail
                {
                    AvailableQty = i.InStock / factor,
                    BatchNo = i.BatchNo,
                    SelectedQty = 0,
                    UnitCost = i.Cost * factor,
                    BPID = i.BPID,
                    OrigialQty = i.InStock / factor,
                    ExpireDate = (DateTime)i.ExpireDate,
                }).ToListAsync();
            BatchNoUnselect data = new()
            {
                BatchNoUnselectDetails = batchNoUnselectDetials.OrderBy(i => i.BatchNo).ToList(),
                TotalAvailableQty = batchNoUnselectDetials.Sum(i => i.AvailableQty),
            };
            return await Task.FromResult(data);
        }

        public void CheckItemSerailReturn<T, TD>(T sale, List<TD> detials, List<SerialNumber> serialNumbers, string direction)
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
                    var serialNumber = serialNumbers.FirstOrDefault(i => i.ItemID == itemId) ?? new SerialNumber();
                    decimal qty = (decimal)((double)GetValue(sd, "Qty") * uom.Factor);
                    if (item.ManItemBy == ManageItemBy.SerialNumbers)
                    {
                        serialNumbers.Add(new SerialNumber
                        {
                            ItemCode = item.Code,
                            ItemName = item.KhmerName,
                            ItemID = itemId,
                            UomID = uomId,
                            Direction = direction,
                            OpenQty = 0,
                            Qty = qty,
                            TotalSelected = qty,
                            WhsCode = whs.Code,
                            BpId = (int)GetValue(sale, "CustomerID"),
                            SaleID = (int)GetValue(sale, "ReceiptID"),
                            SerialNumberSelected = serialNumber.SerialNumberSelected ?? new SerialNumberSelected(),
                            SerialNumberUnselected = new SerialNumberUnselected(),
                            WareId = wareId,
                        });
                    }

                }
            }
        }
        public void CheckItemBatchReturn<T, TD>(T sale, List<TD> detials, List<BatchNo> batchNoes, string direction, TransTypeWD transTypeWD)
        {
            int wareid = (int)GetValue(sale, "WarehouseID");
            var whs = _context.Warehouses.Find(wareid) ?? new Warehouse();
            if (detials.Count > 0)
            {
                foreach (var sd in detials)
                {
                    int itemId = (int)GetValue(sd, "ItemID");
                    int uomId = (int)GetValue(sd, "UomID");
                    int saleId = (int)GetValue(sale, "ReceiptID");
                    ItemMasterData item = _context.ItemMasterDatas.Find(itemId) ?? new ItemMasterData();
                    var uom = _context.GroupDUoMs.FirstOrDefault(w => w.GroupUoMID == item.GroupUomID && w.AltUOM == uomId);
                    //var batched = _context.WarehouseDetails.Where(i => i.SaleID == saleId && i.TransType == transTypeWD && i.ItemID == itemId && !String.IsNullOrEmpty(i.BatchNo)).ToList();
                    var batchedNo = batchNoes.FirstOrDefault(i => i.ItemID == itemId) ?? new BatchNo();
                    double _qty = (double)GetValue(sd, "Qty");
                    if (item.ManItemBy == ManageItemBy.Batches)
                    {
                        decimal qty = (decimal)_qty;
                        batchNoes.Add(new BatchNo
                        {
                            ItemCode = item.Code,
                            ItemName = item.KhmerName,
                            ItemID = itemId,
                            UomID = uomId,
                            Direction = direction,
                            TotalBatches = 0,
                            TotalNeeded = 0,
                            Qty = qty,
                            TotalSelected = qty,
                            WhsCode = whs.Code,
                            BpId = (int)GetValue(sale, "CustomerID"),
                            SaleID = saleId,
                            BatchNoSelected = batchedNo.BatchNoSelected ?? new BatchNoSelected(),
                            BatchNoUnselect = new BatchNoUnselect(),
                            WareId = wareid,
                        });
                    }

                }
            }
        }
        public async Task<SerialNumberSelected> GetSerialDetialsReturnAsync(int itemId, int saleId, int wareId, TransTypeWD transType)
        {
            List<SerialNumberSelectedDetail> serialNumberSelectedDetials = await _context.StockOuts
                .Where(i =>
                    i.WarehouseID == wareId
                    && i.ItemID == itemId &&
                    !string.IsNullOrEmpty(i.SerialNumber) &&
                    i.TransID == saleId &&
                    i.TransType == transType &&
                    i.InStock > 0
                )
                .Select(i => new SerialNumberSelectedDetail
                {
                    // Qty = i.InStock,
                    // SerialNumber = i.SerialNumber,
                    // LotNumber = i.LotNumber,
                    // MfrSerialNo = i.MfrSerialNumber,
                    // UnitCost = i.Cost,
                    // ExpireDate = (DateTime)i.ExpireDate,
                    Qty = (decimal)i.InStock,
                    SerialNumber = i.SerialNumber,
                    LotNumber = i.LotNumber,
                    MfrSerialNo = i.MfrSerialNumber=="NULL"?"":i.MfrSerialNumber,
                    UnitCost = Convert.ToDecimal(i.Cost),
                    ExpireDate = !i.ExpireDate.HasValue||i.ExpireDate == default? string.Empty : i.ExpireDate.GetValueOrDefault().ToString("dd-MM-yyyy"),

                    PlateNumber =i.PlateNumber=="NULL"?"":i.PlateNumber,
                    Color =i.Color=="NULL"?"":i.Color,
                    Brand =i.Brand=="NULL"?"":i.Brand,
                    Condition = i.Condition=="NULL"?"":i.Condition,
                    Type =i.Type=="NULL"?"":i.Type,
                    Power = i.Power=="NULL"?"":i.Power,
                    Year =i.Year=="NULL"?"":i.Year,                 
                      MfrDate = !i.MfrDate.HasValue || i.MfrDate == default ? "" : i.MfrDate.GetValueOrDefault().ToString("dd-MM-yyyy"),
                      AdmissionDate =!i.AdmissionDate.HasValue||i.AdmissionDate == default ? "" : i.AdmissionDate.GetValueOrDefault().ToString("dd-MM-yyyy"),
                      MfrWarDateStart =!i.MfrWarDateStart.HasValue||i.MfrWarDateStart == default ? "" : i.MfrWarDateStart.GetValueOrDefault().ToString("dd-MM-yyyy"),
                      MfrWarDateEnd =!i.MfrWarDateEnd.HasValue||i.MfrWarDateEnd == default ? "" : i.MfrWarDateEnd.GetValueOrDefault().ToString("dd-MM-yyyy"),
                     Location =i.Location=="NULL"?"":i.Location,
                     Details =i.Details=="NULL"?"":i.Details,
                }).ToListAsync();
            SerialNumberSelected data = new()
            {
                SerialNumberSelectedDetails = serialNumberSelectedDetials.OrderBy(i => i.SerialNumber).ToList(),
                TotalSelected = serialNumberSelectedDetials.Sum(i => i.Qty),
            };
            return await Task.FromResult(data);
        }
        public async Task<BatchNoSelected> GetBatchNoDetialsReturnAsync(int itemId, int uomID, int saleId, int wareId, TransTypeWD transType)
        {
            var item = _context.ItemMasterDatas.Find(itemId) ?? new ItemMasterData();
            var uom = _context.GroupDUoMs.FirstOrDefault(w => w.GroupUoMID == item.GroupUomID && w.AltUOM == uomID) ?? new GroupDUoM();

            decimal factor = (decimal)uom.Factor;
            List<BatchNoSelectedDetail> batchNoSelectDetials = await _context.StockOuts
                .Where(i =>
                    i.WarehouseID == wareId
                    && i.ItemID == itemId
                    && !string.IsNullOrEmpty(i.BatchNo)
                    && i.TransID == saleId
                    && i.TransType == transType
                    && i.InStock > 0
                    )
                .Select(i => new BatchNoSelectedDetail
                {
                    AvailableQty = i.InStock / factor,
                    BatchNo = i.BatchNo,
                    SelectedQty = i.InStock / factor,
                    UnitCost = i.Cost * factor,
                    BPID = i.BPID,
                    OrigialQty = i.InStock / factor,
                    ExpireDate = (DateTime)i.ExpireDate,
                }).ToListAsync();
            BatchNoSelected data = new()
            {
                BatchNoSelectedDetails = batchNoSelectDetials.OrderBy(i => i.BatchNo).ToList(),
                TotalSelected = batchNoSelectDetials.Sum(i => i.AvailableQty),
            };
            return await Task.FromResult(data);
        }
        public void CheckCanRingItemSerail<T, TD>(T sale, List<TD> detials, List<SerialNumber> serialNumbers, string saleID)
        {
            int wareId = (int)GetValue(sale, "WarehouseID");
            var whs = _context.Warehouses.Find(wareId) ?? new Warehouse();
            if (detials.Count > 0)
            {
                foreach (var sd in detials)
                {
                    int itemId = (int)GetValue(sd, "ItemChangeID");
                    int uomId = (int)GetValue(sd, "UomChangeID");
                    ItemMasterData item = _context.ItemMasterDatas.Find(itemId) ?? new ItemMasterData();
                    var uom = _context.GroupDUoMs.FirstOrDefault(w => w.GroupUoMID == item.GroupUomID && w.AltUOM == uomId);
                    decimal qty = (decimal)GetValue(sd, "ChangeQty") * (decimal)uom.Factor;
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
        public void CheckCanRingItemBatch<T, TD>(T sale, List<TD> detials, List<BatchNo> batchNoes, string saleID)
        {
            int wareid = (int)GetValue(sale, "WarehouseID");
            var whs = _context.Warehouses.Find(wareid) ?? new Warehouse();
            if (detials.Count > 0)
            {
                foreach (var sd in detials)
                {
                    int itemId = (int)GetValue(sd, "ItemChangeID");
                    int uomId = (int)GetValue(sd, "UomChangeID");
                    ItemMasterData item = _context.ItemMasterDatas.Find(itemId) ?? new ItemMasterData();
                    var uom = _context.GroupDUoMs.FirstOrDefault(w => w.GroupUoMID == item.GroupUomID && w.AltUOM == uomId);
                    decimal qty = (decimal)GetValue(sd, "ChangeQty");
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
        private static object GetValue<T>(T obj, string prop)
        {
            var data = typeof(T).GetProperty(prop).GetValue(obj);
            return data ?? "";
        }
    }
}
