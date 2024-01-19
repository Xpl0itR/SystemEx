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

partial struct MemoryReader
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ReadBoolean() =>
        ReadByte() != 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public sbyte ReadSByte() =>
        (sbyte)ReadByte();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte ReadByte()
    {
        Guard.IsGreaterThanOrEqualTo(_length - Position, 1);

        byte value = Source;
        Position++;

        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Read<T>() where T : unmanaged
    {
        int length = Unsafe.SizeOf<T>();
        Guard.IsGreaterThanOrEqualTo(_length - Position, length);

        T value = Unsafe.As<byte, T>(ref Source);
        Position += length;

        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<byte> ReadBytes(int count)
    {
        Guard.IsGreaterThanOrEqualTo(_length - Position, count);

        ReadOnlySpan<byte> buffer =
            MemoryMarshal.CreateReadOnlySpan(ref Source, count);

        Position += count;
        return buffer;
    }
}