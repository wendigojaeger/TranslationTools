using System;
using System.Windows;

namespace WendigoJaeger.TranslationTool.Windows
{
    public partial class RefObjectBrowserWindow : Window
    {
        public Type RefObjectType { get; internal set; }

        public RefObject SelectedRefObject
        {
            get
            {
                return listBoxObjects.SelectedItem as RefObject;
            }
        }

        public RefObjectBrowserWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            listBoxObjects.ItemsSource = ReferenceDatabase.Instance.ListAll(RefObjectType);
        }

        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void listBoxObjects_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (listBoxObjects.SelectedItem != null)
            {
                buttonOK_Click(this, null);
            }
        }
    }
}
