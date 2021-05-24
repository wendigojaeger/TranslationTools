using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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
using System.Windows.Shapes;
using WendigoJaeger.TranslationTool.Data;
using WendigoJaeger.TranslationTool.Extensions;
using WendigoJaeger.TranslationTool.Extractors;
using WendigoJaeger.TranslationTool.Outputs;
using WendigoJaeger.TranslationTool.Patch;
using WendigoJaeger.TranslationTool.Systems;

namespace WendigoJaeger.TranslationTool.Windows
{
    class LanguageEntry
    {
        public string Language { get; set; }
        public string OutputFile { get; set; }
    }

    public partial class NewProjectWindow : Window
    {
        static Type[] _cachedSystemTypes = null;
        static Type[] _cachedOutputGeneratorTypes = null;
        static Type[] _cachedPatcherTypes = null;

        private ObservableCollection<LanguageEntry> _languages = new();

        public ProjectSettings NewProjectSettings { get; set; }

        public NewProjectWindow()
        {
            InitializeComponent();

            windowInit();
        }

        private void windowInit()
        {
            if (_cachedSystemTypes == null)
            {
                var query = from a in AppDomain.CurrentDomain.GetAssemblies()
                            from t in a.GetTypes()
                            where t.GetInterfaces().Contains(typeof(ISystem))
                            select t;

                _cachedSystemTypes = query.ToArray();
            }

            if (_cachedOutputGeneratorTypes == null)
            {
                var query = from a in AppDomain.CurrentDomain.GetAssemblies()
                            from t in a.GetTypes()
                            where t.IsSubclassOf(typeof(OutputGenerator))
                            select t;

                _cachedOutputGeneratorTypes = query.ToArray();
            }

            if (_cachedPatcherTypes == null)
            {
                var query = from a in AppDomain.CurrentDomain.GetAssemblies()
                            from t in a.GetTypes()
                            where t.GetInterfaces().Contains(typeof(IPatcher))
                            select t;

                _cachedPatcherTypes = query.ToArray();
            }

            List<ISystem> systemList = new();
            foreach (var type in _cachedSystemTypes)
            {
                systemList.Add((ISystem)Activator.CreateInstance(type));
            }
            systemList.Sort((x, y) => x.DisplayName().CompareTo(y.DisplayName()));
            comboSystem.ItemsSource = systemList;
            comboSystem.SelectedIndex = 0;

            List<IPatcher> patcherList = new();
            foreach(var type in _cachedPatcherTypes)
            {
                patcherList.Add((IPatcher)Activator.CreateInstance(type));
            }
            patcherList.Sort((x, y) => x.DisplayName().CompareTo(y.DisplayName()));
            comboPatcher.ItemsSource = patcherList;
            comboPatcher.SelectedIndex = 0;

            List<OutputGenerator> outputGeneratorList = new();
            foreach(var type in _cachedOutputGeneratorTypes)
            {
                outputGeneratorList.Add((OutputGenerator)Activator.CreateInstance(type));
            }
            outputGeneratorList.Sort((x, y) => x.DisplayName().CompareTo(y.DisplayName()));
            comboOutputGenerator.ItemsSource = outputGeneratorList;
            comboOutputGenerator.SelectedIndex = 0;

            dataGridLanguages.ItemsSource = _languages;
        }

        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            NewProjectSettings = new ProjectSettings
            {
                Path = System.IO.Path.GetDirectoryName(filePathOriginalROM.AbsolutePath),
                Project = new Project()
                {
                    Name = textName.Text,
                    InputFile = System.IO.Path.GetFileName(filePathOriginalROM.AbsolutePath),
                    System = (ISystem)comboSystem.SelectedItem,
                    Patcher = (IPatcher)comboPatcher.SelectedItem,
                    OutputGenerator = (OutputGenerator)comboOutputGenerator.SelectedItem,
                    Version = textVersion.Text,
                }
            };

            foreach (var entry in _languages)
            {
                NewProjectSettings.Project.Lang.Add(entry.Language, new LocalizedProjectSettings { OutputFile = entry.OutputFile });
            }

            DialogResult = true;
            Close();
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void buttonAddLanguage_Click(object sender, RoutedEventArgs e)
        {
            SelectCultureWindow selectCultureWindow = new SelectCultureWindow();
            var result = selectCultureWindow.ShowDialog();
            if (result.HasValue && result.Value)
            {
                var selectedCulture = selectCultureWindow.SelectedCulture;
                if (selectedCulture != null)
                {
                    _languages.Add(new LanguageEntry
                    {
                        Language = selectedCulture.Name,
                        OutputFile = ""
                    });
                }
            }
        }

        private void buttonRemoveLanguage_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridLanguages.SelectedIndex < _languages.Count)
            {
                _languages.RemoveAt(dataGridLanguages.SelectedIndex);
            }
        }
    }
}
