using System.Linq;
using WendigoJaeger.TranslationTool.Extractors;
using WendigoJaeger.TranslationTool.Undo;

namespace WendigoJaeger.TranslationTool.Data
{
    public class ScriptSettings : UndoObject
    {
        private string _name = string.Empty;
        private ExternalFile<ScriptFile> _script;
        private UndoObservableCollection<LocalizedFilePathEntry> _targetTableFiles;

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

        public ExternalFile<ScriptFile> Script
        {
            get
            {
                if (_script == null)
                {
                    _script = new ExternalFile<ScriptFile>();
                    _script.UndoArrayChanged += arrayProxy;
                    _script.UndoPropertyChanged += undoProxy;
                }

                return _script;
            }
            set
            {
                _script = value;

                if (_script != null)
                {
                    _script.UndoArrayChanged -= arrayProxy;
                    _script.UndoArrayChanged += arrayProxy;

                    _script.UndoPropertyChanged -= undoProxy;
                    _script.UndoPropertyChanged += undoProxy;
                }
            }
        }

        public long SourceRAMAddress { get; set; }

        public long DestinationRAMAddress { get; set; }

        public long? DestinationEndRAMAddress { get; set; }

        public byte Terminator { get; set; }

        public byte? NewLine { get; set; }

        public string SourceTableFile { get; set; }

        public uint Entries { get; set; }

        public ITextExtractor TextExtractor { get; set; }

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
    }
}
