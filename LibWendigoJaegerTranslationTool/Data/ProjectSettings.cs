using Newtonsoft.Json;
using System.Collections.Generic;
using WendigoJaeger.TranslationTool.Outputs;
using WendigoJaeger.TranslationTool.Patch;
using WendigoJaeger.TranslationTool.Systems;
using WendigoJaeger.TranslationTool.Undo;

namespace WendigoJaeger.TranslationTool.Data
{
    public class LocalizedProjectSettings : UndoObject
    {
        private string _outputFile;
        public string OutputFile
        {
            get
            {
                return _outputFile;
            }
            set
            {
                var oldValue = _outputFile;
                _outputFile = value;
                notifyPropertyChanged(oldValue, value);
            }
        }
    }

    public class Project : UndoObject
    {
        private string _name = string.Empty;
        private string _inputFile = string.Empty;
        private string _version = string.Empty;
        private ISystem _system;
        private OutputGenerator _outputGenerator;
        private IPatcher _patcher;
        private UndoObservableCollection<string> _additionalFilesToPack;

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

        public ISystem System
        {
            get
            {
                return _system;
            }
            set
            {
                var oldValue = _system;
                _system = value;
                notifyPropertyChanged(oldValue, value);
            }
        }

        public OutputGenerator OutputGenerator
        {
            get
            {
                return _outputGenerator;
            }
            set
            {
                var oldValue = _outputGenerator;
                _outputGenerator = value;
                notifyPropertyChanged(oldValue, value);
            }
        }

        public IPatcher Patcher
        {
            get
            {
                return _patcher;
            }
            set
            {
                var oldValue = _patcher;
                _patcher = value;
                notifyPropertyChanged(oldValue, value);
            }
        }

        public UndoObservableCollection<string> AdditionalFilesToPack
        {
            get
            {
                if (_additionalFilesToPack == null)
                {
                    _additionalFilesToPack = new();
                    _additionalFilesToPack.UndoArrayChanged += arrayProxy;
                    _additionalFilesToPack.UndoPropertyChanged += undoProxy;
                }

                return _additionalFilesToPack;
            }
            set
            {
                _additionalFilesToPack = value;

                if (_additionalFilesToPack != null)
                {
                    _additionalFilesToPack.UndoArrayChanged -= arrayProxy;
                    _additionalFilesToPack.UndoArrayChanged += arrayProxy;

                    _additionalFilesToPack.UndoPropertyChanged -= undoProxy;
                    _additionalFilesToPack.UndoPropertyChanged += undoProxy;
                }
            }
        }

        public string Version
        {
            get
            {
                return _version;
            }
            set
            {
                var oldValue = _version;
                _version = value;
                notifyPropertyChanged(oldValue, value);
            }
        }

        public Dictionary<string, LocalizedProjectSettings> Lang { get; set; } = new Dictionary<string, LocalizedProjectSettings>();
    }

    public class ProjectSettings : UndoObject
    {
        private Project _project;
        private UndoObservableCollection<GraphicsSettings> _graphics;
        private UndoObservableCollection<AssemblyFileSettings> _assemblyFileSettings;
        private UndoObservableCollection<FontSettings> _fontSettings;
        private UndoObservableCollection<Palette> _palettes;
        private UndoObservableCollection<TableFile> _tableFiles;
        private UndoObservableCollection<TextPreviewInfo> _textPreviewInfos;
        private UndoObservableCollection<ScriptSettings> _scriptSettings;

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

        public UndoObservableCollection<ScriptSettings> ScriptSettings
        {
            get
            {
                if (_scriptSettings == null)
                {
                    _scriptSettings = new UndoObservableCollection<ScriptSettings>();
                    _scriptSettings.UndoArrayChanged += arrayProxy;
                    _scriptSettings.UndoPropertyChanged += undoProxy;
                }

                return _scriptSettings;
            }
            set
            {
                _scriptSettings = value;

                if (_scriptSettings != null)
                {
                    _scriptSettings.UndoArrayChanged -= arrayProxy;
                    _scriptSettings.UndoArrayChanged += arrayProxy;

                    _scriptSettings.UndoPropertyChanged -= undoProxy;
                    _scriptSettings.UndoPropertyChanged += undoProxy;
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
