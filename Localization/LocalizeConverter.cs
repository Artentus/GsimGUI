using System;
using System.Globalization;

namespace GsimGUI.Localization
{
    internal sealed class LocalizeStringConverter : ValueConverter<string, string>
    {
        protected override string? Convert(string? value, CultureInfo culture)
        {
            if (value is null) return null;
            return Localization.Instance.Get(value);
        }
    }

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
