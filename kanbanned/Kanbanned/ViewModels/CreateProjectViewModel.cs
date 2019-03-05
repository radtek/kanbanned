using Kanbanned.Helpers;
using Kanbanned.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Kanbanned.ViewModels
{
    public class CreateProjectViewModel : ObservableObject, IViewModel
    {
        /// <summary>
        /// Atributi
        /// </summary>
        private string _naziv;
        private DateTime? _datumZavrsetka;
        private string _opis;

        private string _naslov;
        /// <summary>
        /// Komande
        /// </summary>
        /// 
        ICommand _nextCommand;
        ICommand _goBackCommand;

        private ApplicationViewModel parentAppVM;

        public CreateProjectViewModel(IViewModel vm)
        {
            this.parentAppVM = (ApplicationViewModel)vm;
            //Naslov = "Creating new project";
        }

        public DateTime? DatumZavrsetka
        {
            get { return this._datumZavrsetka; }
            set
            {
                this._datumZavrsetka = value;
                OnPropertyChanged("DatumZavrsetka");
            }
        }
        public String Naslov
        {
            get { return this._naslov; }
            set { this._naslov = value; OnPropertyChanged("Naslov"); }
        }
        public string Naziv
        {
            get { return this._naziv; }
            set
            {
                this._naziv = value;
                OnPropertyChanged("Naziv");
            }
        }
        public string Opis
        {
            get { return this._opis; }
            set
            {
                this._opis = value;
                OnPropertyChanged("Opis");
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
        public ICommand NextCommand
        {
            get
            {
                if (_nextCommand == null)
                {
                    _nextCommand = new RelayCommand(
                        param => NextPage(),
                        param => true
                    );
                }
                return _nextCommand;
            }
        }
        private void GoBack()
        {
            //vraca se na StartView
            this.parentAppVM.ChangeViewModel(this.parentAppVM.PageViewModels[0]);
        }
        private void NextPage()
        {
            if(this._naziv != null)
            {
                //tmp objekat
                Projekat noviProjekat = new Projekat(this._naziv, this._opis, this._datumZavrsetka);
                noviProjekat.Privilegija = "KREATOR";
                //u konstruktor stavljamo trenutni objekat jer ce tek kasnije da bude usnimljen u bazu
                //prebacuje se u narednu stranicu za dodavanje radnika
                this.parentAppVM.ChangeViewModel(new DodajRadnikaViewModel(this.parentAppVM, this, noviProjekat));
            }          
        }
    }
}
