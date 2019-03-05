using Kanbanned.ViewModels;
using MahApps.Metro.Controls;
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
using System.Windows.Shapes;

namespace Kanbanned
{
    /// <summary>
    /// Interaction logic for RegisterView.xaml
    /// </summary>
    public partial class RegisterView : MetroWindow
    {
        public RegisterView(MainViewModel mvm)
        {
            InitializeComponent();
            RegisterViewModel vm = new RegisterViewModel(mvm);
            if(vm.CloseAction == null)
            {
                vm.CloseAction = new Action(() => this.Close()); //dodaje se handler za zatvaranje prozora
            }
            this.DataContext = vm;
            this.Show();
        }
    }
}
