// Copyright © 2023 Xpl0itR
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

#if NET7_0_OR_GREATER
using System;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using SystemEx.Memory;

namespace SystemEx.Cryptography.Block;

public static partial class RsaExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int BlockLength(this RSA rsa) =>
        rsa.KeySize >> 3;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int BlockLengthPkcs1(this RSA rsa) =>
        BlockLength(rsa) - 11;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int CiphertextLengthEcbPkcs1(this RSA rsa, int messageLength) =>
        (int)Math.Ceiling((double)messageLength / BlockLengthPkcs1(rsa)) * BlockLength(rsa);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int MessageLengthMaxEcbPkcs1(this RSA rsa, int paddedLength) =>
        paddedLength / BlockLength(rsa) * BlockLengthPkcs1(rsa);

    public static void EncryptEcbPkcs1(this RSA rsa, ReadOnlySpan<byte> source, Span<byte> destination)
    {
        int plainBlockLength  = BlockLengthPkcs1(rsa);
        int paddedBlockLength = BlockLength(rsa);

        int numBlocks = (int)Math.Ceiling((double)source.Length / plainBlockLength);
        for (int i = 0; i < numBlocks; i++)
        {
            int sourceOffset = i * plainBlockLength;
            int sourceLength = Math.Min(plainBlockLength, source.Length - sourceOffset);

            rsa.Encrypt(
                source.Slice(sourceOffset, sourceLength),
                destination.Slice(i * paddedBlockLength, paddedBlockLength),
                RSAEncryptionPadding.Pkcs1);
        }
    }

    public static int DecryptEcbPkcs1(this RSA rsa, ReadOnlySpan<byte> source, Span<byte> destination)
    {
        int plainBlockLength  = BlockLengthPkcs1(rsa);
        int paddedBlockLength = BlockLength(rsa);
        int messageLength     = 0;

        int numBlocks = source.Length / paddedBlockLength;
        for (int i = 0; i < numBlocks; i++)
        {
            messageLength += rsa.Decrypt(
                source.Slice(i * paddedBlockLength, paddedBlockLength),
                destination.Slice(i * plainBlockLength, plainBlockLength),
                RSAEncryptionPadding.Pkcs1);
        }

        return messageLength;
    }

    public static byte[] EncryptEcbPkcs1(this RSA rsa, ReadOnlySpan<byte> source)
    {
        byte[] destination = GC.AllocateUninitializedArray<byte>(
            CiphertextLengthEcbPkcs1(rsa, source.Length));

        EncryptEcbPkcs1(rsa, source, destination);

        return destination;
    }

    public static byte[] DecryptEcbPkcs1(this RSA rsa, ReadOnlySpan<byte> source)
    {
        using RentedArray<byte> buffer = new(
            MessageLengthMaxEcbPkcs1(rsa, source.Length));

        int count = DecryptEcbPkcs1(rsa, source, buffer);

        byte[] destination = GC.AllocateUninitializedArray<byte>(count);
        UnsafeEx.CopyBlockUnaligned(buffer.BackingArray, 0, destination, 0, count);

        return destination;
    }


    // ReSharper disable once InconsistentNaming
    private const string RSACryptoServiceProviderObsoleteMessage =
        $"{nameof(RSACryptoServiceProvider)} does not implement the Span APIs, falling back to shim methods which copy data into heap allocated temporary buffers.";

    [Obsolete(RSACryptoServiceProviderObsoleteMessage)]
    public static void EncryptEcbPkcs1(this RSACryptoServiceProvider rsa, ReadOnlySpan<byte> source, Span<byte> destination) =>
        EncryptEcbPkcs1((RSA)rsa, source, destination);

    [Obsolete(RSACryptoServiceProviderObsoleteMessage)]
    public static void DecryptEcbPkcs1(this RSACryptoServiceProvider rsa, ReadOnlySpan<byte> source, Span<byte> destination) =>
        DecryptEcbPkcs1((RSA)rsa, source, destination);
}
#endif