// Copyright © 2026 Xpl0itR
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using CommunityToolkit.Diagnostics;
using Microsoft.CodeAnalysis;
using SystemEx.Memory;

namespace SystemEx.Cryptography.Stream;

partial class Rc4
{
    [NonCopyable]
    public ref struct Slim
    {
        private readonly Span<byte> _s;

        private int _i;
        private int _j;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Slim(Span<byte> key, Span<byte> s)
        {
            Debug.Assert(key.Length is > 0 and <= StateLength);
            Debug.Assert(s.Length == StateLength);

            _s = s;

            Rc4.InitState(key, _s);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte NextByte() =>
            Rc4.NextByte(_s, ref _i, ref _j);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Fill(Span<byte> keyStream) =>
            Rc4.Fill(keyStream, _s, ref _i, ref _j);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void XorBlock(ReadOnlySpan<byte> src, Span<byte> dest)
        {
            Guard.HasSizeGreaterThanOrEqualTo(src, dest.Length);

            Span<byte> keyStream = stackalloc byte[dest.Length];
            Rc4.Fill(keyStream, _s, ref _i, ref _j);

            XorHelper.XorFastUnsafe(dest, src, keyStream);
        }
    }
}