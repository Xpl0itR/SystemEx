// Copyright © 2024 Xpl0itR
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace SystemEx;

public static class StringExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int CountChar(this string str, char chr) =>
        CountChar(str.AsSpan(), chr);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int CountChar(this ReadOnlySpan<char> str, char chr)
    {
        int n, count = 0;

        while ((n = str.IndexOf(chr)) >= 0)
        {
            str = str.Slice(n + 1);
            count++;
        }
        
        return count;
    }

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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToSnakeCaseLower(this string str) =>
        string.Create(str.Length + CountUpper(str, 1), str, ToSnakeCaseLowerAction);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToSnakeCaseUpper(this string str) =>
        string.Create(str.Length + CountUpper(str, 1), str, ToSnakeCaseUpperAction);

    public static string TrimEnd(this string @string, string trimStr) =>
        @string.EndsWith(trimStr, StringComparison.Ordinal)
            ? @string[..^trimStr.Length]
            : @string;

    private static readonly SpanAction<char, string> ToSnakeCaseLowerAction =
        (newString, oldString) =>
        {
            newString[0] = char.ToLowerInvariant(oldString[0]);

            char chr;
            for (int i = 1, j = 1; i < oldString.Length; i++, j++)
            {
                chr = oldString[i];

                if (char.IsAsciiLetterUpper(chr))
                {
                    newString[j++] = '_';
                    newString[j]   = char.ToLowerInvariant(chr);
                }
                else
                {
                    newString[j] = chr;
                }
            }
        };

    private static readonly SpanAction<char, string> ToSnakeCaseUpperAction =
        (newString, oldString) =>
        {
            newString[0] = char.ToUpperInvariant(oldString[0]);

            char chr;
            for (int i = 1, j = 1; i < oldString.Length; i++, j++)
            {
                chr = oldString[i];

                if (char.IsAsciiLetterUpper(chr))
                {
                    newString[j++] = '_';
                    newString[j]   = chr;
                }
                else
                {
                    newString[j] = char.ToUpperInvariant(chr);
                }
            }
        };
}