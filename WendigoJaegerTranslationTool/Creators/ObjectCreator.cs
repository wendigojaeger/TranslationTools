using System;
using System.Linq;
using WendigoJaeger.TranslationTool.Data;
using WendigoJaeger.TranslationTool.Undo;

namespace WendigoJaeger.TranslationTool.Creators
{
    public interface IObjectCreator
    {
        object CreatedObject { get; }
        Type ObjectType { set; }
        ProjectSettings ProjectSettings { get; set; }
        bool? ShowDialog();
    }

    public class ObjectCreatorAttribute : Attribute
    {
        public Type TypeToCreate { get; private set; }

        public ObjectCreatorAttribute(Type type)
        {
            TypeToCreate = type;
        }
    }

    public static class ObjectCreator
    {
        private static Type[] _creatorTypes = null;

        public static Type[] CreatorTypes
        {
            get
            {
                if (_creatorTypes == null)
                {
                    var types = from a in AppDomain.CurrentDomain.GetAssemblies()
                                from t in a.GetTypes()
                                where t.GetCustomAttributes(typeof(ObjectCreatorAttribute), false) != null
                                select t;

                    _creatorTypes = types.ToArray();
                }

                return _creatorTypes;
            }
        }

        public static T Create<T>(ProjectSettings projectSettings) where T : UndoObject
        {
            var creatorTypeQuery = from t in CreatorTypes
                                   from attribute in t.GetCustomAttributes(typeof(ObjectCreatorAttribute), false) as ObjectCreatorAttribute[]
                                   where attribute.TypeToCreate.IsAssignableFrom(typeof(T))
                                   orderby typeof(T) == attribute.TypeToCreate ? 0 : 1
                                   select t;

            var creatorType = creatorTypeQuery.FirstOrDefault();
            if (creatorType != null)
            {
                IObjectCreator creatorWindow = (IObjectCreator)Activator.CreateInstance(creatorType);
                creatorWindow.ProjectSettings = projectSettings;
                creatorWindow.ObjectType = typeof(T);

                var result = creatorWindow.ShowDialog();
                if (result.HasValue && result.Value)
                {
                    if (creatorWindow.CreatedObject is IRefObjectPtr)
                    {
                        ReferenceDatabase.Instance.Register(creatorWindow.CreatedObject as RefObject);
                    }

                    return creatorWindow.CreatedObject as T;
                }
            }

            return null;
        }
    }
}
