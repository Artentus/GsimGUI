using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Styling;

namespace GsimGUI
{
    internal enum ThemeMode
    {
        Light,
        Dark,
    }

    internal class Theme : AvaloniaObject, IStyle, IResourceProvider
    {
        private readonly Styles _sharedStyles;
        private readonly Styles _fluentLight;
        private readonly Styles _fluentDark;
        private bool _isLoading;
        private IStyle? _loaded;

        /// <summary>
        /// Initializes a new instance of the <see cref="Theme"/> class.
        /// </summary>
        /// <param name="baseUri">The base URL for the XAML context.</param>
        public Theme(Uri baseUri)
        {
            _sharedStyles = new Styles
            {
                new StyleInclude(baseUri)
                {
                    Source = new Uri("avares://Avalonia.Themes.Fluent/Accents/AccentColors.xaml")
                },
                new StyleInclude(baseUri)
                {
                    Source = new Uri("avares://Avalonia.Themes.Fluent/Accents/Base.xaml")
                },
                new StyleInclude(baseUri)
                {
                    Source = new Uri("avares://Avalonia.Themes.Fluent/Controls/FluentControls.xaml")
                },
            };

            _fluentLight = new Styles
            {
                new StyleInclude(baseUri)
                {
                    Source = new Uri("avares://Avalonia.Themes.Fluent/Accents/BaseLight.xaml")
                },
                new StyleInclude(baseUri)
                {
                    Source = new Uri("avares://Avalonia.Themes.Fluent/Accents/FluentControlResourcesLight.xaml")
                },
                new StyleInclude(baseUri)
                {
                    Source = new Uri("avares://GsimGUI/Assets/Styles/Light.axaml")
                },
            };

            _fluentDark = new Styles
            {
                new StyleInclude(baseUri)
                {
                    Source = new Uri("avares://Avalonia.Themes.Fluent/Accents/BaseDark.xaml")
                },
                new StyleInclude(baseUri)
                {
                    Source = new Uri("avares://Avalonia.Themes.Fluent/Accents/FluentControlResourcesDark.xaml")
                },
                new StyleInclude(baseUri)
                {
                    Source = new Uri("avares://GsimGUI/Assets/Styles/Dark.axaml")
                },
            };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Theme"/> class.
        /// </summary>
        /// <param name="serviceProvider">The XAML service provider.</param>
        public Theme(IServiceProvider serviceProvider)
            : this(((IUriContext)serviceProvider.GetService(typeof(IUriContext))!).BaseUri)
        { }

        public static readonly StyledProperty<ThemeMode> ModeProperty =
            AvaloniaProperty.Register<Theme, ThemeMode>(nameof(Mode));

        /// <summary>
        /// Gets or sets the mode of the fluent theme (light, dark).
        /// </summary>
        public ThemeMode Mode
        {
            get => GetValue(ModeProperty);
            set => SetValue(ModeProperty, value);
        }

        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == ModeProperty)
            {
                var loaded = (Loaded as Styles)!;

                switch (Mode)
                {
                    case ThemeMode.Light:
                        if (loaded.Count > 1) loaded[1] = _fluentLight;
                        else loaded.Add(_fluentLight);
                        break;

                    case ThemeMode.Dark:
                        if (loaded.Count > 1) loaded[1] = _fluentDark;
                        else loaded.Add(_fluentDark);
                        break;

                    default:
                        if (loaded.Count > 1) loaded.RemoveAt(1);
                        break;
                }
            }
        }

        public IResourceHost? Owner => (Loaded as IResourceProvider)?.Owner;

        /// <summary>
        /// Gets the loaded style.
        /// </summary>
        public IStyle Loaded
        {
            get
            {
                if (_loaded is null)
                {
                    _isLoading = true;

                    _loaded = Mode switch
                    {
                        ThemeMode.Light => new Styles() { _sharedStyles, _fluentLight },
                        ThemeMode.Dark => new Styles() { _sharedStyles, _fluentDark },
                        _ => new Styles() { _sharedStyles },
                    };

                    _isLoading = false;
                }

                return _loaded;
            }
        }

        bool IResourceNode.HasResources => (Loaded as IResourceProvider)?.HasResources ?? false;

        IReadOnlyList<IStyle> IStyle.Children => _loaded?.Children ?? Array.Empty<IStyle>();

        public event EventHandler OwnerChanged
        {
            add
            {
                if (Loaded is IResourceProvider rp)
                    rp.OwnerChanged += value;
            }
            remove
            {
                if (Loaded is IResourceProvider rp)
                    rp.OwnerChanged -= value;
            }
        }

        public SelectorMatchResult TryAttach(IStyleable target, IStyleHost? host) => Loaded.TryAttach(target, host);

        public bool TryGetResource(object key, out object? value)
        {
            if (!_isLoading && (Loaded is IResourceProvider p))
                return p.TryGetResource(key, out value);

            value = null;
            return false;
        }

        void IResourceProvider.AddOwner(IResourceHost owner) => (Loaded as IResourceProvider)?.AddOwner(owner);
        void IResourceProvider.RemoveOwner(IResourceHost owner) => (Loaded as IResourceProvider)?.RemoveOwner(owner);
    }
}
