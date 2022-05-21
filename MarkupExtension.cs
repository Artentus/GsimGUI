using Avalonia;
using System;

namespace GsimGUI
{
    internal interface IMarkupExtension
    {
        object? ProvideValue(IServiceProvider serviceProvider);
    }

    internal abstract class MarkupExtension<T> : AvaloniaObject, IMarkupExtension
    {
        public abstract T ProvideValue(IServiceProvider serviceProvider);

        object? IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
            => ProvideValue(serviceProvider);
    }
}
