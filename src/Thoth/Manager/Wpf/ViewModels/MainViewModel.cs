using Microsoft.Win32;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using Thismaker.Core;
using Thismaker.Goro;

namespace Thismaker.Thoth.Manager.Wpf
{
    class MainViewModel : BindableBase
    {
        #region Privates
        private LocTable _selTable;
        private LocTranslation _selTransaltion;
        private LocItem _selItem;
        private ObservableCollection<LocTable> _tables;
        private ObservableCollection<LocItem> _items;
        private ObservableCollection<LocTranslation> _translations;
        private ObservableCollection<ManagedLocale> _locales;
        private string _localeName, _localeShortName;
        private ManagedLocale _selLocale;
        private LocalizationData _data;
        private bool? _isDefaultTable, _isDefaultLocale;
        private bool _defTableEnabled, _defLocaleEnabled;
        private readonly LocalizationIO _locIO;

        private string _filePath;
        private bool _edited = false;
        #endregion

        #region Props
        //private static System.Windows.Threading.Dispatcher Dispatcher
        //{
        //    get => Application.Current.Dispatcher;
        //}
        public LocTable SelectedTable
        {
            get => _selTable;
            set => SetProperty(ref _selTable, value);
        }
        public LocTranslation SelectedTranslation
        {
            get => _selTransaltion;
            set => SetProperty(ref _selTransaltion, value);
        }
        public LocItem SelectedItem
        {
            get => _selItem;
            set => SetProperty(ref _selItem, value);
        }
        public ObservableCollection<LocTable> Tables
        {
            get => _tables;
            set => SetProperty(ref _tables, value);
        }
        public ObservableCollection<LocItem> Items
        {
            get => _items;
            set => SetProperty(ref _items, value);
        }
        public ObservableCollection<LocTranslation> Translations
        {
            get => _translations;
            set => SetProperty(ref _translations, value);
        }
        public ObservableCollection<ManagedLocale> Locales
        {
            get => _locales;
            set => SetProperty(ref _locales, value);
        }
        public string LocaleName
        {
            get => _localeName;
            set => SetProperty(ref _localeName, value);
        }
        public string LocaleShortName
        {
            get => _localeShortName;
            set => SetProperty(ref _localeShortName, value);
        }
        public ManagedLocale SelectedLocale
        {
            get => _selLocale;
            set => SetProperty(ref _selLocale, value);
        }
        public bool? IsDefaultTable
        {
            get => _isDefaultTable;
            set => SetProperty(ref _isDefaultTable, value);
        }
        public bool? IsDefaultLocale
        {
            get => _isDefaultLocale;
            set => SetProperty(ref _isDefaultLocale, value);
        }
        public bool IsDefaultLocaleEnabled
        {
            get => _defLocaleEnabled;
            set => SetProperty(ref _defLocaleEnabled, value);
        }
        public bool IsDefaultTableEnabled
        {
            get => _defTableEnabled;
            set => SetProperty(ref _defTableEnabled, value);
        }
        #endregion

        #region Init

        public MainViewModel()
        {
            Tables = new ObservableCollection<LocTable>();
            Items = new ObservableCollection<LocItem>();
            Translations = new ObservableCollection<LocTranslation>();
            Locales = new ObservableCollection<ManagedLocale>();
            _locIO = new LocalizationIO();
            _data = new LocalizationData
            {
                Tables = new Dictionary<string, LocalizationTable>(),
                Locales = new List<Locale>()
            };
            IsDefaultLocaleEnabled = false;
            IsDefaultTableEnabled = false;
            CreateCommands();

            MainWindow.SetContextChangeChecker(CanChangeContext);
            PropertyChanged += OnPropChanged;
        }

        private void OnPropChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            string name = e.PropertyName;
            if (name == nameof(SelectedTable))
            {
                if (SelectedTable != null)
                {
                    Items = SelectedTable.Items;
                    IsDefaultTableEnabled = true;
                    IsDefaultTable = _data.DefaultTableKey == SelectedTable.TableKey;
                }
                else
                {
                    Items = null;
                    IsDefaultTable = false;
                    IsDefaultTableEnabled = false;
                }
            }
            if (name == nameof(SelectedItem))
            {
                if (SelectedItem != null)
                {
                    Translations = SelectedItem.Translations;
                }
                else
                {
                    Translations = null;
                }
            }
            if (name == nameof(SelectedLocale))
            {
                if (SelectedLocale != null)
                {
                    LocaleName = SelectedLocale.Name;
                    LocaleShortName = SelectedLocale.ShortName;
                    IsDefaultLocaleEnabled = true;
                    IsDefaultLocale = _data.DefaultLocale == SelectedLocale.ShortName;
                }
                else
                {
                    LocaleName = null;
                    LocaleShortName = null;
                    IsDefaultLocaleEnabled = false;
                    IsDefaultLocale = false;
                }
            }
            if (name == nameof(IsDefaultTable) && SelectedTable != null)
            {
                if (IsDefaultTable.HasValue && IsDefaultTable.Value)
                {
                    _data.DefaultTableKey = SelectedTable.TableKey;
                }
                else if (SelectedTable.TableKey == _data.DefaultTableKey)
                {
                    _data.DefaultTableKey = null;
                }
            }
            if (name == nameof(IsDefaultLocale) && SelectedLocale != null)
            {
                if (IsDefaultLocale.HasValue && IsDefaultLocale.Value)
                {
                    _data.DefaultLocale = SelectedLocale.ShortName;
                }
                else if (SelectedLocale.ShortName == _data.DefaultLocale)
                {
                    _data.DefaultLocale = null;
                }
            }
        }
        #endregion

        #region Private Methods

        private bool CanChangeContext()
        {
            if (!_edited)
            {
                return true;
            }

            //Ask the user if they wish to save changes:
            bool? result = GoroMessageBox.Show("Changes Pending", "Do you want to save the changes you've made?", MessageBoxButton.YesNoCancel);

            if (result == null)
            {
                return false;
            }

            //Save changes:
            if (result.Value)
            {
                //test save data:
                try
                {
                    SaveData();
                }
                catch
                {
                    return false;
                }

                //Save data officially
                OnSaveData();
            }

            return true;
        }

        #endregion

        #region Commands

        private void CreateCommands()
        {
            CreateDataCommand = new RelayCommand(OnCreateData);
            OpenDataCommand = new RelayCommand(OnOpenData);
            SaveDataCommand = new RelayCommand(OnSaveData, CanSaveData);
            SaveDataAsCommand = new RelayCommand(OnSaveDataAs);
            ExportDataCommand = new(OnExportData);

            AddTableCommand = new RelayCommand(OnAddTable);
            EditTableCommand = new RelayCommand(OnEditTable, CanEditTable);
            DeleteTableCommand = new RelayCommand(OnDeleteTable, CanEditTable);
            ExportTableCommand = new RelayCommand(OnExportTable, CanEditTable);
            ImportTableCommand = new RelayCommand(OnImportTable);

            AddItemCommand = new RelayCommand(OnAddItem, CanAddItem);
            EditItemCommand = new RelayCommand(OnEditItem, CanEditItem);
            DeleteItemCommand = new RelayCommand(OnDeleteItem, CanEditItem);

            AddTranslationCommand = new RelayCommand(OnAddTrans, CanAddTrans);
            EditTranslationCommand = new RelayCommand(OnEditTrans, CanEditTrans);
            DeleteTranslationCommand = new RelayCommand(OnDeleteTrans, CanEditTrans);

            AddLocaleCommand = new RelayCommand(OnAddLocale, CanAddLocale);
            EditLocaleCommand = new RelayCommand(OnEditLocale, CanEditLocale);
            DeleteLocaleCommand = new RelayCommand(OnDeleteLocale, CanDeleteLocale);
        }

        public RelayCommand CreateDataCommand { get; private set; }
        private void OnCreateData()
        {
            if (!CanChangeContext())
            {
                return;
            }

            //Basically clear everything:
            Tables.Clear();
            Locales.Clear();
            _edited = false;
            _filePath = null;
        }
        public RelayCommand OpenDataCommand { get; private set; }
        private async void OnOpenData()
        {
            if (!CanChangeContext())
            {
                return;
            }

            OpenFileDialog file = new();
            bool? result = file.ShowDialog();
            if (!(result.HasValue && result.Value))
            {
                return;
            }

            _filePath = file.FileName;

            using Stream fsData = file.OpenFile();

            try
            {
                _data = await LocalizationIO.LoadAsync(fsData);
            }
            catch
            {
                _ = GoroMessageBox.Show("Error", "Could not load the selected file. Ensure it is a valid localization data file and try again", MessageBoxButton.OK, StatusInfo.Error);
                return;
            }


            Tables.Clear();
            Locales.Clear();
            Tables.AddRange(_data.GetLocTables());
            Locales.AddRange(_data.GetManagedLocales());

            _edited = false;
        }

        public RelayCommand SaveDataCommand { get; private set; }
        private bool CanSaveData()
        {
            return Tables.Count > 0 && Locales.Count > 0;
        }
        private async void OnSaveData()
        {
            if (string.IsNullOrEmpty(_filePath))
            {
                SaveFileDialog file = new();
                bool? result = file.ShowDialog();
                if (!(result.HasValue && result.Value))
                {
                    return;
                }

                _filePath = file.FileName;
            }
            try
            {
                SaveData();
            }
            catch
            {
                return;
            }
            try
            {
                using FileStream fsOpen = File.Open(_filePath, FileMode.Create);
                await LocalizationIO.SaveAsync(fsOpen, _data);
                _edited = false;
            }
            catch
            {
                _ = GoroMessageBox.Show("Error", "Could not write localization data to the selected file. Ensure Thoth has adequate permissions to access it and that no other process is utilizing it and try again!", MessageBoxButton.OK, StatusInfo.Error);
            }
        }
        private void SaveData()
        {
            try
            {
                _data.SetLocTables(Tables);
                _data.SetManagedLocales(Locales);
            }
            catch
            {
                _ = GoroMessageBox.Show("Error", "Something went wrong when saving the localization data. Ensure that all keys are unique and try again!", MessageBoxButton.OK, StatusInfo.Error);
                throw;
            }
        }

        public RelayCommand SaveDataAsCommand { get; private set; }
        private void OnSaveDataAs()
        {
            SaveFileDialog file = new();
            bool? result = file.ShowDialog();
            if (!(result.HasValue && result.Value))
            {
                return;
            }

            _filePath = file.FileName;

            OnSaveData();
        }

        public RelayCommand AddTableCommand { get; private set; }
        private void OnAddTable()
        {
            InputWindow dlg = new("Provide the table key");
            bool? result = dlg.ShowDialog();
            if (!(result.HasValue && result.Value))
            {
                return;
            }

            string tableKey = dlg.Input;

            LocTable table = new()
            {
                TableKey = tableKey,
                Items = new ObservableCollection<LocItem>()
            };

            Tables.Add(table);
            SelectedTable = table;
            _edited = true;
        }

        public RelayCommand EditTableCommand { get; private set; }
        private bool CanEditTable()
        {
            return SelectedTable != null;
        }
        private void OnEditTable()
        {
            InputWindow dlg = new("Provide a new table key", SelectedTable.TableKey);
            bool? result = dlg.ShowDialog();
            if (!(result.HasValue && result.Value))
            {
                return;
            }

            string tableKey = dlg.Input;

            SelectedTable.TableKey = tableKey;
            _edited = true;
        }

        public RelayCommand DeleteTableCommand { get; private set; }
        private void OnDeleteTable()
        {
            bool? result = GoroMessageBox.Show("Warning", "Are you sure you want to delete this table?", MessageBoxButton.YesNo, StatusInfo.Warning);
            if (!(result.HasValue && result.Value))
            {
                return;
            }

            _ = Tables.Remove(SelectedTable);
            _edited = true;
        }

        public RelayCommand ExportTableCommand { get; private set; }

        private void OnExportTable()
        {
            try
            {
                SaveFileDialog file = new();
                bool? result = file.ShowDialog();
                if (!(result.HasValue && result.Value))
                {
                    return;
                }

                string path = file.FileName;


                LocalizationTable locTable = SelectedTable.ToTable();

                List<Locale> locales = new();

                foreach (ManagedLocale m in Locales)
                {
                    locales.Add(m.ToLocale());
                }

                SideloadTable table = new()
                {
                    Items = locTable.Items,
                    Key = SelectedTable.TableKey,
                    Locales = locales
                };

                string json = JsonSerializer.Serialize(table);

                File.WriteAllText(path, json);
            }
            catch
            {
                _ = GoroMessageBox.Show("Failed", "Table could not be successfully exported. Ensure the data is okay and try again", MessageBoxButton.OK, StatusInfo.Error);
            }
            
        }

        public RelayCommand ImportTableCommand { get; private set; }

        private void OnImportTable()
        {
            try
            {
                OpenFileDialog file = new();
                bool? result = file.ShowDialog();
                if (!(result.HasValue && result.Value))
                {
                    return;
                }

                string json = File.ReadAllText(file.FileName);

                SideloadTable table = JsonSerializer.Deserialize<SideloadTable>(json);

                LocTable m = table.ToMTable(table.Key);

                Tables.Add(m);

                _edited = true;
            }
            catch
            {
                _ = GoroMessageBox.Show("Failed", "That importation failed to happen. Check your data and try again", MessageBoxButton.OK, StatusInfo.Error);
            }
           
        }

        public RelayCommand AddItemCommand { get; private set; }
        private bool CanAddItem()
        {
            return SelectedTable != null;
        }
        private void OnAddItem()
        {
            InputWindow dlg = new("Provide the item's key");
            bool? result = dlg.ShowDialog();
            if (!(result.HasValue && result.Value))
            {
                return;
            }

            string itemKey = dlg.Input;

            LocItem item = new()
            {
                Translations = new ObservableCollection<LocTranslation>(),
                Key = itemKey
            };

            SelectedTable.Items.Add(item);
            SelectedItem = item;
            _edited = true;
        }

        public RelayCommand EditItemCommand { get; private set; }
        private bool CanEditItem()
        {
            return SelectedItem != null;
        }
        private void OnEditItem()
        {
            InputWindow dlg = new("Provide a new item key", SelectedItem.Key);
            bool? result = dlg.ShowDialog();
            if (!(result.HasValue && result.Value))
            {
                return;
            }

            string itemKey = dlg.Input;
            SelectedItem.Key = itemKey;
            _edited = true;
        }

        public RelayCommand DeleteItemCommand { get; private set; }
        private void OnDeleteItem()
        {
            bool? result = GoroMessageBox.Show("Warning", "Are you sure you want to delete this item?", MessageBoxButton.YesNo, StatusInfo.Warning);
            if (!(result.HasValue && result.Value))
            {
                return;
            }

            _ = SelectedTable.Items.Remove(SelectedItem);
            _edited = true;
        }

        public RelayCommand AddTranslationCommand { get; private set; }
        private bool CanAddTrans()
        {
            return SelectedItem != null && Locales != null && Locales.Count > 0;
        }
        private void OnAddTrans()
        {
            TranslationEditor editor = new(Locales);
            bool? result = editor.ShowDialog();
            if (!(result.HasValue && result.Value))
            {
                return;
            }

            LocTranslation trans = editor.Translation;
            SelectedItem.Translations.Add(trans);
            SelectedTranslation = trans;
            _edited = true;
        }

        public RelayCommand EditTranslationCommand { get; private set; }
        private bool CanEditTrans()
        {
            return CanAddTrans() && SelectedTranslation != null;
        }
        private void OnEditTrans()
        {
            TranslationEditor editor = new(Locales, SelectedTranslation);
            bool? result = editor.ShowDialog();
            if (!(result.HasValue && result.Value))
            {
                return;
            }

            LocTranslation trans = editor.Translation;

            SelectedTranslation.Locale = trans.Locale;
            SelectedTranslation.Value = trans.Value;
            _edited = true;
        }

        public RelayCommand DeleteTranslationCommand { get; private set; }
        private void OnDeleteTrans()
        {
            bool? result = GoroMessageBox.Show("Warning", "Are you sure you want to delete this translation?", MessageBoxButton.YesNo, StatusInfo.Warning);
            if (!(result.HasValue && result.Value))
            {
                return;
            }

            _ = SelectedItem.Translations.Remove(SelectedTranslation);
            _edited = true;
        }

        public RelayCommand AddLocaleCommand { get; private set; }
        private bool CanAddLocale()
        {
            return LocaleNotEmpty() && !Locales.Any(x => x.ShortName == LocaleShortName);
        }
        private void OnAddLocale()
        {
            ManagedLocale managedLocale = new()
            {
                Name = LocaleName,
                ShortName = LocaleShortName
            };

            LocaleName = null;
            LocaleShortName = null;

            Locales.Add(managedLocale);
            _edited = true;
            SelectedLocale = managedLocale;
        }

        public RelayCommand EditLocaleCommand { get; private set; }
        private bool CanEditLocale()
        {
            return SelectedLocale != null && LocaleNotEmpty();
        }
        private void OnEditLocale()
        {
            SelectedLocale.Name = LocaleName;
            SelectedLocale.ShortName = LocaleShortName;
            _edited = true;
        }

        public RelayCommand DeleteLocaleCommand { get; private set; }
        private bool CanDeleteLocale()
        {
            return SelectedLocale != null;
        }
        private void OnDeleteLocale()
        {
            bool? result = GoroMessageBox.Show("Warning", "Are you sure you want to delete this locale definition?", MessageBoxButton.YesNo, StatusInfo.Warning);
            if (!(result.HasValue && result.Value))
            {
                return;
            }

            _ = Locales.Remove(SelectedLocale);
            _edited = true;
        }

        private bool LocaleNotEmpty()
        {
            return !string.IsNullOrEmpty(LocaleName)
                && !string.IsNullOrEmpty(LocaleShortName);
        }

        public RelayCommand ExportDataCommand { get; private set; }

        private void OnExportData()
        {
            try
            {
                LocalizationData data = new();
                data.SetLocTables(Tables);
                data.SetManagedLocales(Locales);
                GenerateWindow window = new(data);
                window.ShowDialog();
            }
            catch
            {
                _ = GoroMessageBox.Show("Error", "Operation failed. Ensure keys are unique and that you have access to the provided directory", MessageBoxButton.OK, StatusInfo.Error);
                throw;
            }
        }

        #endregion
    }
}
