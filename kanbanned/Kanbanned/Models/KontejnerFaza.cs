using Kanbanned.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanbanned.Models
{
	public class KontejnerFaza : Kontejner
	{
		// 10.06.2018. mstankovic (property prebacen iz klase Kontejner)
		public bool IsVerticalSplit { get; set; }

		public List<Kontejner> Deca;

		// 10.06.2018. mstankovic (ubacen prazan konstruktor)
		public KontejnerFaza()
		{
			Deca = new List<Kontejner>();
		}

		public KontejnerFaza(string ime)
		{
			Ime = ime;
			Deca = new List<Kontejner>();
		}
	}
}
