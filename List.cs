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
    /// <summary> Returns an empty enumeration </summary>
    public static IEnumerable<T> Empty<T>() { yield break; }

    /// <summary> Creates a list with the given items </summary>
    public static List<T> Create<T>(params T[] items)
    {
      return new List<T>(items);
    }
    /// <summary> Creates a list with just the given item in it. </summary>
    public static List<T> Create<T>(T item)
    {
      return new List<T>() { item };
    }

    /// <summary> Returns an enumeration that includes first the given list, and then each item from itemsToAdd </summary>
    public static IEnumerable<T> ConcatItems<T>(this IEnumerable<T> list, params T[] itemsToAdd)
    {
      return Enumerable.Concat(list, itemsToAdd);
    }

    /// <summary> Returns an enumeration that includes 'item' first, followed by each item from 'list' </summary>
    public static IEnumerable<T> Concat<T>(T item, IEnumerable<T> list)
    {
      yield return item;
      foreach(T t in list)
        yield return t;
    }

    /// <summary> Returns an enumeration that includes the items from each of the lists, in order (all the elements
    /// from list 1 in order, all the elements from list 2 in order and so on). </summary>
    public static IEnumerable<T> ConcatLists<T>(params IEnumerable<T>[] lists)
    {
      foreach(var ls in lists)
        foreach(T t in ls)
          yield return t;
    }

    /// <summary> Equivalent to calling new List&lt;T&gt;(items) </summary>
    public static List<T> Copy<T>(this IEnumerable<T> items)
    {
      return new List<T>(items);
    }

    /// <summary> Returns a new list with each item from 'items' in order. </summary>
    public static List<object> Copy(IEnumerable items)
    {
      var result = new List<object>();
      foreach(object o in items)
        result.Add(o);
      return result;
    }

    /// <summary> Returns the first object in ls that when passed to pred yields true.  If no item matches, null is returned. </summary>
    public static object FindUntyped(IEnumerable ls, Func<object, bool> pred)
    {
      foreach(object t in ls)
        if(pred(t))
          return t;
      return null;
    }

    /// <summary> Returns the first element of the sequence that satisfies a condition or a default value if no such element is found. </summary>
    public static T Find<T>(this IEnumerable<T> ls, Func<T, bool> pred)
    {
      return ls.FirstOrDefault(pred);
    }

    /// <summary> The items in ls are passed one by one to 'pred'.  When 'pred' returns true, true if returned from this function. 
    /// If 'pred' never returns true, false is returned from this function. </summary>
    public static bool Contains<T>(this IEnumerable<T> ls, Func<T, bool> pred)
    {
      return ls.Any(pred);
    }

    /// <summary> Returns the 0 based index into ls of the first item that, when passed to 'pred', yields a true value.  
    /// If no item in ls yields true from 'pred', -1 is returned. </summary>
    public static int IndexOf(IEnumerable ls, Func<object, bool> pred)
    {
      int i = 0;
      foreach(var t in ls)
        if(pred(t))
          return i;
        else
          i++;
      return -1;
    }

    /// <summary> Returns the 0 based index into ls of the first item that, when passed to 'pred', yields a true value.  
    /// If no item in ls yields true from 'pred', -1 is returned. </summary>
    public static int IndexOf<T>(this IEnumerable<T> ls, Func<T, bool> pred)
    {
      return IndexOf<T>(ls, pred, 0);
    }

    /// <summary> Returns the 0 based index into ls of the first item at an index of 'start' or greater that, 
    /// when passed to 'pred', yields a true value.  If no item in ls at or after 'start' yields true from 'pred', -1 is returned. </summary>
    /// <param name="ls"> The list to search. </param>
    /// <param name="pred"> The predicate to test items against. </param>
    /// <param name="start"> The index to start at.  If 'start' is negative, the entire list is searched.  If 'start' is equal
    /// to or greater than the size of 'ls', -1 is returned. </param>
    public static int IndexOf<T>(this IEnumerable<T> ls, Func<T, bool> pred, int start)
    {
      int i = start < 0 ? 0 : start;
      foreach(T t in ls.Skip(start))
        if(pred(t))
          return i;
        else
          i++;
      return -1;
    }

    /// <summary> Evaluates 'p' for each item in ls.  The result of each evaluation is passed to the next.  The first evaluation is
    /// passed 'init'.  The result of the last evaluation is returned.  If ls is empty, 'init' is returned. </summary>
    public static R Fold<T, R>(this IEnumerable<T> ls, R init, Func<T, R, R> p)
    {
      R accum = init;
      foreach(T t in ls)
        accum = p(t, accum);
      return accum;
    }

    /// <summary> Forms a new list where the items in the new list are the result of calling 'p' on the items in 'ls', in order. </summary>
    public static List<R> OMap<R>(IEnumerable ls, Func<object, R> p)
    {
      var result = new List<R>();
      foreach(object t in ls)
        result.Add(p(t));
      return result;
    }

    /// <summary> Forms a new list where the items in the new list are the result of calling 'p' on the items in 'ls', in order. </summary>
    public static List<R> ToList<T, R>(this IEnumerable<T> ls, Func<T, R> p)
    {
      return ls.Select(p).ToList();
    }

    /// <summary> Forms a new list where the items in the new list are the result of calling 'p' on the items in 'ls', in order,
    /// passing the zero based index of the item as the second arg. </summary>
    public static List<R> ToList<T, R>(this IEnumerable<T> ls, Func<T, int, R> p)
    {
      return ls.Select(p).ToList();
    }

    /// <summary> 
    /// The items from 'als' and 'bls' are passed to 'p', pairwise and in order.  If false is returned from 'p' or
    /// 'als' is a different length than 'bls', false is returned from this function.  Returns true if
    /// the given lists are both empty.
    /// </summary>
    public static bool AndMap<A, B>(IEnumerable<A> als, IEnumerable<B> bls, Func<A, B, bool> p)
    {
      var a = als.GetEnumerator();
      var b = bls.GetEnumerator();
      while(true)
      {
        var success = a.MoveNext();
        if(success != b.MoveNext()) return false;
        if(!success) return true;
        if(!p(a.Current, b.Current)) return false;
      }
    }

    /// <summary> Creates a string by concatenating the result of calling ToString on each item in ls, in order, with
    /// 'sep' inserted between each pair.  For example, List.Join(new[] {1, 2, 3}, ",") == "1,2,3".  Returns an empty
    /// string if ls is empty.</summary>
    public static string Join<T>(this IEnumerable<T> ls, string sep)
    {
      return Join(ls, sep, x => x.ToString());
    }

    /// <summary> Creates a string by concatenating the result of calling converter on each item in ls, in order, with
    /// 'sep' inserted between each pair.  For example, List.Join(new[] {1, 2, 3}, ",", x => (x+1).ToString()) == "2,3,4".  
    /// Returns an empty string if ls is empty.</summary>
    public static string Join<T>(this IEnumerable<T> ls, string sep, Func<T, string> converter)
    {
      StringBuilder sb = new StringBuilder();
      Fold(ls, "", delegate(T x, string nextPrefix) {
        sb.Append(nextPrefix);
        sb.Append(converter(x));
        return sep;
      });
      return sb.ToString();
    }

    /// <summary> Returns an enumeration containing all items of 'ls' for which 'p' returns true, in order.  Evaluation is lazy. </summary>
    public static IEnumerable<T> Filter<T>(this IEnumerable<T> ls, Predicate<T> p)
    {
      foreach(T t in ls)
        if(p(t))
          yield return t;
    }

    /// <summary> Returns a list containing all items of 'ls' for which 'p' returns true, in order.  </summary>
    public static List<T> FilterList<T>(this IEnumerable<T> ls, Func<T, bool> p)
    {
      return ls.Where(p).ToList();
    }
 
    /// <summary> Returns a list consisting of each item in order from ls, cast to T.  If any item is not a T, an exception is thrown. </summary>
    public static List<T> Cast<T>(this IEnumerable ls)
    {
      return ls.LazyCast<T>().ToList();
    }

    /// <summary> Returns an enumeration consisting of each item in order from ls, cast to T.  
    /// If any item is not a T, an exception is thrown.  Evaluation is lazy. </summary>
    public static IEnumerable<T> LazyCast<T>(this IEnumerable ls)
    {
      foreach(object x in ls)
        yield return (T)x;
    }

    /// <summary> Returns an enumeration consisting of the results of passing each item in 'ls' to 'selector' and discarding
    /// any that 'selector' returns null for. </summary>
    public static IEnumerable<R> SelectNotNull<T, R>(this IEnumerable<T> ls, Func<T, R> selector) where R : class
    {
      foreach(var i in ls)
      {
        var r = selector(i);
        if(r != null)
          yield return r;
      }
    }

    /// <summary> Returns true if 'a' and 'b' are the same size, and IComparable.CompareTo returns zero for each pair
    /// of items from a and b, in order.  Returns true if a and b are empty. </summary>
    public static bool SameContents<T>(IList<T> a, IList<T> b) where T : IComparable
    {
      return AndMap<T, T>(a, b, (x, y) => x.CompareTo(y) == 0);
    }

    /// <summary> Returns true if 'a' and 'b' are the same size, and 'areTheSame' returns true for each pair
    /// of items from a and b, in order. </summary>
    public static bool SameContents<T>(IEnumerable<T> a, IEnumerable<T> b, Func<T, T, bool> areTheSame)
    {
      return AndMap<T, T>(a, b, areTheSame);
    }

    /// <summary> Returns true if 'a' and 'b' are the same size, and for every item in 'a', b.Contains returns true for that object.
    /// Returns false otherwise.  May not perform correctly if items in either list are repeated. </summary>
    public static bool ShallowSetEquals(IList a, IList b)
    {
      if(a.Count != b.Count) return false;
      for(int i = 0; i < a.Count; i++)
      {
        if(!b.Contains(a[i])) return false;
      }
      return true;
    }

    /// <summary> Returns true if 'a' and 'b' are the same size, and for every item in 'a', 'cmp' returns true
    /// for at least one item in 'b'.  Returns false otherwise.  May not operate as expected if items in the lists
    /// are repeated. </summary>
    public static bool SetEquals<A, B>(List<A> a, List<B> b, Func<A, B, bool> cmp)
    {
      return a.Count == b.Count && a.All(x => b.Contains(y => cmp(x, y)));
    }

    /// <summary> Removes each item from 'list' that evaluates to true when passed to pred. </summary>
    /// <returns> Returns the number of items removed. </returns>
    public static int Remove<T>(this IList<T> list, Func<T, bool> pred)
    {
      var toRemove = list.FilterList(pred);  
      foreach(var t in toRemove)
        list.Remove(t);  
      return toRemove.Count; 
    }

    /// <summary>
    /// Returns an enumeration over the input list, only including the items starting with 'startIndex'
    /// and ending with 'endIndex' (inclusive).  If startIndex is greater than endIndex, an empty
    /// enumeration is returned;  otherwise, if 'startIndex' or 'endIndex' are out of range for the given
    /// list, an ArgumentOutOfRangeException exception is thrown.
    /// </summary>
    public static IEnumerable<T> Slice<T>(this IList<T> source, int startIndex, int endIndex)
    {
      if(startIndex > endIndex) return List.Empty<T>();
      if(startIndex < 0) throw new ArgumentOutOfRangeException("startIndex");
      if(endIndex >= source.Count) throw new ArgumentOutOfRangeException("endIndex");
      return source.Skip(startIndex).Take(endIndex - startIndex + 1);
    }

    /// <summary>
    /// Returns the first non-zero value in the given array.  If all values are zero, returns zero.
    /// </summary>
    /// <remarks>
    /// Useful for doing a series of comparisons, where you want to return the result of the first
    /// comparison that finds non-equal values.  For example:
    ///   <code>
    ///     int c1 = foo.CompareTo(bar);
    ///     if (c1 != 0) return c1;
    ///     int c2 = blah.CompareTo(baz);
    ///     if (c2 != 0) return c2;
    ///     // ...
    /// 
    ///     // This can be replaced with:
    ///     FirstNonZero(
    ///             foo.CompareTo(bar), 
    ///             blah.CompareTo(baz)
    ///             // ...
    ///            );
    ///   </code>
    /// </remarks>
    public static int FirstNonZero(params int[] values)
    {
      return values.FirstOrDefault(v => v != 0);
    }

    /// <summary> Runs 'p' on each item in ls, in order. </summary>
    public static void ForEach<T>(this IEnumerable<T> ls, Action<T> p)
    {
      foreach(T t in ls) p(t);
    }

    /// <summary> Runs 'p' on each item in ls, in order.  Passes the zero based index of the item along with the item
    /// itself to 'p'.</summary>
    public static void ForEach<T>(this IEnumerable<T> ls, Action<int, T> p)
    {
      int i = 0;
      foreach(T t in ls) p(i++, t);
    }

    /// <summary> Runs 'p' on each pair of items from 'als' and 'bls', pairwise, in order.  Throws an ArgumentException
    /// if the lists are different sizes.</summary>
    public static void ForEach<A, B>(IEnumerable<A> als, IEnumerable<B> bls, Action<A, B> p)
    {
      var a = als.GetEnumerator();
      var b = bls.GetEnumerator();
      while(true)
      {
        var success = a.MoveNext();
        if(success != b.MoveNext()) throw new ArgumentException("enumerations differ in length");
        if(!success) break;
        p(a.Current, b.Current);
      }
    }

    /// <summary> Returns an enumeration consisting of 'count' successive integers starting from 0. </summary>
    public static IEnumerable<int> IntsFromZero(int count)
    {
      return Enumerable.Range(0, count);
    }

    /// <summary> Executes 'p' 'times' times. </summary>
    public static void Repeat(int times, Action p)
    {
      for(int i = 0; i < times; i++)
        p();
    }

    /// <summary> Executes 'p' 'times' times, passing succesive integers starting at 0. </summary>
    public static void Repeat(int times, Action<int> p)
    {
      for(int i = 0; i < times; i++)
        p(i);
    }

    /// <summary> Returns a list consisting of the result of calling 'p' 'times' times, passing 
    /// it succesive integers starting at 0. </summary>
    public static List<T> Repeat<T>(int times, Func<int, T> p)
    {
      List<T> r = new List<T>(times);
      for(int i = 0; i < times; i++)
        r.Add(p(i));
      return r;
    }

    /// <summary> Breaks the list into a list of lists of up the given size;  the last list may be smaller.  If size is zero or one,
    /// each item in ls is returned in its own list.  Uses lazy evaluation. </summary>
    public static IEnumerable<List<T>> Subdivide<T>(this IEnumerable<T> ls, int size)
    {
      List<T> cur = null;
      foreach(var item in ls)
      {
        if(cur == null) cur = new List<T>();
        cur.Add(item);
        if(cur.Count >= size)
        {
          yield return cur;
          cur = null;
        }
      }
      if(cur != null)
        yield return cur;
    }


    /// <summary> Returns from if any item in 'set' is equal (per the virtual Equals method on the item from 'set') to 'item'. </summary>
    public static bool OneOf<T>(T item, params T[] set) where T : struct
    {
      return set.Any(i => i.Equals(item));
    }

    /// <summary> Returns a new list that has been sorted using the given comparison function. </summary>
    public static List<T> Sort<T>(this IEnumerable<T> ls, Comparison<T> cmp)
    {
      List<T> result = new List<T>(ls);
      result.Sort(cmp);
      return result;
    }

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

    /// <summary>
    /// A more efficient way of writing:  <code>items.Count() == expectedCount</code>
    /// </summary>
    /// <remarks>
    /// Guaranteed not to iterate through more than expectedCount+1 items
    /// (Count() will iterate through the entire enumeration).
    /// </remarks>
    public static bool CountIs<T>(this IEnumerable<T> items, int expectedCount)
    {
      int index = 0;
      var enumer = items.GetEnumerator();
      while(enumer.MoveNext())
      {
        index++;
        if(index > expectedCount)
          return false;
      }
      return index == expectedCount;
    }

    /// <summary> Runs the Enumerable.Distinct function on the given list, using the given equals and hashcode functions
    /// to implement IEquatable. </summary>
    public static IEnumerable<T> Distinct<T>(this IEnumerable<T> ls, Func<T, T, bool> equals, Func<T, int> hashCode)
    {
      return Enumerable.Distinct(ls, new FuncComparer<T>(equals, hashCode));
    }

    class FuncComparer<T> : IEqualityComparer<T>
    {
      Func<T, T, bool> _Equals;
      Func<T, int> _HashCode;
      public FuncComparer(Func<T, T, bool> equals, Func<T, int> hashCode)
      {
        _Equals = equals;
        _HashCode = hashCode;
      }
      public bool Equals(T x, T y)
      {
        return _Equals(x, y);
      }

      public int GetHashCode(T obj)
      {
        return _HashCode(obj);
      }
    }

    /// <summary> Returns all the values of the given enum type.  Throws if T is not an enum type. </summary>
    /// <remarks> Does not return values that have the [NotReleased] attribute (unless we're in DeveloperMode). </remarks>
    public static IEnumerable<T> EnumValues<T>()
    {
      return EnumValues(typeof(T)).LazyCast<T>();
    }
    /// <summary> Returns all the values of the given enum type.  Throws if t is not an enum type. </summary>
    /// <remarks> Does not return values that have the [NotReleased] attribute (unless we're in DeveloperMode). </remarks>
    public static IEnumerable EnumValues(Type t)
    {
      foreach (var val in Enum.GetValues(t))
        yield return val;
    }

    /// <summary>
    /// Returns a comparison for sorting on multiple keys, where key comparisons are passed in decreasing priority order,
    /// i.e., primary key, secondary key, etc.
    /// </summary>
    public static Comparison<T> Compare<T>(params Comparison<T>[] comparisons)
    {
      return (a, b) => comparisons.Select(compare => compare(a, b)).FirstOrDefault(v => v != 0);
    }

    /// <summary>
    /// Uses the predicate supplied to partitions elements of the list into two lists, the elements for which the predicate returns true and the elements for which it returns false,
    /// and returns the result of processing those two lists with the consumer function supplied. Order is preserved within the partitioned lists.
    /// </summary>
    /// <typeparam name="T">Type of elements in the input list and the two partitioned lists</typeparam>
    /// <typeparam name="R">Return type</typeparam>
    /// <param name="ls">List of elements to be partitioned according to the predicate</param>
    /// <param name="pred">Predicate used to partition the input list</param>
    /// <param name="consume">Consumer that takes the list of elements that pass and the list of elements that fail and produces the return value for Partition.</param>
    /// <returns></returns>
    public static R Partition<T, R>(this IEnumerable<T> ls, Func<T, bool> pred, Func<List<T>, List<T>, R> consume)
    {
      var yes = List.Create<T>();
      var no = List.Create<T>();
      ls.ForEach(elt => (pred(elt) ? yes : no).Add(elt));
      return consume(yes, no);
    }
  }
}
