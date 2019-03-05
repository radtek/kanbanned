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
using System.Windows.Controls;
using System.Windows.Input;

namespace Kanbanned.ViewModels
{
    public class DodajRadnikaViewModel : ObservableObject, IViewModel
    {
        ApplicationViewModel parentAppVM;
        IViewModel previousVM;
        private Projekat _projekat;

        private String _ime;

        

        public DodajRadnikaViewModel(IViewModel vm, IViewModel prev, Projekat p)
        {
            this.parentAppVM = (ApplicationViewModel)vm;
            this.previousVM = prev;
            _projekat = p;
        }

        //koristi se za editovanje projekta pa mu se prosledjuju trenutni parametri koji ce da se koristi pri izmeni
        //public DodajRadnikaViewModel(IViewModel vm, IViewModel prev)
        //{
        //    this.parentAppVM = (ApplicationViewModel)vm;
        //    this.previousVM = prev;
        //}

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
                    //i ako da onda se dodaje u trenutnu listu
                    bool nadjen = PKorisnik.Postoji(this._ime);
                    if (nadjen)
                    {
                        //onda se doda u listu i kaze da je dodat
                        RadnikNaProjektu postoji = Radnici.FirstOrDefault(x => x.Ime == this._ime);
                        if (postoji == null)
                        {
                            RadnikNaProjektu r = new RadnikNaProjektu() { Ime = this._ime, Uloga = "RADNIK" };
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
									System.Windows.MessageBox.Show("Korisnik je vec dodat");
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
								System.Windows.MessageBox.Show("Korisnik ne postoji");
							}
						}
					}
                }
            }
        }
        private void Remove(object radnik)
        {
            RadnikNaProjektu r = radnik as RadnikNaProjektu;
            Radnici.Remove(r);
        }
        private void Finish()
        {
            //ovde se prelazi na tabelu
            //crta se trenutna tabela

            //umesto ovoga da se otvori pogled za izbor templejta
            //this.parentAppVM.TrenutniProjekat = this._projekat;
            //Grid gr = TabelaCrtac.InvokeNacrtaj(_projekat.TabelaProjekta, this.parentAppVM);
            //this.parentAppVM.AppView.ProjekatDugmiciManage(true, "create");
            //this.parentAppVM.AppView.SetContent(gr);    

            //otvaranje pogleda za izbor templejta
            this.parentAppVM.ChangeViewModel(new IzborTemplejtaViewModel(this.parentAppVM, this, this._projekat));
        }
    }
}

