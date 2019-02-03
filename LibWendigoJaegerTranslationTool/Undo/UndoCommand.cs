namespace WendigoJaeger.TranslationTool.Undo
{
    public interface IUndoCommand
    {
        string CommandName { get; }

        void Execute();

        void Undo();
        void Redo();
    }

    public class UndoCommand : IUndoCommand
    {
        public virtual string CommandName
        {
            get
            {
                return string.Empty;
            }
        }

        public virtual void Execute()
        {
        }

        public virtual void Redo()
        {
        }

        public virtual void Undo()
        {
        }
    }
}
