using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;

namespace SystemEx;

public static class ThrowHelpers
{
    [DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowEndOfStreamException() =>
        throw new EndOfStreamException("Unable to read beyond the end of the stream.");
}