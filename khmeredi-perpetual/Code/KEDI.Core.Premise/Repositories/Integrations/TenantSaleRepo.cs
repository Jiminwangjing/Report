using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CKBS.AppContext;
using CKBS.Models.Services.Account;
using CKBS.Models.Services.Administrator.General;
using CKBS.Models.Services.Administrator.Inventory;
using CKBS.Models.Services.Administrator.SystemInitialization;
using CKBS.Models.Services.Administrator.Tables;
using CKBS.Models.Services.Banking;
using CKBS.Models.Services.HumanResources;
using CKBS.Models.Services.Inventory.PriceList;
using CKBS.Models.Services.POS;
using CKBS.Models.Services.POS.service;
using CKBS.Models.Services.Responsitory;
using KEDI.Core.Premise.Models.ClientApi.Sale;
using KEDI.Core.Premise.Models.Services.POS;
using KEDI.Core.Premise.Repositories.Sync;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace KEDI.Core.Premise.Repositories.Integrations
{
    public interface ITenantSaleRepo {
        Task<Receipt> CreateReceiptAsync(SaleReceipt invoice);
    }
    public class TenantSaleRepo : ITenantSaleRepo
    {
        private readonly ILogger<TenantSaleRepo> _logger;
        private readonly DataContext _dataContext;
        private readonly IPOS _pos;
        public TenantSaleRepo(
            ILogger<TenantSaleRepo> logger, DataContext dataContext, IPOS pos)
        {
            _logger = logger;
            _dataContext = dataContext;
            _pos = pos;
        }
         //From external sources
        public async Task<Receipt> CreateReceiptAsync(SaleReceipt invoice)
        {
            _ = DateTime.TryParse(invoice.PostingDate, out DateTime _postingDate);
            var saleItems = GetSaleItems();
            var table = await _dataContext.Tables.FirstOrDefaultAsync() ?? new Table();
            var user = await _dataContext.UserAccounts.Include(c => c.Company).FirstOrDefaultAsync() ?? new UserAccount { Company = new Company() };
            var setting = await _dataContext.GeneralSettings.FirstOrDefaultAsync(s => s.UserID == user.ID) ?? new GeneralSetting();
            var cus = await _dataContext.BusinessPartners.FirstOrDefaultAsync(b => b.Type == "Customer") ?? new BusinessPartner();
            var priceList = await _dataContext.PriceLists.FindAsync(setting.PriceListID) ?? new PriceLists();
            var warehouse = await _dataContext.Warehouses.FirstOrDefaultAsync(w => w.ID == setting.WarehouseID) ?? new Warehouse();
            var plRate = await _dataContext.ExchangeRates.FirstOrDefaultAsync(r => r.CurrencyID == priceList.CurrencyID) ?? new ExchangeRate();
            var sysRate = await _dataContext.ExchangeRates.FirstOrDefaultAsync(r => r.CurrencyID == user.Company.SystemCurrencyID) ?? new ExchangeRate();
            var localRate = await _dataContext.ExchangeRates.FirstOrDefaultAsync(w => w.CurrencyID == user.Company.LocalCurrencyID) ?? new ExchangeRate();
            var plCurrency = await _dataContext.Currency.FirstOrDefaultAsync(c => c.ID == priceList.CurrencyID) ?? new Currency();
            var paymentMean = await _dataContext.PaymentMeans.FirstOrDefaultAsync(pm => pm.ID == setting.PaymentMeansID) ?? new PaymentMeans();
            var seriesDetail = await UpdateSeriesAsync(setting);
            var receipt = new Receipt
            {
                ReceiptNo = invoice.ReceiptNo,
                PriceListID = priceList.ID,
                QueueNo = string.Empty,
                OrderNo = string.Empty,
                PostingDate = _postingDate,
                DateIn = DateTime.Today,
                DateOut = DateTime.Today,
                TimeIn = DateTime.Now.ToString("hh\\:mm\\:ss tt"),
                TimeOut = DateTime.Now.ToString("hh\\:mm\\:ss tt"),
                Table = table,
                TableID = table.ID,
                UserAccount = user,
                UserOrderID = user.ID,
                DiscountRate = invoice.DiscountRate,
                DiscountValue = invoice.DiscountValue,
                GrandTotal = (double)invoice.Grandtotal,
                GrandTotalAmount = $"{invoice.Grandtotal}",
                Sub_Total = (double)invoice.Subtotal,
                TaxRate = invoice.TaxRate,
                TaxValue = invoice.TaxValue,
                CustomerID = cus.ID,
                LocalCurrencyID = user.Company.LocalCurrencyID,
                SysCurrencyID = user.Company.SystemCurrencyID,
                ExchangeRate = sysRate.RateOut,
                BranchID = user.BranchID,
                WarehouseID = warehouse.ID,
                PLRate = plRate.SetRate,
                GrandTotal_Sys = (double)invoice.Grandtotal * sysRate.RateOut,
                LocalSetRate = localRate.SetRate,
                SeriesID = seriesDetail.SeriesID,
                SeriesDID = seriesDetail.ID,
                CompanyID = user.Company.ID,
                PLCurrencyID = plCurrency.ID,
                PaymentMeansID = paymentMean.ID,
                Cancel = invoice.Cancelled
            };

            receipt.RececiptDetail = invoice.SaleReceiptDetails
                .Select(invDetail => NewReceiptDetail(receipt.ReceiptID, invDetail, saleItems)).ToList();
            if (receipt.RececiptDetail.Count > 0)
            {
                bool isExisting = _dataContext.Receipt.AsNoTracking().Any(r => r.ReceiptNo == receipt.ReceiptNo);
                if (!isExisting)
                {
                    var db = await _dataContext.Database.BeginTransactionAsync();
                    _dataContext.Receipt.Update(receipt);
                    await _dataContext.SaveChangesAsync();

                    var multiPayments = new MultiPaymentMeans 
                    {
                        ReceiptID = receipt.ReceiptID,
                        PaymentMeanID = receipt.PaymentMeansID,
                        AltCurrencyID = receipt.SysCurrencyID,
                        AltCurrency = plCurrency.Description,
                        PLCurrencyID = receipt.PLCurrencyID, 
                        PLCurrency = plCurrency.Description,
                        Amount = Convert.ToDecimal(receipt.GrandTotal), 
                        OpenAmount = Convert.ToDecimal(receipt.GrandTotal), 
                        Total = Convert.ToDecimal(receipt.Received),
                        AltRate = Convert.ToInt32(receipt.ExchangeRate),
                        PLRate = receipt.PLRate,
                        LCRate = Convert.ToDecimal(receipt.LocalSetRate),
                        SCRate = Convert.ToDecimal(receipt.ExchangeRate)  
                    };
                    _dataContext.MultiPaymentMeans.Add(multiPayments);
                    await _dataContext.SaveChangesAsync();
                    db.Commit();
                }
            }

            return receipt;
        }

        public ReceiptDetail NewReceiptDetail(
            int receiptId, SaleReceiptDetail lineItem, IEnumerable<ServiceItemSales> saleItems = null
        )
        {
            ServiceItemSales item = new ServiceItemSales();
            if (saleItems == null)
            {
                item = GetSaleItems(0, 0, "Standard").FirstOrDefault(i => i.Code == lineItem.ItemCode);
            }
            else
            {
                item = saleItems.FirstOrDefault(i => i.Code == lineItem.ItemCode);
            }

            ReceiptDetail rd = null;
            if (item != null)
            {
                rd = new ReceiptDetail
                {
                    ReceiptID = receiptId,
                    ItemID = item.ItemID,
                    Code = lineItem.ItemCode,
                    TaxRate = lineItem.TaxRate,
                    TaxValue = lineItem.Taxvalue,
                    Description = item.Description,
                    DiscountRate = lineItem.DiscountRate,
                    DiscountValue = (double)lineItem.DiscountValue,
                    Total = (double)lineItem.Total,
                    ItemPrintTo = item.PrintTo,
                    KhmerName = item.KhmerName,
                    EnglishName = item.EnglishName,
                    TaxGroupID = item.TaxGroupSaleID,
                    LineID = DateTime.Now.Ticks.ToString(),
                    UomID = item.UomID,
                    Cost = item.Cost,
                    UnitPrice = (double)lineItem.Price,
                    Qty = (double)lineItem.Qty,
                    TotalNet = (double)lineItem.Total
                };
            }

            return rd;
        }

        public async Task<SeriesDetail> UpdateSeriesAsync(GeneralSetting setting)
        {
            var series = await _dataContext.Series.FirstOrDefaultAsync(w => w.ID == setting.SeriesID) ?? new Series();
            SeriesDetail seriesDetail = new();
            if (series.ID > 0)
            {
                //insert seriesDetail
                seriesDetail.SeriesID = series.ID;
                seriesDetail.Number = series.NextNo;
                _dataContext.Update(seriesDetail);
                //update series
                string Sno = series.NextNo;
                int snoLenth = Sno.Length;
                _ = long.TryParse(Sno, out long _sno);
                _sno++;
                series.NextNo = Convert.ToString(_sno).PadLeft(snoLenth, '0');
                _dataContext.Update(series);
                _dataContext.SaveChanges();
            }
            return seriesDetail;
        }

         public IQueryable<ServiceItemSales> GetSaleItems(int priceListId = 0, int warehouseId = 0, string process = "")
        {
            var itemMasters = _dataContext.ItemMasterDatas.Where(im => !im.Delete && im.Sale);

            if (!string.IsNullOrWhiteSpace(process))
            {
                itemMasters = itemMasters.Where(im => string.Compare(im.Process, process, true) == 0);
            }

            var items = from itm in itemMasters
                        join pld in _dataContext.PriceListDetails on itm.ID equals pld.ItemID
                        join plt in _dataContext.PriceLists on pld.PriceListID equals plt.ID
                        join uom in _dataContext.UnitofMeasures.Where(uom => !uom.Delete) on pld.UomID equals uom.ID
                        join cur in _dataContext.Currency.Where(c => !c.Delete) on pld.CurrencyID equals cur.ID
                        join prt in _dataContext.PrinterNames.Where(p => !p.Delete) on itm.PrintToID equals prt.ID
                        join ws in _dataContext.WarehouseSummary.Where(ws => ws.WarehouseID == warehouseId) on pld.ItemID equals ws.ItemID into wsg
                        join pro in _dataContext.Promotions.Where(p => p.Active && p.StartDate <= DateTime.Now && DateTime.Now <= p.StopDate)
                        on pld.PromotionID equals pro.ID into g
                        let guom = _dataContext.GroupDUoMs.FirstOrDefault(g => g.UoMID == uom.ID && g.GroupUoMID == itm.GroupUomID)
                        from _ws in wsg.DefaultIfEmpty()
                        from _pro in g.DefaultIfEmpty()
                        let discount = _pro == null ? 0 : pld.Discount
                        let promoId = _pro == null ? 0 : _pro.ID
                        select new ServiceItemSales
                        {
                            ID = pld.ID,
                            PromotionID = promoId,
                            ItemID = itm.ID,
                            Code = itm.Code,
                            Barcode = itm.Barcode,
                            KhmerName = itm.KhmerName,
                            EnglishName = itm.EnglishName,
                            UnitPrice = pld.UnitPrice,
                            Description = itm.Description,
                            DiscountRate = discount,
                            TypeDis = pld.TypeDis,
                            GroupUomID = itm.GroupUomID,
                            ItemType = itm.Type,
                            Group1 = itm.ItemGroup1ID,
                            Group2 = (int)itm.ItemGroup2ID,
                            Group3 = (int)itm.ItemGroup3ID,
                            UomID = uom.ID,
                            InStock = (((_ws ?? new WarehouseSummary()).InStock == 0) ? 0 : (_ws ?? new WarehouseSummary()).InStock / guom.Factor),
                            UoM = uom.Name,
                            CurrencyID = cur.ID,
                            PricListID = plt.ID,
                            Cost = pld.Cost,
                            IsScale = itm.Scale,
                            TaxGroupSaleID = itm.TaxGroupSaleID,
                            PrintTo = prt.Name,
                            Process = itm.Process,
                            Image = itm.Image
                        };
            if (priceListId > 0)
            {
                items = items.Where(i => i.PricListID == priceListId);
            }
            return items;
        }
    }
}