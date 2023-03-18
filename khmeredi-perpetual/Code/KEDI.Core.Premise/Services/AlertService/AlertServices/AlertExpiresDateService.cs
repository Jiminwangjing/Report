using CKBS.AlertManagementsServices.Repositories;
using CKBS.Models.Services.AlertManagement;
using KEDI.Core.Premise.Repository;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static KEDI.Core.Premise.Services.AlertService.Repositories.AlertExpiresDateServiceRepo;

namespace KEDI.Core.Premise.Services.AlertService.AlertServices
{
    public class AlertExpiresDateService : BackgroundService
    {
        private readonly UserManager _userModule;
        private readonly ICheckFrequently _checkFrequently;
        private readonly IAlertExpiresDateServiceRepo _alertarservice;
        private readonly ILogger<AlertExpiresDateService> _logger;

        public AlertExpiresDateService(
            UserManager userModule,
            ICheckFrequently checkFrequently,
            IAlertExpiresDateServiceRepo alertARService,
            ILogger<AlertExpiresDateService> logger)
        {
            _userModule = userModule;
            _checkFrequently = checkFrequently;
            _alertarservice = alertARService;
            _logger = logger;
        }

        public int Interval { get; set; }
        private async Task NotifyDueDateAsync()
        {
            var data = await _checkFrequently.GetGeneralServiceSetup("ARServiceContract");
            if (data?.Active == true)
            {
                var activeUsers = _userModule.GetLoggedUsers();
                _logger.LogInformation("Alert Service Contract is running ...");
                Interval = Convert.ToInt32(await _checkFrequently.CheckFrequentAsync(TypeOfAlert.ARServiceContract));
                await _alertarservice.AlertBeforeDateAsync(activeUsers.Keys.ToList());
            }
            else
            {
                Interval = _checkFrequently.Interval;
            }

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //await Task.Delay(60000, stoppingToken);
            while (!stoppingToken.IsCancellationRequested)
            {
                await NotifyDueDateAsync();
                await Task.Delay(Interval, stoppingToken);
            }
        }
    }
}
