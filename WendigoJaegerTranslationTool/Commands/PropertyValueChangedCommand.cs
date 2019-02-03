using WendigoJaeger.TranslationTool.Undo;

namespace WendigoJaeger.TranslationTool.Commands
{
    class PropertyValueChangedCommand : UndoCommand
    {
        private IUndoAware _undoAware;
        private object _instance;
        private readonly string _propertyName;
        private readonly object _oldValue;
        private readonly object _newValue;

        public override string CommandName
        {
            get
            {
                return $"{_propertyName} changed ({_oldValue} -> {_newValue}";
            }
        }

        public PropertyValueChangedCommand(IUndoAware undoAware, object instance, string propertyName, object oldValue, object newValue)
        {
            _undoAware = undoAware;
            _instance = instance;
            _propertyName = propertyName;
            _oldValue = oldValue;
            _newValue = newValue;
        }

        public override void Redo()
        {
            _undoAware.DisableUndoNotify = true;
            _instance.GetType().GetProperty(_propertyName).SetValue(_instance, _newValue);
            _undoAware.DisableUndoNotify = false;
        }

        public override void Undo()
        {
            _undoAware.DisableUndoNotify = true;
            _instance.GetType().GetProperty(_propertyName).SetValue(_instance, _oldValue);
            _undoAware.DisableUndoNotify = false;
        }
    }
}
