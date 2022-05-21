using System;

namespace GsimGUI
{
    internal class EnumBindingSource : MarkupExtension<Array>
    {
        public Type EnumType { get; }

        public EnumBindingSource(Type enumType)
        {
            if (enumType is null) throw new ArgumentNullException(nameof(enumType));
            if (!enumType.IsEnum) throw new ArgumentException("Type must be an Enum.");

            EnumType = enumType;
        }

        public override Array ProvideValue(IServiceProvider serviceProvider)
            => Enum.GetValues(EnumType);
    }
}
