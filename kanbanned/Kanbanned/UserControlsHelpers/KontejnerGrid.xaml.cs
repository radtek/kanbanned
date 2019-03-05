using Kanbanned.Models;
using Kanbanned.ViewModels;
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
using MaterialDesignColors;
using MaterialDesignThemes;

namespace Kanbanned.UserControlsHelpers
{
    /// <summary>
    /// Interaction logic for KontejnerZadataka.xaml
    /// </summary>
    public partial class KontejnerGrid : Grid, IKontejnerGrid
    {
        private ApplicationViewModel vm;
        public KontejnerGrid(ApplicationViewModel vm)
        {
            InitializeComponent();
            this.vm = vm;
        }

        //dodavanje podkolone
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button dodajKolonu = sender as Button;

			//kolona kojoj se dodaje nova kolona
			// 10.06.2018. mstankovic
			//Kontejner kolona = (Kontejner)dodajKolonu.Tag;
			KontejnerFaza kolona = (KontejnerFaza)dodajKolonu.Tag;

			UnosNaziva ctrl = new UnosNaziva();
            if (ctrl.ShowDialog() == true)
            {
                // ukoliko je kreiran projekat vec i postoji parent kolona onda se dodaje nova
                // a ako ne onda se ceka da se doda tek kad se ide na done i zavrsi kreiranje projekta
               
                vm.TrenutniProjekat.TabelaProjekta.DodajKolonu(kolona, ctrl.Naziv, vm.TrenutniProjekat.Id);
                vm.PostaviTrenutniProjekat();
            }
       }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //brisanje kolone
            Button remove = sender as Button;
            Kontejner kolona = (Kontejner)remove.Tag;
            vm.TrenutniProjekat.TabelaProjekta.IzbrisiFazu(kolona);
            vm.PostaviTrenutniProjekat();
        }

        private void labela_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //promena imena kolone
            Label promeniKolonu = sender as Label;
            KontejnerFaza kolona = (KontejnerFaza)promeniKolonu.Tag;

            UnosNaziva ctrl = new UnosNaziva();
            if (ctrl.ShowDialog() == true)
            {
                if (!ctrl.Naziv.Equals(""))
                {
                    kolona.Ime = ctrl.Naziv;
					if (kolona.Id != 0)
						try
						{
							Packages.PFaza.IzmeniInformacije(kolona.Id, kolona.Ime, kolona.Opis, kolona.PocetakIzrade, kolona.KrajIzrade);
						}
						catch (Oracle.ManagedDataAccess.Client.OracleException oraError)
						{
							try
							{
								System.Windows.MessageBox.Show(Packages.PPoruka.VratiPrevod(oraError.Number.ToString()));
							}
							catch (Oracle.ManagedDataAccess.Client.OracleException err)
							{
								System.Windows.MessageBox.Show("Greska pri izmeni informacija faze");
							}
						}

					vm.PostaviTrenutniProjekat();
                }            
            }
        }

        // u slucaju da se zadatak dropuje preko kontejnera faze
        // zadatak bi trebalo da se otkljuca, jer se nije nista desilo, a zavrsena je akcija
        private void Grid_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("myZad"))
            {
                Zadatak z = e.Data.GetData("myZad") as Zadatak;

                if (z.Id != 0 && Packages.PZadatak.IsZakljucan(z.Id))
                {
                    Packages.PZadatak.Otkljucaj(z.Id);
                }
            }
        }
    }
}
