// Copyright © 2024-2025 Xpl0itR
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CommunityToolkit.Diagnostics;

namespace SystemEx.Cryptography.Random;

partial class MT19937_64
{
    [StructLayout(LayoutKind.Auto)]
    public ref struct Slim
    {
        private readonly Span<ulong> _mt;
        private int _mti;

        // ReSharper disable once CommentTypo
        /// <summary>
        ///     Creates an instance of the 64-bit Mersenne Twister and initializes state with a seed
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Slim(Span<ulong> mt, ulong seed = DefaultSeed)
        {
            Guard.HasSizeGreaterThanOrEqualTo(mt, Nn);
            _mt = mt;

            Init(seed);
        }

        // ReSharper disable once CommentTypo
        /// <summary>
        ///     Creates an instance of the 64-bit Mersenne Twister and initializes state with a seed and an array
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Slim(Span<ulong> mt, ReadOnlySpan<ulong> array, ulong seed = DefaultSeedArray)
        {
            Guard.HasSizeGreaterThanOrEqualTo(mt, Nn);
            _mt = mt;

            InitByArray(seed, array);
        }

        /// <summary>
        ///     Initializes state with a seed
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Init(ulong seed) =>
            MT19937_64.Init(seed, _mt, ref _mti);

        /// <summary>
        ///     Initialize state with a seed and an array
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void InitByArray(ulong seed, ReadOnlySpan<ulong> array) =>
            MT19937_64.InitByArray(seed, array, _mt, ref _mti);

        /// <summary>
        ///     Generates a random number on [0, 2^64-1] interval
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong Int64() =>
            MT19937_64.Int64(_mt, ref _mti);

        /// <summary>
        ///     Generates a random number on [0, 2^63-1]-interval
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long Int63() =>
            MT19937_64.Int63(_mt, ref _mti);

        /// <summary>
        ///     Generates a random number on [0,1]-real-interval
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double Real1() =>
            MT19937_64.Real1(_mt, ref _mti);

        /// <summary>
        ///     Generates a random number on [0,1)-real-interval
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double Real2() =>
            MT19937_64.Real2(_mt, ref _mti);

        /// <summary>
        ///     Generates a random number on (0,1)-real-interval
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double Real3() =>
            MT19937_64.Real3(_mt, ref _mti);
    }
}