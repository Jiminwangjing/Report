using KEDI.Core.Premise.Models.Sync;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using CKBS.AppContext;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using KEDI.Core.Utilities;
using Microsoft.EntityFrameworkCore.Internal;
using KEDI.Core.Premise.AppContext.Extensions;

namespace KEDI.Core.Premise.AppContext.Sync
{
    public class EntityContext
    {
        private readonly ILogger<EntityContext> _logger;
        private readonly IQueryContext _query;
        public EntityContext(ILogger<EntityContext> logger, IQueryContext query)
        {
            _logger = logger;
            _query = query;
        }

        public virtual IQueryable<TEntity> FilterChangedEntities<TEntity, TSyncHistory>(
           DataContext dc, IQueryable<TSyncHistory> syncHistories
        ) where TEntity : class, ISyncEntity where TSyncHistory : class, ITransactionHistory
        {
            IQueryable<TEntity> entities = dc.Set<TEntity>().AsNoTracking();
            var changedEntities = from e in entities.Where(e => e.RowId != Guid.Empty)
                                  join h in syncHistories
                                  on e.RowId equals h.RowId into gh
                                  from h in gh.DefaultIfEmpty() where e.ChangeLog > h.ChangeLog || h == null
                                  select new { e, h };
            return changedEntities.Select(x => x.e);
        }

        public virtual async Task<IEnumerable<EntryMap<TEntity>>> MapRangeReferencesAsync<TEntity>(
            IEnumerable<TEntity> entities, Func<TEntity, Task<EntryMap<TEntity>>> func
        ) where TEntity : class, ISyncEntity
        {
            var entryMaps = new List<EntryMap<TEntity>>();
            foreach (var ent in entities)
            {
                entryMaps.Add(await func.Invoke(ent));
            }
            return entryMaps;
        }

        protected virtual async Task<EntryMap<TEntity>> MapForeignKeysAsync<TEntity>(
          TEntity entity, params SyncForeignKey[] foreignKeys
        ) where TEntity : class, ISyncEntity
        {
            try
            {
                var refEntities = new List<SyncEntity>();
                foreach (var fk in foreignKeys)
                {
                    var ent = await FindByPkAsync(fk.EntityType, fk.ForeignKey);
                    if (ent == null) { continue; }
                    var refEnt = new SyncEntity(fk.EntityType, ent.RowId, fk.ForeignKeyName);
                    
                    refEnt.IsOptional = fk.IsOptional;
                    refEnt.ChangeLog = ent.ChangeLog;       
                    refEntities.Add(refEnt);
                }
                var entryMap = new EntryMap<TEntity>(entity, refEntities);
                return entryMap;
            } catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return new EntryMap<TEntity>(entity);
            }    
        }

        protected virtual SyncForeignKey MapWith<TRefEntity>(string fkName, object fkValue, bool isOptional = false)
            where TRefEntity : class, ISyncEntity
        {
            return new SyncForeignKey(typeof(TRefEntity), fkName, fkValue, isOptional);
        }

        public virtual bool Exist<TEntity>(TEntity entity) where TEntity : class, ISyncEntity
        {
            using var db = new DataContext();
            var entities = db.Set<TEntity>().AsNoTracking();
            bool duplicated = entities.Any(ent => ent.RowId == entity.RowId);
            return duplicated;
        }

        public virtual bool Exist(object entity)
        {
            using var db = new DataContext();
            var entities = db.Set(entity.GetType()).AsNoTracking();
            Guid rowId = GetRowId(entity);
            return entities.Any(e => rowId == GetRowId(e));
        }

        public async Task<ISyncEntity> FindByPkAsync(Type entityType, object pk)
        {
            using var _context = new DataContext();
            var ent = await _context.FindAsync(entityType, pk);
            return (ISyncEntity)ent;
        }

        public async Task<TEntity> FindByPkAsync<TEntity>(object pk)
           where TEntity : class, ISyncEntity
        {
            using var _context = new DataContext();
            var ent = await _context.FindAsync<TEntity>(pk);
            return ent;
        }

        public async Task<TEntity> FindByRowIdAsync<TEntity>(Guid rowId, bool asNoTracking = false)
           where TEntity : class, ISyncEntity
        {
            using var _context = new DataContext();
            var entities = _context.Set<TEntity>().AsQueryable();
            if (asNoTracking)
            {
                entities = entities.AsNoTracking();
            }
            var _entity = await entities.SingleOrDefaultAsync(c => c.RowId == rowId);
            return _entity;
        }

        public virtual async Task<TSyncHistory> FindHistoryByRowIdAsync<TSyncHistory>(
            Guid rowId, bool asNoTracking = false
        ) where TSyncHistory : class, ITransactionHistory
        {
            using var _context = new DataContext();
            var entities = _context.Set<TSyncHistory>().AsQueryable();
            if (asNoTracking)
            {
                entities = entities.AsNoTracking();
            }
            var _entity = await entities.SingleOrDefaultAsync(c => c.RowId == rowId);
            return _entity;
        }

        public virtual async Task<TSyncHistory> FindHistoryAsync<TSyncHistory>(
           Func<TSyncHistory, bool> func, bool asNoTracking = false
        ) where TSyncHistory : class, ITransactionHistory
        {
            using var _context = new DataContext();
            var entities = _context.Set<TSyncHistory>().AsQueryable();
            if (asNoTracking)
            {
                entities = entities.AsNoTracking();
            }
            var _entity = await entities.SingleOrDefaultAsync(c => func.Invoke(c));
            return _entity;
        }


        public virtual PropertyInfo GetPkProperty(Type entityType)
        {
            using var dataContext = new DataContext();
            var keyName = dataContext.Model.FindEntityType(entityType).FindPrimaryKey()
                .Properties.Select(x => x.Name).SingleOrDefault();
            return entityType.GetProperty(keyName);
        }

        public virtual PropertyInfo GetPkProperty<TEntity>(TEntity entity)
        {
            using var dataContext = new DataContext();
            var keyName = dataContext.Model.FindEntityType(typeof(TEntity)).FindPrimaryKey()
                .Properties.Select(x => x.Name).SingleOrDefault();
            return entity.GetType().GetProperty(keyName);
        }      

        public virtual async Task<object> GetPkByRowIdAsync(Type entityType, Guid rowId)
        {        
            var ent = await FindByRowIdAsync(entityType, rowId);
            if (ent == null) { return null; }
            string pkName = GetPkProperty(entityType).Name;
            return ent.GetValueOrDefault(pkName, null);
        }

        public virtual async Task<Dictionary<string, object>> FindByRowIdAsync(Type entityType, Guid rowId)
        {
            using var conn = _query.NewSqlConnection();
            var reader = await _query.FromSqlCommand(conn, entityType).ExecuteReaderAsync();
            while (reader.Read())
            {
                Dictionary<string, object> obj = new Dictionary<string, object>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    string key = reader.GetName(i);
                    object value = reader.GetValue(i);
                    obj.Add(key, value);
                }
                _ = Guid.TryParse(obj.GetValueOrDefault(nameof(ISyncEntity.RowId)).ToString(), out Guid _rowId);
                if (rowId == _rowId) { return obj; }
            }
            await reader.CloseAsync();
            return null;
        }

        public virtual Guid GetRowId(Dictionary<string, object> entity)
        {
            entity.TryGetValue(nameof(ISyncEntity.RowId), out object rowId);
            return new Guid($"{rowId}");
        }

        public virtual Guid GetRowId(object entity)
        {
            object rowId = EntityHelper.GetProperty(entity, nameof(ISyncEntity.RowId));
            return new Guid($"{rowId}");
        }
    }
}
