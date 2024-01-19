// Copyright © 2022 Xpl0itR
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace SystemEx.Net.Http.RemoteContainer;

[System.Flags]
internal enum BitFlag : ushort
{
    Utf8Encoding = 0b1 << 11,
    Unsupported  = 0xFFFF & ~((0b1 << 1) | (0b1 << 2) | Utf8Encoding)
}