using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using WendigoJaeger.TranslationTool.Data;
using WendigoJaeger.TranslationTool.Extensions;
using WendigoJaeger.TranslationTool.Undo;

namespace WendigoJaeger.TranslationTool.Commands
{
    class PaletteColorCommand : UndoCommand
    {
        private IUndoAware _undoAware;
        private PaletteColor _instance;
        private Color _oldColor;
        private Color _newColor;

        public override string CommandName
        {
            get
            {
                return $"Palette changed ({_oldColor} -> {_newColor}";
            }
        }

        public PaletteColorCommand(IUndoAware undoAware, PaletteColor instance, Color newColor)
        {
            _undoAware = undoAware;
            _instance = instance;
            _oldColor = _instance.ToWpfColor();
            _newColor = newColor;
        }

        public override void Execute()
        {
            Redo();
        }

        public override void Redo()
        {
            _undoAware.DisableUndoNotify = true;
            _instance.R = _newColor.R;
            _instance.G = _newColor.G;
            _instance.B = _newColor.B;
            _undoAware.DisableUndoNotify = false;
        }

        public override void Undo()
        {
            _undoAware.DisableUndoNotify = true;
            _instance.R = _oldColor.R;
            _instance.G = _oldColor.G;
            _instance.B = _oldColor.B;
            _undoAware.DisableUndoNotify = false;
        }
    }
}
