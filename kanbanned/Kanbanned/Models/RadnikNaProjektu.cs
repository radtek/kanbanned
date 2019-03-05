using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Kanbanned.Models
{
    public class RadnikNaProjektu
    {
        public String Ime { get; set; }
        public String Uloga { get; set; }

        public RadnikNaProjektu(string ime, string uloga)
        {
            Ime = ime;
            Uloga = uloga;
        }

        public RadnikNaProjektu()
        {

        }
    }
}
