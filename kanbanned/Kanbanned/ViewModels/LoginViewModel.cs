using Kanbanned.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

using Oracle.ManagedDataAccess.Client;
using Kanbanned.Views;
using Kanbanned.Models;
using Kanbanned.Packages;

namespace Kanbanned.ViewModels
{
    public class LoginViewModel : ObservableObject, IViewModel
    {
        //username i password koji ce se koristiti da se pretrazi baza i izvristi login korisnika
        private string username;
        private string password;

        /// <summary>
        /// Prosledjuje mu se MainViewModel pa ce on onda odavde da ga ugasi nakon logovanja
        /// </summary>
        private MainViewModel mainViewModel;
        public LoginViewModel(MainViewModel vm)
        {
            this.mainViewModel = vm;
        }

        //komanda koja ce se izvrsiti nakon pritiska na login dugme u LoginView
        private ICommand _loginCommand;
        //go back komanda bi mozda trebalo da se napravi kao posebna komanda jer se koristi na vise mesta
        private ICommand _goBackCommand;
        
        //action za zatvaranje prozora
        public Action CloseAction { get; set; }
        public string Username
        {
            get { return this.username; }
            set
            {
                this.username = value;
                OnPropertyChanged("Username");
            }
        }
        

        public ICommand LoginCommand
        {
            get
            {
                if (_loginCommand == null)
                {
                    _loginCommand = new RelayCommand(
                        param => Login(param),
                        param => true
                    );
                }
                return _loginCommand;
            }
        }
        public ICommand GoBackCommand
        {
            get
            {
                if (_goBackCommand == null)
                {
                    _goBackCommand = new RelayCommand(
                        param => GoBack(),
                        param => true
                    );
                }
                return _goBackCommand;
            }
        }

        private void Login(object param)
        {
            //ovde se radi login
            //vrsi se pretraga baze sa atributima ovog view modela
            //mora ovako da se izvadi password jer ne moze da se binduje password iz
            //bezbednosnih razloga
            //moze preko passwordboxhelpera ali je komplikovanije 
            this.password = ((PasswordBox)param).Password;

			try
			{
				if (!PKorisnik.Postoji(this.username))
					throw new Exception("U_NEXIST");

				PKorisnik.Login(this.username, this.password);
				try
				{
					System.Windows.MessageBox.Show(PPoruka.VratiPrevod("S_LOGIN"));
				}
				catch (OracleException er)
				{
					System.Windows.MessageBox.Show("Uspesno ste se ulogovali.");
				}

				Korisnik.KorisnickoIme = this.username;
				PKorisnik.Ucitaj_Podatke();

				Korisnik.Projekti = PProjekat.VratiSveProjekte(Korisnik.KorisnickoIme);
				Korisnik.Mejlovi = PEmail.VratiSve(Korisnik.KorisnickoIme);

				ApplicationView av = new ApplicationView(this.mainViewModel);

                //ako je uspesan login onda se zatvara ova stranica
				this.CloseView();
                
                //nakon ovoga MainView je i dalje Hide-ovan i tako je tokom celokupnog rada aplikacije
                //prikazuje se tek kada korisnik izvrsi logout
			}
			catch (OracleException oraError)
			{
				try
				{
					System.Windows.MessageBox.Show(PPoruka.VratiPrevod(oraError.Number.ToString()));
				}
				catch(OracleException err)
				{
					System.Windows.MessageBox.Show("Greska");
				}
			}
			catch (System.Exception error)
			{
				try
				{
					System.Windows.MessageBox.Show(PPoruka.VratiPrevod(error.Message));
				}
				catch (OracleException er)
				{
					System.Windows.MessageBox.Show("Greska");
				}
			}
			//nakon ovoga ide prelaz iz LoginView u ApplicationView
            //ovde treba da se vrati objekat tipa Korisnik i da se prosledi ApplicationViewModel-u

		}
        private void GoBack()
        {
            //trebalo bi ovde da se kaze glavnom viewmodelu da promeni na prethodnu stranicu
            //kada se ide na "Back" samo se pozove Show za MainView, jer je prethodon stavljen na Hide
            this.mainViewModel.ShowAction();

            //LoginView se zatvara
            this.CloseView();
        }
        private void CloseView()
        {
            this.CloseAction();
        }
    }
}
