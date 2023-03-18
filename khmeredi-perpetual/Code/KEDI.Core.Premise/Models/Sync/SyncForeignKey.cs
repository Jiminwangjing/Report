using System;
using System.Collections.Generic;

namespace KEDI.Core.Premise.Models.Sync
{
    public class SyncForeignKey
    {
        public SyncForeignKey(Type entityType, string fkName, object fkValue, bool isOptional = false)
        {
            EntityType = entityType;        
            ForeignKeyName = fkName;
            ForeignKey = fkValue;
            IsOptional = isOptional;
        }

        public bool IsOptional { set; get; }
        public string ForeignKeyName { set; get; }
        public object ForeignKey { set; get; }
        public Type EntityType { set; get; }
    }
}
