// Copyright © 2022-2024 Xpl0itR
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using SystemEx.Memory;
using CommunityToolkit.Diagnostics;

namespace SystemEx.Net.Http.RemoteContainer;

public sealed class CentralDirectoryEntry
{
    private const uint   CentralDirectoryHeaderSignature = 0x02014b50;
    private const ushort Zip64ExtraFieldTag              = 0x0001;

    private readonly ushort  _minVersionExtract;
    private readonly BitFlag _bitFlag;
    private readonly uint    _diskNumberStart;

    internal readonly ulong LocalHeaderOffset;
    
    public readonly CompressionMethod CompressionMethod;
    public readonly ushort            LastModTime;
    public readonly ushort            LastModDate;
    public readonly uint              Crc32;
    public readonly ulong             CompressedSize;
    public readonly ulong             UncompressedSize;
    public readonly string            FileName;

    private System.Text.Encoding Encoding =>
        (_bitFlag & BitFlag.Utf8Encoding) > 0
            ? System.Text.Encoding.UTF8
            : System.Text.Encoding.Default;

    internal CentralDirectoryEntry(ref MemoryReader reader)
    {
        if (reader.ReadUInt32LittleEndian() != CentralDirectoryHeaderSignature)
        {
            ThrowHelper.ThrowInvalidDataException("Central Directory header signature mismatch.");
        }

        reader.Position += 2; // version made by

        _minVersionExtract = reader.ReadUInt16LittleEndian();
        _bitFlag           = (BitFlag)reader.ReadUInt16LittleEndian();
        CompressionMethod  = (CompressionMethod)reader.ReadUInt16LittleEndian();
        LastModTime        = reader.ReadUInt16LittleEndian();
        LastModDate        = reader.ReadUInt16LittleEndian();
        Crc32              = reader.ReadUInt32LittleEndian();
        CompressedSize     = reader.ReadUInt32LittleEndian();
        UncompressedSize   = reader.ReadUInt32LittleEndian();

        ushort fileNameLength    = reader.ReadUInt16LittleEndian();
        ushort extraFieldsLength = reader.ReadUInt16LittleEndian();
        ushort fileCommentLength = reader.ReadUInt16LittleEndian();

        _diskNumberStart = reader.ReadUInt16LittleEndian();

        // internal file attributes
        // external file attributes
        reader.Position += 2 + 4;

        LocalHeaderOffset = reader.ReadUInt32LittleEndian();
        FileName = Encoding.GetString(
            reader.ReadBytes(fileNameLength));

        while (extraFieldsLength > 0)
        {
            ushort extraFieldTag    = reader.ReadUInt16LittleEndian();
            ushort extraFieldLength = reader.ReadUInt16LittleEndian();

            extraFieldsLength -= 4;

            if (extraFieldTag == Zip64ExtraFieldTag)
            {
                if (UncompressedSize == uint.MaxValue)
                {
                    UncompressedSize = reader.ReadUInt64LittleEndian();
                }

                if (CompressedSize == uint.MaxValue)
                {
                    CompressedSize = reader.ReadUInt64LittleEndian();
                }

                if (LocalHeaderOffset == uint.MaxValue)
                {
                    LocalHeaderOffset = reader.ReadUInt64LittleEndian();
                }

                if (_diskNumberStart == ushort.MaxValue)
                {
                    _diskNumberStart = reader.ReadUInt32LittleEndian();
                }
            }
            else
            {
                reader.Position += extraFieldLength;
            }

            extraFieldsLength -= extraFieldLength;
        }

        reader.Position += fileCommentLength;
    }

    internal void ThrowIfInvalid()
    {
        if (_minVersionExtract > 45)
        {
            ThrowHelper.ThrowInvalidDataException("ISO/IEC 21320: \"version needed to extract\" shall not be greater than 45.");
        }

        if ((_bitFlag & BitFlag.Unsupported) != 0)
        {
            ThrowHelper.ThrowInvalidDataException("ISO/IEC 21320: Bit 0, bits 3 to 10 and 12 to 15 of the general purpose bit flag, shall not be set.");
        }

        if (_diskNumberStart != 0)
        {
            ThrowHelper.ThrowInvalidDataException("ISO/IEC 21320: Archives shall not be split or spanned.");
        }
    }
}