// Copyright © 2024 Xpl0itR
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;

namespace SystemEx.Memory;

partial struct MemoryReader
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public double ReadDoubleLittleEndian() =>
        BitConverter.IsLittleEndian
            ? Read<double>()
            : BitConverter.Int64BitsToDouble(
                BinaryPrimitives.ReverseEndianness(
                    Read<long>()));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public short ReadInt16LittleEndian()
    {
        short value = Read<short>();

        if (!BitConverter.IsLittleEndian)
            value = BinaryPrimitives.ReverseEndianness(value);

        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int ReadInt32LittleEndian()
    {
        int value = Read<int>();

        if (!BitConverter.IsLittleEndian)
            value = BinaryPrimitives.ReverseEndianness(value);

        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long ReadInt64LittleEndian()
    {
        long value = Read<long>();

        if (!BitConverter.IsLittleEndian)
            value = BinaryPrimitives.ReverseEndianness(value);

        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float ReadSingleLittleEndian() =>
        BitConverter.IsLittleEndian
            ? Read<float>()
            : BitConverter.Int32BitsToSingle(
                BinaryPrimitives.ReverseEndianness(
                    Read<int>()));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ushort ReadUInt16LittleEndian()
    {
        ushort value = Read<ushort>();

        if (!BitConverter.IsLittleEndian)
            value = BinaryPrimitives.ReverseEndianness(value);

        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint ReadUInt32LittleEndian()
    {
        uint value = Read<uint>();

        if (!BitConverter.IsLittleEndian)
            value = BinaryPrimitives.ReverseEndianness(value);

        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ulong ReadUInt64LittleEndian()
    {
        ulong value = Read<ulong>();

        if (!BitConverter.IsLittleEndian)
            value = BinaryPrimitives.ReverseEndianness(value);

        return value;
    }

#if NET6_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Half ReadHalfLittleEndian() =>
        BitConverter.IsLittleEndian
            ? Read<Half>()
            : BitConverter.Int16BitsToHalf(
                BinaryPrimitives.ReverseEndianness(
                    Read<short>()));
#endif
#if NET8_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public nint ReadIntPtrLittleEndian()
    {
        nint value = Read<nint>();

        if (!BitConverter.IsLittleEndian)
            value = BinaryPrimitives.ReverseEndianness(value);

        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public nuint ReadUIntPtrLittleEndian()
    {
        nuint value = Read<nuint>();

        if (!BitConverter.IsLittleEndian)
            value = BinaryPrimitives.ReverseEndianness(value);

        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Int128 ReadInt128LittleEndian()
    {
        Int128 value = Read<Int128>();

        if (!BitConverter.IsLittleEndian)
            value = BinaryPrimitives.ReverseEndianness(value);

        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public UInt128 ReadUInt128LittleEndian()
    {
        UInt128 value = Read<UInt128>();

        if (!BitConverter.IsLittleEndian)
            value = BinaryPrimitives.ReverseEndianness(value);

        return value;
    }
#endif
}