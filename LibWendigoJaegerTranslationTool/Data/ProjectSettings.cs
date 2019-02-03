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

        public Project Project
        {
            get
            {
                if (_project == null)
                {
                    _project = new Project();
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
                    _project.UndoPropertyChanged -= undoProxy;
                    _project.PropertyChanged -= propertyChangedProxy;

                    _project.UndoPropertyChanged += undoProxy;
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
                    _scripts.UndoPropertyChanged += undoProxy;
                }

                return _scripts;
            }
            set
            {
                _scripts = value;

                if (_scripts != null)
                {
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
                    _graphics.UndoPropertyChanged += undoProxy;
                }

                return _graphics;
            }
            set
            {
                _graphics = value;

                if (_graphics != null)
                {
                    _graphics.UndoPropertyChanged -= undoProxy;
                    _graphics.UndoPropertyChanged += undoProxy;
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
