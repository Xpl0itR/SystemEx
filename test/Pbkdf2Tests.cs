using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using SystemEx.Cryptography.KeyDerivation;
using Xunit;

namespace SystemEx.Tests;

public class Pbkdf2Tests
{
    /// <remarks><see href="https://datatracker.ietf.org/doc/html/rfc6070#section-2" /></remarks>
    public static IEnumerable<object[]> TestVectors()
    { // ReSharper disable StringLiteralTypo
        yield return ["password",                 "salt",                                 1,    20, HashAlgorithmName.SHA1, "0c60c80f961f0e71f3a9b524af6012062fe037a6"];
        yield return ["password",                 "salt",                                 2,    20, HashAlgorithmName.SHA1, "ea6c014dc72d6f8ccd1ed92ace1d41f0d8de8957"];
        yield return ["password",                 "salt",                                 4096, 20, HashAlgorithmName.SHA1, "4b007901b765489abead49d926f721d065a429c1"];
        yield return ["passwordPASSWORDpassword", "saltSALTsaltSALTsaltSALTsaltSALTsalt", 4096, 25, HashAlgorithmName.SHA1, "3d2eec4fe41c849b80c8d83662c0e44a8b291a964cf2f07038"];
        yield return ["pass\0word",               "sa\0lt",                               4096, 16, HashAlgorithmName.SHA1, "56fa6aa75548099dcc37d7f03425e0c3"];
    } // ReSharper restore StringLiteralTypo

    [Theory, MemberData(nameof(TestVectors))]
    public void Pbkdf2(string passwordAscii, string saltAscii, int iterations, int dkLen, HashAlgorithmName prf, string expectedKeyHex)
    {
        byte[] password    = System.Text.Encoding.ASCII.GetBytes(passwordAscii);
        byte[] salt        = System.Text.Encoding.ASCII.GetBytes(saltAscii);
        byte[] expectedKey = Convert.FromHexString(expectedKeyHex);
        byte[] derivedKey  = Pkcs5.Pbkdf2(prf, password, salt, iterations, dkLen);

        Assert.Equal(expectedKey, derivedKey);
    }
}