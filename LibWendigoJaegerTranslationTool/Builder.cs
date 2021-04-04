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

                OutputConveter.ConvertScript(Reporter, targetLanguage, outputInfo.System.Endianess, script, outputBank);

                if (Reporter.HasErrors)
                {
                    return;
                }

                outputInfo.Scripts.Add(outputBank);
            }

            foreach(var data in settings.DataSettings)
            {
                OutputDataBank outputData = new()
                {
                    Name = data.Name,
                    FileName = data.DataFile.Path,
                    RAMAddress = data.DestinationRAMAddress,
                    EndRAMAddress = data.DestinationEndRAMAddress,
                };

                OutputConveter.ConvertData(Reporter, targetLanguage, outputInfo.System.Endianess, data, outputData);

                if (Reporter.HasErrors)
                {
                    return;
                }

                outputInfo.DataBanks.Add(outputData);
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

            foreach(var assemblyFile in settings.AssemblyFileSettings)
            {
                outputInfo.AssemblyFiles.Add(new OutputAssemblyFile() { Path = assemblyFile.FilePath });
            }

            settings.Project.OutputGenerator.Generate(Reporter, outputInfo);
        }
    }
}
