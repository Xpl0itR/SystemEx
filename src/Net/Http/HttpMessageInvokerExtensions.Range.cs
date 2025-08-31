// Copyright © 2022-2025 Xpl0itR
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using SystemEx.IO;
using CommunityToolkit.Diagnostics;

namespace SystemEx.Net.Http;

/// <summary>
///     A set of methods to extend the <see cref="HttpMessageInvoker" /> class.
/// </summary>
public static partial class HttpMessageInvokerExtensions
{
    extension(HttpMessageInvoker invoker)
    {
        /// <summary>
        ///     Send a HEAD request to the specified URI to determine whether the server supports
        ///     <see href="https://developer.mozilla.org/docs/Web/HTTP/Range_requests">HTTP range requests</see>.
        /// </summary>
        /// <param name="uri">Uniform Resource Identifier of the resource to be requested.</param>
        /// <param name="ct">A cancellation token to propagate notification that operations should be canceled.</param>
        /// <exception cref="NotSupportedException">The requested resource does not support partial requests.</exception>
        public async Task<HttpResponseMessage> HeadRangeAsync(Uri? uri, CancellationToken ct)
        {
            using HttpRequestMessage request  = new(HttpMethod.Head, uri);
            HttpResponseMessage      response = await SendAsync(invoker, request, ct).ConfigureAwait(false);

            if (!response.EnsureSuccessStatusCode().Headers.AcceptRanges.Contains("bytes"))
            {
                ThrowHelper.ThrowNotSupportedException("The requested resource does not support partial requests.");
            }

            return response;
        }

        /// <summary>
        ///     Send a GET request with the range header set to the specified starting and ending positions and return the content
        ///     represented as a <see cref="Stream" />.
        /// </summary>
        /// <param name="uri">Uniform Resource Identifier of the resource to be requested.</param>
        /// <param name="eTag">
        ///     An <see href="https://developer.mozilla.org/docs/Web/HTTP/Headers/ETag">Entity Tag</see> to be sent
        ///     in the <see href="https://developer.mozilla.org/docs/Web/HTTP/Headers/If-Match">If-Match</see> header of the
        ///     request.
        /// </param>
        /// <param name="rangeStart">
        ///     A zero-based byte offset indicating the beginning of the requested range.
        ///     This value is optional and, if omitted, the value of <paramref name="rangeEnd" />
        ///     will be taken to indicate the number of bytes from the end of the file to return.
        /// </param>
        /// <param name="rangeEnd">
        ///     A zero-based byte offset indicating the end of the requested range.
        ///     This value is optional and, if omitted, the end of the document is taken as the end of the range.
        /// </param>
        /// <param name="ct">A cancellation token to propagate notification that operations should be canceled.</param>
        /// <returns>A <see cref="Stream" /> that represents the requested range of the resource.</returns>
        /// <exception cref="HttpRequestException"></exception>
        public async Task<Stream> GetRangeAsync(Uri? uri, EntityTagHeaderValue? eTag, long? rangeStart, long? rangeEnd, CancellationToken ct)
        {
            using HttpRequestMessage request = new(HttpMethod.Get, uri);
            request.Headers.Range = new RangeHeaderValue(rangeStart, rangeEnd);

            if (eTag != null)
            {
                request.Headers.IfMatch.Add(eTag);
            }

            HttpResponseMessage response = await SendAsync(invoker, request, ct).ConfigureAwait(false);
            if (response.StatusCode != HttpStatusCode.PartialContent)
            {
                ThrowHttpRequestException(response.StatusCode, $"Response body status code was expected to be {HttpStatusCode.PartialContent} but was {response.StatusCode} instead.");
            }

            Stream stream = await ReadAsStreamAsync(response.Content, ct).ConfigureAwait(false);
            return stream.CanSeek // If stream is not seekable the length property is not set
                ? stream
                : new LengthStream(stream, response.Content.Headers.ContentLength);
        }

        /// <summary>
        ///     Send a GET request with the range header set to the specified offset and length and return the content represented
        ///     as a <see cref="Stream" />.
        /// </summary>
        /// <param name="uri">Uniform Resource Identifier of the resource to be requested.</param>
        /// <param name="eTag">
        ///     An <see href="https://developer.mozilla.org/docs/Web/HTTP/Headers/ETag">Entity Tag</see> to be sent
        ///     in the <see href="https://developer.mozilla.org/docs/Web/HTTP/Headers/If-Match">If-Match</see> header of the
        ///     request.
        /// </param>
        /// <param name="offset">A zero-based byte offset indicating the beginning of the requested range.</param>
        /// <param name="length">The number of bytes after the offset to request from the resource.</param>
        /// <param name="ct">A cancellation token to propagate notification that operations should be canceled.</param>
        /// <returns>A <see cref="Stream" /> that represents the requested chunk of the resource.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public Task<Stream> GetChunkAsync(Uri? uri, EntityTagHeaderValue? eTag, long offset, long length, CancellationToken ct)
        {
            Guard.IsGreaterThan(length, 0);
            return GetRangeAsync(invoker, uri, eTag, offset, offset + length - 1, ct);
        }

        public async Task ReadRangeAsync(Memory<byte> buffer, Uri? uri, EntityTagHeaderValue? eTag, long? rangeStart, long? rangeEnd, CancellationToken ct)
        {
            Stream stream = await GetRangeAsync(invoker, uri, eTag, rangeStart, rangeEnd, ct).ConfigureAwait(false);
            await using (stream.ConfigureAwait(false))
                await stream.ReadExactlyAsync(buffer, ct).ConfigureAwait(false);
        }

        public Task ReadChunkAsync(Memory<byte> buffer, Uri? uri, EntityTagHeaderValue? eTag, long offset, CancellationToken ct)
        {
            Guard.HasSizeGreaterThan(buffer, 0);
            return ReadRangeAsync(invoker, buffer, uri, eTag, offset, offset + buffer.Length - 1, ct);
        }

        public Task ReadChunkFromEndAsync(Memory<byte> buffer, Uri? uri, EntityTagHeaderValue? eTag, CancellationToken ct) =>
            ReadRangeAsync(invoker, buffer, uri, eTag, rangeStart: null, buffer.Length, ct);
    }

    private static Task<HttpResponseMessage> SendAsync(HttpMessageInvoker invoker, HttpRequestMessage request, CancellationToken ct) =>
        invoker is HttpClient httpClient
            ? httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct)
            : invoker.SendAsync(request, ct);

    private static Task<Stream> ReadAsStreamAsync(HttpContent content, CancellationToken ct) =>
        content.ReadAsStreamAsync(
#if NET5_0_OR_GREATER
            ct
#endif
        );

    [DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
    private static void ThrowHttpRequestException(HttpStatusCode statusCode, string message) =>
#if NET5_0_OR_GREATER
        throw new HttpRequestException(message, inner: null, statusCode);
#else
        throw new HttpRequestException(message);
#endif
}