// Copyright © 2021 Xpl0itR
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SystemEx.IO;

/// <summary>
///     Wraps a class derived from <see cref="Stream" /> whose length cannot be returned by
///     <see cref="Stream.Length" />. Instead, the length is passed to the constructor.
/// </summary>
/// <remarks>
///     This is intended to be used in cases such as wrapping the stream returned by
///     <see cref="System.Net.Http.HttpContent.ReadAsStream()" /> whose length cannot be returned by
///     <see cref="Stream.Length" />, but is known from
///     <see cref="System.Net.Http.Headers.HttpContentHeaders.ContentLength" />.
/// </remarks>
public sealed class LengthStream : Stream
{
    private readonly long?  _length;
    private readonly Stream _stream;

    /// <summary>
    ///     Initializes a new instance of the <see cref="LengthStream" /> class with
    ///     a class derived from <see cref="Stream" /> and its length in bytes.
    /// </summary>
    /// <param name="stream">A class derived from <see cref="Stream" /> to be wrapped.</param>
    /// <param name="length">
    ///     A <see langword="long" /> value representing the length of <paramref name="stream" /> in bytes.
    ///     If <see langword="null" />, <see cref="Length" />
    ///     will use the value of <see cref="Stream.Length" /> instead of using this value.
    /// </param>
    public LengthStream(Stream stream, long? length) =>
        (_stream, _length) = (stream, length);

    /// <summary>Gets a value indicating whether the current stream supports reading.</summary>
    /// <returns>
    ///     <see langword="true" /> if the stream supports reading; otherwise, <see langword="false" />.
    /// </returns>
    public override bool CanRead => _stream.CanRead;

    /// <summary>Gets a value indicating whether the current stream supports seeking.</summary>
    /// <returns>
    ///     <see langword="true" /> if the stream supports seeking; otherwise, <see langword="false" />.
    /// </returns>
    public override bool CanSeek => _stream.CanSeek;

    /// <inheritdoc />
    public override bool CanTimeout => _stream.CanTimeout;

    /// <summary>Gets a value indicating whether the current stream supports writing.</summary>
    /// <returns>
    ///     <see langword="true" /> if the stream supports writing; otherwise, <see langword="false" />.
    /// </returns>
    public override bool CanWrite => _stream.CanWrite;

    /// <summary>Gets the length in bytes of the stream.</summary>
    /// <exception cref="NotSupportedException">
    ///     A class derived from <see cref="Stream" /> does not support seeking.
    /// </exception>
    /// <exception cref="ObjectDisposedException">Methods were called after the stream was closed.</exception>
    /// <returns>A long value representing the length of the stream in bytes.</returns>
    public override long Length => _length ?? _stream.Length;

    /// <summary>Gets or sets the position within the current stream.</summary>
    /// <exception cref="IOException">An I/O error occurs.</exception>
    /// <exception cref="NotSupportedException">The stream does not support seeking.</exception>
    /// <exception cref="ObjectDisposedException">Methods were called after the stream was closed.</exception>
    /// <returns>The current position within the stream.</returns>
    public override long Position
    {
        get => _stream.Position;
        set => _stream.Position = value;
    }

    /// <inheritdoc />
    public override int ReadTimeout
    {
        get => _stream.ReadTimeout;
        set => _stream.ReadTimeout = value;
    }

    /// <inheritdoc />
    public override int WriteTimeout
    {
        get => _stream.WriteTimeout;
        set => _stream.WriteTimeout = value;
    }

    /// <inheritdoc />
    public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state) =>
        _stream.BeginRead(buffer, offset, count, callback, state);

    /// <inheritdoc />
    public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state) =>
        _stream.BeginWrite(buffer, offset, count, callback, state);

    /// <inheritdoc />
    public override void CopyTo(Stream destination, int bufferSize) =>
        _stream.CopyTo(destination, bufferSize);

    /// <inheritdoc />
    public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken ct) =>
        _stream.CopyToAsync(destination, bufferSize, ct);

    /// <inheritdoc />
    public override ValueTask DisposeAsync() =>
        _stream.DisposeAsync();

    /// <inheritdoc />
    public override int EndRead(IAsyncResult asyncResult) =>
        _stream.EndRead(asyncResult);

    /// <inheritdoc />
    public override void EndWrite(IAsyncResult asyncResult) =>
        _stream.EndWrite(asyncResult);

    /// <summary>Clears all buffers for this stream and causes any buffered data to be written to the underlying device.</summary>
    /// <exception cref="IOException">An I/O error occurs.</exception>
    public override void Flush() =>
        _stream.Flush();

    /// <inheritdoc />
    public override Task FlushAsync(CancellationToken ct) =>
        _stream.FlushAsync(ct);

    /// <summary>
    ///     Reads a sequence of bytes from the current stream and advances the position within the stream by the number of
    ///     bytes read.
    /// </summary>
    /// <param name="buffer">
    ///     An array of bytes. When this method returns, the buffer contains the specified byte array with the
    ///     values between <paramref name="offset" /> and (<paramref name="offset" /> + <paramref name="count" /> - 1) replaced
    ///     by the bytes read from the current source.
    /// </param>
    /// <param name="offset">
    ///     The zero-based byte offset in <paramref name="buffer" /> at which to begin storing the data read
    ///     from the current stream.
    /// </param>
    /// <param name="count">The maximum number of bytes to be read from the current stream.</param>
    /// <exception cref="ArgumentException">
    ///     The sum of <paramref name="offset" /> and <paramref name="count" /> is
    ///     larger than the buffer length.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///     <paramref name="buffer" /> is <see langword="null" />.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     <paramref name="offset" /> or <paramref name="count" /> is negative.
    /// </exception>
    /// <exception cref="IOException">An I/O error occurs.</exception>
    /// <exception cref="NotSupportedException">The stream does not support reading.</exception>
    /// <exception cref="ObjectDisposedException">Methods were called after the stream was closed.</exception>
    /// <returns>
    ///     The total number of bytes read into the buffer. This can be less than the number of bytes requested if that
    ///     many bytes are not currently available, or zero (0) if the end of the stream has been reached.
    /// </returns>
    public override int Read(byte[] buffer, int offset, int count) =>
        _stream.Read(buffer, offset, count);

    /// <inheritdoc />
    public override int Read(Span<byte> buffer) =>
        _stream.Read(buffer);

    /// <inheritdoc />
    public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken ct) =>
        _stream.ReadAsync(buffer, offset, count, ct);

    /// <inheritdoc />
    public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken ct = default) =>
        _stream.ReadAsync(buffer, ct);

    /// <inheritdoc />
    public override int ReadByte() =>
        _stream.ReadByte();

    /// <summary>Sets the position within the current stream.</summary>
    /// <param name="offset">A byte offset relative to the <paramref name="origin" /> parameter.</param>
    /// <param name="origin">
    ///     A value of type <see cref="SeekOrigin" /> indicating the reference point used to
    ///     obtain the new position.
    /// </param>
    /// <exception cref="IOException">An I/O error occurs.</exception>
    /// <exception cref="NotSupportedException">
    ///     The stream does not support seeking, such as if the stream is
    ///     constructed from a pipe or console output.
    /// </exception>
    /// <exception cref="ObjectDisposedException">Methods were called after the stream was closed.</exception>
    /// <returns>The new position within the current stream.</returns>
    public override long Seek(long offset, SeekOrigin origin) =>
        _stream.Seek(offset, origin);

    /// <summary>Sets the length of the current stream.</summary>
    /// <param name="value">The desired length of the current stream in bytes.</param>
    /// <exception cref="IOException">An I/O error occurs.</exception>
    /// <exception cref="NotSupportedException">
    ///     The stream does not support both writing and seeking, such as if the
    ///     stream is constructed from a pipe or console output.
    /// </exception>
    /// <exception cref="ObjectDisposedException">Methods were called after the stream was closed.</exception>
    public override void SetLength(long value) =>
        _stream.SetLength(value);

    /// <summary>
    ///     Writes a sequence of bytes to the current stream and advances the current position within this stream by the
    ///     number of bytes written.
    /// </summary>
    /// <param name="buffer">
    ///     An array of bytes. This method copies <paramref name="count" /> bytes from
    ///     <paramref name="buffer" /> to the current stream.
    /// </param>
    /// <param name="offset">
    ///     The zero-based byte offset in <paramref name="buffer" /> at which to begin copying bytes to the
    ///     current stream.
    /// </param>
    /// <param name="count">The number of bytes to be written to the current stream.</param>
    /// <exception cref="ArgumentException">
    ///     The sum of <paramref name="offset" /> and <paramref name="count" /> is
    ///     greater than the buffer length.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///     <paramref name="buffer" /> is <see langword="null" />.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     <paramref name="offset" /> or <paramref name="count" /> is negative.
    /// </exception>
    /// <exception cref="IOException">An I/O error occurred, such as the specified file cannot be found.</exception>
    /// <exception cref="NotSupportedException">The stream does not support writing.</exception>
    /// <exception cref="ObjectDisposedException">
    ///     <see cref="Stream.Write(byte[],int,int)" /> was called after the stream was
    ///     closed.
    /// </exception>
    public override void Write(byte[] buffer, int offset, int count) =>
        _stream.Write(buffer, offset, count);

    /// <inheritdoc />
    public override void Write(ReadOnlySpan<byte> buffer) =>
        _stream.Write(buffer);

    /// <inheritdoc />
    public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken ct) =>
        _stream.WriteAsync(buffer, offset, count, ct);

    /// <inheritdoc />
    public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken ct = default) =>
        _stream.WriteAsync(buffer, ct);

    /// <inheritdoc />
    public override void WriteByte(byte value) =>
        _stream.WriteByte(value);

    /// <summary>
    ///     Releases the unmanaged resources used by the <see cref="LengthStream" /> and optionally
    ///     releases the managed resources.
    /// </summary>
    /// <param name="disposing">
    ///     <see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to release only
    ///     unmanaged resources.
    /// </param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _stream.Dispose();
        }
    }
}