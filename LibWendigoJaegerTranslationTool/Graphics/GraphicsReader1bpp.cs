using Newtonsoft.Json;
using System.IO;

namespace WendigoJaeger.TranslationTool.Graphics
{
    class GraphicsReader1bpp : IGraphicsReader
    {
        [JsonIgnore]
        public int BytesPerTile => 8;

        [JsonIgnore]
        public string Name => LibResource.gfx1bpp;

        public Tile Read(Stream stream)
        {
            Tile result = Tile.Create();

            byte[] data = new byte[BytesPerTile];
            stream.Read(data, 0, data.Length);

            for(int i=0; i<BytesPerTile; ++i)
            {
                for (int bit = 0; bit < 8; ++bit)
                {
                    result.Data[(i * 8) + bit] = (byte)((data[i] & (1 << (7 - bit))) >> (7 - bit));
                }
            }

            return result;
        }
    }
}
