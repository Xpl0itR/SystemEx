// Copyright © 2023-2024 Xpl0itR
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Security.Cryptography;
using SystemEx.Memory;
using CommunityToolkit.Diagnostics;

namespace SystemEx.Cryptography.Block;

public static partial class AesExtensions
{
    private const int BlockLength = 16;
    private static readonly byte[] Zero16 = new byte[BlockLength];

    /// <remarks><see href="https://datatracker.ietf.org/doc/html/rfc4493" /></remarks>
    public static byte[] ComputeCmac(this Aes aes, ReadOnlySpan<byte> source)
    {
        byte[] destination = GC.AllocateUninitializedArray<byte>(BlockLength);
        ComputeCmac(aes, source, destination);

        return destination;
    }

    /// <remarks><see href="https://datatracker.ietf.org/doc/html/rfc4493" /></remarks>
    public static void ComputeCmac(this Aes aes, ReadOnlySpan<byte> source, Span<byte> destination)
    {
        Guard.HasSizeGreaterThanOrEqualTo(destination, BlockLength);

        ReadOnlySpan<byte> zero = new(Zero16);
        Span<byte>         key  = stackalloc byte[BlockLength];

        aes.EncryptCbc(zero, zero, key, PaddingMode.None);
        ShiftKey(key);

        bool fullBlock = source.Length > 0 && source.Length % 16 == 0;
        int  bufferLen = fullBlock
            ? source.Length
            : source.Length + (BlockLength - source.Length % BlockLength);

        using RentedMemory<byte> rented = new(bufferLen);
        Span<byte> buffer = rented.AsSpan();

        CopyHelper.CopySpanUnchecked(source, buffer);

        if (!fullBlock)
        {
            ShiftKey(key);
            buffer[source.Length] = 0x80;
        }

        for (int i = 0; i < BlockLength; i++)
        {
            buffer[buffer.Length - BlockLength + i] ^= key[i];
        }
        
        aes.EncryptCbc(buffer, zero, buffer, PaddingMode.None);

        CopyHelper.CopySpanUnchecked(
            buffer.SliceReadOnly(bufferLen - BlockLength, BlockLength),
            destination);
    }

    private static void ShiftKey(Span<byte> key)
    {
        int  overflow = 0;
        bool xorLast  = (key[0] & 0x80) == 0x80;

        for (int i = key.Length - 1; i >= 0; i--)
        {
            int byt = key[i] << 1;

            key[i]   = (byte)((byt & 0xff) + overflow);
            overflow = (byt & 0xff00) >> 8;
        }

        if (xorLast)
        {
            key[^1] ^= 0x87;
        }
    }
}