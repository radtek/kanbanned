using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using Oracle.ManagedDataAccess.Client;
using Kanbanned.Models;
using System.Collections.ObjectModel;

namespace Kanbanned.Packages
{
	class PZadatak
	{
		public static int Dodaj(string ime, string opis, DateTime? vr_poc, DateTime? vr_kraj, ZTip tip, int id_faze)
		{
			using (OracleCommand cmd = new OracleCommand("P_ZADATAK.Dodaj", DBConnection.con))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add(new OracleParameter("p_ime", OracleDbType.Varchar2, ParameterDirection.Input));
				cmd.Parameters.Add(new OracleParameter("p_opis", OracleDbType.Varchar2, ParameterDirection.Input));
				cmd.Parameters.Add(new OracleParameter("p_poc", OracleDbType.Date, ParameterDirection.Input));
				cmd.Parameters.Add(new OracleParameter("p_kraj", OracleDbType.Date, ParameterDirection.Input));
				cmd.Parameters.Add(new OracleParameter("p_tip", OracleDbType.Decimal, ParameterDirection.Input));
				cmd.Parameters.Add(new OracleParameter("p_faza", OracleDbType.Decimal, ParameterDirection.Input));
				cmd.Parameters.Add(new OracleParameter("p_id", OracleDbType.Decimal, ParameterDirection.Output));
				cmd.Parameters["p_ime"].Value = ime;
				cmd.Parameters["p_opis"].Value = opis;
				cmd.Parameters["p_poc"].Value = vr_poc;
				cmd.Parameters["p_kraj"].Value = vr_kraj;
				cmd.Parameters["p_tip"].Value = tip;
				cmd.Parameters["p_faza"].Value = id_faze;

				cmd.ExecuteNonQuery();

				return int.Parse(cmd.Parameters["p_id"].Value.ToString());
			}
		}

		public static void Obrisi(int id)
		{
			using (OracleCommand cmd = new OracleCommand("P_ZADATAK.Obrisi", DBConnection.con))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add(new OracleParameter("p_id", OracleDbType.Decimal, ParameterDirection.Input));
				cmd.Parameters["p_id"].Value = id;

				cmd.ExecuteNonQuery();
			}
		}

		public static void IzmeniInformacije(int id, string ime, string opis, DateTime? vr_poc, DateTime? vr_kraj, ZTip tip)
		{
			using (OracleCommand cmd = new OracleCommand("P_ZADATAK.Izmeni_Informacije", DBConnection.con))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add(new OracleParameter("p_id", OracleDbType.Decimal, ParameterDirection.Input));
				cmd.Parameters.Add(new OracleParameter("p_ime", OracleDbType.Varchar2, ParameterDirection.Input));
				cmd.Parameters.Add(new OracleParameter("p_opis", OracleDbType.Varchar2, ParameterDirection.Input));
				cmd.Parameters.Add(new OracleParameter("p_poc", OracleDbType.Date, ParameterDirection.Input));
				cmd.Parameters.Add(new OracleParameter("p_kraj", OracleDbType.Date, ParameterDirection.Input));
				cmd.Parameters.Add(new OracleParameter("p_tip", OracleDbType.Decimal, ParameterDirection.Input));
				cmd.Parameters.Add(new OracleParameter("p_user", OracleDbType.Varchar2, ParameterDirection.Input));
				cmd.Parameters["p_id"].Value = id;
				cmd.Parameters["p_ime"].Value = ime;
				cmd.Parameters["p_opis"].Value = opis;
				cmd.Parameters["p_poc"].Value = vr_poc;
				cmd.Parameters["p_kraj"].Value = vr_kraj;
				cmd.Parameters["p_tip"].Value = tip;
				cmd.Parameters["p_user"].Value = Korisnik.KorisnickoIme;

				cmd.ExecuteNonQuery();
			}
		}

		public static void IzmeniFazu(int id_zadatka, int id_faze)
		{
			using (OracleCommand cmd = new OracleCommand("P_ZADATAK.Izmeni_Fazu", DBConnection.con))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add(new OracleParameter("p_id", OracleDbType.Decimal, ParameterDirection.Input));
				cmd.Parameters.Add(new OracleParameter("p_faza", OracleDbType.Decimal, ParameterDirection.Input));
				cmd.Parameters.Add(new OracleParameter("p_user", OracleDbType.Varchar2, ParameterDirection.Input));
				cmd.Parameters["p_id"].Value = id_zadatka;
				cmd.Parameters["p_faza"].Value = id_faze;
				cmd.Parameters["p_user"].Value = Korisnik.KorisnickoIme;

				cmd.ExecuteNonQuery();
			}
		}

		public static void IzmeniStatus(int id_zadatka, ZStatus status)
		{
			using (OracleCommand cmd = new OracleCommand("P_ZADATAK.Izmeni_Status", DBConnection.con))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add(new OracleParameter("p_id", OracleDbType.Decimal, ParameterDirection.Input));
				cmd.Parameters.Add(new OracleParameter("p_status", OracleDbType.Decimal, ParameterDirection.Input));
				cmd.Parameters.Add(new OracleParameter("p_user", OracleDbType.Varchar2, ParameterDirection.Input));
				cmd.Parameters["p_id"].Value = id_zadatka;
				cmd.Parameters["p_status"].Value = status;
				cmd.Parameters["p_user"].Value = Korisnik.KorisnickoIme;

				cmd.ExecuteNonQuery();
			}
		}

		public static int DodajKomentar(string tekst, int id_zadatka)
		{
			using (OracleCommand cmd = new OracleCommand("P_ZADATAK.Dodaj_Komentar", DBConnection.con))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add(new OracleParameter("p_tekst", OracleDbType.Varchar2, ParameterDirection.Input));
				cmd.Parameters.Add(new OracleParameter("p_zad", OracleDbType.Decimal, ParameterDirection.Input));
                cmd.Parameters.Add(new OracleParameter("p_user", OracleDbType.Varchar2, ParameterDirection.Input));
				cmd.Parameters.Add(new OracleParameter("p_id", OracleDbType.Decimal, ParameterDirection.Output));
				cmd.Parameters["p_tekst"].Value = tekst;
				cmd.Parameters["p_zad"].Value = id_zadatka;
                cmd.Parameters["p_user"].Value = Korisnik.KorisnickoIme;

				cmd.ExecuteNonQuery();

				return int.Parse(cmd.Parameters["p_id"].Value.ToString());
			}
		}

		public static void IzmeniKomentar(int id_komentara, string tekst)
		{
			using (OracleCommand cmd = new OracleCommand("P_ZADATAK.Izmeni_Komentar", DBConnection.con))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add(new OracleParameter("p_tekst", OracleDbType.Varchar2, ParameterDirection.Input));
				cmd.Parameters.Add(new OracleParameter("p_id", OracleDbType.Decimal, ParameterDirection.Input));
				cmd.Parameters["p_tekst"].Value = tekst;
				cmd.Parameters["p_id"].Value = id_komentara;

				cmd.ExecuteNonQuery();
			}
		}

		public static void ObrisiKomentar(int id_komentara, int id_korisnika)
		{
			using (OracleCommand cmd = new OracleCommand("P_ZADATAK.Obrisi_Komentar", DBConnection.con))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add(new OracleParameter("p_id", OracleDbType.Decimal, ParameterDirection.Input));
				cmd.Parameters["p_id"].Value = id_komentara;

				cmd.ExecuteNonQuery();
			}
		}

		public static ObservableCollection<Komentar> VratiSveKomentare(int id_zadatka)
		{
			using (OracleCommand cmd = new OracleCommand("P_ZADATAK.Vrati_Sve_Komentare", DBConnection.con))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add(new OracleParameter("komentari", OracleDbType.RefCursor, ParameterDirection.ReturnValue));
				cmd.Parameters.Add(new OracleParameter("p_zad", OracleDbType.Decimal, ParameterDirection.Input));
				cmd.Parameters["p_zad"].Value = id_zadatka;

				OracleDataReader dr = cmd.ExecuteReader();

                //List<Komentar> komentari = new List<Komentar>();
                ObservableCollection<Komentar> komentari = new ObservableCollection<Komentar>();

				while (dr.Read())
				{
					int id = (int)dr.GetDecimal(0);
					string tekst = dr.GetString(1);
					int zad_id = (int)dr.GetDecimal(2);
					string username = dr.GetString(3);

					Komentar kom = new Komentar()
					{
						Id = id,
						Tekst = tekst,
						Zadatak = zad_id,
						Korisnik = username
					};

					komentari.Add(kom);
				}
				dr.Close();

				return komentari;
			}
		}

		public static int DodajKontrolnuTacku(string ime, int id_zadatka)
		{
			using (OracleCommand cmd = new OracleCommand("P_ZADATAK.Dodaj_Checkpoint", DBConnection.con))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add(new OracleParameter("p_ime", OracleDbType.Varchar2, ParameterDirection.Input));
				cmd.Parameters.Add(new OracleParameter("p_zad", OracleDbType.Decimal, ParameterDirection.Input));
				cmd.Parameters.Add(new OracleParameter("p_id", OracleDbType.Decimal, ParameterDirection.Output));
				cmd.Parameters["p_ime"].Value = ime;
				cmd.Parameters["p_zad"].Value = id_zadatka;

				cmd.ExecuteNonQuery();

				return int.Parse(cmd.Parameters["p_id"].Value.ToString());
			}
		}

		public static void IzmeniKontrolnuTacku(int id, bool status)
		{
			using (OracleCommand cmd = new OracleCommand("P_ZADATAK.Izmeni_Checkpoint", DBConnection.con))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add(new OracleParameter("p_id", OracleDbType.Decimal, ParameterDirection.Input));
				cmd.Parameters.Add(new OracleParameter("p_status", OracleDbType.Decimal, ParameterDirection.Input));
				cmd.Parameters["p_id"].Value = id;
				cmd.Parameters["p_status"].Value = status ? 1 : 0;

				cmd.ExecuteNonQuery();
			}
		}

		public static void ObrisiKontrolnuTacku(int id)
		{
			using (OracleCommand cmd = new OracleCommand("P_ZADATAK.Obrisi_Checkpoint", DBConnection.con))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add(new OracleParameter("p_id", OracleDbType.Decimal, ParameterDirection.Input));
				cmd.Parameters["p_id"].Value = id;

				cmd.ExecuteNonQuery();
			}
		}

		public static ObservableCollection<KontrolnaTacka> VratiSveKontrolneTacke(int id_zadatka)
		{
			using (OracleCommand cmd = new OracleCommand("P_ZADATAK.Vrati_Sve_Checkpointove", DBConnection.con))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add(new OracleParameter("checkpointovi", OracleDbType.RefCursor, ParameterDirection.ReturnValue));
				cmd.Parameters.Add(new OracleParameter("p_zad", OracleDbType.Decimal, ParameterDirection.Input));
				cmd.Parameters["p_zad"].Value = id_zadatka;

				OracleDataReader dr = cmd.ExecuteReader();

				ObservableCollection<KontrolnaTacka> kTacke = new ObservableCollection<KontrolnaTacka>();

				while (dr.Read())
				{
					int id = (int)dr.GetDecimal(0);
					string ime = dr.GetString(1);
					bool status = (int)dr.GetDecimal(4) == 1 ? true : false;

					KontrolnaTacka kt = new KontrolnaTacka()
					{
						Id = id,
						Naziv = ime,
						Vrednost = status
					};

					kTacke.Add(kt);
				}
				dr.Close();

				return kTacke;
			}
		}

		public static void Zakljucaj(int id)
		{
			using (OracleCommand cmd = new OracleCommand("P_ZADATAK.Zakljucaj", DBConnection.con))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add(new OracleParameter("p_id", OracleDbType.Decimal, ParameterDirection.Input));
				cmd.Parameters["p_id"].Value = id;

				cmd.ExecuteNonQuery();
			}
		}

		public static void Otkljucaj (int id)
		{
			using (OracleCommand cmd = new OracleCommand("P_ZADATAK.Otkljucaj", DBConnection.con))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add(new OracleParameter("p_id", OracleDbType.Decimal, ParameterDirection.Input));
				cmd.Parameters["p_id"].Value = id;

				cmd.ExecuteNonQuery();
			}
		}

		public static List<Zadatak> VratiZadatkeProjekta(int id_projekta)
		{
			using (OracleCommand cmd = new OracleCommand("P_ZADATAK.Vrati_Zadatke_Projekta", DBConnection.con))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add(new OracleParameter("zadaci", OracleDbType.RefCursor, ParameterDirection.ReturnValue));
				cmd.Parameters.Add(new OracleParameter("projekat_id", OracleDbType.Varchar2, ParameterDirection.Input));
				cmd.Parameters["projekat_id"].Value = id_projekta;

				OracleDataReader dr = cmd.ExecuteReader();

				List<Zadatak> zadaci = new List<Zadatak>();

				while (dr.Read())
				{
					int id = (int)dr.GetDecimal(0);
					string ime = dr.GetString(1);
					string opis = dr.GetString(2);
					ZStatus status = (ZStatus)dr.GetDecimal(3);
					DateTime? pocetak = dr.IsDBNull(4) ? (DateTime?)null : dr.GetDateTime(4);
					DateTime? kraj = dr.IsDBNull(5) ? (DateTime?)null : dr.GetDateTime(5);
					ZTip tip = (ZTip)dr.GetDecimal(6);

					Zadatak zad = new Zadatak()
					{
						Id = id,
						Ime = ime,
						Opis = opis,
						Status = status,
						PocetakIzrade = pocetak,
						KrajIzrade = kraj,
						Tip = tip
					};

					zadaci.Add(zad);
				}
				dr.Close();

				return zadaci;
			}
		}

		public static List<Zadatak> VratiZadatkeFaze(int id_faze)
		{
			using (OracleCommand cmd = new OracleCommand("P_ZADATAK.Vrati_Zadatke_Faze", DBConnection.con))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add(new OracleParameter("zadaci", OracleDbType.RefCursor, ParameterDirection.ReturnValue));
				cmd.Parameters.Add(new OracleParameter("faza_id", OracleDbType.Varchar2, ParameterDirection.Input));
				cmd.Parameters["faza_id"].Value = id_faze;

				OracleDataReader dr = cmd.ExecuteReader();

				List<Zadatak> zadaci = new List<Zadatak>();

				while (dr.Read())
				{
					int id = (int)dr.GetDecimal(0);
					string ime = dr.GetString(1);
					string opis = dr.IsDBNull(2) ? null : dr.GetString(2);
					ZStatus status = (ZStatus)dr.GetDecimal(3);
					DateTime? pocetak = dr.IsDBNull(4) ? (DateTime?)null : dr.GetDateTime(4);
					DateTime? kraj = dr.IsDBNull(5) ? (DateTime?)null : dr.GetDateTime(5);
					ZTip tip = (ZTip)dr.GetDecimal(6);

					Zadatak zad = new Zadatak()
					{
						Id = id,
						Ime = ime,
						Opis = opis,
						Status = status,
						PocetakIzrade = pocetak,
						KrajIzrade = kraj,
						Tip = tip
					};

					zadaci.Add(zad);
				}
				dr.Close();

				return zadaci;
			}
		}

        public static bool IsZakljucan(int id_zadatka)
        {
            using (OracleCommand cmd = new OracleCommand("P_ZADATAK.Is_Zakljucan", DBConnection.con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new OracleParameter("fleg", OracleDbType.Decimal, ParameterDirection.ReturnValue));
                cmd.Parameters.Add(new OracleParameter("p_id", OracleDbType.Decimal, ParameterDirection.Input));
                cmd.Parameters["p_id"].Value = id_zadatka;

                cmd.ExecuteNonQuery();

                return int.Parse(cmd.Parameters["fleg"].Value.ToString()) == 1;
            }
        }
    }
}
