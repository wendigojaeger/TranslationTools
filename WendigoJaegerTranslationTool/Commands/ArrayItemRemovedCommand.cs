using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WendigoJaeger.TranslationTool.Undo;

namespace WendigoJaeger.TranslationTool.Commands
{
    class ArrayItemRemovedCommand : UndoCommand
    {
        private IUndoAware _undoAware;
        private IList _array;
        private readonly object[] _values;

        public override string CommandName
        {
            get
            {
                return $"Removed {_values.Length} item(s) of type {_values.GetType().Name}";
            }
        }

        public ArrayItemRemovedCommand(IUndoAware undoAware, IList array, object[] values)
        {
            _undoAware = undoAware;
            _array = array;
            _values = values;
        }

        public override void Redo()
        {
            _undoAware.DisableUndoNotify = true;
            foreach (var item in _values)
            {
                _array.Remove(item);
            }
            _undoAware.DisableUndoNotify = false;
        }

        public override void Undo()
        {
            _undoAware.DisableUndoNotify = true;
            foreach (var item in _values)
            {
                _array.Add(item);
            }
            _undoAware.DisableUndoNotify = false;
        }
    }
}
