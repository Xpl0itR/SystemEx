// Copyright © 2022-2025 Xpl0itR
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using SystemEx.Memory;
using CommunityToolkit.Diagnostics;

namespace SystemEx.Net.Http.RemoteContainer;

/// <summary>
///     Parses the headers of a remote ZIP archive in order to allow for the extraction of individual files from within it,
///     without downloading the entire ZIP archive.
/// </summary>
/// <remarks>
///     This implementation is limited to the <see href="https://www.iso.org/standard/60101.html">ISO/IEC 21320-1</see>
///     subset of the <see href="https://pkware.cachefly.net/webdocs/casestudies/APPNOTE.TXT">ZIP Appnote</see>.
///     <br />In addition to this, the "ZIP file comment" (Appnote, 4.3.16) is assumed to have a length of 0 bytes.
/// </remarks>
public sealed class RemoteZipArchive
{
    public readonly CentralDirectory CentralDirectory;

    private readonly HttpMessageInvoker    _http;
    private readonly Uri?                  _uri;
    private readonly EntityTagHeaderValue? _eTag;

    private RemoteZipArchive(HttpMessageInvoker http, Uri? uri, EntityTagHeaderValue? etag, CentralDirectory centralDirectory) =>
        (_http, _uri, _eTag, CentralDirectory) = (http, uri, etag, centralDirectory);

    /// <summary>
    ///     Opens a <see cref="Stream" /> that represents the specified file in the ZIP archive.
    /// </summary>
    /// <param name="centralDirEntry">The central directory entry for the desired file.</param>
    /// <param name="ct">A cancellation token to propagate notification that operations should be canceled.</param>
    /// <returns>
    ///     A <see cref="Stream" /> that represents the specified file in the ZIP archive.
    /// </returns>
    public async Task<Stream> OpenFile(CentralDirectoryEntry centralDirEntry, CancellationToken ct)
    {
        centralDirEntry.ThrowIfInvalid();

        using RentedArray<byte> buffer = new(LocalFileHeader.FixedLength);
        await _http.ReadChunkAsync(buffer, _uri, _eTag, checked((long)centralDirEntry.LocalHeaderOffset), ct).ConfigureAwait(false);

        ulong localFileHeaderLength = buffer.UnsafeRead<LocalFileHeader>().ThrowIfInvalid().Length;
        long  fileDataOffset        = checked((long)(centralDirEntry.LocalHeaderOffset + localFileHeaderLength));

        return await _http.GetChunkAsync(_uri, _eTag, fileDataOffset, checked((long)centralDirEntry.CompressedSize), ct).ConfigureAwait(false);
    }

    /// <inheritdoc cref="Open(HttpMessageInvoker, Uri, CancellationToken)" />
    public static Task<RemoteZipArchive> Open(HttpMessageInvoker http, string uri, CancellationToken ct) =>
        Open(http, new Uri(uri, UriKind.RelativeOrAbsolute), ct);

    /// <summary>
    ///     Asynchronously initializes a new instance of the <see cref="RemoteZipArchive" /> class.
    /// </summary>
    /// <param name="http">HTTP message invoker used to send requests.</param>
    /// <param name="uri">Uniform Resource Identifier of the zip archive to be parsed.</param>
    /// <param name="ct">A cancellation token to propagate notification that operations should be canceled.</param>
    /// <exception cref="InvalidDataException"></exception>
    public static async Task<RemoteZipArchive> Open(HttpMessageInvoker http, Uri? uri, CancellationToken ct)
    {
        using HttpResponseMessage response      = await http.HeadRangeAsync(uri, ct).ConfigureAwait(false);
        long                      contentLength = response.Content.Headers.ContentLength!.Value;
        EntityTagHeaderValue?     eTag          = response.Headers.ETag;
        using RentedArray<byte>   buffer        = new(EndOfCentralDirectory64Record.FixedLength); // temp buffer size of largest struct

        Memory<byte> eocdBuffer = buffer.AsMemory(0, EndOfCentralDirectory64Locator.Length + EndOfCentralDirectoryRecord.Length);
        await http.ReadChunkFromEndAsync(eocdBuffer, uri, eTag, ct).ConfigureAwait(false);

        EndOfCentralDirectory64Locator eocd64Locator = buffer.UnsafeRead<EndOfCentralDirectory64Locator>().ThrowIfInvalid();
        ulong centralDirOffset, centralDirLength, entryCount;

        if (eocd64Locator.Signature == EndOfCentralDirectory64Locator.ExpectedSignature)
        {
            await http.ReadChunkAsync(buffer, uri, eTag, checked((long)eocd64Locator.EOCD64RecordOffset), ct).ConfigureAwait(false);
            EndOfCentralDirectory64Record eocd64Record = buffer.UnsafeRead<EndOfCentralDirectory64Record>().ThrowIfInvalid();

            centralDirOffset = eocd64Record.CentralDirOffset;
            centralDirLength = eocd64Record.CentralDirLength;
            entryCount       = eocd64Record.EntryCount;
        }
        else
        {
            EndOfCentralDirectoryRecord eocdRecord = buffer.UnsafeRead<EndOfCentralDirectoryRecord>(byteOffset: EndOfCentralDirectory64Locator.Length).ThrowIfInvalid();
            centralDirOffset = eocdRecord.CentralDirOffset;
            centralDirLength = eocdRecord.CentralDirLength;
            entryCount       = eocdRecord.EntryCount;
        }

        if (centralDirOffset + centralDirLength > (ulong)contentLength)
        {
            ThrowHelper.ThrowInvalidDataException("ISO/IEC 21320: Archives shall not be split or spanned.");
        }

        using RentedArray<byte> centralDirBuffer = new(checked((int)centralDirLength));
        await http.ReadChunkAsync(centralDirBuffer, uri, eTag, checked((long)centralDirOffset), ct).ConfigureAwait(false);

        CentralDirectory centralDirectory = new(centralDirBuffer, entryCount);
        return new RemoteZipArchive(http, uri, eTag, centralDirectory);
    }
}