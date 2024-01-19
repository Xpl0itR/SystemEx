// Copyright © 2022 Xpl0itR
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System.Runtime.InteropServices;
using CommunityToolkit.Diagnostics;

namespace SystemEx.Net.Http.RemoteContainer;

[StructLayout(LayoutKind.Explicit, Size = FixedLength)]
internal readonly struct EndOfCentralDirectory64Record
{
    internal const int  FixedLength       = 56;
    private  const uint ExpectedSignature = 0x06064b50;

    [FieldOffset(0)]  private  readonly uint   Signature;
    [FieldOffset(4)]  private  readonly ulong  Length;
    [FieldOffset(12)] private  readonly ushort VersionMadeBy;
    [FieldOffset(14)] private  readonly ushort VersionNeededToExtract;
    [FieldOffset(16)] private  readonly uint   DiskNumber;
    [FieldOffset(20)] private  readonly uint   DiskNumberWithStartOfCentralDirectory;
    [FieldOffset(24)] private  readonly ulong  EntryCountOnThisDisk;
    [FieldOffset(32)] internal readonly ulong  EntryCount;
    [FieldOffset(40)] internal readonly ulong  CentralDirLength;
    [FieldOffset(48)] internal readonly ulong  CentralDirOffset;

    internal ulong ExtensibleDataLength =>
        Length - 44;

    internal EndOfCentralDirectory64Record ThrowIfInvalid()
    {
        if (Signature != ExpectedSignature)
        {
            ThrowHelper.ThrowInvalidDataException("ZIP64 End Of Central Directory Record signature mismatch.");
        }

        if (VersionNeededToExtract > 45)
        {
            ThrowHelper.ThrowInvalidDataException("ISO/IEC 21320: \"version needed to extract\" shall not be greater than 45.");
        }

        if (DiskNumber != 0 || DiskNumberWithStartOfCentralDirectory != 0 || EntryCountOnThisDisk != EntryCount)
        {
            ThrowHelper.ThrowInvalidDataException("ISO/IEC 21320: Archives shall not be split or spanned.");
        }

        return this;
    }
}