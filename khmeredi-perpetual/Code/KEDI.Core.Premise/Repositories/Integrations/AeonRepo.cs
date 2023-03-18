using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CKBS.AppContext;
using CKBS.Models.Services.POS;
using CKBS.Models.Services.POS.KVMS;
using KEDI.Core.Premise.Models.ClientApi;
using KEDI.Core.Premise.Models.Integrations.Aeon;
using KEDI.Core.Premise.Repository;
using KEDI.Core.Premise.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace KEDI.Core.Premise.Repositories.Integrations
{
    public interface IAeonRepo : IServiceWorker
    {
        Task<AuthResponse> GetBearerTokenAsync(string username, string password);
        Task<List<TenantSale>> SendAgeingSalesAsync();
        Task<List<TenantSale>> SendReturnSalesAsync();
        Task<List<TenantSale>> GetReportTenantSalesAsync(TenantSaleType saleType, DateTime dateFrom, DateTime dateTo);
    }

    public partial class AeonRepo : ServiceWorker, IAeonRepo {
        private readonly ILogger<AeonRepo> _logger;
        private readonly HttpClient _httpClient;
        private readonly DataContext _context;
        private readonly HostSetting _settings;
        private readonly UtilityModule _util;
        public AeonRepo(ILogger<AeonRepo> logger,
            UtilityModule util,
            DataContext dataContext,
            IHttpClientFactory clientFactory,
            IDictionary<string, HostSetting> hostSettings)
        {   
            _logger = logger;
            _context = dataContext;
            _util = util;
            _httpClient = clientFactory.CreateClient("AeonSync");
            hostSettings.TryGetValue("AeonSync", out HostSetting settings);
            _settings = settings;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try {
                if(!_settings.Enabled){ return; }                        
                double interval = Convert.ToDouble(_settings.TickInterval);
                TimeSpan tickInterval = TimeSpan.FromMinutes(interval);
                if (tickInterval < TimeSpan.FromMinutes(0.5))
                {
                    tickInterval = TimeSpan.FromMinutes(0.5);
                }

                Every = tickInterval;
                StartTime = _settings.StartTime;
                EndTime = _settings.EndTime;

                await Task.Delay(tickInterval, stoppingToken);
                await base.ExecuteAsync(stoppingToken);
            } catch(Exception ex){
                _logger.LogError(ex.Message);
                await ExecuteAsync(stoppingToken);
            }
        }
        protected override async Task WorkAsync(CancellationToken stoppingToken)
        {
            await SendAgeingSalesAsync();
            await SendReturnSalesAsync();
        }
    }

    public partial class AeonRepo : ServiceWorker, IAeonRepo
    {
        public async Task<AuthResponse> GetBearerTokenAsync(string username, string password)
        {
            return await PostDataAsync<AuthResponse>(AeonUrls.BearerToken, new { Username = username, Password = password });
        }

        public async Task<TResult> PostDataAsync<TResult>(string url, object data) where TResult : class, new()
        {
            var responseMsg = await _httpClient.PostAsJsonAsync(url, data);
            responseMsg.EnsureSuccessStatusCode();
            var result = await responseMsg.Content.ReadAsAsync<TResult>() ?? new TResult();
            return result;
        }

        public async Task<TenantSaleResult> PostTenantSaleAsync(TenantSale request)
        {
            try
            {
                if (!_httpClient.DefaultRequestHeaders.Contains("Authorization"))
                {
                    if(string.IsNullOrWhiteSpace(_settings.Username)
                        || string.IsNullOrWhiteSpace(_settings.Password)){
                        throw new ArgumentNullException("Username and password required.");
                    }

                    var auth = await GetBearerTokenAsync(_settings.Username, _settings.Password);
                    _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + auth.Token);
                }

                var responseMsg = await _httpClient.PostAsJsonAsync(AeonUrls.TenantSale, request);
                if (responseMsg.StatusCode == HttpStatusCode.Unauthorized)
                {
                    await Task.Delay(TimeSpan.FromSeconds(5));
                    var auth = await GetBearerTokenAsync(_settings.Username, _settings.Password);
                    _httpClient.DefaultRequestHeaders.Remove("Authorization");
                    _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + auth.Token);
                    responseMsg = await _httpClient.PostAsJsonAsync(AeonUrls.TenantSale, request);
                }

                responseMsg.EnsureSuccessStatusCode();
                var result = await responseMsg.Content.ReadAsAsync<TenantSaleResult>() ?? new TenantSaleResult();
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            return new TenantSaleResult();
        }

        public async Task<List<TenantSale>> SendAgeingSalesAsync()
        {
            var decimalPlace = await (from c in _context.Company.AsNoTracking().Where(c => !c.Delete)
                                      join d in _context.Displays.AsNoTracking() on c.SystemCurrencyID equals d.DisplayCurrencyID
                                      select d).AsNoTracking().FirstOrDefaultAsync();
            var receipts = _context.Receipt.Where(
                                r => !r.Delete && !_context.TransactionAeons.AsNoTracking()
                                    .Any(t => t.RowId.Equals(r.RowId))
                            ).Take(10);
            var ageingSales = new List<TenantSale>();
            if(!receipts.Any()){ return ageingSales; }
            ageingSales = await QueryAgeingSales(receipts, decimalPlace.Amounts).ToListAsync();
            await UpdateTransactionsAsync(ageingSales);
            return ageingSales;
        }

        public async Task<List<TenantSale>> SendReturnSalesAsync()
        {
            var decimalPlace = await (from c in _context.Company.Where(c => !c.Delete)
                                      join d in _context.Displays on c.SystemCurrencyID equals d.DisplayCurrencyID
                                      select d).AsNoTracking().FirstOrDefaultAsync();
            var receiptMemos = _context.ReceiptMemo
                                .Where(r => !_context.TransactionAeons.AsNoTracking()
                                    .Any(t => t.RowId.Equals(r.RowId))
                            ).Take(10);
            var returnedSales = new List<TenantSale>();
            if(!receiptMemos.Any()){ return returnedSales; }
            var returnSales = await QueryReturnSales(receiptMemos, decimalPlace.Amounts).ToListAsync();
            await UpdateTransactionsAsync(returnSales);
            return returnSales;
        }

        private async Task UpdateTransactionsAsync(List<TenantSale> sales)
        {
            if (sales == null) { return; }
            foreach (var s in sales)
            {
                var tenantResult = await PostTenantSaleAsync(s);
                _ = Guid.TryParse(tenantResult.transaction_oid, out Guid _tranId);
                if (!(string.IsNullOrWhiteSpace(tenantResult.transaction_oid) && _tranId == default))
                {
                    _ = DateTime.TryParse(tenantResult.date_time, out DateTime _dt);
                    bool isExist = _context.TransactionAeons.AsNoTracking().Any(t => t.RowId == _tranId);
                    if (!isExist)
                    {
                        _context.TransactionAeons.Add(new TransactionAeon
                        {
                            TxId = tenantResult.transaction_oid,
                            SyncDate = DateTime.Now,
                            ChangeLog = DateTimeOffset.UtcNow,
                            RowId = _tranId
                        });
                        await _context.SaveChangesAsync();
                    }
                }
            }
        }

        public async Task<List<TenantSale>> GetReportTenantSalesAsync(TenantSaleType saleType, DateTime dateFrom, DateTime dateTo)
        {
            List<TenantSale> sales = new List<TenantSale>();
            IQueryable<Receipt> receipts = _context.Receipt.AsNoTracking().Where(r => dateFrom <= r.DateOut && r.DateOut <= dateTo);
            receipts = from r in receipts
                       join t in _context.TransactionAeons.AsNoTracking()
                        on r.RowId equals t.RowId select r;
            IQueryable<ReceiptMemo> receiptMemos = _context.ReceiptMemo.AsNoTracking().Where(r => dateFrom < r.DateOut && r.DateOut < dateTo);
            receiptMemos = from r in receiptMemos
                           join t in _context.TransactionAeons.AsNoTracking()
                        on r.RowId equals t.RowId
                           select r;
            var decimalPlace = await (from c in _context.Company.AsNoTracking().Where(c => !c.Delete)
                                      join d in _context.Displays on c.SystemCurrencyID equals d.DisplayCurrencyID
                                      select d).FirstOrDefaultAsync();
            switch (saleType)
            {
                case TenantSaleType.Ageing:
                    sales = await QueryAgeingSales(receipts, decimalPlace.Amounts).ToListAsync();
                    break;
                case TenantSaleType.Return:
                    sales = await QueryReturnSales(receiptMemos, decimalPlace.Amounts).ToListAsync();
                    break;
                default:
                    sales.AddRange(await QueryAgeingSales(receipts, decimalPlace.Amounts).ToListAsync());
                    sales.AddRange(await QueryReturnSales(receiptMemos, decimalPlace.Amounts).ToListAsync());
                    break;
            }

            sales = sales.Select(s =>
                    {
                        s.date_time = Convert.ToDateTime(s.date_time).ToString("dd-MM-yyyy hh:mm tt");
                        return s;
                    }).ToList();
            return sales;
        }

        public IQueryable<TenantSale> QueryAgeingSales(IQueryable<Receipt> receipts, int decimalPlace = 3)
        {
            var returnSales = from r in receipts
                              join u in _context.UserAccounts on r.UserOrderID equals u.ID
                              let dateTimeOut = r.DateOut.Add(Convert.ToDateTime(r.TimeOut).TimeOfDay)
                              select new TenantSale
                              {
                                  transaction_oid = r.RowId.ToString(),
                                  receipt_id = r.ReceiptNo,
                                  invoice_id = r.ReceiptNo,
                                  document_type = "AgeingSale",
                                  date_time = dateTimeOut.ToString("o"),
                                  currency_name = "Dollar",
                                  discount_type = "ItemOnBill",
                                  discount_amount = (r.DiscountValue * r.ExchangeRate).ToString($"F{decimalPlace}"),
                                  return_qty = "0",
                                  return_amount = "0",
                                  refund_qty = "0",
                                  refund_amount = "0",
                                  payment_method_1 = "Cash",
                                  payment_amount_1 = (r.Received * r.ExchangeRate).ToString($"F{decimalPlace}"),
                                  payment_method_2 = "",
                                  payment_amount_2 = "0",
                                  payment_method_3 = "",
                                  payment_amount_3 = "0",
                                  delivery_service = "Other",
                                  exchange_rate_value = (r.LocalSetRate).ToString($"F{decimalPlace}"),
                                  change_amount_base = (r.Change * r.ExchangeRate).ToString($"F{decimalPlace}"),
                                  change_amount_dollar = (r.Change * r.ExchangeRate).ToString($"F{decimalPlace}"),
                                  vat = (r.TaxValue * r.ExchangeRate).ToString($"F{decimalPlace}"),
                                  cashier_id = u.Username,
                                  amount_before_vat_discount = (r.Sub_Total * r.ExchangeRate).ToString($"F{decimalPlace}")
                              };
            return returnSales.AsNoTracking();
        }

        public IQueryable<TenantSale> QueryReturnSales(IQueryable<ReceiptMemo> receiptMemos, int decimalPlace = 3)
        {
            var returnSales = from r in receiptMemos.Include(rm => rm.ReceiptDetailMemos)
                              join u in _context.UserAccounts on r.UserOrderID equals u.ID
                              let dateTimeOut = r.DateOut.Add(Convert.ToDateTime(r.TimeOut).TimeOfDay)
                              select new TenantSale
                              {
                                  transaction_oid = r.RowId.ToString(),
                                  receipt_id = r.ReceiptNo,
                                  invoice_id = r.ReceiptNo,
                                  document_type = "Return",
                                  date_time = dateTimeOut.ToString("o"),
                                  currency_name = "Dollar",
                                  discount_type = "ItemOnBill",
                                  discount_amount = $"-{r.DisValue}",
                                  return_qty = "0",
                                  return_amount = $"-{(r.BalanceReturn * Convert.ToDecimal(r.ExchangeRate)).ToString($"F{decimalPlace}")}",
                                  refund_qty = "0",
                                  refund_amount = $"-{(r.BalanceReturn * Convert.ToDecimal(r.ExchangeRate)).ToString($"F{decimalPlace}")}",
                                  payment_method_1 = "Cash",
                                  payment_amount_1 = $"-{(r.BalanceReturn * Convert.ToDecimal(r.ExchangeRate)).ToString($"F{decimalPlace}")}",
                                  payment_method_2 = "",
                                  payment_amount_2 = "0",
                                  payment_method_3 = "",
                                  payment_amount_3 = "0",
                                  delivery_service = "Other",
                                  exchange_rate_value = r.LocalSetRate.ToString($"F{decimalPlace}"),
                                  change_amount_dollar = (r.Change * r.ExchangeRate).ToString($"F{decimalPlace}"),
                                  change_amount_base = (r.Change * r.ExchangeRate).ToString($"F{decimalPlace}"),
                                  vat = $"-{(r.TaxValue * r.ExchangeRate).ToString($"F{decimalPlace}")}",
                                  cashier_id = u.Username,
                                  amount_before_vat_discount = $"-{(r.SubTotal * r.ExchangeRate).ToString($"F{decimalPlace}")}"
                              };
            return returnSales.AsNoTracking();
        }
    }
}
