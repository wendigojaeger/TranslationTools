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
    public class BaseScriptSettingsEditor : BaseEditor<ScriptSettings>
    {
    }

    [EditorFor(typeof(ScriptSettings))]
    public partial class ScriptSettingsEditor : BaseScriptSettingsEditor
    {
        private static Type[] _cachedScriptExtractorTypes;

        public override string WindowTitle => Instance.Name;

        public ScriptSettingsEditor()
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

            Binding textExtractorBinding = new()
            {
                Source = Instance,
                Path = new PropertyPath(nameof(Instance.ScriptExtractor)),
                Mode = BindingMode.TwoWay,
            };

            comboScriptExtractors.SetBinding(ComboBox.SelectedItemProperty, textExtractorBinding);
            if (Instance.ScriptExtractor != null)
            {
                comboScriptExtractors.SelectedIndex = scriptExtractors.FindIndex(x => x.GetType().IsAssignableFrom(Instance.ScriptExtractor.GetType()));
            }

            listBoxPointerList.ItemsSource = Instance.ScriptFile.Instance.Pointers;

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

        private void listBoxPointerList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            listBoxPointerList.ToolTip = $"{listBoxPointerList.SelectedItems.Count} selected item(s)";
        }
    }
}
