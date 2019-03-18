using System;
using System.Collections.Generic;

namespace WendigoJaeger.TranslationTool
{
    public class ReferenceDatabase
    {
        private Dictionary<Guid, RefObject> _database = new Dictionary<Guid, RefObject>();

        public static ReferenceDatabase Instance { get; } = new ReferenceDatabase();

        private ReferenceDatabase()
        {
        }

        static ReferenceDatabase()
        {
        }

        public void Register(RefObject obj)
        {
            if (obj.ID != Guid.Empty)
            {
                _database.Add(obj.ID, obj);
            }
        }

        public void Unregister(RefObject obj)
        {
            _database.Remove(obj.ID);
        }

        public T Get<T>(Guid id) where T : RefObject
        {
            RefObject result = null;
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
