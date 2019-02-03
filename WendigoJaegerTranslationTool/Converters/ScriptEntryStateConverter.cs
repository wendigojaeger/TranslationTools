using WendigoJaeger.TranslationTool.Data;
using System;
using System.Globalization;
using System.Windows.Data;

namespace WendigoJaeger.TranslationTool.Converters
{
    public class ScriptEntryStateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var state = (ScriptEntryState)value;

            switch(state)
            {
                case ScriptEntryState.ToTranslate: return Resource.translationStateToTranslate;
                case ScriptEntryState.InProgress: return Resource.translationStateInProgress;
                case ScriptEntryState.Review: return Resource.translationStateReview;
                case ScriptEntryState.Final: return Resource.translationStateFinal;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
