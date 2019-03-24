using System.Windows;
using System.Windows.Data;
using WendigoJaeger.TranslationTool.Controls;
using WendigoJaeger.TranslationTool.Data;

namespace WendigoJaeger.TranslationTool.Editors
{
    public class BaseAssemblyFileSettingsEditor : BaseEditor<AssemblyFileSettings>
    {
    }

    [EditorFor(typeof(AssemblyFileSettings))]
    public partial class AssemblyFileSettingsEditor : BaseAssemblyFileSettingsEditor
    {
        public override string WindowTitle => Instance.Name;

        public AssemblyFileSettingsEditor()
        {
            InitializeComponent();
        }

        public override void Init()
        {
            base.Init();

            filePathBrowser.ProjectSettings = ProjectSettings;
            filePathBrowser.Filter = Resource.filterAssemblyFile;

            Binding pathBinding = new Binding
            {
                Source = Instance,
                Path = new PropertyPath(nameof(AssemblyFileSettings.FilePath)),
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                Mode = BindingMode.TwoWay
            };

            filePathBrowser.SetBinding(RelativePathPickerControl.RelativePathProperty, pathBinding);

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
