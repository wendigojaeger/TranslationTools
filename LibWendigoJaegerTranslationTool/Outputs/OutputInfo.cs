using System.Collections.Generic;
using WendigoJaeger.TranslationTool.Systems;

namespace WendigoJaeger.TranslationTool.Outputs
{
    public class OutputInfo
    {
        public string BuildDirectory { get; set; }

        public string InputFile { get; set; }
        public string OutputFile { get; set; }

        public List<OutputGraphics> Graphics { get; } = new List<OutputGraphics>();

        public List<OutputAssemblyFile> AssemblyFiles { get; } = new List<OutputAssemblyFile>();

        public List<OutputScriptBank> ScriptBanks { get; } = new List<OutputScriptBank>();

        public List<OutputScriptDictionary> ScriptDictionaries { get; } = new List<OutputScriptDictionary>();

        public ISystem System { get; set; }
    }
}
