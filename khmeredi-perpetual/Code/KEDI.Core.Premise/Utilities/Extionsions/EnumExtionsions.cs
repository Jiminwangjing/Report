using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace KEDI.Core.Helpers.Enumerations
{
    public static class EnumExtionsions
    {
        /// <summary>
        ///     A generic extension method that aids in reflecting 
        ///     and retrieving any attribute that is applied to an `Enum`.
        /// </summary>
        public static string GetDisplayName(this Enum enumValue)
        {
            return enumValue.GetType()?
                    .GetMember(enumValue.ToString())?
                    .FirstOrDefault()?
                    .GetCustomAttribute<DisplayAttribute>()?
                    .Name;
        }
    }

    public class EnumHelper<T>
    {
        public static Dictionary<int, string> ToDictionary()
        {
            return EnumHelper.ToDictionary<T>();
        }
    }

    public class EnumHelper
    {
        public static Dictionary<int, string> ToDictionary<T>()
        {
            return ToDictionary(typeof(T));
        }

        public static Dictionary<int, string> ToDictionary(Type type)
        {
            var keyValues = new Dictionary<int, string>();
            var enumValues = Enum.GetValues(type);
            foreach (var pt in enumValues)
            {
                var memberInfo = type.GetMember(pt.ToString());
                var attributes = memberInfo[0].GetCustomAttributes(typeof(DisplayAttribute), false);
                var description = attributes.Length > 0 ? ((DisplayAttribute)attributes[0]).Name : pt.ToString();
                keyValues.TryAdd((int)pt, description);
            }
            return keyValues;
        }
    }
}
