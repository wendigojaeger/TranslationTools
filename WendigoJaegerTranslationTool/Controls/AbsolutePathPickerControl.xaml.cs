using Microsoft.Win32;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace WendigoJaeger.TranslationTool.Controls
{
    public partial class AbsolutePathPickerControl : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public static readonly DependencyProperty RelativePathProperty = DependencyProperty.Register(nameof(AbsolutePath), typeof(string), typeof(AbsolutePathPickerControl), new UIPropertyMetadata(null));
        public string AbsolutePath
        {
            get
            {
                return (string)GetValue(RelativePathProperty);
            }
            set
            {
                SetValue(RelativePathProperty, value);

                notifyPropertyChanged();
            }
        }

        public static readonly DependencyProperty FilterProperty = DependencyProperty.Register(nameof(Filter), typeof(string), typeof(AbsolutePathPickerControl), new UIPropertyMetadata("All Files (*.*)|*.*"));

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

        public AbsolutePathPickerControl()
        {
            InitializeComponent();
        }

        private void buttonBrowse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            if (!string.IsNullOrEmpty(Filter))
            {
                fileDialog.Filter = Filter;
            }

            var result = fileDialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                AbsolutePath = fileDialog.FileName;
            }
        }

        private void notifyPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
