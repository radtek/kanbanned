using Kanbanned.Packages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanbanned.Models
{
    public class Tabela
    {
        public string Naslov { get; set; }
        public string Opis { get; set; }

		// 10.06.2018. mstankovic
		//public Kontejner RootKolona { get; set; }
		public KontejnerFaza RootKolona { get; set; }

		public Tabela(String naslov)
        {
            Naslov = naslov;
            RootKolona = new KontejnerFaza("ROOT");
            RootKolona.IsVerticalSplit = true;

            //vvranic - odavde premestena funkcija za ulancavanje u donji konstruktor
        }

        //kada se otvara postojeci projekat iz baze onda se zove ovaj konstruktor koji povezuje stablo
        public Tabela(String naslov, String opis, int idProjekta)
        {
            Naslov = naslov;
            Opis = opis;
            RootKolona = new KontejnerFaza("ROOT");
            RootKolona.IsVerticalSplit = true;

			// 10.06.2018. mstankovic
			// (BEGIN) ULANCAVANJE U STABLO
			try
			{
				List<Kontejner> faze = Packages.PFaza.VratiFaze(idProjekta); // 7 je ID projekta
				Queue<Kontejner> queue = new Queue<Kontejner>();

				// Prvo se pronadju elementi ciji je roditelj 
				// ROOT kolona (tj. oni ciji je Roditelj.Id = 0),
				// vezu se za nju i ubace u queue
				foreach (Kontejner k in faze)
					if (k.Roditelj.Id == 0)
					{
						RootKolona.Deca.Add(k);
						k.Roditelj = RootKolona;

						// BEGIN PROMENA
						//za pocetak neka bude ovako
						//da max moze da stane 30 zadataka po koloni
						if (k.GetType() == typeof(KontejnerZadataka))
						{
							((KontejnerZadataka)k).SirinaPoZadacima = 3;
							((KontejnerZadataka)k).VisinaPoZadacima = 10;
						}
						// END PROMENA

						queue.Enqueue(k);
					}


				//((KontejnerFaza)RootKolona).Deca = ((KontejnerFaza)RootKolona).Deca.OrderBy(kontejner => kontejner.Pozicija).ToList();
				RootKolona.Deca.Sort((x, y) => x.Pozicija.CompareTo(y.Pozicija));

				// Ostali elementi se ulancavaju po ugledu na algoritam obilaska stabla po sirini (Breadth First)
				while (queue.Count > 0)
				{
					Kontejner k = queue.Dequeue();

					// Za svaki kontejner faza je potrebno da se pronadju njegova deca
					// u ostatku liste, da se ulancaju i ubace u red
					if (k.GetType() == typeof(KontejnerFaza))
					{
						foreach (Kontejner dete in faze)
							if (dete.Roditelj.Id == k.Id)
							{
								((KontejnerFaza)k).Deca.Add(dete);
								dete.Roditelj = (KontejnerFaza)k;

								// BEGIN PROMENA
								//za pocetak neka bude ovako
								//da max moze da stane 30 zadataka po koloni
								if (dete.GetType() == typeof(KontejnerZadataka))
								{
									if (dete.Roditelj.IsVerticalSplit)
									{
										((KontejnerZadataka)dete).SirinaPoZadacima = 3;
										((KontejnerZadataka)dete).VisinaPoZadacima = 10;
									}
									else
									{
										((KontejnerZadataka)dete).SirinaPoZadacima = 6;
										((KontejnerZadataka)dete).VisinaPoZadacima = 5;
									}
									//ograniceno na 30 za pocetak
									((KontejnerZadataka)dete).MaxBrZadataka = 30;
								}
								// END PROMENA

								queue.Enqueue(dete);
							}

					   // 12.06.2018 vvranic
					   //sortiranje kolone po indeksima da bi sve kolone bile prikazane u pravilnom redosledu
					   //u rastucem redosledu ih sortira sto bi trebalo da bude ok 
					   //((KontejnerFaza)k).Deca = ((KontejnerFaza)k).Deca.OrderBy(kontejner => kontejner.Pozicija).ToList();
					   ((KontejnerFaza)k).Deca.Sort((x, y) => x.Pozicija.CompareTo(y.Pozicija));
					}
					else if (k.GetType() == typeof(KontejnerZadataka))
					{
						try
						{
							((KontejnerZadataka)k).Zadaci = Packages.PZadatak.VratiZadatkeFaze(k.Id);
							//((KontejnerZadataka)k).Zadaci.ForEach(zadatak => zadatak.Roditelj = (KontejnerZadataka)k);
							((KontejnerZadataka)k).Zadaci.ForEach(zadatak =>
																 {
																	 zadatak.Roditelj = (KontejnerZadataka)k;
																	 zadatak.Komentari = PZadatak.VratiSveKomentare(zadatak.Id);
																	 zadatak.KontrolneTacke = PZadatak.VratiSveKontrolneTacke(zadatak.Id);
																	 zadatak.KontrolnaTackaChanged();
																 });

							((KontejnerZadataka)k).PreurediZadatke();
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
				// (END) ULANCAVANJE U STABLO
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

        //0 za desno, 1 za levo
        //vvrani - dodate ispravka za pozicije nakon smenjivanja pozicija
        public void Pomeri(Kontejner kolona, bool pravac)
        {
			// 10.06.2016. mstankovic
			//KontejnerFaza kf = ((KontejnerFaza)kolona.Roditelj);
			KontejnerFaza kf = kolona.Roditelj;

			int index = kf.Deca.IndexOf(kolona);
            //desno
            if (pravac == false)
            {
                if(index != kf.Deca.Count - 1)
                {
                    Kontejner tmp = kf.Deca[index];
                    int tmpPoz = kf.Deca[index + 1].Pozicija;
                    kf.Deca[index] = kf.Deca[index + 1];
                    //kf.Deca[index].Pozicija -= 1;
                    kf.Deca[index].Pozicija = tmp.Pozicija;
                    kf.Deca[index + 1] = tmp;
                    //kf.Deca[index + 1].Pozicija += 1;
                    kf.Deca[index + 1].Pozicija = tmpPoz;

					if (kolona.Id != 0)
					{
						try
						{
							PFaza.Pomeri(kolona.Id, 1);
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
            }
            //levo
            else
            {
                if(index != 0)
                {
                    Kontejner tmp = kf.Deca[index];
                    int tmpPoz = kf.Deca[index - 1].Pozicija;
                    kf.Deca[index] = kf.Deca[index - 1];
                    //kf.Deca[index].Pozicija += 1;
                    kf.Deca[index].Pozicija = tmp.Pozicija;
                    kf.Deca[index - 1] = tmp;
                    //kf.Deca[index - 1].Pozicija -= 1;
                    kf.Deca[index - 1].Pozicija = tmpPoz;

					if (kolona.Id != 0)
					{
						try
						{
							PFaza.Pomeri(kolona.Id, -1);
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
            }
        }

		// 10.06.2018. mstankovic
		//public void DodajKolonu(Kontejner roditelj, String naziv)
		public void DodajKolonu(KontejnerFaza roditelj, String naziv, int idProjekta)
		{
            KontejnerZadataka nova = new KontejnerZadataka(naziv, roditelj);

			// kolona se ubacuje u bazu samo ako je projekat vec kreiran
			if (idProjekta != 0)
			{
				try
				{
					nova.Id = PFaza.Dodaj_FZ(nova.Ime, nova.Opis, nova.MaxBrZadataka, nova.PocetakIzrade, nova.KrajIzrade,
											 roditelj.Id, nova.SirinaPoZadacima, nova.VisinaPoZadacima, idProjekta);
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
            
            // 10.06.2018. mstankovic
            //((KontejnerFaza)roditelj).Deca.Add(nova);
            roditelj.Deca.Add(nova);

            //dodata ispravka pozicije 
            nova.Pozicija = roditelj.Deca.IndexOf(nova);
        }

        //brisanje kontejnera zadataka
        //17.06.2018
        public void IzbrisiFazu(Kontejner kz)
        {
            KontejnerFaza roditelj = kz.Roditelj;
            
			//brisanje je dozvoljeno samo ako je kontejner prazan
            if ((kz.GetType() == typeof(KontejnerZadataka) && ((KontejnerZadataka)kz).Zadaci.Count == 0) ||
				(kz.GetType() == typeof(KontejnerFaza) && ((KontejnerFaza)kz).Deca.Count == 0))
            {
                    //brisanje iz baze
                if (kz.Id != 0)
				{
					try
					{
						PFaza.Obrisi(kz.Id);
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

				foreach (Kontejner k in kz.Roditelj.Deca)
					if (k.Pozicija > kz.Pozicija)
						k.Pozicija--;

				roditelj.Deca.Remove(kz);
                //roditelj.Deca.OrderBy(x => x.Pozicija);
            }
        }

		// 10.06.2018. mstankovic
		//public void DodajZadatak(Kontejner roditelj)
		public void DodajZadatak(KontejnerZadataka roditelj)
		{
            // 10.06.2018. mstankovic (naredne 3 linije)
            //Zadatak novi = new Zadatak((KontejnerZadataka)roditelj);
            //((KontejnerZadataka)roditelj).Zadaci.Add(novi);
            //((KontejnerZadataka)roditelj).PreurediZadatke();
            if (roditelj.Zadaci.Count != roditelj.MaxBrZadataka)
            {
                Zadatak novi = new Zadatak(roditelj);

				// dodavanje zadatka u bazu se izvrsava samo ako je projekat kreiran (sto znaci da je roditeljskoj fazi dodeljen id)
				if (roditelj.Id != 0)
				{
					try
					{
						novi.Id = PZadatak.Dodaj(novi.Ime, novi.Opis, novi.PocetakIzrade, novi.KrajIzrade, novi.Tip, roditelj.Id);
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

				roditelj.Zadaci.Add(novi);
                roditelj.PreurediZadatke();
            }		
		}

		//false vertical, true horizontal
		// 10.06.2018. mstankovic
		// public void Split(Kontejner levaPodkolona, bool flag)
		public void Split(KontejnerZadataka kolonaZaSplit, bool nacinSplitovanja, int idProjekta)
		{
            //kreira se parent kolona koja ce da sadrzi dve podkolone
            //on zauzima mesto prethodne kolone koja ce postati leva podkolona
            KontejnerFaza kf = new KontejnerFaza(kolonaZaSplit.Ime);
            kf.Roditelj = kolonaZaSplit.Roditelj;

			// 10.06.2018. mstankovic (naredne 2 linije)
			//int ind = ((KontejnerFaza)kf.Roditelj).Deca.IndexOf(levaPodkolona);
			//((KontejnerFaza)kf.Roditelj).Deca[ind] = kf;
			int ind = kf.Roditelj.Deca.IndexOf(kolonaZaSplit);
			kf.Roditelj.Deca[ind] = kf;
            kf.Pozicija = ind;
			
			//vertical
			if (nacinSplitovanja == false)
				kf.IsVerticalSplit = true;
			//horizontal
			else
				kf.IsVerticalSplit = false;

			kolonaZaSplit.Roditelj = kf;

			List<Zadatak> zadaciRoditelja = new List<Zadatak>();
			foreach (Zadatak z in kolonaZaSplit.Zadaci)
				zadaciRoditelja.Add(z);


			if (idProjekta != 0)
			{
				kf.Id = kolonaZaSplit.Id;
				try
				{
					PFaza.IzmeniPosleSplita(kf.Id, kf.IsVerticalSplit);
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

			//leva podkolona
			KontejnerZadataka kzNovi = new KontejnerZadataka(kolonaZaSplit);
            kzNovi.Pozicija = 0;
			if (idProjekta != 0)
			{
				try
				{
					kzNovi.Id = PFaza.Dodaj_FZ(kzNovi.Ime, kzNovi.Opis, kzNovi.MaxBrZadataka, kzNovi.PocetakIzrade, kzNovi.KrajIzrade, kf.Id, kzNovi.SirinaPoZadacima, kzNovi.VisinaPoZadacima, idProjekta);
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

			foreach (Zadatak z in zadaciRoditelja)
			{
				try
				{
					if (z.Id != 0) PZadatak.IzmeniFazu(z.Id, kzNovi.Id);
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

			//desna podkolona
			KontejnerZadataka kz = new KontejnerZadataka("New subcolumn 2", kf);
            kz.Pozicija = 1;
			if (idProjekta != 0)
			{
				try
				{
					kz.Id = PFaza.Dodaj_FZ(kz.Ime, kz.Opis, kz.MaxBrZadataka, kz.PocetakIzrade, kz.KrajIzrade, kf.Id, kz.SirinaPoZadacima, kz.VisinaPoZadacima, idProjekta);
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

			kf.Deca.Add(kzNovi);
            kf.Deca.Add(kz);
        }


        // Helper za postavljanje id-jeva kolona templejta na 0
        public void PostaviKoloneNaNula()
        {
            Queue<Kontejner> queue = new Queue<Kontejner>();

            this.RootKolona.Id = 0;
            // Prvo se pronadju elementi ciji je roditelj 
            // ROOT kolona (tj. oni ciji je Roditelj.Id = 0),
            // vezu se za nju i ubace u queue
            foreach (Kontejner k in this.RootKolona.Deca)
            { 
                 queue.Enqueue(k);
            }

            // Ostali elementi se ulancavaju po ugledu na algoritam obilaska stabla po sirini (Breadth First)
            while (queue.Count > 0)
            {
                Kontejner k = queue.Dequeue();

                k.Id = 0;
                // Za svaki kontejner faza je potrebno da se pronadju njegova deca
                // u ostatku liste, da se ulancaju i ubace u red
                if (k.GetType() == typeof(KontejnerFaza))
                {
                    foreach (Kontejner dete in ((KontejnerFaza)k).Deca)
                        queue.Enqueue(dete);
                }
            }
   
        }
    }
}
