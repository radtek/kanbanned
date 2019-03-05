using Kanbanned.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Kanbanned.Models;
using Kanbanned.Views;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows;
using MahApps.Metro.Controls;
using Kanbanned.Packages;

namespace Kanbanned.ViewModels
{
    /// <summary>
    /// ViewModel aplikacije koji ce sadrzati listu svih ViewModela i menjati ih u zavisnosti
    /// od trenutne aktivne stranice
    /// </summary>
    public class ApplicationViewModel : ObservableObject, IViewModel
    {
        #region Atributi
        
        //trenutni projekat ce da se smenjuje kad se otvori neki od ponudjenih projekata
        //ili se napravi novi projekat
        private Projekat _trenutniProjekat;

        //lista ViewModela stranica koje ce se prikazivati prilikom rada aplikacije
        private List<IViewModel> _pageViewModels;
        private IViewModel _currentViewModel;
        
        private ICommand _changeViewCommand;

        //pokazivac na ApplicationView da bih izvukao ContentControl i predstavio tabelu
        private ApplicationView _appView;
        #endregion

        public RadSaBazom ProveraBaze { get; set; }
        public MainViewModel mainViewModel { get; set; }
        public ApplicationViewModel(MainViewModel vm)
        {
            mainViewModel = vm;
            //Dodavanje pogleda koji su na raspolaganju tokom rada aplikacije

            PageViewModels.Add(new StartViewModel(this));
           
            //Pocetni pogled
            CurrentPageViewModel = PageViewModels[0];
        }

        #region Propertiji / Komande

        public Projekat TrenutniProjekat
        {
            get { return this._trenutniProjekat; }
            set { this._trenutniProjekat = value; }
        }
        public String ImePrezimeKorisnika
        {
            get
            {
                return Korisnik.Ime + " " + Korisnik.Prezime;
            }
        }

        public ICommand ChangeViewCommand
        {
            get
            {
                if (_changeViewCommand == null)
                {
                    _changeViewCommand = new RelayCommand(
                        p => ChangeViewModel((IViewModel)p),
                        p => p is IViewModel);
                }

                return _changeViewCommand;
            }
        }

        public List<IViewModel> PageViewModels
        {
            get
            {
                if (_pageViewModels == null)
                    _pageViewModels = new List<IViewModel>();

                return _pageViewModels;
            }
        }

        public IViewModel CurrentPageViewModel
        {
            get
            {
                return _currentViewModel;
            }
            set
            {
                if (_currentViewModel != value)
                {
                    _currentViewModel = value;
                    OnPropertyChanged("CurrentPageViewModel");
                }
            }
        }

        public ApplicationView AppView
        {
            get { return this._appView; }
            set { this._appView = value;  }
        }
        #endregion

        #region Metode

        /// <summary>
        /// Logika za smenjivanje stranica
        /// verovatno bi trebalo da se izmeni nacin smenjivanja
        /// </summary>
        /// <param name="viewModel"></param>
        public void ChangeViewModel(IViewModel viewModel)
        {
            if (!PageViewModels.Contains(viewModel))
                PageViewModels.Add(viewModel);

            CurrentPageViewModel = PageViewModels
                .FirstOrDefault(vm => vm == viewModel);
        }
		
        public void PostaviTrenutniProjekat()
        {
            if(this._trenutniProjekat != null)
            {
                Grid gr = TabelaCrtac.InvokeNacrtaj(this._trenutniProjekat.TabelaProjekta, this);
                this.AppView.SetContent(gr);
            }           
        }

        public void PostaviCurrentViewModel()
        {
            Binding binding = new Binding();
            binding.Source = this;
            binding.Path = new PropertyPath("CurrentPageViewModel");
            ContentControl kontent = ((ContentControl)this.AppView).FindChild<ContentControl>("contentCurrentVM");
            BindingOperations.SetBinding(kontent, ContentControl.ContentProperty, binding);
        }
        #endregion
    }

}
