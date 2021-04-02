using System.Windows;
using System.Windows.Data;
using WendigoJaeger.TranslationTool.Controls;
using WendigoJaeger.TranslationTool.Data;

namespace WendigoJaeger.TranslationTool.Editors
{
    public class BaseDataSettingsEditor : BaseEditor<DataSettings>
    {
    }

    [EditorFor(typeof(DataSettings))]
    public partial class DataSettingsEditor : BaseDataSettingsEditor
    {
        public override string WindowTitle => Instance.Name;

        public DataSettingsEditor()
        {
            InitializeComponent();
        }

        public override void Init()
        {
            DataContext = Instance;

            Binding tableFileBinding = new()
            {
                Source = Instance,
                Path = new PropertyPath(nameof(Instance.TableFile)),
                Mode = BindingMode.TwoWay
            };
            tableFilePicker.SetBinding(RefObjectPtrControl.RefObjectPtrProperty, tableFileBinding);

            Binding textPreviewBinding = new()
            {
                Source = Instance,
                Path = new PropertyPath(nameof(Instance.TextPreview)),
                Mode = BindingMode.TwoWay
            };
            textPreviewPicker.SetBinding(RefObjectPtrControl.RefObjectPtrProperty, textPreviewBinding);

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
