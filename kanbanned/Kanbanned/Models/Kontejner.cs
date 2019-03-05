using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;

namespace Kanbanned.Models
{
    public abstract class Kontejner
    {
		// 10.06.2018. mstankovic (ubacen property za Id)
		public int Id { get; set; }

		public string Ime { get; set; }
        public string Opis { get; set; }
        public DateTime? PocetakIzrade { get; set; }
        public DateTime? KrajIzrade { get; set; }

		// 10.06.2018. mstankovic (ubacen property za poziciju u okviru roditelja)
		public int Pozicija { get; set; }

		// 10.06.2018. mstankovic (property izbacen jer se nigde ne koristi)
		//public Tabela TabelaRoditelj { get; set; }

		// 10.06.2018. mstankovic
		//public Kontejner Roditelj { get; set; }
		public KontejnerFaza Roditelj { get; set; }

		// 10.06.2018. mstankovic (propery prebacen u klasu KontejnerFaza)
		//public bool IsVerticalSplit { get; set; }

		// 10.06.2018. mstankovic (2 propertija izbacena jer se nigde ne koriste)
		//public string Putanja { get; set; }
		//public bool IsLeaf { get; set; }

		// 10.06.2018. mstankovic (2 propertija izbacena jer se nigde ne koriste)
		//public GridLength Width { get; set; }
		//public GridLength Height { get; set; }

		//max koliko mogu da stanu
		// 10.06.2018. mstankovic (2 propertija prebacena u klasu KontejnerZadataka)
		//public int SirinaPoZadacima { get; set; }
        //public int VisinaPoZadacima { get; set; }
    }
}
