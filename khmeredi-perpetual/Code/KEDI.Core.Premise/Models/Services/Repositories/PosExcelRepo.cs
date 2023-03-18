using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CKBS.AppContext;
using CKBS.Models.Services.Administrator.Setup;
using CKBS.Models.Services.Administrator.SystemInitialization;
using CKBS.Models.Services.POS;
using CKBS.Models.Services.Responsitory;
using KEDI.Core.Premise.Models.Services.Banking;
using KEDI.Core.Premise.Models.Services.POS;
using KEDI.Core.Premise.Models.ServicesClass.SerialBatches.Sales;
using KEDI.Core.Premise.Utilities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.UserModel;

namespace KEDI.Core.Premise
{
    public interface IPosExcelRepo
    {
        string GetFilePath();
        Task<List<ReceiptExport>> ExportReceiptsAsync();
        Task<List<ReceiptDetailsExport>> ExportReceiptsDetailAsync();
        Task<List<ReceiptFreightExport>> ExportFreightAsync();
        Task<List<PaymentMeanExport>> ExportPaymentMeanAsynce();
        Task ImportReceiptsAsync(IWorkbook workbook, ModelStateDictionary modelState);
    }

    public class PosExcelRepo : IPosExcelRepo
    {
        private readonly IWebHostEnvironment _hostEnv;
        private readonly DataContext _context;
        private readonly WorkbookAdapter _wbContext;
        private readonly IPOS _issueStock;
        public PosExcelRepo(DataContext context, IWebHostEnvironment hostEnv, IPOS issueStock)
        {
            _issueStock = issueStock;
            _context = context;
            _hostEnv = hostEnv;
            _wbContext = new WorkbookAdapter();
        }

        public string GetFilePath()
        {
            var path = Path.Combine(_hostEnv.WebRootPath, "ExcelFile/Receipt");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }
        public async Task<List<ReceiptFreightExport>> ExportFreightAsync()
        {
            var receiptFreights = await (from f in _context.FreightReceipts
                                         join r in _context.Receipt.Where(d => !d.Delete) on f.ReceiptID equals r.ReceiptID
                                         join sd in _context.SeriesDetails on r.SeriesDID equals sd.ID
                                         join s in _context.Series on r.SeriesID equals s.ID
                                         join fn in _context.Freights on f.FreightID equals fn.ID
                                         select new ReceiptFreightExport
                                         {
                                             ID = f.ID,
                                             FreightReceiptType = f.FreightReceiptType,
                                             FreightName = fn.Name,
                                             SeriesCode = s.Name,
                                             SeriesNumber = sd.Number,
                                             AmountReven = f.AmountReven,
                                             IsActive = f.IsActive
                                         }).ToListAsync();
            return receiptFreights;
        }

        public async Task<List<PaymentMeanExport>> ExportPaymentMeanAsynce()
        {
            var paymeanMeans = await (from p in _context.MultiPaymentMeans
                                      join r in _context.Receipt.Where(d => !d.Delete) on p.ReceiptID equals r.ReceiptID
                                      join s in _context.Series on r.SeriesID equals s.ID
                                      join sd in _context.SeriesDetails on r.SeriesDID equals sd.ID
                                      join pm in _context.PaymentMeans.Where(x => !x.Delete) on p.PaymentMeanID equals pm.ID
                                      join cr in _context.Currency.Where(x => !x.Delete) on p.AltCurrencyID equals cr.ID
                                      join c in _context.Currency.Where(x => !x.Delete) on p.PLCurrencyID equals c.ID
                                      select new PaymentMeanExport
                                      {
                                          SerialCode = s.Name,
                                          SeriesNumber = sd.Number,
                                          PaymentMeanType = pm.Type,
                                          AltCurrencyName = cr.Symbol,
                                          PLCurrencyName = c.Symbol,
                                          AltCurrency = p.AltCurrency,
                                          AltRate = p.AltRate,
                                          PLCurrency = p.PLCurrency,
                                          PLRate = p.PLRate,
                                          Amount = p.Amount,
                                          OpenAmount = p.OpenAmount,
                                          Total = p.Total,
                                          SCRate = p.SCRate,
                                          LCRate = p.LCRate,
                                          ReturnStatus = p.ReturnStatus,
                                          Type = p.Type,
                                          Exceed = p.Exceed

                                      }).ToListAsync();
            return paymeanMeans;
        }

        public async Task<List<ReceiptExport>> ExportReceiptsAsync()
        {
            var receiptGets = await (from r in _context.Receipt.Where(x => !x.Delete)

                                     join acc in _context.UserAccounts.Where(d => !d.Delete) on r.UserOrderID equals acc.ID
                                     join cus in _context.BusinessPartners.Where(d => !d.Delete) on r.CustomerID equals cus.ID
                                     join pl in _context.PriceLists.Where(d => !d.Delete) on r.PriceListID equals pl.ID
                                     join lc in _context.Currency.Where(d => !d.Delete) on r.LocalCurrencyID equals lc.ID
                                     join sc in _context.Currency.Where(d => !d.Delete) on r.SysCurrencyID equals sc.ID
                                     join wh in _context.Warehouses.Where(d => !d.Delete) on r.WarehouseID equals wh.ID
                                     join br in _context.Branches.Where(d => !d.Delete) on r.BranchID equals br.ID
                                     join cp in _context.Company.Where(d => !d.Delete) on r.CompanyID equals cp.ID
                                     join plc in _context.Currency.Where(d => !d.Delete) on r.PLCurrencyID equals plc.ID
                                     join sr in _context.Series on r.SeriesID equals sr.ID
                                     join sd in _context.SeriesDetails on r.SeriesDID equals sd.ID
                                     join tax in _context.TaxGroups.Where(d => !d.Delete) on r.TaxGroupID equals tax.ID into gt
                                     from t in gt.DefaultIfEmpty()
                                     let _t = t ?? new TaxGroup()//Optional


                                     select new ReceiptExport
                                     {
                                         PostingDate = r.PostingDate,
                                         OrderNo = r.OrderNo,
                                         TableID = r.TableID,
                                         ReceiptNo = r.ReceiptNo,
                                         QueueNo = r.QueueNo,
                                         DateIn = r.DateIn,
                                         DateOut = r.DateOut,
                                         TimeIn = r.TimeIn,
                                         TimeOut = r.TimeOut,
                                         WaiterID = r.WaiterID,
                                         Username = acc.Username,
                                         UserDiscountID = acc.Username,
                                         CustomerCode = cus.Code,
                                         CustomerCount = r.CustomerCount,
                                         PriceListName = pl.Name,
                                         LocalCurrency = lc.Symbol,
                                         SysCurrency = sc.Symbol,
                                         ExchangeRate = r.ExchangeRate,
                                         WarehouseCode = wh.Code,
                                         BranchName = br.Name,
                                         CompanyID = cp.Name,
                                         Sub_Total = r.Sub_Total,
                                         DiscountRate = r.DiscountRate,
                                         DiscountValue = r.DiscountValue,
                                         TypeDis = r.TypeDis,
                                         TaxRate = r.TaxRate,
                                         TaxValue = r.TaxValue,
                                         OtherPaymentGrandTotal = r.OtherPaymentGrandTotal,
                                         OpenOtherPaymentGrandTotal = r.OpenOtherPaymentGrandTotal,
                                         GrandTotal = r.GrandTotal,
                                         GrandTotal_Sys = r.GrandTotal_Sys,
                                         Tip = r.Tip,
                                         Received = r.Received,
                                         Change = r.Change,
                                         CurrencyDisplay = r.CurrencyDisplay,
                                         DisplayRate = r.DisplayRate,
                                         GrandTotal_Display = r.GrandTotal_Display,
                                         Change_Display = r.Change_Display,
                                         PaymentMeansID = r.PaymentMeansID,
                                         CheckBill = r.CheckBill,
                                         Cancel = r.Cancel,
                                         Delete = r.Delete,
                                         Return = r.Return,
                                         PLCurrencyID = plc.Symbol,
                                         PLRate = r.PLRate,
                                         SeriesCode = sr.Name,
                                         SeriesNumber = sd.Number,
                                         AppliedAmount = r.AppliedAmount,
                                         LocalSetRate = r.LocalSetRate,
                                         Male = r.Male,
                                         Female = r.Female,
                                         Children = r.Children,
                                         Status = r.Status,
                                         AmountFreight = r.AmountFreight,
                                         VehicleID = r.VehicleID,
                                         TaxOption = r.TaxOption,
                                         PromoCodeID = r.PromoCodeID,
                                         PromoCodeDiscRate = r.PromoCodeDiscRate,
                                         PromoCodeDiscValue = r.PromoCodeDiscValue,
                                         RemarkDiscount = r.RemarkDiscount,
                                         BuyXAmountGetXDisID = r.BuyXAmountGetXDisID,
                                         BuyXAmGetXDisRate = r.BuyXAmGetXDisRate,
                                         BuyXAmGetXDisValue = r.BuyXAmGetXDisValue,
                                         CardMemberDiscountRate = r.CardMemberDiscountRate,
                                         CardMemberDiscountValue = r.CardMemberDiscountValue,
                                         ReceivedPoint = r.ReceivedPoint,
                                         CumulativePoint = r.CumulativePoint,
                                         BuyXAmGetXDisType = r.BuyXAmGetXDisType,
                                         TaxGroupID = _t.Code,
                                         //GrandTotalCurrencies = r.GrandTotalCurrencies,
                                         GrandTotalCurrenciesDisplay = r.ChangeCurrenciesDisplay,
                                         //ChangeCurrencies = r.ChangeCurrencies,
                                         ChangeCurrenciesDisplay = r.ChangeCurrenciesDisplay,
                                         GrandTotalOtherCurrenciesDisplay = r.GrandTotalOtherCurrenciesDisplay,
                                         //GrandTotalOtherCurrencies = r.GrandTotalOtherCurrencies,
                                         //DisplayPayOtherCurrency = r.DisplayPayOtherCurrency,
                                         PaymentType = r.PaymentType,
                                         BalanceReturn = r.BalanceReturn,
                                         BalancePay = r.BalancePay,
                                         BalanceToPay = r.BalanceToPay
                                     }).ToListAsync();
            return receiptGets;
        }

        public async Task<List<ReceiptDetailsExport>> ExportReceiptsDetailAsync()
        {
            var receiptDetailGets = await (from rd in _context.ReceiptDetail
                                           join r in _context.Receipt.Where(d => !d.Delete) on rd.ReceiptID equals r.ReceiptID
                                           join s in _context.Series on r.SeriesID equals s.ID
                                           join sd in _context.SeriesDetails on r.SeriesDID equals sd.ID
                                           join tax in _context.TaxGroups on rd.TaxGroupID equals tax.ID into tgt
                                           from _tax in tgt.DefaultIfEmpty()
                                           let taxx = _tax ?? new TaxGroup()
                                           join uom in _context.UnitofMeasures on rd.UomID equals uom.ID
                                           select new ReceiptDetailsExport
                                           {
                                               SerialCode = s.Name,
                                               SeriesNumber = sd.Number,
                                               LineID = rd.LineID,
                                               Line_ID = rd.Line_ID,
                                               Code = rd.Code,
                                               ItemID = rd.ItemID,
                                               KhmerName = rd.KhmerName,
                                               AmountFreight = rd.AmountFreight,
                                               EnglishName = rd.EnglishName,
                                               Qty = rd.Qty,
                                               PrintQty = rd.PrintQty,
                                               OpenQty = rd.OpenQty,
                                               UnitPrice = rd.UnitPrice,
                                               Cost = rd.Cost,
                                               DiscountRate = rd.DiscountRate,
                                               DiscountValue = rd.DiscountValue,
                                               TypeDis = rd.TypeDis,
                                               TaxGroupCode = taxx.Code,
                                               TaxRate = rd.TaxRate,
                                               TaxValue = rd.TaxValue,
                                               Total = rd.Total,
                                               Total_Sys = rd.Total_Sys,
                                               TotalNet = rd.TotalNet,
                                               UomCode = uom.Code,
                                               ItemStatus = rd.ItemStatus,
                                               ItemPrintTo = rd.ItemPrintTo,
                                               Currency = rd.Currency,
                                               Comment = rd.Comment,
                                               ItemType = rd.ItemType,
                                               Description = rd.Description,
                                               ParentLineID = rd.ParentLineID,
                                               ParentLevel = rd.ParentLevel,
                                               PromoType = rd.PromoType,
                                               LinePosition = rd.LinePosition,
                                               //UnitofMeansure = rd.UnitofMeansure,
                                               IsKsms = rd.IsKsms,
                                               IsKsmsMaster = rd.IsKsmsMaster,
                                               KSServiceSetupId = rd.KSServiceSetupId,
                                               VehicleId = rd.VehicleId,
                                               IsScale = rd.IsScale,
                                               ComboSaleType = rd.ComboSaleType,
                                               IsReadonly = rd.IsReadonly,
                                               RemarkDiscountID = rd.RemarkDiscountID
                                           }).ToListAsync();
            return receiptDetailGets;
        }

        public async Task ImportReceiptsAsync(IWorkbook workbook, ModelStateDictionary modelState)
        {
            List<ReceiptExport> receiptExports = new List<ReceiptExport>();
            List<Receipt> receiptsImports = new List<Receipt>();
            List<MultiPaymentMeans> paymentMeansImports = new List<MultiPaymentMeans>();
            List<PaymentMeanExport> paymentMeanExports = new List<PaymentMeanExport>();
            List<ReceiptDetailsExport> receiptDetailsExports = new List<ReceiptDetailsExport>();
            List<ReceiptDetail> receiptDetailsImports = new List<ReceiptDetail>();
            List<ReceiptFreightExport> receiptFreightExports = new List<ReceiptFreightExport>();
            List<FreightReceipt> freightReceiptsImports = new List<FreightReceipt>();

            for (int i = 0; i < workbook.NumberOfSheets; i++)
            {
                ISheet sheet = workbook.GetSheetAt(i);
                switch (sheet.SheetName)
                {
                    case nameof(ReceiptExport):
                        receiptExports = _wbContext.ToList<ReceiptExport>(sheet);
                        receiptsImports = ImportReceiptToDB(receiptExports, receiptsImports);
                        break;
                    case nameof(ReceiptDetailsExport):
                        receiptDetailsExports = _wbContext.ToList<ReceiptDetailsExport>(sheet);
                        receiptDetailsImports = ImportReceiptDetailToDB(receiptDetailsExports, receiptDetailsImports);
                        break;
                    case nameof(ReceiptFreightExport):
                        receiptFreightExports = _wbContext.ToList<ReceiptFreightExport>(sheet);
                        freightReceiptsImports = ImportReceiptFreight(receiptFreightExports, freightReceiptsImports);
                        break;
                    case nameof(PaymentMeanExport):
                        paymentMeanExports = _wbContext.ToList<PaymentMeanExport>(sheet);
                        paymentMeansImports = ImportMultiPaymentMeanToDb(paymentMeanExports, paymentMeansImports);
                        break;
                }
            }
            List<Receipt> _noStockReceipts = new List<Receipt>();
            _noStockReceipts = receiptsImports.Where(r => _issueStock.InvalidStock(r.RececiptDetail = receiptDetailsImports.Where(rd => rd.ReceiptID == r.ReceiptID).ToList())).ToList();
            if (_noStockReceipts.Any())
            {
                modelState.AddModelError("Receipt", "Some receipts have invalid stock.");
            }
            if (modelState.IsValid)
            {
                foreach (var getReceiptImport in receiptsImports)
                {
                    if (_context.InventoryAudits.Any(ia => ia.InvoiceNo == getReceiptImport.ReceiptNo && ia.Qty < 0))
                    {
                        modelState.AddModelError("Receipt", "Receipts have already added. Try to check file again !!");
                        break;
                    }
                    else
                    {
                        getReceiptImport.MultiPaymentMeans = paymentMeansImports;
                        await _issueStock.IssueStockAsync(getReceiptImport, OrderStatus.Paid, new List<SerialNumber>(), new List<BatchNo>(), getReceiptImport.MultiPaymentMeans);
                    }
                }
                if (modelState.IsValid)
                {
                    modelState.AddModelError("Receipt", "Receipts added success.");
                }
            }
        }
        private bool getBool = true;
        private List<Receipt> ImportReceiptToDB(List<ReceiptExport> receiptExportsGets, List<Receipt> receiptsImports)
        {
            foreach (var getReceipt in receiptExportsGets)
            {
                if (getReceipt != null)
                {
                    Receipt importEx = new();
                    importEx.PostingDate = getReceipt.PostingDate;
                    importEx.OrderNo = getReceipt.OrderNo;
                    importEx.TableID = getReceipt.TableID;
                    importEx.ReceiptNo = getReceipt.ReceiptNo;
                    importEx.QueueNo = getReceipt.QueueNo;
                    importEx.DateIn = getReceipt.DateIn;
                    importEx.DateOut = getReceipt.DateOut;
                    importEx.TimeIn = getReceipt.TimeIn;
                    importEx.TimeOut = getReceipt.TimeOut;
                    importEx.WaiterID = getReceipt.WaiterID;
                    importEx.CustomerCount = getReceipt.CustomerCount;
                    importEx.ExchangeRate = getReceipt.ExchangeRate;
                    importEx.Sub_Total = getReceipt.Sub_Total;
                    importEx.DiscountRate = getReceipt.DiscountRate;
                    importEx.DiscountValue = getReceipt.DiscountValue;
                    importEx.TypeDis = getReceipt.TypeDis;
                    importEx.TaxRate = getReceipt.TaxRate;
                    importEx.TaxValue = getReceipt.TaxValue;
                    importEx.OtherPaymentGrandTotal = getReceipt.OtherPaymentGrandTotal;
                    importEx.OpenOtherPaymentGrandTotal = getReceipt.OpenOtherPaymentGrandTotal;
                    importEx.GrandTotal = getReceipt.GrandTotal;
                    importEx.GrandTotal_Sys = getReceipt.GrandTotal_Sys;
                    importEx.Tip = getReceipt.Tip;
                    importEx.Received = getReceipt.Received;
                    importEx.Change = getReceipt.Change;
                    importEx.CurrencyDisplay = getReceipt.CurrencyDisplay;
                    importEx.DisplayRate = getReceipt.DisplayRate;
                    importEx.GrandTotal_Display = getReceipt.GrandTotal_Display;
                    importEx.Change_Display = getReceipt.Change_Display;
                    importEx.PaymentMeansID = getReceipt.PaymentMeansID;
                    importEx.CheckBill = getReceipt.CheckBill;
                    importEx.Cancel = getReceipt.Cancel;
                    importEx.Delete = getReceipt.Delete;
                    importEx.Return = getReceipt.Return;
                    importEx.PLRate = getReceipt.PLRate;
                    importEx.AppliedAmount = getReceipt.AppliedAmount;
                    importEx.LocalSetRate = getReceipt.LocalSetRate;
                    importEx.Male = getReceipt.Male;
                    importEx.Female = getReceipt.Female;
                    importEx.Children = getReceipt.Children;
                    importEx.Status = getReceipt.Status;
                    importEx.AmountFreight = getReceipt.AmountFreight;
                    importEx.VehicleID = getReceipt.VehicleID;
                    importEx.PromoCodeID = getReceipt.PromoCodeID;
                    importEx.PromoCodeDiscRate = getReceipt.PromoCodeDiscRate;
                    importEx.PromoCodeDiscValue = getReceipt.PromoCodeDiscValue;
                    importEx.RemarkDiscount = getReceipt.RemarkDiscount;
                    importEx.BuyXAmountGetXDisID = getReceipt.BuyXAmountGetXDisID;
                    importEx.BuyXAmGetXDisRate = getReceipt.BuyXAmGetXDisRate;
                    importEx.BuyXAmGetXDisValue = getReceipt.BuyXAmGetXDisValue;
                    importEx.CardMemberDiscountRate = getReceipt.CardMemberDiscountRate;
                    importEx.CardMemberDiscountValue = getReceipt.CardMemberDiscountValue;
                    importEx.ReceivedPoint = getReceipt.ReceivedPoint;
                    importEx.CumulativePoint = getReceipt.CumulativePoint;
                    importEx.BuyXAmGetXDisType = getReceipt.BuyXAmGetXDisType;
                    importEx.GrandTotalCurrenciesDisplay = getReceipt.GrandTotalCurrenciesDisplay;
                    importEx.ChangeCurrenciesDisplay = getReceipt.ChangeCurrenciesDisplay;
                    importEx.GrandTotalOtherCurrenciesDisplay = getReceipt.GrandTotalOtherCurrenciesDisplay;
                    importEx.PaymentType = getReceipt.PaymentType;
                    importEx.BalanceReturn = getReceipt.BalanceReturn;
                    importEx.BalancePay = getReceipt.BalancePay;
                    importEx.BalanceToPay = getReceipt.BalanceToPay;
                    importEx.TaxOption = getReceipt.TaxOption;

                    var getTaxId = _context.TaxGroups.FirstOrDefault(t => t.Code == getReceipt.TaxGroupID);
                    var getUserId = _context.UserAccounts.FirstOrDefault(u => u.Username == getReceipt.Username);
                    var getCusId = _context.BusinessPartners.FirstOrDefault(c => c.Code == getReceipt.CustomerCode);
                    var getPLId = _context.PriceLists.FirstOrDefault(p => p.Name == getReceipt.PriceListName);
                    var getWhId = _context.Warehouses.FirstOrDefault(w => w.Code == getReceipt.WarehouseCode);
                    var getBranchId = _context.Branches.FirstOrDefault(b => b.Name == getReceipt.BranchName);
                    var getCompanyId = _context.Company.FirstOrDefault(c => c.Name == getReceipt.CompanyID);
                    var getSeriesId = _context.Series.FirstOrDefault(s => s.Name == getReceipt.SeriesCode);
                    var getCurrencyId = _context.Currency.Where(d => !d.Delete).ToList();

                    foreach (var c in getCurrencyId)
                    {
                        if (c.Symbol == getReceipt.LocalCurrency) { importEx.LocalCurrencyID = c.ID; }
                        if (c.Symbol == getReceipt.SysCurrency) { importEx.SysCurrencyID = c.ID; }
                        if (c.Symbol == getReceipt.PLCurrencyID) { importEx.PLCurrencyID = c.ID; }
                    }
                    if (getTaxId != null) { if (getTaxId.Delete == false) { importEx.TaxGroupID = getTaxId.ID; } }
                    if (getCompanyId.Delete == false) { importEx.CompanyID = getCompanyId.ID; }
                    if (getSeriesId != null) { importEx.SeriesID = getSeriesId.ID; }
                    if (getWhId.Delete == false) { importEx.WarehouseID = getWhId.ID; }
                    if (getBranchId.Delete == false) { importEx.BranchID = getBranchId.ID; }
                    if (getCusId.Delete == false) { importEx.CustomerID = getCusId.ID; }
                    if (getPLId.Delete == false) { importEx.PriceListID = getPLId.ID; }
                    if (getUserId.Delete == false)
                    {
                        importEx.UserOrderID = getUserId.ID;
                        importEx.UserDiscountID = getUserId.ID;
                    }
                    SeriesDetail seriesDetail = new();
                    seriesDetail.Number = getReceipt.SeriesNumber;
                    seriesDetail.SeriesID = getSeriesId.ID;
                    var series = _context.Series.FirstOrDefault(s => s.Name == getReceipt.SeriesCode);
                    var seriesDetails = _context.SeriesDetails.FirstOrDefault(sd => sd.Number == getReceipt.SeriesNumber && sd.SeriesID == series.ID);
                    if (seriesDetails == null || !_context.Receipt.Any(r => r.SeriesDID == seriesDetails.ID))
                    {
                        _context.SeriesDetails.Add(seriesDetail);
                        _context.SaveChanges();
                        var getSerialDID = _context.SeriesDetails.LastOrDefault();
                        importEx.SeriesDID = getSerialDID.ID;
                        _context.Receipt.Add(importEx);
                        _context.SaveChanges();
                        var getReceiptId = _context.Receipt.LastOrDefault();
                        importEx.ReceiptID = getReceiptId.ReceiptID;
                        getBool = true;
                    }
                    else
                    {
                        getBool = false;
                        var getReceiptId = _context.Receipt.FirstOrDefault(r => r.SeriesDID == seriesDetails.ID);
                        importEx.ReceiptID = getReceiptId.ReceiptID;
                        importEx.SeriesDID = seriesDetails.ID;
                    }
                    receiptsImports.Add(importEx);
                }
            }
            return receiptsImports;
        }
        private List<ReceiptDetail> ImportReceiptDetailToDB(List<ReceiptDetailsExport> receiptDetails, List<ReceiptDetail> receiptDetailsImports)
        {
            foreach (var getReceiptDetail in receiptDetails)
            {
                if (getReceiptDetail != null)
                {
                    ReceiptDetail importReceiptDetail = new();
                    importReceiptDetail.LineID = getReceiptDetail.LineID;
                    importReceiptDetail.Line_ID = getReceiptDetail.Line_ID;
                    importReceiptDetail.Code = getReceiptDetail.Code;
                    importReceiptDetail.KhmerName = getReceiptDetail.KhmerName;
                    importReceiptDetail.AmountFreight = getReceiptDetail.AmountFreight;
                    importReceiptDetail.EnglishName = getReceiptDetail.EnglishName;
                    importReceiptDetail.Qty = getReceiptDetail.Qty;
                    importReceiptDetail.PrintQty = getReceiptDetail.PrintQty;
                    importReceiptDetail.OpenQty = getReceiptDetail.OpenQty;
                    importReceiptDetail.UnitPrice = getReceiptDetail.UnitPrice;
                    importReceiptDetail.Cost = getReceiptDetail.Cost;
                    importReceiptDetail.DiscountRate = getReceiptDetail.DiscountRate;
                    importReceiptDetail.DiscountValue = getReceiptDetail.DiscountValue;
                    importReceiptDetail.TypeDis = getReceiptDetail.TypeDis;
                    importReceiptDetail.TaxRate = getReceiptDetail.TaxRate;
                    importReceiptDetail.TaxValue = getReceiptDetail.TaxValue;
                    importReceiptDetail.Total = getReceiptDetail.Total;
                    importReceiptDetail.Total_Sys = getReceiptDetail.Total_Sys;
                    importReceiptDetail.TotalNet = getReceiptDetail.TotalNet;
                    importReceiptDetail.ItemStatus = getReceiptDetail.ItemStatus;
                    importReceiptDetail.ItemPrintTo = getReceiptDetail.ItemPrintTo;
                    importReceiptDetail.Currency = getReceiptDetail.Currency;
                    importReceiptDetail.Comment = getReceiptDetail.Comment;
                    importReceiptDetail.ItemType = getReceiptDetail.ItemType;
                    importReceiptDetail.Description = getReceiptDetail.Description;
                    importReceiptDetail.ParentLineID = getReceiptDetail.ParentLineID;
                    importReceiptDetail.ParentLevel = getReceiptDetail.ParentLevel;
                    importReceiptDetail.PromoType = getReceiptDetail.PromoType;
                    importReceiptDetail.LinePosition = getReceiptDetail.LinePosition;
                    importReceiptDetail.IsKsms = getReceiptDetail.IsKsms;
                    importReceiptDetail.IsKsmsMaster = getReceiptDetail.IsKsmsMaster;
                    importReceiptDetail.KSServiceSetupId = getReceiptDetail.KSServiceSetupId;
                    importReceiptDetail.VehicleId = getReceiptDetail.VehicleId;
                    importReceiptDetail.IsScale = getReceiptDetail.IsScale;
                    importReceiptDetail.ComboSaleType = getReceiptDetail.ComboSaleType;
                    importReceiptDetail.IsReadonly = getReceiptDetail.IsReadonly;
                    importReceiptDetail.RemarkDiscountID = getReceiptDetail.RemarkDiscountID;
                    var getItemId = _context.ItemMasterDatas.FirstOrDefault(im => im.Code == getReceiptDetail.Code);
                    var series = _context.Series.FirstOrDefault(s => s.Name == getReceiptDetail.SerialCode);
                    var seriesDetail = _context.SeriesDetails.FirstOrDefault(sd => sd.SeriesID == series.ID && sd.Number == getReceiptDetail.SeriesNumber);
                    var receipt = _context.Receipt.FirstOrDefault(r => r.SeriesDID == seriesDetail.ID && r.SeriesID == series.ID);
                    var getTaxId = _context.TaxGroups.FirstOrDefault(t => t.Code == getReceiptDetail.TaxGroupCode);
                    var getUOMId = _context.UnitofMeasures.FirstOrDefault(u => u.Code == getReceiptDetail.UomCode);
                    if (getItemId != null) { importReceiptDetail.ItemID = getItemId.ID; }
                    if (receipt != null) { importReceiptDetail.ReceiptID = receipt.ReceiptID; }
                    if (getTaxId != null) { importReceiptDetail.TaxGroupID = getTaxId.ID; }
                    if (getUOMId != null) { importReceiptDetail.UomID = getUOMId.ID; }
                    if (!_context.ReceiptDetail.Any(rd => rd.ReceiptID == receipt.ReceiptID))
                    {
                        _context.ReceiptDetail.Add(importReceiptDetail);
                        _context.SaveChanges();
                    }

                    receiptDetailsImports.Add(importReceiptDetail);
                }
            }
            return receiptDetailsImports;
        }
        private List<FreightReceipt> ImportReceiptFreight(List<ReceiptFreightExport> receiptFreights, List<FreightReceipt> freightReceiptsImports)
        {
            foreach (var getFreightReceipts in receiptFreights)
            {
                if (getFreightReceipts != null)
                {
                    FreightReceipt importFreightReceipt = new();
                    importFreightReceipt.FreightReceiptType = getFreightReceipts.FreightReceiptType;
                    importFreightReceipt.AmountReven = getFreightReceipts.AmountReven;
                    importFreightReceipt.IsActive = getFreightReceipts.IsActive;
                    var getFreightId = _context.Freights.FirstOrDefault(f => f.Name == getFreightReceipts.FreightName);
                    var series = _context.Series.FirstOrDefault(s => s.Name == getFreightReceipts.SeriesCode);
                    var seriesDetail = _context.SeriesDetails.FirstOrDefault(sd => sd.SeriesID == series.ID && sd.Number == getFreightReceipts.SeriesNumber);
                    var receipt = _context.Receipt.FirstOrDefault(r => r.SeriesDID == seriesDetail.ID && r.SeriesID == series.ID);
                    if (getFreightId != null) { importFreightReceipt.FreightID = getFreightId.ID; }
                    if (receipt != null) { importFreightReceipt.ReceiptID = receipt.ReceiptID; }
                    var get = getBool;
                    if (getBool == true)
                    {
                        _context.FreightReceipts.Add(importFreightReceipt);
                        _context.SaveChanges();
                    }
                    freightReceiptsImports.Add(importFreightReceipt);
                }
            }
            return freightReceiptsImports;
        }

        private List<MultiPaymentMeans> ImportMultiPaymentMeanToDb(List<PaymentMeanExport> paymentMeanExports, List<MultiPaymentMeans> paymentMeansImports)
        {
            foreach (var getPaymentMean in paymentMeanExports)
            {
                if (getPaymentMean != null)
                {
                    MultiPaymentMeans importPaymeanMean = new();
                    importPaymeanMean.AltCurrency = getPaymentMean.AltCurrency;
                    importPaymeanMean.AltRate = getPaymentMean.AltRate;
                    importPaymeanMean.PLCurrency = getPaymentMean.PLCurrency;
                    importPaymeanMean.PLRate = getPaymentMean.PLRate;
                    importPaymeanMean.Amount = getPaymentMean.Amount;
                    importPaymeanMean.OpenAmount = getPaymentMean.OpenAmount;
                    importPaymeanMean.Total = getPaymentMean.Total;
                    importPaymeanMean.SCRate = getPaymentMean.SCRate;
                    importPaymeanMean.LCRate = getPaymentMean.LCRate;
                    importPaymeanMean.ReturnStatus = getPaymentMean.ReturnStatus;
                    importPaymeanMean.Type = getPaymentMean.Type;
                    importPaymeanMean.Exceed = getPaymentMean.Exceed;
                    var series = _context.Series.FirstOrDefault(s => s.Name == getPaymentMean.SerialCode);
                    var seriesDetail = _context.SeriesDetails.FirstOrDefault(sd => sd.SeriesID == series.ID && sd.Number == getPaymentMean.SeriesNumber);
                    var receipt = _context.Receipt.FirstOrDefault(r => r.SeriesDID == seriesDetail.ID && r.SeriesID == series.ID);
                    var getPaymentMeanId = _context.PaymentMeans.FirstOrDefault(p => p.Type == getPaymentMean.PaymentMeanType);
                    var getCurrencyId = _context.Currency.Where(d => !d.Delete).ToList();
                    foreach (var getCurrency in getCurrencyId)
                    {
                        if (getCurrency.Symbol == getPaymentMean.PLCurrencyName) { importPaymeanMean.PLCurrencyID = getCurrency.ID; }
                        if (getCurrency.Symbol == getPaymentMean.AltCurrencyName) { importPaymeanMean.AltCurrencyID = getCurrency.ID; }
                    }
                    if (getPaymentMeanId.Delete == false) { importPaymeanMean.PaymentMeanID = getPaymentMeanId.ID; }
                    if (receipt != null) { importPaymeanMean.ReceiptID = receipt.ReceiptID; }
                    var get = getBool;
                    if (getBool == true)
                    {
                        _context.MultiPaymentMeans.Add(importPaymeanMean);
                        _context.SaveChanges();
                    }
                    paymentMeansImports.Add(importPaymeanMean);
                }
            }
            return paymentMeansImports;

        }


    }
}
