using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace KEDI.Core.Utilities
{
    public static class EntityHelper
    {
        public static T Assign<T>(object source) where T : class, new()
        {
            var desObj = new T();
            try
            {             
                PropertyInfo[] props = desObj.GetType().GetProperties();
                for (int i = 0; i < props.Length; i++)
                {
                    PropertyInfo prop = source.GetType().GetProperty(props[i].Name);
                    if(prop == null) { continue; }
                    object sourceValue = prop?.GetValue(source);
                    props[i].SetValue(desObj, sourceValue, null);
                }
                return desObj;
            }
            catch
            {
                return desObj;
            }
        }

        public static void SetProperty(object obj, string key, object value)
        {
            PropertyInfo propInfo = obj.GetType().GetProperty(key);
            if (propInfo == null) { return; }
            Type propType = Nullable.GetUnderlyingType(propInfo.PropertyType) ?? propInfo.PropertyType;
            if (propType.IsEnum)
            {
                value = Enum.ToObject(propType, value);
            }
            else
            {
                value = Convert.ChangeType(value, propType, default);
            }
            propInfo.SetValue(obj, value, null);
        }

        public static object GetProperty(object obj, string key)
        {
            PropertyInfo propInfo = obj.GetType().GetProperty(key);
            object value = propInfo.GetValue(obj, null);
            return value;
        }
    }
}
