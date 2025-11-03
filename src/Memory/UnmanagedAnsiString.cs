// Copyright © 2025 Xpl0itR
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SystemEx.Memory;

[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public readonly struct UnmanagedAnsiString(string? str) : IDisposable
{
    public readonly nint Ptr = Marshal.StringToCoTaskMemAnsi(str);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose() => Marshal.FreeCoTaskMem(Ptr);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator nint (UnmanagedAnsiString str) => str.Ptr;
}