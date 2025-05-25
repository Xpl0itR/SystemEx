#if NET6_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using SystemEx.Cryptography.Block;
using Xunit;

namespace SystemEx.Tests;

/// <remarks>Test vectors from <see href="https://datatracker.ietf.org/doc/html/rfc4493#section-4" /></remarks>
public class AesCmacTests
{
    private static readonly Aes Aes;

    static AesCmacTests()
    {
        Aes     = Aes.Create();
        Aes.Key = Convert.FromHexString("2b7e151628aed2a6abf7158809cf4f3c");
    }

    public static IEnumerable<object[]> TestVectors()
    {
        yield return [string.Empty, "bb1d6929e95937287fa37d129b756746"];
        yield return ["6bc1bee22e409f96e93d7e117393172a", "070a16b46b4d4144f79bdd9dd04a287c"];
        yield return ["6bc1bee22e409f96e93d7e117393172aae2d8a571e03ac9c9eb76fac45af8e5130c81c46a35ce411", "dfa66747de9ae63030ca32611497c827"];
        yield return ["6bc1bee22e409f96e93d7e117393172aae2d8a571e03ac9c9eb76fac45af8e5130c81c46a35ce411e5fbc1191a0a52eff69f2445df4f9b17ad2b417be66c3710", "51f0bebf7e3b9d92fc49741779363cfe"];
    }

    [Theory, MemberData(nameof(TestVectors))]
    public void ComputeCmac(string messageHex, string expectedCmacHex)
    {
        byte[] message      = Convert.FromHexString(messageHex);
        byte[] expectedCmac = Convert.FromHexString(expectedCmacHex);
        byte[] actualCmac   = Aes.ComputeCmac(message);

        Assert.Equal(expectedCmac, actualCmac);
    }
}
#endif