using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace GsimGUI.Models
{
    internal class Pin : Component
    {
        private readonly IntegerProperty _widthProperty = new(nameof(Width), 1, 1);
        private readonly DirectionProperty _directionProperty = new(nameof(Direction), Direction.In);
        private Connection _connection = new(Direction.In, 1);

        public int Width
        {
            get => _widthProperty.Value;
            set => _widthProperty.Value = value;
        }

        public Direction Direction
        {
            get => _directionProperty.Value;
            set => _directionProperty.Value = value;
        }

        public override IReadOnlyList<Property> Properties { get; }

        public override IReadOnlyList<Connection> Connections => new Connection[] { _connection };

        public Pin()
        {
            Properties = new Property[] { _widthProperty, _directionProperty };
            RegisterPropertyEvents();
        }

        protected override void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            switch (propertyName)
            {
                case nameof(Width):
                    _connection = _connection with { Width = Width };
                    base.OnPropertyChanged(nameof(Connections));
                    break;

                case nameof(Direction):
                    _connection = _connection with { Direction = Direction };
                    base.OnPropertyChanged(nameof(Connections));
                    break;
            }

            base.OnPropertyChanged(propertyName);
        }
    }
}
