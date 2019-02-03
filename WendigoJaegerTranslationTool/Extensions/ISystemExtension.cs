using WendigoJaeger.TranslationTool.Systems;
using System.ComponentModel;
using System.Linq;

namespace WendigoJaeger.TranslationTool.Extensions
{
    public static class ISystemExtension
    {
        public static string DisplayName(this ISystem system)
        {
            var displayNameAttribute = system.GetType().GetCustomAttributes(false).FirstOrDefault(x => x.GetType() == typeof(DisplayNameAttribute)) as DisplayNameAttribute;
            if (displayNameAttribute != null)
            {
                return displayNameAttribute.DisplayName;
            }
            else
            {
                return system.GetType().Name;
            }
        }
    }
}
