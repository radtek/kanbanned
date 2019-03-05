using Kanbanned.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

using Oracle.ManagedDataAccess.Client;

namespace Kanbanned.ViewModels
{
    public class RegisterViewModel : ObservableObject, IViewModel
    {
        /// <summary>
        /// na osnovu ovih atributa se vrsi registracija u bazi
        /// </summary>
        private string username;
        private string password;
        private string email;
        private string companyName;
        private string confirmPassword;

        private MainViewModel mainViewModel;
        //isto vazi kao i za LoginViewModel
        public RegisterViewModel(MainViewModel vm)
        {
            this.mainViewModel = vm;
        }

        private ICommand _registerCommand;
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

        public string Email
        {
            get { return this.email; }
            set
            {
                this.email = value;
                OnPropertyChanged("Email");
            }
        }
        public string CompanyName
        {
            get { return this.companyName; }
            set
            {
                this.companyName = value;
                OnPropertyChanged("CompanyName");
            }
        }
        public ICommand RegisterCommand
        {
            get
            {
                if (_registerCommand == null)
                {
                    _registerCommand = new RelayCommand(
                        param => Register(param),
                        param => true
                    );
                }
                return _registerCommand;
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

        private void Register(object param)
        {
            //converter ce da konvertuje vise parametara u jedan niz passwordboxova koji ce onda
            //da se proslede komandi kao jedna parametar i komanda ce se izvrsiti sa njima
            Object[] o = param as Object[];
            this.password = ((PasswordBox)o[0]).Password;
            this.confirmPassword = ((PasswordBox)o[1]).Password;

            try
            {
				if (this.password != this.confirmPassword)
					throw new Exception("RPW_ERROR");

				Packages.PKorisnik.Register(this.username, this.password, this.companyName);
                Packages.PEmail.Dodaj(this.email, this.username);

				try
				{
					System.Windows.MessageBox.Show(Packages.PPoruka.VratiPrevod("SUC_REG"));
				}
				catch (OracleException oraError)
				{
					System.Windows.MessageBox.Show("Uspesno ste se registrovali.");
				}

                //ukoliko se sve pravilno onda se odradi komanda "Back"
                GoBack();
            }
            catch (OracleException oraError)
            {
				System.Windows.MessageBox.Show(Packages.PPoruka.VratiPrevod(oraError.Number.ToString()));
			}
            catch (System.Exception error)
            {
					try
					{
						System.Windows.MessageBox.Show(Packages.PPoruka.VratiPrevod(error.Message));
					}
					catch (OracleException oraError)
					{
						System.Windows.MessageBox.Show("Doslo je do greske");
					}
            }
        }
        private void GoBack()
        {
            //otkrije se MainView
            this.mainViewModel.ShowAction();

            //RegisterView se zatvara
            this.CloseView();
        }

        private void CloseView()
        {
            this.CloseAction();
        }
    }
}
