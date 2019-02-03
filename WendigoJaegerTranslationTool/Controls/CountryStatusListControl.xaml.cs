using WendigoJaeger.TranslationTool.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace WendigoJaeger.TranslationTool.Controls
{
    public class StateToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ScriptEntryState state = (ScriptEntryState)value;

            switch (state)
            {
                case ScriptEntryState.ToTranslate: return Brushes.Red;
                case ScriptEntryState.InProgress: return Brushes.Yellow;
                case ScriptEntryState.Review: return Brushes.LightBlue;
                case ScriptEntryState.Final: return Brushes.Green;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ScriptFileManualRefresher
    {
        private readonly ScriptFile scriptFile;
        private BindingExpression bindingExpression;

        public ScriptFileManualRefresher(ScriptFile scriptFile, BindingExpression bindingExpression)
        {
            this.scriptFile = scriptFile;
            this.bindingExpression = bindingExpression;

            scriptFile.Entries.UndoPropertyChanged -= Entries_UndoPropertyChanged;
            scriptFile.Entries.UndoPropertyChanged += Entries_UndoPropertyChanged;
        }

        private void Entries_UndoPropertyChanged(object sender, Undo.UndoPropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TranslationEntry.State))
            {
                bindingExpression.UpdateTarget();
            }
        }
    }

    public partial class CountryStatusListControl : UserControl
    {
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable<TranslationEntry>), typeof(CountryStatusListControl), new UIPropertyMetadata(null, onItemSourceChanged));
        public IEnumerable<TranslationEntry> ItemsSource
        {
            get
            {
                return (IEnumerable<TranslationEntry>)GetValue(ItemsSourceProperty);
            }
            set
            {
                SetValue(ItemsSourceProperty, value);
            }
        }

        private ScriptFileManualRefresher ManualRefresher = null;

        public CountryStatusListControl()
        {
            InitializeComponent();

            icControls.DataContext = this;
        }

        private static void onItemSourceChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            CountryStatusListControl parent = source as CountryStatusListControl;
            if (parent != null)
            {
                var bindingExpression = parent.GetBindingExpression(ItemsSourceProperty);

                if (bindingExpression != null)
                {
                    ExternalFile<ScriptFile> scriptFileRef = bindingExpression.ResolvedSource as ExternalFile<ScriptFile>;
                    if (scriptFileRef != null)
                    {
                        parent.ManualRefresher = new ScriptFileManualRefresher(scriptFileRef.Instance, bindingExpression);
                    }
                }
            }
        }
    }
}
