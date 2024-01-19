// Copyright © 2024 Xpl0itR
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CommunityToolkit.Diagnostics;

namespace SystemEx.Memory;

partial struct RentedMemory<T>
{
    public readonly unsafe void* Pointer
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            if (!_gcHandle.HasValue)
                ThrowHelper.ThrowInvalidOperationException("backing array not pinned");
            return Unsafe.AsPointer(ref Reference);
        }
    }

    public readonly ref T Reference
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref MemoryMarshal.GetArrayDataReference(this.BackingArray);
    }

    public readonly int LengthAsBytes
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this.Length * Unsafe.SizeOf<T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ref TTo As<TTo>() where TTo : unmanaged =>
        ref Unsafe.As<T, TTo>(ref Reference);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ref TTo As<TTo>(int offset) where TTo : unmanaged =>
        ref MemoryMarshalEx.As<T, TTo>(ref Reference, offset);
}