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
    public double ReadDoubleBigEndian() =>
        BitConverter.IsLittleEndian
            ? BitConverter.Int64BitsToDouble(
                BinaryPrimitives.ReverseEndianness(
                    Read<long>()))
            : Read<double>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Half ReadHalfBigEndian() =>
        BitConverter.IsLittleEndian
            ? BitConverter.Int16BitsToHalf(
                BinaryPrimitives.ReverseEndianness(
                    Read<short>()))
            : Read<Half>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public short ReadInt16BigEndian()
    {
        short value = Read<short>();

        if (BitConverter.IsLittleEndian)
            value = BinaryPrimitives.ReverseEndianness(value);

        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int ReadInt32BigEndian()
    {
        int value = Read<int>();

        if (BitConverter.IsLittleEndian)
            value = BinaryPrimitives.ReverseEndianness(value);

        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long ReadInt64BigEndian()
    {
        long value = Read<long>();

        if (BitConverter.IsLittleEndian)
            value = BinaryPrimitives.ReverseEndianness(value);

        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Int128 ReadInt128BigEndian()
    {
        Int128 value = Read<Int128>();

        if (BitConverter.IsLittleEndian)
            value = BinaryPrimitives.ReverseEndianness(value);

        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public nint ReadIntPtrBigEndian()
    {
        nint value = Read<nint>();

        if (BitConverter.IsLittleEndian)
            value = BinaryPrimitives.ReverseEndianness(value);

        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float ReadSingleBigEndian() =>
        BitConverter.IsLittleEndian
            ? BitConverter.Int32BitsToSingle(
                BinaryPrimitives.ReverseEndianness(
                    Read<int>()))
            : Read<float>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ushort ReadUInt16BigEndian()
    {
        ushort value = Read<ushort>();

        if (BitConverter.IsLittleEndian)
            value = BinaryPrimitives.ReverseEndianness(value);

        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint ReadUInt32BigEndian()
    {
        uint value = Read<uint>();

        if (BitConverter.IsLittleEndian)
            value = BinaryPrimitives.ReverseEndianness(value);

        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ulong ReadUInt64BigEndian()
    {
        ulong value = Read<ulong>();

        if (BitConverter.IsLittleEndian)
            value = BinaryPrimitives.ReverseEndianness(value);

        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public UInt128 ReadUInt128BigEndian()
    {
        UInt128 value = Read<UInt128>();

        if (BitConverter.IsLittleEndian)
            value = BinaryPrimitives.ReverseEndianness(value);

        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public nuint ReadUIntPtrBigEndian()
    {
        nuint value = Read<nuint>();

        if (BitConverter.IsLittleEndian)
            value = BinaryPrimitives.ReverseEndianness(value);

        return value;
    }
}