using System;
using System.Globalization;
using System.Windows.Data;
using WendigoJaeger.TranslationTool.Extensions;
using WendigoJaeger.TranslationTool.Outputs;

namespace WendigoJaeger.TranslationTool.Converters
{
    class OutputGeneratorNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is OutputGenerator outputGenerator)
            {
                return outputGenerator.DisplayName();
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
