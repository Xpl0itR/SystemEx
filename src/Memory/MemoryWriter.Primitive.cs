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

partial struct MemoryWriter
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write<T>(in T value) where T : unmanaged
    {
        int length = Unsafe.SizeOf<T>();
        Guard.IsGreaterThanOrEqualTo(Remaining, length);

        Unsafe.WriteUnaligned(ref Destination, value);
        _position += length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(ArraySegment<byte> buffer)
    {
        Guard.IsNotNull(buffer.Array);
        Write(ref buffer.Array.DangerousGetReferenceAt(buffer.Offset), buffer.Count);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(byte[] buffer) =>
        Write(ref buffer.DangerousGetReference(), buffer.Length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(byte[] buffer, int offset, int count)
    {
        Guard.HasSizeGreaterThanOrEqualTo(buffer, offset + count);
        Write(ref buffer.DangerousGetReferenceAt(offset), count);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(Span<byte> buffer) =>
#if NET7_0_OR_GREATER
        Write(ref MemoryMarshal.GetReference(buffer), buffer.Length);
#else
        buffer.CopyTo(DestinationSpan);
#endif

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(ReadOnlySpan<byte> buffer) =>
#if NET7_0_OR_GREATER
        Write(ref MemoryMarshal.GetReference(buffer), buffer.Length);
#else
        buffer.CopyTo(DestinationSpan);
#endif

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void Write(void* buffer, int byteLength) =>
        Write(ref Unsafe.AsRef<byte>(buffer), byteLength);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(ref byte buffer, int length)
    {
        Guard.IsBetweenOrEqualTo(length, 0, Remaining);

        Unsafe.CopyBlockUnaligned(ref Destination, ref buffer, unchecked((uint)length));
        _position += length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void Write(char chr, System.Text.Encoding encoding)
    {
        fixed (byte* dstPtr = &Destination)
            _position += encoding.GetBytes(&chr, 1, dstPtr, Remaining);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(ReadOnlySpan<char> chars, System.Text.Encoding? encoding = null) =>
        Write(ref chars.DangerousGetReference(), chars.Length, encoding);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void Write(ref char chars, int charCount, System.Text.Encoding? encoding = null)
    {
        encoding ??= System.Text.Encoding.UTF8;

        fixed (char* srcPtr = &chars)
        fixed (byte* dstPtr = &Destination)
            _position += encoding.GetBytes(srcPtr, charCount, dstPtr, Remaining);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteCBool(bool value) =>
        Write<int>(value ? 1 : 0);
}