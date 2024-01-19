// Copyright © 2024 Xpl0itR
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;

namespace SystemEx.Memory;

partial struct MemoryWriter
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteDoubleBigEndian(double value)
    {
        if (BitConverter.IsLittleEndian)
        {
            long temp = BinaryPrimitives.ReverseEndianness(
                BitConverter.DoubleToInt64Bits(value));
            Write(in temp);
        }
        else
        {
            Write(in value);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteHalfBigEndian(Half value)
    {
        if (BitConverter.IsLittleEndian)
        {
            short temp = BinaryPrimitives.ReverseEndianness(
                BitConverter.HalfToInt16Bits(value));
            Write(in temp);
        }
        else
        {
            Write(in value);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteInt16BigEndian(short value)
    {
        if (BitConverter.IsLittleEndian)
            value = BinaryPrimitives.ReverseEndianness(value);
        Write(in value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteInt32BigEndian(int value)
    {
        if (BitConverter.IsLittleEndian)
            value = BinaryPrimitives.ReverseEndianness(value);
        Write(in value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteInt64BigEndian(long value)
    {
        if (BitConverter.IsLittleEndian)
            value = BinaryPrimitives.ReverseEndianness(value);
        Write(in value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteInt128BigEndian(Int128 value)
    {
        if (BitConverter.IsLittleEndian)
            value = BinaryPrimitives.ReverseEndianness(value);
        Write(in value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteIntPtrBigEndian(nint value)
    {
        if (BitConverter.IsLittleEndian)
            value = BinaryPrimitives.ReverseEndianness(value);
        Write(in value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteSingleBigEndian(float value)
    {
        if (BitConverter.IsLittleEndian)
        {
            int temp = BinaryPrimitives.ReverseEndianness(
                BitConverter.SingleToInt32Bits(value));
            Write(in temp);
        }
        else
        {
            Write(in value);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteUInt16BigEndian(ushort value)
    {
        if (BitConverter.IsLittleEndian)
            value = BinaryPrimitives.ReverseEndianness(value);
        Write(in value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteUInt32BigEndian(uint value)
    {
        if (BitConverter.IsLittleEndian)
            value = BinaryPrimitives.ReverseEndianness(value);
        Write(in value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteUInt64BigEndian(ulong value)
    {
        if (BitConverter.IsLittleEndian)
            value = BinaryPrimitives.ReverseEndianness(value);
        Write(in value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteUInt128BigEndian(UInt128 value)
    {
        if (BitConverter.IsLittleEndian)
            value = BinaryPrimitives.ReverseEndianness(value);
        Write(in value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteUIntPtrBigEndian(nuint value)
    {
        if (BitConverter.IsLittleEndian)
            value = BinaryPrimitives.ReverseEndianness(value);
        Write(in value);
    }
}