using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WendigoJaeger.TranslationTool.Undo
{
    public class UndoPropertyChangedEventArgs
    {
        public string PropertyName { get; private set; }
        public object OldValue { get; private set; }
        public object NewValue { get; private set; }

        public UndoPropertyChangedEventArgs(string propertyName, object oldValue, object newValue)
        {
            PropertyName = propertyName;
            OldValue = oldValue;
            NewValue = newValue;
        }
    }

    public class UndoArrayChangedEventArgs
    {
        public enum OperationType
        {
            Add,
            Remove
        }

        public OperationType Operation { get; private set; }
        public object[] Objects { get; private set; }

        public UndoArrayChangedEventArgs(OperationType operation, object[] value)
        {
            Operation = operation;
            Objects = value;
        }
    }

    public delegate void UndoPropertyChangedEventHandler(object sender, UndoPropertyChangedEventArgs e);

    public delegate void UndoArrayChangedEventHandler(object sender, UndoArrayChangedEventArgs e);

    public interface IUndoPropertyChanged : INotifyPropertyChanged
    {
        event UndoPropertyChangedEventHandler UndoPropertyChanged;
    }

    public interface IUndoArrayChanged
    {
        event UndoArrayChangedEventHandler UndoArrayChanged;
    }

    public interface IUndoAware
    {
        bool DisableUndoNotify { get; set; }
    }

    public class UndoObject : IUndoPropertyChanged, IUndoArrayChanged
    {
        public event UndoArrayChangedEventHandler UndoArrayChanged;
        public event UndoPropertyChangedEventHandler UndoPropertyChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        protected void notifyPropertyChanged(object oldValue, object newValue, [CallerMemberName]string propertyName = "")
        {
            UndoPropertyChanged?.Invoke(this, new UndoPropertyChangedEventArgs(propertyName, oldValue, newValue));

            notifyPropertyChanged(propertyName);
        }

        protected void notifyPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void arrayProxy(object sender, UndoArrayChangedEventArgs e)
        {
            UndoArrayChanged?.Invoke(sender, e);
        }

        protected void undoProxy(object sender, UndoPropertyChangedEventArgs e)
        {
            UndoPropertyChanged?.Invoke(sender, e);
        }

        protected void propertyChangedProxy(object sender, PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(sender, e);
        }
    }
}
