// Copyright © 2024 Xpl0itR
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Buffers;
using CommunityToolkit.Diagnostics;

namespace SystemEx.Memory;

partial struct RentedMemory<T>
{
    public BufferWriter CreateBufferWriter() =>
        new(this);

    public sealed class BufferWriter(RentedMemory<T> rented) : IBufferWriter<T>
    {
        private const int MinimumBufferSize = 256;

        /// <summary>
        ///     Returns the amount of data written to the underlying buffer so far.
        /// </summary>
        public int Written { get; private set; }

        /// <inheritdoc />
        public void Advance(int count)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(count);
            Guard.IsLessThanOrEqualTo(Written + count, rented.Length);

            Written += count;
        }

        /// <inheritdoc />
        public Span<T> GetSpan(int sizeHint = 0)
        {
            EnsureCapacity(sizeHint);
            return rented.AsSpan().Slice(Written, sizeHint);
        }

        /// <inheritdoc />
        public Memory<T> GetMemory(int sizeHint = 0)
        {
            EnsureCapacity(sizeHint);
            return rented.AsMemory(Written, sizeHint);
        }

        private void EnsureCapacity(int sizeHint)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(sizeHint);
            if (sizeHint == 0) sizeHint = MinimumBufferSize;

            rented.EnsureCapacity(Written + sizeHint);
        }
    }
}