using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrabel.ICT.Framework.Extended.SimpleInjectorIoc
{
  /// <summary>Methods to work with immutable arrays and some sugar.</summary>
  public static class ArrayTools
  {
    private static class EmptyArray<T>
    {
      public static readonly T[] Value = new T[0];
    }

    /// <summary>Returns singleton empty array of provided type.</summary> 
    /// <typeparam name="T">Array item type.</typeparam> <returns>Empty array.</returns>
    public static T[] Empty<T>() => EmptyArray<T>.Value;

    /// <summary>Wraps item in array.</summary>
    public static T[] One<T>(this T one) => new[] { one };

    /// <summary>Returns true if array is null or have no items.</summary> <typeparam name="T">Type of array item.</typeparam>
    /// <param name="source">Source array to check.</param> <returns>True if null or has no items, false otherwise.</returns>
    public static bool IsNullOrEmpty<T>(this T[] source) => source == null || source.Length == 0;

    /// <summary>Returns empty array instead of null, or source array otherwise.</summary> <typeparam name="T">Type of array item.</typeparam>
    public static T[] EmptyIfNull<T>(this T[] source) => source ?? Empty<T>();

    /// Returns source enumerable if it is array, otherwise converts source to array or an empty array if null.
    public static T[] ToArrayOrSelf<T>(this IEnumerable<T> source) =>
        source == null ? Empty<T>() : (source as T[] ?? source.ToArray());

    /// Returns source enumerable if it is list, otherwise converts source to IList or an empty array if null.
    public static IList<T> ToListOrSelf<T>(this IEnumerable<T> source) =>
        source == null ? Empty<T>() : source as IList<T> ?? source.ToList();

    /// <summary>Array copy</summary>
    public static T[] Copy<T>(this T[] items)
    {
      if (items == null)
        return null;
      var copy = new T[items.Length];
      Array.Copy(items, 0, copy, 0, copy.Length);
      return copy;
    }

    /// <summary>Returns new array consisting from all items from source array then all items from added array.
    /// If source is null or empty, then added array will be returned.
    /// If added is null or empty, then source will be returned.</summary>
    /// <typeparam name="T">Array item type.</typeparam>
    /// <param name="source">Array with leading items.</param>
    /// <param name="added">Array with following items.</param>
    /// <returns>New array with items of source and added arrays.</returns>
    public static T[] Append<T>(this T[] source, params T[] added)
    {
      if (added == null || added.Length == 0)
        return source;
      if (source == null || source.Length == 0)
        return added;

      var result = new T[source.Length + added.Length];
      Array.Copy(source, 0, result, 0, source.Length);
      if (added.Length == 1)
        result[source.Length] = added[0];
      else
        Array.Copy(added, 0, result, source.Length, added.Length);
      return result;
    }

    /// <summary>Append a single item value at the end of source array and returns its copy</summary>
    public static T[] Append<T>(this T[] source, T value)
    {
      if (source == null || source.Length == 0)
        return new[] { value };
      var count = source.Length;
      var result = new T[count + 1];
      Array.Copy(source, 0, result, 0, count);
      result[count] = value;
      return result;
    }

    /// <summary>Performant concat of enumerables in case of arrays.
    /// But performance will degrade if you use Concat().Where().</summary>
    /// <typeparam name="T">Type of item.</typeparam>
    /// <param name="source">goes first.</param>
    /// <param name="other">appended to source.</param>
    /// <returns>empty array or concat of source and other.</returns>
    public static T[] Append<T>(this IEnumerable<T> source, IEnumerable<T> other) =>
        source.ToArrayOrSelf().Append(other.ToArrayOrSelf());

    /// <summary>Returns new array with <paramref name="value"/> appended, 
    /// or <paramref name="value"/> at <paramref name="index"/>, if specified.
    /// If source array could be null or empty, then single value item array will be created despite any index.</summary>
    /// <typeparam name="T">Array item type.</typeparam>
    /// <param name="source">Array to append value to.</param>
    /// <param name="value">Value to append.</param>
    /// <param name="index">(optional) Index of value to update.</param>
    /// <returns>New array with appended or updated value.</returns>
    public static T[] AppendOrUpdate<T>(this T[] source, T value, int index = -1)
    {
      if (source == null || source.Length == 0)
        return new[] { value };
      var sourceLength = source.Length;
      index = index < 0 ? sourceLength : index;
      var result = new T[index < sourceLength ? sourceLength : sourceLength + 1];
      Array.Copy(source, result, sourceLength);
      result[index] = value;
      return result;
    }

    /// <summary>Calls predicate on each item in <paramref name="source"/> array until predicate returns true,
    /// then method will return this item index, or if predicate returns false for each item, method will return -1.</summary>
    /// <typeparam name="T">Type of array items.</typeparam>
    /// <param name="source">Source array: if null or empty, then method will return -1.</param>
    /// <param name="predicate">Delegate to evaluate on each array item until delegate returns true.</param>
    /// <returns>Index of item for which predicate returns true, or -1 otherwise.</returns>
    public static int IndexOf<T>(this T[] source, Func<T, bool> predicate)
    {
      if (source != null && source.Length != 0)
        for (var i = 0; i < source.Length; ++i)
          if (predicate(source[i]))
            return i;
      return -1;
    }

    /// Minimizes the allocations for closure in predicate lambda with the provided <paramref name="state"/>
    public static int IndexOf<T, S>(this T[] source, S state, Func<S, T, bool> predicate)
    {
      if (source != null && source.Length != 0)
        for (var i = 0; i < source.Length; ++i)
          if (predicate(state, source[i]))
            return i;
      return -1;
    }

    /// <summary>Looks up for item in source array equal to provided value, and returns its index, or -1 if not found.</summary>
    /// <typeparam name="T">Type of array items.</typeparam>
    /// <param name="source">Source array: if null or empty, then method will return -1.</param>
    /// <param name="value">Value to look up.</param>
    /// <returns>Index of item equal to value, or -1 item is not found.</returns>
    public static int IndexOf<T>(this T[] source, T value)
    {
      if (source != null && source.Length != 0)
        for (var i = 0; i < source.Length; ++i)
          if (Equals(source[i], value))
            return i;

      return -1;
    }

    /// <summary>The same as `IndexOf` but searching the item by reference</summary>
    public static int IndexOfReference<T>(this T[] source, T reference) where T : class
    {
      if (source != null && source.Length != 0)
        for (var i = 0; i < source.Length; ++i)
          if (ReferenceEquals(source[i], reference))
            return i;

      return -1;
    }

    /// <summary>Produces new array without item at specified <paramref name="index"/>. 
    /// Will return <paramref name="source"/> array if index is out of bounds, or source is null/empty.</summary>
    /// <typeparam name="T">Type of array item.</typeparam>
    /// <param name="source">Input array.</param> <param name="index">Index if item to remove.</param>
    /// <returns>New array with removed item at index, or input source array if index is not in array.</returns>
    public static T[] RemoveAt<T>(this T[] source, int index)
    {
      if (source == null || source.Length == 0 || index < 0 || index >= source.Length)
        return source;
      if (index == 0 && source.Length == 1)
        return new T[0];
      var result = new T[source.Length - 1];
      if (index != 0)
        Array.Copy(source, 0, result, 0, index);
      if (index != result.Length)
        Array.Copy(source, index + 1, result, index, result.Length - index);
      return result;
    }

    /// <summary>Looks for item in array using equality comparison, and returns new array with found item remove, or original array if not item found.</summary>
    /// <typeparam name="T">Type of array item.</typeparam>
    /// <param name="source">Input array.</param> <param name="value">Value to find and remove.</param>
    /// <returns>New array with value removed or original array if value is not found.</returns>
    public static T[] Remove<T>(this T[] source, T value) =>
        source.RemoveAt(source.IndexOf(value));

    /// <summary>Returns first item matching the <paramref name="predicate"/>, or default item value.</summary>
    /// <typeparam name="T">item type</typeparam>
    /// <param name="source">items collection to search</param>
    /// <param name="predicate">condition to evaluate for each item.</param>
    /// <returns>First item matching condition or default value.</returns>
    public static T FindFirst<T>(this T[] source, Func<T, bool> predicate)
    {
      if (source != null && source.Length != 0)
        for (var i = 0; i < source.Length; ++i)
        {
          var item = source[i];
          if (predicate(item))
            return item;
        }

      return default(T);
    }

    /// Version of FindFirst with the fixed state used by predicate to prevent allocations by predicate lambda closure
    public static T FindFirst<T, S>(this T[] source, S state, Func<S, T, bool> predicate)
    {
      if (source != null && source.Length != 0)
        for (var i = 0; i < source.Length; ++i)
        {
          var item = source[i];
          if (predicate(state, item))
            return item;
        }

      return default(T);
    }

    /// <summary>Returns first item matching the <paramref name="predicate"/>, or default item value.</summary>
    /// <typeparam name="T">item type</typeparam>
    /// <param name="source">items collection to search</param>
    /// <param name="predicate">condition to evaluate for each item.</param>
    /// <returns>First item matching condition or default value.</returns>
    public static T FindFirst<T>(this IEnumerable<T> source, Func<T, bool> predicate) =>
        source is T[] sourceArr ? sourceArr.FindFirst(predicate) : source.FirstOrDefault(predicate);

    /// <summary>Returns element if collection consist on single element, otherwise returns default value.
    /// It does not throw for collection with many elements</summary>
    public static T SingleOrDefaultIfMany<T>(this IEnumerable<T> source)
    {
      if (source is IList<T> list)
        return list.Count == 1 ? list[0] : default(T);

      if (source == null)
        return default(T);

      using (var e = source.GetEnumerator())
      {
        if (!e.MoveNext())
          return default(T);
        var it = e.Current;
        return !e.MoveNext() ? it : default(T);
      }
    }

    /// <summary>Does <paramref name="action"/> for each item</summary>
    public static void ForEach<T>(this T[] source, Action<T> action)
    {
      if (!source.IsNullOrEmpty())
        for (var i = 0; i < source.Length; i++)
          action(source[i]);
    }

    /// Appends source to results
    public static T[] AppendTo<T>(T[] source, int sourcePos, int count, T[] results = null)
    {
      if (results == null)
      {
        var newResults = new T[count];
        if (count == 1)
          newResults[0] = source[sourcePos];
        else
          for (int i = 0, j = sourcePos; i < count; ++i, ++j)
            newResults[i] = source[j];
        return newResults;
      }

      var matchCount = results.Length;
      var appendedResults = new T[matchCount + count];
      if (matchCount == 1)
        appendedResults[0] = results[0];
      else
        Array.Copy(results, 0, appendedResults, 0, matchCount);

      if (count == 1)
        appendedResults[matchCount] = source[sourcePos];
      else
        Array.Copy(source, sourcePos, appendedResults, matchCount, count);

      return appendedResults;
    }

    private static R[] AppendTo<T, R>(T[] source, int sourcePos, int count, Func<T, R> map, R[] results = null)
    {
      if (results == null || results.Length == 0)
      {
        var newResults = new R[count];
        if (count == 1)
          newResults[0] = map(source[sourcePos]);
        else
          for (int i = 0, j = sourcePos; i < count; ++i, ++j)
            newResults[i] = map(source[j]);
        return newResults;
      }

      var oldResultsCount = results.Length;
      var appendedResults = new R[oldResultsCount + count];
      if (oldResultsCount == 1)
        appendedResults[0] = results[0];
      else
        Array.Copy(results, 0, appendedResults, 0, oldResultsCount);

      if (count == 1)
        appendedResults[oldResultsCount] = map(source[sourcePos]);
      else
      {
        for (int i = oldResultsCount, j = sourcePos; i < appendedResults.Length; ++i, ++j)
          appendedResults[i] = map(source[j]);
      }

      return appendedResults;
    }

    private static R[] AppendTo<T, S, R>(T[] source, S state, int sourcePos, int count, Func<S, T, R> map, R[] results = null)
    {
      if (results == null || results.Length == 0)
      {
        var newResults = new R[count];
        if (count == 1)
          newResults[0] = map(state, source[sourcePos]);
        else
          for (int i = 0, j = sourcePos; i < count; ++i, ++j)
            newResults[i] = map(state, source[j]);
        return newResults;
      }

      var oldResultsCount = results.Length;
      var appendedResults = new R[oldResultsCount + count];
      if (oldResultsCount == 1)
        appendedResults[0] = results[0];
      else
        Array.Copy(results, 0, appendedResults, 0, oldResultsCount);

      if (count == 1)
        appendedResults[oldResultsCount] = map(state, source[sourcePos]);
      else
      {
        for (int i = oldResultsCount, j = sourcePos; i < appendedResults.Length; ++i, ++j)
          appendedResults[i] = map(state, source[j]);
      }

      return appendedResults;
    }

    /// <summary>MUTATES the source by updating its item or creates another array with the copies,
    /// the source then maybe a partially updated</summary>
    public static T[] UpdateItemOrShrinkUnsafe<T, S>(this T[] source, S state, Func<S, T, T> tryMap) where T : class
    {
      if (source.Length == 1)
      {
        var result0 = tryMap(state, source[0]);
        if (result0 == null)
          return Empty<T>();
        source[0] = result0;
        return source;
      }

      if (source.Length == 2)
      {
        var result0 = tryMap(state, source[0]);
        var result1 = tryMap(state, source[1]);
        if (result0 == null && result1 == null)
          return Empty<T>();
        if (result0 == null)
          return new[] { result1 };
        if (result1 == null)
          return new[] { result0 };
        source[0] = result0;
        source[1] = result1;
        return source;
      }

      var matchStart = 0;
      T[] matches = null;
      T result = null;
      var i = 0;
      for (; i < source.Length; ++i)
      {
        result = tryMap(state, source[i]);
        if (result != null)
          source[i] = result;
        else
        {
          // for accumulated matched items
          if (i != 0 && i > matchStart)
            matches = AppendTo(source, matchStart, i - matchStart, matches);
          matchStart = i + 1; // guess the next match start will be after the non-matched item
        }
      }

      // when last match was found but not all items are matched (hence matchStart != 0)
      if (result != null && matchStart != 0)
        return AppendTo(source, matchStart, i - matchStart, matches);

      return matches ?? (matchStart != 0 ? Empty<T>() : source);
    }

    /// <summary>Where method similar to Enumerable.Where but more performant and non necessary allocating.
    /// It returns source array and does Not create new one if all items match the condition.</summary>
    /// <typeparam name="T">Type of source items.</typeparam>
    /// <param name="source">If null, the null will be returned.</param>
    /// <param name="condition">Condition to keep items.</param>
    /// <returns>New array if some items are filter out. Empty array if all items are filtered out. Original array otherwise.</returns>
    public static T[] Match<T>(this T[] source, Func<T, bool> condition)
    {
      if (source == null || source.Length == 0)
        return source;

      if (source.Length == 1)
        return condition(source[0]) ? source : Empty<T>();

      if (source.Length == 2)
      {
        var condition0 = condition(source[0]);
        var condition1 = condition(source[1]);
        return condition0 && condition1 ? new[] { source[0], source[1] }
            : condition0 ? new[] { source[0] }
            : condition1 ? new[] { source[1] }
            : Empty<T>();
      }

      var matchStart = 0;
      T[] matches = null;
      var matchFound = false;
      var i = 0;
      for (; i < source.Length; ++i)
        if (!(matchFound = condition(source[i])))
        {
          // for accumulated matched items
          if (i != 0 && i > matchStart)
            matches = AppendTo(source, matchStart, i - matchStart, matches);
          matchStart = i + 1; // guess the next match start will be after the non-matched item
        }

      // when last match was found but not all items are matched (hence matchStart != 0)
      if (matchFound && matchStart != 0)
        return AppendTo(source, matchStart, i - matchStart, matches);

      return matches ?? (matchStart != 0 ? Empty<T>() : source);
    }

    /// Match with the additional state to use in <paramref name="condition"/> to minimize the allocations in <paramref name="condition"/> lambda closure 
    public static T[] Match<T, S>(this T[] source, S state, Func<S, T, bool> condition)
    {
      if (source == null || source.Length == 0)
        return source;

      if (source.Length == 1)
        return condition(state, source[0]) ? source : Empty<T>();

      if (source.Length == 2)
      {
        var condition0 = condition(state, source[0]);
        var condition1 = condition(state, source[1]);
        return condition0 && condition1 ? new[] { source[0], source[1] }
            : condition0 ? new[] { source[0] }
            : condition1 ? new[] { source[1] }
            : Empty<T>();
      }

      var matchStart = 0;
      T[] matches = null;
      var matchFound = false;
      var i = 0;
      for (; i < source.Length; ++i)
        if (!(matchFound = condition(state, source[i])))
        {
          // for accumulated matched items
          if (i != 0 && i > matchStart)
            matches = AppendTo(source, matchStart, i - matchStart, matches);
          matchStart = i + 1; // guess the next match start will be after the non-matched item
        }

      // when last match was found but not all items are matched (hence matchStart != 0)
      if (matchFound && matchStart != 0)
        return AppendTo(source, matchStart, i - matchStart, matches);

      return matches ?? (matchStart != 0 ? Empty<T>() : source);
    }

    /// <summary>Where method similar to Enumerable.Where but more performant and non necessary allocating.
    /// It returns source array and does Not create new one if all items match the condition.</summary>
    /// <typeparam name="T">Type of source items.</typeparam> <typeparam name="R">Type of result items.</typeparam>
    /// <param name="source">If null, the null will be returned.</param>
    /// <param name="condition">Condition to keep items.</param> <param name="map">Converter from source to result item.</param>
    /// <returns>New array of result items.</returns>
    public static R[] Match<T, R>(this T[] source, Func<T, bool> condition, Func<T, R> map)
    {
      if (source == null)
        return null;

      if (source.Length == 0)
        return Empty<R>();

      if (source.Length == 1)
      {
        var item = source[0];
        return condition(item) ? new[] { map(item) } : Empty<R>();
      }

      if (source.Length == 2)
      {
        var condition0 = condition(source[0]);
        var condition1 = condition(source[1]);
        return condition0 && condition1 ? new[] { map(source[0]), map(source[1]) }
            : condition0 ? new[] { map(source[0]) }
            : condition1 ? new[] { map(source[1]) }
            : Empty<R>();
      }

      var matchStart = 0;
      R[] matches = null;
      var matchFound = false;

      var i = 0;
      for (; i < source.Length; ++i)
        if (!(matchFound = condition(source[i])))
        {
          // for accumulated matched items
          if (i != 0 && i > matchStart)
            matches = AppendTo(source, matchStart, i - matchStart, map, matches);
          matchStart = i + 1; // guess the next match start will be after the non-matched item
        }

      // when last match was found but not all items are matched (hence matchStart != 0)
      if (matchFound && matchStart != 0)
        return AppendTo(source, matchStart, i - matchStart, map, matches);

      return matches ?? (matchStart == 0 ? AppendTo(source, 0, source.Length, map) : Empty<R>());
    }

    /// Match with the additional state to use in <paramref name="condition"/> and <paramref name="map"/> to minimize the allocations in <paramref name="condition"/> lambda closure 
    public static R[] Match<T, S, R>(this T[] source, S state, Func<S, T, bool> condition, Func<S, T, R> map)
    {
      if (source == null)
        return null;

      if (source.Length == 0)
        return Empty<R>();

      if (source.Length == 1)
      {
        var item = source[0];
        return condition(state, item) ? new[] { map(state, item) } : Empty<R>();
      }

      if (source.Length == 2)
      {
        var condition0 = condition(state, source[0]);
        var condition1 = condition(state, source[1]);
        return condition0 && condition1 ? new[] { map(state, source[0]), map(state, source[1]) }
            : condition0 ? new[] { map(state, source[0]) }
            : condition1 ? new[] { map(state, source[1]) }
            : Empty<R>();
      }

      var matchStart = 0;
      R[] matches = null;
      var matchFound = false;

      var i = 0;
      for (; i < source.Length; ++i)
        if (!(matchFound = condition(state, source[i])))
        {
          // for accumulated matched items
          if (i != 0 && i > matchStart)
            matches = AppendTo(source, state, matchStart, i - matchStart, map, matches);
          matchStart = i + 1; // guess the next match start will be after the non-matched item
        }

      // when last match was found but not all items are matched (hence matchStart != 0)
      if (matchFound && matchStart != 0)
        return AppendTo(source, state, matchStart, i - matchStart, map, matches);

      return matches ?? (matchStart == 0 ? AppendTo(source, state, 0, source.Length, map) : Empty<R>());
    }



    /// <summary>Maps all items from source to result collection. 
    /// If possible uses fast array Map otherwise Enumerable.Select.</summary>
    /// <typeparam name="T">Source item type</typeparam> <typeparam name="R">Result item type</typeparam>
    /// <param name="source">Source items</param> <param name="map">Function to convert item from source to result.</param>
    /// <returns>Converted items</returns>
    public static IEnumerable<R> Map<T, R>(this IEnumerable<T> source, Func<T, R> map) =>
        source is T[] arr ? arr.Map(map) : source?.Select(map);

    /// <summary>If <paramref name="source"/> is array uses more effective Match for array, otherwise just calls Where</summary>
    public static IEnumerable<T> Match<T>(this IEnumerable<T> source, Func<T, bool> condition) =>
        source is T[] arr ? arr.Match(condition) : source?.Where(condition);

    /// <summary>If <paramref name="source"/> is array uses more effective Match for array,otherwise just calls Where, Select</summary>
    public static IEnumerable<R> Match<T, R>(this IEnumerable<T> source, Func<T, bool> condition, Func<T, R> map) =>
        source is T[] arr ? arr.Match(condition, map) : source?.Where(condition).Select(map);
  }

}
