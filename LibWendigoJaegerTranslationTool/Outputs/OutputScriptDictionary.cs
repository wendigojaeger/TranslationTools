using System.Collections.Generic;

namespace WendigoJaeger.TranslationTool.Outputs
{
    public class OutputScriptDictionary
    {
        public string Name { get; set; }

        public long RAMAddress { get; set; }

        public List<OutputScriptBank> Scripts { get; } = new List<OutputScriptBank>();
    }
}
