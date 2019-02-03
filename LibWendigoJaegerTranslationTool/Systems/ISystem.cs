using Newtonsoft.Json;

namespace WendigoJaeger.TranslationTool.Systems
{
    public interface ISystem
    {
        [JsonIgnore]
        Endian Endianess { get; }

        [JsonIgnore]
        long Origin { set; }

        long RAMToPhysical(long address);
        long PhysicalToRAM(long address);

        long AbsoluteRAMAddress(long relative);
    }
}
