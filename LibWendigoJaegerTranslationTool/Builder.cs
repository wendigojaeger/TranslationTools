using System.IO;
using WendigoJaeger.TranslationTool.Data;
using WendigoJaeger.TranslationTool.Outputs;

namespace WendigoJaeger.TranslationTool
{
    public class Builder
    {
        public Reporter Reporter { get; set; }

        public void Build(string targetLanguage, ProjectSettings settings)
        {
            Reporter.Info("Build ROM for language '{0}'", targetLanguage);

            OutputInfo outputInfo = new OutputInfo
            {
                BuildDirectory = Path.GetDirectoryName(settings.Path),
                System = settings.Project.System,
                InputFile = settings.Project.InputFile,
                OutputFile = settings.Project.Lang[targetLanguage].OutputFile,
            };

            foreach (var script in settings.ScriptSettings)
            {
                OutputScriptBank outputScriptBank = new()
                {
                    Name = script.Name,
                    FileName = script.ScriptFile.Path,
                    RAMAddress = script.DestinationRAMAddress,
                    EndRAMAddress = script.DestinationEndRAMAddress,
                };

                OutputConveter.ConverScript(Reporter, targetLanguage, outputInfo.System.Endianess, script, outputScriptBank);

                if (Reporter.HasErrors)
                {
                    return;
                }

                outputInfo.ScriptBanks.Add(outputScriptBank);
            }

            foreach (var graphics in settings.Graphics)
            {
                var outputGraphics = new OutputGraphics
                {
                    FileName = graphics[targetLanguage],
                    RAMAddress = graphics.RAMAddress
                };

                outputInfo.Graphics.Add(outputGraphics);
            }

            foreach (var assemblyFile in settings.AssemblyFileSettings)
            {
                outputInfo.AssemblyFiles.Add(new OutputAssemblyFile() { Path = assemblyFile.FilePath });
            }

            settings.Project.OutputGenerator.Generate(Reporter, outputInfo);
        }
    }
}
