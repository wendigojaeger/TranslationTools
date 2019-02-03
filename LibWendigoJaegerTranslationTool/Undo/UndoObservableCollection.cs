using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace WendigoJaeger.TranslationTool.Undo
{
    public class UndoObservableCollection<T> : ObservableCollection<T>, IUndoPropertyChanged where T : UndoObject
    {
        public event UndoPropertyChangedEventHandler UndoPropertyChanged;

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (UndoObject item in e.NewItems)
                {
                    item.UndoPropertyChanged -= undoPropertyChangedProxy;
                    item.UndoPropertyChanged += undoPropertyChangedProxy;
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (UndoObject item in e.OldItems)
                {
                    item.UndoPropertyChanged -= undoPropertyChangedProxy;
                }
            }
        }

        private void undoPropertyChangedProxy(object sender, UndoPropertyChangedEventArgs e)
        {
            UndoPropertyChanged?.Invoke(sender, e);
        }
    }
}
