using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace ProdKeysManager
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private ObservableCollection<ProdKeyItem> prodKeys = new ObservableCollection<ProdKeyItem>();
        private ObservableCollection<string> managedKeyFiles = new ObservableCollection<string>();
        private GridViewColumnHeader listViewSortCol;
        private ListSortDirection listViewSortDir;
        private bool isEditingKeys;
        private bool isEditingFiles;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<ProdKeyItem> ProdKeys
        {
            get => prodKeys;
            set => SetProperty(ref prodKeys, value);
        }

        public ObservableCollection<string> ManagedKeyFiles
        {
            get => managedKeyFiles;
            set => SetProperty(ref managedKeyFiles, value);
        }

        public bool IsEditingKeys
        {
            get => isEditingKeys;
            set => SetProperty(ref isEditingKeys, value);
        }

        public bool IsEditingFiles
        {
            get => isEditingFiles;
            set => SetProperty(ref isEditingFiles, value);
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            // Load managed key files
            var managedKeyFilesSetting = Properties.Settings.Default.ManagedKeyFiles;
            if (managedKeyFilesSetting != null)
            {
                foreach (var managedKeyFile in managedKeyFilesSetting)
                {
                    ManagedKeyFiles.Add(managedKeyFile);
                }
            }

            // Load prod keys
            var prodKeysSetting = Properties.Settings.Default.ProdKeys;
            if (prodKeysSetting != null)
            {
                foreach (var prodKey in prodKeysSetting)
                {
                    var kv = prodKey.Split('=');
                    if (kv.Length == 2)
                    {
                        ProdKeys.Add(new ProdKeyItem(kv[0].Trim(), kv[1].Trim()));
                    }
                }
            }

            // Listen to ProdKeys changed
            ProdKeys.CollectionChanged += (s, args) =>
            {
                SaveProdKeys();
            };
        }

        private void SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void AddManagedKeyFile(string path)
        {
            ManagedKeyFiles.Add(path);
            var keys = ProdKeyHelper.LoadProdKeys(path);
            keys.Where(k => !prodKeys.Any(p => p.KeyName == k.KeyName)).ToList().ForEach(k => ProdKeys.Add(k));

            // Save the managed key file list to app.config
            var managedKeyFilesSetting = Properties.Settings.Default.ManagedKeyFiles;
            if (managedKeyFilesSetting == null)
            {
                managedKeyFilesSetting = new StringCollection();
                Properties.Settings.Default.ManagedKeyFiles = managedKeyFilesSetting;
            }
            managedKeyFilesSetting
                .AddRange(
                    ManagedKeyFiles
                    .Where(f => !managedKeyFilesSetting.Contains(f))
                    .ToArray()
                );
            Properties.Settings.Default.Save();
        }

        private void RemoveManagedKeyFile(string path)
        {
            ManagedKeyFiles.Remove(path);

            // Save the managed key file list to app.config
            var managedKeyFilesSetting = Properties.Settings.Default.ManagedKeyFiles;
            if (managedKeyFilesSetting != null)
            {
                managedKeyFilesSetting
                    .Remove(path);
                Properties.Settings.Default.Save();
            }
        }

        private void RemoveProdKey(ProdKeyItem prodKey)
        {
            ProdKeys.Remove(prodKey);

            // Save the prod key list to app.config
            var prodKeysSetting = Properties.Settings.Default.ProdKeys;
            if (prodKeysSetting != null)
            {
                prodKeysSetting
                    .Remove(prodKey.KeyName + "=" + prodKey.KeyValue);
                Properties.Settings.Default.Save();
            }
        }

        private void SaveProdKeys()
        {
            var prodKeysSetting = new StringCollection();
            prodKeysSetting.AddRange(ProdKeys.Select(k => $"{k.KeyName}={k.KeyValue}").ToArray());
            Properties.Settings.Default.ProdKeys = prodKeysSetting;
            Properties.Settings.Default.Save();
        }

        private void UpdateAllManagedKeyFiles()
        {
            foreach (var path in ManagedKeyFiles)
            {
                var folder = Path.GetDirectoryName(path);
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
                File.WriteAllText(path, string.Join(Environment.NewLine, ProdKeys.Select(k => $"{k.KeyName} = {k.KeyValue}")));
            }
        }

        private void AddFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "*.txt; prod.keys|*.txt;prod.keys|All files (*.*)|*.*";
            openFileDialog.Multiselect = true;
            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string path in openFileDialog.FileNames)
                {
                    AddManagedKeyFile(path);
                }
            }
        }

        private void RemoveFile_Click(object sender, RoutedEventArgs e)
        {
            RemoveManagedKeyFile((string)((Button)sender).DataContext);
        }

        private void RemoveKey_Click(object sender, RoutedEventArgs e)
        {
            RemoveProdKey((ProdKeyItem)((Button)sender).DataContext);
        }

        private async void AddKey_Click(object sender, RoutedEventArgs e)
        {
            var pki = (ProdKeyItem)await MaterialDesignThemes.Wpf.DialogHost.Show(new AddKeyDialog(), "MainDialogHost");
            if (pki != null && !string.IsNullOrWhiteSpace(pki.KeyName) && !string.IsNullOrWhiteSpace(pki.KeyValue))
            {
                ProdKeys.Add(pki);
            }
        }

        private async void BatchAddKeys_Click(object sender, RoutedEventArgs e)
        {
            var prodKeys = (IList<ProdKeyItem>)await MaterialDesignThemes.Wpf.DialogHost.Show(new BatchAddKeysDialog(), "MainDialogHost");
            if (prodKeys != null)
            {
                prodKeys.Where(k => !prodKeys.Any(p => p.KeyName == k.KeyName)).ToList().ForEach(k => ProdKeys.Add(k));
            }
        }

        private void SyncKeys_Click(object sender, RoutedEventArgs e)
        {
            UpdateAllManagedKeyFiles();
        }

        private void KeyViewHeader_Click(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader column = sender as GridViewColumnHeader;
            string sortBy = column.Tag.ToString();
            if (listViewSortCol != null)
            {
                KeyView.Items.SortDescriptions.Clear();
            }

            ListSortDirection newDir = ListSortDirection.Ascending;
            if (listViewSortCol == column)
                newDir = listViewSortDir == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;

            listViewSortCol = column;
            listViewSortDir = newDir;
            KeyView.Items.SortDescriptions.Add(new SortDescription(sortBy, newDir));
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            this.IsEditingFiles = !this.IsEditingFiles;
            this.IsEditingKeys = !this.IsEditingKeys;
        }
    }

    public class ProdKeyItem
    {
        public string KeyName { get; set; }
        public string KeyValue { get; set; }

        public ProdKeyItem(string keyName, string keyValue)
        {
            KeyName = keyName;
            KeyValue = keyValue;
        }
    }

    public class ManagedKeyFilesSection : ConfigurationSection
    {
        public string[] ManagedKeyFiles { get; set; }
    }


}
