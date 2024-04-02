// Copyright © 2023-2024 Xpl0itR
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Runtime.CompilerServices;

namespace SystemEx.Random;

// ReSharper disable once InconsistentNaming, CommentTypo
/// <summary>
///     64-bit version of the Mersenne Twister pseudorandom number generator.
/// </summary>
/// <remarks><see href="http://www.math.sci.hiroshima-u.ac.jp/m-mat/MT/VERSIONS/C-LANG/mt19937-64.c"/></remarks>
public sealed partial class MT19937_64
{
    public const ulong DefaultSeed      = 5489UL;
    public const ulong DefaultSeedArray = 19650218UL;
    public const int   Nn               = 312;

    private const int   Mm        = 156;
    private const ulong MatrixA   = 0xB5026F5AA96619E9UL;
    private const ulong UpperMask = 0xFFFFFFFF80000000UL; // Most significant 33 bits
    private const ulong LowerMask = 0X7FFFFFFFUL;         // Least significant 31 bits

    private static readonly ulong[] Mag01 = [0UL, MatrixA];

    /// <summary>
    ///     Initializes state with a seed
    /// </summary>
    private static void Init(ulong seed, Span<ulong> mt, ref int mti)
    {
        mt[0] = seed;

        for (mti = 1; mti < Nn; mti++)
        {
            mt[mti] = 6364136223846793005UL * (mt[mti - 1] ^ (mt[mti - 1] >> 62)) + (ulong)mti;
        }
    }

    /// <summary>
    ///     Initialize state with a seed and an array
    /// </summary>
    private static void InitByArray(ulong seed, ReadOnlySpan<ulong> array, Span<ulong> mt, ref int mti)
    {
        Init(seed, mt, ref mti);

        int i = 1;
        for (int j = 0, k = Math.Max(Nn, array.Length); k > 0; k--)
        {
            mt[i] = (mt[i] ^ ((mt[i - 1] ^ (mt[i - 1] >> 62)) * 3935559000370003845UL)) + array[j] + (ulong)j;
            i++;
            j++;

            if (i >= Nn)
            {
                mt[0] = mt[Nn - 1]; i = 1;
            }

            if (j >= array.Length)
            {
                j = 0;
            }
        }

        for (int k = Nn - 1; k > 0; k--)
        {
            mt[i] = (mt[i] ^ ((mt[i - 1] ^ (mt[i - 1] >> 62)) * 2862933555777941757UL)) - (ulong)i;
            i++;

            if (i >= Nn)
            {
                mt[0] = mt[Nn - 1]; i = 1;
            }
        }

        mt[0] = 1UL << 63;
    }

    /// <summary>
    ///     Generates a random number on [0, 2^64-1] interval
    /// </summary>
    private static ulong Int64(Span<ulong> mt, ref int mti)
    {
        ulong x;
        int   i;

        if (mti >= Nn)
        {
            for (i = 0; i < Nn - Mm; i++)
            {
                x     = (mt[i] & UpperMask) | (mt[i + 1] & LowerMask);
                mt[i] = mt[i + Mm] ^ (x >> 1) ^ Mag01[x & 1UL];
            }

            for (; i < Nn - 1; i++)
            {
                x     = (mt[i] & UpperMask) | (mt[i + 1] & LowerMask);
                mt[i] = mt[i + (Mm - Nn)] ^ (x >> 1) ^ Mag01[x & 1UL];
            }

            x          = (mt[Nn - 1] & UpperMask) | (mt[0] & LowerMask);
            mt[Nn - 1] = mt[Mm - 1] ^ (x >> 1) ^ Mag01[x & 1UL];
            mti        = 0;
        }

        x = mt[mti++];

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
    private static long Int63(Span<ulong> mt, ref int mti) =>
        (long)(Int64(mt, ref mti) >> 1);

    /// <summary>
    ///     Generates a random number on [0,1]-real-interval
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static double Real1(Span<ulong> mt, ref int mti) =>
        (Int64(mt, ref mti) >> 11) * (1.0 / 9007199254740991.0);

    /// <summary>
    ///     Generates a random number on [0,1)-real-interval
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static double Real2(Span<ulong> mt, ref int mti) =>
        (Int64(mt, ref mti) >> 11) * (1.0 / 9007199254740992.0);

    /// <summary>
    ///     Generates a random number on (0,1)-real-interval
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static double Real3(Span<ulong> mt, ref int mti) =>
        ((Int64(mt, ref mti) >> 12) + 0.5) * (1.0 / 4503599627370496.0);
}