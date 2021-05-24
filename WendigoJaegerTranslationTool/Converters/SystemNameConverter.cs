using System;
using System.Globalization;
using System.Windows.Data;
using WendigoJaeger.TranslationTool.Extensions;
using WendigoJaeger.TranslationTool.Systems;

namespace WendigoJaeger.TranslationTool.Converters
{
    class SystemNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ISystem system)
            {
                return system.DisplayName();
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
