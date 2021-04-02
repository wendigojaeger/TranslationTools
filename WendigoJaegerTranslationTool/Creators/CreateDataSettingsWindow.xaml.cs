using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using WendigoJaeger.TranslationTool.Data;
using WendigoJaeger.TranslationTool.Extractors;

namespace WendigoJaeger.TranslationTool.Creators
{
    [ObjectCreator(typeof(DataSettings))]
    public partial class CreateDataSettingsWindow : Window, IObjectCreator
    {
        public object CreatedObject { get; set; }
        public ProjectSettings ProjectSettings { get; set; }
        public Type ObjectType { get; set; }

        static Type[] _cachedDataExtractorTypes = null;

        public CreateDataSettingsWindow()
        {
            InitializeComponent();

            windowInit();
        }

        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            DataSettings newDataSettings = new DataSettings
            {
                Name = textName.Text,
                SourceRAMAddress = upDownSourceRAM.Value.Value,
                DestinationRAMAddress = upDownDestinationRAM.Value.Value,
                Entries = upDownEntries.Value.Value
            };

            newDataSettings.DataExtractor = (IDataExtractor)comboDataExtractors.SelectedItem;
            newDataSettings.TableFile.RefID = tableRefObjectPicker.SelectedRefObject.ID;
            newDataSettings.TextPreview.RefID = textPreviewRefObjectPicker.SelectedRefObject.ID;

            newDataSettings.DataFile.Path = ProjectSettings.GetAbsolutePath($"{textName.Text}.wtd");

            CreatedObject = newDataSettings;

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
            if (_cachedDataExtractorTypes == null)
            {
                var query = from a in AppDomain.CurrentDomain.GetAssemblies()
                            from t in a.GetTypes()
                            where t.GetInterfaces().Contains(typeof(IDataExtractor))
                            select t;

                _cachedDataExtractorTypes = query.ToArray();
            }

            List<IDataExtractor> dataExtractors = new();

            foreach (var type in _cachedDataExtractorTypes)
            {
                dataExtractors.Add((IDataExtractor)Activator.CreateInstance(type));
            }
            dataExtractors.Sort((x, y) => x.Name.CompareTo(y.Name));

            comboDataExtractors.ItemsSource = dataExtractors;
        }
    }
}
