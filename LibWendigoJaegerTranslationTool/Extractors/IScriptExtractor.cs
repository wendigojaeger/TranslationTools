using WendigoJaeger.TranslationTool.Data;

namespace WendigoJaeger.TranslationTool.Extractors
{
    public interface IScriptExtractor
    {
        string Name { get; }

        void Extract(Project project, ScriptSettings settings);
    }
}
