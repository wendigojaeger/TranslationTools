using System.Linq;
using WendigoJaeger.TranslationTool.Undo;

namespace WendigoJaeger.TranslationTool.Data
{
    public class TableFile : RefObject
    {
        private byte _terminator;
        private byte? _newLine;
        private byte? _nextWindow;
        private string _sourceTableFile;
        private UndoObservableCollection<LocalizedFilePathEntry> _targetTableFiles;

        public byte Terminator
        {
            get
            {
                return _terminator;
            }
            set
            {
                var oldValue = _terminator;
                _terminator = value;
                notifyPropertyChanged(oldValue, value);
            }
        }

        public byte? NewLine
        {
            get
            {
                return _newLine;
            }
            set
            {
                var oldValue = _newLine;
                _newLine = value;
                notifyPropertyChanged(oldValue, value);
            }
        }

        public byte? NextWindow
        {
            get
            {
                return _nextWindow;
            }
            set
            {
                var oldValue = _nextWindow;
                _nextWindow = value;
                notifyPropertyChanged(oldValue, value);
            }
        }

        public string SourceTableFile
        {
            get
            {
                return _sourceTableFile;
            }
            set
            {
                var oldValue = _sourceTableFile;
                _sourceTableFile = value;
                notifyPropertyChanged(oldValue, value);
            }
        }

        public UndoObservableCollection<LocalizedFilePathEntry> TargetTableFiles
        {
            get
            {
                if (_targetTableFiles == null)
                {
                    _targetTableFiles = new UndoObservableCollection<LocalizedFilePathEntry>();
                    _targetTableFiles.UndoArrayChanged += arrayProxy;
                    _targetTableFiles.UndoPropertyChanged += undoProxy;
                }

                return _targetTableFiles;
            }
            set
            {
                _targetTableFiles = value;

                if (_targetTableFiles != null)
                {
                    _targetTableFiles.UndoArrayChanged -= arrayProxy;
                    _targetTableFiles.UndoArrayChanged += arrayProxy;

                    _targetTableFiles.UndoPropertyChanged -= undoProxy;
                    _targetTableFiles.UndoPropertyChanged += undoProxy;
                }
            }
        }

        public bool HasEntry(string lang)
        {
            return TargetTableFiles.Count(x => x.Lang == lang) > 0;
        }

        public LocalizedFilePathEntry GetTargetTable(string lang)
        {
            return TargetTableFiles.FirstOrDefault(x => x.Lang == lang);
        }

        public string GetTablePath(string lang)
        {
            var entry = GetTargetTable(lang);
            if (entry != null)
            {
                return entry.Path;
            }

            return SourceTableFile;
        }

        public string this[string key]
        {
            get
            {
                var entry = GetTargetTable(key);
                if (entry != null)
                {
                    return entry.Path;
                }

                return string.Empty;
            }
            set
            {
                var entry = GetTargetTable(key);
                if (entry != null)
                {
                    entry.Path = value;
                }
                else
                {
                    TargetTableFiles.Add(new LocalizedFilePathEntry { Lang = key, Path = value });
                }
            }
        }

        public void SyncLanguages(Project project)
        {
            foreach (var langEntry in project.Lang)
            {
                if (GetTargetTable(langEntry.Key) == null)
                {
                    TargetTableFiles.Add(new LocalizedFilePathEntry { Lang = langEntry.Key });
                }
            }
        }
    }
}
