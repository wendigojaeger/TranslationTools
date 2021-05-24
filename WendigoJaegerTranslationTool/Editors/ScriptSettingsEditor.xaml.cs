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
        static Type[] _cachedTextExtractorTypes = null;

        public override string WindowTitle => Instance.Name;

        public ScriptSettingsEditor()
        {
            InitializeComponent();
        }

        public override void Init()
        {
            DataContext = Instance;

            Binding tableFileBinding = new Binding
            {
                Source = Instance,
                Path = new PropertyPath(nameof(Instance.TableFile)),
                Mode = BindingMode.TwoWay
            };
            tableFilePicker.SetBinding(RefObjectPtrControl.RefObjectPtrProperty, tableFileBinding);

            Binding textPreviewBinding = new Binding
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

            if (_cachedTextExtractorTypes == null)
            {
                var query = from a in AppDomain.CurrentDomain.GetAssemblies()
                            from t in a.GetTypes()
                            where t.GetInterfaces().Contains(typeof(IScriptExtractor))
                            select t;

                _cachedTextExtractorTypes = query.ToArray();
            }

            List<IScriptExtractor> textExtractors = new ();

            foreach (var type in _cachedTextExtractorTypes)
            {
                textExtractors.Add((IScriptExtractor)Activator.CreateInstance(type));
            }
            textExtractors.Sort((x, y) => x.Name.CompareTo(y.Name));

            comboTextExtractors.ItemsSource = textExtractors;

            Binding textExtractorBinding = new()
            {
                Source = Instance,
                Path = new PropertyPath(nameof(Instance.TextExtractor)),
                Mode = BindingMode.TwoWay,
            };

            comboTextExtractors.SetBinding(ComboBox.SelectedItemProperty, textExtractorBinding);
            comboTextExtractors.SelectedIndex = textExtractors.FindIndex(x => x.GetType().IsAssignableFrom(Instance.TextExtractor.GetType()));

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
