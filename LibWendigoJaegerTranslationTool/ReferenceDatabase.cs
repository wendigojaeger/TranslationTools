using System;
using System.Collections.Generic;

namespace WendigoJaeger.TranslationTool
{
    public class ReferenceDatabase
    {
        private readonly Dictionary<Guid, RefObject> _database = new();

        public static ReferenceDatabase Instance { get; } = new ReferenceDatabase();

        private ReferenceDatabase()
        {
        }

        static ReferenceDatabase()
        {
        }

        public T CreateNew<T>() where T : RefObject, new()
        {
            T newObject = new();
            Register(newObject);
            return newObject;
        }

        public void Register(RefObject obj)
        {
            if (obj.ID != Guid.Empty)
            {
                _database.TryAdd(obj.ID, obj);
            }
        }

        public void Unregister(RefObject obj)
        {
            _database.Remove(obj.ID);
        }

        public T Get<T>(Guid id) where T : RefObject
        {
            RefObject result;
            _database.TryGetValue(id, out result);
            return result as T;
        }

        public IEnumerable<T> ListAll<T>() where T : RefObject
        {
            foreach(var entry in _database)
            {
                var result = entry.Value as T;
                if (result != null)
                {
                    yield return result;
                }
            }
        }

        public IEnumerable<RefObject> ListAll(Type type)
        {
            foreach(var entry in _database)
            {
                if (type.IsAssignableFrom(entry.Value.GetType()))
                {
                    yield return entry.Value;
                }
            }
        }
    }
}
