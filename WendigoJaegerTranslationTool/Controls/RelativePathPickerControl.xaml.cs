using WendigoJaeger.TranslationTool.Data;
using Microsoft.Win32;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System;

namespace WendigoJaeger.TranslationTool.Controls
{
    public partial class RelativePathPickerControl : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public event Action<object, EventArgs> RelativePathChanged;

        public static readonly DependencyProperty RelativePathProperty = DependencyProperty.Register(nameof(RelativePath), typeof(string), typeof(RelativePathPickerControl), new UIPropertyMetadata(null));
        public string RelativePath
        {
            get
            {
                return (string)GetValue(RelativePathProperty);
            }
            set
            {
                SetValue(RelativePathProperty, value);

                notifyPropertyChanged();

                RelativePathChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public static readonly DependencyProperty ProjectSettingsProperty = DependencyProperty.Register(nameof(ProjectSettings), typeof(ProjectSettings), typeof(RelativePathPickerControl));
        public ProjectSettings ProjectSettings
        {
            get
            {
                return (ProjectSettings)GetValue(ProjectSettingsProperty);
            }
            set
            {
                SetValue(ProjectSettingsProperty, value);
            }
        }

        public static readonly DependencyProperty FilterProperty = DependencyProperty.Register(nameof(Filter), typeof(string), typeof(RelativePathPickerControl), new UIPropertyMetadata("All Files (*.*)|*.*"));

        public string Filter
        {
            get
            {
                return (string)GetValue(FilterProperty);
            }
            set
            {
                SetValue(FilterProperty, value);
            }
        }

        public RelativePathPickerControl()
        {
            InitializeComponent();
        }

        private void buttonBrowse_Click(object sender, RoutedEventArgs e)
        {
            if (ProjectSettings != null)
            {
                OpenFileDialog fileDialog = new OpenFileDialog();
                if (!string.IsNullOrEmpty(Filter))
                {
                    fileDialog.Filter = Filter;
                }

                var result = fileDialog.ShowDialog();
                if (result.HasValue && result.Value)
                {
                    string relativePath = ProjectSettings.GetRelativePath(fileDialog.FileName);
                    RelativePath = relativePath;
                }
            }
        }

        private void notifyPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
