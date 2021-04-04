using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WendigoJaeger.TranslationTool.Commands;
using WendigoJaeger.TranslationTool.Controls;
using WendigoJaeger.TranslationTool.Creators;
using WendigoJaeger.TranslationTool.Data;
using WendigoJaeger.TranslationTool.Editors;
using WendigoJaeger.TranslationTool.Extensions;
using WendigoJaeger.TranslationTool.Extractors;
using WendigoJaeger.TranslationTool.Outputs.SNES;
using WendigoJaeger.TranslationTool.Systems;
using WendigoJaeger.TranslationTool.Undo;

namespace WendigoJaeger.TranslationTool
{
    public class ProjectTreeSubEntry
    {
        public string Name { get; set; }
        public ImageSource Icon { get; set; }
        public IEnumerable<UndoObject> List { get; set; }
    }

    public class ProjectSettingsItemSource : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ProjectSettings projectSettings = value as ProjectSettings;
            if (projectSettings != null)
            {
                return generateItemsSource(projectSettings);
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        private IEnumerable<ProjectTreeSubEntry> generateItemsSource(ProjectSettings projectSettings)
        {
            yield return new ProjectTreeSubEntry()
            {
                Name = Resource.projectHeaderTableFile,
                Icon = new BitmapImage(new Uri("pack://application:,,,/Images/ScriptSettingsIcon.png", UriKind.RelativeOrAbsolute)),
                List = projectSettings.TableFiles
            };

            yield return new ProjectTreeSubEntry() {
                Name = Resource.projectHeaderScripts,
                Icon = new BitmapImage(new Uri("pack://application:,,,/Images/ScriptSettingsIcon.png", UriKind.RelativeOrAbsolute)),
                List = projectSettings.Scripts
            };

            yield return new ProjectTreeSubEntry()
            {
                Name = Resource.projectHeaderData,
                Icon = new BitmapImage(new Uri("pack://application:,,,/Images/ScriptSettingsIcon.png", UriKind.RelativeOrAbsolute)),
                List = projectSettings.DataSettings
            };

            yield return new ProjectTreeSubEntry() {
                Name = Resource.projectHeaderGraphics,
                Icon = new BitmapImage(new Uri("pack://application:,,,/Images/GraphicsIcon.png", UriKind.RelativeOrAbsolute)),
                List = projectSettings.Graphics
            };

            yield return new ProjectTreeSubEntry() {
                Name = Resource.projectHeaderFont,
                Icon = new BitmapImage(new Uri("pack://application:,,,/Images/GraphicsIcon.png", UriKind.RelativeOrAbsolute)),
                List = projectSettings.Fonts
            };

            yield return new ProjectTreeSubEntry() {
                Name = Resource.projectHeaderPalettes,
                Icon = new BitmapImage(new Uri("pack://application:,,,/Images/AssemblyFileIcon.png", UriKind.RelativeOrAbsolute)),
                List = projectSettings.Palettes
            };

            yield return new ProjectTreeSubEntry()
            {
                Name = Resource.projectHeaderTextPreview,
                Icon = new BitmapImage(new Uri("pack://application:,,,/Images/ScriptSettingsIcon.png", UriKind.RelativeOrAbsolute)),
                List = projectSettings.TextPreviewInfos
            };

            yield return new ProjectTreeSubEntry()
            {
                Name = Resource.projectHeaderCustomAssemblyFIle,
                Icon = new BitmapImage(new Uri("pack://application:,,,/Images/AssemblyFileIcon.png", UriKind.RelativeOrAbsolute)),
                List = projectSettings.AssemblyFileSettings
            };
        }
    }

    public class ScriptFileProgressAggregator : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ScriptFile scriptFile = value as ScriptFile;
            if (scriptFile != null)
            {
                List<TranslationEntry> result = new List<TranslationEntry>();

                var projectSettings = ((MainWindow)Application.Current.MainWindow).ProjectSettings;
                if (projectSettings != null)
                {
                    foreach(var lang in projectSettings.Project.Lang.Keys)
                    {
                        TranslationEntry progress = new TranslationEntry();
                        progress.State = ScriptEntryState.ToTranslate;
                        progress.Lang = lang;

                        int[] stateCount = new int[Enum.GetNames(typeof(ScriptEntryState)).Length];

                        foreach (var scriptEntry in scriptFile.Entries)
                        {
                            var translation = scriptEntry.GetTranslation(lang);
                            if (translation != null)
                            {
                                stateCount[(int)translation.State] += 1;
                            }
                        }

                        if (stateCount[(int)ScriptEntryState.Final] == scriptFile.Entries.Count)
                        {
                            progress.State = ScriptEntryState.Final;
                        }
                        else
                        {
                            if (stateCount[(int)ScriptEntryState.ToTranslate] == 0 && stateCount[(int)ScriptEntryState.InProgress] == 0)
                            {
                                progress.State = ScriptEntryState.Review;
                            }
                            else if (stateCount[(int)ScriptEntryState.ToTranslate] == scriptFile.Entries.Count)
                            {
                                progress.State = ScriptEntryState.ToTranslate;
                            }
                            else
                            {
                                progress.State = ScriptEntryState.InProgress;
                            }
                        }

                        result.Add(progress);
                    }
                }

                return result;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class DataFileProgressAggregator : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DataFile dataFile = value as DataFile;
            if (dataFile != null)
            {
                List<TranslationEntry> result = new();

                var projectSettings = ((MainWindow)Application.Current.MainWindow).ProjectSettings;
                if (projectSettings != null)
                {
                    foreach (var lang in projectSettings.Project.Lang.Keys)
                    {
                        TranslationEntry progress = new()
                        {
                            State = ScriptEntryState.ToTranslate,
                            Lang = lang
                        };

                        int[] stateCount = new int[Enum.GetNames(typeof(ScriptEntryState)).Length];

                        foreach (var dataEntry in dataFile.DataEntries)
                        {
                            var translation = dataEntry.GetTranslation(lang);
                            if (translation != null)
                            {
                                stateCount[(int)translation.State] += 1;
                            }
                        }

                        if (stateCount[(int)ScriptEntryState.Final] == dataFile.DataEntries.Count)
                        {
                            progress.State = ScriptEntryState.Final;
                        }
                        else
                        {
                            if (stateCount[(int)ScriptEntryState.ToTranslate] == 0 && stateCount[(int)ScriptEntryState.InProgress] == 0)
                            {
                                progress.State = ScriptEntryState.Review;
                            }
                            else if (stateCount[(int)ScriptEntryState.ToTranslate] == dataFile.DataEntries.Count)
                            {
                                progress.State = ScriptEntryState.ToTranslate;
                            }
                            else
                            {
                                progress.State = ScriptEntryState.InProgress;
                            }
                        }

                        result.Add(progress);
                    }
                }

                return result;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public partial class MainWindow : Window, IUndoAware, INotifyPropertyChanged
    {
        private ProjectSettings _projectSettings;
        private UndoStack _undoStack = new UndoStack();
        private Type[] _cachedEditorTypes = null;
        private IEditor _currentEditor = null;

        public event PropertyChangedEventHandler PropertyChanged;

        internal ProjectSettings ProjectSettings
        {
            get
            {
                return _projectSettings;
            }
            set
            {
                _projectSettings = value;

                _undoStack.Clear();

                if (_projectSettings != null)
                {
                    _projectSettings.UndoArrayChanged -= onUndoArrayChanged;
                    _projectSettings.UndoArrayChanged += onUndoArrayChanged;

                    _projectSettings.UndoPropertyChanged -= onUndoPropertyChanged;
                    _projectSettings.UndoPropertyChanged += onUndoPropertyChanged;

                    treeViewProject.ItemsSource = new ProjectSettings[] { _projectSettings };

                    comboLanguages.ItemsSource = ProjectSettings.Project.Lang;

                    if (ProjectSettings.Project.Lang.Count > 0)
                    {
                        comboLanguages.SelectedIndex = 0;
                    }

                    onUndoStackChanged(null, null);
                }
            }
        }

        public string CustomWindowTitle
        {
            get
            {
                if (ProjectSettings != null)
                {
                    string dirtyStar = _undoStack.IsDirty ? "*" : string.Empty;
                    return $"{Resource.mainWindowTitle} - {dirtyStar}{_projectSettings.Project.Name}";
                }
                else
                {
                    return Resource.mainWindowTitle;
                }
            }
        }

        public bool DisableUndoNotify { get; set; }

        internal UndoStack UndoStack
        {
            get
            {
                return _undoStack;
            }
        }

        internal string CurrentLocale
        {
            get
            {
                if (comboLanguages.SelectedItem != null)
                {
                    return ((KeyValuePair<string, LocalizedProjectSettings>)comboLanguages.SelectedItem).Key;
                }

                return string.Empty;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (_cachedEditorTypes == null)
            {
                var editorQuery = from a in AppDomain.CurrentDomain.GetAssemblies()
                                  from t in a.GetTypes()
                                  from attribute in t.GetCustomAttributes(typeof(EditorForAttribute), false)
                                  select t;

                _cachedEditorTypes = editorQuery.ToArray();
            }

            _undoStack.UndoHistory.CollectionChanged += onUndoStackChanged;

            setBinding(this, TitleProperty, this, nameof(CustomWindowTitle));

            string[] commandLineArgs = Environment.GetCommandLineArgs();
            if (commandLineArgs.Length >= 2)
            {
                openProject(Path.GetFullPath(commandLineArgs[1]));
            }
        }

        private void openProject(string path)
        {
            ProjectSettings openSettings = ProjectSettings.Load(path);
            if (openSettings != null)
            {
                ProjectSettings = openSettings;
                ProjectSettings.Path = path;
            }
        }

        private void New_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = Resource.filterProjectFile;

            var result = saveDialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                ProjectSettings newProjectSettings = new ProjectSettings();

                newProjectSettings.Project.Name = "Mega Man X2";
                newProjectSettings.Project.InputFile = "mmx2_original.sfc";
                newProjectSettings.Project.System = new SNESLoRomSlowRom();
                newProjectSettings.Project.Lang.Add("fr-FR", new LocalizedProjectSettings() { OutputFile = "mmx2_fr.sfc" });
                newProjectSettings.Project.Lang.Add("fr-CA", new LocalizedProjectSettings() { OutputFile = "mmx2_qc.sfc" });
                newProjectSettings.Project.OutputGenerator = new SNESBassOutput();

                string directory = Path.GetDirectoryName(saveDialog.FileName);

                TableFile mainDialogTableFile = new TableFile();
                mainDialogTableFile.NewLine = 0x80;
                mainDialogTableFile.Terminator = 0x82;
                mainDialogTableFile.SourceTableFile = "script_en.tbl";
                mainDialogTableFile["fr-FR"] = "script_fr.tbl";
                mainDialogTableFile["fr-CA"] = "script_fr.tbl";

                ScriptSettings dialogSettings = new ScriptSettings
                {
                    Name = "MainDialog",
                    SourceRAMAddress = 0x27D800,
                    DestinationRAMAddress = 0x27D800,
                    DestinationEndRAMAddress = 0x27FFFF,
                    Entries = 59,
                    TextExtractor = new ScriptExtractorPointer16LittleEndian()
                };
                dialogSettings.TableFile.Instance = mainDialogTableFile;

                dialogSettings.Script.Path = Path.Combine(directory, "mmx2_main_dialog.wts");

                newProjectSettings.Scripts.Add(dialogSettings);

                GraphicsSettings mainFont = new GraphicsSettings
                {
                    RAMAddress = 0x2BC300,
                    Name = "MainFont"
                };
                mainFont["fr-FR"] = "mainfont_fr.gfx";
                mainFont["fr-CA"] = "mainfont_fr.gfx";

                newProjectSettings.Graphics.Add(mainFont);

                newProjectSettings.Save(saveDialog.FileName);

                ProjectSettings = newProjectSettings;
                ProjectSettings.Path = saveDialog.FileName;
            }
        }

        private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog
            {
                Filter = Resource.filterProjectFile
            };

            var result = openDialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                openProject(openDialog.FileName);
            }
        }

        private void Exit_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void About_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MessageBox.Show(Resource.mainWindowTitle, Resource.mnuAbout);
        }

        private void Redo_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _undoStack.CanRedo && ProjectSettings != null;
        }

        private void Redo_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _undoStack.Redo();
        }

        private void Undo_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _undoStack.CanUndo && ProjectSettings != null;
        }

        private void Undo_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _undoStack.Undo();
        }

        private void Save_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _undoStack.CanUndo && ProjectSettings != null;
        }

        private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ProjectSettings.Save(ProjectSettings.Path);
            _undoStack.SetLastSaveCommand();
            onUndoStackChanged(null, null);
        }

        private void SaveAs_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ProjectSettings != null;
        }

        private void SaveAs_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog
            {
                Filter = Resource.filterProjectFile
            };

            var result = saveDialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                ProjectSettings.Save(saveDialog.FileName);
                ProjectSettings.Path = saveDialog.FileName;

                _undoStack.SetLastSaveCommand();

                onUndoStackChanged(null, null);
            }
        }

        private void BuildAndRun_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ProjectSettings != null;
        }

        private void BuildAndRun_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Reporter reporter = new();
            setupReporter(reporter);

            var targetLanguage = CurrentLocale;

            var buildAndRunTask = Task.Run(() =>
            {
                doBuild(targetLanguage, reporter);
            }).ContinueWith((previousTask) =>
            {
                if (!reporter.HasErrors)
                {
                    doRun(targetLanguage, reporter);
                }
            }
            );

            buildAndRunTask.Wait();
        }

        private void Run_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ProjectSettings != null;
        }

        private void Run_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Reporter reporter = new();
            setupReporter(reporter);

            var targetLanguage = CurrentLocale;

            var runTask = Task.Run(() =>
            {
                doRun(targetLanguage, reporter);
            });

            runTask.Wait();
        }

        private void BuildAll_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ProjectSettings != null;
        }

        private void BuildAll_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Reporter reporter = new();
            setupReporter(reporter);

            var taskSequence = new TaskCompletionSource();
            taskSequence.SetResult();
            Task parentTask = taskSequence.Task;
            foreach (var targetLanguage in ProjectSettings.Project.Lang.Keys)
            {
                parentTask = parentTask.ContinueWith(t => doBuild(targetLanguage, reporter));
            }
            parentTask.Wait();
        }

        private void Distribute_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ProjectSettings != null;
        }

        private void Distribute_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Reporter reporter = new();
            setupReporter(reporter);

            var taskSequence = new TaskCompletionSource();
            taskSequence.SetResult();
            Task parentTask = taskSequence.Task;
            foreach (var targetLanguage in ProjectSettings.Project.Lang.Keys)
            {
                parentTask = parentTask.ContinueWith(t => doBuild(targetLanguage, reporter));
            }

            foreach (var langPair in ProjectSettings.Project.Lang)
            {
                parentTask = parentTask.ContinueWith(t =>
                {
                    string ipsPatch = Path.ChangeExtension(ProjectSettings.GetAbsolutePath(langPair.Value.OutputFile), ".ips");

                    reporter.Info("Creating IPS patch for lang '{0}'", langPair.Key);

                    if (ProjectSettings.Project.Patcher != null)
                    {
                        ProjectSettings.Project.Patcher.Create(ProjectSettings.GetAbsolutePath(ProjectSettings.Project.InputFile), ProjectSettings.GetAbsolutePath(langPair.Value.OutputFile), ipsPatch);
                    }
                });
            }

            parentTask = parentTask.ContinueWith(t =>
            {
                var zipFile = ProjectSettings.GetAbsolutePath($"{ProjectSettings.Project.Name}-{ProjectSettings.Project.Version}.zip");

                reporter.Info("Creating ZIP file '{0}' for distribution", Path.GetFileName(zipFile));

                using var distributeZipFile = new FileStream(zipFile, FileMode.Create);
                using ZipArchive zipArchive = new(distributeZipFile, ZipArchiveMode.Create);

                foreach (var langPair in ProjectSettings.Project.Lang)
                {
                    string ipsPatch = Path.ChangeExtension(ProjectSettings.GetAbsolutePath(langPair.Value.OutputFile), ".ips");

                    zipArchive.CreateEntryFromFile(ipsPatch, Path.GetFileName(ipsPatch));
                }

                foreach(var additionalFile in ProjectSettings.Project.AdditionalFilesToPack)
                {
                    string filePath = ProjectSettings.GetAbsolutePath(additionalFile);

                    zipArchive.CreateEntryFromFile(filePath, additionalFile);
                }
            });

            parentTask.Wait();
        }

        private void AddAssemblyFile_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ProjectSettings != null;
        }

        private void AddAssemblyFile_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var newFileSettings = ObjectCreator.Create<AssemblyFileSettings>(ProjectSettings);
            if (newFileSettings != null)
            {
                ProjectSettings.AssemblyFileSettings.Add(newFileSettings);
            }
        }

        private void AddLanguage_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ProjectSettings != null;
        }

        private void AddLanguage_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void AddScript_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ProjectSettings != null;
        }

        private void AddScript_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var newScript = ObjectCreator.Create<ScriptSettings>(ProjectSettings);
            if (newScript != null)
            {
                ProjectSettings.Scripts.Add(newScript);

                showEditor(newScript);
            }
        }

        private void AddData_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ProjectSettings != null;
        }

        private void AddData_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var newData = ObjectCreator.Create<DataSettings>(ProjectSettings);
            if (newData != null)
            {
                ProjectSettings.DataSettings.Add(newData);

                showEditor(newData);
            }
        }

        private void AddGraphics_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ProjectSettings != null;
        }

        private void AddGraphics_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var newGraphics = ObjectCreator.Create<GraphicsSettings>(ProjectSettings);
            if (newGraphics != null)
            {
                ProjectSettings.Graphics.Add(newGraphics);

                showEditor(newGraphics);
            }
        }

        private void AddFont_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ProjectSettings != null;
        }

        private void AddFont_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var newFont = ObjectCreator.Create<FontSettings>(ProjectSettings);
            if (newFont != null)
            {
                ProjectSettings.Fonts.Add(newFont);

                showEditor(newFont);
            }
        }

        private void AddPalette_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ProjectSettings != null;
        }

        private void AddPalette_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var newPalette = ObjectCreator.Create<Palette>(ProjectSettings);
            if (newPalette != null)
            {
                ProjectSettings.Palettes.Add(newPalette);

                showEditor(newPalette);
            }
        }

        private void AddTableFile_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ProjectSettings != null;
        }

        private void AddTableFile_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var newTableFile = ObjectCreator.Create<TableFile>(ProjectSettings);
            if (newTableFile != null)
            {
                ProjectSettings.TableFiles.Add(newTableFile);

                showEditor(newTableFile);
            }
        }

        private void AddTextPreview_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ProjectSettings != null;
        }

        private void AddTextPreview_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var newTextPreviewInfo = ObjectCreator.Create<TextPreviewInfo>(ProjectSettings);
            if (newTextPreviewInfo != null)
            {
                ProjectSettings.TextPreviewInfos.Add(newTextPreviewInfo);

                showEditor(newTextPreviewInfo);
            }
        }

        private void ProjectSettings_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ProjectSettings != null;
        }

        private void ProjectSettings_Executed(object sender, ExecutedRoutedEventArgs e)
        {
        }

        private void ScriptExtract_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ProjectSettings != null && treeViewProject.SelectedItem is ScriptSettings;
        }

        private void ScriptExtract_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var selectedScriptSettings = treeViewProject.SelectedItem as ScriptSettings;

            if (selectedScriptSettings != null)
            {
                if (selectedScriptSettings.TextExtractor != null)
                {
                    selectedScriptSettings.TextExtractor.Extract(ProjectSettings.Project, selectedScriptSettings);
                }
            }
        }

        private void ScriptPrevious_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;

            var selectedScriptEntry = treeViewProject.SelectedItem as ScriptEntry;
            if (selectedScriptEntry != null)
            {
                TreeViewItem item = treeViewProject.ItemContainerGenerator.ContainerFromItemRecursive(selectedScriptEntry);

                if (item != null)
                {
                    var parent = getTreeViewItemParent(item);
                    if (parent != null)
                    {
                        var currentIndex = ((ScriptSettings)parent.DataContext).Script.Instance.Entries.IndexOf(selectedScriptEntry);

                        e.CanExecute = currentIndex > 0;
                    }
                }
            }
        }

        private void ScriptPrevious_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var selectedScriptEntry = treeViewProject.SelectedItem as ScriptEntry;
            if (selectedScriptEntry != null)
            {
                TreeViewItem item = treeViewProject.ItemContainerGenerator.ContainerFromItemRecursive(selectedScriptEntry);

                if (item != null)
                {
                    var parent = getTreeViewItemParent(item);
                    if (parent != null)
                    {
                        var currentIndex = ((ScriptSettings)parent.DataContext).Script.Instance.Entries.IndexOf(selectedScriptEntry);

                        var newIndex = Math.Max(currentIndex - 1, 0);

                        var nextTreeItem = parent.ItemContainerGenerator.ContainerFromIndex(newIndex) as TreeViewItem;
                        if (nextTreeItem != null)
                        {
                            nextTreeItem.IsSelected = true;
                        }
                    }
                }
            }
        }

        private void ScriptNext_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;

            var selectedScriptEntry = treeViewProject.SelectedItem as ScriptEntry;
            if (selectedScriptEntry != null)
            {
                TreeViewItem item = treeViewProject.ItemContainerGenerator.ContainerFromItemRecursive(selectedScriptEntry);

                if (item != null)
                {
                    var parent = getTreeViewItemParent(item);
                    if (parent != null)
                    {
                        var currentIndex = ((ScriptSettings)parent.DataContext).Script.Instance.Entries.IndexOf(selectedScriptEntry);

                        e.CanExecute = currentIndex < (parent.Items.Count - 1);
                    }
                }
            }
        }

        private void ScriptNext_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var selectedScriptEntry = treeViewProject.SelectedItem as ScriptEntry;
            if (selectedScriptEntry != null)
            {
                TreeViewItem item = treeViewProject.ItemContainerGenerator.ContainerFromItemRecursive(selectedScriptEntry);

                if (item != null)
                {
                    var parent = getTreeViewItemParent(item);
                    if (parent != null)
                    {
                        var currentIndex = ((ScriptSettings)parent.DataContext).Script.Instance.Entries.IndexOf(selectedScriptEntry);

                        var newIndex = Math.Min(currentIndex + 1, parent.Items.Count - 1);

                        var nextTreeItem = parent.ItemContainerGenerator.ContainerFromIndex(newIndex) as TreeViewItem;
                        if (nextTreeItem != null)
                        {
                            nextTreeItem.IsSelected = true;
                        }
                    }
                }
            }
        }

        private void ScriptSettings_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ProjectSettings != null && treeViewProject.SelectedItem is ScriptSettings;
        }

        private void ScriptSettings_Executed(object sender, ExecutedRoutedEventArgs e)
        {
        }

        private void DataExtract_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ProjectSettings != null && treeViewProject.SelectedItem is DataSettings;
        }

        private void DataExtract_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var selectedDataSettings = treeViewProject.SelectedItem as DataSettings;

            if (selectedDataSettings != null)
            {
                if (selectedDataSettings.DataFile != null)
                {
                    selectedDataSettings.DataExtractor.Extract(ProjectSettings.Project, selectedDataSettings);
                }
            }
        }

        private void treeViewProject_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var item = e.NewValue as UndoObject;
            if (item != null)
            {
                showEditor(item);
            }
        }

        private void showEditor(UndoObject item)
        {
            if (item == null)
            {
                return;
            }

            var editorQuery = from t in _cachedEditorTypes
                                   from attribute in t.GetCustomAttributes(typeof(EditorForAttribute), false) as EditorForAttribute[]
                                   where attribute.EditedType.IsAssignableFrom(item.GetType())
                                   orderby item.GetType() == attribute.EditedType ? 0 : 1
                                   select t;

            var editorType = editorQuery.FirstOrDefault();
            if (editorType != null)
            {
                IEditor editorInstance = (IEditor)Activator.CreateInstance(editorType);

                if (editorInstance != null)
                {
                    editorInstance.ProjectSettings = ProjectSettings;
                    editorInstance.EditedItem = item;
                    editorInstance.UpdateStatusBar = onUpdateStatusBar;
                    editorInstance.MainWindow = this;
                    editorInstance.CurrentLocale = CurrentLocale;
                    editorInstance.Init();

                    editorContent.Content = editorInstance;
                    _currentEditor = editorInstance;
                }
            }
        }

        private void onUndoArrayChanged(object sender, UndoArrayChangedEventArgs e)
        {
            if (!DisableUndoNotify)
            {
                var array = sender as IList;
                if (array != null)
                {
                    switch(e.Operation)
                    {
                        case UndoArrayChangedEventArgs.OperationType.Add:
                            _undoStack.Execute(new ArrayItemAddedCommand(this, array, e.Objects));
                            break;
                        case UndoArrayChangedEventArgs.OperationType.Remove:
                            _undoStack.Execute(new ArrayItemRemovedCommand(this, array, e.Objects));
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private void onUndoPropertyChanged(object sender, UndoPropertyChangedEventArgs e)
        {
            if (!DisableUndoNotify)
            {
                _undoStack.Execute(new PropertyValueChangedCommand(this, sender, e.PropertyName, e.OldValue, e.NewValue));
            }
        }

        private void onUndoStackChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            notifyPropertyChanged(nameof(CustomWindowTitle));
        }

        private void notifyPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void setBinding(FrameworkElement target, DependencyProperty property, object source, string path)
        {
            Binding binding = new Binding
            {
                Source = source,
                Path = new PropertyPath(path),
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };

            target.SetBinding(property, binding);
        }

        private TreeViewItem getTreeViewItemParent(TreeViewItem item)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(item);
            while (!(parent is TreeViewItem) || parent is TreeView)
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            return parent as TreeViewItem;
        }
        
        private void TreeViewProject_KeyUp(object sender, KeyEventArgs e)
        {
            var treeViewItem = e.OriginalSource as TreeViewItem;
            if (treeViewItem != null)
            {
                var editableTextBlock = FindVisualChild<EditableTextBlock>(treeViewItem);
                if (editableTextBlock != null)
                {
                    editableTextBlock.OnKeyUp(sender, e);
                }
            }
        }

        private void comboLanguages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_currentEditor != null)
            {
                _currentEditor.CurrentLocale = CurrentLocale;
            }
        }

        private void setupReporter(Reporter reporter)
        {
            textBoxOutput.Text = string.Empty;
            reporter.OnLineOutput += onReporterOutput;
        }

        private void doBuild(string targetLanguage, Reporter reporter)
        {
            Builder builder = new();
            builder.Reporter = reporter;
            builder.Build(targetLanguage, ProjectSettings);
        }

        private void doRun(string targetLanguage, Reporter reporter)
        {
            reporter.Info(Resource.runMessage, ProjectSettings.Project.System.DisplayName(), "bsnes-plus", targetLanguage);

            // TODO: Configure this
            Process emulator = new Process();
            emulator.StartInfo.FileName = @"C:\Programmation\Traduction\CommonTools\bsnes-plus\bsnes.exe";
            emulator.StartInfo.WorkingDirectory = Path.GetDirectoryName(ProjectSettings.Path);
            emulator.StartInfo.Arguments = ProjectSettings.Project.Lang[targetLanguage].OutputFile;
            emulator.Start();
        }

        private void onReporterOutput(string line)
        {
            Dispatcher.InvokeAsync(() => textBoxOutput.Text += line);
        }

        private void onUpdateStatusBar(string value)
        {
            textBlockStatusBar.Text = value;
        }

        public static T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        {
            if (obj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
                {
                    var child = VisualTreeHelper.GetChild(obj, i);
                    if (child is T)
                    {
                        return (T)child;
                    }

                    T childItem = FindVisualChild<T>(child);
                    if (childItem != null)
                    {
                        return childItem;
                    }
                }
            }

            return null;
        }

    }
}
