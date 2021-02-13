using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using WendigoJaeger.TranslationTool.Controls;
using WendigoJaeger.TranslationTool.Data;
using WendigoJaeger.TranslationTool.Graphics;

namespace WendigoJaeger.TranslationTool.Editors
{
    public class BaseFontSettingsEditor : BaseEditor<FontSettings>
    {
    }

    [EditorFor(typeof(FontSettings))]
    public partial class FontSettingsEditor : BaseFontSettingsEditor
    {
        public override string WindowTitle => Instance.Name;

        public IGraphicsReader GraphicsReader
        {
            get
            {
                if (Instance != null)
                {
                    return Instance.Graphics.Instance.GraphicsReader;
                }

                return null;
            }
            set
            {
            }
        }

        public string GraphicsPath
        {
            get
            {
                if (Instance != null)
                {
                    return ProjectSettings.GetAbsolutePath(Instance.Graphics.Instance.GetEntry(CurrentLocale).Path);
                }

                return string.Empty;
            }
            set
            {
            }
        }

        public FontSettingsEditor()
        {
            InitializeComponent();
        }

        public override void Init()
        {
            DataContext = Instance;

            notifyPropertyChanged(nameof(GraphicsPath));
            notifyPropertyChanged(nameof(GraphicsReader));

            Binding graphicsBinding = new Binding();
            graphicsBinding.Source = Instance;
            graphicsBinding.Path = new PropertyPath(nameof(Instance.Graphics));
            graphicsBinding.Mode = BindingMode.TwoWay;
            graphicsPicker.SetBinding(RefObjectPtrControl.RefObjectPtrProperty, graphicsBinding);

            Binding paletteBinding = new Binding();
            paletteBinding.Source = Instance;
            paletteBinding.Path = new PropertyPath(nameof(Instance.Palette));
            paletteBinding.Mode = BindingMode.TwoWay;
            palettePicker.SetBinding(RefObjectPtrControl.RefObjectPtrProperty, paletteBinding);

            fontEditor.FontSettings = Instance;

            Instance.PropertyChanged -= updateWindowTitle;
            Instance.PropertyChanged += updateWindowTitle;
        }

        private void updateWindowTitle(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Instance.Name))
            {
                refreshWindowTitle();
            }
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                if (e.Delta > 0)
                {
                    fontEditor.SelectedZoomFactor = Math.Min(fontEditor.SelectedZoomFactor + 1, fontEditor.AvailableZoomFactors.Length - 1);
                }
                else if (e.Delta < 0)
                {
                    fontEditor.SelectedZoomFactor = Math.Max(fontEditor.SelectedZoomFactor - 1, 0);
                }
            }
        }

        protected override void onCurrentLocaleChanged(string newLocale)
        {
            notifyPropertyChanged(nameof(GraphicsPath));
        }
    }
}
