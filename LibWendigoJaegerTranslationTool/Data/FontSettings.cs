using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WendigoJaeger.TranslationTool.Data
{
    public class FontSettings : RefObject
    {
        private int _offset = 0;
        private int _characterWidth = 8;
        private int _characterHeight = 8;

        public int Offset
        {
            get
            {
                return _offset;
            }
            set
            {
                var oldValue = _offset;
                _offset = value;
                notifyPropertyChanged(oldValue, value);
            }
        }

        public RefObjectPtr<GraphicsSettings> Graphics { get; set; } = new RefObjectPtr<GraphicsSettings>();

        public RefObjectPtr<Palette> Palette { get; set; } = new RefObjectPtr<Palette>();

        public int CharacterWidth
        {
            get
            {
                return _characterWidth;
            }
            set
            {
                var oldValue = _characterWidth;
                _characterWidth = value;
                notifyPropertyChanged(oldValue, value);
            }
        }

        public int CharacterHeight
        {
            get
            {
                return _characterHeight;
            }
            set
            {
                var oldValue = _characterHeight;
                _characterHeight = value;
                notifyPropertyChanged(oldValue, value);
            }
        }
    }
}
