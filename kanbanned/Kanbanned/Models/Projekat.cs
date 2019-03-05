using Kanbanned.Helpers;
using Kanbanned.Packages;
using Kanbanned.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanbanned.Models
{
	public class Projekat
	{
		int _id;
		string _ime;
		string _opis;
		DateTime _datKreiranja;
		DateTime? _datZavrsetka;
        private String privilegija;

        private Tabela _tabela;

        //lista radnika koji rade na projektu
        public ObservableCollection<RadnikNaProjektu> RadniciNaProjektu { get; set; }

        //istorija izmena
        public List<String> IstorijaIzmenaFaza { get; set; }
        public List<String> IstorijaIzmenaZadataka { get; set; }

		public Projekat()
		{
		}

		public Projekat(int id, string ime, string opis, DateTime datKreiranja, DateTime? datZavrsetka)
		{
			_id = id;
			_ime = ime;
			_opis = opis;
			_datKreiranja = datKreiranja;
			_datZavrsetka = datZavrsetka;

            //ovde se pravi nova tabela vezana za ovaj projekat
            //konstruktor tabele treba da se izmeni da pravi na pocetku dve kolone samo
            _tabela = new Tabela(Ime);
            RadniciNaProjektu = new ObservableCollection<RadnikNaProjektu>();
		}

        public Projekat(string ime, string opis, DateTime? datZavrsetka)
        {
            _ime = ime;
            _opis = opis;
            _datZavrsetka = datZavrsetka;

            _tabela = new Tabela(_ime);

            RadniciNaProjektu = new ObservableCollection<RadnikNaProjektu>();
        }
        public Tabela TabelaProjekta
        {
            get { return this._tabela; }
            set { this._tabela = value; }
        }
        [DisplayName("Naziv")]
        public String Ime
        {
            get { return this._ime; }
            set { this._ime = value; }
        }
        public String Opis
        {
            get { return this._opis; }
            set { this._opis = value; }
        }

        [DisplayName("Datum kreiranja")]
        public DateTime DatumKreiranja
        {
            get { return this._datKreiranja; }
            set { this._datKreiranja = value; }
        }

        [DisplayName("Datum zavrsetka")]
        public DateTime? DatumZavrsetka
        {
            get { return this._datZavrsetka; }
            set { this._datZavrsetka = value; }
        }
        public String Privilegija
        {
            get { return privilegija; }
            set { privilegija = value; }
        }
        public int Id
        {
            get { return this._id; }
			set { _id = value; }
        }

        // ucitavanje izmena iz baze i vrati trazene izmene
        public List<String> UcitajIstorijuIzmena(PPromena.TipIstorije tip, int brPromena)
        {
            if(tip == PPromena.TipIstorije.Faze)
            {
                IstorijaIzmenaFaza = PPromena.VratiIstoriju(PPromena.TipIstorije.Faze, brPromena, this.Id);
                return IstorijaIzmenaFaza;
            }
            else
            {
                IstorijaIzmenaZadataka = PPromena.VratiIstoriju(PPromena.TipIstorije.Zadaci, brPromena, this.Id);
                return IstorijaIzmenaZadataka;
            }
        }

        // ucitavanje obavestenja iz baze
        //public List<String> UcitajObavestenja()
        //{
        //    IstorijaIzmenaZadataka = PPromena.VratiIstoriju(PPromena.TipIstorije.Zadaci, brPromena, this.Id);
        //    return IstorijaIzmenaZadataka;
        //}
    }
}
