using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using WendigoJaeger.TranslationTool.Data;

namespace WendigoJaeger.TranslationTool.Extractors
{
    class ScriptDictionaryExtractor24BitLittleEndian : IScriptDictionaryExtractor
    {
        [JsonIgnore]
        public string Name => LibResource.scriptDictionaryExtractor24BitLittleEndian;

        public void Extract(Project project, ScriptDictionary scriptDictionary)
        {
            scriptDictionary.Scripts.Clear();

            string romPath = Path.Combine(ConfigSerializer.RootDirectory, project.InputFile);

            using FileStream romFile = File.OpenRead(romPath);
            using BinaryReader reader = new BinaryReader(romFile);

            project.System.Origin = scriptDictionary.RAMAddress;

            long physicalPointerAddress = project.System.RAMToPhysical(scriptDictionary.RAMAddress);

            reader.BaseStream.Seek(physicalPointerAddress, SeekOrigin.Begin);

            for (int entry = 0; entry < scriptDictionary.Entries; ++entry)
            {
                byte[] longAddressBytes = reader.ReadBytes(3);
                long scriptRamAddress = longAddressBytes[0] | (longAddressBytes[1] << 8) | (longAddressBytes[2] << 16);

                ScriptSettings newScript = new()
                {
                    Name = scriptRamAddress > 0 ? $"{scriptDictionary.Name}_{entry:x2}" : $"{scriptDictionary.Name}_Dummy_{entry:x2}",
                    SourceRAMAddress = scriptRamAddress,
                    DestinationRAMAddress = scriptRamAddress,
                    ScriptExtractor = scriptRamAddress > 0 ? new ScriptExtractorPointer16LittleEndian() : null
                };
                newScript.TableFile.Instance = ReferenceDatabase.Instance.ListAll<TableFile>().FirstOrDefault();

                newScript.ScriptFile.Path = $"{newScript.Name}.wtd";

                scriptDictionary.Scripts.Add(newScript);
            }

            foreach (ScriptSettings script in scriptDictionary.Scripts)
            {
                if (script != null)
                {
                    long scriptPhysicalAddress = project.System.RAMToPhysical(script.SourceRAMAddress);
                    romFile.Seek(scriptPhysicalAddress, SeekOrigin.Begin);

                    project.System.Origin = script.SourceRAMAddress;

                    uint entriesCount = 1;

                    byte[] shortAddressBytes = reader.ReadBytes(2);

                    long ramPointerAddress = shortAddressBytes[0] | (shortAddressBytes[1] << 8);
                    long dataRamAddress = project.System.AbsoluteRAMAddress(ramPointerAddress);

                    long currentPosition = project.System.PhysicalToRAM(romFile.Position);

                    while (currentPosition < dataRamAddress)
                    {
                        ++entriesCount;

                        shortAddressBytes = reader.ReadBytes(2);

                        ramPointerAddress = shortAddressBytes[0] | (shortAddressBytes[1] << 8);
                        long newDataRamAddress = project.System.AbsoluteRAMAddress(ramPointerAddress);

                        dataRamAddress = Math.Min(dataRamAddress, newDataRamAddress);

                        currentPosition = project.System.PhysicalToRAM(romFile.Position);
                    }

                    script.Entries = entriesCount;
                }
            }
        }
    }
}
