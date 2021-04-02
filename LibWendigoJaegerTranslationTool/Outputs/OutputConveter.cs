using System;
using System.Collections.Generic;
using System.IO;
using WendigoJaeger.TranslationTool.Data;

namespace WendigoJaeger.TranslationTool.Outputs
{
    class OutputConveter
    {
        public static void ConvertScript(Reporter reporter, string targetLanguage, Endian endian, ScriptSettings script, OutputScriptBank output)
        {
            CorrespondenceTable table = new();

            var tableFile = script.TableFile.Instance;

            string tblFileName = tableFile.GetTargetTable(targetLanguage).Path;
            string tblPath = Path.Combine(ConfigSerializer.RootDirectory, tblFileName);
            table.Parse(endian, tblPath);

            for(int i = 0; i <= 0xFF; ++i)
            {
                table.StringToBytes.Insert($"<{i:x2}>", new byte[] { (byte)i });
            }

            if (tableFile.NewLine.HasValue)
            {
                table.StringToBytes.Insert("\n", new byte[] { tableFile.NewLine.Value });
            }

            output.Terminator = tableFile.Terminator;

            foreach (var entry in script.Script.Instance.Entries)
            {
                List<byte> data = new();

                string scriptLine = entry[targetLanguage];

                TrieNode<char, byte[]> stringToByteNode = table.StringToBytes.Root;

                Stack<Tuple<int, TrieNode<char, byte[]>>> recognizeStack = new Stack<Tuple<int, TrieNode<char, byte[]>>>();

                int line = 1;
                int column = 1;

                int index = 0;
                while (index < scriptLine.Length)
                {
                    stringToByteNode = stringToByteNode.Find(scriptLine[index]);

                    if (scriptLine[index] == '\n')
                    {
                        line++;
                        column = 1;
                    }

                    if (stringToByteNode != null)
                    {
                        if (stringToByteNode.IsLeaf)
                        {
                            data.AddRange(stringToByteNode.Value);

                            stringToByteNode = table.StringToBytes.Root;

                            recognizeStack.Clear();
                        }
                        else
                        {
                            recognizeStack.Push(Tuple.Create(index, stringToByteNode));
                        }
                    }
                    else
                    {
                        while (recognizeStack.Count > 0)
                        {
                            var currentTuple = recognizeStack.Pop();

                            if (currentTuple.Item2.Value != null)
                            {
                                data.AddRange(currentTuple.Item2.Value);

                                stringToByteNode = table.StringToBytes.Root;

                                index = currentTuple.Item1;

                                recognizeStack.Clear();
                                break;
                            }
                        }
                    }

                    if (stringToByteNode == null)
                    {
                        reporter.Error(LibResource.scriptConverterError, scriptLine[index], tblFileName, script.Name, entry.EntryName, targetLanguage, line, column);

                        stringToByteNode = table.StringToBytes.Root;
                    }

                    index++;
                    column++;
                }

                output.Entries.Add(new OutputScriptEntry() { Data = data.ToArray() });
            }
        }

        static readonly byte[] PointerArray = new byte[] { 0xDE, 0xAD, 0xBE, 0xEF };

        public static void ConvertData(Reporter reporter, string targetLanguage, Endian endian, DataSettings dataSettings, OutputDataBank output)
        {
            CorrespondenceTable table = new();

            var tableFile = dataSettings.TableFile.Instance;

            string tblFileName = tableFile.GetTargetTable(targetLanguage).Path;
            string tblPath = Path.Combine(ConfigSerializer.RootDirectory, tblFileName);
            table.Parse(endian, tblPath);

            for (int i = 0; i <= 0xFF; ++i)
            {
                table.StringToBytes.Insert($"<{i:x2}>", new byte[] { (byte)i });
            }

            if (tableFile.NewLine.HasValue)
            {
                table.StringToBytes.Insert("\n", new byte[] { tableFile.NewLine.Value });
            }

            for (int i = 0; i <= 0xFF; ++i)
            {
                table.StringToBytes.Insert($"<PTR{i:d3}>", PointerArray);
            }

            output.Terminator = tableFile.Terminator;

            Dictionary<Guid, OutputDataEntry> sourceToDesinationDataEntries = new();

            foreach (var entry in dataSettings.DataFile.Instance.DataEntries)
            {
                OutputDataEntry outputEntry = new();
                List<byte> data = new();

                string scriptLine = entry[targetLanguage];

                TrieNode<char, byte[]> stringToByteNode = table.StringToBytes.Root;

                Stack<Tuple<int, TrieNode<char, byte[]>>> recognizeStack = new Stack<Tuple<int, TrieNode<char, byte[]>>>();

                int line = 1;
                int column = 1;

                int index = 0;
                while (index < scriptLine.Length)
                {
                    stringToByteNode = stringToByteNode.Find(scriptLine[index]);

                    if (scriptLine[index] == '\n')
                    {
                        line++;
                        column = 1;
                    }

                    if (stringToByteNode != null)
                    {
                        if (stringToByteNode.IsLeaf)
                        {
                            if (stringToByteNode.Value.Length == PointerArray.Length && Equals(stringToByteNode.Value, PointerArray))
                            {
                                outputEntry.Pointers.Add(data.Count);
                            }
                            else
                            {
                                data.AddRange(stringToByteNode.Value);
                            }

                            stringToByteNode = table.StringToBytes.Root;

                            recognizeStack.Clear();
                        }
                        else
                        {
                            recognizeStack.Push(Tuple.Create(index, stringToByteNode));
                        }
                    }
                    else
                    {
                        while (recognizeStack.Count > 0)
                        {
                            var currentTuple = recognizeStack.Pop();

                            if (currentTuple.Item2.Value != null)
                            {
                                if (stringToByteNode.Value.Length == PointerArray.Length && Equals(stringToByteNode.Value, PointerArray))
                                {
                                    outputEntry.Pointers.Add(data.Count);
                                }
                                else
                                {
                                    data.AddRange(currentTuple.Item2.Value);
                                }

                                stringToByteNode = table.StringToBytes.Root;

                                index = currentTuple.Item1;

                                recognizeStack.Clear();
                                break;
                            }
                        }
                    }

                    if (stringToByteNode == null)
                    {
                        reporter.Error(LibResource.scriptConverterError, scriptLine[index], tblFileName, dataSettings.Name, entry.Name, targetLanguage, line, column);

                        stringToByteNode = table.StringToBytes.Root;
                    }

                    index++;
                    column++;
                }

                outputEntry.Data = data.ToArray();

                output.Entries.Add(outputEntry);

                sourceToDesinationDataEntries.Add(entry.ID, outputEntry);
            }

            foreach(var pointer in dataSettings.DataFile.Instance.Pointers)
            {
                output.Pointers.Add(new OutputDataPointer()
                {
                    Index = pointer.PointerIndex,
                    Entry = sourceToDesinationDataEntries[pointer.Pointer.RefID]
                });
            }
        }
    }
}
