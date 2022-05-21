using Avalonia;
using Avalonia.Platform;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace GsimGUI.Localization
{
    internal sealed class Localization : PropertyChangedBase
    {
        private sealed class LocalizedString : IObservable<string>
        {
            private sealed class Unsubscriber : IDisposable
            {
                private readonly List<IObserver<string>> _observers;
                private readonly IObserver<string> _observer;

                public Unsubscriber(List<IObserver<string>> observers, IObserver<string> observer)
                {
                    _observers = observers;
                    _observer = observer;
                }

                public void Dispose() => _observers.Remove(_observer);
            }


            private readonly List<IObserver<string>> _observers = new();
            private readonly string _key;

            public LocalizedString(string key)
            {
                _key = key;
            }

            public IDisposable Subscribe(IObserver<string> observer)
            {
                if (!_observers.Contains(observer))
                {
                    _observers.Add(observer);
                    observer.OnNext(Localization.Instance.Get(_key));
                }
                
                return new Unsubscriber(_observers, observer);
            }

            public void LanguageChanged()
            {
                string s = Localization.Instance.Get(_key);

                foreach (var observer in _observers)
                    observer.OnNext(s);
            }
        }


        public static Localization Instance = new();


        private readonly Dictionary<CultureInfo, IReadOnlyDictionary<string, string>> _locales;
        private readonly Dictionary<string, LocalizedString> _observables = new();
        private readonly IReadOnlyDictionary<string, string> _defaultLocale;
        private CultureInfo _selectedCulture;

        public IReadOnlyList<CultureInfo> AvailableCultures { get; }

        public CultureInfo SelectedCulture
        {
            get => _selectedCulture;
            set
            {
                if (SetProperty(ref _selectedCulture, value))
                {
                    foreach (var observable in _observables.Values)
                        observable.LanguageChanged();
                }
            }
        }

        private static IReadOnlyDictionary<string, string> ParseLocale(Stream json)
            => JsonSerializer.Deserialize<Dictionary<string, string>>(json)!;

        private Localization()
        {
            var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();

            var defaultCulture = new CultureInfo("en");
            var defaultUri = new Uri($"avares://GsimGUI/Assets/en.json");
            var defaultLocale = ParseLocale(assets!.Open(defaultUri));

            _locales = new() { { defaultCulture, defaultLocale } };
            _defaultLocale = defaultLocale;
            _selectedCulture = defaultCulture;

            var langDir = new DirectoryInfo(Path.Combine(App.Current!.BinaryDirectory.FullName, "lang"));
            if (langDir.Exists)
            {
                foreach (var file in langDir.EnumerateFiles("*.json"))
                {
                    var cultureString = file.NameWithoutExtension();
                    try
                    {
                        var culture = new CultureInfo(cultureString);
                        using var stream = file.OpenRead();
                        var locale = ParseLocale(stream);
                        _locales.Add(culture, locale);
                    }
                    catch (CultureNotFoundException)
                    { }
                }
            }

            AvailableCultures = _locales.Keys.ToArray();
        }

        public IObservable<string> GetObservable(string key)
        {
            if (!_observables.TryGetValue(key, out var observable))
            {
                observable = new LocalizedString(key);
                _observables.Add(key, observable);
            }

            return observable;
        }

        public string Get(string key)
        {
            if (!_locales.TryGetValue(SelectedCulture, out var locale))
                locale = _defaultLocale;

            if (!locale.TryGetValue(key, out var value))
                value = $"Unknown key: {key}";

            return value;
        }
    }
}
