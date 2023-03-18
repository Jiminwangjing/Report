using KEDI.Core.Premise.Models.ClientApi;
using KEDI.Core.Premise.Models.Sync;
using KEDI.Core.Premise.Utilities;
using KEDI.Core.Utilities;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Repositories.Sync
{
    public interface ISyncClientWorker : IServiceWorker { }
    public partial class SyncClientWorker : ServiceWorker, ISyncClientWorker
    {
        private readonly ILogger<SyncClientWorker> _logger;
        private readonly IClientEntityFilter _clientFilter;
        private readonly ISyncSender _syncSender;
        private readonly ISyncAdapter _syncAdapter;
        private readonly HostSetting _settings;
        public SyncClientWorker(ILogger<SyncClientWorker> logger,
            IClientEntityFilter clientFilter,
            ISyncSender clientPos,        
            ISyncAdapter syncPosRepo,
            IDictionary<string, HostSetting> serverHosts
        )
        {
            _logger = logger;
            _clientFilter = clientFilter;
            _syncSender = clientPos;
            _syncAdapter = syncPosRepo;
            serverHosts.TryGetValue("ExternalSync", out HostSetting settings);
            _settings = settings;
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
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                await ExecuteAsync(stoppingToken);
            }
           
        }
        protected override async Task WorkAsync(CancellationToken stoppingToken)
        {
            await PullDataAsync();
            await PushDataAsync();
        }
    }

    public partial class SyncClientWorker : ServiceWorker, ISyncClientWorker
    {
        public async Task PullDataAsync()
        {
            var modelState = new ModelStateDictionary();
            var container = await _syncSender.PullEntityContainerAsync();
            PropertyInfo[] props = container.GetType().GetProperties();          
            if(props.Length <= 0) { return; }
            var syncBackContainer = new Dictionary<Type, IEnumerable<ISyncEntity>>();
            foreach (var prop in props) {
                var entryMaps = (IEnumerable<object>)EntityHelper.GetProperty(container, prop.Name);  
                if(entryMaps == null) { continue; }
                if (!entryMaps.Any()) { continue; }

                var entities = new List<ISyncEntity>();
                Type entityType = null;
                foreach (var entryMap in  entryMaps)
                {
                    string nameofEntity = nameof(EntryMap<ISyncEntity>.Entity);
                    string nameofReferences = nameof(EntryMap<ISyncEntity>.References);
                    string nameofIsModified = nameof(EntryMap<ISyncEntity>.IsModified);
                    var entity = EntityHelper.GetProperty(entryMap, nameofEntity);
                    entityType = entity.GetType();
                    if (entity == null) { continue; }
                    var references = EntityHelper.GetProperty(entryMap, nameofReferences);
                    await _syncAdapter.UpdateEntityAsync(
                       modelState, entity, (IEnumerable<SyncEntity>)references
                    );

                    if(modelState.IsValid)
                    {
                        string modified = EntityHelper.GetProperty(entryMap, nameofIsModified).ToString();
                        _ = bool.TryParse(modified, out bool _modified);
                        if (_modified) { entities.Add((ISyncEntity)entity); }
                    }                
                }

                if(entityType == null) { continue; }
                syncBackContainer.TryAdd(entityType, entities);
            }

            if (syncBackContainer.Any())
            {
                await _syncSender.PushBackEntityContainerAsync(syncBackContainer);
            }
        }

        public async Task PushDataAsync()
        {
            await PushRangeReceiptAsync();
            await PushRangeReceiptMemoAsync();
            await PushRangeVoidItemAsync();
            await PushRangeVoidOrderAsync();
            await PushRangeOpenShiftAsync();
            await PushRangeCloseShiftAsync();
        }

        public bool AllowChanges<TEntity>(EntryMap<TEntity> entry)
            where TEntity : class, ISyncEntity
        {
            return (entry.IsValid && entry.IsModified) || entry.IsDuplicate;
        }

        public async Task PushRangeReceiptAsync()
        {
            var receiptContainers = await _clientFilter.GetReceiptContainersAsync();     
            if(!receiptContainers.Any()) { return; }
            var _receiptContainers = await _syncSender.PushRangeReceiptAsync(receiptContainers);
            var receipts = _receiptContainers
                        .Where(c => AllowChanges(c.Receipt))
                        .Select(c => c.Receipt.Entity);
            await _clientFilter.UpdateRangeHistoryAsync(receipts);
        }

        public async Task PushRangeReceiptMemoAsync()
        {
            var receiptMemoContainers = await _clientFilter.GetReceiptMemoContainersAsync();
            if (!receiptMemoContainers.Any()) { return; }
            var _clientReceipts = await _syncSender.PushRangeReceiptMemoAsync(receiptMemoContainers);
            var receiptMemos = _clientReceipts
                            .Where(c => AllowChanges(c.ReceiptMemo))
                            .Select(c => c.ReceiptMemo.Entity);
            await _clientFilter.UpdateRangeHistoryAsync(receiptMemos);
        }

        public async Task PushRangeVoidOrderAsync()
        {
            var voidOrderContainers = await _clientFilter.GetVoidOrderContainersAsync();
            if (voidOrderContainers.Any())
            {
                var _voidItemContainers = await _syncSender.PushRangeVoidOrderAsync(voidOrderContainers);
                var voidOrders = _voidItemContainers
                                .Where(c => AllowChanges(c.VoidOrder))
                                .Select(c => c.VoidOrder.Entity);
                await _clientFilter.UpdateRangeHistoryAsync(voidOrders);
            }
        }

        public async Task PushRangeVoidItemAsync()
        {
            var voidItemContainers = await _clientFilter.GetVoidItemContainersAsync();
            if (!voidItemContainers.Any()) { return; }
            var _voidItemContainers = await _syncSender.PushRangeVoidItemAsync(voidItemContainers);
            var voidItems = _voidItemContainers
                        .Where(c => AllowChanges(c.VoidItem))
                        .Select(c => c.VoidItem.Entity);
            await _clientFilter.UpdateRangeHistoryAsync(voidItems);
        }

        public async Task PushRangeOpenShiftAsync()
        {
            var openShiftEntries = await _clientFilter.GetOpenShiftsAsync();
            if (openShiftEntries.Any())
            {
                var _openShifts = await _syncSender.PushRangeOpenShiftAsync(openShiftEntries);
                var openShifs = _openShifts
                            .Where(c => AllowChanges(c))
                            .Select(rc => rc.Entity);
                await _clientFilter.UpdateRangeHistoryAsync(openShifs);
            }
        }

        public async Task PushRangeCloseShiftAsync()
        {
            var closeShiftEntries = await _clientFilter.GetCloseShiftsAsync();
            if (closeShiftEntries.Any())
            {
                var _closeShiftEntries = await _syncSender.PushRangeCloseShiftAsync(closeShiftEntries);
                var closeShifts = _closeShiftEntries
                                .Where(c => AllowChanges(c))
                                .Select(rc => rc.Entity);
                await _clientFilter.UpdateRangeHistoryAsync(closeShifts);
            }
        }

    }
}
