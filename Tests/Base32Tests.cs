using System;
using System.Collections.Generic;
using SystemEx.Encoding;
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
    public void GetStringFromBytes(string asciiString, string expectedBase32)
    {
        byte[] bytes        = ASCII.GetBytes(asciiString);
        string actualBase32 = Base32.GetString(bytes);

        Assert.Equal(expectedBase32, actualBase32);
    }

    [Theory, MemberData(nameof(TestVectors))]
    public void GetStringFromSpan(string asciiString, string expectedBase32)
    {
        ReadOnlySpan<byte> bytes = ASCII.GetBytes(asciiString);
        string actualBase32 = Base32.GetString(bytes);

        Assert.Equal(expectedBase32, actualBase32);
    }

    [Theory, MemberData(nameof(TestVectors))]
    public void GetChars(string asciiString, string expectedBase32)
    {
        byte[] bytes        = ASCII.GetBytes(asciiString);
        char[] actualBase32 = Base32.GetChars(bytes);

        Assert.Equal(expectedBase32, actualBase32);
    }

    [Theory, MemberData(nameof(TestVectors))]
    public void GetBytes(string expectedAscii, string base32String)
    {
        byte[] bytes       = Base32.GetBytes(base32String);
        string actualAscii = ASCII.GetString(bytes);

        Assert.Equal(expectedAscii, actualAscii);
    }

    [Fact]
    public void Invalid() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => Base32.GetBytes("1"));
}