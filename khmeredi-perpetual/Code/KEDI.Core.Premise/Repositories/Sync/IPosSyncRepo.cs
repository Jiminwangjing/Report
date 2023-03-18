using CKBS.AppContext;
using CKBS.Models.Services.Administrator.SystemInitialization;
using CKBS.Models.Services.POS;
using CKBS.Models.Services.Responsitory;
using KEDI.Core.Premise.AppContext;
using KEDI.Core.Premise.AppContext.Sync;
using KEDI.Core.Premise.Models.Services.POS.KSMS;
using KEDI.Core.Premise.Models.ServicesClass.SerialBatches.Sales;
using KEDI.Core.Premise.Models.Sync;
using KEDI.Core.Premise.Models.Sync.Customs.Clients;
using KEDI.Core.Repository;
using KEDI.Core.System.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Repositories.Sync
{
    public interface IPosSyncRepo
    {
        Task<ReceiptContainer> AddReceiptAsync(ModelStateDictionary modelState, ReceiptContainer container);
        Task<IEnumerable<ReceiptContainer>> AddRangeReceiptAsync(
            ModelStateDictionary modelState, IEnumerable<ReceiptContainer> containers
        );
        Task<ReceiptMemoContainer> AddReceiptMemoAsync(ModelStateDictionary modelState, ReceiptMemoContainer container);
        Task<IEnumerable<ReceiptMemoContainer>> AddRangeReceiptMemoAsync(
            ModelStateDictionary modelState, IEnumerable<ReceiptMemoContainer> containers
        );
        Task<VoidOrderContainer> AddVoidOrderAsync(ModelStateDictionary modelState, VoidOrderContainer container);
        Task<IEnumerable<VoidOrderContainer>> AddRangeVoidOrderAsync(
            ModelStateDictionary modelState, IEnumerable<VoidOrderContainer> containers
        );
        Task<VoidItemContainer> AddVoidItemAsync(ModelStateDictionary modelState, VoidItemContainer container);
        Task<IEnumerable<VoidItemContainer>> AddRangeVoidItemAsync(
            ModelStateDictionary modelState, IEnumerable<VoidItemContainer> containers
        );
        Task<IEnumerable<EntryMap<OpenShift>>> AddRangeOpenShiftAsync(
            ModelStateDictionary modelState, IEnumerable<EntryMap<OpenShift>> openShifts
        );
        Task<IEnumerable<EntryMap<CloseShift>>> AddRangeCloseShiftAsync(
           ModelStateDictionary modelState, IEnumerable<EntryMap<CloseShift>> closeShifts
        );

        Task IssueStockInternalAsync(IEnumerable<Receipt> srcReceipts = null);
        Task IssueStockReturnedInternalAsync();
    }

    public class PosSyncRepo : EntityContext, IPosSyncRepo
    {
        private readonly ILogger<PosSyncRepo> _logger;
        private readonly DataContext _dataContext;
        private readonly ISyncAdapter _syncAdapter;
        private readonly IPOS _pos;
        private readonly IClientApiRepo _clientApiRepo;
        public PosSyncRepo(ILogger<PosSyncRepo> logger,
            IQueryContext query,
            DataContext dataContext,
            IClientApiRepo clientApiRepo,
            ISyncAdapter syncAdapter, IPOS pos
        ) : base(logger, query){
            _logger = logger;
            _dataContext = dataContext;
            _syncAdapter = syncAdapter;
            _pos = pos;
            _clientApiRepo = clientApiRepo;
        }

        public async Task<IEnumerable<ReceiptContainer>> AddRangeReceiptAsync(
            ModelStateDictionary modelState, IEnumerable<ReceiptContainer> containers
        )
        {
            foreach(var container in containers)
            {
                await AddReceiptAsync(modelState, container);
            }
            return containers;
        }

        public async Task<ReceiptContainer> AddReceiptAsync(ModelStateDictionary modelState, ReceiptContainer container)
        {
            var client = _clientApiRepo.GetCurrentClient();
            if (Exist(container.Receipt.Entity)) {
                container.Receipt.IsDuplicate = true;
                _logger.LogInformation("Receipt already exists.");
                return container; 
            }
            await _syncAdapter.UpdateReferencesAsync(modelState, container.Receipt);
            await _syncAdapter.UpdateRangeReferencesAsync(modelState, container.ReceiptDetails);
            await _syncAdapter.UpdateRangeReferencesAsync(modelState, container.MultiPaymentMeans);
            await _syncAdapter.UpdateRangeReferencesAsync(modelState, container.FreightReceipts);

            if (modelState.IsValid)
            {
                using var dbt = await _dataContext.Database.BeginTransactionAsync();

                var receipt = container.Receipt.Entity;
                var userSetting = await _dataContext.GeneralSettings.AsNoTracking()
                                .FirstOrDefaultAsync(s => s.UserID == receipt.UserOrderID);
                var seriesDetail = await AddSeriesDetailAsync(userSetting);
                if (seriesDetail == null) { return container; }

                receipt.ReceiptID = 0;
                receipt.SeriesID = seriesDetail.SeriesID;
                receipt.SeriesDID = seriesDetail.ID;
                await _dataContext.Receipt.AddAsync(receipt);

                var receiptDetails = container.ReceiptDetails.Select(rd => rd.Entity);
                foreach(var rd  in receiptDetails){
                    rd.ID = 0;
                    rd.ReceiptID = receipt.ReceiptID;
                }

                var multiPayments = container.MultiPaymentMeans.Select(mp => mp.Entity);
                foreach(var mp in multiPayments){
                    mp.ID = 0;
                    mp.ReceiptID = receipt.ReceiptID;
                }
                
                var freightReceipts = container.FreightReceipts.Select(fr => fr.Entity);
                foreach(var fr in freightReceipts){
                     fr.ID = 0;
                    fr.ReceiptID = receipt.ReceiptID;
                }

                await _dataContext.ReceiptDetail.AddRangeAsync(receiptDetails);
                await _dataContext.MultiPaymentMeans.AddRangeAsync(multiPayments);
                await _dataContext.FreightReceipts.AddRangeAsync(freightReceipts);
                await _dataContext.SaveChangesAsync();
                await AddKSServiceAsync(receipt, receiptDetails);

                dbt.Commit();
            }
            return container;
        }

        public async Task<IEnumerable<ReceiptMemoContainer>> AddRangeReceiptMemoAsync(
           ModelStateDictionary modelState, IEnumerable<ReceiptMemoContainer> containers
        )
        {
            foreach(var container in containers) {
                await AddReceiptMemoAsync(modelState, container);
            }
            return containers;
        }

        public async Task<ReceiptMemoContainer> AddReceiptMemoAsync(ModelStateDictionary modelState, ReceiptMemoContainer container)
        {
            if(Exist(container.ReceiptMemo.Entity)) { 
                container.ReceiptMemo.IsDuplicate = true;
                _logger.LogInformation("ReceiptMemo already exists.");
                return container; 
            }

            await _syncAdapter.UpdateReferencesAsync(modelState, container.ReceiptMemo);
            await _syncAdapter.UpdateRangeReferencesAsync(modelState, container.ReceiptDetailMemos);
            if (modelState.IsValid)
            {
                using var dbt = await _dataContext.Database.BeginTransactionAsync();

                var receiptMemo = container.ReceiptMemo.Entity;
                var userSetting = await _dataContext.GeneralSettings.AsNoTracking()
                                .FirstOrDefaultAsync(s => s.UserID == receiptMemo.UserOrderID);
                var seriesDetail = await AddSeriesDetailAsync(userSetting);

                receiptMemo.ID = 0;
                receiptMemo.SeriesID = seriesDetail.SeriesID;
                receiptMemo.SeriesDID = seriesDetail.ID;
                await _dataContext.ReceiptMemo.AddAsync(receiptMemo);

                var receiptDetailMemos = container.ReceiptDetailMemos.Select(rdm => rdm.Entity);
                foreach(var rdm in receiptDetailMemos)
                {
                    rdm.ID = 0;
                    rdm.ReceiptMemoID = receiptMemo.ID;
                }

                await _dataContext.ReceiptDetailMemoKvms.AddRangeAsync(receiptDetailMemos);
                await _dataContext.SaveChangesAsync();

                dbt.Commit();
            }
            return container;
        }

        public async Task<EntryMap<TEntity>> AddSingleEntityAsync<TEntity>(
           ModelStateDictionary modelState, EntryMap<TEntity> entry
        ) where TEntity : class, ISyncEntity
        {
            if (Exist(entry.Entity))
            {
                entry.IsDuplicate = true;
                _logger.LogInformation($"{nameof(TEntity)} already exists.");
                return entry;
            }

            await _syncAdapter.UpdateReferencesAsync(modelState, entry);
            if (modelState.IsValid)
            {
                await _dataContext.AddAsync(entry.Entity);
                await _dataContext.SaveChangesAsync();
            }
            return entry;
        }

        public async Task<IEnumerable<EntryMap<OpenShift>>> AddRangeOpenShiftAsync(
           ModelStateDictionary modelState, IEnumerable<EntryMap<OpenShift>> openShifts
        )
        {
            foreach(var os in openShifts)
            {
                os.Entity.ID = 0;
                await AddSingleEntityAsync<OpenShift>(modelState, os);
            }
            return openShifts;
        }

        public async Task<IEnumerable<EntryMap<CloseShift>>> AddRangeCloseShiftAsync(
            ModelStateDictionary modelState, IEnumerable<EntryMap<CloseShift>> closeShifts
        )
        {
            foreach(var cs in closeShifts)
            {
                cs.Entity.ID = 0;
                await AddSingleEntityAsync<CloseShift>(modelState, cs);
            }
            return closeShifts;
        }

        public async Task<IEnumerable<VoidOrderContainer>> AddRangeVoidOrderAsync(
            ModelStateDictionary modelState, IEnumerable<VoidOrderContainer> containers
        )
        {
            foreach(var container in containers)
            {
                await AddVoidOrderAsync(modelState, container);
            }
            return containers;
        }

        public async Task<VoidOrderContainer> AddVoidOrderAsync(
            ModelStateDictionary modelState,  VoidOrderContainer container
        )
        {
            if (Exist(container.VoidOrder.Entity)) { 
                container.VoidOrder.IsDuplicate = true;
                _logger.LogInformation("VoidOrder already exists.");
                return container; 
            }
            await _syncAdapter.UpdateReferencesAsync(modelState, container.VoidOrder);
            await _syncAdapter.UpdateRangeReferencesAsync(modelState, container.VoidOrderDetails);
            if (modelState.IsValid)
            {
                using var t = await _dataContext.Database.BeginTransactionAsync();

                var voidOrder = container.VoidOrder.Entity;
                voidOrder.OrderID = 0;
                await _dataContext.VoidOrders.AddAsync(voidOrder);
                await _dataContext.SaveChangesAsync();
                var voidOrderDetails = container.VoidOrderDetails.Select(vod => vod.Entity);
                foreach(var vod in voidOrderDetails)
                {
                    vod.OrderDetailID = 0;
                    vod.OrderID = voidOrder.OrderID;
                }
               
                await _dataContext.VoidOrderDetails.AddRangeAsync(voidOrderDetails);
                await _dataContext.SaveChangesAsync();

                t.Commit();
            }
            return container;
        }

        public async Task<IEnumerable<VoidItemContainer>> AddRangeVoidItemAsync(
           ModelStateDictionary modelState, IEnumerable<VoidItemContainer> containers
        )
        {
            foreach(var container in containers)
            {
                await AddVoidItemAsync(modelState, container);
            }
            return containers;
        }

        public async Task<VoidItemContainer> AddVoidItemAsync(
            ModelStateDictionary modelState, VoidItemContainer container
        ) {
            if(Exist(container.VoidItem.Entity)) { 
                container.VoidItem.IsDuplicate = true;
                _logger.LogInformation("VoidItem already exists.");
                return container;
            }
            await _syncAdapter.UpdateReferencesAsync(modelState, container.VoidItem);
            await _syncAdapter.UpdateRangeReferencesAsync(modelState, container.VoidItemDetails);
            if (modelState.IsValid)
            {
                using var t  = await _dataContext.Database.BeginTransactionAsync();

                var voidItem = container.VoidItem.Entity;
                voidItem.ID = 0;
                await _dataContext.VoidItems.AddAsync(voidItem);
                await _dataContext.SaveChangesAsync();
                var voidItemDetails = container.VoidItemDetails.Select(vid => vid.Entity);
                foreach(var vid in voidItemDetails)
                {
                    vid.ID = 0;
                    vid.VoidItemID = voidItem.ID;
                }
         
                await _dataContext.VoidItemDetails.AddRangeAsync(voidItemDetails);
                await _dataContext.SaveChangesAsync();

                t.Commit();
            }
            return container;
        }

        public async Task IssueStockInternalAsync(IEnumerable<Receipt> srcReceipts = null)
        {
            var receipts = srcReceipts;
            if (srcReceipts == null)
            {
                var _receipts = _pos.GetNoneIssuedValidStockReceipts();
                receipts = await _receipts.Take(10).ToListAsync();
            }
            else
            {
                receipts = srcReceipts.Where(r => _pos.IsValidStock(r)).ToList();
            }

            if (!receipts.Any()) { return; }
            foreach (var receipt in receipts)
            {
                receipt.RececiptDetail ??= await _dataContext.ReceiptDetail
                        .Where(rd => rd.ReceiptID == receipt.ReceiptID).ToListAsync();

                if (!receipt.RececiptDetail.Any()) { continue; }
                receipt.MultiPaymentMeans = await _dataContext.MultiPaymentMeans
                    .Where(mp => mp.ReceiptID == receipt.ReceiptID)
                    .ToListAsync();
                if (_dataContext.SystemLicenses.AsNoTracking().Any(w => w.Edition == SystemEdition.Basic))
                {
                    await _pos.IssueStockBasicAsync(receipt, OrderStatus.Paid,
                        new List<SerialNumber>(), new List<BatchNo>(), receipt.MultiPaymentMeans);
                }
                else
                {
                    await _pos.IssueStockAsync(receipt, OrderStatus.Paid,
                        new List<SerialNumber>(), new List<BatchNo>(), receipt.MultiPaymentMeans);
                }

            }
        }

        public async Task IssueStockReturnedInternalAsync()
        {
            var receiptMemos = await _dataContext.ReceiptMemo.AsNoTracking()
                .Where(rm => rm.RowId != default
                    && !_dataContext.InventoryAudits.AsNoTracking().Any(ia => ia.SeriesDetailID == rm.SeriesDID)
                ).ToListAsync();
            if (receiptMemos.Count <= 0) { return; }
            foreach (var receiptMemo in receiptMemos)
            {
                if (_dataContext.SystemLicenses.AsNoTracking().Any(w => w.Edition == SystemEdition.Basic))
                {
                    _pos.OrderDetailReturnStockBasic(receiptMemo.ID, new List<SerialNumber>(), new List<BatchNo>(), 0, "return");
                }
                else
                {
                    _pos.OrderDetailReturnStock(receiptMemo.ID, new List<SerialNumber>(), new List<BatchNo>(), 0, "return");
                }
            }
        }

        public async Task<SeriesDetail> AddSeriesDetailAsync(GeneralSetting setting)
        {
            if (setting == null) { return null; }
            var series = await _dataContext.Series.FirstOrDefaultAsync(w => w.ID == setting.SeriesID);
            if (series == null) { return null; }

            SeriesDetail seriesDetail = new();
            //insert seriesDetail
            seriesDetail.SeriesID = series.ID;
            seriesDetail.Number = series.NextNo;
            _dataContext.Update(seriesDetail);
            //update series
            string Sno = series.NextNo;
            int snoLenth = Sno.Length;
            _ = long.TryParse(Sno, out long _sno);
            _sno++;
            series.NextNo = $"{_sno}".PadLeft(snoLenth, '0');
            _dataContext.Update(series);
            await _dataContext.SaveChangesAsync();
            return seriesDetail;
        }

        public async Task AddKSServiceAsync(Receipt receipt, IEnumerable<ReceiptDetail> receiptDetails)
        {
            foreach (var rd in receiptDetails)
            {
                if (rd.IsKsmsMaster)
                {
                    var ksSetUpExisted = _dataContext.KSServices
                        .FirstOrDefault(i => i.KSServiceSetupId == rd.KSServiceSetupId && i.CusId == receipt.CustomerID);
                    if (ksSetUpExisted != null)
                    {
                        ksSetUpExisted.MaxCount += rd.Qty;
                        ksSetUpExisted.Qty += rd.Qty;
                    }
                    else
                    {
                        KSService ksmsService = new()
                        {
                            CusId = receipt.CustomerID,
                            ID = 0,
                            KSServiceSetupId = rd.KSServiceSetupId,
                            MaxCount = rd.Qty,
                            Qty = rd.Qty,
                            UsedCount = 0,
                            VehicleID = rd.VehicleId,
                            ReceiptDID = rd.ID,
                            ReceiptID = receipt.ReceiptID,
                            PriceListID = receipt.PriceListID,
                        };
                        _dataContext.KSServices.Update(ksmsService);
                    }
                }
            }
            await _dataContext.SaveChangesAsync();
        }
    }
}
