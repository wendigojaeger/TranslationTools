using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WendigoJaeger.TranslationTool.Graphics;

namespace WendigoJaeger.TranslationTool.Converters
{
    public static class TileGraphicsConverter
    {
        public static WriteableBitmap ConvertToWpfBitmap(string imageFile, IGraphicsReader converter, Color[] palette)
        {
            WriteableBitmap result = null;

            using (FileStream file = File.OpenRead(imageFile))
            {
                var fileSize = file.Length;

                int tileCount = (int)fileSize / converter.BytesPerTile;

                int tileWidth = 16;
                int tileHeight = tileCount / 16;

                result = BitmapFactory.New(tileWidth * 8, tileHeight * 8);
                result.Lock();

                for (int tile = 0; tile < tileCount; tile++)
                {
                    int tileX = tile % 16;
                    int tileY = tile / 16;

                    var tileData = converter.Read(file);

                    for (int y = 0; y < 8; ++y)
                    {
                        int finalY = tileY * 8 + y;

                        int tileDataStride = y * 8;

                        for (int x = 0; x < 8; ++x)
                        {
                            int finalX = tileX * 8 + x;

                            int paletteIndex = tileData.Data[tileDataStride + x];

                            if (finalX < result.Width && finalY < result.Height && paletteIndex < palette.Length)
                            {
                                result.SetPixel(finalX, finalY, palette[paletteIndex]);
                            }
                        }
                    }
                }

                result.Unlock();
            }

            return result;
        }
    }
}
