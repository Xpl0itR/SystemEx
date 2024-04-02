// Copyright © 2024 Xpl0itR
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CommunityToolkit.Diagnostics;

namespace SystemEx.Random;

partial class MT19937_64
{
    [StructLayout(LayoutKind.Auto)]
    public ref struct OnStack
    {
        private readonly Span<ulong> _mt;
        private int _mti;

        // ReSharper disable once CommentTypo
        /// <summary>
        ///     Creates an instance of the 64-bit Mersenne Twister on the stack and initializes state with a seed
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public OnStack(Span<ulong> mt, ulong seed = DefaultSeed)
        {
            Guard.HasSizeEqualTo(mt, Nn);
            _mt = mt;

            Init(seed);
        }

        // ReSharper disable once CommentTypo
        /// <summary>
        ///     Creates an instance of the 64-bit Mersenne Twister on the stack and initializes state with a seed and an array
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public OnStack(Span<ulong> mt, ReadOnlySpan<ulong> array, ulong seed = DefaultSeedArray)
        {
            Guard.HasSizeEqualTo(mt, Nn);
            _mt = mt;

            InitByArray(seed, array);
        }

        /// <inheritdoc cref="MT19937_64.Init(ulong, Span{ulong}, ref int)" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Init(ulong seed) =>
            MT19937_64.Init(seed, _mt, ref _mti);

        /// <inheritdoc cref="MT19937_64.InitByArray(ulong, ReadOnlySpan{ulong}, Span{ulong}, ref int)" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void InitByArray(ulong seed, ReadOnlySpan<ulong> array) =>
            MT19937_64.InitByArray(seed, array, _mt, ref _mti);

        /// <inheritdoc cref="MT19937_64.Int64(Span{ulong}, ref int)" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong Int64() =>
            MT19937_64.Int64(_mt, ref _mti);

        /// <inheritdoc cref="MT19937_64.Int63(Span{ulong}, ref int)" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long Int63() =>
            MT19937_64.Int63(_mt, ref _mti);

        /// <inheritdoc cref="MT19937_64.Real1(Span{ulong}, ref int)" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double Real1() =>
            MT19937_64.Real1(_mt, ref _mti);

        /// <inheritdoc cref="MT19937_64.Real2(Span{ulong}, ref int)" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double Real2() =>
            MT19937_64.Real2(_mt, ref _mti);

        /// <inheritdoc cref="MT19937_64.Real3(Span{ulong}, ref int)" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double Real3() =>
            MT19937_64.Real3(_mt, ref _mti);
    }
}