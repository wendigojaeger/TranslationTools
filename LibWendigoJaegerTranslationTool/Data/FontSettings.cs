namespace WendigoJaeger.TranslationTool.Data
{
    public class FontSettings : RefObject
    {
        private int _offset = 0;
        private int _characterWidth = 8;
        private int _characterHeight = 8;
        private RefObjectPtr<GraphicsSettings> _graphics;
        private RefObjectPtr<Palette> _palette;

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

        public RefObjectPtr<GraphicsSettings> Graphics
        {
            get
            {
                if (_graphics == null)
                {
                    _graphics = new RefObjectPtr<GraphicsSettings>();
                    _graphics.UndoPropertyChanged += undoProxy;
                    _graphics.PropertyChanged += propertyChangedProxy;
                }

                return _graphics;
            }
            set
            {
                _graphics = value;

                if (_graphics != null)
                {
                    _graphics.UndoPropertyChanged -= undoProxy;
                    _graphics.UndoPropertyChanged += undoProxy;

                    _graphics.PropertyChanged -= propertyChangedProxy;
                    _graphics.PropertyChanged += propertyChangedProxy;
                }
            }
        }

        public RefObjectPtr<Palette> Palette
        {
            get
            {
                if (_palette == null)
                {
                    _palette = new RefObjectPtr<Palette>();
                    _palette.UndoPropertyChanged += undoProxy;
                    _palette.PropertyChanged += propertyChangedProxy;
                }

                return _palette;
            }
            set
            {
                _palette = value;
                if (_palette != null)
                {
                    _palette.UndoPropertyChanged -= undoProxy;
                    _palette.UndoPropertyChanged += undoProxy;

                    _palette.PropertyChanged -= propertyChangedProxy;
                    _palette.PropertyChanged += propertyChangedProxy;
                }
            }
        }

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
