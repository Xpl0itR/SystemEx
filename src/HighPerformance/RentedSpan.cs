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

public readonly ref struct RentedSpan<T> where T : unmanaged
{
    public readonly T[]     BackingArray;
    public readonly Span<T> Span;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public RentedSpan(int requestedLength)
    {
        BackingArray = ArrayPool<T>.Shared.Rent(requestedLength);
        Span = MemoryMarshal.CreateSpan(
            ref MemoryMarshal.GetArrayDataReference(BackingArray),
            requestedLength);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose() =>
        ArrayPool<T>.Shared.Return(BackingArray);

    public static implicit operator Span<T>(RentedSpan<T> rentedSpan) =>
        rentedSpan.Span;

    public static implicit operator ReadOnlySpan<T>(RentedSpan<T> rentedSpan) =>
        rentedSpan.Span;
}