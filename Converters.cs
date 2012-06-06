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
}
