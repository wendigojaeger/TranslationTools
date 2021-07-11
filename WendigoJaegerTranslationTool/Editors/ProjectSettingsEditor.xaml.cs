using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using WendigoJaeger.TranslationTool.Controls;
using WendigoJaeger.TranslationTool.Data;
using WendigoJaeger.TranslationTool.Extensions;
using WendigoJaeger.TranslationTool.Outputs;
using WendigoJaeger.TranslationTool.Patch;
using WendigoJaeger.TranslationTool.Systems;
using WendigoJaeger.TranslationTool.Windows;

namespace WendigoJaeger.TranslationTool.Editors
{
    public class BaseProjectSettingsEditor : BaseEditor<ProjectSettings>
    {
    }

    [EditorFor(typeof(ProjectSettings))]
    public partial class ProjectSettingsEditor : BaseProjectSettingsEditor
    {
        private static Type[] _cachedSystemTypes;
        private static Type[] _cachedOutputGeneratorTypes;
        private static Type[] _cachedPatcherTypes;

        public override string WindowTitle => Instance.Project.Name;

        public ProjectSettingsEditor()
        {
            InitializeComponent();
        }

        public override void Init()
        {
            base.Init();

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

            List<IPatcher> patcherList = new();
            foreach (var type in _cachedPatcherTypes)
            {
                patcherList.Add((IPatcher)Activator.CreateInstance(type));
            }
            patcherList.Sort((x, y) => x.DisplayName().CompareTo(y.DisplayName()));
            comboPatcher.ItemsSource = patcherList;

            List<OutputGenerator> outputGeneratorList = new();
            foreach (var type in _cachedOutputGeneratorTypes)
            {
                outputGeneratorList.Add((OutputGenerator)Activator.CreateInstance(type));
            }
            outputGeneratorList.Sort((x, y) => x.DisplayName().CompareTo(y.DisplayName()));
            comboOutputGenerator.ItemsSource = outputGeneratorList;

            Binding nameBinding = new()
            {
                Source = Instance.Project,
                Path = new PropertyPath(nameof(Instance.Project.Name)),
                Mode = BindingMode.TwoWay,
            };
            textName.SetBinding(TextBox.TextProperty, nameBinding);

            filePathOriginalROM.ProjectSettings = Instance;
            Binding originalROMPath = new()
            {
                Source = Instance.Project,
                Path = new PropertyPath(nameof(Instance.Project.InputFile)),
                Mode = BindingMode.TwoWay,
            };
            filePathOriginalROM.SetBinding(RelativePathPickerControl.RelativePathProperty, originalROMPath);

            Binding systemBinding = new()
            {
                Source = Instance.Project,
                Path = new PropertyPath(nameof(Instance.Project.System)),
                Mode = BindingMode.TwoWay,
            };
            comboSystem.SetBinding(ComboBox.SelectedItemProperty, systemBinding);
            comboSystem.SelectedIndex = ((List<ISystem>)comboSystem.ItemsSource).FindIndex(x => x.GetType().IsAssignableFrom(Instance.Project.System.GetType()));

            Binding outputGeneratorBinding = new()
            {
                Source = Instance.Project,
                Path = new PropertyPath(nameof(Instance.Project.OutputGenerator)),
                Mode = BindingMode.TwoWay,
            };
            comboOutputGenerator.SetBinding(ComboBox.SelectedItemProperty, outputGeneratorBinding);
            comboOutputGenerator.SelectedIndex = ((List<OutputGenerator>)comboOutputGenerator.ItemsSource).FindIndex(x => x.GetType().IsAssignableFrom(Instance.Project.OutputGenerator.GetType()));

            Binding patcherBinding = new()
            {
                Source = Instance.Project,
                Path = new PropertyPath(nameof(Instance.Project.Patcher)),
                Mode = BindingMode.TwoWay,
            };
            comboPatcher.SetBinding(ComboBox.SelectedItemProperty, patcherBinding);
            comboPatcher.SelectedIndex = ((List<IPatcher>)comboPatcher.ItemsSource).FindIndex(x => x.GetType().IsAssignableFrom(Instance.Project.Patcher.GetType()));

            Binding versionBinding = new()
            {
                Source = Instance.Project,
                Path = new PropertyPath(nameof(Instance.Project.Version)),
                Mode = BindingMode.TwoWay,
            };
            textVersion.SetBinding(TextBox.TextProperty, versionBinding);

            dataGridLanguages.ItemsSource = Instance.Project.Lang;

            listBoxAdditonalFiles.ItemsSource = Instance.Project.AdditionalFilesToPack;
        }

        private void buttonAddLanguage_Click(object sender, RoutedEventArgs e)
        {
            SelectCultureWindow selectCultureWindow = new();
            var result = selectCultureWindow.ShowDialog();
            if (result.HasValue && result.Value)
            {
                var selectedCulture = selectCultureWindow.SelectedCulture;
                if (selectedCulture != null)
                {
                    Instance.Project.Lang.Add(selectedCulture.Name, new LocalizedProjectSettings()
                    {
                        OutputFile = ""
                    });
                }
            }
        }

        private void buttonRemoveLanguage_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridLanguages.SelectedIndex < Instance.Project.Lang.Count)
            {
                Instance.Project.Lang.Remove(((KeyValuePair<string, LocalizedProjectSettings>)dataGridLanguages.SelectedItem).Key);
            }
        }

        private void buttonAddAdditonalFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new();
            openFileDialog.InitialDirectory = Path.GetDirectoryName(Instance.Path);

            var result = openFileDialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                Instance.Project.AdditionalFilesToPack.Add(Instance.GetRelativePath(openFileDialog.FileName));
            }
        }

        private void buttonRemoveAdditonalFile_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxAdditonalFiles.SelectedIndex < Instance.Project.AdditionalFilesToPack.Count)
            {
                Instance.Project.AdditionalFilesToPack.RemoveAt(listBoxAdditonalFiles.SelectedIndex);
            }
        }
    }
}
