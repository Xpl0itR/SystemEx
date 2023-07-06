// Copyright © 2023 Xpl0itR
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Security.Cryptography;
using SystemEx.HighPerformance;

namespace SystemEx.Cryptography.Block;

public static partial class AesExtensions
{
    /// <remarks><see href="https://datatracker.ietf.org/doc/html/rfc4493" /></remarks>
    public static byte[] ComputeCmac(this Aes aes, ReadOnlySpan<byte> source)
    {
        byte[] destination = GC.AllocateUninitializedArray<byte>(16);
        ComputeCmac(aes, source, destination);

        return destination;
    }

    /// <remarks><see href="https://datatracker.ietf.org/doc/html/rfc4493" /></remarks>
    public static void ComputeCmac(this Aes aes, ReadOnlySpan<byte> source, Span<byte> destination)
    {
        ReadOnlySpan<byte> zero = new(Zero16);
        Span<byte>         key  = stackalloc byte[16];

        aes.EncryptCbc(zero, zero, key, PaddingMode.None);
        ShiftKey(key);

        bool fullBlock = source.Length > 0 && source.Length % 16 == 0;
        int  bufferLen = fullBlock ? source.Length : source.Length + (16 - source.Length % 16);

        Span<byte> buffer = stackalloc byte[bufferLen];
        CopyHelper.CopySpanUnchecked(source, buffer);

        if (!fullBlock)
        {
            ShiftKey(key);
            buffer[source.Length] = 0x80;
        }

        for (int i = 0; i < 16; i++)
        {
            buffer[buffer.Length - 16 + i] ^= key[i];
        }
        
        aes.EncryptCbc(buffer, zero, buffer, PaddingMode.None);

        CopyHelper.CopySpanUnchecked(
            buffer.SliceReadOnly(buffer.Length - 16, 16),
            destination);
    }

    private static readonly byte[] Zero16 = new byte[16];

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