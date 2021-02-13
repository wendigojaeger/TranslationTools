using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using WendigoJaeger.TranslationTool.Data;

namespace WendigoJaeger.TranslationTool.Extractors
{
    [DisplayName("16-bit Pointer Text Extractor (Little Endian)")]
    public class LittleEndianPointer16TextExtractor : ITextExtractor
    {
        [JsonIgnore]
        public ScriptBankType BankType => ScriptBankType.Pointer16;

        public void Extract(Project project, ScriptSettings settings)
        {
            settings.Script.Instance.Entries.Clear();

            string romPath = Path.Combine(ConfigSerializer.RootDirectory, project.InputFile);

            CorrespondenceTable table = new CorrespondenceTable();

            var tableFile = settings.TableFile.Instance;

            string tblPath = Path.Combine(ConfigSerializer.RootDirectory, tableFile.SourceTableFile);
            table.Parse(project.System.Endianess, tblPath);

            using (var romFile = File.OpenRead(romPath))
            {
                using (var reader = new BinaryReader(romFile))
                {
                    project.System.Origin = settings.SourceRAMAddress;

                    long physicalPointerAddress = project.System.RAMToPhysical(settings.SourceRAMAddress);

                    for (int entry=0; entry<settings.Entries; ++entry)
                    {
                        reader.BaseStream.Seek(physicalPointerAddress, SeekOrigin.Begin);

                        byte lowPointer = reader.ReadByte();
                        byte highPointer = reader.ReadByte();

                        long currentRamPointer = (highPointer << 8) | lowPointer;

                        long physicalDataAddress = project.System.RAMToPhysical(project.System.AbsoluteRAMAddress(currentRamPointer));

                        reader.BaseStream.Seek(physicalDataAddress, SeekOrigin.Begin);

                        List<byte> rawData = new List<byte>();

                        byte readByte = reader.ReadByte();

                        while (readByte != tableFile.Terminator)
                        {
                            rawData.Add(readByte);

                            readByte = reader.ReadByte();
                        }

                        TrieNode<byte, string> byteToStringNode = table.BytesToString.Root;

                        StringBuilder lineBuilder = new StringBuilder();

                        bool previousByteIsText = false;

                        for (int i=0; i<rawData.Count; ++i)
                        {
                            byte value = rawData[i];

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

                        ScriptEntry newEntry = new ScriptEntry();
                        newEntry.EntryName = $"Entry #{entry}";
                        newEntry.Original = lineBuilder.ToString();

                        settings.Script.Instance.Entries.Add(newEntry);

                        physicalPointerAddress += 2;
                    }
                }
            }

            settings.Script.Save();
        }
    }
}
