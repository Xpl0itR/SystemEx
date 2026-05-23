// Copyright © 2023-2026 Xpl0itR
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Runtime.CompilerServices;
using CommunityToolkit.Diagnostics;
using SystemEx.Memory;

namespace SystemEx.Cryptography.Stream;

/// <remarks><see href="https://en.wikipedia.org/wiki/RC4" /></remarks>
public sealed partial class Rc4
{
    public const int StateLength = 256;

    private readonly byte[] _s;

    private int _i;
    private int _j;

    public Rc4(ReadOnlySpan<byte> key)
    {
        Guard.IsBetweenOrEqualTo(key.Length, 1, StateLength);

        _s = new byte[StateLength];

        InitState(key, _s);
    }

    public byte NextByte() =>
        NextByte(_s, ref _i, ref _j);

    public void Fill(Span<byte> keyStream) =>
        Fill(keyStream, _s, ref _i, ref _j);

    public void XorBlock(ReadOnlySpan<byte> src, Span<byte> dest)
    {
        Guard.HasSizeGreaterThanOrEqualTo(src, dest.Length);

        Span<byte> keyStream = stackalloc byte[dest.Length];
        Fill(keyStream);

        XorHelper.XorFastUnsafe(dest, src, keyStream);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void XorSingleBlock(ReadOnlySpan<byte> key, ReadOnlySpan<byte> src, Span<byte> dest)
    {
        Guard.IsBetweenOrEqualTo(key.Length, 1, StateLength);
        Guard.HasSizeGreaterThanOrEqualTo(src, dest.Length);

        Span<byte> s = stackalloc byte[StateLength];
        InitState(key, s);

        int i = 0, j = 0;
        Span<byte> keyStream = stackalloc byte[dest.Length];
        Fill(keyStream, s, ref i, ref j);

        XorHelper.XorFastUnsafe(dest, src, keyStream);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void InitState(ReadOnlySpan<byte> key, Span<byte> s)
    {
        for (int i = 0; i < StateLength; i++)
        {
            s[i] = (byte)i;
        }

        for (int i = 0, j = 0; i < StateLength; i++)
        {
            j = (j + s[i] + key[i % key.Length]) % StateLength;

            (s[i], s[j]) = (s[j], s[i]);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static byte NextByte(Span<byte> s, ref int i, ref int j)
    {
        i = (i + 1)    % StateLength;
        j = (j + s[i]) % StateLength;

        (s[i], s[j]) = (s[j], s[i]);

        return s[(s[i] + s[j]) % StateLength];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Fill(Span<byte> keyStream, Span<byte> s, ref int i, ref int j)
    {
        for (int k = 0; k < keyStream.Length; k++)
        {
            keyStream[k] = NextByte(s, ref i, ref j);
        }
    }
}