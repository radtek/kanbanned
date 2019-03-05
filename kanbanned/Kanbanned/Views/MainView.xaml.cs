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
using Kanbanned.UserControlsHelpers;
using MahApps.Metro.Controls;

namespace Kanbanned
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : MetroWindow
    {
        public MainView()
        {
            InitializeComponent();

            //ovo je glavni prozor u kome ce se smenjivati stranice aplikacije
            //zbog toga se stavi trenutni ViewModel kao DataContext i onda u zavisnosti od trenutnog
            //prikaze se odredjena stranica vezana za taj trenutni ViewModel
            //ApplicationViewModel viewModel = new ApplicationViewModel();
            LoginDemo.App.SelectCulture("en");
            Globals.Jezik = "EN";

            MainViewModel viewModel = new MainViewModel();
            if(viewModel.CloseAction == null && viewModel.HideAction == null && viewModel.ShowAction == null)
            {
                viewModel.CloseAction = new Action(() => this.Close()); //dodaje se handler za zatvaranje prozora
                viewModel.HideAction = new Action(() => this.Hide());
                viewModel.ShowAction = new Action(() => this.Show());
            }          
            this.DataContext = viewModel;
            this.Show();
            
        }

        // za izbor jezika klik na ikonicu
        private void PackIcon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //za izmenu jezika
            BiranjeJezika Jezik = new BiranjeJezika();
            if (Jezik.ShowDialog() == true)
            {
                if (Jezik.Jezik.Equals("EN") && Globals.Jezik.Equals("RS"))
                {
                    Globals.Jezik = "EN";
                    LoginDemo.App.SelectCulture("en");
                }
                else if (Jezik.Jezik.Equals("RS") && Globals.Jezik.Equals("EN"))
                {
                    Globals.Jezik = "RS";
                    LoginDemo.App.SelectCulture("sr");
                }
            }
            Jezik.Close();
        }
    }
}
