using System;
using System.Windows;
using System.Windows.Controls;
using WendigoJaeger.TranslationTool.Data;

namespace WendigoJaeger.TranslationTool.Creators
{
    [ObjectCreator(typeof(FontSettings))]
    public partial class CreateFontSettingsWindow : Window, IObjectCreator
    {
        public object CreatedObject { get; set; }

        public ProjectSettings ProjectSettings { get; set; }

        public CreateFontSettingsWindow()
        {
            InitializeComponent();

            graphicsBrowser.SelectedRefObjectChanged += graphicsBrowser_SelectedRefObjectChanged;
        }

        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            FontSettings newFontSettings = new FontSettings
            {
                Name = textName.Text,
            };

            newFontSettings.Graphics.RefID = graphicsBrowser.SelectedRefObject.ID;

            CreatedObject = newFontSettings;

            DialogResult = true;
            Close();
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void textName_TextChanged(object sender, TextChangedEventArgs e)
        {
            updateButtonOKEnabledState();
        }

        private void graphicsBrowser_SelectedRefObjectChanged(object arg1, EventArgs arg2)
        {
            updateButtonOKEnabledState();
        }

        private void updateButtonOKEnabledState()
        {
            buttonOK.IsEnabled = !string.IsNullOrEmpty(textName.Text) && graphicsBrowser.SelectedRefObject != null;
        }
    }
}
