// Copyright © 2024 Xpl0itR
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SystemEx.Memory;

public partial struct RentedMemory<T> : IDisposable where T : unmanaged
{
    public T[] BackingArray { get; private set; }
    public int Length       { get; private set; }

    private readonly ArrayPool<T> _arrayPool;
    private GCHandle? _gcHandle;

    public RentedMemory(int length = 0, bool pin = false, bool clearMemory = false, ArrayPool<T>? arrayPool = null)
    {
        _arrayPool   = arrayPool ?? ArrayPool<T>.Shared;
        Length       = length;
        BackingArray = length <= 0
            ? Array.Empty<T>()
            : _arrayPool.Rent(length);

        if (clearMemory)
        {
            Unsafe.InitBlockUnaligned(ref As<byte>(), value: 0, (uint)LengthAsBytes);
        }

        if (pin)
        {
            _gcHandle = GCHandle.Alloc(BackingArray, GCHandleType.Pinned);
        }
    }

    public void EnsureCapacity(int capacity)
    {
        if (BackingArray.Length < capacity)
        {
            T[] newArray = _arrayPool.Rent(capacity);

            if (BackingArray.Length > 0)
            {
                CopyHelper.CopyArrayUnchecked(BackingArray, 0, newArray, 0, Length);
                _arrayPool.Return(BackingArray);
            }

            BackingArray = newArray;

            if (_gcHandle.HasValue)
            {
                _gcHandle.Value.Free();
                _gcHandle = GCHandle.Alloc(BackingArray, GCHandleType.Pinned);
            }
        }

        if (Length < capacity)
        {
            Length = capacity;
        }
    }

    /// <inheritdoc />
    public readonly void Dispose()
    {
        _gcHandle?.Free();
        if (BackingArray.Length > 0)
            _arrayPool.Return(BackingArray);
    }
}