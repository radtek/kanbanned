using Kanbanned.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Oracle.ManagedDataAccess.Client;
using System.ComponentModel;
using System.Threading;
using Kanbanned.ViewModels;
using System.Windows.Controls;
using Kanbanned.Helpers;
using System.Windows;

namespace Kanbanned.Packages
{
    public class RadSaBazom
    {
        /// <summary>
        /// U pozadinskom threadu salju se upiti koji ispituju da li je bilo promena u tabeli
        /// </summary>
        public static readonly RefreshBackgroundWorker RefreshTabele = new RefreshBackgroundWorker();
    
        /// <summary>
        /// Snimanje projekta u bazu nakon kreiranja
        /// </summary>
        /// <param name="p">Projekat koji se snima</param>
        /// mozda da stavim da se u p.id ucita id mada mislim da nije potrebno 
        /// jer svejedno osvezavamo listu projekata nakon kreiranja
        public static void KreirajProjekat(Projekat p)
        {
			try
			{
				//snima se projekat u bazu
				//kad se pozove funkcija da se doda projekat treba da vrati id
				p.Id = PProjekat.DodajProjekat(p.Ime, p.Opis);

				//dodavanje kreatora
				PRadiNa.DodajVezu(Korisnik.KorisnickoIme, p.Id, "KREATOR");

				Korisnik.Projekti = PProjekat.VratiSveProjekte(Korisnik.KorisnickoIme);   // osvezava listu projekata korisnika

				//dodavanje radnika
				foreach (RadnikNaProjektu r in p.RadniciNaProjektu)
				{
					PRadiNa.DodajVezu(r.Ime, p.Id, r.Uloga);
				}

				Queue<Kontejner> queue = new Queue<Kontejner>();
				foreach (Kontejner k in p.TabelaProjekta.RootKolona.Deca)
					queue.Enqueue(k);

				// Ostali elementi se ulancavaju po ugledu na algoritam obilaska stabla po sirini (Breadth First)
				while (queue.Count > 0)
				{
					Kontejner k = queue.Dequeue();
					// Za svaki kontejner faza je potrebno da se pronadju njegova deca
					// u ostatku liste, da se ulancaju i ubace u red

					if (k.GetType() == typeof(KontejnerFaza))
					{
						if (k.Roditelj == p.TabelaProjekta.RootKolona)
						{
							//ako je jedna od prvih kolona projekta onda nema roditelja(tj. roditelj joj je sama tabela) tako da se ubacuje za id nula
							k.Id = PFaza.Dodaj_FK(k.Ime, k.Opis, ((KontejnerFaza)k).IsVerticalSplit, null, null, null, p.Id);
						}
						else
						{
							//ubacuje se id roditelja tako da on mora biti procitan u prethodno
							k.Id = PFaza.Dodaj_FK(k.Ime, k.Opis, ((KontejnerFaza)k).IsVerticalSplit, null, null, k.Roditelj.Id, p.Id);
						}

						foreach (Kontejner dete in ((KontejnerFaza)k).Deca)
							queue.Enqueue(dete);

					}
					else if (k.GetType() == typeof(KontejnerZadataka))
					{
						if (k.Roditelj == p.TabelaProjekta.RootKolona)
							k.Id = PFaza.Dodaj_FZ(k.Ime, k.Opis, ((KontejnerZadataka)k).MaxBrZadataka, null, null, null, ((KontejnerZadataka)k).SirinaPoZadacima, ((KontejnerZadataka)k).VisinaPoZadacima, p.Id);
						else
							k.Id = PFaza.Dodaj_FZ(k.Ime, k.Opis, ((KontejnerZadataka)k).MaxBrZadataka, null, null, k.Roditelj.Id, ((KontejnerZadataka)k).SirinaPoZadacima, ((KontejnerZadataka)k).VisinaPoZadacima, p.Id);
						foreach (Zadatak z in ((KontejnerZadataka)k).Zadaci)
						{
							//treba i da se dodaju svi komentari i checkpointi zajedno sa zadatkom
							z.Id = PZadatak.Dodaj(z.Ime, z.Opis, z.PocetakIzrade, z.KrajIzrade, z.Tip, k.Id);
							foreach (Komentar kom in z.Komentari)
							{
								kom.Id = PZadatak.DodajKomentar(kom.Tekst, z.Id);
							}

							foreach (KontrolnaTacka kt in z.KontrolneTacke)
							{
								kt.Id = PZadatak.DodajKontrolnuTacku(kt.Naziv, z.Id);
							}
						}
					}
				}
			}
			catch (OracleException oraError)
			{
				try
				{
					System.Windows.MessageBox.Show(PPoruka.VratiPrevod(oraError.Number.ToString()));
				}
				catch (OracleException err)
				{
					System.Windows.MessageBox.Show("Greska pri kreiranju projekta");
				}
			}
        }

        /// <summary>
        /// Editovanje projekta
        /// Izmena imena i opisa i dodavanje ili brisanje radnika sa projekta
        /// </summary>
        /// <param name="ime">Novo ime projekta</param>
        /// <param name="opis">Opis</param>
        /// <param name="zaDodavanje">Lista radnika koje treba dodati</param>
        /// <param name="zaBrisanje">Lista radnika koje treba ukloniti sa projekta</param>
        public static void IzmenaProjekta(int id, String ime, String opis, List<RadnikNaProjektu> zaDodavanje, List<RadnikNaProjektu> zaBrisanje)
        {
            OracleTransaction txn = null;

            try
            {
                txn = DBConnection.con.BeginTransaction();

                //izmena projekta
                PProjekat.IzmeniProjekat(id, ime, opis);

                //dodavanje radnika
                zaDodavanje.ForEach(radnik => PRadiNa.DodajVezu(radnik.Ime, id, radnik.Uloga));

                //brisanje radnika
                zaBrisanje.ForEach(radnik => PRadiNa.ObrisiVezu(radnik.Ime, id));

                //treba i za izmenu postojecih da se ubaci

                txn.Commit();
            }
            catch (OracleException oraError)
            {
				try
				{
					System.Windows.MessageBox.Show(PPoruka.VratiPrevod(oraError.Number.ToString()));
				}
				catch (OracleException err)
				{
					System.Windows.MessageBox.Show("Greska pri izmeni projekta");
				}
				txn.Rollback();
            }
            catch (Exception error)
            {
                System.Windows.MessageBox.Show("Greska pri izmeni projekta");
                txn.Rollback();
            }
        }
    }
}
