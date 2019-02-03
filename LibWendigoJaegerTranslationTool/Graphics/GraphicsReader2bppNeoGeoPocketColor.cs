using Newtonsoft.Json;
using System.IO;

namespace WendigoJaeger.TranslationTool.Graphics
{
    class GraphicsReader2bppNeoGeoPocketColor : IGraphicsReader
    {
        [JsonIgnore]
        public int BytesPerTile => 16;

        [JsonIgnore]
        public string Name => LibResource.gfx2bppNeoGeoPocketColor;

        public Tile Read(Stream stream)
        {
            Tile result = Tile.Create();

            byte[] data = new byte[BytesPerTile];
            stream.Read(data, 0, data.Length);

            for (int i = 0; i < 8; ++i)
            {
                int pixelData = data[2 * i] | (data[2 * i + 1] << 8);

                for (int column = 0; column < 8; ++column)
                {
                    int bitshift = (16 - ((column + 1) * 2));
                    result.Data[(i * 8) + column] = (byte)(pixelData & (0x3 << bitshift) >> bitshift);
                }
            }

            return result;
        }
    }
}
