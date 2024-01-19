// Copyright © 2022 Xpl0itR
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System.Runtime.InteropServices;
using CommunityToolkit.Diagnostics;

namespace SystemEx.Net.Http.RemoteContainer;

[StructLayout(LayoutKind.Explicit, Size = Length)]
internal readonly struct EndOfCentralDirectory64Locator
{
    internal const int  Length            = 20;
    internal const uint ExpectedSignature = 0x07064b50;

    [FieldOffset(0)]  internal readonly uint  Signature;
    [FieldOffset(4)]  private  readonly uint  EOCD64RecordDiskNumber;
    [FieldOffset(8)]  internal readonly ulong EOCD64RecordOffset;
    [FieldOffset(16)] private  readonly uint  TotalNumberOfDisks;

    internal EndOfCentralDirectory64Locator ThrowIfInvalid()
    {
        if (EOCD64RecordDiskNumber != 0 || TotalNumberOfDisks != 1)
        {
            ThrowHelper.ThrowInvalidDataException("ISO/IEC 21320: Archives shall not be split or spanned.");
        }

        return this;
    }
}