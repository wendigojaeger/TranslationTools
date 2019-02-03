namespace WendigoJaeger.TranslationTool.Outputs
{
    public abstract class OutputGenerator
    {
        public abstract void Generate(Reporter reporter, OutputInfo outputInfo);
    }
}
