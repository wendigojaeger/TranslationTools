using System.Collections.Generic;

namespace WendigoJaeger.TranslationTool.Outputs.SNES
{
    public partial class SNESBassScriptIncludeTemplate : SNESBassScriptIncludeTemplateBase
    {
        enum OutputState
        {
            PrintPointerLabel,
            OutputDB
        }

        public OutputScriptBank ScriptBank { get; set; }

        public List<OutputScriptEntry> Entries
        {
            get
            {
                return ScriptBank.Entries;
            }
        }

        public IEnumerable<string> GetPointers()
        {
            foreach(var pointer in ScriptBank.Pointers)
            {
                int entryIndex = Entries.IndexOf(pointer.Entry);
                yield return GenerateEntryName(entryIndex, pointer.Index);
            }
        }

        public int GetPointerCount(OutputScriptEntry entry)
        {
            int pointerCount = 0;

            foreach(var pointer in ScriptBank.Pointers)
            {
                if (pointer.Entry == entry)
                {
                    ++pointerCount;
                }
            }

            return pointerCount;
        }

        public string GenerateEntryName(int index, int pointer = 0)
        {
            return $"__{ScriptBank.RAMAddress:x}_{index}_{pointer}";
        }

        public static string FormatEntryName(string entryName)
        {
            return SNESBassTemplate.FormatEntryName(entryName);
        }
    }
}
