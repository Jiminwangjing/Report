using CKBS.AppContext;
using CKBS.Models.Services.POS;
using CKBS.Models.Services.POS.KVMS;
using KEDI.Core.Premise.AppContext;
using KEDI.Core.Premise.AppContext.Sync.Customs;
using KEDI.Core.Premise.Models.Services.POS;
using KEDI.Core.Premise.Models.Sync;
using KEDI.Core.Premise.Models.Sync.Customs.Clients;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Repositories.Sync
{
    public interface IClientEntityFilter
    {
        Task<IEnumerable<ReceiptContainer>> GetReceiptContainersAsync();
        Task<IEnumerable<ReceiptMemoContainer>> GetReceiptMemoContainersAsync();
        Task<IEnumerable<VoidItemContainer>> GetVoidItemContainersAsync();
        Task<IEnumerable<VoidOrderContainer>> GetVoidOrderContainersAsync();
        Task<IEnumerable<EntryMap<OpenShift>>> GetOpenShiftsAsync();
        Task<IEnumerable<EntryMap<CloseShift>>> GetCloseShiftsAsync();
        Task UpdateRangeHistoryAsync<TEntity>(
            IEnumerable<TEntity> entities
        ) where TEntity : class, ISyncEntity;
        Task<IEnumerable<TEntity>> GetChangedEntitiesAsync<TEntity>(
          int rowSize = 100
        ) where TEntity : class, ISyncEntity;
    }

    public partial class ClientEntityFilter : CustomEntityContext, IClientEntityFilter
    {
        public async Task<IEnumerable<ReceiptContainer>> GetReceiptContainersAsync()
        {
            var receipts = await GetChangedEntitiesAsync<Receipt>();
            var receiptContainers = new List<ReceiptContainer>();
            foreach (var r in receipts)
            {
                var receiptDetails = _dataContext.ReceiptDetail.Where(rd => rd.ReceiptID == r.ReceiptID);
                var rdEntries = await MapRangeReferencesAsync(receiptDetails, ent => MapReferencesAsync(ent));

                var multiPMs = _dataContext.MultiPaymentMeans.Where(mp => mp.ReceiptID == r.ReceiptID);
                var mpEntries = await MapRangeReferencesAsync(multiPMs, ent => MapReferencesAsync(ent));
    
                var freightReceipts = _dataContext.FreightReceipts.Where(mp => mp.ReceiptID == r.ReceiptID);
                var frEntries = await MapRangeReferencesAsync(freightReceipts, ent => MapReferencesAsync(ent));

                var rc = new ReceiptContainer
                {
                    Receipt = await MapReferencesAsync(r),
                    ReceiptDetails = rdEntries,
                    MultiPaymentMeans = mpEntries,
                    FreightReceipts = frEntries
                };
                receiptContainers.Add(rc);
            }

            return receiptContainers;
        }

        public async Task<IEnumerable<ReceiptMemoContainer>> GetReceiptMemoContainersAsync()
        {
            var receiptMemos = await GetChangedEntitiesAsync<ReceiptMemo>();
            var receiptContainers = new List<ReceiptMemoContainer>();
            foreach (var r in receiptMemos)
            {
                var receiptDetailMemos = _dataContext.ReceiptDetailMemoKvms.Where(rd => rd.ReceiptMemoID == r.ID);
                var rdEntries = await MapRangeReferencesAsync(receiptDetailMemos, ent => MapReferencesAsync(ent));
                
                var rc = new ReceiptMemoContainer
                {
                    ReceiptMemo = await MapReferencesAsync(r),
                    ReceiptDetailMemos = rdEntries
                };
                receiptContainers.Add(rc);
            }

            return receiptContainers;
        }

        public async Task<IEnumerable<VoidItemContainer>> GetVoidItemContainersAsync()
        {
            var voidItems = await GetChangedEntitiesAsync<VoidItem>();
            var voidItemContainers = new List<VoidItemContainer>();
            foreach (var vi in voidItems)
            {
                var voidItemDetails = _dataContext.VoidItemDetails.Where(rd => rd.VoidItemID == vi.ID);
                var viEntries = await MapRangeReferencesAsync(voidItemDetails, ent => MapReferencesAsync(ent));
                var vic = new VoidItemContainer
                {
                    VoidItem = await MapReferencesAsync(vi),
                    VoidItemDetails = viEntries
                };
                voidItemContainers.Add(vic);
            }

            return voidItemContainers;
        }

        public async Task<IEnumerable<VoidOrderContainer>> GetVoidOrderContainersAsync()
        {
            var voidOrders = await GetChangedEntitiesAsync<VoidOrder>();
            var voContainers = new List<VoidOrderContainer>();
            foreach (var r in voidOrders)
            {
                var voidOrderDetails = _dataContext.VoidOrderDetails.Where(rd => rd.OrderID == r.OrderID);
                var vodEntries = await MapRangeReferencesAsync(voidOrderDetails, ent => MapReferencesAsync(ent));

                var rc = new VoidOrderContainer
                {
                    VoidOrder = await MapReferencesAsync(r),
                    VoidOrderDetails = vodEntries
                };
                voContainers.Add(rc);
            }

            return voContainers;
        }

        public async Task<IEnumerable<EntryMap<OpenShift>>> GetOpenShiftsAsync()
        {
            var entities = await GetChangedEntitiesAsync<OpenShift>();
            var openShifts = await MapRangeReferencesAsync(entities, ent => MapReferencesAsync(ent));
            return openShifts;
        }

        public async Task<IEnumerable<EntryMap<CloseShift>>> GetCloseShiftsAsync()
        {
            var entities = await GetChangedEntitiesAsync<CloseShift>();
            var closeShifts = await MapRangeReferencesAsync(entities, ent => MapReferencesAsync(ent));
            return closeShifts;
        }
    }

    public partial class ClientEntityFilter : CustomEntityContext, IClientEntityFilter
    {
        private readonly ILogger<ClientEntityFilter> _logger;
        private readonly DataContext _dataContext;
        private readonly IQueryContext _query;
        public ClientEntityFilter(ILogger<ClientEntityFilter> logger,
            DataContext dataContext,
            IQueryContext query)
        : base(logger, query)
        {
            _logger = logger;
            _dataContext = dataContext;
            _query = query;
        }

        public IQueryable<TEntity> FilterChangedEntities<TEntity>(DataContext dc)
           where TEntity : class, ISyncEntity
        {
            IQueryable<ClientSyncHistory> syncHistories = dc.Set<ClientSyncHistory>().AsNoTracking();
            IQueryable<TEntity> entities = FilterChangedEntities<TEntity, ClientSyncHistory>(dc, syncHistories);
            return entities;
        }

        public async Task<IEnumerable<TEntity>> GetChangedEntitiesAsync<TEntity>(
          int rowSize = 100
        ) where TEntity : class, ISyncEntity
        {
            using var db = new DataContext();
            var entities = FilterChangedEntities<TEntity>(db);
            return await entities.Take(rowSize).ToListAsync();
        }

        public async Task UpdateRangeHistoryAsync<TEntity>(
            IEnumerable<TEntity> entities
        ) where TEntity : class, ISyncEntity
        {
            foreach (var ent in entities)
            {
                var syncHistory = await FindHistoryByRowIdAsync<ClientSyncHistory>(ent.RowId, true);
                if (syncHistory == null)
                {
                    syncHistory = new ClientSyncHistory
                    {
                        RowId = ent.RowId,
                        ChangeLog = DateTimeOffset.UtcNow,
                        TableName = _query.GetTableName(typeof(TEntity)),
                    };
                    await _dataContext.AddAsync(syncHistory);
                }
                else
                {
                    syncHistory.ChangeLog = ent.ChangeLog;
                    _dataContext.Update(syncHistory);
                }
                await _dataContext.SaveChangesAsync();
            }
        }
    }
}
