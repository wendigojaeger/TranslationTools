using Newtonsoft.Json;
using System.ComponentModel;

namespace WendigoJaeger.TranslationTool.Systems
{
    [DisplayName("SNES LoROM (SlowROM)")]
    public class SNESLoRomSlowRom : ISystem
    {
        [JsonIgnore]
        public Endian Endianess { get { return Endian.Little; } }

        [JsonIgnore]
        public long Origin { get; set; }

        public long AbsoluteRAMAddress(long relative)
        {
            return (Origin & 0xFF0000) | (relative & 0x00FFFF);
        }

        public long PhysicalToRAM(long address)
        {
            return ((address & 0xFF8000) << 1)
                | (address & 0xFFFF)
                | 0x8000
                ;
        }

        public long RAMToPhysical(long address)
        {
            return ((address & 0x7F0000) >> 1) | (address & 0x7FFF);
        }
    }

    [DisplayName("SNES LoROM (FastROM)")]
    public class SNESLoRomFastRom : ISystem
    {
        [JsonIgnore]
        public Endian Endianess { get { return Endian.Little; } }

        [JsonIgnore]
        public long Origin { get; set; }

        public long AbsoluteRAMAddress(long relative)
        {
            return (Origin & 0xFF0000) | (relative & 0x00FFFF);
        }

        public long PhysicalToRAM(long address)
        {
            return (0x80 << 16)
                | ((address & 0xFF8000) << 1)
                | (address & 0xFFFF)
                | 0x8000
                ;
        }

        public long RAMToPhysical(long address)
        {
            return ((address & 0x7F0000) >> 1) | (address & 0x7FFF);
        }
    }

    [DisplayName("SNES HiROM")]
    public class SNESHiROM : ISystem
    {
        [JsonIgnore]
        public Endian Endianess { get { return Endian.Little; } }

        [JsonIgnore]
        public long Origin { get; set; }

        public long AbsoluteRAMAddress(long relative)
        {
            return (Origin & 0xFF0000) | (relative & 0x00FFFF);
        }

        public long PhysicalToRAM(long address)
        {
            return (0xC0 << 16) | (address & 0xFFFFFF);
        }

        public long RAMToPhysical(long address)
        {
            return address & 0x3FFFFF;
        }
    }
}
