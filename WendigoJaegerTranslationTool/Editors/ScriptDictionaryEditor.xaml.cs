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
using WendigoJaeger.TranslationTool.Data;
using WendigoJaeger.TranslationTool.Extractors;
using Xceed.Wpf.Toolkit;

namespace WendigoJaeger.TranslationTool.Editors
{
    public class BaseScriptDictionaryEditor : BaseEditor<ScriptDictionary>
    {
    }

    [EditorFor(typeof(ScriptDictionary))]
    public partial class ScriptDictionaryEditor : BaseScriptDictionaryEditor
    {
        private static Type[] _cachedScriptDictionaryExtractorTypes;

        public override string WindowTitle => Instance.Name;

        public ScriptDictionaryEditor()
        {
            InitializeComponent();
        }

        public override void Init()
        {
            Binding sourceRAMBinding = new()
            {
                Source = Instance,
                Path = new PropertyPath(nameof(Instance.RAMAddress)),
                Mode = BindingMode.TwoWay,
            };
            upDownRAMAddress.SetBinding(LongUpDown.ValueProperty, sourceRAMBinding);

            Binding entriesBinding = new()
            {
                Source = Instance,
                Path = new PropertyPath(nameof(Instance.Entries)),
                Mode = BindingMode.TwoWay,
            };
            upDownEntries.SetBinding(UIntegerUpDown.ValueProperty, entriesBinding);

            if (_cachedScriptDictionaryExtractorTypes == null)
            {
                var query = from a in AppDomain.CurrentDomain.GetAssemblies()
                            from t in a.GetTypes()
                            where t.GetInterfaces().Contains(typeof(IScriptDictionaryExtractor))
                            select t;

                _cachedScriptDictionaryExtractorTypes = query.ToArray();
            }

            List<IScriptDictionaryExtractor> scriptExtractors = new();

            foreach (var type in _cachedScriptDictionaryExtractorTypes)
            {
                scriptExtractors.Add((IScriptDictionaryExtractor)Activator.CreateInstance(type));
            }
            scriptExtractors.Sort((x, y) => x.Name.CompareTo(y.Name));

            comboExtractors.ItemsSource = scriptExtractors;

            Binding extractorBinding = new()
            {
                Source = Instance,
                Path = new PropertyPath(nameof(Instance.Extractor)),
                Mode = BindingMode.TwoWay,
            };

            comboExtractors.SetBinding(ComboBox.SelectedItemProperty, extractorBinding);
            if (Instance.Extractor != null)
            {
                comboExtractors.SelectedIndex = scriptExtractors.FindIndex(x => x.GetType().IsAssignableFrom(Instance.Extractor.GetType()));
            }
        }
    }
}
