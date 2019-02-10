using System.Collections.Generic;
using System.IO;

namespace WendigoJaeger.TranslationTool.Outputs.SNES
{
    public partial class SNESBassTemplate : SNESBassTemplateBase
    {
        public bool IsLoROM { get; set; }

        public OutputInfo OutputInfo { get; set; }

        public IEnumerable<OutputScriptBank> Scripts
        {
            get
            {
                return OutputInfo.Scripts;
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

        public string IncludeFileName(OutputScriptBank script)
        {
            return Path.ChangeExtension(script.FileName, ".inc");
        }
    }
}
