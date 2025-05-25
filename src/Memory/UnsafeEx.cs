// Copyright © 2023-2025 Xpl0itR
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

public static class UnsafeEx
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyBlockUnaligned<TSource, TDest>(TSource[] source, int srcOffset, TDest[] destination, int dstOffset, int count)
        where TSource : unmanaged
        where TDest   : unmanaged
    {
        CopyBlockUnaligned(
            ref source.DangerousGetReference(),
            srcOffset,
            ref destination.DangerousGetReference(),
            dstOffset,
            count);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyBlockUnaligned<TSource, TDest>(ref TSource source, int srcOffset, ref TDest destination, int dstOffset, int count)
        where TSource : unmanaged
        where TDest   : unmanaged
    {
        Unsafe.CopyBlockUnaligned(
            ref As<TDest, byte>(ref destination, dstOffset),
            ref As<TSource, byte>(ref source, srcOffset),
            (uint)(count * Unsafe.SizeOf<TSource>()));
    }

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
            ref str.DangerousGetReference(),
            str.Length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref TTo As<TFrom, TTo>(ref TFrom ptr, int offset)
#if NET9_0_OR_GREATER
        where TFrom : allows ref struct
        where TTo   : allows ref struct
#endif
        => ref Unsafe.As<TFrom, TTo>(
            ref Unsafe.Add(ref ptr, (nint)(uint)offset));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int SizeAs<TFrom, TTo>(int fromSize)
#if NET9_0_OR_GREATER
        where TFrom : allows ref struct
        where TTo   : allows ref struct
#endif
    {
        Guard.IsEqualTo(Unsafe.SizeOf<TFrom>() % Unsafe.SizeOf<TTo>(), 0);

        return fromSize * Unsafe.SizeOf<TFrom>() / Unsafe.SizeOf<TTo>();
    }
}