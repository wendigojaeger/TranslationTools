using WendigoJaeger.TranslationTool.Undo;

namespace WendigoJaeger.TranslationTool.Data
{
    public enum ScriptEntryState
    {
        ToTranslate,
        InProgress,
        Review,
        Final
    }

    public class TranslationEntry : UndoObject
    {
        private string _lang;
        private string _value;
        private ScriptEntryState _state;

        public string Lang
        {
            get
            {
                return _lang;
            }
            set
            {
                var oldValue = _lang;
                _lang = value;
                notifyPropertyChanged(oldValue, value);
            }
        }

        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                var oldValue = _value;
                _value = value.Replace("\r", "");
                notifyPropertyChanged(oldValue, _value);
            }
        }

        public ScriptEntryState State
        {
            get
            {
                return _state;
            }
            set
            {
                var oldValue = _state;
                _state = value;
                notifyPropertyChanged(oldValue, value);
            }
        }
    }
}
