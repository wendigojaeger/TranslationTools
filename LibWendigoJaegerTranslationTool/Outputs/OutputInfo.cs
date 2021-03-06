﻿using System.Collections.Generic;
using WendigoJaeger.TranslationTool.Systems;

namespace WendigoJaeger.TranslationTool.Outputs
{
    public class OutputInfo
    {
        public string BuildDirectory { get; set; }

        public string InputFile { get; set; }
        public string OutputFile { get; set; }

        public List<OutputScriptBank> Scripts { get; } = new List<OutputScriptBank>();

        public List<OutputGraphics> Graphics { get; } = new List<OutputGraphics>();

        public List<OutputAssemblyFile> AssemblyFiles { get; } = new List<OutputAssemblyFile>();

        public List<OutputDataBank> DataBanks { get; } = new List<OutputDataBank>();

        public ISystem System { get; set; }
    }
}
