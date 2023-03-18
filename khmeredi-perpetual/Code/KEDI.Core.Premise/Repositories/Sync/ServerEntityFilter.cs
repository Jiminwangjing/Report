using CKBS.AppContext;
using CKBS.Models.Services.Account;
using CKBS.Models.Services.Administrator.General;
using CKBS.Models.Services.Administrator.Inventory;
using CKBS.Models.Services.Administrator.Setup;
using CKBS.Models.Services.Administrator.SystemInitialization;
using CKBS.Models.Services.Administrator.Tables;
using CKBS.Models.Services.Banking;
using CKBS.Models.Services.HumanResources;
using CKBS.Models.Services.Inventory;
using CKBS.Models.Services.Inventory.Category;
using CKBS.Models.Services.Inventory.PriceList;
using CKBS.Models.Services.Inventory.Property;
using CKBS.Models.Services.LoyaltyProgram.BuyXAmountGetXDiscount;
using CKBS.Models.Services.LoyaltyProgram.ComboSale;
using CKBS.Models.Services.POS;
using CKBS.Models.Services.Promotions;
using KEDI.Core.Models.ControlCenter.ApiManagement;
using KEDI.Core.Premise.AppContext;
using KEDI.Core.Premise.AppContext.Sync.Customs;
using KEDI.Core.Premise.Models.Services.Account;
using KEDI.Core.Premise.Models.Services.Administrator.CanRingSetup;
using KEDI.Core.Premise.Models.Services.Banking;
using KEDI.Core.Premise.Models.Services.LoyaltyProgram.BuyXGetX;
using KEDI.Core.Premise.Models.Services.LoyaltyProgram.MemberPoints;
using KEDI.Core.Premise.Models.Services.LoyaltyProgram.PromoCode;
using KEDI.Core.Premise.Models.Services.POS.CanRing;
using KEDI.Core.Premise.Models.Services.POS.LoyaltyProgram;
using KEDI.Core.Premise.Models.Sync;
using KEDI.Core.Premise.Models.Sync.Customs.Server;
using KEDI.Core.Premise.Utilities;
using KEDI.Core.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PointCard = KEDI.Core.Premise.Models.Services.LoyaltyProgram.MemberPoints.PointCard;

namespace KEDI.Core.Premise.Repositories.Sync
{
    public interface IServerEntityFilter
    {
        Task<ServerSyncContainer> GetServerSyncContainerAsync();
        Task<IEnumerable<EntryMap<TEntity>>> GetEntitiesAsync<TEntity>(
            Func<TEntity, Task<EntryMap<TEntity>>> func, int rowSize = 100
        ) where TEntity : class, ISyncEntity;
        Task<IEnumerable<EntryMap<TEntity>>> GetEntitiesAsync<TEntity>(int rowSize = 100)
            where TEntity : class, ISyncEntity;
        Task UpdateRangeHistoryAsync<TEntity>(
           Type entityType, IEnumerable<TEntity> entities
        ) where TEntity : class, ISyncEntity;

    }

    public partial class ServerEntityFilter : CustomEntityContext, IServerEntityFilter
    {
        public async Task<ServerSyncContainer> GetServerSyncContainerAsync()
        {
            var posRef = new ServerSyncContainer
            {
                Currencies = await GetEntitiesAsync<Currency>(),
                Functions = await GetEntitiesAsync<Function>(),
                PrinterNames = await GetEntitiesAsync<PrinterName>(),
                UoMs = await GetEntitiesAsync<UnitofMeasure>(),
                GroupUOMs = await GetEntitiesAsync<GroupUOM>(),
                GroupDefinedUOMs = await GetEntitiesAsync<GroupDUoM>(),
                CardTypes = await GetEntitiesAsync<CardType>(),
                ItemGroup1s = await GetEntitiesAsync<ItemGroup1>(),
                DocumentTypes = await GetEntitiesAsync<DocumentType>(),
                ItemComments = await GetEntitiesAsync<ItemComment>(),
                AuthTemplates = await GetEntitiesAsync<AuthorizationTemplate>(),
                PriceLists = await GetEntitiesAsync<PriceLists>(x => MapReferencesAsync(x)),
                PriceListDetails = await GetEntitiesAsync<PriceListDetail>(x => MapReferencesAsync(x)),
                Promotions = await GetEntitiesAsync<Promotion>(x => MapReferencesAsync(x)),
                Companies = await GetEntitiesAsync<Company>(x => MapReferencesAsync(x)),
                Branches = await GetEntitiesAsync<Branch>(x => MapReferencesAsync(x)),
                Employees = await GetEntitiesAsync<Employee>(x => MapReferencesAsync(x)),
                UserAccounts = await GetEntitiesAsync<UserAccount>(x => MapReferencesAsync(x)),
                UserPrivilleges = await GetEntitiesAsync<UserPrivillege>(x => MapReferencesAsync(x)),
                ExchangeRates = await GetEntitiesAsync<ExchangeRate>(x => MapReferencesAsync(x)),
                GroupTables = await GetEntitiesAsync<GroupTable>(x => MapReferencesAsync(x)),
                Tables = await GetEntitiesAsync<Table>(x => MapReferencesAsync(x)),
                ItemGroup2s = await GetEntitiesAsync<ItemGroup2>(x => MapReferencesAsync(x)),
                ItemGroup3s = await GetEntitiesAsync<ItemGroup3>(x => MapReferencesAsync(x)),
                ItemMasterDatas = await GetEntitiesAsync<ItemMasterData>(x => MapReferencesAsync(x)),
                BuyXGetXs = await GetEntitiesAsync<BuyXGetX>(x => MapReferencesAsync(x)),
                BuyXGetXDetails = await GetEntitiesAsync<BuyXGetXDetail>(x => MapReferencesAsync(x)),
                BuyXQtyGetXDiss = await GetEntitiesAsync<BuyXQtyGetXDis>(x => MapReferencesAsync(x)),
                PBuyXAmountGetXDiss = await GetEntitiesAsync<PBuyXAmountGetXDis>(x => MapReferencesAsync(x)),
                Warehouses = await GetEntitiesAsync<Warehouse>(x => MapReferencesAsync(x)),
                BusinessPartners = await GetEntitiesAsync<BusinessPartner>(x => MapReferencesAsync(x)),
                TaxGroups = await GetEntitiesAsync<TaxGroup>(x => MapReferencesAsync(x)),
                TaxGroupDefinitions = await GetEntitiesAsync<TaxGroupDefinition>(x => MapReferencesAsync(x)),
                CanRingMasters = await GetEntitiesAsync<CanRingMaster>(x => MapReferencesAsync(x)),
                CanRings = await GetEntitiesAsync<CanRing>(x => MapReferencesAsync(x)),
                CanRingDetails = await GetEntitiesAsync<CanRingDetail>(x => MapReferencesAsync(x)),
                Serieses = await GetEntitiesAsync<Series>(x => MapReferencesAsync(x)),
                SeriesDetails = await GetEntitiesAsync<SeriesDetail>(x => MapReferencesAsync(x)),
                SaleCombos = await GetEntitiesAsync<SaleCombo>(x => MapReferencesAsync(x)),
                SaleComboDetails = await GetEntitiesAsync<SaleComboDetail>(x => MapReferencesAsync(x)),
                Redeems = await GetEntitiesAsync<Redeem>(x => MapReferencesAsync(x)),
                RedeemRetails = await GetEntitiesAsync<RedeemRetail>(x => MapReferencesAsync(x)),
                PromoCodeDiscounts = await GetEntitiesAsync<PromoCodeDiscount>(x => MapReferencesAsync(x)),
                PromoCodeDetails = await GetEntitiesAsync<PromoCodeDetail>(x => MapReferencesAsync(x)),
                Properties = await GetEntitiesAsync<Property>(x => MapReferencesAsync(x)),
                PropertyDetails = await GetEntitiesAsync<PropertyDetails>(x => MapReferencesAsync(x)),
                ChildProperties = await GetEntitiesAsync<ChildPreoperty>(x => MapReferencesAsync(x)),
                PaymentMeans = await GetEntitiesAsync<PaymentMeans>(x => MapReferencesAsync(x)),
                Freights = await GetEntitiesAsync<Freight>(x => MapReferencesAsync(x)),
                MemberCards = await GetEntitiesAsync<MemberCard>(x => MapReferencesAsync(x)),
                PointCards = await GetEntitiesAsync<PointCard>(x => MapReferencesAsync(x)),
                PointRedemptions = await GetEntitiesAsync<PointRedemption>(x => MapReferencesAsync(x)),
                PointItems = await GetEntitiesAsync<PointItem>(x => MapReferencesAsync(x)),
                PeriodIndicators = await GetEntitiesAsync<PeriodIndicator>(x => MapReferencesAsync(x)),
                ReceiptInfos = await GetEntitiesAsync<ReceiptInformation>(x => MapReferencesAsync(x))
            };
            return posRef;
        }
    }

    public partial class ServerEntityFilter : CustomEntityContext, IServerEntityFilter
    {
        private readonly DataContext _dataContext;
        private readonly IQueryContext _query;
        private readonly IClientApiRepo _clientApiRepo;
        public ServerEntityFilter(ILogger<ServerEntityFilter> logger,
            DataContext dataContext,
            IClientApiRepo clientApiRepo,
            IQueryContext query
        ) : base(logger, query)
        {
            _query = query;
            _dataContext = dataContext;
            _clientApiRepo = clientApiRepo;
        }

        public async Task UpdateRangeHistoryAsync<TEntity>(
            Type entityType, IEnumerable<TEntity> entities
        ) where TEntity : class, ISyncEntity
        {
            ClientApp clientApp = _clientApiRepo.GetCurrentClient();
            foreach (var ent in entities)
            {
                var syncHistory = await FindHistoryAsync<ServerSyncHistory>(
                    h => h.RowId == ent.RowId && h.ClientId == clientApp.Id);
                if (syncHistory == null)
                {
                    syncHistory = new ServerSyncHistory
                    {
                        RowId = ent.RowId,
                        ClientId = clientApp.Id,
                        TenantId = clientApp.Code,
                        ChangeLog = DateTimeOffset.UtcNow,
                        TableName = _query.GetTableName(entityType)
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

        public IQueryable<TEntity> FilterChangedEntities<TEntity>(DataContext dc, long clientId)
            where TEntity : class, ISyncEntity
        {
            IQueryable<ServerSyncHistory> syncHistories = dc.Set<ServerSyncHistory>().AsNoTracking();
            syncHistories = syncHistories.Where(h => h.ClientId == clientId);
            var changedEntities = FilterChangedEntities<TEntity, ServerSyncHistory>(dc, syncHistories);
            return changedEntities;
        }

        public async Task<IEnumerable<EntryMap<TEntity>>> GetEntitiesAsync<TEntity>(
            Func<TEntity, Task<EntryMap<TEntity>>> func, int rowSize = 100)
            where TEntity : class, ISyncEntity
        {
            ClientApp clientApp = _clientApiRepo.GetCurrentClient();
            using var db = new DataContext();
            var entities = FilterChangedEntities<TEntity>(db, clientApp.Id).Take(rowSize);
            var entryMaps = new List<EntryMap<TEntity>>();
            foreach (var ent in entities)
            {
                entryMaps.Add(await func.Invoke(ent));
            }
            return entryMaps;
        }

        public async Task<IEnumerable<EntryMap<TEntity>>> GetEntitiesAsync<TEntity>(int rowSize = 100)
            where TEntity : class, ISyncEntity
        {
            ClientApp clientApp = _clientApiRepo.GetCurrentClient();
            using var db = new DataContext();
            var entities = FilterChangedEntities<TEntity>(db, clientApp.Id).Take(rowSize);
            return await entities.Select(ent => new EntryMap<TEntity>(ent)).ToListAsync();
        }
    }
}
