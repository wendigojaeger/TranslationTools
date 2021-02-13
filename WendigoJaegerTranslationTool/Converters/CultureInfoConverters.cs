using WendigoJaeger.TranslationTool.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace WendigoJaeger.TranslationTool.Converters
{
    public class CultureDisplayNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                string langValue = string.Empty;

                if (value is string)
                {
                    langValue = (string)value;
                }
                else if (value is KeyValuePair<string, LocalizedProjectSettings>)
                {
                    langValue = ((KeyValuePair<string, LocalizedProjectSettings>)value).Key;
                }

                if (!string.IsNullOrEmpty(langValue))
                {
                    var cultureInfo = new CultureInfo(langValue);

                    return $"{cultureInfo.NativeName} ({cultureInfo.Name})";
                }
            }
            catch (InvalidCastException)
            {
            }
            catch (CultureNotFoundException)
            {
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class CultureImageConverter : IValueConverter
    {
        private Dictionary<string, BitmapImage> _imageCache = new Dictionary<string, BitmapImage>();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                string langValue = string.Empty;

                if (value is string)
                {
                    langValue = (string)value;
                }
                else if (value is KeyValuePair<string, LocalizedProjectSettings>)
                {
                    langValue = ((KeyValuePair<string, LocalizedProjectSettings>)value).Key;
                }

                if (!string.IsNullOrEmpty(langValue))
                {
                    var cultureInfo = new CultureInfo(langValue);

                    string countryCode = cultureInfo.TwoLetterISOLanguageName;

                    if (!cultureInfo.IsNeutralCulture)
                    {
                        var regionInfo = new RegionInfo(cultureInfo.LCID);

                        countryCode = regionInfo.TwoLetterISORegionName;
                    }

                    if (!string.IsNullOrEmpty(countryCode))
                    {
                        BitmapImage image = null;

                        if (!_imageCache.TryGetValue(countryCode, out image))
                        {
                            Uri uri = new Uri($"pack://application:,,,/Images/Flags/{countryCode.ToLower()}.png", UriKind.RelativeOrAbsolute);
                            image = new BitmapImage(uri);
                            _imageCache.Add(countryCode, image);
                        }

                        return image;
                    }
                }
            }
            catch (InvalidCastException)
            {
            }
            catch (CultureNotFoundException)
            {
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
