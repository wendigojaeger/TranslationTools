using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Text;
using WendigoJaeger.TranslationTool.Systems;

namespace WendigoJaeger.TranslationTool.Outputs.SNES
{
    [DisplayName("SNES - Near's bass assembler")]
    public class SNESBassOutput : OutputGenerator
    {
        private static readonly Encoding UTF8 = new UTF8Encoding(false);

        public override void Generate(Reporter reporter, OutputInfo outputInfo)
        {
            foreach(var script in outputInfo.Scripts)
            {
                generateScriptFile(outputInfo.BuildDirectory, script);
            }

            foreach (var dataBank in outputInfo.DataBanks)
            {
                generateDataFile(outputInfo.BuildDirectory, dataBank);
            }

            bool isLorom = !(outputInfo.System is SNESHiROM);

            SNESBassTemplate buildTemplate = new()
            {
                OutputInfo = outputInfo,
                IsLoROM = isLorom
            };

            string buildFileName = Path.Combine(outputInfo.BuildDirectory, "build.asm");

            using (var buildFile = File.Open(buildFileName, FileMode.Create))
            {
                using var writer = new StreamWriter(buildFile, UTF8);
                writer.Write(buildTemplate.TransformText());
            }

            string sourceFileName = Path.Combine(outputInfo.BuildDirectory, outputInfo.InputFile);
            string outputFileName = Path.Combine(outputInfo.BuildDirectory, outputInfo.OutputFile);

            File.Copy(sourceFileName, outputFileName, true);

            Process process = new();
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

            fixChecksum(outputInfo);

            if (process.ExitCode != 0)
            {
                reporter.Error(LibResource.bassError);
            }

            File.Delete(buildFileName);

            foreach (var script in outputInfo.Scripts)
            {
                File.Delete(Path.Combine(outputInfo.BuildDirectory, Path.ChangeExtension(script.FileName, ".inc")));
            }

            foreach (var dataBank in outputInfo.DataBanks)
            {
                File.Delete(Path.Combine(outputInfo.BuildDirectory, Path.ChangeExtension(dataBank.FileName, ".inc")));
            }
        }

        private void fixChecksum(OutputInfo outputInfo)
        {
            string romFilePath = Path.Combine(outputInfo.BuildDirectory, outputInfo.OutputFile);
            byte[] rom = File.ReadAllBytes(romFilePath);

            bool isPowerOfTwo = BitOperations.PopCount((uint)rom.Length) == 1;

            long physicalChecksum = outputInfo.System.RAMToPhysical(0x00FFDC);

            Span<byte> currentChecksum = rom.AsSpan()[(int)physicalChecksum..((int)physicalChecksum + 4)];

            ushort computedChecksum = 0;

            if (isPowerOfTwo)
            {
                for (int i = 0; i < rom.Length; ++i)
                {
                    computedChecksum += rom[i];
                }
            }
            else
            {
                ushort computedChecksum2 = 0;

                uint romSize = (uint)rom.Length;

                uint alignedUpRomSize = (uint)rom.Length;
                alignedUpRomSize--;
                alignedUpRomSize |= alignedUpRomSize >> 1;
                alignedUpRomSize |= alignedUpRomSize >> 2;
                alignedUpRomSize |= alignedUpRomSize >> 4;
                alignedUpRomSize |= alignedUpRomSize >> 8;
                alignedUpRomSize |= alignedUpRomSize >> 16;
                alignedUpRomSize++;

                uint halfRomSize = alignedUpRomSize >> 1;

                int half_end = (int)((halfRomSize > romSize) ? romSize : halfRomSize);
                for (int index = 0; index < half_end; ++index)
                {
                    computedChecksum += rom[index];
                }

                int remainder = (int)(romSize - halfRomSize);
                if (remainder <= 0)
                {
                    remainder = (int)halfRomSize;
                }

                for (int index = (int)halfRomSize; index < romSize; ++index)
                {
                    computedChecksum2 += rom[index];
                }

                computedChecksum += (ushort)(computedChecksum2 * (halfRomSize / remainder));
            }

            long newChecksum = computedChecksum;
            newChecksum += (-currentChecksum[0] - currentChecksum[1] - currentChecksum[2] - currentChecksum[3]) + 2 * 0xff;

            using var fileStream = new FileStream(romFilePath, FileMode.Open, FileAccess.ReadWrite);
            fileStream.Seek(physicalChecksum, SeekOrigin.Begin);

            ushort inverse = (ushort)((newChecksum & 0xFFFF) ^ 0xFFFF);

            fileStream.WriteByte((byte)(inverse & 0xFF));
            fileStream.WriteByte((byte)((inverse >> 8) & 0xFF));
            fileStream.WriteByte((byte)(newChecksum & 0xFF));
            fileStream.WriteByte((byte)((newChecksum >> 8) & 0xFF));
        }

        private void generateScriptFile(string buildDirectory, OutputScriptBank script)
        {
            if (script.BankType == ScriptBankType.Pointer16)
            {
                SNESBassScriptIncludeTemplate scriptTemplate = new()
                {
                    ScriptBank = script
                };

                string scriptFileName = Path.Combine(buildDirectory, Path.ChangeExtension(script.FileName, ".inc"));

                using var buildFile = File.Open(scriptFileName, FileMode.Create);
                using var writer = new StreamWriter(buildFile, UTF8);

                writer.Write(scriptTemplate.TransformText());
            }
        }

        private void generateDataFile(string buildDirectory, OutputDataBank dataBank)
        {
            SNESBassDataIncludeTemplate scriptTemplate = new()
            {
                DataBank = dataBank
            };

            string scriptFileName = Path.Combine(buildDirectory, Path.ChangeExtension(dataBank.FileName, ".inc"));

            using var buildFile = File.Open(scriptFileName, FileMode.Create);
            using var writer = new StreamWriter(buildFile, UTF8);

            writer.Write(scriptTemplate.TransformText());
        }
    }
}
