using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace WendigoJaeger.TranslationTool.Undo
{
    public class UndoObservableCollection<T> : ObservableCollection<T>, IUndoPropertyChanged, IUndoArrayChanged where T : UndoObject
    {
        public event UndoArrayChangedEventHandler UndoArrayChanged;
        public event UndoPropertyChangedEventHandler UndoPropertyChanged;

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (UndoObject item in e.NewItems)
                {
                    item.UndoArrayChanged -= undoArrayChangedProxy;
                    item.UndoArrayChanged += undoArrayChangedProxy;

                    item.UndoPropertyChanged -= undoPropertyChangedProxy;
                    item.UndoPropertyChanged += undoPropertyChangedProxy;
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
                foreach (UndoObject item in e.OldItems)
                {
                    item.UndoArrayChanged -= undoArrayChangedProxy;
                    item.UndoPropertyChanged -= undoPropertyChangedProxy;
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
    }
}
