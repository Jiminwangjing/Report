using CKBS.AppContext;
using CKBS.Models.Services.Administrator.Inventory;
using CKBS.Models.Services.Inventory;
using KEDI.Core.Premise.Models.ServicesClass.SerialBatches.Sales;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.Services.Responsitory
{
    public interface ISaleSerialBatchRepository
    {
        void CheckItemSerail<T, TD>(T sale, List<TD> detials, List<SerialNumber> serialNumbers);
        void CheckItemBatch<T, TD>(T sale, List<TD> detials, List<BatchNo> batchNoes);
        void CheckItemSerailCopy<T, TD>(T sale, List<TD> detials, List<SerialNumber> serialNumbers, string diredction);
        Task<SerialNumberUnselected> GetSerialDetialsAsync(int itemId, int wareId);
        Task<BatchNoUnselect> GetBatchNoDetialsAsync(int itemId, int uomID, int wareId);
        Task<SerialNumberSelected> GetSerialDetialsCopyAsync(int itemId, int saleId, int wareId, TransTypeWD transType);
        Task<BatchNoSelected> GetBatchNoDetialsCopyAsync(int itemId, int uomID, int saleId, int wareId, TransTypeWD transType);
        Task<SerialNumberUnselected> GetSerialDetialsReturnDeliveryAsync(int itemId, int wareId, TransTypeWD transType);
        Task<BatchNoUnselect> GetBatchNoDetialsReturnDeliveryAsync(int itemId, int uomID, int wareId, TransTypeWD transType);
        Task<SerialNumberUnselected> GetSerialDetialsGoodsIssueAsync(int itemId, int wareId, decimal cost);


        void CheckItemBatchCopy<T, TD>(T sale, List<TD> detials, List<BatchNo> batchNoes, string direction, TransTypeWD transTypeWD);
        void CheckItemSerailGoodsIssue<T, TD>(T sale, List<TD> detials, List<SerialNumber> serialNumbers);
        void CheckItemBatchGoodsIssue<T, TD>(T sale, List<TD> detials, List<BatchNo> batchNoes);


        void CheckItemSerailTransfer<T, TD>(T sale, List<TD> detials, List<SerialNumber> serialNumbers);
        void CheckItemBatchTransfer<T, TD>(T sale, List<TD> detials, List<BatchNo> batchNoes);
    }
    public class SaleSerialBatchRepository : ISaleSerialBatchRepository
    {
        private readonly DataContext _context;

        public SaleSerialBatchRepository(DataContext context)
        {
            _context = context;
        }
        public void CheckItemSerail<T, TD>(T sale, List<TD> detials, List<SerialNumber> serialNumbers)
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
                                BpId = (int)GetValue(sale, "CusID"),
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
        public void CheckItemBatch<T, TD>(T sale, List<TD> detials, List<BatchNo> batchNoes)
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
                                BpId = (int)GetValue(sale, "CusID"),
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
        public void CheckItemSerailGoodsIssue<T, TD>(T sale, List<TD> detials, List<SerialNumber> serialNumbers)
        {
            int wareId = (int)GetValue(sale, "WarehouseID");
            var whs = _context.Warehouses.Find(wareId) ?? new Warehouse();
            if (detials.Count > 0)
            {
                foreach (var sd in detials)
                {
                    decimal cost = (decimal)((double)GetValue(sd, "Cost"));
                    int itemId = (int)GetValue(sd, "ItemID");
                    int uomId = (int)GetValue(sd, "UomID");
                    ItemMasterData item = _context.ItemMasterDatas.Find(itemId) ?? new ItemMasterData();
                    var uom = _context.GroupDUoMs.FirstOrDefault(w => w.GroupUoMID == item.GroupUomID && w.AltUOM == uomId);
                    decimal qty = (decimal)((double)GetValue(sd, "Quantity") * uom.Factor);
                    if (item.ManItemBy == ManageItemBy.SerialNumbers)
                    {
                        var serialNumber = serialNumbers.FirstOrDefault(i => i.ItemID == itemId && cost == i.Cost) ?? new SerialNumber();
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
                                        if (serialNumber.SerialNumberSelected.SerialNumberSelectedDetails.Any(j => j.SerialNumber == i.SerialNumber))
                                        {
                                            serialNumber.SerialNumberUnselected.SerialNumberUnselectedDetials.Remove(i);
                                        }
                                    }
                                }
                            }
                        }
                        if (serialNumber.SerialNumberSelected == null
                            || serialNumber.SerialNumberSelected.SerialNumberSelectedDetails == null)
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
                                Cost = cost,
                                SerialNumberUnselected = new SerialNumberUnselected(),
                                SerialNumberSelected = new SerialNumberSelected(),
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
        public void CheckItemBatchGoodsIssue<T, TD>(T sale, List<TD> detials, List<BatchNo> batchNoes)
        {
            int wareid = (int)GetValue(sale, "WarehouseID");
            var whs = _context.Warehouses.Find(wareid) ?? new Warehouse();
            if (detials.Count > 0)
            {
                foreach (var sd in detials)
                {
                    decimal cost = (decimal)((double)GetValue(sd, "Cost"));
                    int itemId = (int)GetValue(sd, "ItemID");
                    int uomId = (int)GetValue(sd, "UomID");
                    ItemMasterData item = _context.ItemMasterDatas.Find(itemId) ?? new ItemMasterData();
                    var uom = _context.GroupDUoMs.FirstOrDefault(w => w.GroupUoMID == item.GroupUomID && w.AltUOM == uomId);
                    double _qty = (double)GetValue(sd, "Quantity");
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
                                Cost = cost,
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
        public void CheckItemSerailTransfer<T, TD>(T sale, List<TD> detials, List<SerialNumber> serialNumbers)
        {
            int wareId = (int)GetValue(sale, "WarehouseFromID");
            var whs = _context.Warehouses.Find(wareId) ?? new Warehouse();
            if (detials.Count > 0)
            {
                foreach (var sd in detials)
                {
                    decimal cost = (decimal)((double)GetValue(sd, "Cost"));
                    int itemId = (int)GetValue(sd, "ItemID");
                    int uomId = (int)GetValue(sd, "UomID");
                    ItemMasterData item = _context.ItemMasterDatas.Find(itemId) ?? new ItemMasterData();
                    var uom = _context.GroupDUoMs.FirstOrDefault(w => w.GroupUoMID == item.GroupUomID && w.AltUOM == uomId);
                    decimal qty = (decimal)((double)GetValue(sd, "Qty") * uom.Factor);
                    if (item.ManItemBy == ManageItemBy.SerialNumbers)
                    {
                        var serialNumber = serialNumbers.FirstOrDefault(i => i.ItemID == itemId && cost == i.Cost) ?? new SerialNumber();
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
                                        if (serialNumber.SerialNumberSelected.SerialNumberSelectedDetails.Any(j => j.SerialNumber == i.SerialNumber))
                                        {
                                            serialNumber.SerialNumberUnselected.SerialNumberUnselectedDetials.Remove(i);
                                        }
                                    }
                                }
                            }
                        }
                        if (serialNumber.SerialNumberSelected == null
                            || serialNumber.SerialNumberSelected.SerialNumberSelectedDetails == null)
                        {
                            serialNumbers.Add(new SerialNumber
                            {
                                ItemCode = item.Code,
                                ItemName = item.KhmerName,
                                ItemID = itemId,
                                UomID = uomId,
                                Direction = "Transfer",
                                OpenQty = qty - totalCreated,
                                Qty = qty,
                                TotalSelected = totalCreated,
                                WhsCode = whs.Code,
                                Cost = cost,
                                SerialNumberUnselected = new SerialNumberUnselected(),
                                SerialNumberSelected = new SerialNumberSelected(),
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
        public void CheckItemBatchTransfer<T, TD>(T sale, List<TD> detials, List<BatchNo> batchNoes)
        {
            int wareid = (int)GetValue(sale, "WarehouseFromID");
            var whs = _context.Warehouses.Find(wareid) ?? new Warehouse();
            if (detials.Count > 0)
            {
                foreach (var sd in detials)
                {
                    decimal cost = (decimal)((double)GetValue(sd, "Cost"));
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
                                Direction = "Transfer",
                                TotalBatches = totalBatches,
                                TotalNeeded = qty - totalCreated,
                                Qty = qty,
                                TotalSelected = totalCreated,
                                WhsCode = whs.Code,
                                Cost = cost,
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
        public void CheckItemSerailCopy<T, TD>(T sale, List<TD> detials, List<SerialNumber> serialNumbers, string direction)
        {
            var whs = _context.Warehouses.Find((int)GetValue(sale, "WarehouseID")) ?? new Warehouse();
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
                            BpId = (int)GetValue(sale, "CusID"),
                            SaleID = (int)GetValue(sale, "BasedOn"),
                            SerialNumberSelected = serialNumber.SerialNumberSelected,
                            WareId = whs.ID,
                        });
                    }

                }
            }
        }
        public void CheckItemBatchCopy<T, TD>(T sale, List<TD> detials, List<BatchNo> batchNoes, string direction, TransTypeWD transTypeWD)
        {
            int wareid = (int)GetValue(sale, "WarehouseID");
            var whs = _context.Warehouses.Find(wareid) ?? new Warehouse();
            if (detials.Count > 0)
            {
                foreach (var sd in detials)
                {
                    int itemId = (int)GetValue(sd, "ItemID");
                    int uomId = (int)GetValue(sd, "UomID");
                    int baseOnId = (int)GetValue(sale, "BasedOn");
                    ItemMasterData item = _context.ItemMasterDatas.Find(itemId) ?? new ItemMasterData();
                    var uom = _context.GroupDUoMs.FirstOrDefault(w => w.GroupUoMID == item.GroupUomID && w.AltUOM == uomId);
                    //var batched = _context.WarehouseDetails.Where(i => i.SaleID == baseOnId && i.TransType == transTypeWD && i.ItemID == itemId && !String.IsNullOrEmpty(i.BatchNo)).ToList();
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
                            BpId = (int)GetValue(sale, "CusID"),
                            SaleID = baseOnId,
                            BatchNoSelected = batchedNo.BatchNoSelected,
                            WareId = wareid,
                        });
                    }

                }
            }
        }
        public async Task<SerialNumberUnselected> GetSerialDetialsAsync(int itemId, int wareId)
        {
            List<SerialNumberUnselectedDetial> serialNumberUnselectedDetials = await _context.WarehouseDetails
                .Where(i => i.InStock > 0 && i.WarehouseID == wareId && i.ItemID == itemId && !string.IsNullOrEmpty(i.SerialNumber))
                .Select(i => new SerialNumberUnselectedDetial
                {
                    //Qty = (decimal)i.InStock,
                    //SerialNumber = i.SerialNumber,
                    //LotNumber = i.LotNumber,
                    //MfrSerialNo = i.MfrSerialNumber,
                    //PlateNumber = i.PlateNumber,
                    // Color       = i.Color,
                    //Brand       = i.Brand,
                    //Condition   = i.Condition,
                    //Type        = i.Type,
                    //Power       = i.Power,
                    //Year        = i.Year,
                    //UnitCost = Convert.ToDecimal(i.Cost),
                    //ExpireDate = i.ExpireDate == null || i.ExpireDate == default ? string.Empty
                    //            : DateTime.ParseExact(i.MfrDate.ToString(), "dd-MM-yyyy", null).ToString(),
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
        public async Task<SerialNumberUnselected> GetSerialDetialsGoodsIssueAsync(int itemId, int wareId, decimal cost)
        {
            List<SerialNumberUnselectedDetial> serialNumberUnselectedDetials = await _context.WarehouseDetails
                .Where(i => i.InStock > 0 && i.WarehouseID == wareId && i.ItemID == itemId && !string.IsNullOrEmpty(i.SerialNumber) && cost == (decimal)i.Cost)
                .Select(i => new SerialNumberUnselectedDetial
                {
                    //Qty = (decimal)i.InStock,
                    //SerialNumber = i.SerialNumber,
                    //LotNumber = i.LotNumber,
                    //PlateNumber = i.PlateNumber,
                    //MfrSerialNo = i.MfrSerialNumber,
                    // Color       = i.Color,
                    //Brand       = i.Brand,
                    //Condition   = i.Condition,
                    //Type        = i.Type,
                    //Power       = i.Power,
                    //Year        = i.Year,
                    //UnitCost = Convert.ToDecimal(i.Cost),
                    //ExpireDate = i.ExpireDate
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
        public async Task<SerialNumberUnselected> GetSerialDetialsReturnDeliveryAsync(int itemId, int wareId, TransTypeWD transType)
        {
            List<SerialNumberUnselectedDetial> serialNumberUnselectedDetials = await _context.StockOuts
                .Where(i =>
                    i.WarehouseID == wareId &&
                    i.ItemID == itemId &&
                    !string.IsNullOrEmpty(i.SerialNumber) &&
                    i.TransType == transType && i.InStock > 0
                )
                .Select(i => new SerialNumberUnselectedDetial
                {
                    //Qty = i.InStock,
                    //SerialNumber = i.SerialNumber,
                    //PlateNumber = i.PlateNumber,
                    //LotNumber = i.LotNumber,
                    //MfrSerialNo = i.MfrSerialNumber,
                    // Color       = i.Color,
                    //Brand       = i.Brand,
                    //Condition   = i.Condition,
                    //Type        = i.Type,
                    //Power       = i.Power,
                    //Year        = i.Year,
                    //UnitCost = i.Cost,
                    //ExpireDate = (DateTime)i.ExpireDate
                     Qty = (decimal)i.InStock,
                    SerialNumber = i.SerialNumber,
                    LotNumber = i.LotNumber,
                    MfrSerialNo = i.MfrSerialNumber=="NULL"?"":i.MfrSerialNumber,
                    UnitCost = Convert.ToDecimal(i.Cost),
                    ExpireDate = !i.ExpireDate.HasValue||i.ExpireDate == default? string.Empty : i.ExpireDate.GetValueOrDefault().ToString("dd-MM-yyyy"),

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
        public async Task<BatchNoUnselect> GetBatchNoDetialsReturnDeliveryAsync(int itemId, int uomID, int wareId, TransTypeWD transType)
        {
            var item = _context.ItemMasterDatas.Find(itemId) ?? new ItemMasterData();
            var uom = _context.GroupDUoMs
                .FirstOrDefault(w => w.GroupUoMID == item.GroupUomID && w.AltUOM == uomID) ?? new GroupDUoM();
            decimal factor = (decimal)uom.Factor;
            List<BatchNoUnselectDetail> batchNoUnselectDetials = await _context.StockOuts
                .Where(i =>
                    i.WarehouseID == wareId
                    && i.ItemID == itemId
                    && !string.IsNullOrEmpty(i.BatchNo)
                    && i.TransType == transType && i.InStock > 0
                    )
                .Select(i => new BatchNoUnselectDetail
                {
                    AvailableQty = i.InStock / factor,
                    BatchNo = i.BatchNo,
                    UnitCost = i.Cost * factor,
                    BPID = 0,
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
        public async Task<BatchNoUnselect> GetBatchNoDetialsAsync(int itemId, int uomID, int wareId)
        {
            var item = _context.ItemMasterDatas.Find(itemId) ?? new ItemMasterData();
            var uom = _context.GroupDUoMs.FirstOrDefault(w => w.GroupUoMID == item.GroupUomID && w.AltUOM == uomID) ?? new GroupDUoM();
            List<BatchNoUnselectDetail> batchNoUnselectDetials = await _context.WarehouseDetails
                .Where(i => i.InStock > 0 && i.WarehouseID == wareId && i.ItemID == itemId && !string.IsNullOrEmpty(i.BatchNo))
                .Select(i => new BatchNoUnselectDetail
                {
                    AvailableQty = (decimal)(i.InStock / uom.Factor),
                    BatchNo = i.BatchNo,
                    SelectedQty = 0,
                    UnitCost = (decimal)(i.Cost * uom.Factor),
                    BPID = i.BPID,
                    OrigialQty = (decimal)(i.InStock / uom.Factor),
                    ExpireDate = i.ExpireDate
                }).ToListAsync();
            BatchNoUnselect data = new()
            {
                BatchNoUnselectDetails = batchNoUnselectDetials.OrderBy(i => i.BatchNo).ToList(),
                TotalAvailableQty = batchNoUnselectDetials.Sum(i => i.AvailableQty),
            };
            return await Task.FromResult(data);
        }
        public async Task<SerialNumberSelected> GetSerialDetialsCopyAsync(int itemId, int saleId, int wareId, TransTypeWD transType)
        {
            List<SerialNumberSelectedDetail> serialNumberSelectedDetials = await _context.StockOuts
                .Where(i =>
                    i.WarehouseID == wareId &&
                    i.ItemID == itemId &&
                    !string.IsNullOrEmpty(i.SerialNumber) &&
                    i.TransID == saleId &&
                    i.TransType == transType &&
                    i.InStock > 0
                )
                .Select(i => new SerialNumberSelectedDetail
                {
                    //Qty = i.InStock,
                    //SerialNumber = i.SerialNumber,
                    //PlateNumber = i.PlateNumber,
                    //LotNumber = i.LotNumber,
                    //MfrSerialNo = i.MfrSerialNumber,
                    // Color       = i.Color,
                    //Brand       = i.Brand,
                    //Condition   = i.Condition,
                    //Type        = i.Type,
                    //Power       = i.Power,
                    //Year        = i.Year,
                    //UnitCost = i.Cost,
                    //ExpireDate = (DateTime)i.ExpireDate,
                    Qty = (decimal)i.InStock,
                    SerialNumber = i.SerialNumber,
                    LotNumber = i.LotNumber,
                    MfrSerialNo = i.MfrSerialNumber=="NULL"?"":i.MfrSerialNumber,
                    UnitCost = Convert.ToDecimal(i.Cost),
                    ExpireDate = !i.ExpireDate.HasValue||i.ExpireDate == default? string.Empty : i.ExpireDate.GetValueOrDefault().ToString("dd-MM-yyyy"),

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
            SerialNumberSelected data = new()
            {
                SerialNumberSelectedDetails = serialNumberSelectedDetials.OrderBy(i => i.SerialNumber).ToList(),
                TotalSelected = serialNumberSelectedDetials.Sum(i => i.Qty),
            };
            return await Task.FromResult(data);
        }
        public async Task<BatchNoSelected> GetBatchNoDetialsCopyAsync(int itemId, int uomID, int saleId, int wareId, TransTypeWD transType)
        {
            var item = _context.ItemMasterDatas.Find(itemId) ?? new ItemMasterData();
            var uom = _context.GroupDUoMs.FirstOrDefault(w => w.GroupUoMID == item.GroupUomID && w.AltUOM == uomID) ?? new GroupDUoM();
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
                    AvailableQty = i.InStock / (decimal)uom.Factor,
                    BatchNo = i.BatchNo,
                    SelectedQty = i.InStock / (decimal)uom.Factor,
                    UnitCost = i.Cost * (decimal)uom.Factor,
                    BPID = 0,
                    OrigialQty = i.InStock / (decimal)uom.Factor,
                    ExpireDate = (DateTime)i.ExpireDate
                }).ToListAsync();
            BatchNoSelected data = new()
            {
                BatchNoSelectedDetails = batchNoSelectDetials.OrderBy(i => i.BatchNo).ToList(),
                TotalSelected = batchNoSelectDetials.Sum(i => i.AvailableQty),
            };
            return await Task.FromResult(data);
        }
        private static object GetValue<T>(T obj, string prop)
        {
            var data = typeof(T).GetProperty(prop).GetValue(obj);
            return data ?? "";
        }
    }
}
