// Copyright © 2024-2025 Xpl0itR
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CommunityToolkit.Diagnostics;
using CommunityToolkit.HighPerformance;

namespace SystemEx.Memory;

public readonly struct RentedArray<T> : IDisposable where T : unmanaged
{
    private readonly ArrayPool<T> _arrayPool;

    public readonly T[] BackingArray;
    public readonly int Length;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public RentedArray(int length, bool clearMemory = false, ArrayPool<T>? arrayPool = null)
    {
        Guard.IsGreaterThan(length, 0);

        _arrayPool   = arrayPool ?? ArrayPool<T>.Shared;
        BackingArray = _arrayPool.Rent(length);
        Length       = length;

        if (clearMemory)
        {
            Unsafe.InitBlockUnaligned(
                ref UnsafeAsBytes(),
                value: 0,
                unchecked((uint)(BackingArray.Length * Unsafe.SizeOf<T>())));
        }
    }

    public int LengthInBytes
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Length * Unsafe.SizeOf<T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose() =>
        _arrayPool.Return(BackingArray);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T GetPinnableReference() =>
        ref BackingArray.DangerousGetReference();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref byte UnsafeAsBytes() =>
        ref Unsafe.As<T, byte>(ref BackingArray.DangerousGetReference());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TTo UnsafeRead<TTo>() where TTo : unmanaged =>
        Unsafe.ReadUnaligned<TTo>(ref UnsafeAsBytes());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TTo UnsafeRead<TTo>(int byteOffset) where TTo : unmanaged =>
        Unsafe.ReadUnaligned<TTo>(ref Unsafe.AddByteOffset(ref UnsafeAsBytes(), (nint)(uint)byteOffset));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<T> AsSpan() =>
        MemoryMarshal.CreateSpan(ref BackingArray.DangerousGetReference(), Length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<T> AsSpan(int offset, int? length = null) =>
        new(BackingArray, offset, length ?? Length - offset);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<T> AsReadOnlySpan() =>
        MemoryMarshal.CreateReadOnlySpan(ref BackingArray.DangerousGetReference(), Length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<T> AsReadOnlySpan(int offset, int? length = null) =>
        new(BackingArray, offset, length ?? Length - offset);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Memory<T> AsMemory(int offset = 0, int? length = null) =>
        new(BackingArray, offset, length ?? Length - offset);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlyMemory<T> AsReadOnlyMemory(int offset = 0, int? length = null) =>
        new(BackingArray, offset, length ?? Length - offset);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ArraySegment<T> AsArraySegment(int offset = 0, int? length = null) =>
        new(BackingArray, offset, length ?? Length - offset);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Span<T>(RentedArray<T> rented) =>
        rented.AsSpan();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ReadOnlySpan<T>(RentedArray<T> rented) =>
        rented.AsReadOnlySpan();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Memory<T>(RentedArray<T> rented) =>
        rented.AsMemory();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ReadOnlyMemory<T>(RentedArray<T> rented) =>
        rented.AsReadOnlyMemory();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ArraySegment<T>(RentedArray<T> rented) =>
        rented.AsArraySegment();
}