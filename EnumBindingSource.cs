using Avalonia;
using System;

namespace GsimGUI
{
    internal class EnumBindingSource : AvaloniaObject
    {
        private Type? _enumType;

        public Type? EnumType
        {
            get => _enumType;
            set
            {
                if (value != _enumType)
                {
                    if (value is not null)
                    {
                        Type enumType = Nullable.GetUnderlyingType(value) ?? value;
                        if (!enumType.IsEnum) throw new ArgumentException("Type must be an Enum.");
                    }

                    _enumType = value;
                }
            }
        }

        public EnumBindingSource()
        { }

        public EnumBindingSource(Type enumType)
        {
            EnumType = enumType;
        }

        public Array ProvideValue(IServiceProvider serviceProvider)
        {
            if (_enumType is null) throw new InvalidOperationException("EnumType must be specified.");

            Type actualEnumType = Nullable.GetUnderlyingType(_enumType) ?? _enumType;
            Array enumValues = Enum.GetValues(actualEnumType);
            if (actualEnumType == _enumType) return enumValues;

            Array tempArray = Array.CreateInstance(actualEnumType, enumValues.Length + 1);
            enumValues.CopyTo(tempArray, 1);
            return tempArray;
        }
    }
}
