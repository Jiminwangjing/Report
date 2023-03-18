using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KEDI.Core.Premise.Models.Sync;
using Microsoft.Extensions.Hosting;
using System.Threading;
using KEDI.Core.Premise.Models.ClientApi;
using KEDI.Core.Premise.Utilities;
using KEDI.Core.Premise.Models.Sync.Customs.Server;

namespace KEDI.Core.Premise.Repositories.Sync
{
    public interface ISyncServerWorker : IHostedService
    {
        Task<ServerSyncContainer> GetServerSyncContainerAsync();
        Task UpdateRangeEntityHistoryAsync(Dictionary<Type, IEnumerable<SyncEntity>> entities);
    }

    public partial class SyncServerWorker : ServiceWorker, ISyncServerWorker
    {
        private readonly ILogger<SyncServerWorker> _logger;
        private readonly IServerEntityFilter _serverFilter;
        private readonly IPosSyncRepo _posSync;
        private readonly HostSetting _settings;
        public SyncServerWorker(ILogger<SyncServerWorker> logger,
            IServerEntityFilter serverFilter,    
            IPosSyncRepo posSync,
            IDictionary<string, HostSetting> hostSettings
        )
        {
            _logger = logger;
            _posSync = posSync;
            _serverFilter = serverFilter;
            hostSettings.TryGetValue("InternalSync", out _settings);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                if (!_settings.Enabled) { return; }                
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
            } catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                await ExecuteAsync(stoppingToken);
            }          
        }

        protected override async Task WorkAsync(CancellationToken stoppingToken)
        {
            await _posSync.IssueStockInternalAsync();
            await _posSync.IssueStockReturnedInternalAsync();
        }
    }

    public partial class SyncServerWorker : ServiceWorker, ISyncServerWorker
    {
        public async Task<ServerSyncContainer> GetServerSyncContainerAsync()
        {
            return await _serverFilter.GetServerSyncContainerAsync();
        }

        public async Task UpdateRangeEntityHistoryAsync(Dictionary<Type, IEnumerable<SyncEntity>> entities)
        {
            foreach (var entity in entities)
            {
                await _serverFilter.UpdateRangeHistoryAsync(entity.Key, entity.Value);
            }     
        }

    }
}
