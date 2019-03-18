using System.Diagnostics;
using System.IO;
using System.Text;
using WendigoJaeger.TranslationTool.Systems;

namespace WendigoJaeger.TranslationTool.Outputs.SNES
{
    public class SNESBassOutput : OutputGenerator
    {
        private static readonly Encoding UTF8 = new UTF8Encoding(false);

        public override void Generate(Reporter reporter, OutputInfo outputInfo)
        {
            foreach(var script in outputInfo.Scripts)
            {
                generateScriptFile(outputInfo.BuildDirectory, script);
            }

            bool isLorom = !(outputInfo.System is SNESHiROM);

            SNESBassTemplate buildTemplate = new SNESBassTemplate
            {
                OutputInfo = outputInfo,
                IsLoROM = isLorom
            };

            string buildFileName = Path.Combine(outputInfo.BuildDirectory, "build.asm");

            using (var buildFile = File.Open(buildFileName, FileMode.Create))
            {
                using (var writer = new StreamWriter(buildFile, UTF8))
                {
                    writer.Write(buildTemplate.TransformText());
                }
            }

            string sourceFileName = Path.Combine(outputInfo.BuildDirectory, outputInfo.InputFile);
            string outputFileName = Path.Combine(outputInfo.BuildDirectory, outputInfo.OutputFile);

            File.Copy(sourceFileName, outputFileName, true);

            Process process = new Process();
            process.StartInfo.FileName = $"\"{Path.Combine(outputInfo.BuildDirectory, "bass.exe")}\"";
            process.StartInfo.WorkingDirectory = outputInfo.BuildDirectory;
            process.StartInfo.Arguments = $"-o {outputInfo.OutputFile} build.asm";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.OutputDataReceived += reporter.ProcessOutputHandler;
            process.Start();

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            reporter.Info(LibResource.bassAssembling);

            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                reporter.Error(LibResource.bassError);
            }

            File.Delete(buildFileName);

            foreach (var script in outputInfo.Scripts)
            {
                File.Delete(Path.Combine(outputInfo.BuildDirectory, Path.ChangeExtension(script.FileName, ".inc")));
            } 
        }

        private void generateScriptFile(string buildDirectory, OutputScriptBank script)
        {
            if (script.BankType == ScriptBankType.Pointer16)
            {
                SNESBassScriptIncludeTemplate scriptTemplate = new SNESBassScriptIncludeTemplate
                {
                    ScriptBank = script
                };

                string scriptFileName = Path.Combine(buildDirectory, Path.ChangeExtension(script.FileName, ".inc"));

                using (var buildFile = File.Open(scriptFileName, FileMode.Create))
                {
                    using (var writer = new StreamWriter(buildFile, UTF8))
                    {
                        writer.Write(scriptTemplate.TransformText());
                    }
                }
            }
        }
    }
}
