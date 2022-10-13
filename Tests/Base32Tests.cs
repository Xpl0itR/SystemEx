// Copyright © 2022 Xpl0itR
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using SystemEx.Encoding.Base32;
using Xunit;
using static System.Text.Encoding;

namespace SystemEx.Tests;

public class Base32Tests
{
    /// <remarks><see href="https://datatracker.ietf.org/doc/html/rfc4648#section-10" /></remarks>
    public static IEnumerable<object[]> TestVectors()
    { // ReSharper disable StringLiteralTypo
        yield return new object[] { "",       ""                 };
        yield return new object[] { "f",      "MY======"         };
        yield return new object[] { "fo",     "MZXQ===="         };
        yield return new object[] { "foo",    "MZXW6==="         };
        yield return new object[] { "foob",   "MZXW6YQ="         };
        yield return new object[] { "fooba",  "MZXW6YTB"         };
        yield return new object[] { "foobar", "MZXW6YTBOI======" };
    } // ReSharper restore StringLiteralTypo

    [Theory, MemberData(nameof(TestVectors))]
    public void TestToString(string asciiString, string expectedBase32)
    {
        byte[] bytes        = ASCII.GetBytes(asciiString);
        string actualBase32 = Base32.ToString(bytes);

        Assert.Equal(expectedBase32, actualBase32);
    }

    [Theory, MemberData(nameof(TestVectors))]
    public void TestToChars(string asciiString, string expectedBase32)
    {
        byte[] bytes        = ASCII.GetBytes(asciiString);
        char[] actualBase32 = Base32.ToChars(bytes);

        Assert.Equal(expectedBase32, actualBase32);
    }

    [Theory, MemberData(nameof(TestVectors))]
    public void TestToBytes(string expectedAscii, string base32String)
    {
        byte[] bytes       = Base32.ToBytes(base32String);
        string actualAscii = ASCII.GetString(bytes);

        Assert.Equal(expectedAscii, actualAscii);
    }

    [Fact]
    public void TestInvalid() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => Base32.ToBytes("1"));
}