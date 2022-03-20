using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProdKeysManager
{
    /// <summary>
    /// BatchAddKeysDialog.xaml 的交互逻辑
    /// </summary>
    public partial class BatchAddKeysDialog : UserControl
    {
        public BatchAddKeysDialog()
        {
            InitializeComponent();
        }

        public List<ProdKeyItem> ProdKeyItems
        {
            get
            {
                var prodKeys = new List<ProdKeyItem>();
                var lines = batchAddKeysTextbox.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in lines)
                {
                    var kv = line.Split('=');
                    var prodKey = new ProdKeyItem(kv[0].Trim(), kv[1].Trim());
                    prodKeys.Add(prodKey);
                }
                return prodKeys;
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            MaterialDesignThemes.Wpf.DialogHost.Close("MainDialogHost", ProdKeyItems);
        }
    }
}
