using System.Collections.Generic;
using System.IO;

namespace WendigoJaeger.TranslationTool.Outputs.SNES
{
    public partial class SNESBassTemplate : SNESBassTemplateBase
    {
        public bool IsLoROM { get; set; }

        public OutputInfo OutputInfo { get; set; }

        public IEnumerable<OutputScriptDictionary> ScriptDictionaries => OutputInfo.ScriptDictionaries;

        public IEnumerable<OutputScriptBank> ScriptBanks
        {
            get
            {
                foreach (OutputScriptBank scriptBank in OutputInfo.ScriptBanks)
                {
                    if (scriptBank.IsValid)
                    {
                        yield return scriptBank;
                    }
                }
            }
        }

        public IEnumerable<OutputGraphics> Graphics => OutputInfo.Graphics;

        public IEnumerable<OutputAssemblyFile> AssemblyFiles => OutputInfo.AssemblyFiles;

        public static string IncludeFileName(OutputScriptBank scriptBank)
        {
            return Path.ChangeExtension(scriptBank.FileName, ".inc");
        }

        public static string FormatScriptBankPointer(OutputScriptBank scriptBank)
        {
            if (scriptBank.IsValid)
            {
                return FormatEntryName(scriptBank.Name);
            }
            else
            {
                return "0";
            }
        }

        public static string FormatEntryName(string entryName)
        {
            return entryName.Replace(' ', '_');
        }
    }
}
