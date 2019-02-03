using Newtonsoft.Json;
using System.IO;

namespace WendigoJaeger.TranslationTool.Graphics
{
    class GraphicsReader8bppSNES : IGraphicsReader
    {
        [JsonIgnore]
        public int BytesPerTile => 64;

        [JsonIgnore]
        public string Name => LibResource.gfx8bppSNES;

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
                    int bitplane3 = (data[2 * i + 16] & (1 << (7 - bit))) >> (7 - bit);
                    int bitplane4 = (data[2 * i + 17] & (1 << (7 - bit))) >> (7 - bit);

                    int bitplane5 = (data[2 * i + 32] & (1 << (7 - bit))) >> (7 - bit);
                    int bitplane6 = (data[2 * i + 33] & (1 << (7 - bit))) >> (7 - bit);
                    int bitplane7 = (data[2 * i + 48] & (1 << (7 - bit))) >> (7 - bit);
                    int bitplane8 = (data[2 * i + 49] & (1 << (7 - bit))) >> (7 - bit);

                    result.Data[(i * 8) + bit] = (byte)(
                        (bitplane8 << 7)
                        | (bitplane7 << 6)
                        | (bitplane6 << 5)
                        | (bitplane5 << 4)
                        | (bitplane4 << 3)
                        | (bitplane3 << 2)
                        | (bitplane2 << 1)
                        | bitplane1
                    );
                }
            }

            return result;
        }
    }
}
