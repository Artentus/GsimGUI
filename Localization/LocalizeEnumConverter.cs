using System;
using System.Globalization;

namespace GsimGUI.Localization
{
    internal sealed class LocalizeEnumConverter<T> : ValueConverter<T, string> where T : struct, Enum
    {
        protected override string? Convert(T value, CultureInfo culture)
        {
            var typeName = typeof(T).Name;
            var valueName = value.ToString();
            var key = $"{typeName}::{valueName}";

            return Localization.Instance.Get(key);
        }
    }
}
