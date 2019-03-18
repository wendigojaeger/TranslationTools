﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
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
using WendigoJaeger.TranslationTool.Commands;
using WendigoJaeger.TranslationTool.Data;
using Xceed.Wpf.Toolkit;

namespace WendigoJaeger.TranslationTool.Editors
{
    public class PaletteColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            PaletteColor paletteColor = value as PaletteColor;
            if (paletteColor != null)
            {
                return Color.FromArgb(255, paletteColor.R, paletteColor.G, paletteColor.B);
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class BasePaletteEditor : BaseEditor<Palette>
    {
    }

    [EditorFor(typeof(Palette))]
    public partial class PaletteEditor : BasePaletteEditor
    {
        public PaletteEditor()
        {
            InitializeComponent();
        }

        public override void Init()
        {
            base.Init();

            listBoxPalette.ItemsSource = Instance.Entries;
            Instance.Entries.PropertyChanged += Entries_PropertyChanged;
        }

        private void Entries_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(listBoxPalette.ItemsSource).Refresh();
        }

        private void buttonAdd_Click(object sender, RoutedEventArgs e)
        {
            Instance.Entries.Add(new PaletteColor());
        }

        private void buttonRemove_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxPalette.SelectedIndex >= 0)
            {
                Instance.Entries.RemoveAt(listBoxPalette.SelectedIndex);
            }
        }

        private void ColorPicker_Closed(object sender, RoutedEventArgs e)
        {
            var picker = sender as ColorPicker;
            if (picker != null)
            {
                var paletteColor = picker.DataContext as PaletteColor;

                execute(new PaletteColorCommand(this, paletteColor, picker.SelectedColor.Value));
            }
        }

        private void listBoxPalette_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                buttonRemove_Click(null, null);
            }
        }
    }
}