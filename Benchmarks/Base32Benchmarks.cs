// Copyright © 2022 Xpl0itR
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System;
using BenchmarkDotNet.Attributes;
using SystemEx.Encoding.Base32;

namespace SystemEx.Benchmarks;

[MemoryDiagnoser]
public class Base32Benchmarks
{
    // ReSharper disable once StringLiteralTypo
    private static readonly byte[] Bytes  = System.Text.Encoding.ASCII.GetBytes("fooba");
    private const           string String = "MZXW6YTB";

    [Benchmark]
    public string ToStringHeap()
    {
        string str = Base32.ToString(Bytes);
        return str;
    }

    [Benchmark]
    public char[] ToCharsHeap()
    {
        char[] chars = Base32.ToChars(Bytes);
        return chars;
    }

    [Benchmark]
    public byte[] ToBytesHeap()
    {
        byte[] bytes = Base32.ToBytes(String);
        return bytes;
    }

    [Benchmark]
    public void ToCharsStack()
    {
        Span<char> chars = stackalloc char[Base32.NumChars(Bytes)];
        Base32.ToChars(Bytes, chars);
    }

    [Benchmark]
    public void ToBytesStack()
    {
        Span<byte> bytes = stackalloc byte[Base32.NumBytes(String)];
        Base32.ToBytes(String, bytes);
    }

    [Benchmark]
    public void ToBytesWithPaddingStack()
    { // ReSharper disable StringLiteralTypo
        ReadOnlySpan<char> data = "MZXW6YTBOI======";
        data = data.TrimEnd('=');

        Span<byte> bytes = stackalloc byte[Base32.NumBytes(data)];
        Base32.ToBytes(data, bytes);
    }
}