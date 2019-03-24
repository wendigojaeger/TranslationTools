using System;
using System.Collections.Generic;
using System.Linq;
using WendigoJaeger.TranslationTool.Data;
using WendigoJaeger.TranslationTool.Graphics;

namespace WendigoJaeger.TranslationTool.Editors
{
    public class BaseGraphicsEditor : BaseEditor<GraphicsSettings>
    {
    }

    [EditorFor(typeof(GraphicsSettings))]
    public partial class GraphicsEditor : BaseGraphicsEditor
    {
        public override string WindowTitle => Instance.Name;

        static Type[] _cachedGfxDecoderTypes = null;

        //WriteableBitmap _textPreview;

        public GraphicsEditor()
        {
            InitializeComponent();
        }

        public override void Init()
        {
            base.Init();

            Instance.SyncLanguages(ProjectSettings.Project);

            if (_cachedGfxDecoderTypes == null)
            {
                var query = from a in AppDomain.CurrentDomain.GetAssemblies()
                            from t in a.GetTypes()
                            where t.GetInterfaces().Contains(typeof(IGraphicsReader))
                            select t;

                _cachedGfxDecoderTypes = query.ToArray();
            }

            List<IGraphicsReader> gfxDecoders = new List<IGraphicsReader>();

            foreach (var type in _cachedGfxDecoderTypes)
            {
                gfxDecoders.Add((IGraphicsReader)Activator.CreateInstance(type));
            }
            gfxDecoders.Sort((x, y) => x.Name.CompareTo(y.Name));

            comboBoxGfxDecoder.ItemsSource = gfxDecoders;

            for(int i=0; i<gfxDecoders.Count; ++i)
            {
                if (Instance.GraphicsReader.GetType() == gfxDecoders[i].GetType())
                {
                    comboBoxGfxDecoder.SelectedIndex = i;
                    break;
                }
            }

            tabControlsLang.ItemsSource = Instance.Entries;

            Instance.PropertyChanged -= Instance_PropertyChanged;
            Instance.PropertyChanged += Instance_PropertyChanged;
        }

        private void Instance_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Instance.GraphicsReader))
            {
                List<IGraphicsReader> gfxDecoders = comboBoxGfxDecoder.ItemsSource as List<IGraphicsReader>;
                for (int i = 0; i < gfxDecoders.Count; ++i)
                {
                    if (Instance.GraphicsReader.GetType() == gfxDecoders[i].GetType())
                    {
                        comboBoxGfxDecoder.SelectedIndex = i;
                        break;
                    }
                }
            }

            if (e.PropertyName == nameof(Instance.Name))
            {
                refreshWindowTitle();
            }
        }

        private void textBoxPreview_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            //_textPreview = BitmapFactory.New(8 * 28, 8 * 6);
            //_textPreview.Lock();

            //_textPreview.FillRectangle(0, 0, (int)_textPreview.Width, (int)_textPreview.Height, Color.FromArgb(255, 0, 0, 0));

            //Rect destRect = new Rect(0, 0, 8, 8);

            //foreach (var character in textBoxPreview.Text)
            //{
            //    int realByte = (byte)character - 0x16;

            //    if (character == '\r')
            //    {
            //        continue;
            //    }

            //    if (character == '\n')
            //    {
            //        destRect.X = 0;
            //        destRect.Y += 8;
            //        continue;
            //    }

            //    if (destRect.X >= (8 * 28))
            //    {
            //        destRect.X = 0;
            //        destRect.Y += 8;
            //    }

            //    int tileX = realByte % 16;
            //    int tileY = realByte / 16;

            //    _textPreview.Blit(destRect, _graphicsSource, new Rect(tileX * 8, tileY * 8, 8, 8));

            //    destRect.X += 8;
            //}

            //_textPreview.Unlock();

            //imageTextPreview.Source = _textPreview;
        }

        private void comboBoxGfxDecoder_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            IGraphicsReader graphicsReader = comboBoxGfxDecoder.SelectedItem as IGraphicsReader;
            if (graphicsReader != null)
            {
                if (Instance.GraphicsReader.GetType() != graphicsReader.GetType())
                {
                    Instance.GraphicsReader = graphicsReader;
                }
            }
        }
    }
}
