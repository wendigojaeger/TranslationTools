using WendigoJaeger.TranslationTool.Data;
using WendigoJaeger.TranslationTool.Undo;
using System;
using System.Windows.Controls;
using System.ComponentModel;
using System.Runtime.CompilerServices;

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
        MainWindow MainWindow { get; set; }

        string WindowTitle { get; }

        void Init();
    }

    public class BaseEditor<T> : UserControl, IEditor, IUndoAware, INotifyPropertyChanged where T : UndoObject
    {
        public event PropertyChangedEventHandler PropertyChanged;

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

        public virtual string WindowTitle
        {
            get
            {
                return string.Empty;
            }
        }

        public ProjectSettings ProjectSettings { get; set; }

        public MainWindow MainWindow { get; set; }

        public bool DisableUndoNotify
        {
            get
            {
                return MainWindow.DisableUndoNotify;
            }
            set
            {
                MainWindow.DisableUndoNotify = value;
            }
        }

        protected T Instance { get; private set; }

        public virtual void Init()
        {
        }

        protected void execute(UndoCommand command)
        {
            if (!DisableUndoNotify)
            {
                MainWindow.UndoStack.Execute(command);
            }
        }

        protected void updateStatusBar(string value)
        {
            UpdateStatusBar?.Invoke(value);
        }

        protected void notifyPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void refreshWindowTitle()
        {
            notifyPropertyChanged(nameof(WindowTitle));
        }
    }
}
