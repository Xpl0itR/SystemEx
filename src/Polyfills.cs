using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using SystemEx;
using SystemEx.Memory;

#if !NET9_0_OR_GREATER
namespace System
{
    public static class ConvertExtensions
    {
        extension(Convert)
        {
            public static string ToHexStringLower(byte[] inArray)
            {
                Guard.IsNotNull(inArray);

                return ToHexStringLower(
                    new ReadOnlySpan<byte>(inArray));
            }

            public static string ToHexStringLower(byte[] inArray, int offset, int length)
            {
                Guard.IsNotNull(inArray);
                Guard.IsGreaterThanOrEqualTo(length, 0);
                Guard.IsGreaterThanOrEqualTo(offset, 0);
                Guard.IsLessThanOrEqualTo(offset, inArray.Length - length);

                return ToHexStringLower(
                    new ReadOnlySpan<byte>(inArray, offset, length));
            }

            public static string ToHexStringLower(ReadOnlySpan<byte> bytes)
            {
                if (bytes.Length == 0)
                    return string.Empty;

                Guard.IsLessThanOrEqualTo(bytes.Length, int.MaxValue / 2, nameof(bytes));

                return HexConverter.ToString(bytes, HexConverter.Casing.Lower);
            }

            public static bool TryToHexStringLower(ReadOnlySpan<byte> source, Span<char> destination, out int charsWritten)
            {
                if (source.Length == 0)
                {
                    charsWritten = 0;
                    return true;
                }

                if (source.Length > int.MaxValue / 2 || destination.Length < source.Length * 2)
                {
                    charsWritten = 0;
                    return false;
                }

                HexConverter.EncodeToUtf16(source, destination, HexConverter.Casing.Lower);
                charsWritten = source.Length * 2;

                return true;
            }
        }

        private static class HexConverter
        {
            public enum Casing : uint
            {
                // Output [ '0' .. '9' ] and [ 'A' .. 'F' ].
                Upper = 0,

                // Output [ '0' .. '9' ] and [ 'a' .. 'f' ].
                // This works because values in the range [ 0x30 .. 0x39 ] ([ '0' .. '9' ])
                // already have the 0x20 bit set, so ORing them with 0x20 is a no-op,
                // while outputs in the range [ 0x41 .. 0x46 ] ([ 'A' .. 'F' ])
                // don't have the 0x20 bit set, so ORing them maps to
                // [ 0x61 .. 0x66 ] ([ 'a' .. 'f' ]), which is what we want.
                Lower = 0x2020U,
            }

            public static string ToString(ReadOnlySpan<byte> bytes, Casing casing = Casing.Upper)
            {
                // .NET 8.0 path (doesn't support 'allow ref struct' feature)
#pragma warning disable CS8500
                unsafe
                {
                    return string.Create(bytes.Length * 2, (RosPtr: (IntPtr)(&bytes), casing), static (chars, args) =>
                        EncodeToUtf16(*(ReadOnlySpan<byte>*)args.RosPtr, chars, args.casing));
                }
#pragma warning restore CS8500
            }

            public static void EncodeToUtf16(ReadOnlySpan<byte> source, Span<char> destination, Casing casing = Casing.Upper)
            {
                Debug.Assert(destination.Length >= (source.Length * 2));

                for (int pos = 0; pos < source.Length; pos++)
                {
                    ToCharsBuffer(source[pos], destination, pos * 2, casing);
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void ToCharsBuffer(byte value, Span<char> buffer, int startingIndex = 0, Casing casing = Casing.Upper)
            {
                uint difference   = (((uint)value & 0xF0U) << 4) + ((uint)value & 0x0FU) - 0x8989U;
                uint packedResult = ((((uint)(-(int)difference) & 0x7070U) >> 4) + difference + 0xB9B9U) | (uint)casing;

                buffer[startingIndex + 1] = (char)(packedResult & 0xFF);
                buffer[startingIndex]     = (char)(packedResult >> 8);
            }
        }
    }
}
#endif

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
            [MethodImpl(MethodImplOptions.AggressiveInlining)]

            public static bool IsAsciiLetterOrDigit(char c) =>
                IsAsciiLetter(c) | IsAsciiDigit(c);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static bool IsAsciiLetter(char c) =>
                IsAsciiLetterLower(unchecked((char)(c | 0x20)));

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static bool IsAsciiLetterLower(char c) =>
                IsBetween(c, 'a', 'z');

            [MethodImpl(MethodImplOptions.AggressiveInlining)]

            public static bool IsAsciiLetterUpper(char c) =>
                IsBetween(c, 'A', 'Z');

            [MethodImpl(MethodImplOptions.AggressiveInlining)]

            public static bool IsAsciiDigit(char c) =>
                IsBetween(c, '0', '9');

            [MethodImpl(MethodImplOptions.AggressiveInlining)]

            public static bool IsBetween(char c, char minInclusive, char maxInclusive) =>
                (uint)(c - minInclusive) <= (uint)(maxInclusive - minInclusive);
        }
    }

    namespace IO
    {
        public static class StreamExtensions
        {
            extension(Stream stream)
            {
                public async Task ReadExactlyAsync(Memory<byte> buffer, CancellationToken ct = default)
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
                    return ThrowHelper.ThrowNotSupportedException<int>(
                        $"Cannot lookup hash length of {hash.AlgorithmName} on netstandard2.X");
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