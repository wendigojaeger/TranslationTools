using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WendigoJaeger.TranslationTool.Converters;
using WendigoJaeger.TranslationTool.Data;
using WendigoJaeger.TranslationTool.Extensions;
using WendigoJaeger.TranslationTool.Graphics;

namespace WendigoJaeger.TranslationTool.Controls
{
    class FontEditorControl : Control, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private WriteableBitmap _fontBitmap = null;
        private static readonly float[] _availableZoomFactors = new float[] { 0.5f, 1f, 2f, 4f, 8f, 12f };

        public float[] AvailableZoomFactors
        {
            get
            {
                return _availableZoomFactors;
            }
            set { }
        }

        public static DependencyProperty FontPathProperty = DependencyProperty.Register(nameof(FontPath), typeof(string), typeof(FontEditorControl), new UIPropertyMetadata(null, refreshFontBitmap));
        public string FontPath
        {
            get
            {
                return (string)GetValue(FontPathProperty);
            }
            set
            {
                SetValue(FontPathProperty, value);
            }
        }

        public static DependencyProperty GraphicsReaderProperty = DependencyProperty.Register(nameof(GraphicsReader), typeof(IGraphicsReader), typeof(FontEditorControl), new UIPropertyMetadata(null, refreshFontBitmap));
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

        public static DependencyProperty FontSettingsProperty = DependencyProperty.Register(nameof(FontSettings), typeof(FontSettings), typeof(FontEditorControl), new UIPropertyMetadata(null, refreshFontBitmap));
        public FontSettings FontSettings
        {
            get
            {
                return (FontSettings)GetValue(FontSettingsProperty);
            }
            set
            {
                SetValue(FontSettingsProperty, value);
            }
        }

        public static readonly DependencyProperty SelectedZoomFactorProperty = DependencyProperty.Register(nameof(SelectedZoomFactor), typeof(int), typeof(FontEditorControl), new UIPropertyMetadata(3, onZoomChanged));
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

        public static readonly DependencyProperty ShowGridProperty = DependencyProperty.Register(nameof(ShowGrid), typeof(bool), typeof(FontEditorControl), new UIPropertyMetadata(true, refreshEditor));
        public bool ShowGrid
        {
            get
            {
                return (bool)GetValue(ShowGridProperty);
            }
            set
            {
                SetValue(ShowGridProperty, value);
            }
        }

        public static readonly DependencyProperty ShowByteValueProperty = DependencyProperty.Register(nameof(ShowByteValue), typeof(bool), typeof(FontEditorControl), new UIPropertyMetadata(true, refreshEditor));
        public bool ShowByteValue
        {
            get
            {
                return (bool)GetValue(ShowByteValueProperty);
            }
            set
            {
                SetValue(ShowByteValueProperty, value);
            }
        }

        public float ZoomFactor { get; private set; } = 4f;

        public FontEditorControl()
        {
            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.NearestNeighbor);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            var fontSettings = FontSettings;
            if (_fontBitmap != null && fontSettings != null)
            {
                drawingContext.DrawImage(_fontBitmap, new Rect(0, 0, _fontBitmap.Width * ZoomFactor, _fontBitmap.Height * ZoomFactor));

                if (ShowGrid || ShowByteValue)
                {
                    int widthCount = (int)_fontBitmap.Width / FontSettings.CharacterWidth;
                    int heightCount = (int)_fontBitmap.Height / FontSettings.CharacterHeight;

                    int currentByte = FontSettings.Offset;

                    Typeface typeface = new Typeface(FontFamily, FontStyle, FontWeight, FontStretch);

                    for (int y = 0; y < heightCount; ++y)
                    {
                        double finalY = y * FontSettings.CharacterHeight * ZoomFactor;

                        for (int x = 0; x < widthCount; ++x)
                        {
                            double finalX = x * FontSettings.CharacterWidth * ZoomFactor;

                            var boxRect = new Rect(finalX,
                                finalY,
                                FontSettings.CharacterWidth * ZoomFactor,
                                FontSettings.CharacterHeight * ZoomFactor
                                );


                            if (ShowGrid)
                            {
                                drawingContext.DrawRectangle(null, new Pen(Brushes.Yellow, 1f), boxRect);
                            }

                            if (ShowByteValue)
                            {
                                drawingContext.DrawRectangle(new SolidColorBrush(Color.FromArgb(100, 0, 0, 255)), null, boxRect);

                                FormattedText textInfo = new FormattedText(currentByte.ToString("X2"), CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, 6 + (2 * ZoomFactor), Brushes.White, 96.0);
                                drawingContext.DrawText(textInfo, new Point(finalX + (boxRect.Width / 2 - textInfo.Width / 2), finalY + (boxRect.Height / 2 - textInfo.Height / 2)));
                            }

                            ++currentByte;
                        }
                    }
                }
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (_fontBitmap != null )
            {
                return new Size(_fontBitmap.Width * ZoomFactor, _fontBitmap.Height * ZoomFactor);
            }

            return base.MeasureOverride(availableSize);
        }

        private static void refreshFontBitmap(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            FontEditorControl control = dependencyObject as FontEditorControl;

            if (control != null)
            {
                if (e.Property == FontSettingsProperty)
                {
                    if (control.FontSettings != null)
                    {
                        control.FontSettings.PropertyChanged -= control.FontSettings_PropertyChanged;
                        control.FontSettings.PropertyChanged += control.FontSettings_PropertyChanged;
                    }
                }

                if (!string.IsNullOrEmpty(control.FontPath)
                    && control.GraphicsReader != null
                    && control.FontSettings != null
                    )
                {
                    Color[] palette = new Color[4];
                    palette[0] = Color.FromArgb(255, 0, 0, 0);
                    palette[1] = Color.FromArgb(255, 0xF0, 0xF0, 0xF0);
                    palette[2] = Color.FromArgb(255, 0x88, 0xe8, 0xf0);
                    palette[3] = Color.FromArgb(255, 0x03, 0x15, 0x4e);

                    //var palette = control.FontSettings.Palette.Instance;
                    //if (palette != null)
                    //{
                    control._fontBitmap = TileGraphicsConverter.ConvertToWpfBitmap(control.FontPath, control.GraphicsReader, palette);

                    control.InvalidateVisual();
                    //}
                }
            }
        }

        private void FontSettings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            InvalidateVisual();
        }

        private static void refreshEditor(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            FontEditorControl control = dependencyObject as FontEditorControl;

            if (control != null)
            {
                control.InvalidateVisual();
            }
        }

        private static void onZoomChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            FontEditorControl control = source as FontEditorControl;
            if (control != null && (int)e.NewValue >= 0 && (int)e.NewValue < _availableZoomFactors.Length)
            {
                control.ZoomFactor = _availableZoomFactors[(int)e.NewValue];

                control.InvalidateMeasure();
                control.InvalidateVisual();
            }
        }

        private void notifyPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
