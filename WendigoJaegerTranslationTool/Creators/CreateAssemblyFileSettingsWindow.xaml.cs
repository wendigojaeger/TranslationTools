using System;
using System.Windows;
using WendigoJaeger.TranslationTool.Data;

namespace WendigoJaeger.TranslationTool.Creators
{
    [ObjectCreator(typeof(AssemblyFileSettings))]
    public partial class CreateAssemblyFileSettingsWindow : Window, IObjectCreator
    {
        public object CreatedObject { get; set; }

        public ProjectSettings ProjectSettings { get; set; }

        public CreateAssemblyFileSettingsWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, EventArgs e)
        {
            filePathBrowser.ProjectSettings = ProjectSettings;
            filePathBrowser.Filter = Resource.filterAssemblyFile;
        }

        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            AssemblyFileSettings assemblyFileSettings = new AssemblyFileSettings
            {
                Name = textName.Text,
                FilePath = filePathBrowser.RelativePath
            };

            CreatedObject = assemblyFileSettings;

            DialogResult = true;
            Close();
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void textName_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            updateButtonOKEnabledState();
        }

        private void filePathBrowser_RelativePathChanged(object arg1, EventArgs arg2)
        {
            updateButtonOKEnabledState();
        }

        private void updateButtonOKEnabledState()
        {
            buttonOK.IsEnabled = !string.IsNullOrEmpty(textName.Text) && !string.IsNullOrEmpty(filePathBrowser.RelativePath);
        }
    }
}
