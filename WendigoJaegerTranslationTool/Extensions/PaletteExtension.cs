using System.Windows.Media;
using WendigoJaeger.TranslationTool.Data;

namespace WendigoJaeger.TranslationTool.Extensions
{
    public static class PaletteColorExtension
    {
        public static Color ToWpfColor(this PaletteColor value)
        {
            return Color.FromArgb(255, value.R, value.G, value.B);
        }
    }

    public static class PaletteExtension
    {
        public static Color[] ToWpfColorArray(this Palette value)
        {
            Color[] result = new Color[value.Entries.Count];
            for(int i=0; i<result.Length; ++i)
            {
                result[i] = value.Entries[i].ToWpfColor();
            }

            return result;
        }
    }
}
