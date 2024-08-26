// Copyright © 2024 Xpl0itR
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

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