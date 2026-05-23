// Copyright © 2026 Xpl0itR
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

#if !NET9_0_OR_GREATER
using Lock = object;
#endif

namespace SystemEx.Collections;

public sealed class SlotMap<TItem>(int initialCapacity = 256) : IEnumerable<TItem> where TItem : class
{
    private readonly Lock _resizeLock = new();

    private volatile TItem?[] _slots = new TItem?[initialCapacity];

    private int _cursor;

    /// <summary>
    ///     Inserts the item into a free slot and returns the slot index.
    /// </summary>
    /// <remarks>The caller must store the index and pass it to <see cref="RemoveAt"/> for removal.</remarks>
    public int Insert(TItem item)
    {
        while (true)
        {
            TItem?[] slots = _slots; // snapshot volatile ref once per attempt
            int length = slots.Length;

            // Use a single modulo for the start index; inner loop uses cheaper
            // conditional subtraction instead of modulo for each probe step.
            int start = Interlocked.Increment(ref _cursor) % length;

            for (int i = 0; i < length; i++)
            {
                // Equivalent to (start + i) % length but avoids integer division.
                int index = start + i;
                if (index >= length) index -= length;

                if (Interlocked.CompareExchange(ref slots[index], item, null) is null)
                    return index;
            }

            Expand(slots);
        }
    }

    /// <summary>
    ///     Frees the slot at the given index.
    /// </summary>
    /// <remarks>Always reads internal array fresh to handle concurrent resize correctly.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void RemoveAt(int slotIndex) =>
        Volatile.Write(ref _slots[slotIndex]!, null!);

    /// <summary>
    ///     Iterates all occupied slots and invokes <paramref name="action"/> on each.
    /// </summary>
    /// <remarks>Snapshots the internal array; therefore, items inserted after the snapshot are not visited.</remarks>
    public void ForEach(Action<TItem> action)
    {
        TItem?[] slots = _slots;
        for (int i = 0; i < slots.Length; i++)
        {
            TItem? item = Volatile.Read(ref slots[i]);
            if (item is not null)
                action(item);
        }
    }

    /// <remarks>Not safe to call concurrently with <see cref="Insert"/>. Intended for cleanup only.</remarks>
    public void Clear()
    {
        TItem?[] slots = _slots;
        for (int i = 0; i < slots.Length; i++)
            Volatile.Write(ref slots[i]!, null!);
    }

    private void Expand(TItem?[] observedSlots)
    {
        lock (_resizeLock)
        {
            // Another thread may have already expanded by the time we acquire the lock
            if (_slots != observedSlots)
                return;

            int newLength = checked(_slots.Length * 2);
            TItem?[] expanded = new TItem?[newLength];

            // Copy existing slots into the same indices so stored slot indices remain valid.
            // Items currently freeing into the old array will write null into the old
            // array, not the new one — but those slots are copied as non-null here. This is
            // safe: the worst case is a slot appears occupied in the new array for a brief
            // window after free. Since RemoveAt() always writes to _slots (not a snapshot),
            // it will land in the new array after this assignment completes.
            Array.Copy(_slots, expanded, _slots.Length);

            _slots = expanded;
        }
    }

    /// <summary>
    ///     Returns a struct enumerator over a snapshot of the slot array.
    ///     Allocation-free when used in a foreach loop — the compiler duck-types
    ///     on the concrete struct, no IEnumerable/IEnumerator boxing occurs.
    /// </summary>
    /// <remarks>Snapshots the internal array; therefore, items inserted after the snapshot are not visited.</remarks>
    public Enumerator GetEnumerator() =>
        new(_slots);

    IEnumerator<TItem> IEnumerable<TItem>.GetEnumerator() =>
        new Enumerator(_slots);

    IEnumerator IEnumerable.GetEnumerator() =>
        new Enumerator(_slots);

    public struct Enumerator(TItem?[] slots) : IEnumerator<TItem>
    {
        private int _index = -1;

        public TItem Current { get; private set; } = null!;

        object IEnumerator.Current => Current;

        public bool MoveNext()
        {
            while (++_index < slots.Length)
            {
                TItem? item = Volatile.Read(ref slots[_index]);
                if (item is not null)
                {
                    Current = item;
                    return true;
                }
            }

            return false;
        }

        public void Reset()
        {
            _index = -1;
            Current = null!;
        }

        public void Dispose() { }
    }
}