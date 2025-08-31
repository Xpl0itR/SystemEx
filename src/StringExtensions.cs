// Copyright © 2024-2025 Xpl0itR
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using CommunityToolkit.Diagnostics;
using SystemEx.Memory;

namespace SystemEx;

public static class StringExtensions
{
    extension(string str)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CountUpper(int i = 0) =>
            CountUpper(str.AsSpan(), i);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ToSnakeCaseLower() =>
            ToSnakeCaseLower(str.AsSpan());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ToSnakeCaseUpper() =>
            ToSnakeCaseUpper(str.AsSpan());

        public string TrimEnd(string trimStr)
        {
            ReadOnlySpan<char> trimmed = TrimEnd(str.AsSpan(), trimStr.AsSpan());
            return trimmed.Length == str.Length
                ? str
                : trimmed.ToString();
        }
    }

    extension(ReadOnlySpan<char> str)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CountUpper(int i = 0)
        {
            int count = 0;

            for (; i < str.Length; i++)
                if (char.IsAsciiLetterUpper(str[i]))
                    count++;

            return count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<char> TrimEnd(ReadOnlySpan<char> trimStr) =>
            str.EndsWith(trimStr)
                ? str.Slice(0, str.Length - trimStr.Length)
                : str;

        public string ToSnakeCaseLower()
        {
            string     snakeStr = new('\0', str.Length + CountUpper(str, 1));
            Span<char> dest     = snakeStr.AsWriteableSpan();

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

        public string ToSnakeCaseUpper()
        {
            string     snakeStr = new('\0', str.Length + CountUpper(str, 1));
            Span<char> dest     = snakeStr.AsWriteableSpan();

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

    extension(Convert)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FromHexString(ReadOnlySpan<char> chars, Span<byte> bytes)
        {
            if (chars.Length == 0)
                return;

            Guard.HasSizeGreaterThanOrEqualTo(bytes, chars.Length >> 1);
            
            if ((uint)chars.Length % 2 != 0)
                ThrowHelper.ThrowFormatException("The input is not a valid hex string as its length is not a multiple of 2.");

            if (!HexConverter.TryDecodeFromUtf16_Scalar(chars, bytes, out _))
                ThrowHelper.ThrowFormatException("The input is not a valid hex string as it contains a non-hex character.");
        }
    }

    // Licensed to the .NET Foundation under one or more agreements.
    // The .NET Foundation licenses this file to you under the MIT license.
    // source: https://github.com/dotnet/runtime/blob/9a22df00772b15c3bc1ad37bf01266064295ef10/src/libraries/Common/src/System/HexConverter.cs
    private static class HexConverter
    {
        internal static bool TryDecodeFromUtf16_Scalar(ReadOnlySpan<char> source, Span<byte> destination, out int charsProcessed)
        {
            Debug.Assert((source.Length % 2) == 0, "Un-even number of characters provided");
            Debug.Assert((source.Length / 2) == destination.Length, "Target buffer not right-sized for provided characters");

            int i = 0;
            int j = 0;
            int byteLo = 0;
            int byteHi = 0;

            while (j < destination.Length)
            {
                byteLo = FromChar(source[i + 1]);
                byteHi = FromChar(source[i]);

                // byteHi hasn't been shifted to the high half yet, so the only way the bitwise or produces this pattern
                // is if either byteHi or byteLo was not a hex character.
                if ((byteLo | byteHi) == 0xFF)
                {
                    break;
                }

                destination[j++] = (byte)((byteHi << 4) | byteLo);
                i += 2;
            }

            if (byteLo == 0xFF)
            {
                i++;
            }

            charsProcessed = i;
            return (byteLo | byteHi) != 0xFF;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int FromChar(int c)
        {
            return (c >= CharToHexLookup.Length) ? 0xFF : CharToHexLookup[c];
        }

        /// <summary>Map from an ASCII char to its hex value, e.g. arr['b'] == 11. 0xFF means it's not a hex digit.</summary>
        private static ReadOnlySpan<byte> CharToHexLookup =>
        [
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // 15
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // 31
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // 47
            0x0,  0x1,  0x2,  0x3,  0x4,  0x5,  0x6,  0x7,  0x8,  0x9,  0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // 63
            0xFF, 0xA,  0xB,  0xC,  0xD,  0xE,  0xF,  0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // 79
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // 95
            0xFF, 0xa,  0xb,  0xc,  0xd,  0xe,  0xf,  0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // 111
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // 127
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // 143
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // 159
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // 175
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // 191
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // 207
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // 223
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // 239
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF  // 255
        ];
    }
}