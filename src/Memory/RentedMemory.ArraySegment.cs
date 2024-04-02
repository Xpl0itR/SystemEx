// Copyright © 2024 Xpl0itR
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Runtime.CompilerServices;

namespace SystemEx.Memory;

partial struct RentedMemory<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ArraySegment<T> AsArraySegment(int offset = 0, int? length = null) =>
        new(this.BackingArray, offset, length ?? this.Length - offset);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ArraySegment<T>(RentedMemory<T> rented) =>
        rented.AsArraySegment();
}