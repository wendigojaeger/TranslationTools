using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WendigoJaeger.TranslationTool.Data;

namespace WendigoJaeger.TranslationTool.Extractors
{
    public interface IScriptDictionaryExtractor
    {
        string Name { get; }

        void Extract(Project project, ScriptDictionary scriptDictionary);
    }
}
