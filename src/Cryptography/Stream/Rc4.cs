// Copyright © 2023 Xpl0itR
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using CommunityToolkit.Diagnostics;

namespace SystemEx.Cryptography.Stream;

/// <remarks><see href="https://en.wikipedia.org/wiki/RC4" /></remarks>
public sealed partial class Rc4
{
    private readonly byte[] _s;

    private int _i;
    private int _j;

    public Rc4(ReadOnlySpan<byte> key)
    {
        Guard.IsNotEmpty(key);
        Guard.HasSizeLessThanOrEqualTo(key, 256);

        _s = GC.AllocateUninitializedArray<byte>(256);
        InitState(key, _s);
    }

    public byte NextByte() =>
        NextByte(_s, ref _i, ref _j);
}