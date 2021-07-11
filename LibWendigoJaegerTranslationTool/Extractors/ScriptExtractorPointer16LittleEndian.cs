using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using WendigoJaeger.TranslationTool.Data;
using System;
using Newtonsoft.Json;

namespace WendigoJaeger.TranslationTool.Extractors
{
    public class ScriptExtractorPointer16LittleEndian : IScriptExtractor
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
        public string Name => LibResource.scriptExtractorPointer16LittleEndian;

        public void Extract(Project project, ScriptSettings settings)
        {
            settings.ScriptFile.Instance.Clear();

            string romPath = Path.Combine(ConfigSerializer.RootDirectory, project.InputFile);

            CorrespondenceTable table = new();

            var tableFile = settings.TableFile.Instance;

            string tblPath = Path.Combine(ConfigSerializer.RootDirectory, tableFile.SourceTableFile);
            table.Parse(project.System.Endianess, tblPath);

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

                    long physicalDataAddress = project.System.RAMToPhysical(project.System.AbsoluteRAMAddress(currentRamPointer));

                    RawExtractedData foundExtractedData = extractedData.Where(x => x.Inside(currentRamPointer)).FirstOrDefault();
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
                        reader.BaseStream.Seek(physicalDataAddress, SeekOrigin.Begin);

                        RawExtractedData newExtracterData = new();
                        newExtracterData.StartAddress = currentRamPointer;
                        newExtracterData.Pointers.Add(currentRamPointer);

                        byte readByte = reader.ReadByte();

                        while (readByte != tableFile.Terminator)
                        {
                            newExtracterData.RawData.Add(readByte);

                            readByte = reader.ReadByte();
                        }

                        extractedData.Add(newExtracterData);
                        pointerToExtracterData.Add(new PointerToExtractedData()
                        {
                            Pointer = currentRamPointer,
                            ExtractedData = newExtracterData
                        });

                        sourcePhysicalEndAddress = Math.Max(reader.BaseStream.Position, sourcePhysicalEndAddress);
                    }

                    physicalPointerAddress += 2;
                }

                extractedData.Sort((x, y) => x.StartAddress.CompareTo(y.StartAddress));

                foreach(var data in extractedData)
                {
                    TrieNode<byte, string> byteToStringNode = table.BytesToString.Root;

                    StringBuilder lineBuilder = new();

                    bool previousByteIsText = false;

                    Queue<long> relativePointers = new();

                    if(data.Pointers.Count > 0)
                    {
                        data.Pointers.Sort();
                        foreach(var pointer in data.Pointers)
                        {
                            relativePointers.Enqueue(pointer - data.StartAddress);
                        }
                    }

                    int pointerCount = 0;
                    for (int i = 0; i < data.RawData.Count; ++i)
                    {
                        byte value = data.RawData[i];

                        if (relativePointers.Count > 0)
                        {
                            if (relativePointers.Peek() == i)
                            {
                                lineBuilder.Append($"<PTR{pointerCount:d3}>");

                                relativePointers.Dequeue();

                                ++pointerCount;
                            }
                        }

                        if (tableFile.NewLine.HasValue && tableFile.NewLine.Value == value && previousByteIsText)
                        {
                            lineBuilder.Append('\n');
                        }
                        else
                        {
                            byteToStringNode = byteToStringNode.Find(value);
                            if (byteToStringNode != null && byteToStringNode.IsValid)
                            {
                                lineBuilder.Append(byteToStringNode.Value);

                                byteToStringNode = table.BytesToString.Root;

                                previousByteIsText = true;
                            }
                            else if (byteToStringNode == null || byteToStringNode.IsLeaf)
                            {
                                lineBuilder.AppendFormat("<{0:x2}>", value);

                                byteToStringNode = table.BytesToString.Root;

                                previousByteIsText = false;
                            }
                        }
                    }

                    ScriptEntry newEntry = ReferenceDatabase.Instance.CreateNew<ScriptEntry>();
                    newEntry.EntryName = data.ToString();
                    newEntry.Original = lineBuilder.ToString();

                    settings.ScriptFile.Instance.Entries.Add(newEntry);

                    data.DataEntry = newEntry;
                }

                foreach(var entry in pointerToExtracterData)
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
