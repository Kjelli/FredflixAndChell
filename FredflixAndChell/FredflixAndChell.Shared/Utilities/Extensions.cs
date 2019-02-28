using System;
using System.Collections.Generic;

namespace FredflixAndChell.Shared.Utilities
{
    public static class Extensions
    {
        public static T Next<T>(this T src) where T : struct
        {
            if (!typeof(T).IsEnum) throw new ArgumentException(string.Format("Argument {0} is not an Enum", typeof(T).FullName));

            T[] Arr = (T[])Enum.GetValues(src.GetType());
            int j = Array.IndexOf<T>(Arr, src) + 1;
            return (Arr.Length == j) ? Arr[0] : Arr[j];
        }
        #region Dictionary value parsing
        public static bool TryParseFloat(this Dictionary<string, string> dict, string key, out float value)
        {
            if (dict.ContainsKey(key) && !string.IsNullOrWhiteSpace(dict[key]))
            {
                var success = float.TryParse(dict[key], out float parsed);
                if (success)
                {
                    value = parsed;
                    return true;
                }
            }
            value = -1;
            return false;
        }

        public static bool TryParseInt(this Dictionary<string, string> dict, string key, out int value)
        {
            if (dict.ContainsKey(key) && !string.IsNullOrWhiteSpace(dict[key]))
            {
                var success = int.TryParse(dict[key], out int parsed);
                if (success)
                {
                    value = parsed;
                    return true;
                }
            }
            value = -1;
            return false;
        }

        public static bool TryParseBool(this Dictionary<string, string> dict, string key, out bool value)
        {
            if (dict.ContainsKey(key) && !string.IsNullOrWhiteSpace(dict[key]))
            {
                var success = bool.TryParse(dict[key], out bool parsed);
                if (success)
                {
                    value = parsed;
                    return true;
                }
            }
            value = false;
            return false;
        }

        public static string[] ParseCommaSeparatedList(this Dictionary<string, string> dict, string key)
        {
            if (dict.ContainsKey(key) && !string.IsNullOrWhiteSpace(dict[key]))
            {
                var elements = dict[key].Split(',');
                return elements;
            }
            return null;
        }
        #endregion
    }
}
