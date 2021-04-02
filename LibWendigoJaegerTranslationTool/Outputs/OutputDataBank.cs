using System.Collections.Generic;

namespace WendigoJaeger.TranslationTool.Outputs
{
    public class OutputDataEntry
    {
        public List<int> Pointers = new();
        public byte[] Data;
    }

    public struct OutputDataPointer
    {
        public int Index;
        public OutputDataEntry Entry;
    }

    public class OutputDataBank
    {
        public string Name { get; set; }

        public string FileName { get; set; }

        public long RAMAddress { get; set; }

        public long? EndRAMAddress { get; set; }

        public byte Terminator { get; set; }

        public List<OutputDataEntry> Entries { get; } = new List<OutputDataEntry>();

        public List<OutputDataPointer> Pointers { get; } = new List<OutputDataPointer>();
    }
}
