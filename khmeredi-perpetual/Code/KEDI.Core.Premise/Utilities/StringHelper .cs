using System.Text.RegularExpressions;

namespace KEDI.Core.Utilities
{
    public static class StringHelper
    {
        public static bool Compare(string valueA, string valueB, bool ignoreCase = false, bool includeEmpty = false)
        {
            if (!includeEmpty)
            {
                if(string.IsNullOrWhiteSpace(valueA) || string.IsNullOrWhiteSpace(valueB))
                {
                    return false;
                }
            }

            return string.Compare(valueA, valueB, ignoreCase) == 0;
        }

        public static bool Equals(string valueA, string valueB, bool ignoreCase = false, bool includeEmpty = false)
        {
            return Compare(valueA, valueB, ignoreCase, includeEmpty);
        }

        public static bool IsEqual(this string baseValue, string value, bool ignoreCase = false, bool includeEmpty = false)
        {
            return Compare(baseValue, value, ignoreCase, includeEmpty);
        }

        public static string RemoveSpace(string input, bool onlySpaceAround = false)
        {
            if (string.IsNullOrWhiteSpace(input)) { return string.Empty; };
            if (onlySpaceAround)
            {
                return input.Trim();
            }
            return Regex.Replace(input, "\\s+", string.Empty);
        }

        public static string NoSpace(this string input, bool onlySpaceAround = false)
        {
            return RemoveSpace(input, onlySpaceAround);
        }
        static public string Capitalize(this string text)
        {
            string value = Regex.Replace(text, @"(^\w)|(\s\w)", m => m.Value.ToUpper());
            return value;
        }
    }
}
