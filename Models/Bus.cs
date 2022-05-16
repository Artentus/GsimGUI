using System.Collections.Generic;
using System.ComponentModel;

namespace GsimGUI.Models
{
    internal class Bus : PropertyChangedBase, IHasProperties
    {
        private readonly IntegerProperty _widthProperty = new(nameof(Width), 1, 1);

        public string Name => nameof(Bus);

        public int Width
        {
            get => _widthProperty.Value;
            set => _widthProperty.Value = value;
        }

        public IReadOnlyList<Property> Properties { get; }

        public Bus()
        {
            Properties = new Property[] { _widthProperty };

            void PropertyOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
            {
                if ((sender is Property property) && (e.PropertyName == "Value"))
                    OnPropertyChanged(property.Name);
            }

            _widthProperty.PropertyChanged += PropertyOnPropertyChanged;
        }
    }
}
