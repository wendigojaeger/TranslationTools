using WendigoJaeger.TranslationTool.Data;

namespace WendigoJaeger.TranslationTool.Extractors
{
    public interface IDataExtractor
    {
        string Name { get; }

        void Extract(Project project, DataSettings settings);
    }
}
