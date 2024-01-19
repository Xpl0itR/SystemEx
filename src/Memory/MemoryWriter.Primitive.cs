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

partial struct MemoryWriter
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(bool value) =>
        Write(value ? (byte)1 : (byte)0);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(sbyte value) =>
        Write((byte)value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(byte value)
    {
        Guard.IsGreaterThanOrEqualTo(_length - Position, 1);

        Destination = value;
        Position++;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write<T>(in T value) where T : unmanaged
    {
        int length = Unsafe.SizeOf<T>();
        Guard.IsGreaterThanOrEqualTo(_length - Position, length);

        Unsafe.As<byte, T>(ref Destination) =  value;
        Position                            += length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(byte[] buffer) =>
        Write(buffer, 0, buffer.Length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(byte[] buffer, int offset, int count) =>
        Write(ref MemoryMarshalEx.GetArrayDataReference(buffer, offset), count);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(Span<byte> buffer) =>
        Write(ref MemoryMarshal.GetReference(buffer), buffer.Length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(ReadOnlySpan<byte> buffer) =>
        Write(ref MemoryMarshal.GetReference(buffer), buffer.Length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(ref byte buffer, int length)
    {
        Guard.IsGreaterThanOrEqualTo(_length - Position, length);

        Unsafe.CopyBlock(ref Destination, ref buffer, checked((uint)length));
        Position += length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void Write(char chr, System.Text.Encoding? encoding = null)
    {
        encoding ??= System.Text.Encoding.UTF8;

        Span<byte> dst = DestinationSpan;
        fixed (byte* dstPtr = dst)
            Position += encoding.GetBytes(&chr, 1, dstPtr, dst.Length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(ReadOnlySpan<char> chars, System.Text.Encoding? encoding = null)
    {
        encoding ??= System.Text.Encoding.UTF8;
        Position += encoding.GetBytes(chars, DestinationSpan);
    }
}