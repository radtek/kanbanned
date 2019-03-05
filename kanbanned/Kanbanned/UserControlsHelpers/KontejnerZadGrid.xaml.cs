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

namespace Kanbanned.UserControlsHelpers
{
    /// <summary>
    /// Interaction logic for KontejnerZadGrid.xaml
    /// </summary>
    public partial class KontejnerZadGrid : Grid, IKontejnerGrid
    {
        private ApplicationViewModel vm;
        public KontejnerZadGrid(ApplicationViewModel vm)
        {
            InitializeComponent();
            this.vm = vm;
        }

        private void UserControl_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("myZad"))
            {
                //provera da li mozda ima max broja zadataka u koloni gde hocemo da ubacimo novi zadatak
                KontejnerZadGrid kontejner = sender as KontejnerZadGrid;

                //izvuce se KontejnerZadGrid u koji se stavlja
                Border bor1 = (Border)kontejner.Children[1];
                Grid kolonaContentGrid = (Grid)bor1.Child;

                //izvuce se model podataka kolone zadataka
                Kontejner kz = (Kontejner)(((Grid)kontejner).Tag);

                if(((KontejnerZadataka)kz).Zadaci.Count != ((KontejnerZadataka)kz).MaxBrZadataka)
                {
                    //izvuce se zadatak
                    Zadatak z = e.Data.GetData("myZad") as Zadatak;

                    //zakomentarisi ako necemo otkljucavanje
                    //lock zadatka u bazi
                    if (z.Id != 0)
                    {
						try
						{
							Packages.PZadatak.Otkljucaj(z.Id);
						}
						catch (Oracle.ManagedDataAccess.Client.OracleException oraError)
						{
							try
							{
								System.Windows.MessageBox.Show(Packages.PPoruka.VratiPrevod(oraError.Number.ToString()));
							}
							catch (Oracle.ManagedDataAccess.Client.OracleException err)
							{
								System.Windows.MessageBox.Show("Error");
							}
						}
					}

					try
					{
						// pomeranje zadatka u drugu fazu i usnimanje promene u bazi
						// treba sve da se stavi u if ovo ispod jer moze da dodje do slucaja da se
						// izbrise faza a onda neko pokusa da ubaci nesto u nju
						// ako se to desi treba da se obradi exception i kaze korisniku da prvo treba da refreshuje
						if (kz.Id != 0 && z.Id != 0) Packages.PZadatak.IzmeniFazu(z.Id, kz.Id);

						//menjaju se informacije zadataka vezane za njegovu poziciju
						((KontejnerZadataka)kz).Zadaci.Add(z);
						((KontejnerZadataka)kz).PreurediZadatke();

						//trebalo bi da se ostali zadaci pomere za mesto unazad da i da im se izmene indeksi 
						//remove bi trebalo to da uradi sama
						z.Roditelj.Zadaci.Remove(z);
						z.Roditelj.PreurediZadatke();
						z.Roditelj = ((KontejnerZadataka)kz);
						//ovde funkcija koja ce ponovo da namesti indekse zadacima

						this.vm.PostaviTrenutniProjekat();

						//!!! ISPITATI SLUCAJ UKOLIKO NEMA DOVOLJNO ROW ILI COLUMN DEFINICIJA 
						//TAKO DA SE UBACI NOVI I TEK ONDA UBACI ZADATAK
						//u zavisnosti od max sirine za vertical split i obrnuto
						//odredjuje se indeks zadatka
					}
					catch (Oracle.ManagedDataAccess.Client.OracleException oraError)
					{
						try
						{
							System.Windows.MessageBox.Show(Packages.PPoruka.VratiPrevod(oraError.Number.ToString()));
						}
						catch (Oracle.ManagedDataAccess.Client.OracleException err)
						{
							System.Windows.MessageBox.Show("Error");
						}
					}
				}
            }
        }

        private void UserControl_DragEnter(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent("myZad") || sender == e.Source)
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //dodavanje zadatka u kolonu
            Button dodajKolonu = sender as Button;

			//kolona kojoj se dodaje novi zadatak
			// 10.06.2018. mstankovic (izmenjen i gornji komentar u "...novi zadatak", pre je bilo "...nova kolona")
			//Kontejner kolona = (Kontejner)dodajKolonu.Tag;
			KontejnerZadataka kolona = (KontejnerZadataka)dodajKolonu.Tag;

			vm.TrenutniProjekat.TabelaProjekta.DodajZadatak(kolona);
            vm.PostaviTrenutniProjekat();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //splitovanje kolone
            Button split = sender as Button;
			// 10.06.2018. mstankovic
			//Kontejner levaPodkolona = (Kontejner)split.Tag;
			KontejnerZadataka kolonaZaSplit = (KontejnerZadataka)split.Tag;

			VerticalHorizontalSplit vhs = new VerticalHorizontalSplit();
            if(vhs.ShowDialog() == true)
            {
                vm.TrenutniProjekat.TabelaProjekta.Split(kolonaZaSplit, vhs.VerticalOrHorizontal, vm.TrenutniProjekat.Id);
                vm.PostaviTrenutniProjekat();
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            //pomeranje u desno
            Button split = sender as Button;
            Kontejner kolona = (Kontejner)split.Tag;
            vm.TrenutniProjekat.TabelaProjekta.Pomeri(kolona, false);
            vm.PostaviTrenutniProjekat();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            //pomeranje u levo
            Button split = sender as Button;
            Kontejner kolona = (Kontejner)split.Tag;
            vm.TrenutniProjekat.TabelaProjekta.Pomeri(kolona, true);
            vm.PostaviTrenutniProjekat();
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
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
            KontejnerZadataka kolona = (KontejnerZadataka)promeniKolonu.Tag;

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
								System.Windows.MessageBox.Show("Error");
							}
						}
					vm.PostaviTrenutniProjekat();
                }
            }
        }
    }
}
