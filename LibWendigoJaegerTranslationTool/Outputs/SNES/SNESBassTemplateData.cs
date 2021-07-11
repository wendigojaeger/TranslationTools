using System.Collections.Generic;
using System.IO;

namespace WendigoJaeger.TranslationTool.Outputs.SNES
{
    public partial class SNESBassTemplate : SNESBassTemplateBase
    {
        public bool IsLoROM { get; set; }

        public OutputInfo OutputInfo { get; set; }

        public IEnumerable<OutputScriptBank> ScriptBanks
        {
            get
            {
                return OutputInfo.ScriptBanks;
            }
        }

        public IEnumerable<OutputGraphics> Graphics
        {
            get
            {
                return OutputInfo.Graphics;
            }
        }

        public IEnumerable<OutputAssemblyFile> AssemblyFiles
        {
            get
            {
                return OutputInfo.AssemblyFiles;
            }
        }

        public static string IncludeFileName(OutputScriptBank scriptBank)
        {
            return Path.ChangeExtension(scriptBank.FileName, ".inc");
        }
    }
}
