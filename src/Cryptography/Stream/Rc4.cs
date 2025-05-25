// Copyright © 2023-2025 Xpl0itR
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using CommunityToolkit.Diagnostics;

namespace SystemEx.Cryptography.Stream;

/// <remarks><see href="https://en.wikipedia.org/wiki/RC4" /></remarks>
public sealed class Rc4 : IDisposable
{
    private const int KeyLengthMax = 256;

    private readonly ArrayPool<byte> _arrayPool;
    private readonly byte[] _s;

    private int _i;
    private int _j;

    public Rc4(ReadOnlySpan<byte> key, ArrayPool<byte>? arrayPool = null)
    {
        Guard.IsBetweenOrEqualTo(key.Length, 1, KeyLengthMax);

        _arrayPool = arrayPool ?? ArrayPool<byte>.Shared;
        _s         = _arrayPool.Rent(KeyLengthMax);

        InitState(key, _s);
    }

    public byte NextByte() =>
        NextByte(_s, ref _i, ref _j);

    public void XorBlock(Span<byte> block)
    {
        for (int i = 0; i < block.Length; i++)
        {
            block[i] ^= NextByte();
        }
    }

    public void Dispose() =>
        _arrayPool.Return(_s);

    public static void XorBlock(ReadOnlySpan<byte> key, Span<byte> block)
    {
        Guard.IsBetweenOrEqualTo(key.Length, 1, KeyLengthMax);

        Span<byte> s = stackalloc byte[KeyLengthMax];
        InitState(key, s);

        for (int i = 0, j = 0, k = 0; k < block.Length; k++)
        {
            block[k] ^= NextByte(s, ref i, ref j);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void InitState(ReadOnlySpan<byte> key, Span<byte> s)
    {
        for (int i = 0; i < KeyLengthMax; i++)
        {
            s[i] = (byte)i;
        }

        for (int i = 0, j = 0; i < KeyLengthMax; i++)
        {
            j = (j + s[i] + key[i % key.Length]) % KeyLengthMax;

            (s[i], s[j]) = (s[j], s[i]);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static byte NextByte(Span<byte> s, ref int i, ref int j)
    {
        i = (i + 1)    % KeyLengthMax;
        j = (j + s[i]) % KeyLengthMax;

        (s[i], s[j]) = (s[j], s[i]);

        return s[(s[i] + s[j]) % KeyLengthMax];
    }
}