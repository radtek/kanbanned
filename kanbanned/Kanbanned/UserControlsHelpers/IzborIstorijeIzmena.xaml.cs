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
using static Kanbanned.Packages.PPromena;

namespace Kanbanned.UserControlsHelpers
{
    /// <summary>
    /// Interaction logic for IzborIstorijeIzmena.xaml
    /// </summary>
    public partial class IzborIstorijeIzmena : MetroWindow
    {
        public int BrojPromena { get; set; }
        public TipIstorije Tip { get; set; }
        public IzborIstorijeIzmena()
        {
            InitializeComponent();
            BrojPromena = 0;
        }

        private void IstorijaFaza_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            Tip = TipIstorije.Faze;
        }

        private void IstorijaZadataka_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            Tip = TipIstorije.Zadaci;
        }
    }
}
