using Kanbanned.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanbanned.Models
{
	public class Korisnik : ObservableObject
	{
        #region Properties

		public static string KorisnickoIme { get; set; }

		public static string Ime { get; set; }

		public static string Prezime { get; set; }

		public static string Kompanija { get; set; }

		public static Dictionary<Projekat, string> Projekti { get; set; }

		public static List<string> Mejlovi { get; set; }

		#endregion Properties
	}
}
