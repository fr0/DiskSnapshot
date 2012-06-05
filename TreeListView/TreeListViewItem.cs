using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;

namespace DiskSnapshot
{
  public class TreeListViewItem : TreeViewItem
  {
    static TreeListViewItem()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeListViewItem), new FrameworkPropertyMetadata(typeof(TreeListViewItem)));
    }

    public static readonly RoutedEvent IsMultiSelectedChangedEvent = EventManager.RegisterRoutedEvent("IsMultiSelectedChanged", RoutingStrategy.Bubble,
      typeof(TreeListViewItemSelectionEventHandler), typeof(TreeListViewItem));
    public event TreeListViewItemSelectionEventHandler IsMultiSelectedChanged
    {
      add { AddHandler(IsMultiSelectedChangedEvent, value); }
      remove { RemoveHandler(IsMultiSelectedChangedEvent, value); }
    }

    public static readonly DependencyProperty IsMultiSelectedProperty =
        DependencyProperty.Register("IsMultiSelected", typeof(bool), typeof(TreeListViewItem), new UIPropertyMetadata(false, HandleIsMultiSelectedChanged));
    public bool IsMultiSelected
    {
      get { return (bool)GetValue(IsMultiSelectedProperty); }
      set { SetValue(IsMultiSelectedProperty, value); }
    }
    static void HandleIsMultiSelectedChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
      ((TreeListViewItem)sender).HandleIsMultiSelectedChanged(e);
    }
    void HandleIsMultiSelectedChanged(DependencyPropertyChangedEventArgs e)
    {
      RaiseEvent(new TreeListViewItemSelectionEventArgs(IsMultiSelectedChangedEvent, this, IsMultiSelected));
    }

    /// <summary>
    /// Item's hierarchy in the tree
    /// </summary>
    public int Level
    {
      get
      {
        if (_level == -1)
        {
          TreeListViewItem parent = ItemsControl.ItemsControlFromItemContainer(this) as TreeListViewItem;
          _level = (parent != null) ? parent.Level + 1 : 0;
        }
        return _level;
      }
    }

    TreeListView _ParentTreeListView = null;
    protected TreeListView ParentTreeListView
    {
      get
      {
        if (_ParentTreeListView == null)
         _ParentTreeListView = WPFUtils.FindVisualAncestor<TreeListView>(this);
        return _ParentTreeListView;
      }
    }
    protected bool SupportsMultiSelect
    {
      get
      {
        var tv = ParentTreeListView;
        return tv != null && tv.AllowMultiSelect;
      }
    }

    protected override DependencyObject GetContainerForItemOverride()
    {
      return new TreeListViewItem();
    }

    protected override bool IsItemItsOwnContainerOverride(object item)
    {
      return item is TreeListViewItem;
    }

    protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
      base.OnItemsChanged(e);
      RaiseEvent(new RoutedEventArgs(TreeListView.TreeItemsChangedEvent, this));
    }

    protected override void OnSelected(RoutedEventArgs e)
    {
      IsMultiSelected = true;
      var parent = ParentTreeListView;
      if (parent != null && parent.AllowMultiSelect && !parent.InMultiSelect)
        parent.SelectionStart = this;
      base.OnSelected(e);
      Focus();      
    }
    protected override void OnUnselected(RoutedEventArgs e)
    {
      if (ParentTreeListView == null || (!ParentTreeListView.InMultiSelect && ParentTreeListView.SelectSource != SelectionSource.Mouse))
        IsMultiSelected = false;
      base.OnUnselected(e);
      if (ParentTreeListView != null && ParentTreeListView.SelectSource == SelectionSource.Unknown)
        ParentTreeListView.CleanupMultiSelects();
    }
    
    protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
    {
      bool wasMultiSelected = IsMultiSelected;
      bool multiSelect = SupportsMultiSelect && (IsShiftPressed || IsCtrlPressed) && ParentTreeListView != null;
      if (multiSelect)
        ParentTreeListView.BeginMultiSelect();
      if (ParentTreeListView != null)
        ParentTreeListView.SelectSource = SelectionSource.Mouse;
      base.OnMouseLeftButtonDown(e);
      if (multiSelect)
      {
        if (IsShiftPressed)
          ParentTreeListView.MultiSelectTo(this);
        ParentTreeListView.EndMultiSelect();
      }
      if (!multiSelect && !wasMultiSelected)
        ParentTreeListView.CleanupMultiSelects();
      if (ParentTreeListView != null)
        ParentTreeListView.SelectSource = SelectionSource.Unknown;
    }

    protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
    {
      base.OnMouseLeftButtonUp(e);
      if (!IsShiftPressed && !IsCtrlPressed && ParentTreeListView != null)
        ParentTreeListView.CleanupMultiSelects();
    }

    protected override void OnPreviewMouseRightButtonDown(MouseButtonEventArgs e)
    {
      Focus();
      base.OnPreviewMouseRightButtonDown(e);
    }

    protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e)
    {
      if (ParentTreeListView != null)
        ParentTreeListView.SelectSource = SelectionSource.Keyboard;
      bool wasSelected = IsSelected;
      base.OnKeyDown(e);
      if (wasSelected && !IsSelected)
      {
        if (SupportsMultiSelect && IsShiftPressed)
          IsMultiSelected = true;
        else
          ParentTreeListView.CleanupMultiSelects();
      }
      if (ParentTreeListView != null)
        ParentTreeListView.SelectSource = SelectionSource.Unknown;
    }

    private int _level = -1;

    static bool IsShiftPressed
    {
      get
      {
        return Keyboard.IsKeyDown(Key.LeftShift)
            || Keyboard.IsKeyDown(Key.RightShift);
      }
    }
    static bool IsCtrlPressed
    {
      get
      {
        return Keyboard.IsKeyDown(Key.LeftCtrl)
            || Keyboard.IsKeyDown(Key.RightCtrl);
      }
    }

  }

  public delegate void TreeListViewItemSelectionEventHandler(object sender, TreeListViewItemSelectionEventArgs e);
  public class TreeListViewItemSelectionEventArgs : RoutedEventArgs
  {
    public TreeListViewItemSelectionEventArgs(RoutedEvent ev, object source, bool selected)
      : base(ev, source)
    {
      this.Selected = selected;
    }

    public bool Selected { get; private set; }
  }

  /// <summary>
  /// Exposes attached behaviors that can be
  /// applied to TreeViewItem objects.
  /// </summary>
  public static class TreeViewItemBehavior
  {
    #region IsBroughtIntoViewWhenSelected

    public static bool GetIsBroughtIntoViewWhenSelected(TreeViewItem treeViewItem)
    {
      return (bool)treeViewItem.GetValue(IsBroughtIntoViewWhenSelectedProperty);
    }

    public static void SetIsBroughtIntoViewWhenSelected(
      TreeViewItem treeViewItem, bool value)
    {
      treeViewItem.SetValue(IsBroughtIntoViewWhenSelectedProperty, value);
    }

    public static readonly DependencyProperty IsBroughtIntoViewWhenSelectedProperty =
        DependencyProperty.RegisterAttached(
        "IsBroughtIntoViewWhenSelected",
        typeof(bool),
        typeof(TreeViewItemBehavior),
        new UIPropertyMetadata(false, OnIsBroughtIntoViewWhenSelectedChanged));

    static void OnIsBroughtIntoViewWhenSelectedChanged(
      DependencyObject depObj, DependencyPropertyChangedEventArgs e)
    {
      TreeViewItem item = depObj as TreeViewItem;
      if (item == null)
        return;

      if (e.NewValue is bool == false)
        return;

      if ((bool)e.NewValue)
        item.Selected += OnTreeViewItemSelected;
      else
        item.Selected -= OnTreeViewItemSelected;
    }

    static void OnTreeViewItemSelected(object sender, RoutedEventArgs e)
    {
      // Only react to the Selected event raised by the TreeViewItem
      // whose IsSelected property was modified. Ignore all ancestors
      // who are merely reporting that a descendant's Selected fired.
      if (!Object.ReferenceEquals(sender, e.OriginalSource))
        return;

      TreeViewItem item = e.OriginalSource as TreeViewItem;
      if (item != null)
        item.BringIntoView();
    }

    #endregion // IsBroughtIntoViewWhenSelected
  }
}
