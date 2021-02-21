using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WendigoJaeger.TranslationTool.Converters;
using WendigoJaeger.TranslationTool.Data;
using WendigoJaeger.TranslationTool.Extensions;

namespace WendigoJaeger.TranslationTool.Controls
{
    class TextPreviewRenderControl : Control, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private WriteableBitmap _fontBitmap = null;
        private WriteableBitmap _textBitmap = null;
        private CorrespondenceTable _correspondence;

        private static readonly float[] _availableZoomFactors = new float[] { 0.5f, 1f, 2f, 4f, 8f, 12f };

        public float[] AvailableZoomFactors
        {
            get
            {
                return _availableZoomFactors;
            }
            set { }
        }

        public static DependencyProperty TextPreviewProperty = DependencyProperty.Register(nameof(TextPreviewInfo), typeof(TextPreviewInfo), typeof(TextPreviewRenderControl), new UIPropertyMetadata(null, onTextPreviewChanged));
        public TextPreviewInfo TextPreview
        {
            get
            {
                return (TextPreviewInfo)GetValue(TextPreviewProperty);
            }
            set
            {
                SetValue(TextPreviewProperty, value);

                notifyPropertyChanged();
            }
        }

        public static readonly DependencyProperty SelectedZoomFactorProperty = DependencyProperty.Register(nameof(SelectedZoomFactor), typeof(int), typeof(TextPreviewRenderControl), new UIPropertyMetadata(3, onZoomChanged));
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

        public static readonly DependencyProperty CurrentLocaleProperty = DependencyProperty.Register(nameof(CurrentLocale), typeof(string), typeof(TextPreviewRenderControl), new UIPropertyMetadata(null, onTableChanged));
        public string CurrentLocale
        {
            get
            {
                return (string)GetValue(CurrentLocaleProperty);
            }
            set
            {
                SetValue(CurrentLocaleProperty, value);

                notifyPropertyChanged();
            }
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(TextPreviewRenderControl), new UIPropertyMetadata(null, onTextChanged));
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

        public static readonly DependencyProperty TableProperty = DependencyProperty.Register(nameof(Table), typeof(TableFile), typeof(TextPreviewRenderControl), new UIPropertyMetadata(null, onTableChanged));
        public TableFile Table
        {
            get
            {
                return (TableFile)GetValue(TableProperty);
            }
            set
            {
                SetValue(TableProperty, value);

                notifyPropertyChanged();
            }
        }

        public float ZoomFactor { get; private set; } = 4f;

        public ProjectSettings ProjectSettings { get; set; }

        public TextPreviewRenderControl()
        {
            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.NearestNeighbor);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            if (_textBitmap != null)
            {
                drawingContext.DrawImage(_textBitmap, new Rect(0, 0, _textBitmap.Width * ZoomFactor, _textBitmap.Height * ZoomFactor));
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            var textPreview = TextPreview;

            if (textPreview != null)
            {
                var font = textPreview.Font.Instance;
                if (font != null)
                {
                    var width = font.CharacterWidth * textPreview.MaxPerLine;
                    var height = font.CharacterHeight * textPreview.MaxLines;

                    return new Size(width * ZoomFactor, height * ZoomFactor);
                }
            }

            return base.MeasureOverride(availableSize);
        }

        private static void onTextChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            TextPreviewRenderControl control = source as TextPreviewRenderControl;
            if (control != null)
            {
                control.updateTextBitmap();

                control.InvalidateVisual();
            }
        }

        private static void onTextPreviewChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            TextPreviewRenderControl control = source as TextPreviewRenderControl;
            if (control != null && control.ProjectSettings != null && !string.IsNullOrEmpty(control.CurrentLocale))
            {
                var textPreview = control.TextPreview;
                if (textPreview != null)
                {
                    var font = textPreview.Font.Instance;
                    if (font != null)
                    {
                        var graphics = font.Graphics.Instance;
                        if (graphics != null)
                        {
                            var fontPath = control.ProjectSettings.GetAbsolutePath(graphics.GetGraphicsPath(control.CurrentLocale));

                            var palette = font.Palette.Instance;
                            if (palette != null)
                            {
                                control._fontBitmap = TileGraphicsConverter.ConvertToWpfBitmap(fontPath, graphics.GraphicsReader, palette.ToWpfColorArray());

                                onTextChanged(source, e);
                            }
                        }
                    }
                }

                control.InvalidateMeasure();
                control.InvalidateVisual();
            }
        }

        private static void onZoomChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            TextPreviewRenderControl control = source as TextPreviewRenderControl;
            if (control != null && (int)e.NewValue >= 0 && (int)e.NewValue < _availableZoomFactors.Length)
            {
                control.ZoomFactor = _availableZoomFactors[(int)e.NewValue];

                control.InvalidateMeasure();
                control.InvalidateVisual();
            }
        }

        private static void onTableChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            TextPreviewRenderControl control = source as TextPreviewRenderControl;
            if (control != null)
            {
                control.updateCorrespondaceTable();

                onTextPreviewChanged(source, e);
            }
        }

        private void updateCorrespondaceTable()
        {
            if (Table == null)
            {
                return;
            }
            if (ProjectSettings == null)
            {
                return;
            }

            _correspondence = new CorrespondenceTable();

            string tblPath = ProjectSettings.GetAbsolutePath(Table.GetTablePath(CurrentLocale));
            _correspondence.Parse(ProjectSettings.Project.System.Endianess, tblPath);

            for (int i = 0; i < 0xFF; ++i)
            {
                _correspondence.StringToBytes.Insert($"<{i:x2}>", Array.Empty<byte>());
            }

            if (Table.NewLine.HasValue)
            {
                _correspondence.StringToBytes.Insert("\n", new byte[] { Table.NewLine.Value });
            }
        }

        private void updateTextBitmap()
        { 
            if (_fontBitmap == null)
            {
                return;
            }

            if (_correspondence == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(Text))
            {
                return;
            }

            var textPreview = TextPreview;

            if (textPreview != null)
            {
                var font = textPreview.Font.Instance;
                if (font != null)
                {
                    var palette = font.Palette.Instance;
                    if (palette != null)
                    {
                        var width = font.CharacterWidth * textPreview.MaxPerLine;
                        var height = font.CharacterHeight * textPreview.MaxLines;

                        _textBitmap = BitmapFactory.New(width, height);
                        _textBitmap.Lock();
                        _textBitmap.Clear(palette.Entries[0].ToWpfColor());

                        List<byte> scriptGameData = convertTextToGameData(Text);

                        int destX = 0;
                        int destY = 0;

                        int numberOfLines = 0;

                        int fontBitmapWidthInTiles = (int)(_fontBitmap.Width / font.CharacterWidth);
                        int fontBitmapHeightInTiles = (int)(_fontBitmap.Height / font.CharacterHeight);

                        for (int index = 0; index < scriptGameData.Count; ++index)
                        {
                            if (Table.NewLine.HasValue && scriptGameData[index] == Table.NewLine.Value)
                            {
                                destX = 0;

                                ++numberOfLines;
                                if (numberOfLines >= textPreview.MaxPerLine)
                                {
                                    break;
                                }

                                destY = numberOfLines * font.CharacterHeight;

                                continue;
                            }

                            var graphicsIndex = scriptGameData[index] - font.Offset;

                            int sourceX = graphicsIndex % fontBitmapWidthInTiles;
                            int sourceY = graphicsIndex / fontBitmapHeightInTiles;

                            _textBitmap.Blit(new Rect(destX, destY, font.CharacterWidth, font.CharacterHeight), _fontBitmap, new Rect(sourceX * font.CharacterWidth, sourceY * font.CharacterHeight, font.CharacterWidth, font.CharacterHeight));

                            destX += font.CharacterWidth;

                            if (destX >= width)
                            {
                                destX = 0;
                                destY += font.CharacterHeight;
                            }
                        }

                        _textBitmap.Unlock();
                    }
                }
            }
        }

        private List<byte> convertTextToGameData(string text)
        {
            List<byte> data = new List<byte>();

            TrieNode<char, byte[]> stringToByteNode = _correspondence.StringToBytes.Root;

            Stack<Tuple<int, TrieNode<char, byte[]>>> recognizeStack = new Stack<Tuple<int, TrieNode<char, byte[]>>>();

            int index = 0;
            while (index < text.Length)
            {
                stringToByteNode = stringToByteNode.Find(text[index]);

                if (stringToByteNode != null)
                {
                    if (stringToByteNode.IsLeaf)
                    {
                        data.AddRange(stringToByteNode.Value);

                        stringToByteNode = _correspondence.StringToBytes.Root;

                        recognizeStack.Clear();
                    }
                    else
                    {
                        recognizeStack.Push(Tuple.Create(index, stringToByteNode));
                    }
                }
                else
                {
                    while (recognizeStack.Count > 0)
                    {
                        var currentTuple = recognizeStack.Pop();

                        if (currentTuple.Item2.Value != null)
                        {
                            data.AddRange(currentTuple.Item2.Value);

                            stringToByteNode = _correspondence.StringToBytes.Root;

                            index = currentTuple.Item1;

                            recognizeStack.Clear();
                            break;
                        }
                    }
                }

                if (stringToByteNode == null)
                {
                    stringToByteNode = _correspondence.StringToBytes.Root;
                }

                index++;
            }

            return data;
        }

        private void notifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
