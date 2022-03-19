using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
    /// AddKeyDialog.xaml 的交互逻辑
    /// </summary>
    public partial class AddKeyDialog : UserControl, INotifyPropertyChanged
    {
        private ProdKeyItem prodKeyItem;

        public AddKeyDialog()
        {
            InitializeComponent();
            this.DataContext = this;

            ProdKeyItem = new ProdKeyItem("", "");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ProdKeyItem ProdKeyItem
        {
            get => prodKeyItem;
            set => SetProperty(ref prodKeyItem, value);
        }
    }
}
