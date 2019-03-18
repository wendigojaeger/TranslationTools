using WendigoJaeger.TranslationTool.Undo;

namespace WendigoJaeger.TranslationTool.Data
{
    public class PaletteColor : UndoObject
    {
        private byte _r;
        private byte _g;
        private byte _b;

        public byte R
        {
            get
            {
                return _r;
            }
            set
            {
                var oldValue = _r;
                _r = value;
                notifyPropertyChanged(oldValue, value);
            }
        }

        public byte G
        {
            get
            {
                return _g;
            }
            set
            {
                var oldValue = _g;
                _g = value;
                notifyPropertyChanged(oldValue, value);
            }
        }

        public byte B
        {
            get
            {
                return _b;
            }
            set
            {
                var oldValue = _b;
                _b = value;
                notifyPropertyChanged(oldValue, value);
            }
        }
    };

    public class Palette : RefObject
    {
        private UndoObservableCollection<PaletteColor> _entries = null;

        public UndoObservableCollection<PaletteColor> Entries
        {
            get
            {
                if (_entries == null)
                {
                    _entries = new UndoObservableCollection<PaletteColor>();
                    _entries.UndoArrayChanged += arrayProxy;
                    _entries.UndoPropertyChanged += undoProxy;
                }

                return _entries;
            }
            set
            {
                _entries = value;

                if (_entries != null)
                {
                    _entries.UndoArrayChanged -= arrayProxy;
                    _entries.UndoArrayChanged += arrayProxy;

                    _entries.UndoPropertyChanged -= undoProxy;
                    _entries.UndoPropertyChanged += undoProxy;
                }
            }
        }
    }
}
 