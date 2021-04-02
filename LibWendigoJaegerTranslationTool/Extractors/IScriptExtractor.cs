using Newtonsoft.Json;
using WendigoJaeger.TranslationTool.Data;

namespace WendigoJaeger.TranslationTool.Extractors
{
    public interface IScriptExtractor
    {
        [JsonIgnore]
        ScriptBankType BankType { get; }

        string Name { get; }

        void Extract(Project project, ScriptSettings settings);
    }
}
