using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using Kanbanned.Models;

namespace Kanbanned.Packages
{
	public class PKorisnik
	{
		public static void Register(string username, string password, string company)
		{
			using (OracleCommand cmd = new OracleCommand("P_KORISNIK.Register", DBConnection.con))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add(new OracleParameter("p_username", OracleDbType.Varchar2, ParameterDirection.Input));
				cmd.Parameters.Add(new OracleParameter("p_password", OracleDbType.Varchar2, ParameterDirection.Input));
				cmd.Parameters.Add(new OracleParameter("p_company", OracleDbType.Varchar2, ParameterDirection.Input));
				cmd.Parameters["p_username"].Value = username;
				cmd.Parameters["p_password"].Value = password;
				cmd.Parameters["p_company"].Value = company;

				cmd.ExecuteNonQuery();
			}
		}

		public static void Login(string username, string password)
		{
			using (OracleCommand cmd = new OracleCommand("P_KORISNIK.Login", DBConnection.con))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add(new OracleParameter("p_username", OracleDbType.Varchar2, ParameterDirection.Input));
				cmd.Parameters.Add(new OracleParameter("p_password", OracleDbType.Varchar2, ParameterDirection.Input));
				cmd.Parameters["p_username"].Value = username;
				cmd.Parameters["p_password"].Value = password;

				cmd.ExecuteNonQuery();
			}
		}

		public static void PromeniDetalje(string username, string ime, string prezime, string kompanija)
		{
			using (OracleCommand cmd = new OracleCommand("P_KORISNIK.Promeni_Detalje", DBConnection.con))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add(new OracleParameter("p_user", OracleDbType.Varchar2, ParameterDirection.Input));
				cmd.Parameters.Add(new OracleParameter("p_ime", OracleDbType.Varchar2, ParameterDirection.Input));
				cmd.Parameters.Add(new OracleParameter("p_prezime", OracleDbType.Varchar2, ParameterDirection.Input));
				cmd.Parameters.Add(new OracleParameter("p_company", OracleDbType.Varchar2, ParameterDirection.Input));
				cmd.Parameters["p_user"].Value = username;
				cmd.Parameters["p_ime"].Value = ime;
				cmd.Parameters["p_prezime"].Value = prezime;
				cmd.Parameters["p_company"].Value = kompanija;

				cmd.ExecuteNonQuery();
			}
		}

		public static void PromeniLozinku(string username, string password)
		{
			using (OracleCommand cmd = new OracleCommand("P_KORISNIK.Promeni_Lozinku", DBConnection.con))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add(new OracleParameter("p_username", OracleDbType.Varchar2, ParameterDirection.Input));
				cmd.Parameters.Add(new OracleParameter("p_password", OracleDbType.Varchar2, ParameterDirection.Input));
				cmd.Parameters["p_username"].Value = username;
				cmd.Parameters["p_password"].Value = password;

				cmd.ExecuteNonQuery();
			}
		}

		public static void PromeniJezik(string username, string novi_jezik)
		{
			using (OracleCommand cmd = new OracleCommand("P_KORISNIK.Promeni_Jezik", DBConnection.con))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add(new OracleParameter("p_user", OracleDbType.Varchar2, ParameterDirection.Input));
				cmd.Parameters.Add(new OracleParameter("p_jezik", OracleDbType.Varchar2, ParameterDirection.Input));
				cmd.Parameters["p_user"].Value = username;
				cmd.Parameters["p_jezik"].Value = novi_jezik;

				cmd.ExecuteNonQuery();
			}
		}

		public static void Ucitaj_Podatke()
		{
			using (OracleCommand cmd = new OracleCommand("P_KORISNIK.Ucitaj", DBConnection.con))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add(new OracleParameter("podaci", OracleDbType.RefCursor, ParameterDirection.ReturnValue));
				cmd.Parameters.Add(new OracleParameter("p_user", OracleDbType.Varchar2, ParameterDirection.Input));
				cmd.Parameters["p_user"].Value = Korisnik.KorisnickoIme;

				OracleDataReader dr = cmd.ExecuteReader();

				while (dr.Read())
				{
					Korisnik.Ime = dr.IsDBNull(0) ? null : dr.GetString(0);
					Korisnik.Prezime = dr.IsDBNull(1) ? null : dr.GetString(1);
					Korisnik.Kompanija = dr.IsDBNull(2) ? null : dr.GetString(2);
				}

				dr.Close();
			}
		}

		public static bool Postoji(string username)
		{
			using (OracleCommand cmd = new OracleCommand("P_KORISNIK.Postoji", DBConnection.con))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add(new OracleParameter("f_postoji", OracleDbType.Decimal, ParameterDirection.ReturnValue));
				cmd.Parameters.Add(new OracleParameter("p_user", OracleDbType.Varchar2, ParameterDirection.Input));
				cmd.Parameters["p_user"].Value = username;

				cmd.ExecuteNonQuery();

				int count = int.Parse(cmd.Parameters["f_postoji"].Value.ToString());

				return count != 0;
			}
		}
	}

	
}
