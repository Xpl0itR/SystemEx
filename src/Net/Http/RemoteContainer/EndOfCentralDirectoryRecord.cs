// Copyright © 2022 Xpl0itR
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System.Runtime.InteropServices;
using CommunityToolkit.Diagnostics;

namespace SystemEx.Net.Http.RemoteContainer;

[StructLayout(LayoutKind.Explicit, Size = Length)]
internal readonly struct EndOfCentralDirectoryRecord
{
    internal const int  Length            = 22;
    private  const uint ExpectedSignature = 0x06054b50;

    [FieldOffset(0)]  private  readonly uint   Signature;
    [FieldOffset(4)]  private  readonly ushort DiskNumber;
    [FieldOffset(6)]  private  readonly ushort DiskNumberWithStartOfCentralDirectory;
    [FieldOffset(8)]  private  readonly ushort EntryCountOnThisDisk;
    [FieldOffset(10)] internal readonly ushort EntryCount;
    [FieldOffset(12)] internal readonly uint   CentralDirLength;
    [FieldOffset(16)] internal readonly uint   CentralDirOffset;
    [FieldOffset(20)] private  readonly ushort CommentLength;

    internal EndOfCentralDirectoryRecord ThrowIfInvalid()
    {
        if (CommentLength != 0)
        {
            ThrowHelper.ThrowNotSupportedException("This implementation expects the \"ZIP file comment\" to have a length of 0 bytes.");
        }

        if (Signature != ExpectedSignature)
        {
            ThrowHelper.ThrowInvalidDataException("Signature mismatch.");
        }

        if (DiskNumber != 0 || DiskNumberWithStartOfCentralDirectory != 0 || EntryCountOnThisDisk != EntryCount)
        {
            ThrowHelper.ThrowInvalidDataException("ISO/IEC 21320: Archives shall not be split or spanned.");
        }

        return this;
    }
}