using Newtonsoft.Json;
using System.IO;

namespace WendigoJaeger.TranslationTool.Graphics
{
    class GraphicsReader3bppSNES : IGraphicsReader
    {
        [JsonIgnore]
        public int BytesPerTile => 24;

        [JsonIgnore]
        public string Name => LibResource.gfx3bppSNES;

        public Tile Read(Stream stream)
        {
            Tile result = Tile.Create();

            byte[] data = new byte[BytesPerTile];
            stream.Read(data, 0, data.Length);

            for (int i = 0; i < 8; ++i)
            {
                for (int bit = 0; bit < 8; ++bit)
                {
                    int bitplane1 = (data[2 * i] & (1 << (7 - bit))) >> (7 - bit);
                    int bitplane2 = (data[2 * i + 1] & (1 << (7 - bit))) >> (7 - bit);
                    int bitplane3 = (data[16 + i] & (1 << (7 - bit))) >> (7 - bit);

                    result.Data[(i * 8) + bit] = (byte)((bitplane3 << 2) | (bitplane2 << 1) | bitplane1);
                }
            }

            return result;
        }
    }
}
