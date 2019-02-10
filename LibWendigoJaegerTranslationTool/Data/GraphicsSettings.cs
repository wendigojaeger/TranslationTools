using System.Linq;
using WendigoJaeger.TranslationTool.Graphics;
using WendigoJaeger.TranslationTool.Undo;

namespace WendigoJaeger.TranslationTool.Data
{
    public class GraphicsSettings : UndoObject
    {
        private string _name;
        private UndoObservableCollection<LocalizedFilePathEntry> _entries;
        private IGraphicsReader _graphicsReader;
        private long _ramAddress;

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                var oldValue = _name;
                _name = value;
                notifyPropertyChanged(oldValue, value);
            }
        }

        public long RAMAddress
        {
            get
            {
                return _ramAddress;
            }
            set
            {
                var oldValue = _ramAddress;
                _ramAddress = value;
                notifyPropertyChanged(oldValue, value);
            }
        }

        public IGraphicsReader GraphicsReader
        {
            get
            {
                return _graphicsReader;
            }
            set
            {
                var oldValue = _graphicsReader;
                _graphicsReader = value;
                notifyPropertyChanged(oldValue, value);
            }
        }

        public UndoObservableCollection<LocalizedFilePathEntry> Entries
        {
            get
            {
                if (_entries == null)
                {
                    _entries = new UndoObservableCollection<LocalizedFilePathEntry>();
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

        public bool HasEntry(string lang)
        {
            return Entries.Count(x => x.Lang == lang) > 0;
        }

        public LocalizedFilePathEntry GetEntry(string lang)
        {
            return Entries.FirstOrDefault(x => x.Lang == lang);
        }

        public string this[string key]
        {
            get
            {
                var entry = GetEntry(key);
                if (entry != null)
                {
                    return entry.Path;
                }

                return string.Empty;
            }
            set
            {
                var entry = GetEntry(key);
                if (entry != null)
                {
                    entry.Path = value;
                }
                else
                {
                    Entries.Add(new LocalizedFilePathEntry { Lang = key, Path = value });
                }
            }
        }
    }
}
