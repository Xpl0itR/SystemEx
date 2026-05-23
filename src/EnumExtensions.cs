// Copyright © 2026 Xpl0itR
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Runtime.CompilerServices;
using CommunityToolkit.Diagnostics;

namespace SystemEx;

public static class EnumExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasFlagFast<TEnum>(this TEnum value, TEnum flag) where TEnum : unmanaged, Enum =>
        Unsafe.SizeOf<TEnum>() switch
        {
            1 => (Unsafe.As<TEnum, byte>(ref value)   & Unsafe.As<TEnum, byte>(ref flag))   == Unsafe.As<TEnum, byte>(ref flag),
            2 => (Unsafe.As<TEnum, ushort>(ref value) & Unsafe.As<TEnum, ushort>(ref flag)) == Unsafe.As<TEnum, ushort>(ref flag),
            4 => (Unsafe.As<TEnum, uint>(ref value)   & Unsafe.As<TEnum, uint>(ref flag))   == Unsafe.As<TEnum, uint>(ref flag),
            8 => (Unsafe.As<TEnum, ulong>(ref value)  & Unsafe.As<TEnum, ulong>(ref flag))  == Unsafe.As<TEnum, ulong>(ref flag),
            _ => ThrowHelper.ThrowNotSupportedException<bool>($"Enum size {value} is not supported.")
        };
}