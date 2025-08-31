// Copyright © 2023-2025 Xpl0itR
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

#if NET6_0_OR_GREATER
using System;
using System.Security.Cryptography;
using SystemEx.Memory;
using CommunityToolkit.Diagnostics;

namespace SystemEx.Cryptography.Block;

public static partial class AesExtensions
{
    private const int BlockLength = 16;
    private static readonly byte[] Zero16 = new byte[BlockLength];

    extension(Aes aes)
    {
        /// <remarks><see href="https://datatracker.ietf.org/doc/html/rfc4493" /></remarks>
        public byte[] ComputeCmac(ReadOnlySpan<byte> source)
        {
            byte[] destination = GC.AllocateUninitializedArray<byte>(BlockLength);
            ComputeCmac(aes, source, destination);

            return destination;
        }

        /// <remarks><see href="https://datatracker.ietf.org/doc/html/rfc4493" /></remarks>
        public void ComputeCmac(ReadOnlySpan<byte> source, Span<byte> destination)
        {
            Guard.HasSizeGreaterThanOrEqualTo(destination, BlockLength);

            ReadOnlySpan<byte> zero = new(Zero16);
            Span<byte>         key  = stackalloc byte[BlockLength];

            aes.EncryptCbc(zero, zero, key, PaddingMode.None);
            ShiftKey(key);

            bool fullBlock = source.Length > 0 && source.Length % 16 == 0;
            int bufferLen = fullBlock
                ? source.Length
                : source.Length + (BlockLength - source.Length % BlockLength);

            using RentedArray<byte> rented = new(bufferLen, clearMemory: true);
            Span<byte>              buffer = rented.AsSpan();

            source.CopyTo(buffer);

            if (!fullBlock)
            {
                ShiftKey(key);
                buffer[source.Length] = 0x80;
            }

            int start = bufferLen - BlockLength;
            for (int i = 0; i < BlockLength; i++)
            {
                buffer[start + i] ^= key[i];
            }

            aes.EncryptCbc(buffer, zero, buffer, PaddingMode.None);
            buffer.Slice(start, BlockLength).CopyTo(destination);
        }
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
#endif