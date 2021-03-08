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

        static Type[] _cachedTextExtractorTypes = null;

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

            newScriptSettings.TextExtractor = (ITextExtractor)comboTextExtractors.SelectedItem;
            newScriptSettings.TableFile.RefID = tableRefObjectPicker.SelectedRefObject.ID;
            newScriptSettings.TextPreview.RefID = textPreviewRefObjectPicker.SelectedRefObject.ID;

            newScriptSettings.Script.Path = ProjectSettings.GetAbsolutePath($"{textName.Text}.wts");

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
            if (_cachedTextExtractorTypes == null)
            {
                var query = from a in AppDomain.CurrentDomain.GetAssemblies()
                            from t in a.GetTypes()
                            where t.GetInterfaces().Contains(typeof(ITextExtractor))
                            select t;

                _cachedTextExtractorTypes = query.ToArray();
            }

            List<ITextExtractor> textExtractors = new List<ITextExtractor>();

            foreach (var type in _cachedTextExtractorTypes)
            {
                textExtractors.Add((ITextExtractor)Activator.CreateInstance(type));
            }
            textExtractors.Sort((x, y) => x.Name.CompareTo(y.Name));

            comboTextExtractors.ItemsSource = textExtractors;
        }
    }
}
