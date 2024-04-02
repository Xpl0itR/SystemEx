using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using SystemEx.Cryptography.Otp;
using Xunit;

namespace SystemEx.Tests;

/// <remarks>Test vectors from <see href="https://datatracker.ietf.org/doc/html/rfc6238#appendix-B" /></remarks>
public class OtpTests
{
    private const int    NumDigits        = 8;
    private const string SecretSha1Base32 = "GEZDGNBVGY3TQOJQGEZDGNBVGY3TQOJQ";

    private static readonly byte[] SecretSha1   = "12345678901234567890"u8.ToArray();
    private static readonly byte[] SecretSha256 = "12345678901234567890123456789012"u8.ToArray();
    private static readonly byte[] SecretSha512 = "1234567890123456789012345678901234567890123456789012345678901234"u8.ToArray();

    public static IEnumerable<object[]> TestVectors()
    { //             | Time (sec) | HMAC Secret |           Mode          |  TOTP  |
        yield return [59,          SecretSha1,   HashAlgorithmName.SHA1,   94287082];
        yield return [59,          SecretSha256, HashAlgorithmName.SHA256, 46119246];
        yield return [59,          SecretSha512, HashAlgorithmName.SHA512, 90693936];
        yield return [1111111109,  SecretSha1,   HashAlgorithmName.SHA1,   07081804];
        yield return [1111111109,  SecretSha256, HashAlgorithmName.SHA256, 68084774];
        yield return [1111111109,  SecretSha512, HashAlgorithmName.SHA512, 25091201];
        yield return [1111111111,  SecretSha1,   HashAlgorithmName.SHA1,   14050471];
        yield return [1111111111,  SecretSha256, HashAlgorithmName.SHA256, 67062674];
        yield return [1111111111,  SecretSha512, HashAlgorithmName.SHA512, 99943326];
        yield return [1234567890,  SecretSha1,   HashAlgorithmName.SHA1,   89005924];
        yield return [1234567890,  SecretSha256, HashAlgorithmName.SHA256, 91819424];
        yield return [1234567890,  SecretSha512, HashAlgorithmName.SHA512, 93441116];
        yield return [2000000000,  SecretSha1,   HashAlgorithmName.SHA1,   69279037];
        yield return [2000000000,  SecretSha256, HashAlgorithmName.SHA256, 90698825];
        yield return [2000000000,  SecretSha512, HashAlgorithmName.SHA512, 38618901];
        yield return [20000000000, SecretSha1,   HashAlgorithmName.SHA1,   65353130];
        yield return [20000000000, SecretSha256, HashAlgorithmName.SHA256, 77737706];
        yield return [20000000000, SecretSha512, HashAlgorithmName.SHA512, 47863826];
    }

    [Theory, MemberData(nameof(TestVectors))]
    public void ComputeCode(long unixTime, byte[] secret, HashAlgorithmName hashAlg, int expectedCode)
    {
        using TOtp tOtp       = new(secret, hashAlg) { NumDigits = NumDigits };
        int        actualCode = tOtp.ComputeCode(unixTime);

        Assert.Equal(expectedCode, actualCode);
    }

    [Fact]
    public void ComputeCodeWithDateTime()
    {
        using TOtp     tOtp       = new(SecretSha1) { NumDigits = NumDigits };
        DateTimeOffset time       = new(2603, 10, 11, 11, 33, 20, TimeSpan.Zero);
        int            actualCode = tOtp.ComputeCode(time);

        Assert.Equal(65353130, actualCode);
    }

    [Fact]
    public void ComputeCodeWithBase32Key()
    {
        using TOtp tOtp       = new(SecretSha1Base32) { NumDigits = NumDigits };
        int        actualCode = tOtp.ComputeCode(1111111111);

        Assert.Equal(14050471, actualCode);
    }
}