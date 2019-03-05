using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanbanned.Models
{
    public class KontrolnaTacka
    {
        public int Id { get; set; }
        public bool Vrednost { get; set; }
        public String Naziv { get; set; }

		public KontrolnaTacka()
		{
		}

        public KontrolnaTacka(String naziv)
        {
            Naziv = naziv;
            Vrednost = false;
        }
    }
}
