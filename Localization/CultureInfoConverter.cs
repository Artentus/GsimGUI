using System.Globalization;

namespace GsimGUI.Localization
{
    internal sealed class CultureInfoConverter : ValueConverter<CultureInfo, string>
    {
        protected override string? Convert(CultureInfo? value, CultureInfo culture)
        {
            if (value is null) return null;

            if (!value.IsNeutralCulture) value = value.Parent;
            return $"{value.NativeName} ({value.EnglishName})";
        }
    }
}
