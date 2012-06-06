using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace DiskSnapshot
{
  /// <summary>
  /// Compares two strings, taking numeric chunks into account.  For
  /// example, "Foo-2" will be less than "Foo-10";  case
  /// is considered if the strings are equivalent without regard to case.
  /// </summary>
  /// <returns>
  ///   Less than 0 if s1 is less than s2;
  ///   0 if s1 and s2 are equivalent;
  ///   Greater than 0 if s1 is greater than s2.
  /// </returns>
  public class LogicalStringComparer : IComparer<string>
  {
    // Jason P decided to create a singleton instance of this class for performance reasons.
    public static readonly LogicalStringComparer Comparer = new LogicalStringComparer();

    // For the cultures we currently support (en-US, de-DE, es-ES, fr-CA, fr-FR, it-IT, jp-JP, and zh-CN),
    // Digits is ["0", "1", "2", "3", "4", "5", "6", "7", "8", "9"]
    string[] Digits = CultureInfo.CurrentCulture.NumberFormat.NativeDigits;
    // For the cultures we currently support, Zero is '0'
    char Zero = CultureInfo.CurrentCulture.NumberFormat.NativeDigits[0][0];
    // For the cultures we currently support, DigitSet is {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9'}
    HashSet<char> DigitSet = new HashSet<char>(CultureInfo.CurrentCulture.NumberFormat.NativeDigits.Select(d => d[0]));

    public int Compare(string x, string y)
    {
      return DoCompare(x, y);
    }

    /// <summary>
    /// Compares two strings, taking numeric chunks into account.  For
    /// example, "Cytomat-2" will be less than "Cytomat-10";  case
    /// is considered if the strings are equivalent without regard to case.
    /// </summary>
    /// <returns>
    ///   Less than 0 if s1 is less than s2;
    ///   0 if s1 and s2 are equivalent;
    ///   Greater than 0 if s1 is greater than s2.
    /// </returns>
    int DoCompare(string s1, string s2)
    {
      int c = DoCompare(s1, s2, true);
      if(c != 0) return c;
      return DoCompare(s1, s2, false);
    }

    /// <summary>
    /// Compares two strings, taking numeric chunks into account.  For
    /// example, "Cytomat-2" will be less than "Cytomat-10";  case is considered if ignoreCase is false.
    /// </summary>
    /// <returns>
    ///   Less than 0 if s1 is less than s2;
    ///   0 if s1 and s2 are equivalent;
    ///   Greater than 0 if s1 is greater than s2.
    /// </returns>
    int DoCompare(string s1, string s2, bool ignoreCase)
    {
      int i1 = 0;
      int i2 = 0;
      while(true)
      {
        string c1 = ReadNextChunk(s1, ref i1);
        string c2 = ReadNextChunk(s2, ref i2);
        if(c1 == null)
          return (c2 == null) ? string.Compare(s1, s2, ignoreCase, CultureInfo.CurrentCulture) : -1;
        if(c2 == null) return 1;
        int c = CompareChunk(c1, c2, ignoreCase);
        if(c != 0) return c;
      }
    }

    string ReadNextChunk(string s, ref int i)
    {
      if((s == null) || (i >= s.Length)) return null;
      int start = i;
      bool isDigit = DigitSet.Contains(s[i++]);
      while((i < s.Length) && DigitSet.Contains(s[i]) == isDigit) i++;
      return s.Substring(start, i - start);
    }

    int CompareChunk(string c1, string c2, bool ignoreCase)
    {
      if(DigitSet.Contains(c1[0]))
      {
        if(!DigitSet.Contains(c2[0])) return -1;
        string s1 = StripLeadingZeros(c1);
        string s2 = StripLeadingZeros(c2);
        int c = s1.Length - s2.Length;
        if(c != 0) return c;
        return string.CompareOrdinal(s1, s2);
      }
      if(DigitSet.Contains(c2[0])) return 1;
      return string.Compare(c1, c2, ignoreCase, CultureInfo.CurrentCulture);
    }

    string StripLeadingZeros(string s)
    {
      return s.TrimStart(Zero);
    }
  }
}
