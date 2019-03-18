using System;
using System.Runtime.Serialization;
using WendigoJaeger.TranslationTool.Undo;

namespace WendigoJaeger.TranslationTool
{
    public class RefObject : UndoObject, IDisposable
    {
        private string _name;

        public Guid ID { get; set; } = Guid.NewGuid();

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

        public void Dispose()
        {
            ReferenceDatabase.Instance.Unregister(this);
        }

        [OnDeserialized]
        public void OnDeserialized(StreamingContext context)
        {
            ReferenceDatabase.Instance.Register(this);
        }
    }
}
