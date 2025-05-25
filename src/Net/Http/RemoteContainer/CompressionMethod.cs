// Copyright © 2025 Xpl0itR
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace SystemEx.Net.Http.RemoteContainer;

/// <remarks><see href="https://pkware.cachefly.net/webdocs/casestudies/APPNOTE.TXT">ZIP Appnote - section 4.4.5</see></remarks>
public enum CompressionMethod : ushort
{
    /// <summary>
    ///     The file is stored (no compression)
    /// </summary>
    Stored = 0,

    /// <summary>
    ///     The file is Shrunk
    /// </summary>
    Shrunk = 1,

    /// <summary>
    ///     The file is Reduced with compression factor 1
    /// </summary>
    Reduced1 = 2,

    /// <summary>
    ///     The file is Reduced with compression factor 2
    /// </summary>
    Reduced2 = 3,

    /// <summary>
    ///     The file is Reduced with compression factor 3
    /// </summary>
    Reduced3 = 4,

    /// <summary>
    ///     The file is Reduced with compression factor 4
    /// </summary>
    Reduced4 = 5,

    /// <summary>
    ///     The file is Imploded
    /// </summary>
    Imploded = 6,

    /// <summary>
    ///     The file is Deflated
    /// </summary>
    Deflate = 8,

    /// <summary>
    ///     Enhanced Deflating using Deflate64
    /// </summary>
    Deflate64 = 9,

    /// <summary>
    ///     PKWARE Data Compression Library Imploding (old IBM TERSE)
    /// </summary>
    TERSE_Old = 10,

    /// <summary>
    ///     File is compressed using BZIP2 algorithm
    /// </summary>
    BZip2 = 12,

    /// <summary>
    ///     File is compressed using LZMA algorithm
    /// </summary>
    LZMA = 14,

    /// <summary>
    ///     IBM z/OS CMPSC Compression
    /// </summary>
    CMPSC = 16,

    /// <summary>
    ///     File is compressed using IBM TERSE (new)
    /// </summary>
    TERSE_New = 18,

    /// <summary>
    ///     IBM LZ77 z Architecture 
    /// </summary>
    LZ77 = 19,

    /// <summary>
    ///     Zstandard Compression 
    /// </summary>
    Zstd = 93,

    /// <summary>
    ///     MP3 Compression
    /// </summary>
    MP3 = 94,

    /// <summary>
    ///     XZ Compression 
    /// </summary>
    XZ = 95,

    /// <summary>
    ///     JPEG variant
    /// </summary>
    JPEG = 96,

    /// <summary>
    ///     WavPack compressed data
    /// </summary>
    WavPack = 97,

    /// <summary>
    ///     PPMd version I, Rev 1
    /// </summary>
    PPMd_V1 = 98,

    /// <summary>
    ///     AE-x encryption marker
    /// </summary>
    /// <remarks>see <see href="https://pkware.cachefly.net/webdocs/casestudies/APPNOTE.TXT">APPENDIX E</see></remarks>
    AE_X = 99
}