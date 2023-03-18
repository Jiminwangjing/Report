using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KEDI.Core.Premise.Models.Sync
{
    public interface ISyncEntity
    {
        Guid RowId { set; get; }
        public DateTimeOffset ChangeLog { set; get; }
    }

    public partial class SyncEntity : ISyncEntity
    {
        public SyncEntity(){}
        public SyncEntity(Type entityType, Guid rowId, string foreignKeyName){
            EntityType = entityType;
            RowId = rowId;
            ForeignKeyName = foreignKeyName;
        }
        public SyncEntity(Guid rowId, DateTimeOffset changeLog) {
            RowId = rowId;
            ChangeLog = changeLog;
        }

        public bool IsOptional { set; get; }
        public string ForeignKeyName { set; get; }
        public Type EntityType { set; get; }
        public Guid RowId { set; get; }
        public DateTimeOffset ChangeLog { set; get; }
    }
}
