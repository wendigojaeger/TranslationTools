using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
        }
    }
}
