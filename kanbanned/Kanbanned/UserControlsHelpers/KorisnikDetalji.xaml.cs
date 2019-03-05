using Kanbanned.Helpers;
using Kanbanned.Models;
using Kanbanned.Packages;
using MahApps.Metro.Controls;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Kanbanned.UserControlsHelpers
{
    /// <summary>
    /// Interaction logic for KorisnikDetalji.xaml
    /// </summary>
    public partial class KorisnikDetalji : MetroWindow, INotifyPropertyChanged
    {
        private string _kime;
        private string _ime;
        private string _prezime;
        private string _kompanija;
        private string _sifra;
        private string _novasifra;
        
        public string KorisnickoIme
        {
            get { return _kime; }
            set { _kime = value; OnPropertyChanged("KorisnickoIme"); }
        }
        public string Ime
        {
            get { return _ime; }
            set { _ime = value; OnPropertyChanged("Ime"); }
        }
        public string Prezime
        {
            get { return _prezime; }
            set { _prezime = value; OnPropertyChanged("Prezime"); }
        }
        public string Kompanija
        {
            get { return _kompanija; }
            set { _kompanija = value; OnPropertyChanged("Kompanija"); }
        }
        public PasswordBox Sifra { get; set; }
        public PasswordBox NovaSifra { get; set; }

        // true za detalje, false za sifru
        public bool DetaljiIliSifra { get; set; }
        public KorisnikDetalji()
        {
            InitializeComponent();
            KorisnickoIme = Korisnik.KorisnickoIme;
            Ime = Korisnik.Ime;
            Prezime = Korisnik.Prezime;
            Kompanija = Korisnik.Kompanija;

            Sifra = (PasswordBox)this.FindName("txtPassword");
            NovaSifra = (PasswordBox)this.FindName("txtConfirmPassword");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            this.VerifyPropertyName(propertyName);

            if (this.PropertyChanged != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                this.PropertyChanged(this, e);
            }
        }

        #region Debugging Aides

        /// <summary>
        /// Warns the developer if this object does not have
        /// a public property with the specified name. This
        /// method does not exist in a Release build.
        /// </summary>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public virtual void VerifyPropertyName(string propertyName)
        {
            // Verify that the property name matches a real,
            // public, instance property on this object.
            if (TypeDescriptor.GetProperties(this)[propertyName] == null)
            {
                string msg = "Invalid property name: " + propertyName;

                if (this.ThrowOnInvalidPropertyName)
                    throw new Exception(msg);
                else
                    Debug.Fail(msg);
            }
        }

        /// <summary>
        /// Returns whether an exception is thrown, or if a Debug.Fail() is used
        /// when an invalid property name is passed to the VerifyPropertyName method.
        /// The default value is false, but subclasses used by unit tests might
        /// override this property's getter to return true.
        /// </summary>
        protected virtual bool ThrowOnInvalidPropertyName { get; private set; }

        #endregion // Debugging Aides

        // izmena detalja o korisniku
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if((Ime != null && !Ime.Equals(Korisnik.Ime)) || (Prezime != null && !Prezime.Equals(Korisnik.Prezime)) || (Kompanija != null && !Kompanija.Equals(Korisnik.Kompanija)))
            {
                // ako je bilo promene u detaljima onda treba novi da se usnime
                DetaljiIliSifra = true;
                this.DialogResult = true;
            }
        }

        // izmena sifre
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // treba prvo da se ispita da li je dobra trenutna lozinka
            // u slucaju da jeste i nova lozinka se razlikuje od nje
            //MessageBox.Show("stara sifra:" + Sifra.Password);
            //MessageBox.Show("stara sifra:" + NovaSifra.Password);
            try
            {
                PKorisnik.Login(Korisnik.KorisnickoIme, Sifra.Password);
				
                if (!NovaSifra.Password.Equals(Sifra.Password))
                {
					DetaljiIliSifra = false;
                    this.DialogResult = true;
                }
                else
                {
					try
					{
						System.Windows.MessageBox.Show(PPoruka.VratiPrevod("PW_SAME"));
					}
					catch (OracleException err)
					{
						System.Windows.MessageBox.Show("Greska pri promeni sifre");
					}

					this.DialogResult = false;
                }
            }
            catch (OracleException oraError)
            {
				try
				{
					System.Windows.MessageBox.Show(PPoruka.VratiPrevod(oraError.Number.ToString()));
				}
				catch (OracleException er)
				{
					System.Windows.MessageBox.Show("Greska");
				}
				this.DialogResult = false;
            }
            catch (System.Exception error)
            {
				try
				{
					System.Windows.MessageBox.Show(PPoruka.VratiPrevod(error.Message));
				}
				catch (OracleException er)
				{
					System.Windows.MessageBox.Show("Greska");
				}
				this.DialogResult = false;
            }        
        }       
    }
}
