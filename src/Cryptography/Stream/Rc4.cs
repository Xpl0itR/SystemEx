// Copyright © 2023-2024 Xpl0itR
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Runtime.CompilerServices;
using CommunityToolkit.Diagnostics;

namespace SystemEx.Cryptography.Stream;

/// <remarks><see href="https://en.wikipedia.org/wiki/RC4" /></remarks>
public sealed partial class Rc4
{
    private readonly byte[] _s;
    private int _i;
    private int _j;

    public Rc4(ReadOnlySpan<byte> key, byte[]? state = null)
    {
        Guard.IsBetweenOrEqualTo(key.Length, 1, KeyLengthMax);

        if (state is null)
        {
            _s = GC.AllocateUninitializedArray<byte>(KeyLengthMax);
        }
        else
        {
            Guard.HasSizeGreaterThanOrEqualTo(state, KeyLengthMax);
            _s = state;
        }
        
        InitState(key, _s);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte NextByte() =>
        NextByte(_s, ref _i, ref _j);

    public void XorBlock(Span<byte> block)
    {
        for (int i = 0; i < block.Length; i++)
        {
            block[i] ^= NextByte();
        }
    }
}