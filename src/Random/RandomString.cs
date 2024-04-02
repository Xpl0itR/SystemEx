// Copyright © 2023 Xpl0itR
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace SystemEx.Random;

public static class RandomString
{
    private const string Alphanumerics = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Alphanumeric(int length) =>
        RandomNumberGenerator.GetString(Alphanumerics, length);
}