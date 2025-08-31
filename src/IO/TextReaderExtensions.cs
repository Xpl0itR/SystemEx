// Copyright © 2025 Xpl0itR
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;

namespace SystemEx.IO;

public static class TextReaderExtensions
{
    extension(TextReader reader)
    {
        public IEnumerable<string> ReadAllLines()
        {
            while (reader.ReadLine() is { } line)
                yield return line;
        }

        public async IAsyncEnumerable<string> ReadAllLinesAsync([EnumeratorCancellation] CancellationToken ct = default)
        {
#if NET7_0_OR_GREATER
            while (await reader.ReadLineAsync(ct).ConfigureAwait(false) is { } line)
                yield return line;
#else
            ct.ThrowIfCancellationRequested();
            while (await reader.ReadLineAsync().ConfigureAwait(false) is { } line)
            {
                yield return line;
                ct.ThrowIfCancellationRequested();
            }
#endif
        }
    }
}