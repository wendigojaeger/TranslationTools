using WendigoJaeger.TranslationTool.Extractors;
using WendigoJaeger.TranslationTool.Undo;

namespace WendigoJaeger.TranslationTool.Data
{
    public class ScriptSettings : UndoObject
    {
        private string _name = string.Empty;
        private ExternalFile<ScriptFile> _script;
        private RefObjectPtr<TableFile> _tableFile;
        private RefObjectPtr<TextPreviewInfo> _textPreview;

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

        public uint Entries { get; set; }

        public ITextExtractor TextExtractor { get; set; }

        public RefObjectPtr<TableFile> TableFile
        {
            get
            {
                if (_tableFile == null)
                {
                    _tableFile = new RefObjectPtr<TableFile>();
                    _tableFile.UndoPropertyChanged += undoProxy;
                    _tableFile.PropertyChanged += propertyChangedProxy;
                }

                return _tableFile;
            }
            set
            {
                _tableFile = value;
                if (_tableFile != null)
                {
                    _tableFile.UndoPropertyChanged -= undoProxy;
                    _tableFile.UndoPropertyChanged += undoProxy;

                    _tableFile.PropertyChanged -= propertyChangedProxy;
                    _tableFile.PropertyChanged += propertyChangedProxy;
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
    }
}
