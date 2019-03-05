using Kanbanned.Helpers;
using Kanbanned.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using Oracle.ManagedDataAccess.Client;
using Kanbanned.Packages;
using System.Windows;
using System.Windows.Controls;

namespace Kanbanned.ViewModels
{
    public class StartViewModel : ObservableObject, IViewModel
    {
        /// <summary>
        /// Ovaj ViewModel treba da prikaze listu projekata na kojima je angazovan korisnik
        /// zbog toga treba da mu se prenese u konstruktor lista projekata koju treba prikazati
        /// </summary> 


        private ListaProjekata _projekti;

        private ApplicationViewModel parentAppVM;

        public StartViewModel(IViewModel vm)
        {
            parentAppVM = (ApplicationViewModel)vm;

            OsveziListuProjekata();            			
		}

        //osvezava listu projekata tako sto ponovo ucita celu listu
        public void OsveziListuProjekata()
        {
            Projekti = new ListaProjekata();
            if (Korisnik.Projekti != null)
            {
                foreach (Projekat p in Korisnik.Projekti.Keys)
                {
                    Dictionary<String, String> radnici = PProjekat.VratiKorisnike(p.Id);
                    foreach(KeyValuePair<String, String> radnik in radnici)
                    {
                        if(radnik.Key != Korisnik.KorisnickoIme)
                        {
                            p.RadniciNaProjektu.Add(new RadnikNaProjektu(radnik.Key, radnik.Value));
                        }                      
                    }
                    Projekti.Add(p);
                }
            }
        }
        /// <summary>
        /// Komande
        /// </summary>
        private ICommand _openProjectCommand;
        private ICommand _createNewCommand;
        private ICommand _editCommand;
        private ICommand _removeCommand;

        public ICommand EditCommand
        {
            get
            {
                if (_editCommand == null)
                {
                    _editCommand = new RelayCommand(
                        p => EditProject(p),
                        p => true);
                }

                return _editCommand;
            }
        }
        public ICommand RemoveCommand
        {
            get
            {
                if (_removeCommand == null)
                {
                    _removeCommand = new RelayCommand(
                        p => RemoveProject(p),
                        p => true);
                }

                return _removeCommand;
            }
        }
        public ICommand OpenProjectCommand
        {
            get
            {
                if (_openProjectCommand == null)
                {
                    _openProjectCommand = new RelayCommand(
                        p => OpenProject(p),
                        p => true);
                }

                return _openProjectCommand;
            }        
        }

        
        public ICommand CreateNewCommand
        {
            get
            {
                if (_createNewCommand == null)
                {
                    _createNewCommand = new RelayCommand(
                        p => CreateNewProject(),
                        p => true);
                }

                return _createNewCommand;
            }
        }
		#region Propertiji

		public Action CloseAction { get; set; }
		private String _pageTitle;

        public String PageTitle
        {
            get { return _pageTitle; }
            set { _pageTitle = value; }
        }

        public ListaProjekata Projekti
        {
            get { return _projekti; }
            set
            {
                _projekti = value;
                OnPropertyChanged("Projekti");
            }
        }

		public int ProjektiLength
        {
            get { return _projekti.Count; }
            
        }
        #endregion


        #region Helpers

    
        private void CreateNewProject()
        {
            //ovde bi trebalo da pozovem parrent application view model i da pozovem funkciju
            //da smeni trenutni view model i prikaze formu za unos podataka o projektu
            //smenjuje ga na pogled za kreiranje projekta
            //prenosim parent application view model za slucaj da zatreba
            this.parentAppVM.ChangeViewModel(new CreateProjectViewModel(this.parentAppVM));
        }
        private void EditProject(object param)
        {
            Projekat projekat = (Projekat)param;
            this.parentAppVM.ChangeViewModel(new EditProjectViewModel(this.parentAppVM, projekat));
        }
        private void RemoveProject(object param)
        {
            Projekat projekat = (Projekat)param;
            if(projekat.Privilegija == "KREATOR")
            {
				string poruka;
				string remove;

				try
				{
					poruka = PPoruka.VratiPrevod("SURE?");
					remove = PPoruka.VratiPrevod("REMOVE");
				}
				catch (Exception er)
				{
					poruka = "Da li ste sigurni?";
					remove = "Remove";
				}

                MessageBoxResult mbRes = System.Windows.MessageBox.Show(poruka, remove, System.Windows.MessageBoxButton.YesNo);
                if(mbRes == MessageBoxResult.Yes)
                {
					try
					{
						PProjekat.ObrisiProjekat(projekat.Id);
						Korisnik.Projekti = PProjekat.VratiSveProjekte(Korisnik.KorisnickoIme);   // osvezava listu projekata korisnika
                        OsveziListuProjekata();
					}
					catch (OracleException oraError)
					{
						try
						{
							System.Windows.MessageBox.Show(PPoruka.VratiPrevod(oraError.Number.ToString()));
						}
						catch (OracleException err)
						{
							System.Windows.MessageBox.Show("Greska pri vracanju svih projekata");
						}
					}
                }               
            }
        }
		#endregion
		
        private void OpenProject(object param)
        {
            Projekat p = (Projekat)param;
            this.parentAppVM.TrenutniProjekat = p;
            // projekat vec ima napravljenu praznu tabelu pa se onda pravi nova
            //koja ce da bude procitana iz baze
            this.parentAppVM.TrenutniProjekat.TabelaProjekta = new Tabela(p.Ime, p.Opis, p.Id);
            Grid gr = TabelaCrtac.InvokeNacrtaj(p.TabelaProjekta, this.parentAppVM);

            this.parentAppVM.AppView.ProjekatDugmiciManage(true, "open");
            this.parentAppVM.AppView.SetContent(gr);

            //bworker test
            RadSaBazom.RefreshTabele.TrProjekat = this.parentAppVM.TrenutniProjekat;
        }

		private void CloseView()
		{
			this.CloseAction();
		}
	}
}
