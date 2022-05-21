using Avalonia.Data;
using System;

namespace GsimGUI.Localization
{
    internal sealed class LocalizedExtension : MarkupExtension<Binding>
    {
        private record struct ObservableWrapper(IObservable<string> Observable);

        public string Key { get; }

        public LocalizedExtension(string key)
        {
            Key = key;
        }

        public override Binding ProvideValue(IServiceProvider serviceProvider)
        {
            var observable = Localization.Instance.GetObservable(Key);
            var wrapper = new ObservableWrapper(observable);

            return new Binding("Observable^")
            {
                Source = wrapper,
            };
        }
    }
}
