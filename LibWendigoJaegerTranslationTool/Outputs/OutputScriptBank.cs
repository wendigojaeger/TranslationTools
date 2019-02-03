using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WendigoJaeger.TranslationTool.Outputs
{
    public class OutputScriptEntry
    {
        public byte[] Data;
    }

    public class OutputScriptBank
    {
        public string Name { get; set; }

        public string FileName { get; set; }

        public long RAMAddress { get; set; }

        public long? EndRAMAddress { get; set; }

        public byte Terminator { get; set; }

        public ScriptBankType BankType { get; set; }

        public List<OutputScriptEntry> Entries { get; } = new List<OutputScriptEntry>();
    }
}
