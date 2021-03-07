using System;
using System.Collections.Generic;
using System.IO;
using WendigoJaeger.TranslationTool.Data;
using WendigoJaeger.TranslationTool.Outputs;

namespace WendigoJaeger.TranslationTool
{
    class ScriptConveter
    {
        public static void Convert(Reporter reporter, string targetLanguage, Endian endian, ScriptSettings script, OutputScriptBank output)
        {
            CorrespondenceTable table = new CorrespondenceTable();

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
                List<byte> data = new List<byte>();

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
    }
}
