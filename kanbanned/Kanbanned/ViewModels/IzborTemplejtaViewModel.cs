using Kanbanned.Helpers;
using Kanbanned.Models;
using Kanbanned.Packages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace Kanbanned.ViewModels
{
    public class IzborTemplejtaViewModel : ObservableObject, IViewModel
    {
        ApplicationViewModel parentAppVM;
        IViewModel previousVM;

        private Projekat _projekat;

        private Projekat _templejt;

        //treba da se vrati lista templejta koje korisnik ima
        public ListaProjekata Templejti { get; set; }

        public IzborTemplejtaViewModel(IViewModel vm, IViewModel prev, Projekat p)
        {
            this.parentAppVM = (ApplicationViewModel)vm;
            this.previousVM = prev;
            _projekat = p;

			// vrati sve templejte
			try
			{
				Templejti = PProjekat.VratiTemplejte();
			}
			catch (Oracle.ManagedDataAccess.Client.OracleException oraError)
			{
				try
				{
					System.Windows.MessageBox.Show(PPoruka.VratiPrevod(oraError.Number.ToString()));
				}
				catch (Oracle.ManagedDataAccess.Client.OracleException err)
				{
					System.Windows.MessageBox.Show("Greska pri vracanju templejta");
				}
			}
        }

     
        ICommand _finishCommand;
        //ICommand _goBackCommand;
        ICommand _addCommand;

        //public ICommand GoBackCommand
        //{
        //    get
        //    {
        //        if (_goBackCommand == null)
        //        {
        //            _goBackCommand = new RelayCommand(
        //                param => GoBack(),
        //                param => true
        //            );
        //        }
        //        return _goBackCommand;
        //    }
        //}
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
        public ICommand ChooseCommand
        {
            get
            {
                if (_addCommand == null)
                {
                    _addCommand = new RelayCommand(
                        param => Choose(param),
                        param => true
                    );
                }
                return _addCommand;
            }
        }
     
        //private void GoBack()
        //{
        //    this.parentAppVM.ChangeViewModel(this.previousVM);
        //}
        private void Choose(object t)
        {
            Projekat templejt = t as Projekat;

            //kopira se tabela iz templejta i dodeljuje se projektu koji se kreira
            templejt.TabelaProjekta = new Tabela(_projekat.Ime, _projekat.Opis, templejt.Id);
            templejt.TabelaProjekta.PostaviKoloneNaNula();
            _projekat.TabelaProjekta = templejt.TabelaProjekta;

            Finish();
        }

        private void Finish()
        {
            //crta se pocetna tabela
            this.parentAppVM.TrenutniProjekat = this._projekat;
            Grid gr = TabelaCrtac.InvokeNacrtaj(_projekat.TabelaProjekta, this.parentAppVM);
            this.parentAppVM.AppView.ProjekatDugmiciManage(true, "create");
            this.parentAppVM.AppView.SetContent(gr);
        }
    }
}
