using System;
using System.Collections.Generic;
using SystemEx.Cryptography.Stream;
using SystemEx.Memory;
using Xunit;

namespace SystemEx.Tests;

public class Rc4Tests
{
    /// <remarks><see href="https://datatracker.ietf.org/doc/html/rfc6229#section-2" /></remarks>
    public static IEnumerable<object[]> TestVectors()
    {
        yield return new object[] { "0102030405", 0,    "b2396305f03dc027ccc3524a0a1118a8" };
        yield return new object[] { "0102030405", 768,  "eb62638d4f0ba1fe9fca20e05bf8ff2b" };
        yield return new object[] { "0102030405", 1536, "d8729db41882259bee4f825325f5a130" };
        yield return new object[] { "0102030405", 3072, "ec0e11c479dc329dc8da7968fe965681" };

        yield return new object[] { "0102030405060708", 0,    "97ab8a1bf0afb96132f2f67258da15a8" };
        yield return new object[] { "0102030405060708", 768,  "44173a103b6616c5d5ad1cee40c863d0" };
        yield return new object[] { "0102030405060708", 1536, "8369e1a965610be887fbd0c79162aafb" };
        yield return new object[] { "0102030405060708", 3072, "bc7683205d4f443dc1f29dda3315c87b" };

        yield return new object[] { "0102030405060708090a0b0c0d0e0f10", 0,    "9ac7cc9a609d1ef7b2932899cde41b97" };
        yield return new object[] { "0102030405060708090a0b0c0d0e0f10", 768,  "eccbe13de1fcc91c11a0b26c0bc8fa4d" };
        yield return new object[] { "0102030405060708090a0b0c0d0e0f10", 1536, "ffa0b514647ec04f6306b892ae661181" };
        yield return new object[] { "0102030405060708090a0b0c0d0e0f10", 3072, "c05d88abd50357f935a63c59ee537623" };

        yield return new object[] { "0102030405060708090a0b0c0d0e0f101112131415161718191a1b1c1d1e1f20", 0,    "eaa6bd25880bf93d3f5d1e4ca2611d91" };
        yield return new object[] { "0102030405060708090a0b0c0d0e0f101112131415161718191a1b1c1d1e1f20", 768,  "e7a7b9e9ec540d5ff43bdb12792d1b35" };
        yield return new object[] { "0102030405060708090a0b0c0d0e0f101112131415161718191a1b1c1d1e1f20", 1536, "3e34135c79db010200767651cf263073" };
        yield return new object[] { "0102030405060708090a0b0c0d0e0f101112131415161718191a1b1c1d1e1f20", 3072, "625a1ab00ee39a5327346bddb01a9c18" };
    }

    [Theory, MemberData(nameof(TestVectors))]
    public void Stream(string keyHex, int offset, string expectedOutputHex)
    {
        byte[] key            = Convert.FromHexString(keyHex);
        byte[] expectedOutput = Convert.FromHexString(expectedOutputHex);
        byte[] actualOutput   = new byte[16];

        Rc4 rc4 = new(key);

        for (int i = 0; i < offset; i++)
        {
            rc4.NextByte();
        }
        for (int i = 0; i < actualOutput.Length; i++)
        {
            actualOutput[i] = rc4.NextByte();
        }

        Assert.Equal(expectedOutput, actualOutput);
    }

    [Theory, MemberData(nameof(TestVectors))]
    public void OneShot(string keyHex, int offset, string expectedOutputHex)
    {
        byte[] key            = Convert.FromHexString(keyHex);
        byte[] expectedOutput = Convert.FromHexString(expectedOutputHex);
        byte[] actualOutput   = new byte[16];
        byte[] temp           = new byte[offset + actualOutput.Length];

        Rc4.XorBlock(key, temp);
        CopyHelper.CopyArrayUnchecked(temp, offset, actualOutput, 0, actualOutput.Length);

        Assert.Equal(expectedOutput, actualOutput);
    }
}