using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using WendigoJaeger.TranslationTool.Controls;
using WendigoJaeger.TranslationTool.Data;
using WendigoJaeger.TranslationTool.Extractors;
using Xceed.Wpf.Toolkit;

namespace WendigoJaeger.TranslationTool.Editors
{
    public class BaseDataSettingsEditor : BaseEditor<DataSettings>
    {
    }

    [EditorFor(typeof(DataSettings))]
    public partial class DataSettingsEditor : BaseDataSettingsEditor
    {
        static Type[] _cachedDataExtractorTypes = null;

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

            Binding sourceRAMBinding = new()
            {
                Source = Instance,
                Path = new PropertyPath(nameof(Instance.SourceRAMAddress)),
                Mode = BindingMode.TwoWay,
            };
            upDownSourceRAM.SetBinding(LongUpDown.ValueProperty, sourceRAMBinding);

            Binding destinationRAMBinding = new()
            {
                Source = Instance,
                Path = new PropertyPath(nameof(Instance.DestinationRAMAddress)),
                Mode = BindingMode.TwoWay,
            };
            upDownDestinationRAM.SetBinding(LongUpDown.ValueProperty, destinationRAMBinding);

            Binding destinationRAMEndBinding = new()
            {
                Source = Instance,
                Path = new PropertyPath(nameof(Instance.DestinationEndRAMAddress)),
                Mode = BindingMode.TwoWay,
            };
            upDownDestinationRAMEnd.SetBinding(LongUpDown.ValueProperty, destinationRAMEndBinding);

            Binding entriesBinding = new()
            {
                Source = Instance,
                Path = new PropertyPath(nameof(Instance.Entries)),
                Mode = BindingMode.TwoWay,
            };
            upDownEntries.SetBinding(UIntegerUpDown.ValueProperty, entriesBinding);

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

            Binding textExtractorBinding = new()
            {
                Source = Instance,
                Path = new PropertyPath(nameof(Instance.DataExtractor)),
                Mode = BindingMode.TwoWay,
            };

            comboDataExtractors.SetBinding(ComboBox.SelectedItemProperty, textExtractorBinding);
            comboDataExtractors.SelectedIndex = dataExtractors.FindIndex(x => x.GetType().IsAssignableFrom(Instance.DataExtractor.GetType()));

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
