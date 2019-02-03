namespace WendigoJaeger.TranslationTool.Undo
{
    public class UndoStack
    {
        public ObservableStack<IUndoCommand> UndoHistory { get; } = new ObservableStack<IUndoCommand>();
        public ObservableStack<IUndoCommand> RedoHistory { get; } = new ObservableStack<IUndoCommand>();

        public bool CanUndo
        {
            get
            {
                return UndoHistory.Count > 0;
            }
        }

        public bool CanRedo
        {
            get
            {
                return RedoHistory.Count > 0;
            }
        }

        public bool IsDirty
        {
            get
            {
                return Top != LastSavedCommand;
            }
        }

        public IUndoCommand Top
        {
            get
            {
                if (UndoHistory.Count > 0)
                {
                    return UndoHistory.Peek();
                }

                return null;
            }
        }

        public IUndoCommand LastSavedCommand { get; set; }

        public void Clear()
        {
            UndoHistory.Clear();
            RedoHistory.Clear();
        }

        public void Execute(IUndoCommand command)
        {
            if (command == null)
            {
                return;
            }

            command.Execute();

            UndoHistory.Push(command);

            RedoHistory.Clear();
        }

        public void SetLastSaveCommand()
        {
            LastSavedCommand = Top;
        }

        public IUndoCommand Undo()
        {
            if (CanUndo)
            {
                var command = UndoHistory.Pop();

                command.Undo();

                RedoHistory.Push(command);

                return command;
            }

            return null;
        }

        public IUndoCommand Redo()
        {
            if (CanRedo)
            {
                var command = RedoHistory.Pop();

                command.Redo();

                UndoHistory.Push(command);

                return command;
            }

            return null;
        }
    }
}
