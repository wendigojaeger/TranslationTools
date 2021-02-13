using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using Xceed.Wpf.Toolkit;

namespace WendigoJaeger.TranslationTool.Editors
{
    public class BaseTableFileEditor : BaseEditor<TableFile>
    {
    }

    [EditorFor(typeof(TableFile))]
    public partial class TableFileEditor : BaseTableFileEditor
    {
        public override string WindowTitle => Instance.Name;

        public TableFileEditor()
        {
            InitializeComponent();
        }

        public override void Init()
        {
            DataContext = Instance;

            Instance.SyncLanguages(ProjectSettings.Project);

            Binding pathBinding = new Binding
            {
                Source = Instance,
                Path = new PropertyPath(nameof(TableFile.SourceTableFile)),
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                Mode = BindingMode.TwoWay
            };
            sourceTableFilePicker.ProjectSettings = ProjectSettings;
            sourceTableFilePicker.SetBinding(RelativePathPickerControl.RelativePathProperty, pathBinding);

            Binding targetPathBinding = new Binding
            {
                Path = new PropertyPath(nameof(LocalizedFilePathEntry.Path)),
                Mode = BindingMode.TwoWay
            };
            targetTableFilePicker.ProjectSettings = ProjectSettings;
            targetTableFilePicker.SetBinding(RelativePathPickerControl.RelativePathProperty, targetPathBinding);

            onCurrentLocaleChanged(CurrentLocale);

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

        protected override void onCurrentLocaleChanged(string newLocale)
        {
            var newLocalizedEntry = Instance.GetTargetTable(newLocale);

            imageFlagTargetTable.DataContext = newLocalizedEntry;
            targetTableFilePicker.DataContext = newLocalizedEntry;
        }
    }
}

