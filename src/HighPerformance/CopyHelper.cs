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

public static class CopyHelper
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyArray<TSource, TDest>(TSource[] source, int srcOffset, TDest[] destination, int dstOffset, int srcCount)
        where TSource : unmanaged
        where TDest   : unmanaged
    {
        Guard.HasSizeGreaterThanOrEqualTo(source,      srcOffset + srcCount);
        Guard.HasSizeGreaterThanOrEqualTo(destination, dstOffset + MemoryMarshalEx.SizeAs<TSource, TDest>(srcCount));

        CopyArrayUnchecked(source, srcOffset, destination, dstOffset, srcCount);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyArrayUnchecked<TSource, TDest>(TSource[] source, int srcOffset, TDest[] destination, int dstOffset, int srcCount)
        where TSource : unmanaged
        where TDest   : unmanaged
    {
        Unsafe.CopyBlock(
            ref Unsafe.As<TDest, byte>(ref Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(destination), dstOffset)),
            ref Unsafe.As<TSource, byte>(ref Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(source),    srcOffset)),
            (uint)(srcCount * Unsafe.SizeOf<TSource>()));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyBlockUnchecked(ref byte source, int srcOffset, ref byte destination, int dstOffset, int count) =>
        Unsafe.CopyBlock(
            ref Unsafe.AddByteOffset(ref destination, (IntPtr)dstOffset),
            ref Unsafe.AddByteOffset(ref source,      (IntPtr)srcOffset),
            (uint)count);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopySpan(ReadOnlySpan<byte> source, Span<byte> destination)
    {
        Guard.HasSizeGreaterThanOrEqualTo(destination, source.Length);

        CopySpanUnchecked(source, destination);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopySpan<TSource, TDest>(ReadOnlySpan<TSource> source, Span<TDest> destination)
        where TSource : unmanaged
        where TDest   : unmanaged
    {
        Guard.HasSizeGreaterThanOrEqualTo(destination, MemoryMarshalEx.SizeAs<TSource, TDest>(source.Length));

        CopySpanUnchecked(source, destination);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopySpanUnchecked(ReadOnlySpan<byte> source, Span<byte> destination) =>
        Unsafe.CopyBlock(
            ref MemoryMarshal.GetReference(destination),
            ref MemoryMarshal.GetReference(source),
            (uint)source.Length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopySpanUnchecked<TSource, TDest>(ReadOnlySpan<TSource> source, Span<TDest> destination)
        where TSource : unmanaged
        where TDest   : unmanaged
    {
        Unsafe.CopyBlock(
            ref Unsafe.As<TDest, byte>(ref MemoryMarshal.GetReference(destination)),
            ref Unsafe.As<TSource, byte>(ref MemoryMarshal.GetReference(source)),
            (uint)(source.Length * Unsafe.SizeOf<TSource>()));
    }
}