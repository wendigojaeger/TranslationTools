using System;
using System.Globalization;
using System.Windows.Data;
using WendigoJaeger.TranslationTool.Extensions;
using WendigoJaeger.TranslationTool.Patch;

namespace WendigoJaeger.TranslationTool.Converters
{
    class PatcherNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IPatcher patcher)
            {
                return patcher.DisplayName();
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
