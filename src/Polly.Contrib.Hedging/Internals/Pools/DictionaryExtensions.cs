// © Microsoft Corporation. All rights reserved.

using System.Collections.Generic;

namespace Polly.Contrib.Hedging.Internals
{
    internal static class DictionaryExtensions
    {
        public static bool Remove<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, out TValue? value)
        {
            if (dictionary.TryGetValue(key, out value))
            {
                _ = dictionary.Remove(key);
                return true;
            }

            value = default;
            return false;
        }
    }
}
