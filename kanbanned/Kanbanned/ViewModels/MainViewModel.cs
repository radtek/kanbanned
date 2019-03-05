using Kanbanned.Helpers;
using System.Windows.Input;
using MahApps.Metro.Controls;

using Oracle.ManagedDataAccess.Client;
using System;

namespace Kanbanned
{
    public class MainViewModel : ObservableObject, IViewModel
    {
        /// <summary>
        /// Zbog zatvaranja View-a zato sto je prilicno komplikovano da se binduje
        /// Ne bi trebalo da postoji dependency injection izmedju View i ViewModela
        /// vec bi trebalo da postoje veze samo na osnovu komandi i bindovanja propertija
        /// </summary>
        public MainViewModel()
        {
			//otvara se konekcija
			//DBConnection.Open();

			//Kanbanned.ViewModels.TableViewModel tbl = new Kanbanned.ViewModels.TableViewModel();
			//tbl.InvokeNacrtaj();
		}
		//komande za otvaranje prozora za login i registraciju
		private ICommand _loginOpenCommand;
        private ICommand _registerOpenCommand;

        //action koji ce da se pokrene kad treba da se zatvori prozor
        //nije u skladu sa MVVM-om jer se u behind code dodeljuje event ovom actionu
        //ali da ne komplikujemo sa dodatnim komandama
        //inace moze da se prosledi Window kao parametar komandi i onda da se na neki nacin zatvori
        public Action CloseAction { get; set; }
        public Action HideAction { get; set; }
        public Action ShowAction { get; set; }
        #region Komande

        public ICommand LoginOpenCommand
        {
            get
            {
                if (_loginOpenCommand== null)
                {
                    _loginOpenCommand = new RelayCommand(
                        param => OpenWindow("login"),
                        param => true
                    );
                }
                return _loginOpenCommand;
            }
        }

        public ICommand RegisterOpenCommand
        {
            get
            {
                if (_registerOpenCommand == null)
                {
                    _registerOpenCommand = new RelayCommand(
                        param => OpenWindow("register"),
                        param => true
                    );
                }
                return _registerOpenCommand;
            }
        }

        #endregion

        //private helpers
        private void OpenWindow(string s)
        {
			if (s == "login")
            {
                //prvo se napravi view i dodeli mu se trenutni
                LoginView lv = new LoginView(this);

                //sakrije se kada se otvori login
                HideView();              
            }
            else if(s == "register")
            {
                RegisterView rw = new RegisterView(this);

                //sakrije se kada se otvori register
                HideView();
            }          
        }
        private void HideView()
        {
            this.HideAction();
        }
        private void CloseView()
        {
            this.CloseAction();

            //zatvara se konekcija tek kada se ugasi MainView
            DBConnection.Close();
        }
        private void ShowView()
        {
            this.ShowAction();
        }
    }
}
