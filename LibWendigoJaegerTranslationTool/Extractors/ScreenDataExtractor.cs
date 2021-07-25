using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using WendigoJaeger.TranslationTool.Data;

namespace WendigoJaeger.TranslationTool.Extractors
{
    class ScreenDataExtractor : IScriptExtractor
    {
        class RawExtractedData
        {
            public List<byte> RawData { get; } = new List<byte>();
            public long StartAddress { get; set; }
            public List<long> Pointers { get; } = new List<long>();
            public ScriptEntry DataEntry { get; set; }

            public bool Inside(long address)
            {
                return address >= StartAddress && address <= (StartAddress + RawData.Count);
            }

            public override string ToString()
            {
                return $"Entry 0x{StartAddress:x}";
            }
        }

        struct PointerToExtractedData
        {
            public long Pointer;
            public RawExtractedData ExtractedData;
        }

        [JsonIgnore]
        public string Name => "Screen Data Extractor";

        public void Extract(Project project, ScriptSettings settings)
        {
            settings.ScriptFile.Instance.Clear();

            if (settings.TableFile.Instance == null)
            {
                return;
            }

            string romPath = Path.Combine(ConfigSerializer.RootDirectory, project.InputFile);

            CorrespondenceTable table = new();

            var tableFile = settings.TableFile.Instance;

            string tblPath = Path.Combine(ConfigSerializer.RootDirectory, tableFile.SourceTableFile);
            table.Parse(project.System.Endianess, tblPath);

            if (tableFile.NewLine.HasValue)
            {
                table.BytesToString.Insert(new byte[] { tableFile.NewLine.Value }, "\n");
            }

            table.BytesToString.InsertTerminator(tableFile.Terminator);

            List<RawExtractedData> extractedData = new();
            List<PointerToExtractedData> pointerToExtracterData = new();

            long sourcePhysicalEndAddress = 0;

            using (var romFile = File.OpenRead(romPath))
            {
                using var reader = new BinaryReader(romFile);
                project.System.Origin = settings.SourceRAMAddress;

                long physicalPointerAddress = project.System.RAMToPhysical(settings.SourceRAMAddress);

                for (int entry = 0; entry < settings.Entries; ++entry)
                {
                    reader.BaseStream.Seek(physicalPointerAddress, SeekOrigin.Begin);

                    byte lowPointer = reader.ReadByte();
                    byte highPointer = reader.ReadByte();

                    long currentRamPointer = (highPointer << 8) | lowPointer;

                    RawExtractedData foundExtractedData = extractedData.FirstOrDefault(x => x.Inside(currentRamPointer));
                    if (foundExtractedData != null)
                    {
                        pointerToExtracterData.Add(new PointerToExtractedData()
                        {
                            Pointer = currentRamPointer,
                            ExtractedData = foundExtractedData,
                        });

                        if (!foundExtractedData.Pointers.Contains(currentRamPointer))
                        {
                            foundExtractedData.Pointers.Add(currentRamPointer);
                        }
                    }
                    else
                    {
                        RawExtractedData newExtracterData = new();
                        newExtracterData.StartAddress = currentRamPointer;
                        newExtracterData.Pointers.Add(currentRamPointer);

                        extractedData.Add(newExtracterData);
                        pointerToExtracterData.Add(new PointerToExtractedData()
                        {
                            Pointer = currentRamPointer,
                            ExtractedData = newExtracterData
                        });
                    }

                    physicalPointerAddress += 2;
                }

                extractedData.Sort((x, y) => x.StartAddress.CompareTo(y.StartAddress));

                int extractIndex = 0;
                for(; extractIndex < (extractedData.Count - 1); ++extractIndex)
                {
                    long startAddress = extractedData[extractIndex].StartAddress;
                    long endAddress = extractedData[extractIndex + 1].StartAddress;

                    int size = (int)(endAddress - startAddress - 1);

                    long physicalDataAddress = project.System.RAMToPhysical(project.System.AbsoluteRAMAddress(startAddress));

                    reader.BaseStream.Seek(physicalDataAddress, SeekOrigin.Begin);

                    extractedData[extractIndex].RawData.AddRange(reader.ReadBytes(size));

                    sourcePhysicalEndAddress = Math.Max(reader.BaseStream.Position, sourcePhysicalEndAddress);
                }

                if (extractIndex < extractedData.Count)
                {
                    long startAddress = extractedData[extractIndex].StartAddress;
                    long physicalDataAddress = project.System.RAMToPhysical(project.System.AbsoluteRAMAddress(startAddress));

                    reader.BaseStream.Seek(physicalDataAddress, SeekOrigin.Begin);

                    byte readByte = reader.ReadByte();

                    bool readingText = true;

                    while (readingText)
                    {
                        extractedData[extractIndex].RawData.Add(readByte);

                        switch (readByte)
                        {
                            case 0xF0:
                                extractedData[extractIndex].RawData.AddRange(reader.ReadBytes(3));
                                break;
                            case 0xF1:
                                extractedData[extractIndex].RawData.AddRange(reader.ReadBytes(2));
                                break;
                            case 0xF2:
                                extractedData[extractIndex].RawData.AddRange(reader.ReadBytes(3));
                                break;
                            case 0xF3:
                                extractedData[extractIndex].RawData.AddRange(reader.ReadBytes(3));
                                break;
                            case 0xF4:
                                break;
                            case 0xF5:
                                extractedData[extractIndex].RawData.AddRange(reader.ReadBytes(1));
                                break;
                            case 0xF6:
                                extractedData[extractIndex].RawData.AddRange(reader.ReadBytes(1));
                                break;
                            case 0xF7:
                                extractedData[extractIndex].RawData.AddRange(reader.ReadBytes(2));
                                break;
                            case 0xF8:
                                extractedData[extractIndex].RawData.AddRange(reader.ReadBytes(1));
                                break;
                            case 0xF9:
                                extractedData[extractIndex].RawData.AddRange(reader.ReadBytes(4));
                                break;
                            case 0xFA:
                                extractedData[extractIndex].RawData.AddRange(reader.ReadBytes(1));
                                break;
                            case 0xFB:
                                break;
                            case 0xFC:
                                extractedData[extractIndex].RawData.AddRange(reader.ReadBytes(2));
                                break;
                            case 0xFD:
                                extractedData[extractIndex].RawData.AddRange(reader.ReadBytes(2));
                                break;
                            case 0xFE:
                                extractedData[extractIndex].RawData.AddRange(reader.ReadBytes(1));
                                break;
                            case 0xFF:
                                extractedData[extractIndex].RawData.RemoveAt(extractedData[extractIndex].RawData.Count - 1);
                                readingText = false;
                                break;
                            default:
                                break;
                        }

                        readByte = reader.ReadByte();
                    }

                    sourcePhysicalEndAddress = Math.Max(reader.BaseStream.Position, sourcePhysicalEndAddress);
                }

                foreach (var data in extractedData)
                {
                    TrieNode<byte, string> byteToStringNode = table.BytesToString.Root;

                    StringBuilder lineBuilder = new();

                    bool previousByteIsText = false;

                    Queue<long> relativePointers = new();

                    if (data.Pointers.Count > 0)
                    {
                        data.Pointers.Sort();
                        foreach (var pointer in data.Pointers)
                        {
                            relativePointers.Enqueue(pointer - data.StartAddress);
                        }
                    }

                    Queue<TrieNode<byte, string>> recognizeQueue = new();

                    int pointerCount = 0;
                    for(int dataIndex = 0; dataIndex < data.RawData.Count; ++dataIndex)
                    {
                        byte value = data.RawData[dataIndex];

                        if (relativePointers.Count > 0)
                        {
                            if (relativePointers.Peek() == dataIndex)
                            {
                                lineBuilder.Append($"<PTR{pointerCount:d3}>");

                                relativePointers.Dequeue();

                                ++pointerCount;
                            }
                        }

                        byteToStringNode = byteToStringNode.Find(value);

                        // Special case for the new line byte to not insert a new line character
                        // if previous symbols weren't recognized as text
                        if (byteToStringNode != null
                            && byteToStringNode.IsValid
                            && byteToStringNode.Value == "\n"
                            && !previousByteIsText)
                        {
                            byteToStringNode = null;
                        }

                        if (byteToStringNode != null)
                        {
                            if (byteToStringNode.IsValid)
                            {
                                lineBuilder.Append(byteToStringNode.Value);

                                byteToStringNode = table.BytesToString.Root;

                                recognizeQueue.Clear();

                                previousByteIsText = true;
                            }
                            else
                            {
                                recognizeQueue.Enqueue(byteToStringNode);
                            }
                        }
                        else
                        {
                            while (recognizeQueue.Count > 0)
                            {
                                TrieNode<byte, string> currentNode = recognizeQueue.Dequeue();
                                lineBuilder.AppendFormat("<{0:x2}>", currentNode.Key);
                            }

                            lineBuilder.AppendFormat("<{0:x2}>", value);

                            byteToStringNode = table.BytesToString.Root;
                        }
                    }

                    ScriptEntry newEntry = ReferenceDatabase.Instance.CreateNew<ScriptEntry>();
                    newEntry.EntryName = data.ToString();
                    newEntry.Original = lineBuilder.ToString();

                    foreach (var langKeyPair in project.Lang)
                    {
                        if (!newEntry.HasTranslation(langKeyPair.Key))
                        {
                            newEntry.Translations.Add(new TranslationEntry { Lang = langKeyPair.Key, Value = newEntry.Original });
                        }
                    }

                    settings.ScriptFile.Instance.Entries.Add(newEntry);

                    data.DataEntry = newEntry;
                }

                foreach (var entry in pointerToExtracterData)
                {
                    settings.ScriptFile.Instance.Pointers.Add(new ScriptPointer
                    {
                        PointerIndex = entry.ExtractedData.Pointers.IndexOf(entry.Pointer),
                        Pointer = new RefObjectPtr<ScriptEntry>() { Instance = entry.ExtractedData.DataEntry }
                    });
                }
            }

            settings.DestinationEndRAMAddress = settings.DestinationRAMAddress + (project.System.PhysicalToRAM(sourcePhysicalEndAddress) - settings.SourceRAMAddress);

            settings.ScriptFile.Save();
        }
    }
}
