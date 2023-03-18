using CKBS.AppContext;
using CKBS.Models.Services.Administrator.Inventory;
using CKBS.Models.Services.Inventory;
using KEDI.Core.Premise.Models.ServicesClass.SerialBatches.SerialNumbersManagement;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.Services.Responsitory
{
    public interface IItemSerialNumberRepository
    {
        Task<List<SerialNumberDocumentViewModel>> GetSerialNumberDocAsync(SNFilter filter, ItemMasterData item);
        List<CreatedSerialNumbersViewModel> GetCreatedSerials(string propId, int itemId, int saleId, TransTypeWD transType);
        List<CreatedSerialNumbersViewModel> GetCreatedSerials(int itemId, int saleId, TransTypeWD transType);
        void UpdateSerialNumber(List<SerialNumberDocumentViewModel> serials);
        void UpdateSerialNumberDetial(ItemSerialNumberDetialView serialDetail);
        List<ItemSerialNumberDetialView> GetSerialItemDetails(string itemcode);
        Task<List<ItemMasterData>> GetItemMasterDataAsync();
    }
    public class ItemSerialNumberRepository : IItemSerialNumberRepository
    {
        private readonly DataContext _context;

        public ItemSerialNumberRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<List<SerialNumberDocumentViewModel>> GetSerialNumberDocAsync(SNFilter filter, ItemMasterData item)
        {
            List<SerialNumberDocumentViewModel> serialsPO = new();
            List<SerialNumberDocumentViewModel> serialsAP = new();
            List<SerialNumberDocumentViewModel> serialsAPCredit = new();
            List<SerialNumberDocumentViewModel> serialsDeli = new();
            List<SerialNumberDocumentViewModel> serialsReDeli = new();
            List<SerialNumberDocumentViewModel> serialsAR = new();
            List<SerialNumberDocumentViewModel> serialsARCredit = new();
            List<SerialNumberDocumentViewModel> serialsGI = new();
            List<SerialNumberDocumentViewModel> serialsGR = new();
            var whStockIn = _context.WarehouseDetails.Where(i=> !string.IsNullOrEmpty(i.SerialNumber)).ToList();
            var whStockOut = _context.StockOuts.Where(i => !string.IsNullOrEmpty(i.SerialNumber)).ToList();
            if (filter.GoodsReceiptPO)
            {
                var data = _context.GoodsReciptPOs.ToList();
                serialsPO = GetSerialDoc("ID", "InStockFrom", "SeriesID", "SeriesDetailID", data, whStockIn, item, TransTypeWD.GRPO);
            }
            if (filter.APInvoices)
            {
                var data = _context.Purchase_APs.ToList();
                serialsAP = GetSerialDoc("PurchaseAPID", "InStockFrom", "SeriesID", "SeriesDetailID", data, whStockIn, item, TransTypeWD.PurAP);
            }
            if (filter.APCreditMemos)
            {
                var data = _context.PurchaseCreditMemos.ToList();
                serialsAPCredit = GetSerialDoc("PurchaseMemoID", "TransID", "SeriesID", "SeriesDetailID", data, whStockOut, item, TransTypeWD.PurCreditMemo);
            }
            if (filter.Deliveries)
            {
                var data = _context.SaleDeliveries.ToList();
                serialsDeli = GetSerialDoc("SDID", "TransID", "SeriesID", "SeriesDID", data, whStockOut, item, TransTypeWD.Delivery);
            }
            if (filter.ReturnDeliveries)
            {
                var data = _context.ReturnDeliverys.ToList();
                serialsReDeli = GetSerialDoc("ID", "InStockFrom", "SeriesID", "SeriesDID", data, whStockIn, item, TransTypeWD.ReturnDelivery);
            }
            if (filter.ARInvoices)
            {
                var data = _context.SaleARs.ToList();
                serialsAR = GetSerialDoc("SARID", "TransID", "SeriesID", "SeriesDID", data, whStockOut, item, TransTypeWD.AR);
            }
            if (filter.ARCreditMemos)
            {
                var data = _context.SaleCreditMemos.ToList();
                serialsARCredit = GetSerialDoc("SCMOID", "InStockFrom", "SeriesID", "SeriesDID", data, whStockIn, item, TransTypeWD.CreditMemo);
            }
            if (filter.GoodsIssue)
            {
                var data = _context.GoodIssues.ToList();
                serialsGI = GetSerialDoc("GoodIssuesID", "TransID", "SeriseID", "SeriseDID", data, whStockOut, item, TransTypeWD.GoodsIssue);
            }
            if (filter.GoodsReciept)
            {
                var data = _context.GoodsReceipts.ToList();
                serialsGR = GetSerialDoc("GoodsReceiptID", "InStockFrom", "SeriseID", "SeriseDID", data, whStockIn, item, TransTypeWD.GoodsReceipt);
            }
            List<SerialNumberDocumentViewModel> summary = new
                (
                serialsPO.Count + serialsAP.Count +
                serialsAPCredit.Count + serialsDeli.Count +
                serialsReDeli.Count + serialsAR.Count +
                serialsARCredit.Count + serialsGI.Count + serialsGR.Count);
            summary.AddRange(serialsPO);
            summary.AddRange(serialsAP);
            summary.AddRange(serialsAPCredit);
            summary.AddRange(serialsDeli);
            summary.AddRange(serialsReDeli);
            summary.AddRange(serialsAR);
            summary.AddRange(serialsARCredit);
            summary.AddRange(serialsGI);
            summary.AddRange(serialsGR);
            return await Task.FromResult(summary);
        }
        private List<SerialNumberDocumentViewModel> GetSerialDoc<T, MT>(
            string idProp,
            string wdps,
            string seriesId,
            string seriesD,
            List<T> data,
            List<MT> masterData,
            ItemMasterData item,
            TransTypeWD transType)
        {
            List<SerialNumberDocumentViewModel> serials = (from wd in masterData
                            .Where(i => (int)GetValue(i, "ItemID") == item.ID && (TransTypeWD)i.GetType().GetProperty("TransType").GetValue(i) == transType)
                                                           join wh in _context.Warehouses on GetValue(wd, "WarehouseID") equals wh.ID
                                                           join pur in data on GetValue(wd, wdps) equals GetValue(pur, idProp)
                                                           join series in _context.Series on GetValue(pur, seriesId) equals series.ID
                                                           join seriesd in _context.SeriesDetails on GetValue(pur, seriesD) equals seriesd.ID
                                                           group new { wd, pur, series, seriesd, wh } by new { series.Name, seriesd.Number } into g
                                                           let _data = g.FirstOrDefault()
                                                           select new SerialNumberDocumentViewModel
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

        public List<CreatedSerialNumbersViewModel> GetCreatedSerials(string propId, int itemId, int saleId, TransTypeWD transType)
        {
            List<CreatedSerialNumbersViewModel> _serials = (from wd in _context.WarehouseDetails
                                                            .Where(i => 
                                                                i.ItemID == itemId && 
                                                                i.TransType == transType && 
                                                                (int)GetValue(i, propId) == saleId && 
                                                                !string.IsNullOrEmpty(i.SerialNumber))
                                                            join wh in _context.Warehouses on wd.WarehouseID equals wh.ID
                                                            select new CreatedSerialNumbersViewModel
                                                            {
                                                                AdmissionDate = wd.AdmissionDate,
                                                                Details = wd.Details,
                                                                ExpirationDate = wd.ExpireDate,
                                                                Location = wd.Location,
                                                                LotNumber = wd.LotNumber,
                                                                MfrDate = wd.MfrDate,
                                                                MfrSerialNo = wd.MfrSerialNumber,
                                                                MfrWarrantyEnd = wd.MfrWarDateEnd,
                                                                MfrWarrantyStart = wd.MfrWarDateStart,
                                                                RefWarehouseID = wd.ID,
                                                                SerialNumber = wd.SerialNumber,
                                                                SerialNumberOG = wd.SerialNumber,
                                                                TransType = transType,
                                                                UnitCost = (decimal)wd.Cost,
                                                                WarehouseID = wd.ID
                                                            }).ToList();
            return _serials;
        }
        public List<CreatedSerialNumbersViewModel> GetCreatedSerials(int itemId, int saleId, TransTypeWD transType)
        {
            List<CreatedSerialNumbersViewModel> _serials = (from wd in _context.StockOuts
                                                            .Where(i => 
                                                                i.ItemID == itemId && 
                                                                i.TransType == transType && 
                                                                (int)GetValue(i, "TransID") == saleId && 
                                                                !string.IsNullOrEmpty(i.SerialNumber))
                                                            join wh in _context.Warehouses on wd.WarehouseID equals wh.ID
                                                            select new CreatedSerialNumbersViewModel
                                                            {
                                                                AdmissionDate = wd.AdmissionDate,
                                                                Details = wd.Details,
                                                                ExpirationDate = wd.ExpireDate,
                                                                Location = wd.Location,
                                                                LotNumber = wd.LotNumber,
                                                                MfrDate = wd.MfrDate,
                                                                MfrSerialNo = wd.MfrSerialNumber,
                                                                MfrWarrantyEnd = wd.MfrWarDateEnd,
                                                                MfrWarrantyStart = wd.MfrWarDateStart,
                                                                RefWarehouseID = wd.ID,
                                                                SerialNumber = wd.SerialNumber,
                                                                SerialNumberOG = wd.SerialNumber,
                                                                TransType = transType,
                                                                UnitCost = wd.Cost,
                                                                WarehouseID = wd.ID
                                                            }).ToList();
            return _serials;
        }
        private static object GetValue<T>(T obj, string prop)
        {
            var data = typeof(T).GetProperty(prop).GetValue(obj);
            return data ?? "";
        }
        public void UpdateSerialNumber(List<SerialNumberDocumentViewModel> serials)
        {
            serials = serials.Where(i => i.Serials.Count > 0).ToList();
            foreach (var serial in serials)
            {
                foreach (CreatedSerialNumbersViewModel serialDetial in serial.Serials)
                {
                    var warehouseDetials = _context.WarehouseDetails.Where(i => i.SerialNumber == serialDetial.SerialNumberOG).ToList();
                    var stockOuts = _context.StockOuts.Where(i => i.SerialNumber == serialDetial.SerialNumberOG).ToList();
                    foreach (var _stockOut in stockOuts)
                    {
                        _stockOut.SerialNumber = serialDetial.SerialNumber;
                        _stockOut.LotNumber = serialDetial.LotNumber;
                        _stockOut.MfrSerialNumber = serialDetial.MfrSerialNo;
                        _stockOut.MfrDate = serialDetial.MfrDate;
                        _stockOut.MfrWarDateEnd = serialDetial.MfrWarrantyEnd;
                        _stockOut.MfrWarDateStart = serialDetial.MfrWarrantyStart;
                        _stockOut.AdmissionDate = serialDetial.AdmissionDate;
                        _stockOut.ExpireDate = serialDetial.ExpirationDate;
                        _stockOut.Details = serialDetial.Details;
                        _stockOut.Location = serialDetial.Location;
                        _context.SaveChanges();
                    }
                    foreach (var _warehouseDetial in warehouseDetials)
                    {
                        _warehouseDetial.SerialNumber = serialDetial.SerialNumber;
                        _warehouseDetial.LotNumber = serialDetial.LotNumber;
                        _warehouseDetial.MfrSerialNumber = serialDetial.MfrSerialNo;
                        _warehouseDetial.MfrDate = serialDetial.MfrDate;
                        _warehouseDetial.MfrWarDateEnd = serialDetial.MfrWarrantyEnd;
                        _warehouseDetial.MfrWarDateStart = serialDetial.MfrWarrantyStart;
                        _warehouseDetial.AdmissionDate = serialDetial.AdmissionDate;
                        _warehouseDetial.ExpireDate = (DateTime)serialDetial.ExpirationDate;
                        _warehouseDetial.Details = serialDetial.Details;
                        _warehouseDetial.Location = serialDetial.Location;
                        _context.SaveChanges();
                    }
                }
            }
        }

        public List<ItemSerialNumberDetialView> GetSerialItemDetails(string itemcode)
        {
            var data = (from item in _context.ItemMasterDatas
                        .Where(i => i.Code.ToLower() == itemcode.ToLower() && i.ManItemBy == ManageItemBy.SerialNumbers)
                        join whd in _context.WarehouseDetails.Where(i=> !string.IsNullOrEmpty(i.SerialNumber)) on item.ID equals whd.ItemID
                        join wh in _context.Warehouses on whd.WarehouseID equals wh.ID
                        group new { item, whd, wh } by new { whd.SerialNumber } into g
                        let _data = g.FirstOrDefault()
                        select new ItemSerialNumberDetialView
                        {
                            SerialNumber = _data.whd.SerialNumber,
                            AdmissionDate = _data.whd.AdmissionDate,
                            Details = _data.whd.Details,
                            ExpirationDate = _data.whd.ExpireDate,
                            ItemCode = _data.item.Code,
                            ItemName = _data.item.KhmerName,
                            Location = _data.whd.Location,
                            LotNumber = _data.whd.LotNumber,
                            ManufacturingDate = _data.whd.MfrDate,
                            MfrSerialNo = _data.whd.MfrSerialNumber,
                            MfrWarrantyEnd = _data.whd.MfrWarDateEnd,
                            MfrWarrantyStart = _data.whd.MfrWarDateStart,
                            SerialNumberOG = _data.whd.SerialNumber,
                            SystemNo = _data.whd.SysNum,
                            WhsName = _data.wh.Name,
                        }).ToList();
            return data;
        }

        public void UpdateSerialNumberDetial(ItemSerialNumberDetialView serialDetail)
        {
            var whds = _context.WarehouseDetails.Where(i => i.SerialNumber == serialDetail.SerialNumberOG).ToList();
            var stockOuts = _context.StockOuts.Where(i => i.SerialNumber == serialDetail.SerialNumberOG).ToList();
            foreach (var _stockOut in stockOuts)
            {
                _stockOut.SerialNumber = serialDetail.SerialNumber;
                _stockOut.LotNumber = serialDetail.LotNumber;
                _stockOut.MfrSerialNumber = serialDetail.MfrSerialNo;
                _stockOut.MfrDate = serialDetail.ManufacturingDate;
                _stockOut.MfrWarDateEnd = serialDetail.MfrWarrantyEnd;
                _stockOut.MfrWarDateStart = serialDetail.MfrWarrantyStart;
                _stockOut.AdmissionDate = serialDetail.AdmissionDate;
                _stockOut.ExpireDate = serialDetail.ExpirationDate;
                _stockOut.Details = serialDetail.Details;
                _stockOut.Location = serialDetail.Location;
                _context.SaveChanges();
            }
            foreach (var whd in whds)
            {
                whd.SerialNumber = serialDetail.SerialNumber;
                whd.LotNumber = serialDetail.LotNumber;
                whd.MfrSerialNumber = serialDetail.MfrSerialNo;
                whd.MfrDate = serialDetail.ManufacturingDate;
                whd.MfrWarDateEnd = serialDetail.MfrWarrantyEnd;
                whd.MfrWarDateStart = serialDetail.MfrWarrantyStart;
                whd.AdmissionDate = serialDetail.AdmissionDate;
                whd.ExpireDate = (DateTime)serialDetail.ExpirationDate;
                whd.Details = serialDetail.Details;
                whd.Location = serialDetail.Location;
                _context.SaveChanges();
            }
        }

        public async Task<List<ItemMasterData>> GetItemMasterDataAsync()
        {
            var data = await _context.ItemMasterDatas.Where(i =>
                i.Delete == false && i.ManItemBy == ManageItemBy.SerialNumbers
            ).Select(i=> new ItemMasterData { 
                Code = i.Code,
                ID = i.ID,
                KhmerName = i.KhmerName,
                Barcode = i.Barcode ?? "",
            }).ToListAsync();
            return data;
        }
    }
}
