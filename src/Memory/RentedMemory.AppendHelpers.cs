// Copyright © 2024 Xpl0itR
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Runtime.InteropServices;
using CommunityToolkit.Diagnostics;

namespace SystemEx.Memory;

partial struct RentedMemory<T>
{
    public void Append(ArraySegment<T> src)
    {
        Guard.IsNotNull(src.Array);
        Append(src.Array, src.Offset, src.Count);
    }

    public void Append(T[] src, int offset, int length)
    {
        Guard.HasSizeGreaterThanOrEqualTo(src, offset + length);

        int start = this.Length;
        Expand(length);

        CopyHelper.CopyArrayUnchecked(src, offset, this.BackingArray, start, length);
    }

    public void Append(ref byte src, int length)
    {
        int start = this.Length;
        Expand(length);

        CopyHelper.CopyBlockUnchecked(ref src, 0, ref As<byte>(), start, length);
    }

    public void Append(ReadOnlySpan<T> src)
    {
        int start = this.Length;
        Expand(src.Length);

        CopyHelper.CopyBlockUnchecked(
            ref MemoryMarshal.GetReference(src),
            0,
            ref Reference,
            start,
            src.Length);
    }
}