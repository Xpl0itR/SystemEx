// Copyright © 2024-2025 Xpl0itR
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using SystemEx.IO;

namespace SystemEx.Json;

public static partial class JsonLinesSerializer
{
    public static IEnumerable<T> Deserialize<T>(string jsonl, JsonTypeInfo<T> jsonTypeInfo)
    {
        using StringReader reader = new(jsonl);
        return Deserialize(reader, jsonTypeInfo);
    }

    public static IEnumerable<T> Deserialize<T>(TextReader reader, JsonTypeInfo<T> jsonTypeInfo) =>
        Deserialize(reader.ReadAllLines(), jsonTypeInfo);

    public static IAsyncEnumerable<T> DeserializeAsync<T>(TextReader reader, JsonTypeInfo<T> jsonTypeInfo, CancellationToken ct = default) =>
        Deserialize(reader.ReadAllLinesAsync(ct), jsonTypeInfo);

    public static IEnumerable<T> Deserialize<T>(IEnumerable<string> lines, JsonTypeInfo<T> jsonTypeInfo)
    {
        foreach (string line in lines)
            yield return JsonSerializer.Deserialize(line.AsSpan(), jsonTypeInfo)
                      ?? ThrowHelper.ThrowInvalidDataException<T>();
    }

    public static async IAsyncEnumerable<T> Deserialize<T>(IAsyncEnumerable<string> lines, JsonTypeInfo<T> jsonTypeInfo)
    {
        await foreach (string line in lines.ConfigureAwait(false))
            yield return JsonSerializer.Deserialize(line.AsSpan(), jsonTypeInfo)
                      ?? ThrowHelper.ThrowInvalidDataException<T>();
    }
}