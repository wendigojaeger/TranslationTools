namespace WendigoJaeger.TranslationTool.Patch
{
    public interface IPatcher
    {
        void Create(string sourceFilePath, string targetFilePath, string patchFile);
    }
}
