using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WendigoJaeger.TranslationTool.Undo;

namespace WendigoJaeger.TranslationTool.Data
{
    public class AssemblyFileSettings : UndoObject
    {
        private string _name;
        private string _filePath;

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                var oldValue = _name;
                _name = value;
                notifyPropertyChanged(oldValue, value);
            }
        }

        public string FilePath
        {
            get
            {
                return _filePath;
            }
            set
            {
                var oldValue = _filePath;
                _filePath = value;
                notifyPropertyChanged(oldValue, value);
            }
        }
    }
}
