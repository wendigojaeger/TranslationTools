using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WendigoJaeger.TranslationTool.Outputs.SNES
{
    public partial class SNESBassDataIncludeTemplate : SNESBassDataIncludeTemplateBase
    {
        enum OutputState
        {
            PrintPointerLabel,
            OutputDB
        }

        public OutputDataBank DataBank { get; set; }

        public List<OutputDataEntry> Entries
        {
            get
            {
                return DataBank.Entries;
            }
        }

        public IEnumerable<string> GetPointers()
        {
            foreach(var pointer in DataBank.Pointers)
            {
                int entryIndex = Entries.IndexOf(pointer.Entry);
                yield return GenerateEntryName(entryIndex, pointer.Index);
            }
        }

        public string GenerateEntryName(int index, int pointer = 0)
        {
            return $"__{DataBank.RAMAddress:x}_{index}_{pointer}";
        }

        public string FormatEntryName(string entryName)
        {
            return entryName.Replace(' ', '_');
        }

        public void PrintEntries()
        {
            for(int entryIndex = 0; entryIndex < Entries.Count; ++entryIndex)
            {
                var entry = Entries[entryIndex];

                int pointerCount = 0;
                Queue<int> pointerIndices = new(entry.Pointers);

                OutputState state = OutputState.OutputDB;

                int dataIndex = 0;
                while(dataIndex < entry.Data.Length)
                {
                    switch (state)
                    {
                        case OutputState.PrintPointerLabel:
                        {
                            // <#= GenerateEntryName(entryIndex, pointerCount): #>
                            // db
                            pointerCount++;
                            state = OutputState.OutputDB;
                            break;
                        }
                        case OutputState.OutputDB:
                        {
                            if (pointerIndices.Count > 0 && pointerIndices.Peek() == dataIndex)
                            {
                                pointerIndices.Dequeue();
                                state = OutputState.PrintPointerLabel;
                            }
                            else
                            {
                                // <#= $"{entry.Data[dataIndex]:x}" #>
                                dataIndex++;
                            }
                            break;
                        }
                    }
                }
            }
        }
    }
}
