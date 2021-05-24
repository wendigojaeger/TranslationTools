using System.ComponentModel;
using System.Linq;
using WendigoJaeger.TranslationTool.Outputs;

namespace WendigoJaeger.TranslationTool.Extensions
{
    public static class OutputGeneratorExtension
    {
        public static string DisplayName(this OutputGenerator output)
        {
            DisplayNameAttribute displayNameAttribute = output.GetType().GetCustomAttributes(false).FirstOrDefault(x => x.GetType() == typeof(DisplayNameAttribute)) as DisplayNameAttribute;
            if (displayNameAttribute != null)
            {
                return displayNameAttribute.DisplayName;
            }
            else
            {
                return output.GetType().Name;
            }
        }
    }
}
