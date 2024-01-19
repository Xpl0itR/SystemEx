// Copyright © 2023-2024 Xpl0itR
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CommunityToolkit.Diagnostics;

namespace SystemEx.Memory;

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
            ref Unsafe.AsRef(
                in str.GetPinnableReference()),
            str.Length); // They say strings are immutable, I must disagree

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref TTo As<TFrom, TTo>(ref TFrom ptr, int offset)
        where TFrom : unmanaged
        where TTo   : unmanaged =>
            ref Unsafe.As<TFrom, TTo>(
                ref Unsafe.Add(ref ptr, offset));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int SizeAs<TFrom, TTo>(int fromSize)
        where TFrom : unmanaged
        where TTo   : unmanaged
    {
        Guard.IsEqualTo(Unsafe.SizeOf<TFrom>() % Unsafe.SizeOf<TTo>(), 0);
        
        return fromSize * Unsafe.SizeOf<TFrom>() / Unsafe.SizeOf<TTo>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T GetReference<T>(Span<T> span, int offset) =>
        ref Unsafe.Add(
            ref MemoryMarshal.GetReference(span),
            offset);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T GetArrayDataReference<T>(T[] array, int offset) =>
        ref Unsafe.Add(
            ref MemoryMarshal.GetArrayDataReference(array),
            offset);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<T> SliceReadOnly<T>(this Span<T> span, int start, int length) =>
        MemoryMarshal.CreateReadOnlySpan(
            ref Unsafe.Add(
                ref MemoryMarshal.GetReference(span),
                start),
            length);
}