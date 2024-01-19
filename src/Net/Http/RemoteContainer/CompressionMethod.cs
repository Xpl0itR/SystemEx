// Copyright © 2024 Xpl0itR
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace SystemEx.Net.Http.RemoteContainer;

/// <summary>
///     I only defined some common methods in this enum. Feel free to PR any you need.
///     You can see the whole list at section 4.4.5 of the
///     <see href="https://pkware.cachefly.net/webdocs/casestudies/APPNOTE.TXT">ZIP Appnote</see>
/// </summary>
public enum CompressionMethod : ushort
{
    Store     = 0,
    Deflate   = 8,
    Deflate64 = 9,
    BZIP2     = 12,
    LZMA      = 14,
    Zstandard = 93
}