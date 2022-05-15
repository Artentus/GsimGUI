using System.Collections.Generic;

namespace GsimGUI.Models
{
    internal enum PinDirection
    {
        In,
        Out,
        InOut,
    }

    internal class Pin : Component
    {
        private readonly Property.Integer _widthProperty = new(nameof(Width), 1, 1);
        private readonly Property.PinDirection _directionProperty = new(nameof(Direction), PinDirection.In);

        public int Width
        {
            get => _widthProperty.Value;
            set => _widthProperty.Value = value;
        }

        public PinDirection Direction
        {
            get => _directionProperty.Value;
            set => _directionProperty.Value = value;
        }

        public override IReadOnlyList<Property> Properties { get; }

        public Pin()
        {
            Properties = new Property[] { _widthProperty, _directionProperty };
            RegisterPropertyEvents();
        }
    }
}
