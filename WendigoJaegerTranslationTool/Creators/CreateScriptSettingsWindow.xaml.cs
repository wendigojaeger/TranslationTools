using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using WendigoJaeger.TranslationTool.Data;
using WendigoJaeger.TranslationTool.Extractors;

namespace WendigoJaeger.TranslationTool.Creators
{
    [ObjectCreator(typeof(ScriptSettings))]
    public partial class CreateScriptSettingsWindow : Window, IObjectCreator
    {
        public object CreatedObject { get; set; }
        public ProjectSettings ProjectSettings { get; set; }
        public Type ObjectType { get; set; }

        private static Type[] _cachedScriptExtractorTypes;

        public CreateScriptSettingsWindow()
        {
            InitializeComponent();

            windowInit();
        }

        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            ScriptSettings newScriptSettings = new ScriptSettings
            {
                Name = textName.Text,
                SourceRAMAddress = upDownSourceRAM.Value.Value,
                DestinationRAMAddress = upDownDestinationRAM.Value.Value,
                Entries = upDownEntries.Value.Value
            };

            newScriptSettings.ScriptExtractor = (IScriptExtractor)comboScriptExtractors.SelectedItem;
            newScriptSettings.TableFile.RefID = tableRefObjectPicker.SelectedRefObject.ID;
            newScriptSettings.TextPreview.RefID = textPreviewRefObjectPicker.SelectedRefObject.ID;

            newScriptSettings.ScriptFile.Path = ProjectSettings.GetAbsolutePath($"{textName.Text}.wtd");

            CreatedObject = newScriptSettings;

            DialogResult = true;
            Close();
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void windowInit()
        {
            if (_cachedScriptExtractorTypes == null)
            {
                var query = from a in AppDomain.CurrentDomain.GetAssemblies()
                            from t in a.GetTypes()
                            where t.GetInterfaces().Contains(typeof(IScriptExtractor))
                            select t;

                _cachedScriptExtractorTypes = query.ToArray();
            }

            List<IScriptExtractor> scriptExtractors = new();

            foreach (var type in _cachedScriptExtractorTypes)
            {
                scriptExtractors.Add((IScriptExtractor)Activator.CreateInstance(type));
            }
            scriptExtractors.Sort((x, y) => x.Name.CompareTo(y.Name));

            comboScriptExtractors.ItemsSource = scriptExtractors;
        }
    }
}
