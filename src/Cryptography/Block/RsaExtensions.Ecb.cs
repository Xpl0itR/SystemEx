// Copyright © 2023-2025 Xpl0itR
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
    extension(RSA rsa)
    {
        public int BlockLength
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => rsa.KeySize >> 3;
        }

        public int BlockLengthPkcs1
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => rsa.BlockLength - 11;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CiphertextLengthEcbPkcs1(int messageLength) =>
            (int)Math.Ceiling((double)messageLength / rsa.BlockLengthPkcs1) * rsa.BlockLength;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int MessageLengthMaxEcbPkcs1(int paddedLength) =>
            paddedLength / rsa.BlockLength * rsa.BlockLengthPkcs1;

        public void EncryptEcbPkcs1(ReadOnlySpan<byte> source, Span<byte> destination)
        {
            int plainBlockLength  = rsa.BlockLengthPkcs1;
            int paddedBlockLength = rsa.BlockLength;

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

        public int DecryptEcbPkcs1(ReadOnlySpan<byte> source, Span<byte> destination)
        {
            int plainBlockLength  = rsa.BlockLengthPkcs1;
            int paddedBlockLength = rsa.BlockLength;
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

        public byte[] EncryptEcbPkcs1(ReadOnlySpan<byte> source)
        {
            byte[] destination = GC.AllocateUninitializedArray<byte>(
                CiphertextLengthEcbPkcs1(rsa, source.Length));

            EncryptEcbPkcs1(rsa, source, destination);

            return destination;
        }

        public byte[] DecryptEcbPkcs1(ReadOnlySpan<byte> source)
        {
            using RentedArray<byte> buffer = new(
                MessageLengthMaxEcbPkcs1(rsa, source.Length));

            int count = DecryptEcbPkcs1(rsa, source, buffer);

            byte[] destination = GC.AllocateUninitializedArray<byte>(count);
            UnsafeEx.CopyBlockUnaligned(buffer.BackingArray, 0, destination, 0, count);

            return destination;
        }
    }


    // ReSharper disable once InconsistentNaming
    private const string RSACryptoServiceProviderObsoleteMessage =
        $"{nameof(RSACryptoServiceProvider)} does not implement the Span APIs, falling back to shim methods which copy data into heap allocated temporary buffers.";

    extension(RSACryptoServiceProvider rsa)
    {
        [Obsolete(RSACryptoServiceProviderObsoleteMessage)]
        public void EncryptEcbPkcs1(ReadOnlySpan<byte> source, Span<byte> destination) =>
            EncryptEcbPkcs1((RSA)rsa, source, destination);

        [Obsolete(RSACryptoServiceProviderObsoleteMessage)]
        public int DecryptEcbPkcs1(ReadOnlySpan<byte> source, Span<byte> destination) =>
            DecryptEcbPkcs1((RSA)rsa, source, destination);

        [Obsolete(RSACryptoServiceProviderObsoleteMessage)]
        public byte[] EncryptEcbPkcs1(ReadOnlySpan<byte> source) =>
            EncryptEcbPkcs1((RSA)rsa, source);

        [Obsolete(RSACryptoServiceProviderObsoleteMessage)]
        public byte[] DecryptEcbPkcs1(ReadOnlySpan<byte> source) =>
            DecryptEcbPkcs1((RSA)rsa, source);
    }
}
#endif