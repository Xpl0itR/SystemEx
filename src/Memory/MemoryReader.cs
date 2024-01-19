// Copyright © 2024 Xpl0itR
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CommunityToolkit.Diagnostics;

namespace SystemEx.Memory;

[StructLayout(LayoutKind.Auto)]
public ref partial struct MemoryReader
{
    private readonly ref byte _ref;
    private readonly int _length;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public MemoryReader(ref byte @ref, int length)
    {
        _ref    = ref @ref;
        _length = length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public MemoryReader(Span<byte> span)
    {
        _ref    = ref MemoryMarshal.GetReference(span);
        _length = span.Length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public MemoryReader(ReadOnlySpan<byte> span)
    {
        _ref    = ref MemoryMarshal.GetReference(span);
        _length = span.Length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public MemoryReader(byte[] array)
    {
        _ref    = ref MemoryMarshal.GetArrayDataReference(array);
        _length = array.Length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public MemoryReader(ArraySegment<byte> segment)
    {
        Guard.IsNotNull(segment.Array);

        _length = segment.Count;
        _ref    = ref MemoryMarshalEx.GetArrayDataReference(
            segment.Array, segment.Offset);
    }

    public int Position;

    public readonly ref byte Source
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref Unsafe.AddByteOffset(ref _ref, Position);
    }

    public readonly ReadOnlySpan<byte> SourceSpan
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => MemoryMarshal.CreateReadOnlySpan(ref Source, _length - Position);
    }
}