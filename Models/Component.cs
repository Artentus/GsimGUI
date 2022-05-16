using System.Collections.Generic;
using System.ComponentModel;

namespace GsimGUI.Models
{
    internal enum Direction
    {
        In,
        Out,
        InOut,
    }

    internal sealed record class Connection(Direction Direction, int Width)
    {
        public Bus? ConnectedBus { get; set; } = null;
    }

    internal abstract class Component : PropertyChangedBase
    {
        public abstract IReadOnlyList<Property> Properties { get; }

        protected void RegisterPropertyEvents()
        {
            void PropertyOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
            {
                if ((sender is Property property) && (e.PropertyName == "Value"))
                    OnPropertyChanged(property.Name);
            }

            foreach (var property in Properties)
                property.PropertyChanged += PropertyOnPropertyChanged;
        }

        public abstract IReadOnlyList<Connection> Connections { get; }
    }
}
