using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using Kanbanned.Models;

namespace Kanbanned.Packages
{
	public class PProjekat
	{
		public static int DodajProjekat(string ime, string opis)
		{
			using (OracleCommand cmd = new OracleCommand("P_PROJEKAT.Dodaj", DBConnection.con))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add(new OracleParameter("p_ime", OracleDbType.Varchar2, ParameterDirection.Input));
				cmd.Parameters.Add(new OracleParameter("p_opis", OracleDbType.Varchar2, ParameterDirection.Input));
				cmd.Parameters.Add(new OracleParameter("p_id", OracleDbType.Decimal, ParameterDirection.Output));
				cmd.Parameters["p_ime"].Value = ime;
				cmd.Parameters["p_opis"].Value = opis;

				cmd.ExecuteNonQuery();

                return int.Parse(cmd.Parameters["p_id"].Value.ToString());
			}
		}

		public static void IzmeniProjekat(int id, string ime, string opis)
		{
			using (OracleCommand cmd = new OracleCommand("P_PROJEKAT.Izmeni", DBConnection.con))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add(new OracleParameter("p_id", OracleDbType.Decimal, ParameterDirection.Input));
				cmd.Parameters.Add(new OracleParameter("p_ime", OracleDbType.Varchar2, ParameterDirection.Input));
				cmd.Parameters.Add(new OracleParameter("p_opis", OracleDbType.Varchar2, ParameterDirection.Input));
				cmd.Parameters["p_id"].Value = id;
				cmd.Parameters["p_ime"].Value = ime;
				cmd.Parameters["p_opis"].Value = opis;

				cmd.ExecuteNonQuery();

				Korisnik.Projekti = VratiSveProjekte(Korisnik.KorisnickoIme);	// osvezava listu projekata korisnika
			}
		}

		public static void ObrisiProjekat(int id)
		{
			using (OracleCommand cmd = new OracleCommand("P_PROJEKAT.Obrisi", DBConnection.con))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add(new OracleParameter("p_id", OracleDbType.Decimal, ParameterDirection.Input));
				cmd.Parameters["p_id"].Value = id;

				cmd.ExecuteNonQuery();

				Korisnik.Projekti = VratiSveProjekte(Korisnik.KorisnickoIme);
			}
		}

		public static Dictionary<Projekat, string> VratiSveProjekte(string id_korisnika)
		{
			using (OracleCommand cmd = new OracleCommand("P_PROJEKAT.Vrati_Sve", DBConnection.con))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add(new OracleParameter("projekti", OracleDbType.RefCursor, ParameterDirection.ReturnValue));
				cmd.Parameters.Add(new OracleParameter("id_korisnika", OracleDbType.Varchar2, ParameterDirection.Input));
				cmd.Parameters["id_korisnika"].Value = id_korisnika;

				OracleDataReader dr = cmd.ExecuteReader();

				Dictionary<Projekat, string> projekti = new Dictionary<Projekat, string>();		// sadrzi listu korisnickih projekata sa imenima uloga koje on obavlja na njima

				while (dr.Read())
				{
					int id = (int)dr.GetDecimal(0);
					string ime = dr.GetString(1);
                    string opis = dr.IsDBNull(2) ? null : dr.GetString(2);
                    DateTime kreiran = dr.GetDateTime(3);
					DateTime? kraj = dr.IsDBNull(4) ? (DateTime?)null : dr.GetDateTime(4);
					string uloga = dr.GetString(5);

                    Projekat projekat = new Projekat(id, ime, opis, kreiran, kraj);
                    projekat.Privilegija = uloga;
                    projekti.Add(projekat, uloga);
				}
				dr.Close();

				return projekti;
			}
		}

		public static Dictionary<string, string> VratiKorisnike(int id_projekta)
		{
			using (OracleCommand cmd = new OracleCommand("P_PROJEKAT.Vrati_Korisnike", DBConnection.con))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add(new OracleParameter("korisnici", OracleDbType.RefCursor, ParameterDirection.ReturnValue));
				cmd.Parameters.Add(new OracleParameter("id_projekta", OracleDbType.Varchar2, ParameterDirection.Input));
				cmd.Parameters["id_projekta"].Value = id_projekta;

				OracleDataReader dr = cmd.ExecuteReader();

				Dictionary<string, string> korisnici = new Dictionary<string, string>();     // sadrzi listu imena korisnika sa imenima uloga koje on obavlja na datom projektu

				while (dr.Read())
					korisnici.Add(dr.GetString(0), dr.GetString(1));

				dr.Close();

				return korisnici;
			}
		}

		public static void SacuvajKaoTemplejt(int id_projekta, string ime_templejta, string opis_templejta)
		{
			using (OracleCommand cmd = new OracleCommand("P_PROJEKAT.Sacuvaj_Kao_Templejt", DBConnection.con))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add(new OracleParameter("p_proj", OracleDbType.Decimal, ParameterDirection.Input));
				cmd.Parameters.Add(new OracleParameter("p_un", OracleDbType.Varchar2, ParameterDirection.Input));
				cmd.Parameters.Add(new OracleParameter("p_ime", OracleDbType.Varchar2, ParameterDirection.Input));
				cmd.Parameters.Add(new OracleParameter("p_opis", OracleDbType.Varchar2, ParameterDirection.Input));
				cmd.Parameters["p_proj"].Value = id_projekta;
				cmd.Parameters["p_un"].Value = Korisnik.KorisnickoIme;
				cmd.Parameters["p_ime"].Value = ime_templejta;
				cmd.Parameters["p_opis"].Value = opis_templejta;

				cmd.ExecuteNonQuery();

				Korisnik.Projekti = VratiSveProjekte(Korisnik.KorisnickoIme);   // osvezava listu projekata korisnika
			}
		}

		public static ListaProjekata VratiTemplejte()
		{
			using (OracleCommand cmd = new OracleCommand("P_PROJEKAT.Vrati_Sve_Templejte", DBConnection.con))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add(new OracleParameter("templejti", OracleDbType.RefCursor, ParameterDirection.ReturnValue));
				cmd.Parameters.Add(new OracleParameter("p_user", OracleDbType.Varchar2, ParameterDirection.Input));
				cmd.Parameters["p_user"].Value = Korisnik.KorisnickoIme;

				OracleDataReader dr = cmd.ExecuteReader();

                ListaProjekata templejti = new ListaProjekata();

				while (dr.Read())
				{
					int id = (int)dr.GetDecimal(0);
					string ime = dr.GetString(1);
					string opis = dr.IsDBNull(2) ? null : dr.GetString(2);
					DateTime kreiranje = dr.GetDateTime(3);
					DateTime? kraj = dr.IsDBNull(4) ? (DateTime?)null : dr.GetDateTime(4);

					Projekat tpl = new Projekat()
					{
						Id = id,
						Ime = ime,
						Opis = opis,
						DatumKreiranja = kreiranje,
						DatumZavrsetka = kraj,
					};

					templejti.Add(tpl);
				}
				dr.Close();

				return templejti;
			}
		}
	}
}
