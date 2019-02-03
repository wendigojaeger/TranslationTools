using WendigoJaeger.TranslationTool.Data;
using WendigoJaeger.TranslationTool.Undo;
using System;
using System.Windows.Controls;

namespace WendigoJaeger.TranslationTool.Editors
{
    public class EditorForAttribute : Attribute
    {
        public Type EditedType { get; private set; }

        public EditorForAttribute(Type type)
        {
            EditedType = type;
        }
    }

    public interface IEditor
    {
        Action<string> UpdateStatusBar { get; set; }

        object EditedItem { get; set; }
        ProjectSettings ProjectSettings { get;  set; }

        void Init();
    }

    public class BaseEditor<T> : UserControl, IEditor where T : UndoObject
    {
        public Action<string> UpdateStatusBar { get; set; }

        public object EditedItem
        {
            get
            {
                return Instance;
            }
            set
            {
                Instance = (T)value;
            }
        }

        public ProjectSettings ProjectSettings { get; set; }

        protected T Instance { get; private set; }

        public virtual void Init()
        {
        }

        protected void updateStatusBar(string value)
        {
            UpdateStatusBar?.Invoke(value);
        }
    }
}
