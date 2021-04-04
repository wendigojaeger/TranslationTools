using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace WendigoJaeger.TranslationTool.Undo
{
    public class UndoObservableCollection<T> : ObservableCollection<T>, IUndoPropertyChanged, IUndoArrayChanged, INotifyPropertyChanged
    {
        public event UndoArrayChangedEventHandler UndoArrayChanged;
        public event UndoPropertyChangedEventHandler UndoPropertyChanged;
#pragma warning disable CS0114 // Member hides inherited member; missing override keyword
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0114 // Member hides inherited member; missing override keyword

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                if (typeof(T).IsSubclassOf(typeof(UndoObject)))
                {
                    foreach (UndoObject item in e.NewItems)
                    {
                        item.UndoArrayChanged -= undoArrayChangedProxy;
                        item.UndoArrayChanged += undoArrayChangedProxy;

                        item.UndoPropertyChanged -= undoPropertyChangedProxy;
                        item.UndoPropertyChanged += undoPropertyChangedProxy;

                        item.PropertyChanged -= propertyChangedProxy;
                        item.PropertyChanged += propertyChangedProxy;
                    }
                }

                object[] affectedObjects = new object[e.NewItems.Count];
                for(int i=0; i<affectedObjects.Length; ++i)
                {
                    affectedObjects[i] = e.NewItems[i];
                }

                undoArrayChangedProxy(this, new UndoArrayChangedEventArgs(UndoArrayChangedEventArgs.OperationType.Add, affectedObjects));
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                if (typeof(T).IsSubclassOf(typeof(UndoObject)))
                {
                    foreach (UndoObject item in e.OldItems)
                    {
                        item.UndoArrayChanged -= undoArrayChangedProxy;
                        item.UndoPropertyChanged -= undoPropertyChangedProxy;
                        item.PropertyChanged -= propertyChangedProxy;
                    }
                }

                object[] affectedObjects = new object[e.OldItems.Count];
                for (int i = 0; i < affectedObjects.Length; ++i)
                {
                    affectedObjects[i] = e.OldItems[i];
                }

                undoArrayChangedProxy(this, new UndoArrayChangedEventArgs(UndoArrayChangedEventArgs.OperationType.Remove, affectedObjects));
            }
        }

        private void undoArrayChangedProxy(object sender, UndoArrayChangedEventArgs e)
        {
            UndoArrayChanged?.Invoke(sender, e);
        }

        private void undoPropertyChangedProxy(object sender, UndoPropertyChangedEventArgs e)
        {
            UndoPropertyChanged?.Invoke(sender, e);
        }

        private void propertyChangedProxy(object sender, PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(sender, e);
        }
    }
}
