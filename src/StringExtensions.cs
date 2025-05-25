// Copyright © 2024-2025 Xpl0itR
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Runtime.CompilerServices;
using SystemEx.Memory;

namespace SystemEx;

public static class StringExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int CountUpper(this string str, int i = 0) =>
        CountUpper(str.AsSpan(), i);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int CountUpper(this ReadOnlySpan<char> str, int i = 0)
    {
        int count = 0;

        for (; i < str.Length; i++)
            if (char.IsAsciiLetterUpper(str[i]))
                count++;

        return count;
    }

    public static string TrimEnd(this string str, string trimStr)
    {
        ReadOnlySpan<char> trimmed = TrimEnd(str.AsSpan(), trimStr.AsSpan());
        return trimmed.Length == str.Length
            ? str
            : trimmed.ToString();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<char> TrimEnd(this ReadOnlySpan<char> str, ReadOnlySpan<char> trimStr) =>
        str.EndsWith(trimStr)
            ? str.Slice(0, str.Length - trimStr.Length)
            : str;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToSnakeCaseLower(this string str) =>
        ToSnakeCaseLower(str.AsSpan());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToSnakeCaseUpper(this string str) =>
        ToSnakeCaseUpper(str.AsSpan());

    public static string ToSnakeCaseLower(this ReadOnlySpan<char> str)
    {
        string snakeStr = new('\0', str.Length + CountUpper(str, 1));
        Span<char> dest = snakeStr.AsWriteableSpan();

        dest[0] = char.ToLowerInvariant(str[0]);

        char chr;
        for (int i = 1, j = 1; i < str.Length; i++, j++)
        {
            chr = str[i];

            if (char.IsAsciiLetterUpper(chr))
            {
                dest[j++] = '_';
                dest[j]   = char.ToLowerInvariant(chr);
            }
            else
            {
                dest[j] = chr;
            }
        }

        return snakeStr;
    }

    public static string ToSnakeCaseUpper(this ReadOnlySpan<char> str)
    {
        string snakeStr = new('\0', str.Length + CountUpper(str, 1));
        Span<char> dest = snakeStr.AsWriteableSpan();

        dest[0] = char.ToUpperInvariant(str[0]);

        char chr;
        for (int i = 1, j = 1; i < str.Length; i++, j++)
        {
            chr = str[i];

            if (char.IsAsciiLetterUpper(chr))
            {
                dest[j++] = '_';
                dest[j]   = chr;
            }
            else
            {
                dest[j] = char.ToUpperInvariant(chr);
            }
        }

        return snakeStr;
    }
}