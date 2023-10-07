using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace GChan
{
    public static class EnumHelper
    {
        public static string GetEnumDescription(Enum value)
        {
            var fi = value.GetType().GetField(value.ToString());
            var attributes = fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes.Length > 0)
                return ((DescriptionAttribute)attributes[0]).Description;
            else
                return value.ToString();
        }

        public static T GetEnumFromDescription<T>(string stringValue) where T : Enum
        {
            foreach (object e in Enum.GetValues(typeof(T)))
                if (GetEnumDescription((Enum)e).Equals(stringValue))
                    return (T)e;

            throw new ArgumentException("No matching enum value found.");
        }

        public static IEnumerable<string> GetEnumDescriptions(Type enumType)
        {
            var strings = new Collection<string>();

            foreach (Enum e in Enum.GetValues(enumType))
                strings.Add(GetEnumDescription(e));

            return strings;
        }
    }
}
