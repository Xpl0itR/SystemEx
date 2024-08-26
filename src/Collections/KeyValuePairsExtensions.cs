// Copyright © 2023-2024 Xpl0itR
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.Linq;

namespace SystemEx.Collections;

public static class KeyValuePairsExtensions
{
    public static void Add<TKey, TValue>(this ICollection<KeyValuePair<TKey, TValue>> keyValuePairs, TKey key, TValue value) =>
        keyValuePairs.Add(new KeyValuePair<TKey, TValue>(key, value));
        
    public static bool ContainsDuplicateKey<TKey, TValue>(
        this IEnumerable<KeyValuePair<TKey, TValue>> keyValuePairs,
        IEqualityComparer<TKey>?                     comparer = null)
    {
        HashSet<TKey> set = new(5, comparer);

        foreach (KeyValuePair<TKey, TValue> kvp in keyValuePairs)
        {
            if (!set.Add(kvp.Key))
            {
                return true;
            }
        }

        return false;
    }
    
    public static bool ContainsDuplicateBy<T, TKey>(
        this IEnumerable<T> enumerable,
        Func<T, TKey>       keySelector)
    {
        HashSet<TKey> set = new(5);
        
        foreach (T i in enumerable)
        {
            if (!set.Add(keySelector(i)))
            {
                return true;
            }
        }
    
        return false;
    }

    public static IOrderedEnumerable<KeyValuePair<TKey, TValue>> Sort<TKey, TValue>(
        this IEnumerable<KeyValuePair<TKey, TValue>> keyValuePairs,
        IComparer<TKey>? comparer = null) =>
#pragma warning disable HAA0303
            keyValuePairs.OrderBy(static pair => pair.Key, comparer);
#pragma warning restore HAA0303
}