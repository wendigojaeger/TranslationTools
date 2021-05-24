using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace WendigoJaeger.TranslationTool.Patch
{
    [DisplayName("IPS Patcher")]
    public class IpsPatcher : IPatcher
    {
        const string Header = "PATCH";
        const string EOF = "EOF";
        const int EOFInt = 0x454F46;
        const int SimilarBytesThreshold = 6;
        const int RLEThreshold = 13;

        enum CreateState
        {
            FindingDiff,
            ReadingDiff
        }

        struct RleRecord
        {
            public int StartIndex;
            public int RleSize;
            public byte RleByte;
        }

        public void Create(string sourceFilePath, string targetFilePath, string patchFile)
        {
            using var ipsWriter = new FileStream(patchFile, FileMode.Create);

            ipsWriter.Write(Encoding.ASCII.GetBytes(Header));

            byte[] sourceBuffer = File.ReadAllBytes(sourceFilePath);
            byte[] targetBuffer = File.ReadAllBytes(targetFilePath);

            CreateState state = CreateState.FindingDiff;

            int similarBytes = 0;

            int startOffset = 0;
            int blockSize = 0;

            int index = 0;

            while (index < sourceBuffer.Length && index < targetBuffer.Length)
            {
                switch (state)
                {
                    case CreateState.FindingDiff:
                        while (index < sourceBuffer.Length && index < targetBuffer.Length && sourceBuffer[index] == targetBuffer[index])
                        {
                            ++index;
                        }

                        startOffset = index;
                        blockSize = 0;
                        similarBytes = 0;
                        state = CreateState.ReadingDiff;
                        break;
                    case CreateState.ReadingDiff:
                        while (index < sourceBuffer.Length && index < targetBuffer.Length && similarBytes < SimilarBytesThreshold && blockSize <= ushort.MaxValue)
                        {
                            if (sourceBuffer[index] == targetBuffer[index])
                            {
                                ++similarBytes;
                            }
                            else
                            {
                                blockSize += similarBytes + 1;
                                similarBytes = 0;
                            }

                            ++index;
                        }

                        if (startOffset == EOFInt)
                        {
                            startOffset--;
                            blockSize++;
                        }

                        var blockSpan = targetBuffer.AsSpan()[startOffset..(startOffset + blockSize)];

                        List<RleRecord> records = new();

                        int sameByteCount = 0;
                        byte sameByte = 0;
                        int previousRleIndex = 0;
                        for (int rleIndex = 0; (rleIndex + 1) < blockSpan.Length; ++rleIndex)
                        {
                            if (blockSpan[rleIndex] == blockSpan[rleIndex + 1])
                            {
                                sameByte = blockSpan[rleIndex];
                                sameByteCount++;
                            }
                            else
                            {
                                if (sameByteCount >= RLEThreshold)
                                {
                                    RleRecord rleRecord = new();
                                    rleRecord.StartIndex = previousRleIndex;
                                    rleRecord.RleSize = sameByteCount;
                                    rleRecord.RleByte = sameByte;
                                    records.Add(rleRecord);
                                }

                                sameByteCount = 0;
                                sameByte = 0;

                                previousRleIndex = rleIndex;
                            }
                        }

                        if (records.Count > 0)
                        {
                            previousRleIndex = 0;

                            foreach (var record in records)
                            {
                                if (previousRleIndex != record.StartIndex)
                                {
                                    writeBigEndian24(ipsWriter, startOffset + previousRleIndex);
                                    writeBigEndian16(ipsWriter, (record.StartIndex - previousRleIndex) + 1);

                                    ipsWriter.Write(blockSpan[previousRleIndex..record.StartIndex]);
                                }

                                writeBigEndian24(ipsWriter, startOffset + record.StartIndex);
                                writeBigEndian16(ipsWriter, 0);
                                writeBigEndian16(ipsWriter, record.RleSize);
                                ipsWriter.WriteByte(record.RleByte);

                                previousRleIndex = record.StartIndex + record.RleSize;
                            }

                            if (previousRleIndex < blockSpan.Length)
                            {
                                writeBigEndian24(ipsWriter, startOffset + previousRleIndex);
                                writeBigEndian16(ipsWriter, (blockSpan.Length - previousRleIndex) + 1);

                                ipsWriter.Write(blockSpan[previousRleIndex..^1]);
                            }
                        }
                        else
                        {
                            writeBigEndian24(ipsWriter, startOffset);
                            writeBigEndian16(ipsWriter, blockSize);
                            ipsWriter.Write(targetBuffer.AsSpan()[startOffset..(startOffset + blockSize)]);
                        }

                        similarBytes = 0;
                        blockSize = 0;
                        index++;
                        state = CreateState.FindingDiff;
                        break;
                }
            }

            ipsWriter.Write(Encoding.ASCII.GetBytes(EOF));
        }

        private static void writeBigEndian24(Stream writer, int value)
        {
            writer.WriteByte((byte)((value >> 16) & 0xFF));
            writer.WriteByte((byte)((value >> 8) & 0xFF));
            writer.WriteByte((byte)(value & 0xFF));
        }

        private static void writeBigEndian16(Stream writer, int value)
        {
            writer.WriteByte((byte)((value >> 8) & 0xFF));
            writer.WriteByte((byte)(value & 0xFF));
        }
    }
}
