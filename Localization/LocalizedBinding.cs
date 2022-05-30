using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using Avalonia.Markup.Xaml.XamlIl.Runtime;
using System.Collections.Generic;
using System.Globalization;

namespace GsimGUI.Localization
{
    internal class LocalizedBinding : IBinding
    {
        private readonly Binding _inner;
        private readonly Binding _cultureBinding;
        private IValueConverter? _converter;
        private object? _converterParameter;

        public LocalizedBinding()
        {
            _inner = new Binding();
            _cultureBinding = new Binding(nameof(Localization.SelectedCulture))
            {
                Source = Localization.Instance,
            };
        }

        public LocalizedBinding(string path, BindingMode mode = BindingMode.Default)
        {
            _inner = new Binding(path, mode);
            _cultureBinding = new Binding(nameof(Localization.SelectedCulture))
            {
                Source = Localization.Instance,
            };
        }

        public IValueConverter? Converter
        {
            get => _converter;
            set => _converter = value;
        }

        public object? ConverterParameter
        {
            get => _converterParameter;
            set => _converterParameter = value;
        }

        public object? FallbackValue
        {
            get => _inner.FallbackValue;
            set => _inner.FallbackValue = value;
        }

        public object? TargetNullValue
        {
            get => _inner.TargetNullValue;
            set => _inner.TargetNullValue = value;
        }

        public BindingMode Mode
        {
            get => _inner.Mode;
            set => _inner.Mode = value;
        }

        public BindingPriority Priority
        {
            get => _inner.Priority;
            set => _inner.Priority = value;
        }

        public string? StringFormat
        {
            get => _inner.StringFormat;
            set => _inner.StringFormat = value;
        }

        public WeakReference? DefaultAnchor
        {
            get => _inner.DefaultAnchor;
            set => _inner.DefaultAnchor = value;
        }

        public WeakReference<INameScope>? NameScope
        {
            get => _inner.NameScope;
            set => _inner.NameScope = value;
        }

        public string? ElementName
        {
            get => _inner.ElementName;
            set => _inner.ElementName = value;
        }

        public RelativeSource? RelativeSource
        {
            get => _inner.RelativeSource;
            set => _inner.RelativeSource = value;
        }

        public object? Source
        {
            get => _inner.Source;
            set => _inner.Source = value;
        }

        public string Path
        {
            get => _inner.Path;
            set => _inner.Path = value;
        }

        public Func<string, string, Type>? TypeResolver
        {
            get => _inner.TypeResolver;
            set => _inner.TypeResolver = value;
        }

        public InstancedBinding? Initiate(
            IAvaloniaObject target,
            AvaloniaProperty? targetProperty,
            object? anchor = null,
            bool enableDataValidation = false)
        {
            object? ConvertValue(IList<object?> value)
            {
                if (value[1] is BindingNotification notification1)
                {
                    if (!notification1.HasValue) return notification1;
                }

                object? value0 = value[0];

                if (value0 is BindingNotification notification0)
                {
                    if (!notification0.HasValue) return notification0;
                    else value0 = notification0.Value;
                }

                if (_converter is not null)
                    return _converter.Convert(value0, typeof(string), null, CultureInfo.CurrentUICulture);

                return value0;
            }

            var innerInit = _inner.Initiate(target, null);
            var cultureBindingInit = _cultureBinding.Initiate(target, null);

            var children = new IObservable<object?>[] { innerInit!.Observable!, cultureBindingInit!.Observable! };
            var input = children.CombineLatest().Select(ConvertValue);

            var mode = (Mode == BindingMode.Default)
                ? targetProperty?.GetMetadata(target.GetType()).DefaultBindingMode
                : Mode;

            return mode switch
            {
                BindingMode.OneTime => InstancedBinding.OneTime(input!, Priority),
                BindingMode.OneWay => InstancedBinding.OneWay(input!, Priority),
                _ => throw new NotSupportedException("LocalizedBinding only supports OneTime and OneWay BindingMode."),
            };
        }
    }

    internal static class XamlExtensions
    {
        public static T? GetService<T>(this IServiceProvider sp)
            => (T?)sp?.GetService(typeof(T));

        public static Uri? GetContextBaseUri(this IServiceProvider ctx)
            => ctx.GetService<IUriContext>()?.BaseUri;

        public static T? GetFirstParent<T>(this IServiceProvider ctx) where T : class
            => ctx.GetService<IAvaloniaXamlIlParentStackProvider>()?.Parents.OfType<T>().FirstOrDefault();

        public static T? GetLastParent<T>(this IServiceProvider ctx) where T : class
            => ctx.GetService<IAvaloniaXamlIlParentStackProvider>()?.Parents.OfType<T>().LastOrDefault();

        public static IEnumerable<T>? GetParents<T>(this IServiceProvider sp)
            => sp.GetService<IAvaloniaXamlIlParentStackProvider>()?.Parents.OfType<T>();

        public static Type? ResolveType(this IServiceProvider ctx, string namespacePrefix, string type)
        {
            var tr = ctx.GetService<IXamlTypeResolver>();
            string name = string.IsNullOrEmpty(namespacePrefix) ? type : $"{namespacePrefix}:{type}";
            return tr?.Resolve(name);
        }

        public static object? GetDefaultAnchor(this IServiceProvider provider)
        {
            object? anchor = provider.GetFirstParent<IControl>();
            if (anchor is null) anchor = provider.GetFirstParent<IDataContextProvider>();

            return anchor ??
                   provider.GetService<IRootObjectProvider>()?.RootObject as IStyle ??
                   provider.GetLastParent<IStyle>();
        }
    }

    internal sealed class LocalizedBindingExtension
    {
        public LocalizedBindingExtension()
        {
            FallbackValue = AvaloniaProperty.UnsetValue;
            TargetNullValue = AvaloniaProperty.UnsetValue;
        }

        public LocalizedBindingExtension(string path)
            : this()
        {
            Path = path;
        }

        public LocalizedBinding ProvideValue(IServiceProvider serviceProvider)
        {
            static WeakReference<T>? CreateWeakReference<T>(T? target) where T: class
            {
                if (target is null) return null;
                return new WeakReference<T>(target);
            }

            var descriptorContext = (ITypeDescriptorContext)serviceProvider;

            return new LocalizedBinding()
            {
                TypeResolver = descriptorContext.ResolveType!,
                Converter = Converter,
                ConverterParameter = ConverterParameter,
                ElementName = ElementName,
                FallbackValue = FallbackValue,
                Mode = Mode,
                Path = Path,
                Priority = Priority,
                Source = Source,
                StringFormat = StringFormat,
                RelativeSource = RelativeSource,
                DefaultAnchor = new WeakReference(descriptorContext.GetDefaultAnchor()),
                TargetNullValue = TargetNullValue,
                NameScope = CreateWeakReference(serviceProvider.GetService<INameScope>())
            };
        }

        public IValueConverter? Converter { get; set; }

        public object? ConverterParameter { get; set; }

        public string? ElementName { get; set; }

        public object? FallbackValue { get; set; }

        public BindingMode Mode { get; set; }

        [ConstructorArgument("path")]
        public string Path { get; set; } = "";

        public BindingPriority Priority { get; set; }

        public object? Source { get; set; }

        public string? StringFormat { get; set; }

        public RelativeSource? RelativeSource { get; set; }

        public object? TargetNullValue { get; set; }
    }
}
