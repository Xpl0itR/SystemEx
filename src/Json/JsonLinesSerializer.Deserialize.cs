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
using CommunityToolkit.Diagnostics;

namespace SystemEx.Json;

public static partial class JsonLinesSerializer
{
    public static IEnumerable<T> Deserialize<T>(string jsonl, JsonTypeInfo<T> jsonTypeInfo)
    {
        using StringReader reader = new(jsonl);
        return Deserialize(reader, jsonTypeInfo);
    }

    public static IEnumerable<T> Deserialize<T>(TextReader reader, JsonTypeInfo<T> jsonTypeInfo) =>
        Deserialize(ReadLines(reader), jsonTypeInfo);

    public static IEnumerable<T> Deserialize<T>(IEnumerable<string> lines, JsonTypeInfo<T> jsonTypeInfo) =>
        lines.Select(line => JsonSerializer.Deserialize(line.AsSpan(), jsonTypeInfo) ?? ThrowHelper.ThrowInvalidDataException<T>());

    public static async IAsyncEnumerable<T> DeserializeAsync<T>(TextReader reader, JsonTypeInfo<T> jsonTypeInfo, CancellationToken ct = default)
    {
        while (await reader.ReadLineAsync(ct).ConfigureAwait(false) is { } line)
        {
            yield return JsonSerializer.Deserialize(line, jsonTypeInfo)
                         ?? ThrowHelper.ThrowInvalidDataException<T>();
        }
    }

    private static IEnumerable<string> ReadLines(TextReader reader)
    {
        while (reader.ReadLine() is { } line)
            yield return line;
    }
}