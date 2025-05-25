// Copyright © 2024-2025 Xpl0itR
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CommunityToolkit.Diagnostics;
using CommunityToolkit.HighPerformance;

namespace SystemEx.Memory;

[StructLayout(LayoutKind.Auto)]
public ref partial struct MemoryWriter
{
#if NET7_0_OR_GREATER
    private readonly ref byte _ref;
    private readonly int _length;
#else
    private readonly Span<byte> _span;
#endif

    private int _position;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public MemoryWriter(ref byte @ref, int length)
    {
        Guard.IsGreaterThan(length, 0);

#if NET7_0_OR_GREATER
        _ref    = ref @ref;
        _length = length;
#else
        _span = MemoryMarshal.CreateSpan(ref @ref, length);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe MemoryWriter(void* ptr, int byteLength)
    {
        Guard.IsGreaterThan(byteLength, 0);

#if NET7_0_OR_GREATER
        _ref    = ref Unsafe.AsRef<byte>(ptr);
        _length = byteLength;
#else
        _span = MemoryMarshal.CreateSpan(
            ref Unsafe.AsRef<byte>(ptr),
            byteLength);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public MemoryWriter(Span<byte> span)
    {
#if NET7_0_OR_GREATER
        _ref    = ref MemoryMarshal.GetReference(span);
        _length = span.Length;
#else
        _span = span;
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public MemoryWriter(byte[] array)
    {
#if NET7_0_OR_GREATER
        _ref    = ref MemoryMarshal.GetArrayDataReference(array);
        _length = array.Length;
#else
        _span = MemoryMarshal.CreateSpan(
            ref array.DangerousGetReference(),
            array.Length);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public MemoryWriter(ArraySegment<byte> segment)
    {
        Guard.IsNotNull(segment.Array);

#if NET7_0_OR_GREATER
        _ref    = ref segment.Array.DangerousGetReferenceAt(segment.Offset);
        _length = segment.Count;
#else
        _span = MemoryMarshal.CreateSpan(
            ref segment.Array.DangerousGetReferenceAt(segment.Offset),
            segment.Count);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public MemoryWriter(RentedArray<byte> rentedArray)
    {
#if NET7_0_OR_GREATER
        _ref    = ref rentedArray.GetPinnableReference();
        _length = rentedArray.Length;
#else
        _span = rentedArray.AsSpan();
#endif
    }

    public int Position
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly get => _position;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            Guard.IsBetweenOrEqualTo(value, 0,
#if NET7_0_OR_GREATER
                _length
#else
                _span.Length
#endif
            );

            _position = value;
        }
    }

    public readonly int Remaining
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get =>
#if NET7_0_OR_GREATER
            _length
#else
            _span.Length
#endif
          - _position;
    }

    public readonly ref byte Destination
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref Unsafe.AddByteOffset(
#if NET7_0_OR_GREATER
            ref _ref,
#else
            ref MemoryMarshal.GetReference(_span),
#endif
            (nint)(uint)_position);
    }

    public readonly Span<byte> DestinationSpan
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get =>
#if NET7_0_OR_GREATER
            MemoryMarshal.CreateSpan(ref Destination, Remaining);
#else
            _span.Slice(_position);
#endif
    }
}