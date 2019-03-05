using Kanbanned.Helpers;
using Kanbanned.Models;
using Kanbanned.Packages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Kanbanned.ViewModels
{
    /// <summary>
    /// Isti kao i DodajRadnikaViewModel ali ima neke sitne razlike
    /// Ovde kada se brise ili dodaje radnik odmah vrsi izmenu u bazi
    /// A kada se ide na OK onda se vrsi izmena sa prethodne strane (osnovni podaci o projektu)
    /// </summary>
    public class EditDodajRadnikaViewModel : ObservableObject, IViewModel
    {
        ApplicationViewModel parentAppVM;
        IViewModel previousVM;
        private Projekat _projekat;

        private String _ime;

        //pamte se trenutni dodati i izbrisani koji ce posle stvarno da se dodaju ili izbrisu nakon query-ja 
        List<RadnikNaProjektu> tmpDodati { get; set; }
        List<RadnikNaProjektu> tmpIzbrisani { get; set; }

        public EditDodajRadnikaViewModel(IViewModel vm, IViewModel prev, Projekat p)
        {
            this.parentAppVM = (ApplicationViewModel)vm;
            this.previousVM = prev;
            _projekat = p;

            tmpDodati = new List<RadnikNaProjektu>();
            tmpIzbrisani = new List<RadnikNaProjektu>();
        }

      
        public String Ime
        {
            get { return _ime; }
            set { _ime = value; OnPropertyChanged("Ime"); }
        }

        public ObservableCollection<RadnikNaProjektu> Radnici
        {
            get { return _projekat.RadniciNaProjektu; }
            set { this._projekat.RadniciNaProjektu = value; }
        }

        ICommand _finishCommand;
        ICommand _goBackCommand;
        ICommand _addCommand;
        ICommand _removeCommand;

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
        public ICommand NextCommand
        {
            get
            {
                if (_finishCommand == null)
                {
                    _finishCommand = new RelayCommand(
                        param => Finish(),
                        param => true
                    );
                }
                return _finishCommand;
            }
        }
        public ICommand AddCommand
        {
            get
            {
                if (_addCommand == null)
                {
                    _addCommand = new RelayCommand(
                        param => Add(),
                        param => true
                    );
                }
                return _addCommand;
            }
        }
        public ICommand RemoveCommand
        {
            get
            {
                if (_removeCommand == null)
                {
                    _removeCommand = new RelayCommand(
                        param => Remove(param),
                        param => true
                    );
                }
                return _removeCommand;
            }
        }
        private void GoBack()
        {
            this.parentAppVM.ChangeViewModel(this.previousVM);
        }
        private void Add()
        {
            if (this._ime != null)
            {
                if (!this._ime.Equals(""))
                {
                    //ovde se proverava da li postoji taj korisnik
                    //i ako da onda se dodaje u listu privremeno dodatih radnika
                    bool nadjen = PKorisnik.Postoji(this._ime);
                    if (nadjen)
                    {
                        RadnikNaProjektu postoji = Radnici.FirstOrDefault(x => x.Ime == this._ime);
                        if(postoji == null)
                        {
                            //onda se doda u listu i kaze da je dodat
                            RadnikNaProjektu r = new RadnikNaProjektu() { Ime = this._ime, Uloga = "RADNIK" };

                            tmpDodati.Add(r);
                            Radnici.Add(r);
                        }
                        else
                        {
							try
							{
								MessageBox.Show(Packages.PPoruka.VratiPrevod("USR_ADDED"));
							}
							catch (Oracle.ManagedDataAccess.Client.OracleException oraError)
							{
								try
								{
									System.Windows.MessageBox.Show(PPoruka.VratiPrevod(oraError.Number.ToString()));
								}
								catch (Oracle.ManagedDataAccess.Client.OracleException err)
								{
									System.Windows.MessageBox.Show("Greska");
								}
							}
						}
                    }
                    else
                    {
						//ne postoji
						try
						{
							System.Windows.MessageBox.Show(PPoruka.VratiPrevod("USR_NEXIST"));
						}
						catch (Oracle.ManagedDataAccess.Client.OracleException oraError)
						{
							try
							{
								System.Windows.MessageBox.Show(PPoruka.VratiPrevod(oraError.Number.ToString()));
							}
							catch (Oracle.ManagedDataAccess.Client.OracleException err)
							{
								System.Windows.MessageBox.Show("Greska");
							}
						}
					}
                }
            }
        }
        private void Remove(object radnik)
        {
            //stavlja se u listu privremeno izbrisanih 
            RadnikNaProjektu r = radnik as RadnikNaProjektu;
            tmpIzbrisani.Add(r);
            Radnici.Remove(r);
        }
        private void Finish()
        {
            //vrati se na pocetnu stranicu
            RadSaBazom.IzmenaProjekta(_projekat.Id, ((EditProjectViewModel)previousVM).Naziv, ((EditProjectViewModel)previousVM).Opis, tmpDodati, tmpIzbrisani);
            //osvezava se lista projekata i prebacuje se na pocetan pogled
            ((StartViewModel)(((ApplicationViewModel)parentAppVM).PageViewModels[0])).OsveziListuProjekata();
            ((ApplicationViewModel)parentAppVM).CurrentPageViewModel = ((ApplicationViewModel)parentAppVM).PageViewModels[0];
        }
    }
}

