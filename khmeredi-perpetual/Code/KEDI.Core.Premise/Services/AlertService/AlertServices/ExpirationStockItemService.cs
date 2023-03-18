using CKBS.AlertManagementsServices.Repositories;
using CKBS.AppContext;
using CKBS.Models.Services.AlertManagement;
using KEDI.Core.Premise.DependencyInjection;
using KEDI.Core.Premise.Models.Services.GeneralSettingAdmin;
using KEDI.Core.Premise.Repository;
using KEDI.Core.Premise.Services.AlertService.Repositories;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Services.AlertService.AlertServices
{
    public class ExpirationStockItemService : BackgroundService
    {
        private readonly IExpirationStockItemRepo _alertExpireStockItem;
        private readonly UserManager _userModule;
        private readonly ICheckFrequently _checkFrequently;
        private readonly ILogger<ExpirationStockItemService> _logger;

        public ExpirationStockItemService(
            IServiceScopeProvider provider,
            ILogger<ExpirationStockItemService> logger)
        {
            _alertExpireStockItem = provider.GetService<IExpirationStockItemRepo>();
            _userModule = provider.GetService<UserManager>();
            _checkFrequently = provider.GetService<ICheckFrequently>();
            _logger = logger;
        }

        public int Interval { get; set; }
        private async Task NotifyExpirationStockItemAsync()
        {
            var data = await _checkFrequently.GetGeneralServiceSetup("ExItem");
            if (data?.Active == true)
            {
                var interval = await _checkFrequently.CheckFrequentAsync(TypeOfAlert.ExpireItem);
                Interval = interval;
                _logger.LogInformation("Alert Expiration Stock Item is running ...");
                var userActives = _userModule.GetLoggedUsers();
                await _alertExpireStockItem.CheckStockAsync(userActives.Keys.ToList());
            }
            else
            {
                Interval = _checkFrequently.Interval;
            }
            
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(60000, stoppingToken);
            while (!stoppingToken.IsCancellationRequested)
            {
                await NotifyExpirationStockItemAsync();
                await Task.Delay(Interval, stoppingToken);
            }
        }
    }
}
