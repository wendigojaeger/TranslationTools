using Newtonsoft.Json;
using System;

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

    public interface IRefObjectPtr
    {
        Guid RefID { get; set; }
    }

    public class RefObjectPtr<T> : IRefObjectPtr where T : RefObject
    {
        public Guid RefID
        {
            get;
            set;
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
