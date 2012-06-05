using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace DiskSnapshot
{
  public static class WPFUtils
  {
    public static TChildItem FindVisualChild<TChildItem>(DependencyObject obj) where TChildItem : DependencyObject
    {
      if (obj != null)
      {
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
        {
          DependencyObject child = VisualTreeHelper.GetChild(obj, i);
          if (child != null && child is TChildItem)
            return (TChildItem)child;
          else
          {
            TChildItem childOfChild = FindVisualChild<TChildItem>(child);
            if (childOfChild != null)
              return childOfChild;
          }
        }

      }
      return null;
    }

    public static IEnumerable<TChildItem> FindVisualChildren<TChildItem>(DependencyObject obj, Predicate<TChildItem> filter)
      where TChildItem : DependencyObject
    {
      if (obj != null)
      {
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
        {
          DependencyObject child = VisualTreeHelper.GetChild(obj, i);
          if (child != null && child is TChildItem && filter((TChildItem)child))
            yield return (TChildItem)child;
          foreach (TChildItem childOfChild in FindVisualChildren<TChildItem>(child, filter))
            yield return childOfChild;
        }
      }
    }

    /// <summary>
    /// Returns the first object of the given type in the given element's visual tree ancestor chain.
    /// (E.g. walks via VisualTreeHelper.GetParent).  Does include the element, so if typeof(element) == typeof(T),
    /// this will return element.
    /// </summary>
    /// <param name="element">The starting point for the search</param>
    /// <param name="highestAncestorToLook">If we encounter this object, we'll stop looking.</param>
    /// <returns>The first visual ancestor of the given type, or null</returns>
    public static T GetAncestorByType<T>(DependencyObject element, DependencyObject highestAncestorToLook)
      where T : DependencyObject
    {
      if (element == null) return null;
      if (element is T) return (T)element;
      if (element == highestAncestorToLook) return null;
      return GetAncestorByType<T>(VisualTreeHelper.GetParent(element), highestAncestorToLook);
    }

    public static T MakeFrozen<T>(this T freezable)
      where T : Freezable
    {
      if (freezable.CanFreeze)
        freezable.Freeze();
      return freezable;
    }

    /// <summary>
    /// Finds the first parent in the element's logical tree that is of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of parent to find.</typeparam>
    /// <param name="control">The descendent element.</param>
    /// <returns>The parent element, or null.</returns>
    public static T FindLogicalAncestor<T>(DependencyObject control)
    {
      DependencyObject parent = LogicalTreeHelper.GetParent(control);
      if (parent is T)
        return (T)((object)parent);
      if (parent == null)
        return default(T);
      return FindLogicalAncestor<T>(parent);
    }

    /// <summary>
    /// Finds the first parent in the element's visual tree that is of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of parent to find.</typeparam>
    /// <param name="control">The descendent element.</param>
    /// <returns>The parent element, or null.</returns>
    public static T FindVisualAncestor<T>(DependencyObject control)
    {
      DependencyObject parent = VisualTreeHelper.GetParent(control);
      if (parent is T)
        return (T)((object)parent);
      if (parent == null)
        return default(T);
      return FindVisualAncestor<T>(parent);
    }

    /// <summary>
    /// Finds the first parent in the element's visual tree that is of the specified type, and
    /// matches the specified predicate.
    /// </summary>
    /// <typeparam name="T">The type of parent to find.</typeparam>
    /// <param name="control">The descendent element.</param>
    /// <param name="pred">The condition that the element must match.</param>
    /// <returns>The parent element, or null.</returns>
    public static T FindVisualAncestor<T>(DependencyObject control, Predicate<T> pred)
    {
      DependencyObject parent = VisualTreeHelper.GetParent(control);
      if (parent is T && pred((T)((object)parent)))
        return (T)((object)parent);
      if (parent == null)
        return default(T);
      return FindVisualAncestor<T>(parent, pred);
    }

    /// <summary>
    /// Runs the given action, once the given element is visible.  If the element is visible now,
    /// then the action will be called immediately.  This is a one-shot deal; it doesn't happen every
    /// time the element's visibility changes.
    /// </summary>
    /// <param name="element">The UI element whose visibility we'll check for.</param>
    /// <param name="action">The code to run on IsVisible=true.</param>
    public static void WhenVisible(UIElement element, Action action)
    {
      if (element.IsVisible)
        action();
      else
      {
        DependencyPropertyChangedEventHandler handler = null;
        handler = delegate
        {
          if (element.IsVisible)
          {
            element.IsVisibleChanged -= handler;
            action();
          }
        };
        element.IsVisibleChanged += handler;
      }
    }

    public static void WhenVisibleAndLoaded(FrameworkElement element, Action action)
    {
      WhenVisible(element, delegate
      {
        WhenLoaded(element, action);
      });
    }

    public static void WhenLoaded(FrameworkElement element, Action action)
    {
      if (element.IsLoaded)
        action();
      else
      {
        RoutedEventHandler handler = null;
        handler = delegate
        {
          element.Loaded -= handler;
          action();
        };
        element.Loaded += handler;
      }
    }
  }

  public static class FocusHelper
  {
    public static void Focus(UIElement element)
    {
      Focus(element, System.Windows.Threading.DispatcherPriority.Normal);
    }

    public static void Focus(UIElement element, System.Windows.Threading.DispatcherPriority priority)
    {
      Focus(element, priority, () => true);
    }

    public static void Focus(UIElement element, System.Windows.Threading.DispatcherPriority priority, Func<bool> shouldSetFocus)
    {
      Debug.Assert(element != null);
      element.Dispatcher.BeginInvoke(priority, (Action)(() =>
      {
        if (shouldSetFocus())
        {
          element.Focus();
          if (element.Focusable) Keyboard.Focus(element);
        }
      }));
    }

    #region FocusWhen attached property
    public static readonly DependencyProperty FocusWhenProperty =
      DependencyProperty.RegisterAttached("FocusWhen", typeof(bool), typeof(FocusHelper), new UIPropertyMetadata(new PropertyChangedCallback(FocusWhenChanged)));

    public static bool GetFocusWhen(DependencyObject obj)
    {
      return (bool)obj.GetValue(FocusWhenProperty);
    }

    public static void SetFocusWhen(DependencyObject obj, bool value)
    {
      obj.SetValue(FocusWhenProperty, value);
    }

    private static void FocusWhenChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      if ((bool)args.NewValue)
      {
        // Uses Render priority so that DataBinding we may depend on for IsEnabled or Focusable completes before we try to set focus.
        var element = obj as UIElement;
        if (element != null)
          Focus(element, System.Windows.Threading.DispatcherPriority.Render);
      }
    }
    #endregion
  }
}
