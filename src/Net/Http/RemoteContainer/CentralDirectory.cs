// Copyright © 2022-2024 Xpl0itR
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using SystemEx.Memory;

namespace SystemEx.Net.Http.RemoteContainer;

public sealed class CentralDirectory : IReadOnlyDictionary<string, CentralDirectoryEntry>
{
    private readonly Dictionary<string, CentralDirectoryEntry> _centralDirEntries;

    internal CentralDirectory(RentedMemory<byte> buffer, ulong entryCount)
    {
        _centralDirEntries = new Dictionary<string, CentralDirectoryEntry>(
            entryCount > int.MaxValue
                ? int.MaxValue
                : (int)entryCount,
            StringComparer.Ordinal);

        MemoryReader centralDirReader = new(ref buffer.Reference, buffer.Length);
        while (centralDirReader.Position < buffer.Length)
        {
            CentralDirectoryEntry entry = new(ref centralDirReader);
            _centralDirEntries.Add(entry.FileName, entry);
        }
    }

    /// <inheritdoc />
    public IEnumerator<KeyValuePair<string, CentralDirectoryEntry>> GetEnumerator() =>
        _centralDirEntries.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() =>
        _centralDirEntries.GetEnumerator();

    /// <inheritdoc />
    public int Count =>
        _centralDirEntries.Count;

    /// <inheritdoc />
    public bool ContainsKey(string key) =>
        _centralDirEntries.ContainsKey(key);

    /// <inheritdoc />
    public bool TryGetValue(string key, [MaybeNullWhen(false)] out CentralDirectoryEntry value) =>
        _centralDirEntries.TryGetValue(key, out value);

    /// <inheritdoc />
    public CentralDirectoryEntry this[string key] =>
        _centralDirEntries[key];

    /// <inheritdoc />
    public IEnumerable<string> Keys =>
        _centralDirEntries.Keys;

    /// <inheritdoc />
    public IEnumerable<CentralDirectoryEntry> Values =>
        _centralDirEntries.Values;
}