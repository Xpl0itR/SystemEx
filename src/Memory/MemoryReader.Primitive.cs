// Copyright © 2024-2025 Xpl0itR
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CommunityToolkit.Diagnostics;
using CommunityToolkit.HighPerformance;

namespace SystemEx.Memory;

partial struct MemoryReader
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Read<T>() where T : unmanaged
    {
        int length = Unsafe.SizeOf<T>();
        Guard.IsGreaterThanOrEqualTo(Remaining, length);

        T value = Unsafe.ReadUnaligned<T>(ref Source);
        _position += length;

        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte[] ReadBytes(int count)
    {
        Guard.IsBetweenOrEqualTo(count, 0, Remaining);

        byte[] buffer =
#if NET5_0_OR_GREATER
            GC.AllocateUninitializedArray<byte>(count);
#else
            new byte[count];
#endif

        Unsafe.CopyBlockUnaligned(
            ref buffer.DangerousGetReference(),
            ref Source,
            unchecked((uint)count));

        _position += count;
        return buffer;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<byte> ReadSlice(int count)
    {
        Guard.IsBetweenOrEqualTo(count, 0, Remaining);

        ReadOnlySpan<byte> slice =
#if NET7_0_OR_GREATER
            MemoryMarshal.CreateReadOnlySpan(ref Source, count);
#else
            _span.Slice(_position, count);
#endif

        _position += count;
        return slice;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int ReadChars(int byteCount, ReadOnlySpan<char> chars, System.Text.Encoding? encoding = null) =>
        ReadChars(byteCount, ref chars.DangerousGetReference(), chars.Length, encoding);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe int ReadChars(int byteCount, ref char chars, int charCount, System.Text.Encoding? encoding = null)
    {
        Guard.IsBetweenOrEqualTo(byteCount, 0, Remaining);
        encoding ??= System.Text.Encoding.UTF8;

        fixed (byte* srcPtr = &Source)
        fixed (char* dstPtr = &chars)
        {
            _position += byteCount;
            return encoding.GetChars(srcPtr, byteCount, dstPtr, charCount);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe string ReadString(int byteCount, System.Text.Encoding? encoding = null)
    {
        Guard.IsBetweenOrEqualTo(byteCount, 0, Remaining);
        encoding ??= System.Text.Encoding.UTF8;
        
        fixed (byte* ptr = &Source)
        {
            _position += byteCount;
            return encoding.GetString(ptr, byteCount);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe string ReadCString()
    {
        switch (IndexOf((byte)'\0'))
        {
            case -1:
                return ThrowHelper.ThrowInvalidDataException<string>("Null terminator was not found before the end of the buffer.");
            case 0:
                return string.Empty;
            case var count:
                fixed (byte* ptr = &Source)
                {
                    _position += count;
                    return System.Text.Encoding.UTF8.GetString(ptr, count);
                }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe bool TryReadCString([NotNullWhen(true)] out string? str)
    {
        switch (IndexOf((byte)'\0'))
        {
            case -1:
                str = null;
                return false;
            case 0:
                str = string.Empty;
                return true;
            case var count:
                fixed (byte* ptr = &Source)
                    str = System.Text.Encoding.UTF8.GetString(ptr, count);
                _position += count;
                return true;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ReadCBoolean() =>
        Read<int>() != 0;
}