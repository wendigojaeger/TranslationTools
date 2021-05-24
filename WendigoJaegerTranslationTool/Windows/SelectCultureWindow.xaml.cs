using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Media.Imaging;
using WendigoJaeger.TranslationTool.Converters;

namespace WendigoJaeger.TranslationTool.Windows
{
    public partial class SelectCultureWindow : Window
    {
        static CultureInfo[] availableCultures = null;

        public CultureInfo SelectedCulture
        {
            get
            {
                return (CultureInfo)comboCulture.SelectedItem;
            }
        }

        public SelectCultureWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (availableCultures == null)
            {
                CultureInfo[] allCultures = CultureInfo.GetCultures(CultureTypes.AllCultures);

                CultureImageConverter imageConveter = new();

                List<CultureInfo> availableCultureList = new();

                foreach (var culture in allCultures)
                {
                    BitmapImage cultureImage = imageConveter.Convert(culture.Name, null, null, CultureInfo.InvariantCulture) as BitmapImage;
                    if (cultureImage != null)
                    {
                        availableCultureList.Add(culture);
                    }
                }

                availableCultures = availableCultureList.ToArray();
            }

            comboCulture.ItemsSource = availableCultures;
        }

        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
