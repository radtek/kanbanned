using Kanbanned.Models;
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

namespace Kanbanned.UserControlsHelpers
{
    /// <summary>
    /// Interaction logic for DetaljiZadatak.xaml
    /// </summary>
    public partial class DetaljiZadatak : MetroWindow
    {
        // promenljiva na osnovu koje se ispita da li je bilo promena detalja o zadatku
        // i tek ako je bilo se vrsi upis u bazu
        private bool promena;

        public DetaljiZadatak(Zadatak z, Projekat p)
        {
            InitializeComponent();
            this.DataContext = z;
            this.Tag = p;
            this.Show();         
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //dodaj novi komentar
            String komentar = new TextRange(tbNoviKomentar.Document.ContentStart, tbNoviKomentar.Document.ContentEnd).Text;
            String ime = Korisnik.KorisnickoIme;
            ((Zadatak)this.DataContext).DodajKomentar(ime, komentar);
        }

        private void MetroWindow_ContentRendered(object sender, EventArgs e)
        {
            this.Focus();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //dodaj novi checkpoint
            String naziv = new TextRange(tbKontrolnaTacka.Document.ContentStart, tbKontrolnaTacka.Document.ContentEnd).Text;
            ((Zadatak)this.DataContext).DodajKontrolnuTacku(naziv);

            ((Zadatak)this.DataContext).KontrolnaTackaChanged();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            ((Zadatak)this.DataContext).KontrolnaTackaChanged();
        }

        private void detalji_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (((Projekat)(this.Tag)).Id != 0 && !((Projekat)(this.Tag)).Privilegija.Equals("RADNIK"))
            {
                if(promena)
                {
                    Zadatak z = ((Zadatak)this.DataContext);
					try
					{
						Packages.PZadatak.IzmeniInformacije(z.Id, z.Ime, z.Opis, z.PocetakIzrade, z.KrajIzrade, z.Tip);
						Packages.PZadatak.IzmeniStatus(z.Id, z.Status);
					}
					catch (Oracle.ManagedDataAccess.Client.OracleException oraError)
					{
						try
						{
							System.Windows.MessageBox.Show(Packages.PPoruka.VratiPrevod(oraError.Number.ToString()));
						}
						catch (Oracle.ManagedDataAccess.Client.OracleException err)
						{
							System.Windows.MessageBox.Show("Greska");
						}
					}
                }
            }
            //izmena podataka 
        }

        // promenilo se ime
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            promena = true;
        }

        // promenio se datum pocetka izrade
        private void dpStartDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            promena = true;
        }

        // promenio se datum kraja izrade
        private void dpFinishDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            promena = true;
        }

        // promenio se opis
        private void tbOpis_TextChanged(object sender, TextChangedEventArgs e)
        {
            promena = true;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            promena = true;
            if(((Zadatak)this.DataContext).Id == 0)
            {
                ((Zadatak)this.DataContext).Status = ZStatus.NotStarted;
            }
        }
    }
}
