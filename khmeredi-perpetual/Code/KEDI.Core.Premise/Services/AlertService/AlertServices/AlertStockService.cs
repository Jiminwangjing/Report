using CKBS.AlertManagementsServices.Repositories;
using CKBS.AppContext;
using CKBS.Models.Services.AlertManagement;
using KEDI.Core.Premise.DependencyInjection;
using KEDI.Core.Premise.Models.Services.GeneralSettingAdmin;
using KEDI.Core.Premise.Repository;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CKBS.AlertManagementsServices.AlertManagementServices
{
    public class AlertStockService : BackgroundService
    {
        private readonly IAlertStockRepo _alertStock;
        private readonly UserManager _userModule;
        private readonly ICheckFrequently _checkFrequently;
        private readonly ILogger<AlertStockService> _logger;

        public AlertStockService(
            IServiceScopeProvider provider,
            ILogger<AlertStockService> logger)
        {
            _userModule = provider.GetService<UserManager>();
            _alertStock = provider.GetService<IAlertStockRepo>();
            _checkFrequently = provider.GetService<ICheckFrequently>();
            _logger = logger;
        }

        public int Interval { get; set; }
        private async Task NotifyStockAsync()
        {
            var data = await _checkFrequently.GetGeneralServiceSetup("Stock");
            if (data?.Active == true)
            {
                var interval = await _checkFrequently.CheckFrequentAsync(TypeOfAlert.Stock);
                Interval = interval;
                var userActives = _userModule.GetLoggedUsers();

                _logger.LogInformation("Alert Stock is running.....");

                await _alertStock.CheckStockAsync(userActives.Keys.ToList());
            }
            else
            {
                Interval = _checkFrequently.Interval;
            }
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //await Task.Delay(60000);
            await Task.Delay(60000, stoppingToken);
            while (!stoppingToken.IsCancellationRequested)
            {
                await NotifyStockAsync();

                await Task.Delay(Interval, stoppingToken);
            }
        }

    }
}
