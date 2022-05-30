using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace GsimGUI
{
    internal abstract class ValueConverter<TSource, TTarget, TParameter> : IValueConverter
    {
        protected abstract TTarget? Convert(TSource? value, TParameter? parameter, CultureInfo culture);

        protected virtual TSource? ConvertBack(TTarget? value, TParameter? parameter, CultureInfo culture)
            => throw new NotSupportedException();

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
            => Convert((TSource?)value, (TParameter?)parameter, culture);

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => ConvertBack((TTarget?)value, (TParameter?)parameter, culture);
    }

    internal abstract class ValueConverter<TSource, TTarget> : IValueConverter
    {
        protected abstract TTarget? Convert(TSource? value, CultureInfo culture);

        protected virtual TSource? ConvertBack(TTarget? value, CultureInfo culture)
            => throw new NotSupportedException();

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
            => Convert((TSource?)value, culture);

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => ConvertBack((TTarget?)value, culture);
    }

    internal abstract class MultiValueConverter<TSource, TTarget, TParameter> : IMultiValueConverter
    {
        protected abstract TTarget? Convert(IList<TSource?> values, TParameter? parameter, CultureInfo culture);

        public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
            => Convert((IList<TSource?>)values, (TParameter?)System.Convert.ChangeType(parameter, typeof(TParameter)), culture);
    }

    internal abstract class MultiValueConverter<TSource, TTarget> : IMultiValueConverter
    {
        protected abstract TTarget? Convert(IList<TSource?> values, CultureInfo culture);

        public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
            => Convert((IList<TSource?>)values, culture);
    }

    internal sealed class NthValueConverter : MultiValueConverter<object?, object?, int>
    {
        protected override object? Convert(IList<object?> values, int parameter, CultureInfo culture)
            => values[parameter]?.ToString();
    }
}
