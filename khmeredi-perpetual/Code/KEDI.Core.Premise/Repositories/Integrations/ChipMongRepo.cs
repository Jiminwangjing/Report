using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CKBS.AppContext;
using CKBS.Models.Services.Banking;
using CKBS.Models.Services.LoyaltyProgram.ComboSale;
using CKBS.Models.Services.POS;
using CKBS.Models.Services.POS.KVMS;
using KEDI.Core.Premise.DependencyInjection;
using KEDI.Core.Premise.Models.ClientApi;
using KEDI.Core.Premise.Models.Integrations;
using KEDI.Core.Premise.Models.Integrations.Aeon;
using KEDI.Core.Premise.Models.Integrations.ChipMong;
using KEDI.Core.Premise.Repository;
using KEDI.Core.Premise.Services;
using KEDI.Core.Premise.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace KEDI.Core.Premise.Repositories.Integrations
{
    public interface IChipMongRepo : IServiceWorker
    {
        Task<Dictionary<string, string>> LoginAsync(string username, string password);
        Task SendSaleInvoicesAsync();
        Task<List<SaleInvoice>> GetReportSalesAsync(DateTime dateFrom, DateTime dateTo);
    }

    public partial class ChipMongRepo : ServiceWorker, IChipMongRepo 
    {
        private readonly ILogger<ChipMongRepo> _logger;
        private readonly DataContext _context;
        private readonly HttpClient _httpClient;
        private readonly UtilityModule _util;
        private readonly IConfiguration _config;
        private readonly HostSetting _settings;
        public ChipMongRepo(ILogger<ChipMongRepo> logger,
            DataContext context, IHttpClientFactory clientFactory,
            IDictionary<string, HostSetting> serverHosts)
        {
            _logger = logger;
            _context = context;
            _httpClient = clientFactory.CreateClient("ChipMongSync");
            serverHosts.TryGetValue("ChipMongSync", out HostSetting settings);
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
            await SendSaleInvoicesAsync();         
        }
    }

    public partial class ChipMongRepo : ServiceWorker, IChipMongRepo
    {
        private async Task<TResult> PostDataAsync<TResult>(string url, object data) where TResult : class, new()
        {
            var responseMsg = await _httpClient.PostAsJsonAsync(url, data);
            responseMsg.EnsureSuccessStatusCode();
            var result = await responseMsg.Content.ReadAsAsync<TResult>() ?? new TResult();
            return result;
        }

        public async Task<Dictionary<string, string>> LoginAsync(string username, string password)
        {
            var result = await PostDataAsync<Dictionary<string, string>>
                (
                    ChipMongUrls.Login,
                    new { userId = username, pwd = password }
                );

            return result;
        }

        public async Task<Dictionary<string, string>> PostTenantSaleAsync(SaleInvoice sale)
        {
            Dictionary<string, string> responseResult = new Dictionary<string, string>();
            try
            {
                if (!_httpClient.DefaultRequestHeaders.Contains("Authorization"))
                {
                    if (string.IsNullOrWhiteSpace(_settings.Username)
                        || string.IsNullOrWhiteSpace(_settings.Password))
                    {
                        _logger.LogInformation("Username and password required.");
                        return new Dictionary<string, string>();
                    }

                    var auth = await LoginAsync(_settings.Username, _settings.Password);
                    if (auth.ContainsKey("token"))
                    {
                        _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + auth["token"]);
                    }
                }

                var responseMsg = await _httpClient.PostAsJsonAsync(ChipMongUrls.InsertSale, sale);
                if (responseMsg.StatusCode == HttpStatusCode.Unauthorized)
                {
                    await Task.Delay(TimeSpan.FromSeconds(5));
                    var auth = await LoginAsync(_settings.Username, _settings.Password);
                    _httpClient.DefaultRequestHeaders.Remove("Authorization");
                    _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + auth["token"]);
                    responseMsg = await _httpClient.PostAsJsonAsync(ChipMongUrls.InsertSale, sale);
                }

                responseMsg.EnsureSuccessStatusCode();
                responseResult = await responseMsg.Content.ReadAsAsync<Dictionary<string, string>>() ?? new Dictionary<string, string>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            return responseResult;
        }

        private string GetPosId()
        {
            return "CM271KOONA-CM271MegaMall-POS01";
        }

        private string GetTxId(DateTime dateTime)
        {
            return $"{dateTime.ToBinary()}";
        }

        private string GetTxId(string dateTime)
        {
            _ = DateTime.TryParse(dateTime, out DateTime _dateTime);
            return GetTxId(_dateTime);
        }

        public async Task SendSaleInvoicesAsync()
        {
            var receipts = _context.Receipt
                .Where(r => !r.Delete && !_context.TransactionChipMongs.AsNoTracking()
                    .Any(t => t.TxId == GetTxId(r.DateOut))
                );
            var sales = new List<SaleInvoice>();
            if (!receipts.Any()) { return; }
            sales = await QuerySaleInvoices(receipts).ToListAsync();
            await AddTransactionAsync(sales);
        }

        private async Task AddTransactionAsync(List<SaleInvoice> sales)
        {
            if (sales == null) { return; }
            foreach (var sale in sales)
            {
                var response = await PostTenantSaleAsync(sale);
                if (response.ContainsKey("result"))
                {
                    string txId = GetTxId(sale.date);
                    bool isExisting = _context.TransactionChipMongs.AsNoTracking().Any(t => t.TxId == txId);
                    if (!isExisting)
                    {
                        var data = new TransactionChipMong
                        {
                            PosId = sale.posId,
                            TxId = txId,
                            ChangeLog = DateTimeOffset.UtcNow,
                            RowId = Guid.NewGuid(),
                            SyncDate = DateTime.Now
                        };
                        _context.TransactionChipMongs.Add(data);
                        await _context.SaveChangesAsync();
                    }
                }
            }
        }

        public IQueryable<SaleInvoice> QuerySaleInvoices(IQueryable<Receipt> receipts)
        {
            var invoicesByReceiptId = from r in receipts
                           join mp in _context.MultiPaymentMeans on r.ReceiptID equals mp.ReceiptID
                           join p in _context.PaymentMeans.Where(p => !p.Delete) on mp.PaymentMeanID equals p.ID
                           group new {r, p, mp } by new {r.ReceiptID, r} into g                           
                           select new {
                                g.Key.r,
                                CreditCard =  g.Where(x => x.p.PaymentMethod == PaymentMethod.CreditCard)
                                                .Sum(x => x.mp.Amount),
                                Cash = g.Where(x => x.p.PaymentMethod == PaymentMethod.Cash)
                                            .Sum(x => x.mp.Amount),
                                BankWire = g.Where(x => x.p.PaymentMethod == PaymentMethod.BankTransfer)
                                            .Sum(x => x.mp.Amount),
                                GrossSale = Convert.ToDecimal(g.Max(x => x.r.GrandTotal)),
                                TaxAmount = Convert.ToDecimal(g.Max(x => x.r.TaxValue)),
                                NetSale = Convert.ToDecimal(g.Max(x => x.r.Sub_Total)),
                                CreditCardCount = g.Where(x => x.p.PaymentMethod == PaymentMethod.CreditCard).Count()
                            };
            
            var invoicesByDate = from i in invoicesByReceiptId group i by i.r.DateOut into g
                            select new {
                                g.Key,
                                CreditCard = g.Sum(x => x.CreditCard),
                                Cash = g.Sum(x => x.Cash),
                                BankWire = g.Sum(x => x.BankWire),
                                GrossSale = g.Sum(x => x.GrossSale),
                                TaxAmount = g.Sum(x => x.TaxAmount),
                                NetSale = g.Sum(x => x.NetSale),
                                TransactionCount = g.Count(),
                                ExchangeRate = g.Max(x => x.r.LocalSetRate),
                                TotalCreditCardCount = g.Sum(x => x.CreditCardCount)
                            };
            var _invoices = from x in invoicesByDate
                select new SaleInvoice
                {
                    tenantName = "CM271KOONA",
                    mallName = "CM271MegaMall",                 
                    date = x.Key.ToString("yyyy-MM-dd"),
                    grossSale = Convert.ToDecimal(x.GrossSale.ToString("F2")),
                    taxAmount = Convert.ToDecimal(x.TaxAmount.ToString("F2")),
                    netSale = Convert.ToDecimal(x.NetSale.ToString("F2")),
                    cashAmount = Convert.ToDecimal(x.Cash.ToString("F2")),
                    cashAmountUsd = Convert.ToDecimal(x.Cash.ToString("F2")),
                    cashAmountRiel = 0,
                    creditCardAmount = Convert.ToDecimal(x.CreditCard.ToString("F2")),
                    otherAmount = Convert.ToDecimal(x.BankWire.ToString("F2")),
                    totalCreditCardTransaction = x.TotalCreditCardCount,
                    totalTransaction = x.TransactionCount,
                    depositAmountUsd = 0,
                    depositAmountRiel = 0,
                    exchangeRate = Convert.ToDecimal(x.ExchangeRate),
                    posId = GetPosId()
                };
            return _invoices.AsNoTracking();
        }
        
        public async Task<List<SaleInvoice>> GetReportSalesAsync(DateTime dateFrom, DateTime dateTo)
        {
            List<SaleInvoice> sales = new List<SaleInvoice>();
            IQueryable<Receipt> receipts = _context.Receipt.AsNoTracking().Where(r => !r.Delete && dateFrom <= r.DateOut && r.DateOut <= dateTo);
            receipts = from r in receipts
                       join t in _context.TransactionChipMongs.AsNoTracking()
                    on GetTxId(r.DateOut) equals t.TxId
                       select r;
            if (!receipts.Any()) { return sales; }
            sales = await QuerySaleInvoices(receipts).ToListAsync();
            sales = sales.Select(s =>
            {
                s.date = Convert.ToDateTime(s.date).ToString("dd-MM-yyyy");
                return s;
            }).ToList();
            return sales;
        }
    }
}