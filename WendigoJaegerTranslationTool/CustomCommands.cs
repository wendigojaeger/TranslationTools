using System.Windows.Input;

namespace WendigoJaeger.TranslationTool
{
    public static class ShortcutCreator
    {
        public static InputGestureCollection Create(Key key)
        {
            return new InputGestureCollection(new InputGesture[] { new KeyGesture(key) });
        }

        public static InputGestureCollection Create(Key key, ModifierKeys modifiers)
        {
            return new InputGestureCollection(new InputGesture[] { new KeyGesture(key, modifiers) });
        }
    }

    public static class CustomCommands
    {
        public static RoutedUICommand About = new RoutedUICommand(Resource.mnuAbout, "mnuAbout", typeof(CustomCommands));
        public static RoutedUICommand BuildAll = new RoutedUICommand(Resource.mnuBuildAll, "mnuBuildAll", typeof(CustomCommands), ShortcutCreator.Create(Key.F7));
        public static RoutedUICommand BuildAndRun = new RoutedUICommand(Resource.mnuBuildAndRun, "mnuBuildAndRun", typeof(CustomCommands), ShortcutCreator.Create(Key.F5));
        public static RoutedUICommand Run = new RoutedUICommand(Resource.mnuRun, "mnuRun", typeof(CustomCommands), ShortcutCreator.Create(Key.F6));
        public static RoutedUICommand Exit = new RoutedUICommand(Resource.mnuFileExit, "mnuFileExit", typeof(CustomCommands), ShortcutCreator.Create(Key.F4, ModifierKeys.Alt));
        public static RoutedUICommand AddScript = new RoutedUICommand(Resource.mnuProjectAddScript, "mnuProjectAddScript", typeof(CustomCommands));
        public static RoutedUICommand AddLanguage = new RoutedUICommand(Resource.mnuProjectAddLanguage, "mnuProjectAddLanguage", typeof(CustomCommands));
        public static RoutedUICommand ProjectSettings = new RoutedUICommand(Resource.mnuProjectSettings, "mnuProjectSettings", typeof(CustomCommands));
        public static RoutedUICommand ScriptExtract = new RoutedUICommand(Resource.mnuScriptExtract, "mnuScriptExtract", typeof(CustomCommands));
        public static RoutedUICommand ScriptPrevious = new RoutedUICommand(Resource.mnuScriptPrevious, "mnuScriptPrevious", typeof(CustomCommands), ShortcutCreator.Create(Key.NumPad8));
        public static RoutedUICommand ScriptNext = new RoutedUICommand(Resource.mnuScriptNext, "mnuScriptNext", typeof(CustomCommands), ShortcutCreator.Create(Key.NumPad2));
        public static RoutedUICommand ScriptSettings = new RoutedUICommand(Resource.mnuScriptSettings, "mnuScriptSettings", typeof(CustomCommands));
    }
}
