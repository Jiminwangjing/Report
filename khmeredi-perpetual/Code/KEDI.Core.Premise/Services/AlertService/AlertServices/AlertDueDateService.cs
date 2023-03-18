using CKBS.AlertManagementsServices.Repositories;
using CKBS.AppContext;
using CKBS.Models.Services.AlertManagement;
using KEDI.Core.Models.Validation;
using KEDI.Core.Premise.DependencyInjection;
using KEDI.Core.Premise.Models.Services.GeneralSettingAdmin;
using KEDI.Core.Premise.Repository;
using KEDI.Core.Security;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CKBS.AlertManagementsServices.AlertManagementServices
{
    public class AlertDueDateService : BackgroundService
    {
        private readonly UserManager _userModule;
        private readonly ICheckFrequently _checkFrequently;
        private readonly IAlertDueDateRepo _alertDueDate;
        private readonly ILogger<AlertDueDateService> _logger;

        public AlertDueDateService(
            IServiceScopeProvider provider,
            // ICheckFrequently checkFrequently,
            // IAlertDueDateRepo alertDueDate,
            ILogger<AlertDueDateService> logger)
        {
            _userModule = provider.GetService<UserManager>();
            _checkFrequently = provider.GetService<ICheckFrequently>();
            _alertDueDate = provider.GetService<IAlertDueDateRepo>();
            _logger = logger;
        }

        public int Interval { get; set; }
        private async Task NotifyDueDateAsync()
        {
            var data = await _checkFrequently.GetGeneralServiceSetup("DueDate");
            if (data?.Active == true)
            {
                var activeUsers = _userModule.GetLoggedUsers();
                _logger.LogInformation("Alert Due date is running ...");
                Interval = Convert.ToInt32(await _checkFrequently.CheckFrequentAsync(TypeOfAlert.DueDate));
                await _alertDueDate.AlertBeforeDateAsync(activeUsers.Keys.ToList());
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
                await NotifyDueDateAsync();
                await Task.Delay(Interval, stoppingToken);
            }
        }
    }
}
