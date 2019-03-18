using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WendigoJaeger.TranslationTool.Converters;
using WendigoJaeger.TranslationTool.Data;
using WendigoJaeger.TranslationTool.Graphics;

namespace WendigoJaeger.TranslationTool.Controls
{
    public partial class GraphicsPreviewControl : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private WriteableBitmap _graphicsSource = null;
        private static readonly float[] _availableZoomFactors = new float[] { 0.5f, 1f, 2f, 4f, 8f, 12f };

        public static readonly DependencyProperty ImageRelativePathProperty = DependencyProperty.Register(nameof(ImageRelativePath), typeof(string), typeof(GraphicsPreviewControl), new UIPropertyMetadata(null, onRefreshImage));
        public string ImageRelativePath
        {
            get
            {
                return (string)GetValue(ImageRelativePathProperty);
            }
            set
            {
                SetValue(ImageRelativePathProperty, value);
            }
        }

        public static readonly DependencyProperty GraphicsReaderProperty = DependencyProperty.Register(nameof(GraphicsReader), typeof(IGraphicsReader), typeof(GraphicsPreviewControl), new UIPropertyMetadata(null, onRefreshImage));
        public IGraphicsReader GraphicsReader
        {
            get
            {
                return (IGraphicsReader)GetValue(GraphicsReaderProperty);
            }
            set
            {
                SetValue(GraphicsReaderProperty, value);
            }
        }

        public static readonly DependencyProperty ProjectSettingsProperty = DependencyProperty.Register(nameof(ProjectSettings), typeof(ProjectSettings), typeof(GraphicsPreviewControl), new UIPropertyMetadata(null, onRefreshImage));
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

        public static readonly DependencyProperty SelectedZoomFactorProperty = DependencyProperty.Register(nameof(SelectedZoomFactor), typeof(int), typeof(GraphicsPreviewControl), new UIPropertyMetadata(0, onZoomChanged));
        public int SelectedZoomFactor
        {
            get
            {
                return (int)GetValue(SelectedZoomFactorProperty);
            }
            set
            {
                SetValue(SelectedZoomFactorProperty, value);

                notifyPropertyChanged();
            }
        }

        public GraphicsPreviewControl()
        {
            InitializeComponent();
        }

        private void GraphicsPreviewControl_Loaded(object sender, RoutedEventArgs e)
        {
            comboZoom.ItemsSource = _availableZoomFactors;
            SelectedZoomFactor = 3;
        }

        private static void onRefreshImage(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            GraphicsPreviewControl control = source as GraphicsPreviewControl;
            if (control != null
                && control.ProjectSettings != null
                && control.GraphicsReader != null
                && !string.IsNullOrEmpty(control.ImageRelativePath)
                )
            {
                control.drawPreview();
            }
        }

        private static void onZoomChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            GraphicsPreviewControl control = source as GraphicsPreviewControl;
            if (control != null && (int)e.NewValue >= 0 && (int)e.NewValue < _availableZoomFactors.Length)
            {
                control.imageTarget.LayoutTransform = new ScaleTransform(_availableZoomFactors[(int)e.NewValue], _availableZoomFactors[(int)e.NewValue]);
            }
        }

        private void drawPreview()
        {
            // TODO: Define palette
            Color[] palette = new Color[4];
            palette[0] = Color.FromArgb(255, 0, 0, 0);
            palette[1] = Color.FromArgb(255, 64, 64, 64);
            palette[2] = Color.FromArgb(255, 128, 128, 128);
            palette[3] = Color.FromArgb(255, 255, 255, 255);

            _graphicsSource = TileGraphicsConverter.ConvertToWpfBitmap(ProjectSettings.GetAbsolutePath(ImageRelativePath), GraphicsReader, palette);

            if (_graphicsSource != null)
            {
                imageTarget.Width = _graphicsSource.Width;
                imageTarget.Height = _graphicsSource.Height;
                imageTarget.Source = _graphicsSource;
            }
        }

        private void notifyPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                if (e.Delta < 0)
                {
                    SelectedZoomFactor = Math.Max(SelectedZoomFactor - 1, 0);
                }
                else if (e.Delta > 1)
                {
                    SelectedZoomFactor = Math.Min(SelectedZoomFactor + 1, _availableZoomFactors.Length - 1);
                }

                e.Handled = true;
            }
        }
    }
}
