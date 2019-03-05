using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanbanned.Models
{
    public class Komentar
    {
        private int _id;
        private string _korisnik;
        private string _tekst;
		private int _zadatak;

        public int Id
        {
            get { return this._id; }
            set { this._id = value; }
        }

        public String Korisnik
        {
            get { return this._korisnik; }
            set { this._korisnik = value; }
        }

        public String Tekst
        {
            get { return this._tekst; }
            set { this._tekst = value; }
        }

		public int Zadatak
		{
			get { return _zadatak; }
			set { _zadatak = value; }
		}

		public Komentar()
		{
		}

        public Komentar(String ime, String vrednost)
        {
            Korisnik = ime;
            Tekst = vrednost;
        }
    }
}
