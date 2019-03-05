using Kanbanned.Helpers;
using Kanbanned.Packages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanbanned.Models
{
	// 10.06.2018. mstankovic (ubacen enum za tip zadatka)
	public enum ZTip
	{
		Feature       = 0,
		Documentation = 1,
		Issue	      = 2,
		Bug           = 3,
		Improvement   = 4
	}

	// 10.06.2018. mstankovic (ubacen enum za status zadatka)
	public enum ZStatus
	{
		NotStarted = 0,
		InProgress = 1,
		Finished   = 2,
		Canceled   = 3
	}

    public class Zadatak : ObservableObject
    {
        private int _kompletnost;
        private string _ime;
        private ZStatus _status;

        public int Id { get; set; }
        public String Ime
        {
            get
            {
                return _ime;
            }
            set
            {
                _ime = value;
                OnPropertyChanged("Ime");
            }
        }
        public String Opis { get; set; } 
		public ZTip Tip { get; set; }
		public ZStatus Status
        {
            get { return this._status; }
            set { this._status = value; OnPropertyChanged("Status"); }
        }

        public int Kompletnost
        {
            get
            {
                return this._kompletnost;
            }
            set
            {
                this._kompletnost = value;
                OnPropertyChanged("Kompletnost");
            }
        }
        public DateTime? PocetakIzrade { get; set; }
        public DateTime? KrajIzrade { get; set; }

        //kljuc je ime, vrednost je komentar
        private ObservableCollection<Komentar> _komentari;
        public ObservableCollection<Komentar> Komentari
        {
            get { return this._komentari; }
            set { this._komentari = value; OnPropertyChanged("Komentari"); }
        }
        //string je kljuc, a bool je vrednost koja kaze da li je checkovan ili nije     
        private ObservableCollection<KontrolnaTacka> _kontrolneTacke;
        public ObservableCollection<KontrolnaTacka> KontrolneTacke
        {
            get { return this._kontrolneTacke; }
            set { this._kontrolneTacke = value; OnPropertyChanged("KontrolneTacke"); }
        }


        public int Row { get; set; }
        public int Column { get; set; }
       
        public KontejnerZadataka Roditelj { get; set; }

        public Action KontrolnaTackaChanged { get; set; }
        

        public Zadatak()
        {
            Komentari = new ObservableCollection<Komentar>();
            KontrolneTacke = new ObservableCollection<KontrolnaTacka>();
            Status = ZStatus.NotStarted;

			KontrolnaTackaChanged = IzracunajStatus;
		}

        public Zadatak(KontejnerZadataka roditelj)
        {
            Ime = "Neki zadatak";
            Roditelj = roditelj;
            Status = ZStatus.NotStarted;

            Komentari = new ObservableCollection<Komentar>();    
            KontrolneTacke = new ObservableCollection<KontrolnaTacka>();

            KontrolnaTackaChanged = IzracunajStatus;          
        }

        public void DodajKomentar(String korisnik, String komentar)
        {
            Komentar noviKomentar = new Komentar(korisnik, komentar);

            Komentar test = Komentari.FirstOrDefault(x => x.Tekst == komentar && x.Korisnik == korisnik);

            if(test == null)
            {
				// dodavanje komentara u bazu
				if (this.Id != 0)
				{
					try
					{
						PZadatak.DodajKomentar(komentar, this.Id);
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

                Komentari.Add(noviKomentar);
            }            
        }
        public void DodajKontrolnuTacku(String naziv)
        {
            KontrolnaTacka nova = new KontrolnaTacka(naziv);
            // dodavanje kontrolne tacke u bazu

            KontrolnaTacka test = KontrolneTacke.FirstOrDefault(x => x.Naziv == naziv);

            if(test == null)
            {
				if (this.Id != 0)
				{
					try
					{
						PZadatak.DodajKontrolnuTacku(naziv, this.Id);
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
				
				KontrolneTacke.Add(nova);
            }
        }

        private void IzracunajStatus()
        {
            int brTacaka = this.KontrolneTacke.Count;
            int brStikliranih = 0;
            foreach(KontrolnaTacka kt in this.KontrolneTacke)
            {
                if(kt.Vrednost == true)
                {
                    brStikliranih++;
                    
                }
				//izmena u bazi
				if (kt.Id != 0)
				{
					try
					{
						PZadatak.IzmeniKontrolnuTacku(kt.Id, kt.Vrednost);
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

            if(brTacaka != 0)
            {
                Kompletnost = (brStikliranih * 100) / brTacaka;
            }           
        }

      
    }
}
