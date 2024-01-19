// Copyright © 2024 Xpl0itR
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Runtime.CompilerServices;

namespace SystemEx;

public static class StringExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int CountUpper(this string str, int i = 0)
    {
        int count = 0;

        for (; i < str.Length; i++)
            if (char.IsAsciiLetterUpper(str[i]))
                count++;

        return count;
    }

    public static string ToSnakeCaseLower(this string str) =>
        string.Create(str.Length + CountUpper(str, 1), str, (newString, oldString) =>
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
        });

    public static string ToSnakeCaseUpper(this string str) =>
        string.Create(str.Length + CountUpper(str, 1), str, (newString, oldString) =>
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
        });

    public static string TrimEnd(this string @string, string trimStr) =>
        @string.EndsWith(trimStr, StringComparison.Ordinal)
            ? @string[..^trimStr.Length]
            : @string;
}