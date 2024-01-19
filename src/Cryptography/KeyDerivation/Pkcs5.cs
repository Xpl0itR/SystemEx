// Copyright © 2023 Xpl0itR
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Buffers.Binary;
using System.Security.Cryptography;
using SystemEx.Memory;
using CommunityToolkit.Diagnostics;

namespace SystemEx.Cryptography.KeyDerivation;

/// <remarks><see href="https://datatracker.ietf.org/doc/html/rfc2898" /></remarks>
public static class Pkcs5
{
    /// <summary>
    ///     Derive a cryptographically secure key using Password-Based Key Derivation Function 2
    /// </summary>
    /// <param name="prf">Pseudo-Random Function (PRF) used to hash the blocks</param>
    /// <param name="password">Password (P) to be used as a seed by the Pseudo-Random Function</param>
    /// <param name="salt">Salt (S) to be used as part of the input data of the first hash of every block</param>
    /// <param name="iterations">Number of rounds (c) of hashing per block</param>
    /// <param name="length">Intended length of the derived key (dkLen), a positive integer, at most (2^32 - 1) * hLen</param>
    /// <returns>The Derived key (DK) produced by this function</returns>
    /// <remarks><see href="https://tools.ietf.org/html/rfc2898#section-5.2" /></remarks>
    public static byte[] Pbkdf2(HashAlgorithmName prf, ReadOnlySpan<byte> password, ReadOnlySpan<byte> salt, int iterations, int length)
    {
        byte[] derivedKey = GC.AllocateUninitializedArray<byte>(length);
        Pbkdf2(prf, password, salt, iterations, derivedKey);

        return derivedKey;
    }

    /// <summary>
    ///     Derive a cryptographically secure key using Password-Based Key Derivation Function 2
    /// </summary>
    /// <param name="prf">Pseudo-Random Function (PRF) used to hash the blocks</param>
    /// <param name="password">Password (P) to be used as a seed by the Pseudo-Random Function</param>
    /// <param name="salt">Salt (S) to be used as part of the input data of the first hash of every block</param>
    /// <param name="iterations">Number of rounds (c) of hashing per block</param>
    /// <param name="derivedKey">The Derived key (DK) produced by this function</param>
    /// <remarks><see href="https://tools.ietf.org/html/rfc2898#section-5.2" /></remarks>
    public static void Pbkdf2(HashAlgorithmName prf, ReadOnlySpan<byte> password, ReadOnlySpan<byte> salt, int iterations, Span<byte> derivedKey)
    {
        using IncrementalHash hmac = IncrementalHash.CreateHMAC(prf, password);
        Pbkdf2(hmac, salt, iterations, derivedKey);
    }

    /// <summary>
    ///     Derive a cryptographically secure key using Password-Based Key Derivation Function 2
    /// </summary>
    /// <param name="hmac">The HMAC function, constructed from the Pseudo-Random Function (PRF) and the Password (P), used to hash the blocks</param>
    /// <param name="salt">Salt (S) to be used as part of the input data of the first hash of every block</param>
    /// <param name="iterations">Number of rounds (c) of hashing per block</param>
    /// <param name="derivedKey">The Derived key (DK) produced by this function</param>
    /// <remarks><see href="https://tools.ietf.org/html/rfc2898#section-5.2" /></remarks>
    public static void Pbkdf2(IncrementalHash hmac, ReadOnlySpan<byte> salt, int iterations, Span<byte> derivedKey)
    {
        int dkLen = derivedKey.Length;
        int hLen  = hmac.HashLengthInBytes;

        if (dkLen > (2 ^ 32 - 1) * hLen)
        {
            ThrowHelper.ThrowArgumentOutOfRangeException(nameof(derivedKey), "derived key too long");
        }

        Span<byte> hash = stackalloc byte[hLen];

        int blockCount = (int)Math.Ceiling((double)dkLen / hLen);
        for (int blockNum = 1; blockNum <= blockCount; blockNum++)
        {
            int index = BitConverter.IsLittleEndian
                ? BinaryPrimitives.ReverseEndianness(blockNum)
                : blockNum;

            hmac.AppendData(salt);
            hmac.AppendData(index.AsReadOnlyBytes());
            hmac.GetHashAndReset(hash);

            int blockOffset = (blockNum - 1) * hLen;
            int blockLength = Math.Min(dkLen - blockOffset, hLen);

            Span<byte> block = derivedKey.Slice(blockOffset, blockLength);
            CopyHelper.CopySpanUnchecked(
                hash.SliceReadOnly(0, blockLength),
                block);

            for (int i = 1; i < iterations; i++)
            {
                hmac.AppendData(hash);
                hmac.GetHashAndReset(hash);

                for (int j = 0; j < blockLength; j++)
                {
                    block[j] ^= hash[j];
                }
            }
        }
    }
}