// Copyright © 2022-2025 Xpl0itR
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Security.Cryptography;
using SystemEx.Encoding;

namespace SystemEx.Cryptography.Otp;

// ReSharper disable MemberCanBeProtected.Global
/// <remarks><see href="https://datatracker.ietf.org/doc/html/rfc4226" /></remarks>
public class HOtp : IDisposable
{
    private readonly IncrementalHash _hmac;
    private readonly int _mod = 1000000;

    public HOtp(
#if NET5_0_OR_GREATER
        ReadOnlySpan<byte> key
#else
        byte[] key
#endif
    ) : this(key, HashAlgorithmName.SHA1) { }

    public HOtp(
#if NET5_0_OR_GREATER
        ReadOnlySpan<byte> key,
#else
        byte[] key,
#endif
        HashAlgorithmName hashAlg) =>
            _hmac = IncrementalHash.CreateHMAC(hashAlg, key);

    public HOtp(ReadOnlySpan<char> keyBase32)
        : this(keyBase32, HashAlgorithmName.SHA1)
    { }

    public HOtp(ReadOnlySpan<char> keyBase32, HashAlgorithmName hashAlg)
    {
#if NET5_0_OR_GREATER
        Span<byte> key = stackalloc byte
#else
        byte[] key = new byte
#endif
            [Base32.CountBytes(keyBase32.Length)];

        Base32.GetBytesUnchecked(keyBase32, key);

        _hmac = IncrementalHash.CreateHMAC(hashAlg, key);
    }

    public uint NumDigits
    {
        init => _mod = (int)Math.Pow(10, value);
    }

    public int ComputeCode(ReadOnlySpan<byte> counter)
    {
        Span<byte> hash = stackalloc byte[_hmac.HashLengthInBytes];

        lock (_hmac)
        {
            _hmac.AppendData(counter);
            _hmac.GetHashAndReset(hash);
        }

        int offset = hash[^1] & 0x0F;
        int code   = (hash[offset]     & 0x7F) << 24
                   | (hash[offset + 1] & 0xFF) << 16
                   | (hash[offset + 2] & 0xFF) << 8
                   | (hash[offset + 3] & 0xFF);

        return code % _mod;
    }

    /// <inheritdoc />
    public void Dispose() =>
        _hmac.Dispose();
}