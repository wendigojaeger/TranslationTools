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

            OutputInfo outputInfo = new()
            {
                BuildDirectory = Path.GetDirectoryName(settings.Path),
                System = settings.Project.System,
                InputFile = settings.Project.InputFile,
                OutputFile = settings.Project.Lang[targetLanguage].OutputFile,
            };

            foreach (ScriptDictionary scriptDictionary in settings.ScriptDictionaries)
            {
                OutputScriptDictionary outputScriptDictionary = new();
                outputScriptDictionary.Name = scriptDictionary.Name;
                outputScriptDictionary.RAMAddress = scriptDictionary.RAMAddress;

                foreach (ScriptSettings script in scriptDictionary.Scripts)
                {
                    OutputScriptBank outputScriptBank = convertScriptSettingsToScriptBank(script, targetLanguage, outputInfo);
                    if (outputScriptBank != null)
                    {
                        outputScriptDictionary.Scripts.Add(outputScriptBank);

                        outputInfo.ScriptBanks.Add(outputScriptBank);
                    }
                }

                outputInfo.ScriptDictionaries.Add(outputScriptDictionary);
            }

            foreach (ScriptSettings script in settings.ScriptSettings)
            {
                OutputScriptBank outputScriptBank = convertScriptSettingsToScriptBank(script, targetLanguage, outputInfo);

                if (outputScriptBank != null)
                {
                    outputInfo.ScriptBanks.Add(outputScriptBank);
                }
            }

            foreach (GraphicsSettings graphics in settings.Graphics)
            {
                var outputGraphics = new OutputGraphics
                {
                    FileName = graphics[targetLanguage],
                    RAMAddress = graphics.RAMAddress
                };

                outputInfo.Graphics.Add(outputGraphics);
            }

            foreach (AssemblyFileSettings assemblyFile in settings.AssemblyFileSettings)
            {
                outputInfo.AssemblyFiles.Add(new OutputAssemblyFile() { Path = assemblyFile.FilePath });
            }

            settings.Project.OutputGenerator.Generate(Reporter, outputInfo);
        }

        private OutputScriptBank convertScriptSettingsToScriptBank(ScriptSettings script, string targetLanguage, OutputInfo outputInfo)
        {
            OutputScriptBank outputScriptBank = new()
            {
                Name = script.Name,
                FileName = script.ScriptFile.Path,
                RAMAddress = script.DestinationRAMAddress,
                EndRAMAddress = script.DestinationEndRAMAddress,
            };

            OutputConveter.ConvertScript(Reporter, targetLanguage, outputInfo.System.Endianess, script, outputScriptBank);

            if (Reporter.HasErrors)
            {
                return null;
            }

            return outputScriptBank;
        }
    }
}
