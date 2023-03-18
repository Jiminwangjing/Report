using CKBS.AppContext;
using CKBS.Models.Services.Account;
using CKBS.Models.Services.Administrator.Inventory;
using CKBS.Models.Services.Administrator.SystemInitialization;
using CKBS.Models.Services.Inventory.Transaction;
using CKBS.Models.Services.LoyaltyProgram.ComboSale;
using CKBS.Models.Services.POS;
using CKBS.Models.Services.POS.KVMS;
using CKBS.Models.Services.Purchase;
using KEDI.Core.Premise.Models.GeneralSettingAdmin;
using KEDI.Core.Premise.Models.Sale;
using KEDI.Core.Premise.Models.Services.Administrator.Inventory;
using KEDI.Core.Premise.Models.ServicesClass.SerialBatches.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Repository
{
    public interface ISerialBatchReportRepository
    {
        Task<List<ItemBatchView>> GetBatchAsyns(string dateFrom, string dateTo, int branchID, int wahouseID, int userId);
        Task<List<ItemBatchView>> GetSerialsAsync(string dateFrom, string dateTo, int branchID, int wahouseID, int userId, int status, int sysCurID);
    }

    public class SerialBatchReportRepository : ISerialBatchReportRepository
    {
        private readonly DataContext _context;
        private readonly UtilityModule _format;
        public SerialBatchReportRepository(DataContext context, UtilityModule format)
        {
            _context = context;
            _format = format;
        }
        public async Task<List<ItemBatchView>> GetSerialsAsync(string dateFrom, string dateTo, int branchID, int wahouseID, int userId, int status, int sysCurID)
        {
            List<WarehouseDetail> goodsFilter = new();
            List<StockOut> Serialstockout = new();
            DateTime _dateFrom = Convert.ToDateTime(dateFrom);
            DateTime _dateTo = Convert.ToDateTime(dateTo);
            var curName = _context.Currency.Find(sysCurID);
            var displayCurr = _context.DisplayCurrencies.FirstOrDefault(c => c.AltCurrencyID == curName.ID) ?? new DisplayCurrency();
            var disformat = _context.Displays.FirstOrDefault(c => c.DisplayCurrencyID == displayCurr.AltCurrencyID) ?? new Display();
            if (dateTo != null && dateTo != null && branchID != 0 && wahouseID != 0)
            {
                goodsFilter = _context.WarehouseDetails.Where(i => !string.IsNullOrEmpty(i.SerialNumber) && i.SyetemDate >= _dateFrom && i.SyetemDate <= _dateTo && i.WarehouseID == wahouseID).ToList();
                Serialstockout = _context.StockOuts.Where(i => !string.IsNullOrEmpty(i.SerialNumber) && i.SyetemDate >= _dateFrom && i.SyetemDate <= _dateTo && i.WarehouseID == wahouseID).ToList();
            }
            else if (dateTo != null && dateTo != null && branchID != 0 && wahouseID != 0)
            {
                goodsFilter = _context.WarehouseDetails.Where(i => !string.IsNullOrEmpty(i.SerialNumber) && i.SyetemDate >= _dateFrom && i.SyetemDate <= _dateTo && i.WarehouseID == wahouseID).ToList();
                Serialstockout = _context.StockOuts.Where(i => !string.IsNullOrEmpty(i.SerialNumber) && i.SyetemDate >= _dateFrom && i.SyetemDate <= _dateTo && i.WarehouseID == wahouseID).ToList();
            }
            else if (dateTo != null && dateTo != null && branchID != 0 && wahouseID == 0)
            {
                goodsFilter = _context.WarehouseDetails.Where(i => !string.IsNullOrEmpty(i.SerialNumber) && i.SyetemDate >= _dateFrom && i.SyetemDate <= _dateTo).ToList();
                Serialstockout = _context.StockOuts.Where(i => !string.IsNullOrEmpty(i.SerialNumber) && i.SyetemDate >= _dateFrom && i.SyetemDate <= _dateTo).ToList();
            }

            else if (dateTo != null && dateTo != null && branchID != 0 && wahouseID == 0)
            {
                goodsFilter = _context.WarehouseDetails.Where(i => !string.IsNullOrEmpty(i.SerialNumber) && i.SyetemDate >= _dateFrom && i.SyetemDate <= _dateTo).ToList();
                Serialstockout = _context.StockOuts.Where(i => !string.IsNullOrEmpty(i.SerialNumber) && i.SyetemDate >= _dateFrom && i.SyetemDate <= _dateTo).ToList();
            }
            //(b & u)=0
            else if (dateTo != null && dateTo != null && branchID == 0 && wahouseID != 0)
            {
                goodsFilter = _context.WarehouseDetails
                    .Where(i => i.WarehouseID == wahouseID && !string.IsNullOrEmpty(i.SerialNumber) && i.SyetemDate >= _dateFrom && i.SyetemDate <= _dateTo).ToList();
                Serialstockout = _context.StockOuts.Where(i => i.WarehouseID == wahouseID && !string.IsNullOrEmpty(i.SerialNumber) && i.SyetemDate >= _dateFrom && i.SyetemDate <= _dateTo).ToList();
            }
            //(w b u )=0
            else if (dateTo != null && dateTo != null && branchID == 0 && wahouseID == 0)
            {
                goodsFilter = _context.WarehouseDetails.Where(i => !string.IsNullOrEmpty(i.SerialNumber) && i.SyetemDate >= _dateFrom && i.SyetemDate <= _dateTo).ToList();
                Serialstockout = _context.StockOuts.Where(i => !string.IsNullOrEmpty(i.SerialNumber) && i.SyetemDate >= _dateFrom && i.SyetemDate <= _dateTo).ToList();
            }

            var Warehouse = goodsFilter;
            var filterSerial = (from wh in Warehouse
                                join it in _context.ItemMasterDatas on wh.ItemID equals it.ID
                                join bp in _context.BusinessPartners on wh.BPID equals bp.ID
                                join warehouse in _context.Warehouses on wh.WarehouseID equals warehouse.ID
                                join acc in _context.UserAccounts on wh.UserID equals acc.ID
                                join brand in _context.Branches on acc.BranchID equals brand.ID
                                select new ItemBatchView
                                {
                                    ID = it.ID,
                                    Code = it.Code,
                                    KhmerName = it.KhmerName,
                                    EnglishName = it.EnglishName,
                                    StockIn = wh.InStock.ToString(),
                                    StockCommit = it.StockCommit.ToString(),
                                    Cost = wh.Cost.ToString(),
                                    UniPrice = it.UnitPrice.ToString(),
                                    Process = it.Process,
                                    // BatchNo = wh.SerialNumber,
                                    Transaction = "Available",
                                    Systemdate = wh.SyetemDate.ToShortDateString(),
                                    TransType = wh.TransType,
                                    TransID = wh.InStockFrom,
                                    BranchID = acc.BranchID,
                                    TransactionOut = "Available",
                                    ///
                                    BatchNo = wh.BatchNo,
                                    MfrSerialNo = wh.MfrSerialNumber,
                                    SerialNumber = wh.SerialNumber,
                                    LotNumber = wh.LotNumber,
                                    PlateNumber = wh.PlateNumber,
                                    Status = wh.InStock <= 0 ? "Unavailable" : "Available",
                                    Admisiondate = ((DateTime)wh.AdmissionDate).ToString("dd-MM-yyyy"),
                                    Expiredate = wh.ExpireDate.ToString("dd-MM-yyyy"),
                                    MfrDate = wh.MfrDate == null ? "" : wh.MfrDate?.ToString("dd-MM-yyyy"),
                                    InDate = wh.SyetemDate.ToString("dd-MM-yyyy"),
                                    MfrWarrantyStart = wh.MfrWarDateStart?.ToString("dd-MM-yyyy"),
                                    MfrWarrantyEnd = wh.MfrWarDateEnd?.ToString("dd-MM-yyyy"),
                                    InWarehouse = warehouse.Name,
                                    SupplierCode = bp.Code,
                                    SupplierName = bp.Name,


                                }).ToList();

            var stockout = Serialstockout;
            var filterSerialStcok = (from wh in stockout
                                     join it in _context.ItemMasterDatas on wh.ItemID equals it.ID
                                     join bp in _context.BusinessPartners on wh.BPID equals bp.ID
                                     join warehouse in _context.Warehouses on wh.WarehouseID equals warehouse.ID
                                     join acc in _context.UserAccounts on wh.UserID equals acc.ID
                                     join brand in _context.Branches on acc.BranchID equals brand.ID
                                     select new ItemBatchView
                                     {
                                         ID = it.ID,
                                         Code = it.Code,
                                         KhmerName = it.KhmerName,
                                         EnglishName = it.EnglishName,
                                         StockOut = wh.InStock.ToString(),
                                         StockCommit = it.StockCommit.ToString(),
                                         Cost = wh.Cost.ToString(),
                                         UniPrice = it.UnitPrice.ToString(),
                                         Process = it.Process,
                                         SerNumberout = wh.SerialNumber,
                                         Systemdate = wh.SyetemDate.ToShortDateString(),
                                         //Expiredate = ((DateTime)wh.ExpireDate).ToShortDateString(),
                                         //Admisiondate = ((DateTime)wh.AdmissionDate).ToShortDateString(),
                                         TransType = wh.TransType,
                                         TransID = wh.TransID,
                                         Transaction = "Close",
                                         BranchID = acc.BranchID,
                                         TransactionOut = "Close",
                                         //  BatchNo         = wh.BatchNo,
                                         BatchNo = wh.BatchNo,
                                         MfrSerialNo = wh.MfrSerialNumber,
                                         SerialNumber = wh.SerialNumber,
                                         LotNumber = wh.LotNumber,
                                         PlateNumber = wh.PlateNumber,
                                         Status = wh.InStock <= 0 ? "Unavailable" : "Available",
                                         Admisiondate = ((DateTime)wh.AdmissionDate).ToString("dd-MM-yyyy"),
                                         Expiredate = ((DateTime)wh.ExpireDate).ToString("dd-MM-yyyy"),
                                         MfrDate = wh.SyetemDate.ToString("dd-MM-yyyy"),
                                         OutDate = wh.SyetemDate.ToString("dd-MM-yyyy"),
                                         MfrWarrantyStart = wh.MfrWarDateStart?.ToString("dd-MM-yyyy"),
                                         MfrWarrantyEnd = wh.MfrWarDateEnd?.ToString("dd-MM-yyyy"),
                                         OutWarehouse = warehouse.Name,
                                         CustomerCode = bp.Code,
                                         CustomerName = bp.Name,

                                     }).ToList();
            List<ItemBatchView> totalSerial = new();
            if (branchID > 0)
            {
                filterSerial = filterSerial.Where(i => i.BranchID == branchID).ToList();
                filterSerialStcok = filterSerialStcok.Where(i => i.BranchID == branchID).ToList();
            }
            foreach (var i in filterSerial)
            {
                CheckTransaction(i, i.TransType, i.TransID);
            }
            foreach (var i in filterSerialStcok)
            {
                CheckTransaction(i, i.TransType, i.TransID);
            }

            foreach (var i in filterSerial)
            {
                var serial = filterSerialStcok.FirstOrDefault(s => s.SerialNumber == i.SerialNumber) ?? new ItemBatchView();

                totalSerial.Add(new ItemBatchView
                {
                    Transaction = string.IsNullOrEmpty(serial.SerNumberout) ? i.Transaction : serial.Transaction,

                    ID = i.ID,
                    Code = i.Code,
                    KhmerName = i.KhmerName,
                    EnglishName = i.EnglishName,
                    Process = i.Process,
                    BatchNo = i.BatchNo,

                    MfrSerialNo = i.MfrSerialNo,
                    SerialNumber = i.SerialNumber,
                    PlateNumber = i.PlateNumber,
                    LotNumber = i.LotNumber,
                    Status = i.Status,
                    Systemdate = i.Systemdate,
                    Expiredate = i.Expiredate,
                    Admisiondate = i.Admisiondate,
                    MfrDate = i.MfrDate,
                    InType = i.InType,
                    InDoc = i.InDoc,
                    InDate = i.InDate,
                    InQuantity = i.InQuantity,
                    InWarehouse = i.InWarehouse,
                    SupplierCode = i.SupplierCode,
                    SupplierName = i.SupplierName,

                    OutType = serial.OutType,
                    OutDoc = serial.OutDoc,
                    OutDate = serial.OutDate,
                    OutQuantity = serial.OutQuantity,
                    OutWarehouse = serial.OutWarehouse,
                    CustomerCode = serial.CustomerCode,
                    CustomerName = serial.CustomerName,
                    MfrWarrantyStart = i.MfrWarrantyStart,
                    MfrWarrantyEnd = i.MfrWarrantyEnd,
                    StockOnHand = i.Status == "Unavailable" ? 0 : 1,
                    //Transaction = "Out",
                    InvoiceNo = i.InvoiceNo,
                    SerNumberout = serial.SerNumberout,
                    InvoceTrand = serial.InvoiceNo,
                    SystemdateOut = serial.Systemdate,
                    ExpiredateOut = serial.Expiredate,
                    AdmisiondateOut = serial.Admisiondate,
                    Cost = i.Cost,
                    ItemNameOut = serial.KhmerName,
                    TransactionIn = i.TransactionIn,
                    TransactionOut = i.TransactionOut,
                    ProseccIn = i.Process,
                    //

                });
            }

            double grandtototal = totalSerial.Sum(s => s.StockOnHand);
            var groupSerials = totalSerial.GroupBy(ts => ts.Code);
            var list = totalSerial.Select(ibv =>
            {
                var totalStockOnHand = groupSerials.Where(t => t.FirstOrDefault().Code == ibv.Code).Select(ts => ts.Sum(t => t.StockOnHand)).FirstOrDefault();
                ibv.CountStockIn = groupSerials.Count();
                ibv.TotalGroup = _format.ToCurrency((decimal)totalStockOnHand, disformat.Amounts);
                ibv.GrandTotal = _format.ToCurrency(grandtototal, disformat.Amounts);


                return ibv;
            }).ToList();

            //   double lists= totalSerial.GroupBy(s=>s.Code).Select(group => group.Sum(item => item.StockOnHand));

            list = (status == 0 ? list : status == 1 ? list.Where(s => s.StockOnHand == 1) : list.Where(s => s.StockOnHand == 0)).ToList();
            if (status == 2)
            {
                foreach (var item in list)
                {
                    item.TotalGroup = _format.ToCurrency(0.00, disformat.Amounts);
                    item.GrandTotal = _format.ToCurrency(0.00, disformat.Amounts);
                }
            }

            return await Task.FromResult(list);
        }

        public async Task<List<ItemBatchView>> GetBatchAsyns(string dateFrom, string dateTo, int branchID, int wahouseID, int userId)
        {
            List<WarehouseDetail> goodsFilter = new();
            List<StockOut> batchstockout = new();
            DateTime _dateFrom = Convert.ToDateTime(dateFrom);
            DateTime _dateTo = Convert.ToDateTime(dateTo);
            if (dateTo != null && dateTo != null && branchID != 0 && wahouseID != 0 && userId != 0)
            {
                batchstockout = _context.StockOuts.Where(i => !string.IsNullOrEmpty(i.BatchNo) && i.SyetemDate >= _dateFrom && i.SyetemDate <= _dateTo && i.WarehouseID == wahouseID && i.UserID == userId).ToList();
                goodsFilter = _context.WarehouseDetails.Where(i => !string.IsNullOrEmpty(i.BatchNo) && i.SyetemDate >= _dateFrom && i.SyetemDate <= _dateTo && i.WarehouseID == wahouseID && i.UserID == userId).ToList();
            }
            else if (dateTo != null && dateTo != null && branchID != 0 && wahouseID != 0 && userId == 0)
            {
                batchstockout = _context.StockOuts.Where(i => !string.IsNullOrEmpty(i.BatchNo) && i.SyetemDate >= _dateFrom && i.SyetemDate <= _dateTo && i.WarehouseID == wahouseID).ToList();
                goodsFilter = _context.WarehouseDetails.Where(i => !string.IsNullOrEmpty(i.BatchNo) && i.SyetemDate >= _dateFrom && i.SyetemDate <= _dateTo && i.WarehouseID == wahouseID).ToList();
            }
            else if (dateTo != null && dateTo != null && branchID != 0 && wahouseID == 0 && userId != 0)
            {
                batchstockout = _context.StockOuts.Where(i => !string.IsNullOrEmpty(i.BatchNo) && i.SyetemDate >= _dateFrom && i.SyetemDate <= _dateTo && i.UserID == userId).ToList();
                goodsFilter = _context.WarehouseDetails.Where(i => !string.IsNullOrEmpty(i.BatchNo) && i.SyetemDate >= _dateFrom && i.SyetemDate <= _dateTo && i.UserID == userId).ToList();
            }
            else if (dateTo != null && dateTo != null && branchID != 0 && wahouseID == 0 && userId == 0)
            {
                batchstockout = _context.StockOuts.Where(i => !string.IsNullOrEmpty(i.BatchNo) && i.SyetemDate >= _dateFrom && i.SyetemDate <= _dateTo).ToList();
                goodsFilter = _context.WarehouseDetails.Where(i => !string.IsNullOrEmpty(i.BatchNo) && i.SyetemDate >= _dateFrom && i.SyetemDate <= _dateTo).ToList();
            }
            //(b w u) =0
            else if (dateTo != null && dateTo != null && branchID == 0 && wahouseID == 0 && userId == 0)
            {
                batchstockout = _context.StockOuts.Where(i => !string.IsNullOrEmpty(i.BatchNo) && i.SyetemDate >= _dateFrom && i.SyetemDate <= _dateTo).ToList();
                goodsFilter = _context.WarehouseDetails.Where(i => !string.IsNullOrEmpty(i.BatchNo) && i.SyetemDate >= _dateFrom && i.SyetemDate <= _dateTo).ToList();
            }
            //(b u)=0
            else if (dateTo != null && dateTo != null && branchID == 0 && wahouseID != 0 && userId == 0)
            {
                goodsFilter = _context.WarehouseDetails
                    .Where(i => i.WarehouseID == wahouseID && !string.IsNullOrEmpty(i.BatchNo) && i.SyetemDate >= _dateFrom && i.SyetemDate <= _dateTo).ToList();
                batchstockout = _context.StockOuts.Where(i => i.WarehouseID == wahouseID && !string.IsNullOrEmpty(i.BatchNo) && i.SyetemDate >= _dateFrom && i.SyetemDate <= _dateTo).ToList();
            }



            var Batchwaehouse = goodsFilter;
            var FillerBatchlStcok = batchstockout;
            var FillerBatch = (from wh in Batchwaehouse
                               join it in _context.ItemMasterDatas on wh.ItemID equals it.ID
                               let acc = _context.UserAccounts.FirstOrDefault(i => i.ID == userId) ?? new UserAccount()
                               select new ItemBatchView
                               {

                                   ID = it.ID,
                                   Code = it.Code,
                                   KhmerName = it.KhmerName,
                                   EnglishName = it.EnglishName,
                                   //StockIn = wh.InStock.ToString(),
                                   TotalStockIn = Batchwaehouse.Where(i => i.BatchNo == wh.BatchNo).Sum(i => i.InStock).ToString(),
                                   //TotalStockIn = batchstockout.Sum(x=>x.InStock).ToString(),
                                   StockCommit = it.StockCommit.ToString(),
                                   Cost = wh.Cost.ToString(),
                                   UniPrice = it.UnitPrice.ToString(),
                                   Process = it.Process,
                                   BatchNo = wh.BatchNo,
                                   Systemdate = wh.SyetemDate.ToShortDateString(),
                                   Expiredate = wh.ExpireDate.ToShortDateString(),
                                   BatchAttr1 = wh.BatchAttr1,
                                   BatchAttr2 = wh.BatchAttr2,
                                   Admisiondate = ((DateTime)wh.AdmissionDate).ToShortDateString(),
                                   Transaction = "In",
                                   TransType = wh.TransType,
                                   TransID = wh.InStockFrom,
                                   BranchID = acc.BranchID,
                                   Time = wh.TimeIn.ToString(),
                               }).ToList();
            var stockout = FillerBatchlStcok;
            var FillerStcokOut = (from wh in stockout
                                  join it in _context.ItemMasterDatas on wh.ItemID equals it.ID
                                  let acc = _context.UserAccounts.FirstOrDefault(i => i.ID == userId) ?? new UserAccount()
                                  select new ItemBatchView
                                  {
                                      ID = it.ID,
                                      Code = it.Code,
                                      KhmerName = it.KhmerName,
                                      EnglishName = it.EnglishName,
                                      StockOut = wh.InStock.ToString(),
                                      //TotalStockOut = stockout.Where(i => i.BatchNo == wh.BatchNo).Sum(i => i.InStock).ToString(),
                                      StockCommit = it.StockCommit.ToString(),
                                      Cost = wh.Cost.ToString(),
                                      UniPrice = it.UnitPrice.ToString(),
                                      Process = it.Process,
                                      BatchNo = wh.BatchNo,
                                      Systemdate = wh.SyetemDate.ToShortDateString(),
                                      Expiredate = ((DateTime)wh.ExpireDate).ToShortDateString(),
                                      BatchAttr1 = wh.BatchAttr1,
                                      BatchAttr2 = wh.BatchAttr2,
                                      Admisiondate = ((DateTime)wh.AdmissionDate).ToShortDateString(),
                                      TransType = wh.TransType,
                                      TransID = wh.TransID,
                                      Transaction = "Out",
                                      BranchID = acc.BranchID,
                                      Time = wh.TimeIn.ToString(),
                                  }).ToList();


            var totalBatch = new List<ItemBatchView>
                (FillerBatch.Count + FillerStcokOut.Count);
            totalBatch.AddRange(FillerBatch);
            totalBatch.AddRange(FillerStcokOut);

            foreach (var i in totalBatch)
            {
                CheckTransaction(i, i.TransType, i.TransID);
            }
            return await Task.FromResult(totalBatch);
        }
        private void SetInvoiceNumber<T>(T master, ItemBatchView itemBatchView, int docTypeId, string invoiceProp, double qty = 0)
        {
            var doctype = _context.DocumentTypes.FirstOrDefault(i => i.ID == docTypeId) ?? new DocumentType();
            itemBatchView.InvoiceNo = $"{doctype.Code}-{GetValue(master, invoiceProp)}";
            itemBatchView.StockIn = qty.ToString();

        }
        private void CheckTransaction(ItemBatchView itemBatchView, TransTypeWD transType, int transId)
        {
            switch (transType)
            {
                case TransTypeWD.AR:
                    var saleAr = _context.SaleARs.FirstOrDefault(i => i.SARID == transId);
                    var sa = _context.SaleARDetails.FirstOrDefault(i => i.SARDID == saleAr.SARID && i.ItemID == itemBatchView.ID) ?? new SaleARDetail();
                    if (saleAr != null) SetInvoiceNumber(saleAr, itemBatchView, saleAr.DocTypeID, "InvoiceNumber", sa.Qty);
                    break;
                case TransTypeWD.PurAP:
                    var purchaseAP = _context.Purchase_APs.FirstOrDefault(i => i.PurchaseAPID == transId);
                    var apd = _context.PurchaseAPDetail.FirstOrDefault(i => i.PurchaseAPID == purchaseAP.PurchaseAPID && i.ItemID == itemBatchView.ID) ?? new Purchase_APDetail();
                    if (purchaseAP != null) SetInvoiceNumber(purchaseAP, itemBatchView, purchaseAP.DocumentTypeID, "Number", apd.Qty);
                    break;
                case TransTypeWD.POS:
                    var receipt = _context.Receipt.FirstOrDefault(i => i.ReceiptID == transId);
                    ////var rp = _context.ReceiptDetail.FirstOrDefault(i => i.ReceiptID == receipt.ReceiptID && i.ItemID == itemBatchView.ID) ?? new ReceiptDetail();
                    var docType = _context.DocumentTypes.FirstOrDefault(i => i.Code == "SP") ?? new DocumentType();
                    if (receipt != null) SetInvoiceNumber(receipt, itemBatchView, docType.ID, "ReceiptNo");
                    break;
                case TransTypeWD.Delivery:
                    var Delivery = _context.SaleDeliveries.FirstOrDefault(i => i.SDID == transId);
                    //var dv = _context.SaleDeliveryDetails.FirstOrDefault(i => i.SDDID == Delivery.SDID && i.ItemID == itemBatchView.ID) ?? new SaleDeliveryDetail();
                    if (Delivery != null) SetInvoiceNumber(Delivery, itemBatchView, Delivery.DocTypeID, "InvoiceNumber");
                    break;
                case TransTypeWD.CreditMemo:
                    var cm = _context.SaleCreditMemos.FirstOrDefault(i => i.SCMOID == transId);
                    var c = _context.SaleComboDetails.FirstOrDefault(i => i.ID == cm.SCMOID && i.ItemID == itemBatchView.ID) ?? new SaleComboDetail();
                    if (cm != null) SetInvoiceNumber(cm, itemBatchView, cm.DocTypeID, "InvoiceNumber", c.Qty);
                    break;
                case TransTypeWD.ReturnDelivery:
                    var RetunDelivery = _context.ReturnDeliverys.FirstOrDefault(i => i.ID == transId);
                    var rd = _context.ReturnDeliveryDetails.FirstOrDefault(i => i.ID == RetunDelivery.ID && i.ItemID == itemBatchView.ID) ?? new ReturnDeliveryDetail();
                    if (RetunDelivery != null) SetInvoiceNumber(RetunDelivery, itemBatchView, RetunDelivery.DocTypeID, "InvoiceNumber", rd.Qty);
                    break;
                case TransTypeWD.ARDownPayment:
                    var ARDP = _context.ARDownPayments.FirstOrDefault(i => i.ARDID == transId);
                    //var ard = _context.ARDownPaymentDetails.FirstOrDefault(i => i.ID == ARDP.ARDID && i.ItemID == itemBatchView.ID) ?? new ARDownPaymentDetail();
                    if (ARDP != null) SetInvoiceNumber(ARDP, itemBatchView, ARDP.DocTypeID, "InvoiceNumber");
                    break;
                case TransTypeWD.PurOrder:
                    var PurchaseOrder = _context.PurchaseOrders.FirstOrDefault(i => i.PurchaseOrderID == transId);

                    if (PurchaseOrder != null) SetInvoiceNumber(PurchaseOrder, itemBatchView, PurchaseOrder.DocumentTypeID, "Number");
                    break;
                case TransTypeWD.GRPO:
                    var GRPOs = _context.GoodsReciptPOs.FirstOrDefault(i => i.ID == transId);
                    var drp = _context.GoodReciptPODetails.FirstOrDefault(i => i.GoodsReciptPOID == GRPOs.ID && i.ItemID == itemBatchView.ID) ?? new GoodReciptPODetail();
                    if (GRPOs != null) SetInvoiceNumber(GRPOs, itemBatchView, GRPOs.DocumentTypeID, "Number", drp.Qty);
                    break;
                case TransTypeWD.PurCreditMemo:
                    var PCM = _context.PurchaseCreditMemos.FirstOrDefault(i => i.PurchaseMemoID == transId);
                    //var pm = _context.PurchaseCreditMemoDetails.FirstOrDefault(i => i.PurchaseMemoDetailID == PCM.PurchaseMemoID && i.ItemID == itemBatchView.ID)?? new PurchaseCreditMemoDetail();
                    if (PCM != null) SetInvoiceNumber(PCM, itemBatchView, PCM.DocumentTypeID, "Number");
                    break;
                case TransTypeWD.GoodsReceipt:
                    var GR = _context.GoodsReceipts.FirstOrDefault(i => i.GoodsReceiptID == transId);
                    var gr = _context.GoodReceiptDetails.FirstOrDefault(i => i.GoodReceitpDetailID == GR.GoodsReceiptID && i.ItemID == itemBatchView.ID) ?? new GoodReceiptDetail();
                    if (GR != null) SetInvoiceNumber(GR, itemBatchView, GR.DocTypeID, "Number_No", gr.Quantity);
                    break;
                case TransTypeWD.GoodsIssue:
                    var GI = _context.GoodIssues.FirstOrDefault(i => i.GoodIssuesID == transId);
                    //var gi = _context.GoodIssuesDetails.FirstOrDefault(i => i.GoodIssuesDetailID == GI.GoodIssuesID && i.ItemID == itemBatchView.ID) ?? new GoodIssuesDetail();
                    if (GI != null) SetInvoiceNumber(GI, itemBatchView, GI.DocTypeID, "Number_No");
                    break;
                case TransTypeWD.ReturnPOS:
                    var PS = _context.ReceiptMemo.FirstOrDefault(i => i.ID == transId);
                    var ps = _context.ReceiptDetailMemoKvms.FirstOrDefault(i => i.ID == PS.ID && i.ItemID == itemBatchView.ID) ?? new ReceiptDetailMemo();
                    if (PS != null) SetInvoiceNumber(PS, itemBatchView, PS.DocTypeID, "ReceiptMemoNo", ps.Qty);
                    break;
                case TransTypeWD.POSService:
                    var KS = _context.KSServiceMaster.FirstOrDefault(i => i.ID == transId);
                    if (KS != null) SetInvoiceNumber(KS, itemBatchView, KS.DocTypeID, "Number");
                    break;
                case TransTypeWD.PointRedempt:
                    var PRM = _context.PointRedemptionMasters.FirstOrDefault(i => i.ID == transId);
                    if (PRM != null) SetInvoiceNumber(PRM, itemBatchView, PRM.DocTypeID, "Number");
                    break;
                default:
                    break;
            }
        }
        private static object GetValue<T>(T obj, string prop)
        {
            var data = typeof(T).GetProperty(prop).GetValue(obj);
            return data ?? "";
        }
    }
}
