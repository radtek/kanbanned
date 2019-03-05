using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using Oracle.ManagedDataAccess.Client;
using Kanbanned.Models;

namespace Kanbanned.Packages
{
	public class PFaza
	{
		public static int Dodaj_FZ		// dodavanje kontejnera zadataka u bazu
			(
				string ime,
				string opis,
				int max_br_zad,
				DateTime? vr_poc,
				DateTime? vr_kraj,
				int? roditelj,
				int sirina,
				int visina,
				int id_projekta
			)
		{
			using (OracleCommand cmd = new OracleCommand("P_FAZA.Dodaj_FZ", DBConnection.con))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add(new OracleParameter("p_ime", OracleDbType.Varchar2, ParameterDirection.Input));
				cmd.Parameters.Add(new OracleParameter("p_opis", OracleDbType.Varchar2, ParameterDirection.Input));
				cmd.Parameters.Add(new OracleParameter("p_zad", OracleDbType.Decimal, ParameterDirection.Input));
				cmd.Parameters.Add(new OracleParameter("p_poc", OracleDbType.Date, ParameterDirection.Input));
				cmd.Parameters.Add(new OracleParameter("p_kraj", OracleDbType.Date, ParameterDirection.Input));
				cmd.Parameters.Add(new OracleParameter("p_rod", OracleDbType.Decimal, ParameterDirection.Input));
				cmd.Parameters.Add(new OracleParameter("p_w", OracleDbType.Decimal, ParameterDirection.Input));
				cmd.Parameters.Add(new OracleParameter("p_h", OracleDbType.Decimal, ParameterDirection.Input));
				cmd.Parameters.Add(new OracleParameter("p_proj", OracleDbType.Decimal, ParameterDirection.Input));
				cmd.Parameters.Add(new OracleParameter("p_id", OracleDbType.Decimal, ParameterDirection.Output));
				cmd.Parameters["p_ime"].Value = ime;
				cmd.Parameters["p_opis"].Value = opis;
				cmd.Parameters["p_zad"].Value = max_br_zad;
				cmd.Parameters["p_poc"].Value = vr_poc;
				cmd.Parameters["p_kraj"].Value = vr_kraj;
				cmd.Parameters["p_rod"].Value = roditelj == 0 ? null : roditelj;
				cmd.Parameters["p_w"].Value = sirina;
				cmd.Parameters["p_h"].Value = visina;
				cmd.Parameters["p_proj"].Value = id_projekta;

				cmd.ExecuteNonQuery();

				return int.Parse(cmd.Parameters["p_id"].Value.ToString());
			}
		}

		public static int Dodaj_FK     // dodavanje kontejnera faza u bazu
			(
				string ime,
				string opis,
				bool vsplit,
				DateTime? vr_poc,
				DateTime? vr_kraj,
				int? roditelj,
				int id_projekta
			)
		{
			using (OracleCommand cmd = new OracleCommand("P_FAZA.Dodaj_FK", DBConnection.con))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add(new OracleParameter("p_ime", OracleDbType.Varchar2, ParameterDirection.Input));
				cmd.Parameters.Add(new OracleParameter("p_opis", OracleDbType.Varchar2, ParameterDirection.Input));
				cmd.Parameters.Add(new OracleParameter("p_vsplit", OracleDbType.Decimal, ParameterDirection.Input));
				cmd.Parameters.Add(new OracleParameter("p_poc", OracleDbType.Date, ParameterDirection.Input));
				cmd.Parameters.Add(new OracleParameter("p_kraj", OracleDbType.Date, ParameterDirection.Input));
				cmd.Parameters.Add(new OracleParameter("p_rod", OracleDbType.Decimal, ParameterDirection.Input));
				cmd.Parameters.Add(new OracleParameter("p_proj", OracleDbType.Decimal, ParameterDirection.Input));
				cmd.Parameters.Add(new OracleParameter("p_id", OracleDbType.Decimal, ParameterDirection.Output));
				cmd.Parameters["p_ime"].Value = ime;
				cmd.Parameters["p_opis"].Value = opis;
				cmd.Parameters["p_vsplit"].Value = (vsplit ? 1 : 0);
				cmd.Parameters["p_poc"].Value = vr_poc;
				cmd.Parameters["p_kraj"].Value = vr_kraj;
				cmd.Parameters["p_rod"].Value = roditelj == 0 ? null : roditelj;
				cmd.Parameters["p_proj"].Value = id_projekta;

				cmd.ExecuteNonQuery();

				return int.Parse(cmd.Parameters["p_id"].Value.ToString());
			}
		}

		public static void Obrisi(int id)
		{
			using (OracleCommand cmd = new OracleCommand("P_FAZA.Obrisi", DBConnection.con))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add(new OracleParameter("p_id", OracleDbType.Decimal, ParameterDirection.Input));
				cmd.Parameters["p_id"].Value = id;

				cmd.ExecuteNonQuery();
			}
		}

		public static void IzmeniPosleSplita(int id, bool vsplit)
		{
			using (OracleCommand cmd = new OracleCommand("P_FAZA.Izmeni_Tip_Nakon_Splita", DBConnection.con))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add(new OracleParameter("p_id", OracleDbType.Decimal, ParameterDirection.Input));
				cmd.Parameters.Add(new OracleParameter("p_vsplit", OracleDbType.Decimal, ParameterDirection.Input));
				cmd.Parameters["p_id"].Value = id;
				cmd.Parameters["p_vsplit"].Value = vsplit ? 1 : 0;

				cmd.ExecuteNonQuery();
			}
		}

		public static void IzmeniInformacije(int id, string ime, string opis, DateTime? pocetak, DateTime? kraj)
		{
			using (OracleCommand cmd = new OracleCommand("P_FAZA.Izmeni_Informacije", DBConnection.con))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add(new OracleParameter("p_id", OracleDbType.Decimal, ParameterDirection.Input));
				cmd.Parameters.Add(new OracleParameter("p_ime", OracleDbType.Varchar2, ParameterDirection.Input));
				cmd.Parameters.Add(new OracleParameter("p_opis", OracleDbType.Varchar2, ParameterDirection.Input));
				cmd.Parameters.Add(new OracleParameter("p_poc", OracleDbType.Date, ParameterDirection.Input));
				cmd.Parameters.Add(new OracleParameter("p_kraj", OracleDbType.Date, ParameterDirection.Input));
				cmd.Parameters.Add(new OracleParameter("p_user", OracleDbType.Varchar2, ParameterDirection.Input));
				cmd.Parameters["p_id"].Value = id;
				cmd.Parameters["p_ime"].Value = ime;
				cmd.Parameters["p_opis"].Value = opis;
				cmd.Parameters["p_poc"].Value = pocetak;
				cmd.Parameters["p_kraj"].Value = kraj;
				cmd.Parameters["p_user"].Value = Korisnik.KorisnickoIme;

				cmd.ExecuteNonQuery();
			}
		}

		public static void IzmeniDimenzije(int id, int sirina, int visina)
		{
			using (OracleCommand cmd = new OracleCommand("P_FAZA.Izmeni_Dimenzije", DBConnection.con))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add(new OracleParameter("p_id", OracleDbType.Decimal, ParameterDirection.Input));
				cmd.Parameters.Add(new OracleParameter("p_w", OracleDbType.Decimal, ParameterDirection.Input));
				cmd.Parameters.Add(new OracleParameter("p_h", OracleDbType.Decimal, ParameterDirection.Input));
				cmd.Parameters.Add(new OracleParameter("p_user", OracleDbType.Varchar2, ParameterDirection.Input));
				cmd.Parameters["p_id"].Value = id;
				cmd.Parameters["p_w"].Value = sirina;
				cmd.Parameters["p_h"].Value = visina;
				cmd.Parameters["p_user"].Value = Korisnik.KorisnickoIme;
				
				cmd.ExecuteNonQuery();
			}
		}

		public static void Pomeri(int id, int smer)
		{
			using (OracleCommand cmd = new OracleCommand("P_FAZA.Pomeri", DBConnection.con))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add(new OracleParameter("p_id", OracleDbType.Decimal, ParameterDirection.Input));
				cmd.Parameters.Add(new OracleParameter("p_smer", OracleDbType.Decimal, ParameterDirection.Input));
				cmd.Parameters["p_id"].Value = id;
				cmd.Parameters["p_smer"].Value = smer;

				cmd.ExecuteNonQuery();
			}
		}

		public static void IzmeniPoziciju(int id, int nova_pozicija)
		{
			using (OracleCommand cmd = new OracleCommand("P_FAZA.Izmeni_Poziciju", DBConnection.con))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add(new OracleParameter("p_id", OracleDbType.Decimal, ParameterDirection.Input));
				cmd.Parameters.Add(new OracleParameter("p_new_pos", OracleDbType.Decimal, ParameterDirection.Input));
				cmd.Parameters.Add(new OracleParameter("p_user", OracleDbType.Varchar2, ParameterDirection.Input));
				cmd.Parameters["p_id"].Value = id;
				cmd.Parameters["p_new_pos"].Value = nova_pozicija;
				cmd.Parameters["p_user"].Value = Korisnik.KorisnickoIme;

				cmd.ExecuteNonQuery();
			}
		}

		public static void IzmeniMaxBrZadataka(int id, int br_zadataka)
		{
			using (OracleCommand cmd = new OracleCommand("P_FAZA.Izmeni_Max_Br_Zad", DBConnection.con))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add(new OracleParameter("p_id", OracleDbType.Decimal, ParameterDirection.Input));
				cmd.Parameters.Add(new OracleParameter("p_zad", OracleDbType.Decimal, ParameterDirection.Input));
				cmd.Parameters.Add(new OracleParameter("p_user", OracleDbType.Varchar2, ParameterDirection.Input));
				cmd.Parameters["p_id"].Value = id;
				cmd.Parameters["p_zad"].Value = br_zadataka;
				cmd.Parameters["p_user"].Value = Korisnik.KorisnickoIme;

				cmd.ExecuteNonQuery();
			}
		}

		public static int VratiMaxPoziciju(int? id_faze, int id_projekta)
		{
			using (OracleCommand cmd = new OracleCommand("P_FAZA.Vrati_Max_Poziciju", DBConnection.con))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add(new OracleParameter("max_pos", OracleDbType.Decimal, ParameterDirection.ReturnValue));
				cmd.Parameters.Add(new OracleParameter("id_faze", OracleDbType.Decimal, ParameterDirection.Input));
				cmd.Parameters.Add(new OracleParameter("id_projekta", OracleDbType.Decimal, ParameterDirection.Input));
				cmd.Parameters["id_faze"].Value = id_faze;
				cmd.Parameters["id_projekta"].Value = id_projekta;

				cmd.ExecuteNonQuery();

				return int.Parse(cmd.Parameters["max_pos"].Value.ToString());
			}
		}

		public static int VratiMaxSirinu(int? id_faze, int id_projekta)
		{
			using (OracleCommand cmd = new OracleCommand("P_FAZA.Vrati_Max_Sirinu", DBConnection.con))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add(new OracleParameter("width", OracleDbType.Decimal, ParameterDirection.ReturnValue));
				cmd.Parameters.Add(new OracleParameter("id_faze", OracleDbType.Decimal, ParameterDirection.Input));
				cmd.Parameters.Add(new OracleParameter("id_projekta", OracleDbType.Decimal, ParameterDirection.Input));
				cmd.Parameters["id_faze"].Value = id_faze;
				cmd.Parameters["id_projekta"].Value = id_projekta;

				cmd.ExecuteNonQuery();

				return int.Parse(cmd.Parameters["width"].Value.ToString());
			}
		}

		public static int VratiMaxVisinu(int? id_faze, int id_projekta)
		{
			using (OracleCommand cmd = new OracleCommand("P_FAZA.Vrati_Max_Visinu", DBConnection.con))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add(new OracleParameter("height", OracleDbType.Decimal, ParameterDirection.ReturnValue));
				cmd.Parameters.Add(new OracleParameter("id_faze", OracleDbType.Decimal, ParameterDirection.Input));
				cmd.Parameters.Add(new OracleParameter("id_projekta", OracleDbType.Decimal, ParameterDirection.Input));
				cmd.Parameters["id_faze"].Value = id_faze;
				cmd.Parameters["id_projekta"].Value = id_projekta;

				cmd.ExecuteNonQuery();

				return int.Parse(cmd.Parameters["height"].Value.ToString());
			}
		}

		public static int VratiBrojDece(int? id_faze, int id_projekta)
		{
			using (OracleCommand cmd = new OracleCommand("P_FAZA.Vrati_Broj_Dece", DBConnection.con))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add(new OracleParameter("br_dece", OracleDbType.Decimal, ParameterDirection.ReturnValue));
				cmd.Parameters.Add(new OracleParameter("id_faze", OracleDbType.Decimal, ParameterDirection.Input));
				cmd.Parameters.Add(new OracleParameter("id_projekta", OracleDbType.Decimal, ParameterDirection.Input));
				cmd.Parameters["id_faze"].Value = id_faze;
				cmd.Parameters["id_projekta"].Value = id_projekta;

				cmd.ExecuteNonQuery();

				return int.Parse(cmd.Parameters["br_dece"].Value.ToString());
			}
		}

		public static List<Kontejner> VratiFaze(int id_projekta)
		{
			List<Kontejner> faze = new List<Kontejner>();

			using (OracleCommand cmd = new OracleCommand("P_FAZA.Vrati_Faze_K", DBConnection.con))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add(new OracleParameter("faze_k", OracleDbType.RefCursor, ParameterDirection.ReturnValue));
				cmd.Parameters.Add(new OracleParameter("id_projekta", OracleDbType.Decimal, ParameterDirection.Input));
				cmd.Parameters["id_projekta"].Value = id_projekta;

				OracleDataReader dr = cmd.ExecuteReader();

				while (dr.Read())
				{
					int id = (int)dr.GetDecimal(0);
					string ime = dr.GetString(1);
					string opis = dr.IsDBNull(2) ? null : dr.GetString(2);
					DateTime? vp = dr.IsDBNull(3) ? (DateTime?)null : dr.GetDateTime(3);
					DateTime? vk = dr.IsDBNull(4) ? (DateTime?)null : dr.GetDateTime(4);
					int? roditelj = dr.IsDBNull(5) ? (int?)null : (int)dr.GetDecimal(5);
					int pozicija = (int)dr.GetDecimal(6);
					bool vsplit = (int)dr.GetDecimal(7) == 1 ? true : false;

					KontejnerFaza kf = new KontejnerFaza()
					{
						Id = id,
						Ime = ime,
						Opis = opis,
						PocetakIzrade = vp,
						KrajIzrade = vk,
						Pozicija = pozicija,
						IsVerticalSplit = vsplit,
						Roditelj = new KontejnerFaza() { Id = roditelj ?? default(int) }
					};

					faze.Add(kf);
				}

				dr.Close();
			}

			using (OracleCommand cmd = new OracleCommand("P_FAZA.Vrati_Faze_Z", DBConnection.con))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add(new OracleParameter("faze_z", OracleDbType.RefCursor, ParameterDirection.ReturnValue));
				cmd.Parameters.Add(new OracleParameter("id_projekta", OracleDbType.Decimal, ParameterDirection.Input));
				cmd.Parameters["id_projekta"].Value = id_projekta;

				OracleDataReader dr = cmd.ExecuteReader();

				while (dr.Read())
				{
					int id = (int)dr.GetDecimal(0);
					string ime = dr.GetString(1);
					string opis = dr.IsDBNull(2) ? null : dr.GetString(2);
					DateTime? vp = dr.IsDBNull(3) ? (DateTime?)null : dr.GetDateTime(3);
					DateTime? vk = dr.IsDBNull(4) ? (DateTime?)null : dr.GetDateTime(4);
					int? roditelj = dr.IsDBNull(5) ? (int?)null : (int)dr.GetDecimal(5);
					int pozicija = (int)dr.GetDecimal(6);
					int max_zad = (int)dr.GetDecimal(7);
					int w = (int)dr.GetDecimal(8);
					int h = (int)dr.GetDecimal(9);

					KontejnerZadataka kz = new KontejnerZadataka()
					{
						Id = id,
						Ime = ime,
						Opis = opis,
						PocetakIzrade = vp,
						KrajIzrade = vk,
						Pozicija = pozicija,
						MaxBrZadataka = max_zad,
						//SirinaPoZadacima = w,
						//VisinaPoZadacima = h,
						Roditelj = new KontejnerFaza() { Id = roditelj ?? default(int) }
					};

					faze.Add(kz);
				}

				dr.Close();
			}

			return faze;
		}



	}
}
