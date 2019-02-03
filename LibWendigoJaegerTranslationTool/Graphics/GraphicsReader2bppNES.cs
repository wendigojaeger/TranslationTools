using Newtonsoft.Json;
using System.IO;

namespace WendigoJaeger.TranslationTool.Graphics
{
    class GraphicsReader2bppNES : IGraphicsReader
    {
        [JsonIgnore]
        public int BytesPerTile => 16;

        [JsonIgnore]
        public string Name => LibResource.gfx2bppNES;

        public Tile Read(Stream stream)
        {
            Tile result = Tile.Create();

            byte[] data = new byte[BytesPerTile];
            stream.Read(data, 0, data.Length);

            for (int i = 0; i < 8; ++i)
            {
                for (int bit = 0; bit < 8; ++bit)
                {
                    int bitplane1 = (data[i] & (1 << (7 - bit))) >> (7 -bit);
                    int bitplane2 = (data[i + 8] & (1 << (7 - bit))) >> (7 - bit);

                    result.Data[(i * 8) + bit] = (byte)((bitplane2 << 1) | bitplane1);
                }
            }

            return result;
        }
    }
}
