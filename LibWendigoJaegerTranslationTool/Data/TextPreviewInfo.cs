namespace WendigoJaeger.TranslationTool.Data
{
    public class TextPreviewInfo : RefObject
    {
        private int _maxPerLine = 0;
        private int _maxLines = 0;
        private RefObjectPtr<FontSettings> _font;

        public int MaxPerLine
        {
            get
            {
                return _maxPerLine;
            }
            set
            {
                var oldValue = _maxPerLine;
                _maxPerLine = value;
                notifyPropertyChanged(oldValue, value);
            }
        }

        public int MaxLines
        {
            get
            {
                return _maxLines;
            }
            set
            {
                var oldValue = _maxLines;
                _maxLines = value;
                notifyPropertyChanged(oldValue, value);
            }
        }

        public RefObjectPtr<FontSettings> Font
        {
            get
            {
                if (_font == null)
                {
                    _font = new RefObjectPtr<FontSettings>();
                    _font.UndoPropertyChanged += undoProxy;
                    _font.PropertyChanged += propertyChangedProxy;
                }

                return _font;
            }
            set
            {
                _font = value;
                if (_font != null)
                {
                    _font.UndoPropertyChanged -= undoProxy;
                    _font.UndoPropertyChanged += undoProxy;

                    _font.PropertyChanged -= propertyChangedProxy;
                    _font.PropertyChanged += propertyChangedProxy;
                }
            }
        }
    }
}
