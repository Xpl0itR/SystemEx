// Copyright © 2022-2024 Xpl0itR
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
    /// <summary>
    ///     Send a HEAD request to the specified URI to determine whether the server supports
    ///     <see href="https://developer.mozilla.org/docs/Web/HTTP/Range_requests">HTTP range requests</see>.
    /// </summary>
    /// <param name="httpMessageInvoker">HTTP message invoker used to send requests.</param>
    /// <param name="uri">Uniform Resource Identifier of the resource to be requested.</param>
    /// <param name="ct">A cancellation token to propagate notification that operations should be canceled.</param>
    /// <exception cref="NotSupportedException">The requested resource does not support partial requests.</exception>
    public static async Task<HttpResponseMessage> HeadRangeAsync(this HttpMessageInvoker httpMessageInvoker, Uri? uri, CancellationToken ct)
    {
        using HttpRequestMessage request  = new(HttpMethod.Head, uri);
        HttpResponseMessage      response = await SendAsync(httpMessageInvoker, request, ct).ConfigureAwait(false);

        if (!response.EnsureSuccessStatusCode()
                     .Headers.AcceptRanges.Contains("bytes"))
        {
            ThrowHelper.ThrowNotSupportedException("The requested resource does not support partial requests.");
        }

        return response;
    }

    /// <summary>
    ///     Send a GET request with the range header set to the specified starting and ending positions and return the content
    ///     represented as a <see cref="Stream" />.
    /// </summary>
    /// <param name="httpMessageInvoker">HTTP message invoker used to send requests.</param>
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
    public static async Task<Stream> GetRangeAsync(this HttpMessageInvoker httpMessageInvoker, Uri? uri, EntityTagHeaderValue? eTag, long? rangeStart, long? rangeEnd, CancellationToken ct)
    {
        using HttpRequestMessage request = new(HttpMethod.Get, uri);
        request.Headers.Range = new RangeHeaderValue(rangeStart, rangeEnd);

        if (eTag != null)
        {
            request.Headers.IfMatch.Add(eTag);
        }

        HttpResponseMessage response = await SendAsync(httpMessageInvoker, request, ct).ConfigureAwait(false);
        if (response.StatusCode != HttpStatusCode.PartialContent)
        {
            ThrowHttpRequestException(response.StatusCode,
                                      $"Response body status code was expected to be {HttpStatusCode.PartialContent} but was {response.StatusCode} instead.");
        }
        response.Dispose();
        Stream stream = await response.Content.ReadAsStreamAsync(ct).ConfigureAwait(false);
        return stream.CanSeek // If stream is not seekable the length property is not set
            ? stream
            : new LengthStream(stream, response.Content.Headers.ContentLength);
    }

    /// <summary>
    ///     Send a GET request with the range header set to the specified offset and length and return the content represented
    ///     as a <see cref="Stream" />.
    /// </summary>
    /// <param name="httpMessageInvoker">HTTP message invoker used to send requests.</param>
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
    public static Task<Stream> GetChunkAsync(this HttpMessageInvoker httpMessageInvoker, Uri? uri, EntityTagHeaderValue? eTag, long offset, long length, CancellationToken ct)
    {
        Guard.IsGreaterThan(length, 0);
        return GetRangeAsync(httpMessageInvoker, uri, eTag, offset, offset + length - 1, ct);
    }

    public static async Task ReadRangeAsync(this HttpMessageInvoker httpMessageInvoker, Memory<byte> buffer, Uri? uri, EntityTagHeaderValue? eTag, long? rangeStart, long? rangeEnd, CancellationToken ct)
    {
        Stream stream = await GetRangeAsync(httpMessageInvoker, uri, eTag, rangeStart, rangeEnd, ct).ConfigureAwait(false);
        await using (stream.ConfigureAwait(false))
            await stream.ReadExactlyAsync(buffer, ct).ConfigureAwait(false);
    }

    public static Task ReadChunkAsync(this HttpMessageInvoker httpMessageInvoker, Memory<byte> buffer, Uri? uri, EntityTagHeaderValue? eTag, long offset, CancellationToken ct)
    {
        Guard.HasSizeGreaterThan(buffer, 0);
        return ReadRangeAsync(httpMessageInvoker, buffer, uri, eTag, offset, offset + buffer.Length - 1, ct);
    }

    public static Task ReadChunkFromEndAsync(this HttpMessageInvoker httpMessageInvoker, Memory<byte> buffer, Uri? uri, EntityTagHeaderValue? eTag, CancellationToken ct) =>
        ReadRangeAsync(httpMessageInvoker, buffer, uri, eTag, rangeStart: null, buffer.Length, ct);

    private static Task<HttpResponseMessage> SendAsync(HttpMessageInvoker httpMessageInvoker, HttpRequestMessage request, CancellationToken ct) =>
        httpMessageInvoker is HttpClient httpClient
            ? httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct)
            : httpMessageInvoker.SendAsync(request, ct);

    [DoesNotReturn, MethodImpl(MethodImplOptions.NoInlining)]
    private static void ThrowHttpRequestException(HttpStatusCode statusCode, string message) =>
        throw new HttpRequestException(message, inner: null, statusCode);
}