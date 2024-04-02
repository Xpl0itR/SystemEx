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
    public void Expand(int count)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(count);
        this.EnsureCapacity(this.Length + count);
    }

    public void Append(ArraySegment<T> src)
    {
        Guard.IsNotNull(src.Array);
        Append(src.Array, src.Offset, src.Count);
    }

    public void Append(T[] src) =>
        Append(src, 0, src.Length);

    public void Append(T[] src, int offset, int count)
    {
        Guard.HasSizeGreaterThanOrEqualTo(src, offset + count);

        int start = this.Length;
        Expand(count);

        CopyHelper.CopyArrayUnchecked(src, offset, this.BackingArray, start, count);
    }

    public void Append(Span<T> src) =>
        Append(ref MemoryMarshal.GetReference(src), src.Length);

    public void Append(ReadOnlySpan<T> src) =>
        Append(ref MemoryMarshal.GetReference(src), src.Length);

    public void Append(ref T src, int length)
    {
        int start = this.Length;
        Expand(length);

        CopyHelper.CopyBlockUnchecked(ref src, 0, ref Reference, start, length);
    }
}