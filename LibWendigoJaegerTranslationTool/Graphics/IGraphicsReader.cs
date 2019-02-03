using System.IO;

namespace WendigoJaeger.TranslationTool.Graphics
{
    public interface IGraphicsReader
    {
        int BytesPerTile { get; }
        string Name { get; }
        Tile Read(Stream stream);
    }
}
