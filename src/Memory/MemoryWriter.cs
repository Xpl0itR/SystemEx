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
public ref partial struct MemoryWriter
{
    private readonly ref byte _ref;
    private readonly int _length;
    private int _position;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public MemoryWriter(ref byte @ref, int length)
    {
        _ref    = ref @ref;
        _length = length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public MemoryWriter(Span<byte> span)
    {
        _ref    = ref MemoryMarshal.GetReference(span);
        _length = span.Length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public MemoryWriter(byte[] array)
    {
        _ref    = ref MemoryMarshal.GetArrayDataReference(array);
        _length = array.Length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public MemoryWriter(ArraySegment<byte> segment)
    {
        Guard.IsNotNull(segment.Array);

        _length = segment.Count;
        _ref    = ref MemoryMarshalEx.GetArrayDataReference(
            segment.Array, segment.Offset);
    }

    public int Position
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly get => _position;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            ArgumentOutOfRangeException.ThrowIfNegative(value);
            _position = value;
        }
    }

    public readonly int Remaining
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _length - _position;
    }

    public readonly ref byte Destination
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref Unsafe.AddByteOffset(ref _ref, _position);
    }

    public readonly Span<byte> DestinationSpan
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => MemoryMarshal.CreateSpan(ref Destination, Remaining);
    }
}