using System.Collections.Generic;

namespace WendigoJaeger.TranslationTool.Outputs
{
    public class OutputScriptEntry
    {
        public List<int> Pointers = new();
        public byte[] Data;
    }

    public struct OutputScriptPointer
    {
        public int Index;
        public OutputScriptEntry Entry;
    }

    public class OutputScriptBank
    {
        public string Name { get; set; }

        public string FileName { get; set; }

        public long RAMAddress { get; set; }

        public long? EndRAMAddress { get; set; }

        public byte Terminator { get; set; }

        public List<OutputScriptEntry> Entries { get; } = new List<OutputScriptEntry>();

        public List<OutputScriptPointer> Pointers { get; } = new List<OutputScriptPointer>();

        public bool IsValid => RAMAddress > 0 && Entries.Count > 0;
    }
}
