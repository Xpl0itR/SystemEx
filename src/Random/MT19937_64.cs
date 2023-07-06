// Copyright © 2023 Xpl0itR
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Runtime.CompilerServices;
using CommunityToolkit.Diagnostics;

namespace SystemEx.Random;

// ReSharper disable once InconsistentNaming, CommentTypo
/// <summary>
///     64-bit version of the Mersenne Twister pseudorandom number generator.
/// </summary>
/// <remarks><see href="http://www.math.sci.hiroshima-u.ac.jp/m-mat/MT/VERSIONS/C-LANG/mt19937-64.c"/></remarks>
public ref struct MT19937_64
{
    public const ulong DefaultSeed      = 5489UL;
    public const ulong DefaultSeedArray = 19650218UL;
    public const int   Nn               = 312;

    private const int   Mm          = 156;
    private const ulong MatrixA     = 0xB5026F5AA96619E9UL;
    private const ulong UpperMask   = 0xFFFFFFFF80000000UL; // Most significant 33 bits
    private const ulong LowerMask   = 0X7FFFFFFFUL;         // Least significant 31 bits

    private static readonly ulong[] Mag01 = { 0UL, MatrixA };

    private readonly Span<ulong> _mt;
    private int _mti;

    // ReSharper disable once CommentTypo
    /// <summary>
    ///     Creates an instance of the 64-bit Mersenne Twister and initializes state with a seed
    /// </summary>
    public MT19937_64(Span<ulong> mt, ulong seed = DefaultSeed)
    {
        Guard.HasSizeEqualTo(mt, Nn);
        _mt = mt;

        Init(seed);
    }

    // ReSharper disable once CommentTypo
    /// <summary>
    ///     Creates an instance of the 64-bit Mersenne Twister and initializes state with a seed and an array
    /// </summary>
    public MT19937_64(Span<ulong> mt, ReadOnlySpan<ulong> array, ulong seed = DefaultSeedArray)
    {
        Guard.HasSizeEqualTo(mt, Nn);
        _mt = mt;

        InitByArray(seed, array);
    }

    /// <summary>
    ///     Initializes state with a seed
    /// </summary>
    public void Init(ulong seed)
    {
        _mt[0] = seed;

        for (_mti = 1; _mti < Nn; _mti++)
        {
            _mt[_mti] = 6364136223846793005UL * (_mt[_mti - 1] ^ (_mt[_mti - 1] >> 62)) + (ulong)_mti;
        }
    }

    /// <summary>
    ///     Initialize state with a seed and an array
    /// </summary>
    public void InitByArray(ulong seed, ReadOnlySpan<ulong> array)
    {
        Init(seed);

        int i = 1;
        for (int j = 0, k = Math.Max(Nn, array.Length); k > 0; k--)
        {
            _mt[i] = (_mt[i] ^ ((_mt[i - 1] ^ (_mt[i - 1] >> 62)) * 3935559000370003845UL)) + array[j] + (ulong)j;
            i++;
            j++;

            if (i >= Nn)
            {
                _mt[0] = _mt[Nn - 1]; i = 1;
            }

            if (j >= array.Length)
            {
                j = 0;
            }
        }

        for (int k = Nn - 1; k > 0; k--)
        {
            _mt[i] = (_mt[i] ^ ((_mt[i - 1] ^ (_mt[i - 1] >> 62)) * 2862933555777941757UL)) - (ulong)i;
            i++;

            if (i >= Nn)
            {
                _mt[0] = _mt[Nn - 1]; i = 1;
            }
        }

        _mt[0] = 1UL << 63;
    }

    /// <summary>
    ///     Generates a random number on [0, 2^64-1] interval
    /// </summary>
    public ulong Int64()
    {
        ulong x;
        int   i;

        if (_mti >= Nn)
        {
            for (i = 0; i < Nn - Mm; i++)
            {
                x      = (_mt[i] & UpperMask) | (_mt[i + 1] & LowerMask);
                _mt[i] = _mt[i + Mm] ^ (x >> 1) ^ Mag01[x & 1UL];
            }

            for (; i < Nn - 1; i++)
            {
                x      = (_mt[i] & UpperMask) | (_mt[i + 1] & LowerMask);
                _mt[i] = _mt[i + (Mm - Nn)] ^ (x >> 1) ^ Mag01[x & 1UL];
            }

            x           = (_mt[Nn - 1] & UpperMask) | (_mt[0] & LowerMask);
            _mt[Nn - 1] = _mt[Mm - 1] ^ (x >> 1) ^ Mag01[x & 1UL];
            _mti        = 0;
        }

        x = _mt[_mti++];

        x ^= (x >> 29) & 0x5555555555555555UL;
        x ^= (x << 17) & 0x71D67FFFEDA60000UL;
        x ^= (x << 37) & 0xFFF7EEE000000000UL;
        x ^= x >> 43;

        return x;
    }

    /// <summary>
    ///     Generates a random number on [0, 2^63-1]-interval
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long Int63() =>
        (long)(Int64() >> 1);

    /// <summary>
    ///     Generates a random number on [0,1]-real-interval
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public double Real1() =>
        (Int64() >> 11) * (1.0 / 9007199254740991.0);

    /// <summary>
    ///     Generates a random number on [0,1)-real-interval
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public double Real2() =>
        (Int64() >> 11) * (1.0 / 9007199254740992.0);

    /// <summary>
    ///     Generates a random number on (0,1)-real-interval
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public double Real3() =>
        ((Int64() >> 12) + 0.5) * (1.0 / 4503599627370496.0);
}