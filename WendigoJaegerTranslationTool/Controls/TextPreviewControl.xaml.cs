using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WendigoJaeger.TranslationTool.Data;

namespace WendigoJaeger.TranslationTool.Controls
{
    public partial class TextPreviewControl : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public TextPreviewInfo TextPreview
        {
            get
            {
                return textPreviewRender.TextPreview;
            }
            set
            {
                textPreviewRender.TextPreview = value;

                notifyPropertyChanged();
            }
        }

        public string CurrentLocale
        {
            get
            {
                return textPreviewRender.CurrentLocale;
            }
            set
            {
                textPreviewRender.CurrentLocale = value;

                notifyPropertyChanged();
            }
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(TextPreviewControl), new UIPropertyMetadata(null, onTextChanged));
        public string Text
        {
            get
            {
                return (string)GetValue(TextProperty);
            }
            set
            {
                SetValue(TextProperty, value);

                notifyPropertyChanged();
            }
        }

        public TableFile Table
        {
            get
            {
                return textPreviewRender.Table;
            }
            set
            {
                textPreviewRender.Table = value;

                notifyPropertyChanged();
            }
        }
        public ProjectSettings ProjectSettings
        {
            get
            {
                return textPreviewRender.ProjectSettings;
            }
            set
            {
                textPreviewRender.ProjectSettings = value;

                notifyPropertyChanged();
            }
        }

        public TextPreviewControl()
        {
            InitializeComponent();
        }

        private static void onTextChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            TextPreviewControl control = source as TextPreviewControl;
            if (control != null)
            {
                control.textPreviewRender.Text = e.NewValue as string;
            }
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                if (e.Delta > 0)
                {
                    textPreviewRender.SelectedZoomFactor = Math.Min(textPreviewRender.SelectedZoomFactor + 1, textPreviewRender.AvailableZoomFactors.Length - 1);
                }
                else if (e.Delta < 0)
                {
                    textPreviewRender.SelectedZoomFactor = Math.Max(textPreviewRender.SelectedZoomFactor - 1, 0);
                }
            }
        }

        private void buttonFirst_Click(object sender, RoutedEventArgs e)
        {
            textPreviewRender.CurrentWindow = 0;
        }

        private void buttonPrevious_Click(object sender, RoutedEventArgs e)
        {
            textPreviewRender.CurrentWindow -= 1;
        }

        private void buttonNext_Click(object sender, RoutedEventArgs e)
        {
            textPreviewRender.CurrentWindow += 1;
        }

        private void buttonLast_Click(object sender, RoutedEventArgs e)
        {
            textPreviewRender.CurrentWindow = textPreviewRender.MaxWindow;
        }

        private void notifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
