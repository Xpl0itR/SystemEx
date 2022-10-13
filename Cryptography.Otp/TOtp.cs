// Copyright © 2022 Xpl0itR
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace SystemEx.Cryptography.Otp;

// ReSharper disable once InconsistentNaming
/// <remarks><see href="https://datatracker.ietf.org/doc/html/rfc6238" /></remarks>
public sealed class TOtp : HOtp
{
    public TOtp(ReadOnlySpan<byte> key)
        : base(key)
    { }

    public TOtp(ReadOnlySpan<byte> key, HashAlgorithmName hashAlg)
        : base(key, hashAlg)
    { }

    public TOtp(ReadOnlySpan<char> keyBase32)
        : base(keyBase32)
    { }

    public TOtp(ReadOnlySpan<char> keyBase32, HashAlgorithmName hashAlg)
        : base(keyBase32, hashAlg)
    { }

    public int TimeStep { get; set; } = 30;

    public int Now() =>
        ComputeCode(DateTimeOffset.UtcNow);

    public int ComputeCode(DateTimeOffset timestamp) =>
        ComputeCode(timestamp.ToUnixTimeSeconds());

    public int ComputeCode(long unixTimestamp)
    {
        Span<byte> counter = stackalloc byte[sizeof(long)];
        Unsafe.As<byte, long>(ref counter[0]) = unixTimestamp / TimeStep;

        if (BitConverter.IsLittleEndian)
            counter.Reverse();

        return base.ComputeCode(counter);
    }
}