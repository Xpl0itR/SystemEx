// Copyright © 2026 Xpl0itR
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CommunityToolkit.Diagnostics;
#if NET7_0_OR_GREATER
using System.Runtime.Intrinsics;
#endif
using CommunityToolkit.HighPerformance;

namespace SystemEx.Memory;

public static class XorHelper
{
    public static void XorFast(Span<byte> dest, ReadOnlySpan<byte> left, ReadOnlySpan<byte> right)
    {
        Guard.HasSizeGreaterThanOrEqualTo(left,  dest.Length);
        Guard.HasSizeGreaterThanOrEqualTo(right, dest.Length);

        XorFastUnsafe(dest, left, right);
    }

    public static void XorFast(byte[] dest, byte[] left, byte[] right)
    {
        Guard.HasSizeGreaterThanOrEqualTo(left,  dest.Length);
        Guard.HasSizeGreaterThanOrEqualTo(right, dest.Length);

        XorFastUnsafe(dest, left, right);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void XorFastUnsafe(Span<byte> dest, ReadOnlySpan<byte> left, ReadOnlySpan<byte> right) =>
        XorFastUnsafe(
            ref MemoryMarshal.GetReference(dest),
            ref MemoryMarshal.GetReference(left),
            ref MemoryMarshal.GetReference(right),
            unchecked((nuint)dest.Length));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void XorFastUnsafe(byte[] dest, byte[] left, byte[] right) =>
        XorFastUnsafe(
            ref dest.DangerousGetReference(),
            ref left.DangerousGetReference(),
            ref right.DangerousGetReference(),
            unchecked((nuint)dest.Length));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe void XorFastUnsafe(byte* dest, byte* left, byte* right, nuint length) =>
        XorFastUnsafe(
            ref Unsafe.AsRef<byte>(dest),
            ref Unsafe.AsRef<byte>(left),
            ref Unsafe.AsRef<byte>(right),
            length);

    public static void XorFastUnsafe(ref byte dest, ref byte left, ref byte right, nuint length)
    {
        nuint offset = 0;
#if NET8_0_OR_GREATER
        if (Vector512.IsHardwareAccelerated)
        {
            while (offset + 128 <= length)
            {
                Vector512<byte> left1  = Vector512.LoadUnsafe(ref left,  offset);
                Vector512<byte> left2  = Vector512.LoadUnsafe(ref left,  offset + 64);
                Vector512<byte> right1 = Vector512.LoadUnsafe(ref right, offset);
                Vector512<byte> right2 = Vector512.LoadUnsafe(ref right, offset + 64);

                (left1 ^ right1).StoreUnsafe(ref dest, offset);
                (left2 ^ right2).StoreUnsafe(ref dest, offset + 64);
                offset += 128;
            }
            if (offset + 64 <= length)
            {
                (Vector512.LoadUnsafe(ref left, offset) ^ Vector512.LoadUnsafe(ref right, offset)).StoreUnsafe(ref dest, offset);
                offset += 64;
            }
        }
#endif
#if NET7_0_OR_GREATER
        if (Vector256.IsHardwareAccelerated)
        {
            while (offset + 64 <= length)
            {
                Vector256<byte> left1  = Vector256.LoadUnsafe(ref left,  offset);
                Vector256<byte> left2  = Vector256.LoadUnsafe(ref left,  offset + 32);
                Vector256<byte> right1 = Vector256.LoadUnsafe(ref right, offset);
                Vector256<byte> right2 = Vector256.LoadUnsafe(ref right, offset + 32);

                (left1 ^ right1).StoreUnsafe(ref dest, offset);
                (left2 ^ right2).StoreUnsafe(ref dest, offset + 32);
                offset += 64;
            }
            if (offset + 32 <= length)
            {
                (Vector256.LoadUnsafe(ref left, offset) ^ Vector256.LoadUnsafe(ref right, offset)).StoreUnsafe(ref dest, offset);
                offset += 32;
            }
        }

        if (Vector128.IsHardwareAccelerated)
        {
            while (offset + 32 <= length)
            {
                Vector128<byte> left1  = Vector128.LoadUnsafe(ref left,  offset);
                Vector128<byte> left2  = Vector128.LoadUnsafe(ref left,  offset + 16);
                Vector128<byte> right1 = Vector128.LoadUnsafe(ref right, offset);
                Vector128<byte> right2 = Vector128.LoadUnsafe(ref right, offset + 16);

                (left1 ^ right1).StoreUnsafe(ref dest, offset);
                (left2 ^ right2).StoreUnsafe(ref dest, offset + 16);
                offset += 32;
            }
            if (offset + 16 <= length)
            {
                (Vector128.LoadUnsafe(ref left, offset) ^ Vector128.LoadUnsafe(ref right, offset)).StoreUnsafe(ref dest, offset);
                offset += 16;
            }
        }

        if (Vector64.IsHardwareAccelerated)
        {
            while (offset + 16 <= length)
            {
                Vector64<byte> left1  = Vector64.LoadUnsafe(ref left,  offset);
                Vector64<byte> left2  = Vector64.LoadUnsafe(ref left,  offset + 8);
                Vector64<byte> right1 = Vector64.LoadUnsafe(ref right, offset);
                Vector64<byte> right2 = Vector64.LoadUnsafe(ref right, offset + 8);

                (left1 ^ right1).StoreUnsafe(ref dest, offset);
                (left2 ^ right2).StoreUnsafe(ref dest, offset + 8);
                offset += 16;
            }
            if (offset + 8 <= length)
            {
                (Vector64.LoadUnsafe(ref left, offset) ^ Vector64.LoadUnsafe(ref right, offset)).StoreUnsafe(ref dest, offset);
                offset += 8;
            }
        }
#endif
        for (; offset < length; offset++)
        {
            Unsafe.AddByteOffset(ref dest, offset) = (byte)(Unsafe.AddByteOffset(ref left, offset) ^ Unsafe.AddByteOffset(ref right, offset));
        }
    }
}