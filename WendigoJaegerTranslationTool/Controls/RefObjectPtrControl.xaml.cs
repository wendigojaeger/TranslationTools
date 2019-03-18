using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using WendigoJaeger.TranslationTool.Windows;

namespace WendigoJaeger.TranslationTool.Controls
{
    public partial class RefObjectPtrControl : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public event Action<object, EventArgs> SelectedRefObjectChanged;

        public string ObjectName
        {
            get
            {
                if (SelectedRefObject != null)
                {
                    return SelectedRefObject.Name;
                }

                return string.Empty;
            }
            set
            {}
        }

        public static readonly DependencyProperty SelectedRefObjectProperty = DependencyProperty.Register(nameof(SelectedRefObject), typeof(RefObject), typeof(RefObjectPtrControl), new UIPropertyMetadata(null, onSelectedRefObjectChanged));
        public RefObject SelectedRefObject
        {
            get
            {
                return (RefObject)GetValue(SelectedRefObjectProperty);
            }
            set
            {
                SetValue(SelectedRefObjectProperty, value);

                SelectedRefObjectChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public static readonly DependencyProperty RefObjectTypeProperty = DependencyProperty.Register(nameof(RefObjectType), typeof(Type), typeof(RefObjectPtrControl));
        public Type RefObjectType
        {
            get
            {
                return (Type)GetValue(RefObjectTypeProperty);
            }
            set
            {
                SetValue(RefObjectTypeProperty, value);
            }
        }

        public RefObjectPtrControl()
        {
            InitializeComponent();
        }

        private void buttonBrowse_Click(object sender, RoutedEventArgs e)
        {
            RefObjectBrowserWindow browseWindow = new RefObjectBrowserWindow();
            browseWindow.RefObjectType = RefObjectType;

            var result = browseWindow.ShowDialog();
            if (result.HasValue && result.Value)
            {
                SelectedRefObject = browseWindow.SelectedRefObject;
            }
        }

        private static void onSelectedRefObjectChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            RefObjectPtrControl control = dependencyObject as RefObjectPtrControl;
            if (control != null)
            {
                control.notifyPropertyChanged(nameof(ObjectName));
            }
        }

        private void notifyPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
