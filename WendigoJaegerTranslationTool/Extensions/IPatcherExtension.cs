using System.ComponentModel;
using System.Linq;
using WendigoJaeger.TranslationTool.Patch;

namespace WendigoJaeger.TranslationTool.Extensions
{
    public static class IPatcherExtension
    {
        public static string DisplayName(this IPatcher patcher)
        {
            var displayNameAttribute = patcher.GetType().GetCustomAttributes(false).FirstOrDefault(x => x.GetType() == typeof(DisplayNameAttribute)) as DisplayNameAttribute;
            if (displayNameAttribute != null)
            {
                return displayNameAttribute.DisplayName;
            }
            else
            {
                return patcher.GetType().Name;
            }
        }
    }
}
