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
using System.Threading.Tasks;

namespace SystemEx.Json;

public static partial class JsonLinesSerializer
{
    public static string SerializeToString<T>(IEnumerable<T> values, JsonTypeInfo<T> jsonTypeInfo) =>
        string.Join(Environment.NewLine, Serialize(values, jsonTypeInfo));

    public static void Serialize<T>(TextWriter writer, IEnumerable<T> values, JsonTypeInfo<T> jsonTypeInfo)
    {
        foreach (string line in Serialize(values, jsonTypeInfo))
            writer.WriteLine(line);
    }

    public static async Task SerializeAsync<T>(TextWriter writer, IEnumerable<T> values, JsonTypeInfo<T> jsonTypeInfo)
    {
        foreach (string line in Serialize(values, jsonTypeInfo))
            await writer.WriteLineAsync(line).ConfigureAwait(false);
    }

    public static IEnumerable<string> Serialize<T>(IEnumerable<T> values, JsonTypeInfo<T> jsonTypeInfo)
    {
        foreach (T value in values)
            yield return JsonSerializer.Serialize(value, jsonTypeInfo);
    }
}