// Copyright © 2024 Xpl0itR
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using CommunityToolkit.Diagnostics;

namespace SystemEx.Random;

partial class MT19937_64
{
    private readonly ulong[] _mt;
    private int _mti;

    // ReSharper disable once CommentTypo
    /// <summary>
    ///     Creates an instance of the 64-bit Mersenne Twister on the heap and initializes state with a seed
    /// </summary>
    public MT19937_64(ulong seed = DefaultSeed, ulong[]? mt = null)
    {
        if (mt is null)
        {
            _mt = GC.AllocateUninitializedArray<ulong>(Nn);
        }
        else
        {
            Guard.HasSizeEqualTo(mt, Nn);
            _mt = mt;
        }

        Init(seed, _mt, ref _mti);
    }

    // ReSharper disable once CommentTypo
    /// <summary>
    ///     Creates an instance of the 64-bit Mersenne Twister on the heap and initializes state with a seed and an array
    /// </summary>
    public MT19937_64(ReadOnlySpan<ulong> array, ulong seed = DefaultSeedArray, ulong[]? mt = null)
    {
        if (mt is null)
        {
            _mt = GC.AllocateUninitializedArray<ulong>(Nn);
        }
        else
        {
            Guard.HasSizeEqualTo(mt, Nn);
            _mt = mt;
        }

        InitByArray(seed, array, _mt, ref _mti);
    }

    /// <inheritdoc cref="Int64(Span{ulong}, ref int)" />
    public ulong Int64() =>
        Int64(_mt, ref _mti);

    /// <inheritdoc cref="Int63(Span{ulong}, ref int)" />
    public long Int63() =>
        Int63(_mt, ref _mti);

    /// <inheritdoc cref="Real1(Span{ulong}, ref int)" />
    public double Real1() =>
        Real1(_mt, ref _mti);

    /// <inheritdoc cref="Real2(Span{ulong}, ref int)" />
    public double Real2() =>
        Real2(_mt, ref _mti);

    /// <inheritdoc cref="Real3(Span{ulong}, ref int)" />
    public double Real3() =>
        Real3(_mt, ref _mti);
}