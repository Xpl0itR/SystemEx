// Copyright © 2023 Xpl0itR
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SystemEx.HighPerformance;

public readonly struct RentedMemory<T> where T : unmanaged
{
    public readonly T[]       BackingArray;
    public readonly Memory<T> Memory;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public RentedMemory(int requestedLength)
    {
        BackingArray = ArrayPool<T>.Shared.Rent(requestedLength);
        Memory       = new Memory<T>(BackingArray, 0, requestedLength);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose() =>
        ArrayPool<T>.Shared.Return(BackingArray);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<T> AsSpan() =>
        MemoryMarshal.CreateSpan(
            ref MemoryMarshal.GetArrayDataReference(BackingArray),
            Memory.Length);

    public static implicit operator Memory<T>(RentedMemory<T> rentedSpan) =>
        rentedSpan.Memory;

    public static implicit operator ReadOnlyMemory<T>(RentedMemory<T> rentedSpan) =>
        rentedSpan.Memory;
}