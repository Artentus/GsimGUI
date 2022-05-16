using System;

namespace GsimGUI.Models
{
    internal abstract class Property : PropertyChangedBase
    {
        public string Name { get; }

        protected Property(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            Name = name;
        }

        public abstract void Reset();

        public T Match<T>(Func<BooleanProperty, T> onBoolean, Func<IntegerProperty, T> onInteger, Func<DirectionProperty, T> onDirection)
        {
            return this switch
            {
                BooleanProperty bp => onBoolean(bp),
                IntegerProperty ip => onInteger(ip),
                DirectionProperty dp => onDirection(dp),
                _ => throw new InvalidOperationException("Property is of unknown type."),
            };
        }

        public void Match(Action<BooleanProperty> onBoolean, Action<IntegerProperty> onInteger, Action<DirectionProperty> onDirection)
        {
            switch (this)
            {
                case BooleanProperty bp:
                    onBoolean(bp);
                    break;

                case IntegerProperty ip:
                    onInteger(ip);
                    break;

                case DirectionProperty dp:
                    onDirection(dp);
                    break;

                default:
                    throw new InvalidOperationException("Property is of unknown type.");
            }
        }
    }

    internal sealed class BooleanProperty : Property
    {
        private bool _value;

        public bool DefaultValue { get; }

        public bool Value
        {
            get => _value;
            set => SetProperty(ref _value, value);
        }

        public BooleanProperty(string name, bool defaultValue)
            : base(name)
        {
            DefaultValue = defaultValue;
            Value = defaultValue;
        }

        public override void Reset()
            => Value = DefaultValue;
    }

    internal sealed class IntegerProperty : Property
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

        public IntegerProperty(string name, int defaultValue, int minimumValue = 0, int maximumValue = int.MaxValue)
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

    internal sealed class DirectionProperty : Property
    {
        private Models.Direction _value;

        public Models.Direction DefaultValue { get; }

        public Models.Direction Value
        {
            get => _value;
            set => SetProperty(ref _value, value);
        }

        public DirectionProperty(string name, Models.Direction defaultValue)
            : base(name)
        {
            DefaultValue = defaultValue;
            Value = defaultValue;
        }

        public override void Reset()
            => Value = DefaultValue;
    }
}
