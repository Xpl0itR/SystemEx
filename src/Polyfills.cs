using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using SystemEx;
using SystemEx.Memory;

#if !NET8_0_OR_GREATER
namespace System.Security.Cryptography
{
    public static class RandomNumberGeneratorExtensions
    {
        extension(RandomNumberGenerator)
        {
            public static string GetString(ReadOnlySpan<char> choices, int length)
            {
                string str = new('\0', length);
                Span<char> destination = str.AsWriteableSpan();

                if ((choices.Length & (choices.Length - 1)) == 0 && choices.Length is > 0 and <= 256)
                {
                    Span<byte> buffer = stackalloc byte[512];

                    while (!destination.IsEmpty)
                    {
                        if (destination.Length < buffer.Length)
                        {
                            buffer = buffer.Slice(0, destination.Length);
                        }

                        RandomNumberGenerator.Fill(buffer);

                        int mask = choices.Length - 1;
                        for (int i = 0; i < buffer.Length; i++)
                        {
                            destination[i] = choices[buffer[i] & mask];
                        }

                        destination = destination.Slice(buffer.Length);
                    }

                    return str;
                }

                for (int i = 0; i < length; i++)
                {
                    destination[i] = choices[RandomNumberGenerator.GetInt32(choices.Length)];
                }

                return str;
            }
        }
    }
}
#endif

#if !NET7_0_OR_GREATER
namespace System
{
    public static class CharExtensions
    {
        extension(char)
        {
            public static bool IsAsciiLetter(char c) =>
                (uint)((c | 0x20) - 'a') <= 'z' - 'a';

            public static bool IsAsciiLetterLower(char c) =>
                IsBetween(c, 'a', 'z');

            public static bool IsAsciiLetterUpper(char c) =>
                IsBetween(c, 'A', 'Z');

            public static bool IsAsciiDigit(char c) =>
                IsBetween(c, '0', '9');

            public static bool IsAsciiLetterOrDigit(char c) =>
                IsAsciiLetter(c) | IsBetween(c, '0', '9');

            public static bool IsBetween(char c, char minInclusive, char maxInclusive) =>
                (uint)(c - minInclusive) <= (uint)(maxInclusive - minInclusive);
        }
    }

    namespace IO
    {
        public static class StreamExtensions
        {
            public static async Task ReadExactlyAsync(this Stream stream, Memory<byte> buffer, CancellationToken ct = default)
            {
                int offset = 0;
                while (offset < buffer.Length)
                {
                    int read = await stream.ReadAsync(buffer.Slice(offset), ct).ConfigureAwait(false);
                    if (read == 0)
                        ThrowHelpers.ThrowEndOfStreamException();

                    offset += read;
                }
            }
        }
    }
}
#endif

#if !NET5_0_OR_GREATER
namespace System.Runtime.CompilerServices
{
    internal class IsExternalInit { }
}

namespace System.Security.Cryptography
{
    public static class IncrementalHashExtensions
    {
        extension(IncrementalHash hash)
        {
            public int HashLengthInBytes
            {
                get
                {
                    if (hash.AlgorithmName == HashAlgorithmName.MD5)
                        return 16;
                    if (hash.AlgorithmName == HashAlgorithmName.SHA1)
                        return 20;
                    if (hash.AlgorithmName == HashAlgorithmName.SHA256)
                        return 32;
                    if (hash.AlgorithmName == HashAlgorithmName.SHA384)
                        return 48;
                    if (hash.AlgorithmName == HashAlgorithmName.SHA512)
                        return 64;

                    throw new NotSupportedException($"Cannot lookup hash length of {hash.AlgorithmName} on netstandard2.X");
                }
            }

            public int GetHashAndReset(Span<byte> destination)
            {
                if (!hash.TryGetHashAndReset(destination, out int written))
                    ThrowHelper.ThrowArgumentException(nameof(destination), "Destination is too short.");
                return written;
            }
        }
    }
}
#endif