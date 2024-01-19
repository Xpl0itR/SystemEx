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
    public void WriteDoubleLittleEndian(double value)
    {
        if (BitConverter.IsLittleEndian)
        {
            Write(in value);
        }
        else
        {
            long temp = BinaryPrimitives.ReverseEndianness(
                BitConverter.DoubleToInt64Bits(value));
            Write(in temp);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteHalfLittleEndian(Half value)
    {
        if (BitConverter.IsLittleEndian)
        {
            Write(in value);
        }
        else
        {
            short temp = BinaryPrimitives.ReverseEndianness(
                BitConverter.HalfToInt16Bits(value));
            Write(in temp);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteInt16LittleEndian(short value)
    {
        if (!BitConverter.IsLittleEndian)
            value = BinaryPrimitives.ReverseEndianness(value);
        Write(in value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteInt32LittleEndian(int value)
    {
        if (!BitConverter.IsLittleEndian)
            value = BinaryPrimitives.ReverseEndianness(value);
        Write(in value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteInt64LittleEndian(long value)
    {
        if (!BitConverter.IsLittleEndian)
            value = BinaryPrimitives.ReverseEndianness(value);
        Write(in value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteInt128LittleEndian(Int128 value)
    {
        if (!BitConverter.IsLittleEndian)
            value = BinaryPrimitives.ReverseEndianness(value);
        Write(in value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteIntPtrLittleEndian(nint value)
    {
        if (!BitConverter.IsLittleEndian)
            value = BinaryPrimitives.ReverseEndianness(value);
        Write(in value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteSingleLittleEndian(float value)
    {
        if (BitConverter.IsLittleEndian)
        {
            Write(in value);
        }
        else
        {
            int temp = BinaryPrimitives.ReverseEndianness(
                BitConverter.SingleToInt32Bits(value));
            Write(in temp);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteUInt16LittleEndian(ushort value)
    {
        if (!BitConverter.IsLittleEndian)
            value = BinaryPrimitives.ReverseEndianness(value);
        Write(in value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteUInt32LittleEndian(uint value)
    {
        if (!BitConverter.IsLittleEndian)
            value = BinaryPrimitives.ReverseEndianness(value);
        Write(in value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteUInt64LittleEndian(ulong value)
    {
        if (!BitConverter.IsLittleEndian)
            value = BinaryPrimitives.ReverseEndianness(value);
        Write(in value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteUInt128LittleEndian(UInt128 value)
    {
        if (!BitConverter.IsLittleEndian)
            value = BinaryPrimitives.ReverseEndianness(value);
        Write(in value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteUIntPtrLittleEndian(nuint value)
    {
        if (!BitConverter.IsLittleEndian)
            value = BinaryPrimitives.ReverseEndianness(value);
        Write(in value);
    }
}