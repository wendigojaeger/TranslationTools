using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using WendigoJaeger.TranslationTool.Controls;
using WendigoJaeger.TranslationTool.Data;

namespace WendigoJaeger.TranslationTool.Editors
{
    public class BaseTextPreviewInfoEditor : BaseEditor<TextPreviewInfo>
    {
    }

    [EditorFor(typeof(TextPreviewInfo))]
    public partial class TextPreviewInfoEditor : BaseTextPreviewInfoEditor
    {
        public override string WindowTitle => Instance.Name;

        public TextPreviewInfoEditor()
        {
            InitializeComponent();
        }

        public override void Init()
        {
            DataContext = Instance;

            Binding fontBindings = new Binding
            {
                Source = Instance,
                Path = new PropertyPath(nameof(Instance.Font)),
                Mode = BindingMode.TwoWay
            };
            fontPicker.SetBinding(RefObjectPtrControl.RefObjectPtrProperty, fontBindings);

            Instance.PropertyChanged -= updateWindowTitle;
            Instance.PropertyChanged += updateWindowTitle;
        }

        private void updateWindowTitle(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Instance.Name))
            {
                refreshWindowTitle();
            }
        }
    }
}
