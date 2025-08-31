// Copyright © 2025 Xpl0itR
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Runtime.CompilerServices;
using System.Text.Encodings.Web;

namespace SystemEx;

public static class ValueStringBuilderExtensions
{
    extension(ValueStringBuilder builder)
    {
        public void AppendUtf8(ReadOnlySpan<byte> buffer) =>
            System.Text.Encoding.UTF8.GetChars(
                buffer, builder.AppendSpan(buffer.Length));

        public void AppendQueryStringPair(string key, string? value, bool isFirst = false) =>
            builder.AppendQueryStringPair(UrlEncoder.Default, key, value, isFirst);

        public void AppendQueryStringPair(UrlEncoder encoder, string key, string? value, bool isFirst = false) =>
            builder.AppendQueryStringPairRaw(encoder.Encode(key), value is null ? null : encoder.Encode(value), isFirst);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendQueryStringPairRaw(string key, string? value, bool isFirst = false)
        {
            builder.Append(isFirst ? '?' : '&');
            builder.Append(key);
            builder.Append('=');

            if (value is not null)
                builder.Append(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyTo(ref ValueStringBuilder destBuilder) =>
            builder.AsSpan().CopyTo(
                destBuilder.AppendSpan(builder.Length));

        public Uri ToUri(UriKind uriKind) =>
            new(builder.ToString(), uriKind);
    }
}