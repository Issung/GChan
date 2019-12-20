using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace GChan
{
    public static class EnumHelper
    {
        // your enum->string method (I just decluttered it a bit :))
        public static string GetEnumDescription(Enum value)
        {
            var fi = value.GetType().GetField(value.ToString());
            var attributes = fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes.Length > 0)
                return ((DescriptionAttribute)attributes[0]).Description;
            else
                return value.ToString();
        }

        // the method to go from string->enum
        public static T GetEnumFromDescription<T>(string stringValue)
            where T : struct
        {
            foreach (object e in Enum.GetValues(typeof(T)))
                if (GetEnumDescription((Enum)e).Equals(stringValue))
                    return (T)e;
            throw new ArgumentException("No matching enum value found.");
        }

        // and a method to get a list of string values - no KeyValuePair needed
        public static IEnumerable<string> GetEnumDescriptions(Type enumType)
        {
            var strings = new Collection<string>();
            foreach (Enum e in Enum.GetValues(enumType))
                strings.Add(GetEnumDescription(e));
            return strings;
        }
    }
}
