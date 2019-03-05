using Kanbanned.Helpers;
using Kanbanned.Models;
using Kanbanned.Packages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Kanbanned.ViewModels
{
    public class EditProjectViewModel : ObservableObject, IViewModel
    {
        /// <summary>
        /// Atributi
        /// prekopiraju se vrednosti iz projekte koji treba da se edituje kako se ne bi odmah snimali podaci
        /// tek na narednoj stranici kada se ide na finish se snimaju podaci u stvarnom objektu
        /// </summary>
        ///
        private string _naziv;
        private DateTime? _datumZavrsetka;
        private string _opis;

        Projekat _projekat;

        private String _naslov;
        /// <summary>
        /// Komande
        /// </summary>
        /// 
        ICommand _nextCommand;
        ICommand _goBackCommand;

        private ApplicationViewModel parentAppVM;

        public EditProjectViewModel(IViewModel vm, Projekat p)
        {
            this.parentAppVM = (ApplicationViewModel)vm;
            this._projekat = p;
            this._naziv = p.Ime;
            this._opis = p.Opis;
            this._datumZavrsetka = p.DatumZavrsetka;
            //Naslov = "Editing an existing project";
        }
        public String Naslov
        {
            get { return this._naslov; }
            set { this._naslov = value; OnPropertyChanged("Naslov"); }
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
            if (this._naziv != null)
            {                  
                this.parentAppVM.ChangeViewModel(new EditDodajRadnikaViewModel(this.parentAppVM, this, _projekat));           
            }
        }
    }
}
