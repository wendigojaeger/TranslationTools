using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.IO;

namespace WendigoJaeger.TranslationTool
{
    public class ConfigSerializer
    {
        private static JsonSerializer _serializer;
        private static JsonSerializer Serializer
        {
            get
            {
                if (_serializer == null)
                {
                    _serializer = new JsonSerializer
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        DefaultValueHandling = DefaultValueHandling.Ignore,
                        Formatting = Formatting.Indented,
                        TypeNameHandling = TypeNameHandling.All
                    };
                    _serializer.Converters.Add(new StringEnumConverter());
                }

                return _serializer;
            }
        }

        public static string RootDirectory { get; set; }

        public static T Load<T>(string path)
        {
            using (StreamReader sr = new StreamReader(path))
            {
                using (JsonReader reader = new JsonTextReader(sr))
                {
                    object readObject = Serializer.Deserialize(reader);
                    if (readObject is T)
                    {
                        return (T)readObject;
                    }
                }
            }

            return default(T);
        }

        public static void Save<T>(T obj, string path)
        {
            string directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            try
            {
                using (StreamWriter sw = new StreamWriter(path))
                {
                    using (JsonWriter writer = new JsonTextWriter(sw))
                    {
                        Serializer.Serialize(writer, obj);
                    }
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
