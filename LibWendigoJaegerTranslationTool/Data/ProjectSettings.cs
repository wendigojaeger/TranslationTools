using Newtonsoft.Json;
using System.Collections.Generic;
using WendigoJaeger.TranslationTool.Outputs;
using WendigoJaeger.TranslationTool.Systems;
using WendigoJaeger.TranslationTool.Undo;

namespace WendigoJaeger.TranslationTool.Data
{
    public class LocalizedProjectSettings
    {
        public string OutputFile { get; set; }
    }

    public class Project : UndoObject
    {
        private string _name = string.Empty;
        private string _inputFile = string.Empty;

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

        public string InputFile
        {
            get
            {
                return _inputFile;
            }
            set
            {
                var oldValue = _inputFile;
                _inputFile = value;
                notifyPropertyChanged(oldValue, value);
            }
        }

        public ISystem System { get; set; }

        public OutputGenerator OutputGenerator { get; set; }

        public Dictionary<string, LocalizedProjectSettings> Lang { get; set; } = new Dictionary<string, LocalizedProjectSettings>();
    }

    public class ProjectSettings : UndoObject
    {
        private Project _project;
        private UndoObservableCollection<ScriptSettings> _scripts;
        private UndoObservableCollection<GraphicsSettings> _graphics;
        private UndoObservableCollection<AssemblyFileSettings> _assemblyFileSettings;
        private UndoObservableCollection<FontSettings> _fontSettings;
        private UndoObservableCollection<Palette> _palettes;
        private UndoObservableCollection<TableFile> _tableFiles;
        private UndoObservableCollection<TextPreviewInfo> _textPreviewInfos;
        private UndoObservableCollection<DataSettings> _dataSettings;

        public Project Project
        {
            get
            {
                if (_project == null)
                {
                    _project = new Project();
                    _project.UndoArrayChanged += arrayProxy;
                    _project.UndoPropertyChanged += undoProxy;
                    _project.PropertyChanged += propertyChangedProxy;
                }

                return _project;
            }
            set
            {
                _project = value;

                if (_project != null)
                {
                    _project.UndoArrayChanged -= arrayProxy;
                    _project.UndoArrayChanged += arrayProxy;

                    _project.UndoPropertyChanged -= undoProxy;
                    _project.UndoPropertyChanged += undoProxy;

                    _project.PropertyChanged -= propertyChangedProxy;
                    _project.PropertyChanged += propertyChangedProxy;
                }
            }
        }

        public UndoObservableCollection<ScriptSettings> Scripts
        {
            get
            {
                if (_scripts == null)
                {
                    _scripts = new UndoObservableCollection<ScriptSettings>();
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

        public UndoObservableCollection<GraphicsSettings> Graphics
        {
            get
            {
                if (_graphics == null)
                {
                    _graphics = new UndoObservableCollection<GraphicsSettings>();
                    _graphics.UndoArrayChanged += arrayProxy;
                    _graphics.UndoPropertyChanged += undoProxy;
                }

                return _graphics;
            }
            set
            {
                _graphics = value;

                if (_graphics != null)
                {
                    _graphics.UndoArrayChanged -= arrayProxy;
                    _graphics.UndoArrayChanged += arrayProxy;

                    _graphics.UndoPropertyChanged -= undoProxy;
                    _graphics.UndoPropertyChanged += undoProxy;
                }
            }
        }

        public UndoObservableCollection<AssemblyFileSettings> AssemblyFileSettings
        {
            get
            {
                if (_assemblyFileSettings == null)
                {
                    _assemblyFileSettings = new UndoObservableCollection<AssemblyFileSettings>();
                    _assemblyFileSettings.UndoArrayChanged += arrayProxy;
                    _assemblyFileSettings.UndoPropertyChanged += undoProxy;
                }

                return _assemblyFileSettings;
            }
            set
            {
                _assemblyFileSettings = value;

                if (_assemblyFileSettings != null)
                {
                    _assemblyFileSettings.UndoArrayChanged -= arrayProxy;
                    _assemblyFileSettings.UndoArrayChanged += arrayProxy;

                    _assemblyFileSettings.UndoPropertyChanged -= undoProxy;
                    _assemblyFileSettings.UndoPropertyChanged += undoProxy;
                }
            }
        }

        public UndoObservableCollection<FontSettings> Fonts
        {
            get
            {
                if (_fontSettings == null)
                {
                    _fontSettings = new UndoObservableCollection<FontSettings>();
                    _fontSettings.UndoArrayChanged += arrayProxy;
                    _fontSettings.UndoPropertyChanged += undoProxy;
                }

                return _fontSettings;
            }
            set
            {
                _fontSettings = value;

                if (_fontSettings != null)
                {
                    _fontSettings.UndoArrayChanged -= arrayProxy;
                    _fontSettings.UndoArrayChanged += arrayProxy;

                    _fontSettings.UndoPropertyChanged -= undoProxy;
                    _fontSettings.UndoPropertyChanged += undoProxy;
                }
            }
        }

        public UndoObservableCollection<Palette> Palettes
        {
            get
            {
                if (_palettes == null)
                {
                    _palettes = new UndoObservableCollection<Palette>();
                    _palettes.UndoArrayChanged += arrayProxy;
                    _palettes.UndoPropertyChanged += undoProxy;
                }

                return _palettes;
            }
            set
            {
                _palettes = value;

                if (_palettes != null)
                {
                    _palettes.UndoArrayChanged -= arrayProxy;
                    _palettes.UndoArrayChanged += arrayProxy;

                    _palettes.UndoPropertyChanged -= undoProxy;
                    _palettes.UndoPropertyChanged += undoProxy;
                }
            }
        }

        public UndoObservableCollection<TableFile> TableFiles
        {
            get
            {
                if (_tableFiles == null)
                {
                    _tableFiles = new UndoObservableCollection<TableFile>();
                    _tableFiles.UndoArrayChanged += arrayProxy;
                    _tableFiles.UndoPropertyChanged += undoProxy;
                }

                return _tableFiles;
            }
            set
            {
                _tableFiles = value;

                if (_tableFiles != null)
                {
                    _tableFiles.UndoArrayChanged -= arrayProxy;
                    _tableFiles.UndoArrayChanged += arrayProxy;

                    _tableFiles.UndoPropertyChanged -= undoProxy;
                    _tableFiles.UndoPropertyChanged += undoProxy;
                }
            }
        }

        public UndoObservableCollection<TextPreviewInfo> TextPreviewInfos
        {
            get
            {
                if (_textPreviewInfos == null)
                {
                    _textPreviewInfos = new UndoObservableCollection<TextPreviewInfo>();
                    _textPreviewInfos.UndoArrayChanged += arrayProxy;
                    _textPreviewInfos.UndoPropertyChanged += undoProxy;
                }

                return _textPreviewInfos;
            }
            set
            {
                _textPreviewInfos = value;

                if (_textPreviewInfos != null)
                {
                    _textPreviewInfos.UndoArrayChanged -= arrayProxy;
                    _textPreviewInfos.UndoArrayChanged += arrayProxy;

                    _textPreviewInfos.UndoPropertyChanged -= undoProxy;
                    _textPreviewInfos.UndoPropertyChanged += undoProxy;
                }
            }
        }

        public UndoObservableCollection<DataSettings> DataSettings
        {
            get
            {
                if (_dataSettings == null)
                {
                    _dataSettings = new UndoObservableCollection<DataSettings>();
                    _dataSettings.UndoArrayChanged += arrayProxy;
                    _dataSettings.UndoPropertyChanged += undoProxy;
                }

                return _dataSettings;
            }
            set
            {
                _dataSettings = value;

                if (_dataSettings != null)
                {
                    _dataSettings.UndoArrayChanged -= arrayProxy;
                    _dataSettings.UndoArrayChanged += arrayProxy;

                    _dataSettings.UndoPropertyChanged -= undoProxy;
                    _dataSettings.UndoPropertyChanged += undoProxy;
                }
            }
        }

        [JsonIgnore]
        public string Path { get; set; }

        public static ProjectSettings Load(string path)
        {
            ConfigSerializer.RootDirectory = System.IO.Path.GetDirectoryName(System.IO.Path.GetFullPath(path));

            return ConfigSerializer.Load<ProjectSettings>(path);
        }

        public void Save(string path)
        {
            ConfigSerializer.RootDirectory = System.IO.Path.GetDirectoryName(System.IO.Path.GetFullPath(path));

            ConfigSerializer.Save(this, path);
        }

        public string GetAbsolutePath(string filename)
        {
            return System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Path), filename);
        }

        public string GetRelativePath(string path)
        {
            return System.IO.Path.GetFullPath(path).Substring(System.IO.Path.GetDirectoryName(System.IO.Path.GetFullPath(Path)).Length + 1);
        }
    }
}
