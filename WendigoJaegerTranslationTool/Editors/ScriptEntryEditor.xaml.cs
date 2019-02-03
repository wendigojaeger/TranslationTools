using WendigoJaeger.TranslationTool.Data;
using System.Windows;
using System.Windows.Controls;

namespace WendigoJaeger.TranslationTool.Editors
{
    public class BaseScriptEditor : BaseEditor<ScriptEntry>
    {
    }

    [EditorFor(typeof(ScriptEntry))]
    public partial class ScriptEntryEditor : BaseScriptEditor
    {
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

            textOriginal.Text = Instance.Original;
            icTranslations.ItemsSource = Instance.Translations;

            updateStatusBar("Ln: 1 Col: 1");
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
    }
}
