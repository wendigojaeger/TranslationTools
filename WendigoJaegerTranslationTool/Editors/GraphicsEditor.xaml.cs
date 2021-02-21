using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using WendigoJaeger.TranslationTool.Controls;
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

        public string CurrentLocalizedPath
        {
            get
            {
                return Instance.GetEntry(CurrentLocale).Path;
            }
        }

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

            sourceRelativePathControl.DataContext = Instance;
            sourceRelativePathControl.ProjectSettings = ProjectSettings;

            translatedRelativePathControl.ProjectSettings = ProjectSettings;

            Binding graphicsReaderBinding = new Binding
            {
                Source = comboBoxGfxDecoder,
                Path = new PropertyPath(nameof(comboBoxGfxDecoder.SelectedItem)),
                Mode = BindingMode.OneWay
            };
            originalGraphicsPreview.SetBinding(GraphicsPreviewControl.GraphicsReaderProperty, graphicsReaderBinding);
            originalGraphicsPreview.ProjectSettings = ProjectSettings;

            Binding originalImageRelativePathBinding = new Binding
            {
                Path = new PropertyPath(nameof(GraphicsSettings.OriginalPath)),
                Mode = BindingMode.OneWay,
                Source = Instance
            };
            originalGraphicsPreview.SetBinding(GraphicsPreviewControl.ImageRelativePathProperty, originalImageRelativePathBinding);

            Binding translatedImageRelativePathBinding = new Binding
            {
                Path = new PropertyPath(nameof(LocalizedFilePathEntry.Path)),
                Mode = BindingMode.OneWay
            };
            translatedGraphicsPreviewControl.SetBinding(GraphicsPreviewControl.ImageRelativePathProperty, translatedImageRelativePathBinding);

            translatedGraphicsPreviewControl.SetBinding(GraphicsPreviewControl.GraphicsReaderProperty, graphicsReaderBinding);
            translatedGraphicsPreviewControl.ProjectSettings = ProjectSettings;

            onCurrentLocaleChanged(CurrentLocale);

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

        protected override void onCurrentLocaleChanged(string newLocale)
        {
            var newLocalizedEntry = Instance.GetEntry(newLocale);

            imageFlag.DataContext = newLocalizedEntry;
            translatedRelativePathControl.DataContext = newLocalizedEntry;
            translatedGraphicsPreviewControl.DataContext = newLocalizedEntry;
        }
    }
}
