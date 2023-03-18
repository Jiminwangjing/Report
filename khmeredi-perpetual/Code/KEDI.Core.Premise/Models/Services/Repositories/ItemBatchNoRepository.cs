using CKBS.AppContext;
using CKBS.Models.Services.Administrator.Inventory;
using CKBS.Models.Services.Inventory;
using KEDI.Core.Premise.Models.ServicesClass.SerialBatches.BatchNoManagement;
using KEDI.Core.Premise.Models.ServicesClass.SerialBatches.SerialNumbersManagement;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.Services.Responsitory
{
    public interface IItemBatchNoRepository
    {
        List<CreatedBatchNoViewModel> GetBatchCreated(int itemId, int saleId, TransTypeWD transType);
        List<CreatedBatchNoViewModel> GetBatchCreated(string propId, int itemId, int saleId, TransTypeWD transType);
        Task<List<BatchNoDocumentViewModel>> GetBatchNoDocAsync(SNFilter filter, ItemMasterData item);
        List<BatchNoDetialViewModel> GetBatchNoDetails(string itemcode);
        Task<List<ItemMasterData>> GetItemMasterDataAsync();
        void UpdateBatchNo(List<BatchNoDocumentViewModel> batches);
        void UpdateBatchNoDetial(BatchNoDetialViewModel batchDetail);
    }

    public class ItemBatchNoRepository : IItemBatchNoRepository
    {
        private readonly DataContext _context;

        public ItemBatchNoRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<List<ItemMasterData>> GetItemMasterDataAsync()
        {
            var data = await _context.ItemMasterDatas.Where(i =>
                i.Delete == false && i.ManItemBy == ManageItemBy.Batches
            ).Select(i => new ItemMasterData
            {
                Code = i.Code,
                ID = i.ID,
                KhmerName = i.KhmerName,
                Barcode = i.Barcode ?? "",
            }).ToListAsync();
            return data;
        }
        public async Task<List<BatchNoDocumentViewModel>> GetBatchNoDocAsync(SNFilter filter, ItemMasterData item)
        {
            List<BatchNoDocumentViewModel> batchesPO = new();
            List<BatchNoDocumentViewModel> batchesAP = new();
            List<BatchNoDocumentViewModel> batchesAPCredit = new();
            List<BatchNoDocumentViewModel> batchesDeli = new();
            List<BatchNoDocumentViewModel> batchesReDeli = new();
            List<BatchNoDocumentViewModel> batchesAR = new();
            List<BatchNoDocumentViewModel> batchesARCredit = new();
            List<BatchNoDocumentViewModel> batchesGI = new();
            List<BatchNoDocumentViewModel> batchesGR = new();
            var whStockIn = _context.WarehouseDetails.Where(i=> !string.IsNullOrEmpty(i.BatchNo)).ToList();
            var whStockOut = _context.StockOuts.Where(i => !string.IsNullOrEmpty(i.BatchNo)).ToList();
            if (filter.GoodsReceiptPO)
            {
                var data = _context.GoodsReciptPOs.ToList();
                batchesPO = GetSerialDoc("ID", "InStockFrom", "SeriesID", "SeriesDetailID", data, whStockIn, item, TransTypeWD.GRPO);
            }
            if (filter.APInvoices)
            {
                var data = _context.Purchase_APs.ToList();
                batchesAP = GetSerialDoc("PurchaseAPID", "InStockFrom", "SeriesID", "SeriesDetailID", data, whStockIn, item, TransTypeWD.PurAP);
            }
            if (filter.APCreditMemos)
            {
                var data = _context.PurchaseCreditMemos.ToList();
                batchesAPCredit = GetSerialDoc("PurchaseMemoID", "TransID", "SeriesID", "SeriesDetailID", data, whStockOut, item, TransTypeWD.PurCreditMemo);
            }
            if (filter.Deliveries)
            {
                var data = _context.SaleDeliveries.ToList();
                batchesDeli = GetSerialDoc("SDID", "TransID", "SeriesID", "SeriesDID", data, whStockOut, item, TransTypeWD.Delivery);
            }
            if (filter.ReturnDeliveries)
            {
                var data = _context.ReturnDeliverys.ToList();
                batchesReDeli = GetSerialDoc("ID", "InStockFrom", "SeriesID", "SeriesDID", data, whStockIn, item, TransTypeWD.ReturnDelivery);
            }
            if (filter.ARInvoices)
            {
                var data = _context.SaleARs.ToList();
                batchesAR = GetSerialDoc("SARID", "TransID", "SeriesID", "SeriesDID", data, whStockOut, item, TransTypeWD.AR);
            }
            if (filter.ARCreditMemos)
            {
                var data = _context.SaleCreditMemos.ToList();
                batchesARCredit = GetSerialDoc("SCMOID", "InStockFrom", "SeriesID", "SeriesDID", data, whStockIn, item, TransTypeWD.CreditMemo);
            }
            if (filter.GoodsIssue)
            {
                var data = _context.GoodIssues.ToList();
                batchesGI = GetSerialDoc("GoodIssuesID", "TransID", "SeriseID", "SeriseDID", data, whStockOut, item, TransTypeWD.GoodsIssue);
            }
            if (filter.GoodsReciept)
            {
                var data = _context.GoodsReceipts.ToList();
                batchesGR = GetSerialDoc("GoodsReceiptID", "InStockFrom", "SeriseID", "SeriseDID", data, whStockIn, item, TransTypeWD.GoodsReceipt);
            }
            List<BatchNoDocumentViewModel> summary = new
                (
                batchesPO.Count + batchesAP.Count +
                batchesAPCredit.Count + batchesDeli.Count +
                batchesReDeli.Count + batchesAR.Count +
                batchesARCredit.Count + batchesGI.Count + batchesGR.Count);
            summary.AddRange(batchesPO);
            summary.AddRange(batchesAP);
            summary.AddRange(batchesAPCredit);
            summary.AddRange(batchesDeli);
            summary.AddRange(batchesReDeli);
            summary.AddRange(batchesAR);
            summary.AddRange(batchesARCredit);
            summary.AddRange(batchesGI);
            summary.AddRange(batchesGR);
            return await Task.FromResult(summary);
        }
        List<BatchNoDocumentViewModel> GetSerialDoc<T, MT>(
           string idProp,
           string wdps,
           string seriesId,
           string seriesD,
           List<T> data,
           List<MT> masterData,
           ItemMasterData item,
           TransTypeWD transType)
        {
            List<BatchNoDocumentViewModel> serials = (from wd in masterData
                            .Where(i => (int)GetValue(i, "ItemID") == item.ID && (TransTypeWD)i.GetType().GetProperty("TransType").GetValue(i) == transType)
                                                           join wh in _context.Warehouses on GetValue(wd, "WarehouseID") equals wh.ID
                                                           join pur in data on GetValue(wd, wdps) equals GetValue(pur, idProp)
                                                           join series in _context.Series on GetValue(pur, seriesId) equals series.ID
                                                           join seriesd in _context.SeriesDetails on GetValue(pur, seriesD) equals seriesd.ID
                                                           group new { wd, pur, series, seriesd, wh } by new { series.Name, seriesd.Number } into g
                                                           let _data = g.FirstOrDefault()
                                                           select new BatchNoDocumentViewModel
                                                           {
                                                               DocNo = $"{_data.series.Name}-{_data.seriesd.Number}",
                                                               ItemCode = item.Code,
                                                               ItemName1 = item.KhmerName,
                                                               ItemName2 = item.EnglishName,
                                                               WhsCode = _data.wh.Code,
                                                               WhsName = _data.wh.Name,
                                                               ItemID = item.ID,
                                                               TransId = (int)GetValue(_data.pur, idProp),
                                                               TransType = transType
                                                           }).ToList();
            return serials;
        }
        public List<CreatedBatchNoViewModel> GetBatchCreated(int itemId, int saleId, TransTypeWD transType)
        {
            List<CreatedBatchNoViewModel> _serials = (from wd in _context.StockOuts
                                                        .Where(i =>
                                                            i.ItemID == itemId &&
                                                            i.TransType == transType &&
                                                            (int)GetValue(i, "TransID") == saleId &&
                                                            !string.IsNullOrEmpty(i.BatchNo)
                                                            )
                                                      join wh in _context.Warehouses on wd.WarehouseID equals wh.ID
                                                      select new CreatedBatchNoViewModel
                                                      {
                                                          AdmissionDate = wd.AdmissionDate,
                                                          Details = wd.Details,
                                                          ExpirationDate = wd.ExpireDate,
                                                          Location = wd.Location,
                                                          BatchAttribute1 = wd.BatchAttr1,
                                                          MfrDate = wd.MfrDate,
                                                          BatchAttribute2 = wd.BatchAttr2,
                                                          RefWarehouseID = wd.ID,
                                                          Batch = wd.BatchNo,
                                                          BatchOG = wd.BatchNo,
                                                          TransType = transType,
                                                          UnitCost = wd.Cost,
                                                          WarehouseID = wd.ID
                                                      }).ToList();
            return _serials;
        }
        public List<CreatedBatchNoViewModel> GetBatchCreated(string propId, int itemId, int saleId, TransTypeWD transType)
        {
            List<CreatedBatchNoViewModel> _serials = (from wd in _context.WarehouseDetails
                                                        .Where(i =>
                                                            i.ItemID == itemId &&
                                                            i.TransType == transType &&
                                                            (int)GetValue(i, propId) == saleId &&
                                                            !string.IsNullOrEmpty(i.BatchNo)
                                                            )
                                                      join wh in _context.Warehouses on wd.WarehouseID equals wh.ID
                                                      select new CreatedBatchNoViewModel
                                                      {
                                                          AdmissionDate = wd.AdmissionDate,
                                                          Details = wd.Details,
                                                          ExpirationDate = wd.ExpireDate,
                                                          Location = wd.Location,
                                                          BatchAttribute1 = wd.BatchAttr1,
                                                          MfrDate = wd.MfrDate,
                                                          BatchAttribute2 = wd.BatchAttr2,
                                                          RefWarehouseID = wd.ID,
                                                          Batch = wd.BatchNo,
                                                          BatchOG = wd.BatchNo,
                                                          TransType = transType,
                                                          UnitCost = (decimal)wd.Cost,
                                                          WarehouseID = wd.ID,
                                                      }).ToList();
            return _serials;
        }
        public void UpdateBatchNo(List<BatchNoDocumentViewModel> batches)
        {
            batches = batches.Where(i => i.Batches.Count > 0).ToList();
            foreach (var batch in batches)
            {
                foreach (CreatedBatchNoViewModel batchDetial in batch.Batches)
                {
                    var warehouseDetials = _context.WarehouseDetails.Where(i => i.BatchNo == batchDetial.BatchOG).ToList();
                    var stockOuts = _context.StockOuts.Where(i => i.BatchNo == batchDetial.BatchOG).ToList();
                    foreach (var _stockOut in stockOuts)
                    {
                        _stockOut.BatchNo = batchDetial.Batch;
                        _stockOut.BatchAttr1 = batchDetial.BatchAttribute1;
                        _stockOut.BatchAttr2 = batchDetial.BatchAttribute2;
                        _stockOut.MfrDate = batchDetial.MfrDate;
                        _stockOut.AdmissionDate = batchDetial.AdmissionDate;
                        _stockOut.ExpireDate = batchDetial.ExpirationDate;
                        _stockOut.Details = batchDetial.Details;
                        _stockOut.Location = batchDetial.Location;
                        _context.SaveChanges();
                    }
                    foreach (var _warehouseDetial in warehouseDetials)
                    {
                        _warehouseDetial.BatchNo = batchDetial.Batch;
                        _warehouseDetial.BatchAttr1 = batchDetial.BatchAttribute1;
                        _warehouseDetial.BatchAttr2 = batchDetial.BatchAttribute2;
                        _warehouseDetial.MfrDate = batchDetial.MfrDate;
                        _warehouseDetial.AdmissionDate = batchDetial.AdmissionDate;
                        _warehouseDetial.ExpireDate = (DateTime)batchDetial.ExpirationDate;
                        _warehouseDetial.Details = batchDetial.Details;
                        _warehouseDetial.Location = batchDetial.Location;
                        _context.SaveChanges();
                    }
                }
            }
        }
        public void UpdateBatchNoDetial(BatchNoDetialViewModel batchDetail)
        {
            var whds = _context.WarehouseDetails.Where(i => i.BatchNo == batchDetail.BatchOG).ToList();
            var stockOuts = _context.StockOuts.Where(i => i.BatchNo == batchDetail.BatchOG).ToList();
            foreach (var _stockOut in stockOuts)
            {
                _stockOut.BatchNo = batchDetail.Batch;
                _stockOut.BatchAttr1= batchDetail.BatchAttribute1;
                _stockOut.BatchAttr2= batchDetail.BatchAttribute2;
                _stockOut.MfrDate = batchDetail.ManufacturingDate;
                _stockOut.AdmissionDate = batchDetail.AdmissionDate;
                _stockOut.ExpireDate = batchDetail.ExpirationDate;
                _stockOut.Details = batchDetail.Details;
                _stockOut.Location = batchDetail.Location;
                _context.SaveChanges();
            }
            foreach (var whd in whds)
            {
                whd.BatchNo = batchDetail.Batch;
                whd.BatchAttr1 = batchDetail.BatchAttribute1;
                whd.BatchAttr2 = batchDetail.BatchAttribute2;
                whd.MfrDate = batchDetail.ManufacturingDate;
                whd.AdmissionDate = batchDetail.AdmissionDate;
                whd.ExpireDate = (DateTime)batchDetail.ExpirationDate;
                whd.Details = batchDetail.Details;
                whd.Location = batchDetail.Location;
                _context.SaveChanges();
            }
        }
        public List<BatchNoDetialViewModel> GetBatchNoDetails(string itemcode)
        {
            var data = (from item in _context.ItemMasterDatas
                        .Where(i => i.Code.ToLower() == itemcode.ToLower() && i.ManItemBy == ManageItemBy.Batches)
                        join whd in _context.WarehouseDetails.Where(i => !string.IsNullOrEmpty(i.BatchNo)) on item.ID equals whd.ItemID
                        join wh in _context.Warehouses on whd.WarehouseID equals wh.ID
                        group new { item, whd, wh } by new { whd.BatchNo } into g 
                        let _data = g.FirstOrDefault()
                        select new BatchNoDetialViewModel
                        {
                            Batch = _data.whd.BatchNo,
                            AdmissionDate = _data.whd.AdmissionDate,
                            Details = _data.whd.Details,
                            ExpirationDate = _data.whd.ExpireDate,
                            ItemCode = _data.item.Code,
                            ItemName = _data.item.KhmerName,
                            Location = _data.whd.Location,
                            BatchAttribute2 = _data.whd.BatchAttr2,
                            ManufacturingDate = _data.whd.MfrDate,
                            BatchAttribute1 = _data.whd.BatchAttr1,
                            BatchOG = _data.whd.BatchNo,
                            WhsName = _data.wh.Name,
                        }).ToList();
            return data;
        }
        private static object GetValue<T>(T obj, string prop)
        {
            var data = typeof(T).GetProperty(prop).GetValue(obj);
            return data ?? "";
        }
    }
}
