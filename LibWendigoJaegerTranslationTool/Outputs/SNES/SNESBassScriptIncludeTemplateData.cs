using System.Collections.Generic;

namespace WendigoJaeger.TranslationTool.Outputs.SNES
{
    public partial class SNESBassScriptIncludeTemplate : SNESBassScriptIncludeTemplateBase
    {
        public OutputScriptBank ScriptBank { get; set; }

        public List<OutputScriptEntry> Entries
        {
            get
            {
                return ScriptBank.Entries;
            }
        }

        public string GenerateEntryName(int index)
        {
            return $"__{ScriptBank.RAMAddress:x}_{index}";
        }

        public string FormatEntryName(string entryName)
        {
            return entryName.Replace(' ', '_');
        }
    }
}
