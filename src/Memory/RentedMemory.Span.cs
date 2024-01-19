// Copyright © 2024 Xpl0itR
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SystemEx.Memory;

partial struct RentedMemory<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Span<T> AsSpan() =>
        MemoryMarshal.CreateSpan(ref this.Reference, this.Length);
        
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ReadOnlySpan<T> AsReadOnlySpan() =>
        MemoryMarshal.CreateReadOnlySpan(ref this.Reference, this.Length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Span<T>(RentedMemory<T> rentedArray) =>
        rentedArray.AsSpan();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ReadOnlySpan<T>(RentedMemory<T> rentedArray) =>
        rentedArray.AsReadOnlySpan();
}