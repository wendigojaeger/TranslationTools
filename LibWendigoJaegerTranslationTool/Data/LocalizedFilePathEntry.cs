using WendigoJaeger.TranslationTool.Undo;

namespace WendigoJaeger.TranslationTool.Data
{
    public class LocalizedFilePathEntry : UndoObject
    {
        private string _lang;
        private string _path;

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

        public string Path
        {
            get
            {
                return _path;
            }
            set
            {
                var oldValue = _path;
                _path = value;
                notifyPropertyChanged(oldValue, _path);
            }
        }
    }
}
