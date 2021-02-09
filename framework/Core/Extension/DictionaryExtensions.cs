using System;
using System.Collections.Generic;

namespace Infrabel.ICT.Framework.Extension
{
    public static class DictionaryExtensions
    {
        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue) =>
            dictionary.TryGetValue(key, out var value) ? value : defaultValue;

        public static TValue GetValueOrDefaultFactory<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> defaultValueProvider) =>
            dictionary.TryGetValue(key, out var value) ? value : defaultValueProvider();
    }
}