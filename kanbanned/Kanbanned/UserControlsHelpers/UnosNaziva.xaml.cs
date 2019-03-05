using Kanbanned.Models;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Kanbanned.UserControlsHelpers
{
    /// <summary>
    /// Interaction logic for UnosNaziva.xaml
    /// </summary>
    public partial class UnosNaziva : MetroWindow
    {

        public UnosNaziva()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //add

            if(tbNaziv.Text != null && !tbNaziv.Text.Equals(""))
            {
                Naziv = tbNaziv.Text;
                this.DialogResult = true;

            }           
        }

        public String Naziv { get; set; }

        private void MetroWindow_ContentRendered(object sender, EventArgs e)
        {
            tbNaziv.SelectAll();
            tbNaziv.Focus();
        }
    }
}
