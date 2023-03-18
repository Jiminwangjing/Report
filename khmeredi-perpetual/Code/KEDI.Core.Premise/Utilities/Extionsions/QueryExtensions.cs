using KEDI.Core.Utilities;
using NPOI.XSSF.Streaming.Properties;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace KEDI.Core.Premise.Utilities.Extionsions
{
    public static class QueryExtensions
    {
        public static IEnumerable<TEntity> Set<TEntity>(this IDataReader reader) 
            where TEntity : new()
        {
            while (reader.Read())
            {
                TEntity obj = new TEntity();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    string key = reader.GetName(i);
                    object value = reader.GetValue(i);
                    EntityHelper.SetProperty(obj, key, value);                  
                }
                yield return obj;
            }
        }

        public static IEnumerable<Dictionary<string, object>> ToDictionaries(this IDataReader reader)
        {
            while (reader.Read())
            {
                Dictionary<string, object> obj = new Dictionary<string, object>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    string key = reader.GetName(i);
                    object value = reader.GetValue(i);
                    obj.Add(key, value);
                }
                yield return obj;
            }
        }
    }
}
