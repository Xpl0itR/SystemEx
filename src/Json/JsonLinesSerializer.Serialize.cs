// Copyright © 2024 Xpl0itR
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using System.Threading;
using System.Threading.Tasks;

namespace SystemEx.Json;

public static partial class JsonLinesSerializer
{
    public static IEnumerable<string> Serialize<T>(IEnumerable<T> values, JsonTypeInfo<T> jsonTypeInfo) =>
        values.Select(value => JsonSerializer.Serialize(value, jsonTypeInfo));

    public static void Serialize<T>(TextWriter writer, IEnumerable<T> values, JsonTypeInfo<T> jsonTypeInfo)
    {
        foreach (string line in Serialize(values, jsonTypeInfo))
        {
            writer.WriteLine(line);
        }
    }

    public static void Serialize<T>(Utf8JsonWriter writer, IEnumerable<T> values, JsonTypeInfo<T> jsonTypeInfo)
    {
        foreach (T value in values)
        {
            JsonSerializer.Serialize(writer, value, jsonTypeInfo);
        }
    }

    public static async Task SerializeAsync<T>(Stream stream, IEnumerable<T> values, JsonTypeInfo<T> jsonTypeInfo, CancellationToken ct = default)
    {
        foreach (T value in values)
        {
            await JsonSerializer.SerializeAsync(stream, value, jsonTypeInfo, ct).ConfigureAwait(false);
            await WriteNewLine(stream, ct).ConfigureAwait(false);
        }
    }

    private static ReadOnlyMemory<byte>? _newLineUtf8;
    private static async Task WriteNewLine(Stream stream, CancellationToken ct)
    {
        if (!_newLineUtf8.HasValue)
        {
            byte[] newLineUtf8 = new byte[Environment.NewLine.Length];
            System.Text.Encoding.UTF8.GetBytes(Environment.NewLine, newLineUtf8);
            _newLineUtf8 = newLineUtf8;
        }

        await stream.WriteAsync(_newLineUtf8.Value, ct).ConfigureAwait(false);
    }
}