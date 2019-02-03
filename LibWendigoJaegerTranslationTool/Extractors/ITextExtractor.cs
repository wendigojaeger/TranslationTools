using Newtonsoft.Json;
using WendigoJaeger.TranslationTool.Data;

namespace WendigoJaeger.TranslationTool.Extractors
{
    public interface ITextExtractor
    {
        [JsonIgnore]
        ScriptBankType BankType { get; }

        void Extract(Project project, ScriptSettings settings);
    }
}
