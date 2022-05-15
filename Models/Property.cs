using System;

namespace GsimGUI.Models
{
    internal abstract class Property : PropertyChangedBase
    {
        public string Name { get; }

        private Property(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            Name = name;
        }

        public abstract void Reset();

        public sealed class Boolean : Property
        {
            private bool _value;

            public bool DefaultValue { get; }

            public bool Value
            {
                get => _value;
                set => SetProperty(ref _value, value);
            }

            public Boolean(string name, bool defaultValue)
                : base(name)
            {
                DefaultValue = defaultValue;
                Value = defaultValue;
            }

            public override void Reset()
                => Value = DefaultValue;
        }

        public sealed class Integer : Property
        {
            private int _value;

            public int DefaultValue { get; }

            public int MinimumValue { get; }

            public int MaximumValue { get; }

            public int Value
            {
                get => _value;
                set => SetProperty(ref _value, Math.Clamp(value, MinimumValue, MaximumValue));
            }

            public Integer(string name, int defaultValue, int minimumValue = 0, int maximumValue = int.MaxValue)
                : base(name)
            {
                if (minimumValue > maximumValue) throw new ArgumentException("Minimum value must be less than or equal to maximum value.");
                if (defaultValue < minimumValue) throw new ArgumentOutOfRangeException(nameof(defaultValue), "Default value must be greater than or equal to minimum value.");
                if (defaultValue > maximumValue) throw new ArgumentOutOfRangeException(nameof(defaultValue), "Default value must be less than or equal to maximum value.");

                DefaultValue = defaultValue;
                MinimumValue = minimumValue;
                MaximumValue = maximumValue;
                Value = defaultValue;
            }

            public override void Reset()
                => Value = DefaultValue;
        }

        public sealed class PinDirection : Property
        {
            private Models.PinDirection _value;

            public Models.PinDirection DefaultValue { get; }

            public Models.PinDirection Value
            {
                get => _value;
                set => SetProperty(ref _value, value);
            }

            public PinDirection(string name, Models.PinDirection defaultValue)
                : base(name)
            {
                DefaultValue = defaultValue;
                Value = defaultValue;
            }

            public override void Reset()
                => Value = DefaultValue;
        }

        public T Match<T>(Func<Boolean, T> onBoolean, Func<Integer, T> onInteger, Func<PinDirection, T> onPinDirection)
        {
            return this switch
            {
                Boolean bp => onBoolean(bp),
                Integer ip => onInteger(ip),
                PinDirection pdp => onPinDirection(pdp),
                _ => throw new InvalidOperationException("Property is of unknown type."),
            };
        }

        public void Match(Action<Boolean> onBoolean, Action<Integer> onInteger, Action<PinDirection> onPinDirection)
        {
            switch (this)
            {
                case Boolean bp:
                    onBoolean(bp);
                    break;

                case Integer ip:
                    onInteger(ip);
                    break;

                case PinDirection pdp:
                    onPinDirection(pdp);
                    break;

                default:
                    throw new InvalidOperationException("Property is of unknown type.");
            }
        }
    }
}
