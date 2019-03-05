using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanbanned.Models
{
    public class KontejnerZadataka : Kontejner
    {
		// 10.06.2018. mstankovic (2 propertija prebacena iz klase Kontejner)
		public int SirinaPoZadacima { get; set; }
		public int VisinaPoZadacima { get; set; }

		// 10.06.2018. mstankovic (ubacen property za max broj zadataka)
		public int MaxBrZadataka { get; set; }

		//public List<Zadatak> Zadaci { get; set; }
		public List<Zadatak> Zadaci;

		// 10.06.2018. mstankovic (ubacen prazan konstruktor)
		public KontejnerZadataka()
		{
			Zadaci = new List<Zadatak>();
		}

		// 10.06.2018. mstankovic
		//public KontejnerZadataka(string ime, Kontejner roditelj)
		public KontejnerZadataka(string ime, KontejnerFaza roditelj)
		{
			Ime = ime;
            Zadaci = new List<Zadatak>();
            Roditelj = roditelj;

            //za pocetak neka bude ovako
            //da max moze da stane 30 zadataka po koloni
            if(roditelj.IsVerticalSplit)
            {
                SirinaPoZadacima = 3;
                VisinaPoZadacima = 10;
            }
            else
            {
                SirinaPoZadacima = 6;
                VisinaPoZadacima = 5;
            }
            MaxBrZadataka = 30;
        }
        public KontejnerZadataka(KontejnerZadataka kz)
        {
            Ime = "New subcolumn 1";
            Zadaci = kz.Zadaci;
            Roditelj = kz.Roditelj;
            if (Roditelj.IsVerticalSplit)
            {
                SirinaPoZadacima = 3;
                VisinaPoZadacima = 10;
            }
            else
            {
                SirinaPoZadacima = 6;
                VisinaPoZadacima = 5;
            }
            //ograniceno na 30
            MaxBrZadataka = kz.MaxBrZadataka;
            PreurediZadatke();
        }
        //podesava ponovo indexe zadacima
        //jer se oni pomere za jedno mesto unazad kada se izbrise zadatak
        public void PreurediZadatke()
        {
            foreach(Zadatak z in this.Zadaci)
            {
                int index = this.Zadaci.IndexOf(z);
                z.Row = index / this.SirinaPoZadacima;
                z.Column = index % this.SirinaPoZadacima;
            }
        }
        
    }
}
