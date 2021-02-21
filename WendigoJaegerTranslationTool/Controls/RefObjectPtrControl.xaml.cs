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
                else if (RefObjectPtr != null)
                {
                    return RefObjectPtr.ObjectName;
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

        public static readonly DependencyProperty RefObjectPtrProperty = DependencyProperty.Register(nameof(RefObjectPtr), typeof(IRefObjectPtr), typeof(RefObjectPtrControl), new UIPropertyMetadata(null, onRefObjectPtrChanged));
        public IRefObjectPtr RefObjectPtr
        {
            get
            {
                return (IRefObjectPtr)GetValue(RefObjectPtrProperty);
            }
            set
            {
                SetValue(SelectedRefObjectProperty, value);
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
                if (RefObjectPtr != null)
                {
                    RefObjectPtr.RefID = browseWindow.SelectedRefObject.ID;
                }
                else
                {
                    SelectedRefObject = browseWindow.SelectedRefObject;
                }

                SelectedRefObjectChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private void buttonClear_Click(object sender, RoutedEventArgs e)
        {
            if (RefObjectPtr != null)
            {
                RefObjectPtr.RefID = Guid.Empty;
            }
            else
            {
                SelectedRefObject = null;
            }

            SelectedRefObjectChanged?.Invoke(this, EventArgs.Empty);
        }

        private static void onSelectedRefObjectChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            RefObjectPtrControl control = dependencyObject as RefObjectPtrControl;
            if (control != null)
            {
                control.refreshObjectName();
            }
        }

        private static void onRefObjectPtrChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            RefObjectPtrControl control = dependencyObject as RefObjectPtrControl;
            if (control != null)
            {
                IRefObjectPtr target = e.NewValue as IRefObjectPtr;
                if (target != null)
                {
                    target.PropertyChanged -= control.onRefIDChanged;
                    target.PropertyChanged += control.onRefIDChanged;
                }

                control.refreshObjectName();
            }
        }

        private void onRefIDChanged(object sender, PropertyChangedEventArgs e)
        {
            refreshObjectName();
        }

        private void refreshObjectName()
        {
            notifyPropertyChanged(nameof(ObjectName));
        }

        private void notifyPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
