using System.Windows;
using System.Windows.Data;
using WendigoJaeger.TranslationTool.Controls;
using WendigoJaeger.TranslationTool.Data;

namespace WendigoJaeger.TranslationTool.Editors
{
    public class BaseScriptSettingsEditor : BaseEditor<ScriptSettings>
    {
    }

    [EditorFor(typeof(ScriptSettings))]
    public partial class ScriptSettingsEditor : BaseScriptSettingsEditor
    {
        public override string WindowTitle => Instance.Name;

        public ScriptSettingsEditor()
        {
            InitializeComponent();
        }

        public override void Init()
        {
            DataContext = Instance;

            Binding tableFileBinding = new Binding();
            tableFileBinding.Source = Instance;
            tableFileBinding.Path = new PropertyPath(nameof(Instance.TableFile));
            tableFileBinding.Mode = BindingMode.TwoWay;
            tableFilePicker.SetBinding(RefObjectPtrControl.RefObjectPtrProperty, tableFileBinding);

            Instance.PropertyChanged -= updateWindowTitle;
            Instance.PropertyChanged += updateWindowTitle;
        }

        private void updateWindowTitle(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Instance.Name))
            {
                refreshWindowTitle();
            }
        }
    }
}
