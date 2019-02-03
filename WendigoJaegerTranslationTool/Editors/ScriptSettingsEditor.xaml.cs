using WendigoJaeger.TranslationTool.Data;

namespace WendigoJaeger.TranslationTool.Editors
{
    public class BaseScriptSettingsEditor : BaseEditor<ScriptSettings>
    {
    }

    [EditorFor(typeof(ScriptSettings))]
    public partial class ScriptSettingsEditor : BaseScriptSettingsEditor
    {
        public ScriptSettingsEditor()
        {
            InitializeComponent();
        }
    }
}
