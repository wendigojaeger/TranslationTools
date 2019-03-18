using Newtonsoft.Json;
using System;
using WendigoJaeger.TranslationTool.Undo;

namespace WendigoJaeger.TranslationTool
{
    public class RefObjectPtrConverter : JsonConverter
    {
        public override bool CanRead
        {
            get
            {
                return true;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(IRefObjectPtr).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (existingValue == null)
            {
                existingValue = Activator.CreateInstance(objectType);
            }

            ((IRefObjectPtr)existingValue).RefID = Guid.Parse(reader.Value.ToString());

            return existingValue;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((IRefObjectPtr)value).RefID.ToString());
        }
    }

    public interface IRefObjectPtr : IUndoPropertyChanged
    {
        Guid RefID { get; set; }
        string ObjectName { get; }
    }

    public class RefObjectPtr<T> : UndoObject, IRefObjectPtr where T : RefObject
    {
        private Guid _refID;

        public Guid RefID
        {
            get
            {
                return _refID;
            }
            set
            {
                var oldValue = _refID;
                _refID = value;
                notifyPropertyChanged(oldValue, value);
            }
        }

        public string ObjectName
        {
            get
            {
                var instance = Instance;
                if (instance != null)
                {
                    return instance.Name;
                }

                return string.Empty;
            }
        }

        public T Instance
        {
            get
            {
                return ReferenceDatabase.Instance.Get<T>(RefID);
            }
            set
            {
                RefID = value.ID;
            }
        }
    }
}
