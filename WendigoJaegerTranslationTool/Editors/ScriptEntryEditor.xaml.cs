using WendigoJaeger.TranslationTool.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using WendigoJaeger.TranslationTool.Controls;

namespace WendigoJaeger.TranslationTool.Editors
{
    public class BaseScriptEditor : BaseEditor<ScriptEntry>
    {
    }

    [EditorFor(typeof(ScriptEntry))]
    public partial class ScriptEntryEditor : BaseScriptEditor
    {
        public override string WindowTitle => Instance.EntryName;

        public ScriptEntryEditor()
        {
            InitializeComponent();
        }

        public override void Init()
        {
            base.Init();

            foreach (var langSettings in ProjectSettings.Project.Lang)
            {
                if (!Instance.HasTranslation(langSettings.Key))
                {
                    Instance.Translations.Add(new TranslationEntry { Lang = langSettings.Key, Value = Instance.Original });
                }
            }

            Binding textPreviewRefBinding = new Binding();
            textPreviewRefBinding.Source = Instance;
            textPreviewRefBinding.Path = new PropertyPath(nameof(Instance.TextPreview));
            textPreviewRefBinding.Mode = BindingMode.TwoWay;
            textPreviewRefControl.SetBinding(RefObjectPtrControl.RefObjectPtrProperty, textPreviewRefBinding);

            var table = FindTableFile();
            var textPreview = FindPreviewInfo();

            textOriginal.Text = Instance.Original;

            originalTextPreview.ProjectSettings = ProjectSettings;
            originalTextPreview.Table = table;
            originalTextPreview.TextPreview = textPreview;
            originalTextPreview.Text = Instance.Original;
            originalTextPreview.CurrentLocale = "Default";

            translatedTextPreview.ProjectSettings = ProjectSettings;
            translatedTextPreview.Table = table;
            translatedTextPreview.TextPreview = textPreview;

            onCurrentLocaleChanged(CurrentLocale);

            Instance.PropertyChanged -= updateWindowTitle;
            Instance.PropertyChanged += updateWindowTitle;

            updateStatusBar("Ln: 1 Col: 1");
        }

        private TextPreviewInfo FindPreviewInfo()
        {
            foreach (var script in ProjectSettings.Scripts)
            {
                foreach (var entry in script.Script.Instance.Entries)
                {
                    if (entry == Instance)
                    {
                        return entry.TextPreview.Instance != null ? entry.TextPreview.Instance : script.TextPreview.Instance;
                    }
                }
            }

            return null;
        }

        private TableFile FindTableFile()
        {
            foreach (var script in ProjectSettings.Scripts)
            {
                if (script.Script.Instance.Entries.Contains(Instance))
                {
                    return script.TableFile.Instance;
                }
            }

            return null;
        }

        private void updateWindowTitle(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Instance.EntryName))
            {
                refreshWindowTitle();
            }
        }

        private void TextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null)
            {
                int line = textBox.GetLineIndexFromCharacterIndex(textBox.CaretIndex);
                if (line >= 0)
                {
                    int column = textBox.CaretIndex - textBox.GetCharacterIndexFromLineIndex(line);

                    updateStatusBar($"Ln: {line + 1}, Col: {column + 1}");
                }
            }
        }

        protected override void onCurrentLocaleChanged(string newLocale)
        {
            var newEntry = Instance.GetTranslation(newLocale);

            imageFlag.DataContext = newEntry;
            comboEntryState.DataContext = newEntry;
            textTranslatedEntry.DataContext = newEntry;

            translatedTextPreview.DataContext = newEntry;
            translatedTextPreview.CurrentLocale = newLocale;
        }

        private void textPreviewRefControl_SelectedRefObjectChanged(object sender, System.EventArgs args)
        {
            var newTextPreview = FindPreviewInfo();

            originalTextPreview.TextPreview = newTextPreview;
            translatedTextPreview.TextPreview = newTextPreview;
        }
    }
}
