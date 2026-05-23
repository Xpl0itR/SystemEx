// Copyright © 2026 Xpl0itR
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Buffers;
using System.IO;
using CommunityToolkit.Diagnostics;

namespace SystemEx.Memory;

public sealed unsafe class UnmanagedMemory(byte* ptr, int length)
    : UnmanagedMemory<byte>(ptr, length)
{
    public UnmanagedMemoryStream ToStream() =>
        new(this.Pointer, this.Length);
}

public unsafe class UnmanagedMemory<T> : MemoryManager<T> where T : unmanaged
{
    public readonly T* Pointer;
    public readonly int Length;

    public UnmanagedMemory(T* ptr, int length)
    {
        Guard.IsGreaterThan(length, 0);

        Pointer = ptr;
        Length = length;
    }

    public T this[int index]
    {
        get
        {
            Guard.IsLessThan(unchecked((uint)index), unchecked((uint)Length));

            return Pointer[index];
        }
    }

    public override Span<T> GetSpan() =>
        new(Pointer, Length);

    public override Memory<T> Memory =>
        this.CreateMemory(Length);

    public Memory<T> Slice(int start)
    {
        Guard.IsLessThan(unchecked((uint)start), unchecked((uint)Length));

        return this.CreateMemory(start, Length - start);
    }

    public Memory<T> Slice(int start, int length)
    {
        Guard.IsLessThan(unchecked((uint)start), unchecked((uint)Length));
        Guard.IsLessThanOrEqualTo(unchecked((uint)length), unchecked((uint)(Length - start)));

        return this.CreateMemory(start, length);
    }

    public override MemoryHandle Pin(int index = 0)
    {
        Guard.IsLessThan(unchecked((uint)index), unchecked((uint)Length));

        return new MemoryHandle(Pointer + index);
    }

    public override void Unpin() { }

    protected override void Dispose(bool _) { }
}