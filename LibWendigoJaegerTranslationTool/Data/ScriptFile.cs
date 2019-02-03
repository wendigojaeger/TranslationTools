using System.Linq;
using WendigoJaeger.TranslationTool.Undo;

namespace WendigoJaeger.TranslationTool.Data
{
    public enum ScriptEntryState
    {
        ToTranslate,
        InProgress,
        Review,
        Final
    }

    public class TranslationEntry : UndoObject
    {
        private string _lang;
        private string _value;
        private ScriptEntryState _state;

        public string Lang
        {
            get
            {
                return _lang;
            }
            set
            {
                var oldValue = _lang;
                _lang = value;
                notifyPropertyChanged(oldValue, value);
            }
        }

        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                var oldValue = _value;
                _value = value.Replace("\r", "");
                notifyPropertyChanged(oldValue, _value);
            }
        }

        public ScriptEntryState State
        {
            get
            {
                return _state;
            }
            set
            {
                var oldValue = _state;
                _state = value;
                notifyPropertyChanged(oldValue, value);
            }
        }
    }

    public class ScriptEntry : UndoObject
    {
        private string _entryName = string.Empty;
        private string _original = string.Empty;
        private string _comment = string.Empty;
        
        private UndoObservableCollection<TranslationEntry> _translations;

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
                    _translations.UndoPropertyChanged += undoProxy;
                }

                return _translations;
            }
            set
            {
                _translations = value;

                if (_translations != null)
                {
                    _translations.UndoPropertyChanged -= undoProxy;
                    _translations.UndoPropertyChanged += undoProxy;
                }
            }
        }

        public bool HasTranslation(string lang)
        {
            return Translations.Count(x => x.Lang == lang) > 0;
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

    public class ScriptFile : UndoObject
    {
        private UndoObservableCollection<ScriptEntry> _entries;

        public UndoObservableCollection<ScriptEntry> Entries
        {
            get
            {
                if (_entries == null)
                {
                    _entries = new UndoObservableCollection<ScriptEntry>();
                    _entries.UndoPropertyChanged += undoProxy;
                }

                return _entries;
            }
            set
            {
                _entries = value;

                if (_entries != null)
                {
                    _entries.UndoPropertyChanged -= undoProxy;
                    _entries.UndoPropertyChanged += undoProxy;
                }
            }
        }
    }
}
