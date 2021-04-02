using WendigoJaeger.TranslationTool.Extractors;
using WendigoJaeger.TranslationTool.Undo;

namespace WendigoJaeger.TranslationTool.Data
{
    public class DataSettings  : UndoObject
    {
        private string _name = string.Empty;
        private ExternalFile<DataFile> _dataFile;
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

        public ExternalFile<DataFile> DataFile
        {
            get
            {
                if (_dataFile == null)
                {
                    _dataFile = new ExternalFile<DataFile>();
                    _dataFile.UndoArrayChanged += arrayProxy;
                    _dataFile.UndoPropertyChanged += undoProxy;
                }

                return _dataFile;
            }
            set
            {
                _dataFile = value;

                if (_dataFile != null)
                {
                    _dataFile.UndoArrayChanged -= arrayProxy;
                    _dataFile.UndoArrayChanged += arrayProxy;

                    _dataFile.UndoPropertyChanged -= undoProxy;
                    _dataFile.UndoPropertyChanged += undoProxy;
                }
            }
        }

        public long SourceRAMAddress { get; set; }

        public long DestinationRAMAddress { get; set; }

        public long? DestinationEndRAMAddress { get; set; }

        public uint Entries { get; set; }

        public IDataExtractor DataExtractor { get; set; }

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
