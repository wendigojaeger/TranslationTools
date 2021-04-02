using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using WendigoJaeger.TranslationTool.Data;

namespace WendigoJaeger.TranslationTool.Editors
{
    public class BaseDataEditor : BaseEditor<DataEntry>
    {
    }

    [EditorFor(typeof(DataEntry))]
    public partial class DataEntryEditor : BaseDataEditor
    {
        public override string WindowTitle => Instance.EntryName;

        public IEnumerable<string> OtherTranslations
        {
            get
            {
                if (Instance != null)
                {
                    foreach (var translation in Instance.Translations)
                    {
                        if (translation.Lang != CurrentLocale)
                        {
                            yield return translation.Lang;
                        }
                    }
                }
                else
                {
                    yield return null;
                }
            }
        }

        public DataEntryEditor()
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

            Binding otherTranslationItemsSource = new()
            {
                Source = this,
                Path = new PropertyPath(nameof(OtherTranslations)),
            };
            contextMenuItemCopyTranslation.SetBinding(ItemsControl.ItemsSourceProperty, otherTranslationItemsSource);

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
            foreach (var dataSettings in ProjectSettings.DataSettings)
            {
                foreach (var entry in dataSettings.DataFile.Instance.DataEntries)
                {
                    if (entry == Instance)
                    {
                        return dataSettings.TextPreview.Instance;
                    }
                }
            }

            return null;
        }

        private TableFile FindTableFile()
        {
            foreach (var dataSettings in ProjectSettings.DataSettings)
            {
                foreach (var entry in dataSettings.DataFile.Instance.DataEntries)
                {
                    if (entry == Instance)
                    {
                        return dataSettings.TableFile.Instance;
                    }
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

                    var selectionLength = textBox.SelectedText.Length;

                    updateStatusBar($"Ln: {line + 1}, Col: {column + 1} ({selectionLength} / 0x{selectionLength:x})");
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

            var binding = contextMenuItemCopyTranslation.GetBindingExpression(ItemsControl.ItemsSourceProperty);
            if (binding != null)
            {
                binding.UpdateTarget();
            }
        }

        private void textPreviewRefControl_SelectedRefObjectChanged(object sender, System.EventArgs args)
        {
            var newTextPreview = FindPreviewInfo();

            originalTextPreview.TextPreview = newTextPreview;
            translatedTextPreview.TextPreview = newTextPreview;
        }

        private void contextMenuItemCopyTranslation_Click(object sender, RoutedEventArgs e)
        {
            MenuItem clickedMenuItem = e.OriginalSource as MenuItem;

            string sourceLang = clickedMenuItem.Header.ToString();

            translatedTextPreview.Text = Instance.GetTranslation(sourceLang).Value;
        }
    }
}
