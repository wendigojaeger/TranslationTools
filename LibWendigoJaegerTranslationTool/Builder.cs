using System.IO;
using WendigoJaeger.TranslationTool.Data;
using WendigoJaeger.TranslationTool.Outputs;

namespace WendigoJaeger.TranslationTool
{
    public class Builder
    {
        public Reporter Reporter { get; } = new Reporter();

        public void Build(string targetLanguage, ProjectSettings settings)
        {
            OutputInfo outputInfo = new OutputInfo
            {
                BuildDirectory = Path.GetDirectoryName(settings.Path),
                System = settings.Project.System,
                InputFile = settings.Project.InputFile,
                OutputFile = settings.Project.Lang[targetLanguage].OutputFile
            };

            foreach (var script in settings.Scripts)
            {
                OutputScriptBank outputBank = new OutputScriptBank
                {
                    Name = script.Name,
                    BankType = script.TextExtractor.BankType,
                    FileName = script.Script.Path,
                    RAMAddress = script.DestinationRAMAddress,
                    EndRAMAddress = script.DestinationEndRAMAddress
                };

                ScriptConveter.Convert(Reporter, targetLanguage, outputInfo.System.Endianess, script, outputBank);

                if (Reporter.HasErrors)
                {
                    return;
                }

                outputInfo.Scripts.Add(outputBank);
            }

            foreach(var graphics in settings.Graphics)
            {
                var outputGraphics = new OutputGraphics
                {
                    FileName = graphics[targetLanguage],
                    RAMAddress = graphics.RAMAddress
                };

                outputInfo.Graphics.Add(outputGraphics);
            }

            settings.Project.OutputGenerator.Generate(Reporter, outputInfo);
        }
    }
}
