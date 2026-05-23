// Copyright © 2023-2025 Xpl0itR
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CommunityToolkit.HighPerformance;

namespace SystemEx.Memory;

public static class StringEx
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Allocate(int length)
    {
#if NET8_0_OR_GREATER
        return FastAllocateString(null, length);
            
        [UnsafeAccessor(UnsafeAccessorKind.StaticMethod), MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET10_0_OR_GREATER
        static extern string FastAllocateString(string? _, nint length);
#else
        static extern string FastAllocateString(string? _, int length);
#endif
#else
        return new string('\0', length);
#endif
    }

    extension(string str)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<char> AsWriteableSpan() =>
            MemoryMarshal.CreateSpan(
                ref str.DangerousGetReference(),
                str.Length);
    }
}