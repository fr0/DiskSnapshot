using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Linq;

namespace DiskSnapshot
{
  /// <summary> Methods for list manipulation </summary>
  public static class List
  {
    /// <summary>
    /// Adds the item to the correct spot in 'items' base on the given comparison, assuming that 'items' is already
    /// sorted according to 'cmp'.
    /// </summary>
    public static void AddSorted<T>(this IList<T> items, T item, Comparison<T> cmp)
    {
      int index = BinarySearch(items, x => cmp(x, item));
      if(index >= 0)
        items.Insert(index, item);
      else
        items.Insert(~index, item);
    }

    /// <summary> Searches an entire list for a specific element, using the given comparer. </summary>
    /// <returns> The index of the specified value in the specified array, if value is found. If value is 
    /// not found and value is less than one or more elements in array, a negative number which is the bitwise 
    /// complement of the index of the first element that is larger than value. If value is not found and value 
    /// is greater than any of the elements in array, a negative number which is the bitwise complement of (the index of the 
    /// last element plus 1).</returns>
    static int BinarySearch<T>(this IList<T> list, Func<T,int> comparer)
    {
      int min = 0;
      int max = list.Count - 1;

      while(min <= max)
      {
        int mid = (min + max) / 2;
        int comparison = comparer(list[mid]);
        if(comparison == 0)
          return mid;

        if(comparison < 0)
          min = mid + 1;
        else if(comparison > 0)
          max = mid - 1;
      }
      return ~min;
    }
  }
}
