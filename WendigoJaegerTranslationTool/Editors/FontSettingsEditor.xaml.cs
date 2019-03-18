using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WendigoJaeger.TranslationTool.Controls;
using WendigoJaeger.TranslationTool.Data;
using WendigoJaeger.TranslationTool.Graphics;

namespace WendigoJaeger.TranslationTool.Editors
{
    public class BaseFontSettingsEditor : BaseEditor<FontSettings>
    {
    }

    [EditorFor(typeof(FontSettings))]
    public partial class FontSettingsEditor : BaseFontSettingsEditor, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

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
                    return ProjectSettings.GetAbsolutePath(Instance.Graphics.Instance.Entries.First().Path);
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
            graphicsBinding.Source = Instance.Graphics;
            graphicsBinding.Path = new PropertyPath(nameof(Instance.Graphics.Instance));
            graphicsBinding.Mode = BindingMode.TwoWay;
            graphicsPicker.SetBinding(RefObjectPtrControl.SelectedRefObjectProperty, graphicsBinding);

            Binding paletteBinding = new Binding();
            paletteBinding.Source = Instance.Palette;
            paletteBinding.Path = new PropertyPath(nameof(Instance.Palette.Instance));
            paletteBinding.Mode = BindingMode.TwoWay;
            palettePicker.SetBinding(RefObjectPtrControl.SelectedRefObjectProperty, paletteBinding);

            fontEditor.FontSettings = Instance;
        }

        private void notifyPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
    }
}
