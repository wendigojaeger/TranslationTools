using System.Collections.Generic;
using System.Collections.Specialized;

namespace WendigoJaeger.TranslationTool.Undo
{
    public class ObservableStack<T> : Stack<T>, INotifyCollectionChanged
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public ObservableStack()
        {
        }

        public ObservableStack(IEnumerable<T> collection)
        {
            foreach (var item in collection)
            {
                base.Push(item);
            }
        }

        public new void Clear()
        {
            base.Clear();
            onCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public new T Pop()
        {
            var item = base.Pop();
            onCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, Count));
            return item;
        }

        public new void Push(T item)
        {
            base.Push(item);
            onCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        }

        private void onCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged?.Invoke(this, e);
        }
    }
}
