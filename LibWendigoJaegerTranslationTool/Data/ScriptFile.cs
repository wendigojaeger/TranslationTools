using System.Linq;
using WendigoJaeger.TranslationTool.Undo;

namespace WendigoJaeger.TranslationTool.Data
{
    public class ScriptEntry : RefObject
    {
        private string _entryName = string.Empty;
        private string _original = string.Empty;
        private string _comment = string.Empty;
        private UndoObservableCollection<TranslationEntry> _translations;
        private RefObjectPtr<TextPreviewInfo> _textPreview;

        public string EntryName
        {
            get
            {
                return _entryName;
            }
            set
            {
                var oldValue = _entryName;
                _entryName = value;
                notifyPropertyChanged(oldValue, value);
            }
        }

        public string Original
        {
            get
            {
                return _original;
            }
            set
            {
                var oldValue = _original;
                _original = value;
                notifyPropertyChanged(oldValue, value);
            }
        }

        public string Comment
        {
            get
            {
                return _comment;
            }
            set
            {
                var oldValue = _comment;
                _comment = value.Replace("\r", "");
                notifyPropertyChanged(oldValue, _comment);
            }
        }

        public UndoObservableCollection<TranslationEntry> Translations
        {
            get
            {
                if (_translations == null)
                {
                    _translations = new UndoObservableCollection<TranslationEntry>();
                    _translations.UndoArrayChanged += arrayProxy;
                    _translations.UndoPropertyChanged += undoProxy;
                }

                return _translations;
            }
            set
            {
                _translations = value;

                if (_translations != null)
                {
                    _translations.UndoArrayChanged -= arrayProxy;
                    _translations.UndoArrayChanged += arrayProxy;

                    _translations.UndoPropertyChanged -= undoProxy;
                    _translations.UndoPropertyChanged += undoProxy;
                }
            }
        }

        public RefObjectPtr<TextPreviewInfo> TextPreview
        {
            get
            {
                if (_textPreview == null)
                {
                    _textPreview = new RefObjectPtr<TextPreviewInfo>();
                    _textPreview.UndoPropertyChanged += undoProxy;
                    _textPreview.PropertyChanged += propertyChangedProxy;
                }

                return _textPreview;
            }
            set
            {
                _textPreview = value;
                if (_textPreview != null)
                {
                    _textPreview.UndoPropertyChanged -= undoProxy;
                    _textPreview.UndoPropertyChanged += undoProxy;

                    _textPreview.PropertyChanged -= propertyChangedProxy;
                    _textPreview.PropertyChanged += propertyChangedProxy;
                }
            }
        }

        public bool HasTranslation(string lang)
        {
            return Translations.Any(x => x.Lang == lang);
        }

        public TranslationEntry GetTranslation(string lang)
        {
            return Translations.FirstOrDefault(x => x.Lang == lang);
        }

        public string this[string key]
        {
            get
            {
                var entry = GetTranslation(key);
                if (entry != null)
                {
                    return entry.Value;
                }

                return string.Empty;
            }
            set
            {
                var entry = GetTranslation(key);
                if (entry != null)
                {
                    entry.Value = value;
                }
                else
                {
                    Translations.Add(new TranslationEntry { Lang = key, Value = value });
                }
            }
        }
    }

    public class ScriptPointer : UndoObject
    {
        private int _pointerIndex;
        private RefObjectPtr<ScriptEntry> _pointer;

        public int PointerIndex
        {
            get
            {
                return _pointerIndex;
            }
            set
            {
                var oldValue = _pointerIndex;
                _pointerIndex = value;
                notifyPropertyChanged(oldValue, value);
            }
        }

        public RefObjectPtr<ScriptEntry> Pointer
        {
            get
            {
                if (_pointer == null)
                {
                    _pointer = new();
                    _pointer.UndoPropertyChanged += undoProxy;
                    _pointer.PropertyChanged -= propertyChangedProxy;
                }

                return _pointer;
            }
            set
            {
                if (_pointer != null)
                {
                    _pointer.UndoPropertyChanged -= undoProxy;
                    _pointer.PropertyChanged -= propertyChangedProxy;
                }

                _pointer = value;

                if (_pointer != null)
                {
                    _pointer.UndoPropertyChanged += undoProxy;
                    _pointer.PropertyChanged += propertyChangedProxy;
                }
            }
        }
    }

    public class ScriptFile : UndoObject
    {
        private UndoObservableCollection<ScriptEntry> _entries;
        private UndoObservableCollection<ScriptPointer> _pointers;

        public UndoObservableCollection<ScriptPointer> Pointers
        {
            get
            {
                if (_pointers == null)
                {
                    _pointers = new();
                    _pointers.UndoArrayChanged += arrayProxy;
                    _pointers.UndoPropertyChanged += undoProxy;
                }

                return _pointers;
            }
            set
            {
                _pointers = value;

                if (_pointers != null)
                {
                    _pointers.UndoArrayChanged -= arrayProxy;
                    _pointers.UndoArrayChanged += arrayProxy;

                    _pointers.UndoPropertyChanged -= undoProxy;
                    _pointers.UndoPropertyChanged += undoProxy;
                }
            }
        }

        public UndoObservableCollection<ScriptEntry> Entries
        {
            get
            {
                if (_entries == null)
                {
                    _entries = new();
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

        public void Clear()
        {
            Pointers.Clear();
            Entries.Clear();
        }
    }
}
