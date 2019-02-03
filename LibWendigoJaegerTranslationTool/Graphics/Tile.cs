using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WendigoJaeger.TranslationTool.Graphics
{
    public struct Tile
    {
        public byte[] Data;

        public static Tile Create()
        {
            return new Tile() { Data = new byte[8 * 8] };
        }
    }
}
