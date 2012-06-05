using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace DiskSnapshot
{
  /// <summary>
  /// A simple <see cref="IValueConverter"/> that casts the value to bool, and returns its negation.
  /// </summary>
  public class BooleanNotConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return !((bool)value);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return !((bool)value);
    }
  }

  /// <summary>
  /// Converts multiple boolean values into a Visibility.  Only returns Visible if all values are true; returns Collapsed otherwise.
  /// </summary>
  /// <remarks>
  /// If there are no values, <see cref="Convert"/> will return Collapsed.
  /// The multi-value version of <see cref="ConvertBack"/> is not supported.
  /// </remarks>
  public class BooleanToVisibilityMultiConverter : IMultiValueConverter, IValueConverter
  {
    #region IMultiValueConverter Members
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      if (values.Length == 0) return Visibility.Collapsed;
      foreach (object value in values)
      {
        bool flag = false;
        if (value is bool)
          flag = (bool)value;
        else if (value is bool?)
        {
          bool? nullable = (bool?)value;
          flag = nullable.HasValue ? nullable.Value : false;
        }
        if (!flag)
          return Visibility.Collapsed;
      }
      return Visibility.Visible;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
    #endregion

    #region IValueConverter Members
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      bool flag = false;
      if (value is bool)
      {
        flag = (bool)value;
      }
      else if (value is bool?)
      {
        bool? nullable = (bool?)value;
        flag = nullable.HasValue ? nullable.Value : false;
      }
      return (flag ? Visibility.Visible : Visibility.Collapsed);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return ((value is Visibility) && (((Visibility)value) == Visibility.Visible));
    }
    #endregion
  }

  /// <summary>
  /// Base class for IValueConverters that do not support ConvertBack.
  /// </summary>
  public abstract class OneWayConverter : IValueConverter
  {
    public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotSupportedException();
    }
  }

  /// <summary>
  /// Converts the value (which should be either a bool or nullable bool) into a <see cref="Visibility"/> value.
  /// Will return Visibility.Visible if the value is true, or Visibility.Hidden otherwise.
  /// </summary>
  public class BooleanToVisibilityHiddenConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      bool flag = false;
      if (value is bool)
      {
        flag = (bool)value;
      }
      else if (value is bool?)
      {
        bool? nullable = (bool?)value;
        flag = nullable.HasValue ? nullable.Value : false;
      }
      return (flag ? Visibility.Visible : Visibility.Hidden);
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return ((value is Visibility) && (((Visibility)value) == Visibility.Visible));
    }
  }

  /// <summary>
  /// Converts the value (which should be either a bool or nullable bool) into a <see cref="Visibility"/> value.
  /// Will return Visibility.Hidden if the value is true, or Visibility.Visible otherwise.
  /// </summary>
  public class BooleanToVisibilityHiddenNegatedConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      bool flag = false;
      if (value is bool)
      {
        flag = (bool)value;
      }
      else if (value is bool?)
      {
        bool? nullable = (bool?)value;
        flag = nullable.HasValue ? nullable.Value : false;
      }
      return (flag ? Visibility.Hidden : Visibility.Visible);
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return ((value is Visibility) && (((Visibility)value) != Visibility.Visible));
    }
  }

  /// <summary>
  /// Converts the value (which should be either a bool or nullable bool) into a <see cref="Visibility"/> value.
  /// Will return Visibility.Collapsed if the value is true, or Visibility.Visible otherwise.
  /// </summary>
  public class BooleanToVisibilityNegatedConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      bool flag = false;
      if (value is bool)
      {
        flag = (bool)value;
      }
      else if (value is bool?)
      {
        bool? nullable = (bool?)value;
        flag = nullable.HasValue ? nullable.Value : false;
      }
      return (flag ? Visibility.Collapsed : Visibility.Visible);
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return ((value is Visibility) && (((Visibility)value) != Visibility.Visible));
    }
  }

  public class IsEnabledToOpacityConverter : OneWayConverter
  {
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return ((bool)value) ? OpacityTrue : OpacityFalse;
    }
    public const double OpacityTrue = 1.0;
    public const double OpacityFalse = 0.5;
  }

  /// <summary>
  /// If value and paramter are .Equals, then we return Visibility.Visible; otherwise, we return Visibility.Collapsed.
  /// </summary>
  /// <remarks>
  /// Does not support back conversion.
  /// </remarks>
  public class EnumToVisibilityConverter : OneWayConverter
  {
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (parameter.Equals(value))
        return Visibility.Visible;
      else
        return Visibility.Collapsed;
    }
  }

  /// <summary>
  /// If value and paramter are .Equals, then we return Visibility.Visible; otherwise, we return Visibility.Hidden.
  /// </summary>
  /// <remarks>
  /// Does not support back conversion.
  /// </remarks>
  public class EnumToVisibilityHiddenConverter : OneWayConverter
  {
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (parameter.Equals(value))
        return Visibility.Visible;
      else
        return Visibility.Hidden;
    }
  }

  /// <summary>
  /// If value and paramter are .Equals, then we return Visibility.Collapsed; otherwise, we return Visibility.Visible.
  /// </summary>
  /// <remarks>
  /// Does not support back conversion.
  /// </remarks>
  public class EnumToVisibilityNegatedConverter : OneWayConverter
  {
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (!parameter.Equals(value))
        return Visibility.Visible;
      else
        return Visibility.Collapsed;
    }
  }

  /// <summary>
  /// Returns Visibility.Visible if the value is non-null; returns Visibility.Collapsed otherwise.
  /// </summary>
  /// <remarks>
  /// Does not support back conversion.
  /// </remarks>
  public class NullToVisibilityConverter : OneWayConverter
  {
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value == null)
        return Visibility.Collapsed;
      else
        return Visibility.Visible;
    }
  }

  /// <summary>
  /// Returns Visibility.Collapsed if the value is non-null; returns Visibility.Visible otherwise.
  /// </summary>
  /// <remarks>
  /// Does not support back conversion.
  /// </remarks>
  public class NullToVisibilityNegatedConverter : OneWayConverter
  {
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value != null)
        return Visibility.Collapsed;
      else
        return Visibility.Visible;
    }
  }


  //Causes display order to be one more than the actual index (which is 0 based)
  [ValueConversion(typeof(Int32), typeof(Int32))]
  public class ZeroBasedtoOneBasedConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      Int32 index = (Int32)value;
      return index + 1;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      Int32 index = (Int32)value;
      return index - 1;
    }
  }

  /// <summary>
  /// If value is true, we'll return <see cref="TrueValue"/>.  Otherwise, we'll return <see cref="FalseValue"/>.
  /// </summary>
  /// <remarks>
  /// Does not support back conversion.
  /// </remarks>
  public class BooleanToObjectConverter : DependencyObject, IValueConverter
  {
    public static readonly DependencyProperty TrueValueProperty =
        DependencyProperty.Register("TrueValue", typeof(object), typeof(BooleanToObjectConverter), new UIPropertyMetadata());
    public object TrueValue
    {
      get { return (object)GetValue(TrueValueProperty); }
      set { SetValue(TrueValueProperty, value); }
    }

    public static readonly DependencyProperty FalseValueProperty =
        DependencyProperty.Register("FalseValue", typeof(object), typeof(BooleanToObjectConverter), new UIPropertyMetadata());
    public object FalseValue
    {
      get { return (object)GetValue(FalseValueProperty); }
      set { SetValue(FalseValueProperty, value); }
    }

    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if ((bool)value)
        return TrueValue;
      else
        return FalseValue;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotSupportedException();
    }
  }

  public class ObjectTypeToVisibilityConverter : IValueConverter
  {
    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value == null) return Visibility.Collapsed;
      if (!(value is Type))
        value = value.GetType();
      return ((Type)parameter).IsAssignableFrom((Type)value) ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotSupportedException();
    }

    #endregion
  }

  /// <summary>
  /// Converts multiple boolean values into a Visibility.  Only returns Collapsed if all values are true; returns Visible otherwise.
  /// </summary>
  /// <remarks>
  /// If there are no values, Convert will return Visible.
  /// The multi-value version of ConvertBack is not supported.
  /// </remarks>
  public class BooleanToVisibilityMultiNegatedConverter : IMultiValueConverter, IValueConverter
  {
    #region IMultiValueConverter Members
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      if (values.Length == 0) return Visibility.Visible;
      foreach (object value in values)
      {
        bool flag = false;
        if (value is bool)
          flag = (bool)value;
        else if (value is bool?)
        {
          bool? nullable = (bool?)value;
          flag = nullable.HasValue ? nullable.Value : false;
        }
        if (!flag)
          return Visibility.Visible;
      }
      return Visibility.Collapsed;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
    #endregion

    #region IValueConverter Members
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      bool flag = false;
      if (value is bool)
      {
        flag = (bool)value;
      }
      else if (value is bool?)
      {
        bool? nullable = (bool?)value;
        flag = nullable.HasValue ? nullable.Value : false;
      }
      return (flag ? Visibility.Collapsed : Visibility.Visible);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return ((value is Visibility) && (((Visibility)value) != Visibility.Visible));
    }
    #endregion
  }

  /// <summary>
  /// Converts a number to a string, with the exception that 0 is the empty string.
  /// </summary>
  public class NumberToStringNoZeroesConverter : IValueConverter
  {
    public static bool IsZero(object obj)
    {
      if (obj is double) return ((double)obj) == 0.0;
      if (obj is float) return ((float)obj) == 0.0f;
      if (obj is decimal) return ((decimal)obj) == 0m;
      if (obj is int) return ((int)obj) == 0;
      if (obj is short) return ((short)obj) == 0;
      if (obj is long) return ((long)obj) == 0;
      if (obj is byte) return ((byte)obj) == 0;
      if (obj is uint) return ((uint)obj) == 0;
      if (obj is ushort) return ((ushort)obj) == 0;
      if (obj is ulong) return ((ulong)obj) == 0;
      if (obj is sbyte) return ((sbyte)obj) == 0;
      if (obj is char) return ((char)obj) == 0;
      return false;
    }

    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      int decimals = 1;
      if (value == null) return "";
      if (IsZero(value)) return "";
      if (parameter is int)
        decimals = System.Convert.ToInt32(decimals);
      return Math.Round(System.Convert.ToDouble(value), decimals).ToString();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotSupportedException();
    }

    #endregion
  }

  /// <summary>
  /// Converts a number to a string.
  /// Optionally, allows for a limited number of decimal places (default is 1).
  /// </summary>
  public class NumberToStringConverter : IValueConverter
  {
    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      int decimals = 1;
      if (value == null) return "";
      if (parameter is int)
        decimals = System.Convert.ToInt32(decimals);
      if (parameter is string)
      {
        int d;
        if (int.TryParse((string)parameter, out d))
          decimals = d;
      }
      try
      {
        var d = System.Convert.ToDouble(value);
        if (double.IsNaN(d))
          return "";
        return Math.Round(d, decimals).ToString();
      }
      catch
      {
        return value.ToString();
      }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotSupportedException();
    }

    #endregion
  }

  public class NumberToVisibilityConverter : IValueConverter
  {
    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value == null) return Visibility.Collapsed;
      if (NumberToStringNoZeroesConverter.IsZero(value)) return Visibility.Collapsed;
      return Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotSupportedException();
    }

    #endregion
  }

  public class StringToTimeSpanConverter : IValueConverter
  {
    public static TimeSpan? ParseHoursAndMinutes(string text)
    {
      double d;
      if (double.TryParse(text, out d))
        return TimeSpan.FromMinutes(d);
      else if (text.Contains(':'))
      {
        int colon = text.IndexOf(':');
        if (text.LastIndexOf(':') != colon)
          return null;
        double h = 0, m = 0;
        double.TryParse(text.Substring(0, colon), out h);
        if (colon < text.Length - 1)
          double.TryParse(text.Substring(colon + 1), out m); // 00:
        return TimeSpan.Zero.Add(TimeSpan.FromHours(h)).Add(TimeSpan.FromMinutes(m));
      }
      return null;
    }

    public static string TimeSpanToString(TimeSpan ts, int minuteDecimalPlaces)
    {
      int hours = (ts.Days * 24) + ts.Hours;
      decimal minutes = Math.Round(ts.Minutes + (ts.Seconds / 60m));
      if (minutes >= 60m)
      {
        minutes -= 60m;
        hours++;
      }
      return string.Format("{0}:{1}", hours, minutes.ToString("#00"));
    }

    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is TimeSpan)
        return (TimeSpan)value;
      if (value == null)
        return TimeSpan.Zero;
      var ts = ParseHoursAndMinutes(value.ToString());
      if (ts.HasValue)
        return ts.Value;
      else
        return TimeSpan.Zero;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value == null)
        return null;
      return value.ToString();
    }

    #endregion
  }

  public class TimeSpanToStringConverter : IValueConverter
  {
    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value == null) return "";
      int decimals = 0;
      if (parameter is int)
        decimals = System.Convert.ToInt32(decimals);
      if (parameter is string)
      {
        int d;
        if (int.TryParse((string)parameter, out d))
          decimals = d;
      }
      if (value is TimeSpan)
        return StringToTimeSpanConverter.TimeSpanToString((TimeSpan)value, decimals);
      else
      {
        var ts = StringToTimeSpanConverter.ParseHoursAndMinutes(value.ToString());
        if (!ts.HasValue)
          return null;
        return StringToTimeSpanConverter.TimeSpanToString(ts.Value, decimals);
      }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value == null) return null;
      var ts = StringToTimeSpanConverter.ParseHoursAndMinutes(value.ToString());
      if (ts.HasValue)
        return ts.Value;
      else
        return null;
    }

    #endregion
  }

  public class EnumToBooleanConverter : IValueConverter
  {
    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value == null || parameter == null)
        return false;
      return value.Equals(parameter);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value == null)
        return 0;
      if (value is bool && ((bool)value))
        return parameter;
      if (value is bool? && ((bool?)value) == true)
        return parameter;
      return 0;
    }

    #endregion
  }

  public class BooleanAndMultiConverter : IMultiValueConverter
  {
    #region IMultiValueConverter Members

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      bool result = true;
      foreach (var value in values)
      {
        if (value is bool)
          result = (result) && ((bool)value);
        else if (value is bool?)
          result = (result) && ((bool?)value) == true;
        else
          result = false;
      }
      return result;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    #endregion
  }
}
