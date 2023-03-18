using CKBS.AppContext;
using CKBS.Models.Services.Administrator.General;
using CKBS.Models.Services.Administrator.Tables;
using CKBS.Models.Services.POS;
using CKBS.Models.Services.POS.service;
using CKBS.Models.Services.ReportSale.dev;
using CKBS.Models.SignalR;
using KEDI.Core.Premise.DependencyInjection;
using KEDI.Core.Premise.Models.Services.POS.Templates;
using KEDI.Core.Premise.Models.ServicesClass.PrintBarcode;
using KEDI.Core.Premise.Repository;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Services
{
    public interface IPosClientSignal : IHostedService
    {
        Task PrintOrderAsync(List<PrintOrder> items, List<PrinterName> printers, List<GeneralSetting> setting);
        Task PrintBillAsync(List<PrintBill> list, List<GeneralSetting> setting, string ip);
        Task PrintCashoutAsync(List<CashoutReportMaster> cashouts, List<VoidItemReport> voidItems, List<GeneralSetting> settings);
        Task PrintCashoutPaymentMeanSummaryAsync(List<CashoutReportMaster> cashouts, List<GeneralSetting> settings);
        Task PrintCategoryAndPaymentAsync(List<CashoutReportMaster> cashoutReports, List<GeneralSetting> settings);
        Task PrintCashoutSummaryAndPaymentSummaryAsync(List<CashoutReportMaster> cashoutReports, List<GeneralSetting> settings);
        Task PrintCashoutPaymentMeansAsync(List<CashoutReportMaster> cashouts, List<GeneralSetting> settings);
        Task PrintCashoutSummaryAsync(List<CashoutReportMaster> cashouts, List<GeneralSetting> settings);
        Task PrintBarcodeItemsAsync(List<ItemPrintBarcodeView> items, string printerName);
        Task<Table> UpdateTimeTableAsync(int tableId, char status, DateTimeOffset startDT);
        Task<Table> TransferTableAsync(int srcTableId, int desTableId);
        Task ChangeStatusTimeTablesAsync(params Table[] timeTables);     
    }

    public partial class PosClientSignal : BackgroundService, IPosClientSignal 
    {
        private readonly ILogger<PosClientSignal> _logger;
        private readonly IHubContext<SignalRClient> _hubContext;
        private readonly DataContext _dataContext;
        private readonly UserManager _userModule;
        public PosClientSignal(ILogger<PosClientSignal> logger,
            DataContext dataContext,
            UserManager userModule,
            IHubContext<SignalRClient> hubContext,
            IServiceScopeProvider provider, IHostApplicationLifetime halt)
        {
            _logger = logger;
            _hubContext = hubContext;
            _dataContext = dataContext;
            _userModule = userModule;
        }

        private IClientProxy _clients => _hubContext.Clients.All;
        private static Dictionary<int, Table> _timeTables = new();
        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try {          
                await Task.Delay(5000, stoppingToken);   
                await OnTickSendAsync(stoppingToken);
            } catch(Exception ex){
                _logger.LogError(ex.Message);               
                await ExecuteAsync(CancellationToken.None);
            }    
        }

        private async Task OnTickSendAsync(CancellationToken stoppingToken)
        {
            var tables = await _dataContext.Tables.AsNoTracking()
                .Where(t => !t.Delete && t.Status != 'A').ToListAsync(CancellationToken.None);
            _timeTables = tables.ToDictionary(k => k.ID);
            while (!stoppingToken.IsCancellationRequested)
            {   
                await _clients.SendAsync("tick", _timeTables.Values, CancellationToken.None);
                _timeTables = _timeTables.Where(t => t.Value.Status == 'B').ToDictionary(t => t.Key, t => t.Value);  
                await Task.Delay(TimeSpan.FromSeconds(1), CancellationToken.None);     
            }
        }
    }

    public partial class PosClientSignal : BackgroundService, IPosClientSignal
    {    
        public async Task<Table> TransferTableAsync(int srcTableId, int desTableId)
        {
            var srcTable = await _dataContext.Tables.FindAsync(srcTableId);
            var desTable = await _dataContext.Tables.FindAsync(desTableId);
            if (srcTable != null && desTable != null)
            {
                desTable.Status = srcTable.Status;
                desTable.Time = srcTable.Time;
                desTable.StartDateTime = srcTable.StartDateTime;
                srcTable.StartDateTime = default;
                srcTable.Status = 'A';
                srcTable.Time = "00:00:00";
                await _dataContext.SaveChangesAsync();
                _timeTables.Remove(srcTable.ID);
                _timeTables.TryAdd(srcTable.ID, srcTable);
                _timeTables.Remove(desTable.ID);
                _timeTables.TryAdd(desTable.ID, desTable);
            }
            return desTable;
        }

        public async Task<Table> UpdateTimeTableAsync(int tableId, char status, DateTimeOffset startDT)
        {
            var table = await _dataContext.Tables.FindAsync(tableId);
            if (table == null) { throw new NullReferenceException($"Table[{tableId}] not found."); }

            switch (status)
            {
                case 'A':
                    table.StartDateTime = default;
                    table.EndDateTime = default;
                    break;
                case 'B':
                    if (table.StartDateTime == default)
                    {
                        table.StartDateTime = startDT;
                    }
                    break;
                case 'P':
                    if (table.StartDateTime == default)
                    {
                        table.StartDateTime = DateTimeOffset.Now;
                    }
                    table.EndDateTime = DateTimeOffset.Now;
                    break;
            }

            table.Status = status;
            await _dataContext.SaveChangesAsync();
            _timeTables.Remove(table.ID);
            _timeTables.TryAdd(table.ID, table);
            return table;
        }

        private static DateTime ToDateTime(string time = "00:00:00")
        {
            DateTime.TryParseExact(time, "HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime _dateTime);
            return _dateTime;
        }

        private static string ValidTime(string time)
        {
            return ToDateTime(time).ToString("HH:mm:ss", DateTimeFormatInfo.InvariantInfo);
        }

        public async Task ChangeStatusTimeTablesAsync(params Table[] timeTables)
        {
            await _clients.SendAsync("changeStatusTimeTables", timeTables);
        }

        // Print section
        public async Task PrintOrderAsync(List<PrintOrder> items, List<PrinterName> printers, List<GeneralSetting> setting)
        {
            await _hubContext.Clients.All.SendAsync("PrintItemOrder", items, printers, setting);
        }

        public async Task PrintBillAsync(List<PrintBill> list, List<GeneralSetting> setting, string ip)
        {
            if (list.Count > 0)
            {
                await _hubContext.Clients.All.SendAsync("PrintItemBill", list, setting, ip);
                await _hubContext.Clients.All.SendAsync("PushStatusBill", list[0].OrderID);
            }
        }

        public async Task PrintCashoutAsync(List<CashoutReportMaster> cashouts, List<VoidItemReport> voidItems, List<GeneralSetting> settings)
        {
            await _clients.SendAsync("PrintCashout", cashouts, settings);
            await _clients.SendAsync("PrintVoidItem", voidItems, settings);
        }

        public async Task PrintCashoutPaymentMeanSummaryAsync(List<CashoutReportMaster> cashouts, List<GeneralSetting> settings)
        {
            await _clients.SendAsync("PrintCashoutPaymentMeanSummary", cashouts, settings);
        }

        public async Task PrintCategoryAndPaymentAsync(List<CashoutReportMaster> cashoutReports, List<GeneralSetting> settings)
        {
            await _clients.SendAsync("PrintCashoutandPayment", cashoutReports, settings);
        }

        public async Task PrintCashoutSummaryAndPaymentSummaryAsync(List<CashoutReportMaster> cashoutReports, List<GeneralSetting> settings)
        {
            await _clients.SendAsync("PrintCashoutSummaryAndPaymentSummary", cashoutReports, settings);
        }

        public async Task PrintCashoutPaymentMeansAsync(List<CashoutReportMaster> cashouts, List<GeneralSetting> settings)
        {
            await _clients.SendAsync("PrintCashoutPaymentMeans", cashouts, settings);
        }

        public async Task PrintCashoutSummaryAsync(List<CashoutReportMaster> cashouts, List<GeneralSetting> settings)
        {
            await _clients.SendAsync("PrintCashoutSummary", cashouts, settings);
        }

        public async Task PrintBarcodeItemsAsync(List<ItemPrintBarcodeView> items, string printerName)
        {
            await _clients.SendAsync("PrintBarcodeItems", items, printerName);
        }

    }
}
