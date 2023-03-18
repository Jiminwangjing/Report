using CKBS.AppContext;
using KEDI.Core.Premise.AppContext;
using KEDI.Core.Premise.AppContext.Sync;
using KEDI.Core.Premise.Models.Sync;
using KEDI.Core.Utilities;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Repositories.Sync
{
    public interface ISyncAdapter
    {
        Task<TEntity> UpdateEntityAsync<TEntity>(
            ModelStateDictionary modelState, EntryMap<TEntity> entityMap
        ) where TEntity : class, ISyncEntity;
        Task<object> UpdateEntityAsync(
            ModelStateDictionary modelState, object entity, IEnumerable<SyncEntity> references
        );
       
        Task UpdateRangeEntityAsync<TEntity>(
            ModelStateDictionary modelState, IEnumerable<EntryMap<TEntity>> entryMaps
        ) where TEntity : class, ISyncEntity;

        Task<bool> UpdateReferencesAsync<TEntity>(
            ModelStateDictionary modelState, EntryMap<TEntity> entryMap
        ) where TEntity : class, ISyncEntity;

        Task<bool> UpdateReferencesAsync<TEntity>(
           ModelStateDictionary modelState, TEntity entity, IEnumerable<SyncEntity> references
        ) where TEntity : class, ISyncEntity;

        Task<bool> UpdateReferencesAsync(
            ModelStateDictionary modelState, object entity, IEnumerable<SyncEntity> references
        );

        Task<bool> UpdateRangeReferencesAsync<TEntity>(
            ModelStateDictionary modelState, IEnumerable<EntryMap<TEntity>> entryMaps
        ) where TEntity : class, ISyncEntity;
    }

    public class SyncAdapter : EntityContext, ISyncAdapter
    {
        private readonly DataContext _dataContext;
        private readonly ILogger<SyncAdapter> _logger;    
        public SyncAdapter(ILogger<SyncAdapter> logger, 
            DataContext dataContext, IQueryContext query
        ) : base(logger, query)
        {
            _logger = logger;
            _dataContext = dataContext;         
        }

        public async Task UpdateRangeEntityAsync<TEntity>(
            ModelStateDictionary modelState,  IEnumerable<EntryMap<TEntity>> entryMaps
        ) where TEntity : class, ISyncEntity
        {
            foreach (var entryMap in entryMaps)
            {
                await UpdateEntityAsync(modelState, entryMap);
            }
        }

        public async Task<TEntity> UpdateEntityAsync<TEntity>(
            ModelStateDictionary modelState, EntryMap<TEntity> entityMap
        ) where TEntity : class, ISyncEntity
        {
            var references = entityMap.References;
            var entity = entityMap.Entity;
            await UpdateReferencesAsync(modelState, entity, references);
            if (modelState.IsValid)
            {
                var originEntity = await FindByRowIdAsync<TEntity>(entity.RowId, true);
                string pkName = GetPkProperty(entity).Name;
                if (originEntity == null)
                {
                    EntityHelper.SetProperty(entity, pkName, 0);
                    await _dataContext.AddAsync(entity);
                }
                else
                {
                    object pkValue = EntityHelper.GetProperty(originEntity, pkName);
                    EntityHelper.SetProperty(entity, pkName, pkValue);
                    _dataContext.Entry(originEntity).State = EntityState.Detached;
                    var entityState = _dataContext.Entry(entity).State;
                    if (entityState == EntityState.Detached)
                    {
                        _dataContext.Attach(entity);
                        _dataContext.Entry(entity).State = EntityState.Modified;
                    }
                    _dataContext.Update(entity);
                }
                await _dataContext.SaveChangesAsync();
                _dataContext.Entry(entity).State = EntityState.Detached;
            }
            
            return entity;
        }

        public async Task<object> UpdateEntityAsync(
            ModelStateDictionary modelState, object entity, 
            IEnumerable<SyncEntity> references = null
        )
        {
            try {
                await UpdateReferencesAsync(modelState, entity, references);   
                if(modelState.IsValid)
                {
                    object rowId = entity.GetType().GetProperty(nameof(ISyncEntity.RowId)).GetValue(entity, null);
                    _ = Guid.TryParse(rowId.ToString(), out Guid _rowId);
                    var originEntity = await FindByRowIdAsync(entity.GetType(), _rowId);
                    string pkName = GetPkProperty(entity.GetType()).Name;
                    if (originEntity == null)
                    {
                        EntityHelper.SetProperty(entity, pkName, 0);
                        await _dataContext.AddAsync(entity);
                    }
                    else
                    {
                        _ = originEntity.TryGetValue(pkName, out object pkValue);
                        EntityHelper.SetProperty(entity, pkName, pkValue);
                        var entityState = _dataContext.Entry(entity).State;
                        if (entityState == EntityState.Detached)
                        {
                            _dataContext.Attach(entity);
                            _dataContext.Entry(entity).State = EntityState.Modified;
                        }

                        _dataContext.Update(entity);
                    }
                    await _dataContext.SaveChangesAsync();
                    _dataContext.Entry(entity).State = EntityState.Detached;
                }
               
            } catch(Exception ex){
                _logger.LogError(ex.Message);
            }
            return entity;
        }

        public async Task<bool> UpdateRangeReferencesAsync<TEntity>(
          ModelStateDictionary modelState,  IEnumerable<EntryMap<TEntity>> entryMaps
        ) where TEntity : class, ISyncEntity
        {          
            if(entryMaps == null) { return modelState.IsValid; }
            foreach(var entryMap in entryMaps)
            {                
                await UpdateReferencesAsync(modelState, entryMap);
            }
            return modelState.IsValid;
        }

        public async Task<bool> UpdateReferencesAsync<TEntity>(
            ModelStateDictionary modelState, EntryMap<TEntity> entryMap
        ) where TEntity : class, ISyncEntity
        {
            if (entryMap == null) { return modelState.IsValid; }
            entryMap.IsValid = await UpdateReferencesAsync(modelState, entryMap.Entity, entryMap.References);
            return entryMap.IsValid;
        }

        public async Task<bool> UpdateReferencesAsync<TEntity>(
            ModelStateDictionary modelState, TEntity entity, 
            IEnumerable<SyncEntity> references
        ) where TEntity : class, ISyncEntity
        {
            if (references == null) { return modelState.IsValid; }
            foreach (var refEnt in references)
            {
                object pk = await GetPkByRowIdAsync(refEnt.EntityType, refEnt.RowId);
                if (pk == null)
                {
                    if(!refEnt.IsOptional) {
                        modelState.AddModelError(refEnt.ForeignKeyName, $"{refEnt.ForeignKeyName} is not matched.");
                    }                  
                    continue;
                }
                EntityHelper.SetProperty(entity, refEnt.ForeignKeyName, pk);
            }

            return modelState.IsValid;
        }

        public async Task<bool> UpdateReferencesAsync(
           ModelStateDictionary modelState, object entity, 
           IEnumerable<SyncEntity> references
        )
        {
            if (references == null) { return modelState.IsValid; }
            foreach (var refEnt in references)
            {
                object pk = await GetPkByRowIdAsync(refEnt.EntityType, refEnt.RowId);
                if (pk == null)
                {
                    if(!refEnt.IsOptional) {
                        modelState.AddModelError(refEnt.ForeignKeyName, $"{refEnt.ForeignKeyName} is not matched.");
                    }                   
                    
                    continue;
                }
                EntityHelper.SetProperty(entity, refEnt.ForeignKeyName, pk);
            }
            return modelState.IsValid;
        }
    }
}
