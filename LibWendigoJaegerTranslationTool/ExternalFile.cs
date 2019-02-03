using WendigoJaeger.TranslationTool.Undo;
using Newtonsoft.Json;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization;

namespace WendigoJaeger.TranslationTool
{
    public class ExternalFile<T> : IUndoPropertyChanged where T : UndoObject, new()
    {
        private T _instance;

        public string Path { get; set; }

        [JsonIgnore]
        public T Instance
        {
            get
            {
                if (_instance == null)
                {
                    string fullPath = System.IO.Path.Combine(ConfigSerializer.RootDirectory, Path);

                    if (!string.IsNullOrEmpty(ConfigSerializer.RootDirectory) && File.Exists(fullPath))
                    {
                        _instance = ConfigSerializer.Load<T>(fullPath);
                    }
                    else
                    {
                        _instance = new T();
                    }

                    _instance.UndoPropertyChanged += undoProxy;
                    _instance.PropertyChanged += propertyChangedProxy;
                }

                return _instance;
            }
        }

        public event UndoPropertyChangedEventHandler UndoPropertyChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        public void Save()
        {
            ConfigSerializer.Save(Instance, System.IO.Path.Combine(ConfigSerializer.RootDirectory, Path));
        }

        [OnSerializing]
        internal void OnSerializing(StreamingContext context)
        {
            Path = System.IO.Path.GetFileName(Path);
            Save();
        }

        private void undoProxy(object sender, UndoPropertyChangedEventArgs e)
        {
            UndoPropertyChanged?.Invoke(sender, e);
        }

        private void propertyChangedProxy(object sender, PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(sender, e);
        }
    }
}
