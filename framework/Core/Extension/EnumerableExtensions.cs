using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrabel.ICT.Framework.Extension
{
    public static class EnumerableExtensions
    {
        public static IDictionary<TKey, IEnumerable<T>> ToLookupDictionary<T, TKey>(this IEnumerable<T> values, Func<T, TKey> keySelector, IEqualityComparer<TKey> equalityComparer = null) where TKey : IComparable
        {
            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));
            Dictionary<TKey, IList<T>> result = equalityComparer == null ? new Dictionary<TKey, IList<T>>() : new Dictionary<TKey, IList<T>>(equalityComparer);
            if (values == null)
                return result.ToDictionary(k => k.Key, v => v.Value.AsEnumerable());

            values.Where(v => v != null).Execute(v =>
            {
                var key = keySelector(v);
                if (result.ContainsKey(key))
                    result[key].Add(v);
                else
                    result.Add(key, new List<T> { v });
            });

            return result.ToDictionary(k => k.Key, v => v.Value.AsEnumerable());
        }

        public static void ConvertAndExecute<TFrom, TTo>(this IEnumerable<TFrom> values,
            Action<TTo> action, Predicate<TFrom> filter = null, Func<TFrom, object> selector = null) where TFrom : class
            where TTo : class
        {
            if (values == null)
                return;

            foreach (var value in values)
                if (filter?.Invoke(value) ?? true)
                {
                    object toConvert = value;
                    if (selector != null)
                        toConvert = selector(value);

                    if (toConvert is TTo convertedValue)
                        action(convertedValue);
                }
        }

        public static IEnumerable<TFrom> LazyConvertAndExecute<TFrom, TTo>(this IEnumerable<TFrom> values,
            Action<TTo> action, Predicate<TFrom> filter = null, Func<TFrom, object> selector = null) where TFrom : class
            where TTo : class
        {
            if (values == null)
                yield break;

            foreach (var value in values)
            {
                if (filter?.Invoke(value) ?? true)
                {
                    object toConvert = value;
                    if (selector != null)
                        toConvert = selector(value);

                    if (toConvert is TTo convertedValue)
                        action(convertedValue);
                }

                yield return value;
            }
        }

        public static IEnumerable<T> LazyExecuteOnPredicate<T>(this IEnumerable<T> values, Action<T> execute, Predicate<T> predicate)
        {
            if (values == null)
                yield break;

            foreach (var value in values)
            {
                if (predicate(value))
                    execute(value);

                yield return value;
            }
        }

        public static void ExecuteOnPredicate<T>(this IEnumerable<T> values, Action<T> execute, Predicate<T> predicate)
        {
            if (values == null)
                return;

            foreach (var value in values)
                if (predicate(value))
                    execute(value);
        }

        public static void Execute<T>(this IEnumerable<T> values, Action<T> execute)
        {
            if (values == null)
                return;

            foreach (var value in values) execute(value);
        }

        public static IEnumerable<T> LazyExecute<T>(this IEnumerable<T> values, Action<T> execute)
        {
            if (values == null)
                yield break;

            foreach (var value in values)
            {
                execute(value);
                yield return value;
            }
        }

        public static async Task ExecuteAsync<T>(this IEnumerable<T> values, Func<T, Task> execute)
        {
            if (values == null)
                return;

            foreach (var value in values)
                await execute(value);
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> value)
        {
            return value == null || !value.Any();
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> value)
        {
            return value.OrderBy(t => Guid.NewGuid());
        }
    }
}