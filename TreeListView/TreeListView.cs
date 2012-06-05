using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;

namespace DiskSnapshot
{
  public class TreeListView : TreeView
  {
    static TreeListView()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeListView), new FrameworkPropertyMetadata(typeof(TreeListView)));
    }
    public TreeListView()
    {
      AddHandler(TreeListViewItem.IsMultiSelectedChangedEvent, new TreeListViewItemSelectionEventHandler(ItemMultiSelectChanged));
    }

    public static readonly RoutedEvent TreeItemsChangedEvent = EventManager.RegisterRoutedEvent("TreeItemsChanged", RoutingStrategy.Bubble,
      typeof(RoutedEventHandler), typeof(TreeListView));
    public event RoutedEventHandler TreeItemsChanged
    {
      add { AddHandler(TreeItemsChangedEvent, value); }
      remove { RemoveHandler(TreeItemsChangedEvent, value); }
    }

    public static readonly DependencyProperty AllowMultiSelectProperty =
        DependencyProperty.Register("AllowMultiSelect", typeof(bool), typeof(TreeListView), new UIPropertyMetadata(false));
    public bool AllowMultiSelect
    {
      get { return (bool)GetValue(AllowMultiSelectProperty); }
      set { SetValue(AllowMultiSelectProperty, value); }
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
      RaiseEvent(new RoutedEventArgs(TreeItemsChangedEvent, this));
    }

    protected override void OnSelectedItemChanged(RoutedPropertyChangedEventArgs<object> e)
    {
      base.OnSelectedItemChanged(e);
    }

    public event Action SelectionChanged;

    void ItemMultiSelectChanged(object sender, TreeListViewItemSelectionEventArgs e)
    {
      if (SelectionChanged != null)
        SelectionChanged();
    }

    internal TreeListViewItem SelectionStart { get; set; }

    internal bool InMultiSelect { get { return MultiSelectCounter > 0; } }
    int MultiSelectCounter = 0;
    internal void BeginMultiSelect()
    {
      MultiSelectCounter++;
    }
    internal void EndMultiSelect()
    {
      MultiSelectCounter--;
    }

    internal void CleanupMultiSelects()
    {
      foreach (var item in MultiSelects.ToList())
      {
        if (!item.IsSelected)
        {
          item.IsMultiSelected = false;
          if (item == SelectionStart)
            SelectionStart = null;
        }
      }
    }

    internal SelectionSource SelectSource = SelectionSource.Unknown;

    /// <summary>
    /// Selects everything between SelectionStart and the given item.
    /// </summary>
    internal void MultiSelectTo(TreeListViewItem to)
    {
      if (SelectionStart == null) return;
      var from = SelectionStart;
      bool inRange = false;
      foreach (var item in this.EnumerateItems(i => i.IsExpanded))
      {
        if (item == to || item == from)
        {
          if (inRange)
            break;
          else
            inRange = true;
        }
        else if (inRange)
          item.IsMultiSelected = true;
      }
      to.IsMultiSelected = true;
      from.IsMultiSelected = true;
    }

    public IEnumerable<TreeListViewItem> MultiSelects
    {
      get { return this.EnumerateItems().Where(i => i.IsMultiSelected); }
    }

    public IEnumerable<TreeListViewItem> SelectedItems
    {
      get { return MultiSelects; }
    }
    public System.Collections.IEnumerable SelectedValues
    {
      get { return MultiSelects.Where(i => i.DataContext != null).Select(i => i.DataContext); }
    }
    public System.Collections.IEnumerable AllValues
    {
      get { return this.EnumerateItems().Where(i => i.DataContext != null).Select(i => i.DataContext); }
    }

    #region Public Properties

    /// <summary> GridViewColumn List</summary>
    public GridViewColumnCollection Columns
    {
      get
      {
        if (_columns == null)
        {
          _columns = new GridViewColumnCollection();
        }

        return _columns;
      }
    }

    private GridViewColumnCollection _columns;

    #endregion
  }

  public static class TreeListViewUtils
  {
    public static IEnumerable<TreeListViewItem> EnumerateItems(this TreeListView tv)
    {
      return EnumerateItems(tv, item => true);
    }

    public static IEnumerable<TreeListViewItem> EnumerateItems(this TreeListView tv, Func<TreeListViewItem,bool> walkChildren)
    {
      foreach (var item in tv.Items)
      {
        var tvi = (TreeListViewItem)tv.ItemContainerGenerator.ContainerFromItem(item);
        if (tvi != null)
        {
          yield return tvi;
          if (walkChildren(tvi))
            foreach (var sub in EnumerateChildren(tvi, walkChildren))
              yield return sub;
        }
      }
    }
    public static IEnumerable<TreeListViewItem> EnumerateChildren(this TreeListViewItem item, Func<TreeListViewItem, bool> walkChildren)
    {
      foreach (var child in item.Items)
      {
        var tvi = (TreeListViewItem)item.ItemContainerGenerator.ContainerFromItem(child);
        if (tvi != null)
        {
          yield return tvi;
          if (walkChildren(tvi))
            foreach (var sub in EnumerateChildren(tvi, walkChildren))
              yield return sub;
        }
      }
    }
  }

  enum SelectionSource
  {
    Keyboard,
    Mouse,
    Unknown
  }
}
