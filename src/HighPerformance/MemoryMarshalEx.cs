// Copyright © 2023 Xpl0itR
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CommunityToolkit.Diagnostics;

namespace SystemEx.HighPerformance;

public static class MemoryMarshalEx
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<byte> AsBytes<T>(this ref T value) where T : unmanaged =>
        MemoryMarshal.CreateSpan(
            ref Unsafe.As<T, byte>(ref value),
            Unsafe.SizeOf<T>());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<byte> AsReadOnlyBytes<T>(this ref T value) where T : unmanaged =>
        MemoryMarshal.CreateReadOnlySpan(
            ref Unsafe.As<T, byte>(ref value),
            Unsafe.SizeOf<T>());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<char> AsWriteableSpan(this string str) =>
        MemoryMarshal.CreateSpan(
            ref Unsafe.AsRef(str.GetPinnableReference()), // They say strings are immutable, I must disagree
            str.Length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T As<T>(ref byte ptr, int offset) where T : unmanaged =>
        ref Unsafe.As<byte, T>(ref Unsafe.AddByteOffset(ref ptr, (IntPtr)offset));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int SizeAs<TFrom, TTo>(int fromSize)
        where TFrom : unmanaged
        where TTo   : unmanaged
    {
        Guard.IsEqualTo(Unsafe.SizeOf<TFrom>() % Unsafe.SizeOf<TTo>(), 0);
        
        return fromSize * Unsafe.SizeOf<TFrom>() / Unsafe.SizeOf<TTo>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<T> SliceReadOnly<T>(this Span<T> span, int start, int length) =>
        MemoryMarshal.CreateReadOnlySpan(
            ref Unsafe.Add(ref MemoryMarshal.GetReference(span), start),
            length);
}