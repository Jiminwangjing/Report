
using CKBS.Models.Services.POS;
using KEDI.Core.Premise.Models.Services.POS;
using System;
using System.Collections;
using System.Collections.Generic;

namespace KEDI.Core.Premise.Models.Sync
{
    public class EntryMap<TEntity> : IEntityState 
        where TEntity : class, ISyncEntity
    {
        public EntryMap() { }
        public EntryMap(TEntity entity)
        {
            Entity = entity;
            ChangeLog = entity.ChangeLog;
        }

        public EntryMap(TEntity entity, ICollection<SyncEntity> references)
        {
            Entity = entity;
            ChangeLog = entity.ChangeLog;
            References = references;
        }

        public TEntity Entity { set; get; }
        public ICollection<SyncEntity> References { set; get; }
        public DateTimeOffset ChangeLog { private set; get; } // Source modified date  
        public bool IsModified => Entity.ChangeLog > ChangeLog;
        public bool IsValid { set; get; }
        public bool IsDuplicate { set; get; }
        
    }
}
