// Copyright © 2022 Xpl0itR
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System.Runtime.InteropServices;
using CommunityToolkit.Diagnostics;

namespace SystemEx.Net.Http.RemoteContainer;

[StructLayout(LayoutKind.Explicit, Size = FixedLength)]
internal readonly struct LocalFileHeader
{
    internal const int  FixedLength       = 30;
    private  const uint ExpectedSignature = 0x04034b50;

    [FieldOffset(0)]  private readonly uint              Signature;
    [FieldOffset(4)]  private readonly ushort            VersionNeededToExtract;
    [FieldOffset(6)]  private readonly BitFlag           BitFlag;
    [FieldOffset(8)]  private readonly CompressionMethod CompressionMethod;
    [FieldOffset(10)] private readonly ushort            LastModifiedTime;
    [FieldOffset(12)] private readonly ushort            LastModifiedDate;
    [FieldOffset(14)] private readonly uint              Crc32;
    [FieldOffset(18)] private readonly uint              CompressedSize;
    [FieldOffset(22)] private readonly uint              UncompressedSize;
    [FieldOffset(26)] private readonly ushort            FileNameLength;
    [FieldOffset(28)] private readonly ushort            ExtraFieldLength;

    internal ulong Length =>
        (ulong)FixedLength + FileNameLength + ExtraFieldLength;

    internal LocalFileHeader ThrowIfInvalid()
    {
        if (Signature != ExpectedSignature)
        {
            ThrowHelper.ThrowInvalidDataException("Local File Header signature mismatch.");
        }

        if (VersionNeededToExtract > 45)
        {
            ThrowHelper.ThrowInvalidDataException("ISO/IEC 21320: \"version needed to extract\" shall not be greater than 45.");
        }

        if ((BitFlag & BitFlag.Unsupported) != 0)
        {
            ThrowHelper.ThrowInvalidDataException("ISO/IEC 21320: Bit 0, bits 3 to 10 and 12 to 15 of the general purpose bit flag, shall not be set.");
        }

        return this;
    }
}