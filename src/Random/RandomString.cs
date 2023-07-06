// Copyright © 2023 Xpl0itR
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Security.Cryptography;

namespace SystemEx.Random;

public static class RandomString
{
    private const string Alphanumerics = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

    public static string Alphanumeric(int length) =>
        string.Create(length, Alphanumerics, (token, alphanumerics) =>
        {
            Span<byte> rand = stackalloc byte[token.Length];
            RandomNumberGenerator.Fill(rand);

            for (int i = 0; i < token.Length; i++)
            {
                token[i] = alphanumerics[rand[i] % alphanumerics.Length];
            }
        });

    public static string Hexadecimal(int length, HexConverter.Casing casing = HexConverter.Casing.Lower)
    {
        Span<byte> rand = stackalloc byte[length];
        RandomNumberGenerator.Fill(rand);

        return HexConverter.ToString(rand, casing);
    }
}