using WendigoJaeger.TranslationTool.Extractors;
using WendigoJaeger.TranslationTool.Undo;

namespace WendigoJaeger.TranslationTool.Data
{
    public class ScriptDictionary : RefObject
    {
        private long _ramAddress;
        private uint _entries;
        private IScriptDictionaryExtractor _extractor;
        private UndoObservableCollection<ScriptSettings> _scripts;

        public long RAMAddress
        {
            get
            {
                return _ramAddress;
            }
            set
            {
                var oldValue = value;
                _ramAddress = value;
                notifyPropertyChanged(oldValue, value);
            }
        }

        public uint Entries
        {
            get
            {
                return _entries;
            }
            set
            {
                var oldValue = value;
                _entries = value;
                notifyPropertyChanged(oldValue, value);
            }
        }

        public IScriptDictionaryExtractor Extractor
        {
            get
            {
                return _extractor;
            }
            set
            {
                var oldValue = value;
                _extractor = value;
                notifyPropertyChanged(oldValue, value);
            }
        }

        public UndoObservableCollection<ScriptSettings> Scripts
        {
            get
            {
                if (_scripts == null)
                {
                    _scripts = new();
                    _scripts.UndoArrayChanged += arrayProxy;
                    _scripts.UndoPropertyChanged += undoProxy;
                }

                return _scripts;
            }
            set
            {
                _scripts = value;

                if (_scripts != null)
                {
                    _scripts.UndoArrayChanged -= arrayProxy;
                    _scripts.UndoArrayChanged += arrayProxy;

                    _scripts.UndoPropertyChanged -= undoProxy;
                    _scripts.UndoPropertyChanged += undoProxy;
                }
            }
        }
    }
}
