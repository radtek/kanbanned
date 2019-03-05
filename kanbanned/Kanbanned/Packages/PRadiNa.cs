using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace Kanbanned.Packages
{
	public class PRadiNa
	{
		public static void DodajVezu(string korisnik, int projekat, string uloga)
		{
			using (OracleCommand cmd = new OracleCommand("P_RADI_NA.Dodaj_Vezu", DBConnection.con))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add(new OracleParameter("p_korisnik", OracleDbType.Varchar2, ParameterDirection.Input));
				cmd.Parameters.Add(new OracleParameter("p_projekat", OracleDbType.Decimal, ParameterDirection.Input));
				cmd.Parameters.Add(new OracleParameter("p_uloga", OracleDbType.Varchar2, ParameterDirection.Input));
				cmd.Parameters["p_korisnik"].Value = korisnik;
				cmd.Parameters["p_projekat"].Value = projekat;
				cmd.Parameters["p_uloga"].Value = uloga.ToUpper();

				cmd.ExecuteNonQuery();
			}
		}

		public static void IzmeniUlogu(string korisnik, int projekat, string nova_uloga)
		{
			using (OracleCommand cmd = new OracleCommand("P_RADI_NA.Izmeni_Ulogu", DBConnection.con))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add(new OracleParameter("p_korisnik", OracleDbType.Varchar2, ParameterDirection.Input));
				cmd.Parameters.Add(new OracleParameter("p_projekat", OracleDbType.Decimal, ParameterDirection.Input));
				cmd.Parameters.Add(new OracleParameter("p_new_uloga", OracleDbType.Varchar2, ParameterDirection.Input));
				cmd.Parameters["p_korisnik"].Value = korisnik;
				cmd.Parameters["p_projekat"].Value = projekat;
				cmd.Parameters["p_new_uloga"].Value = nova_uloga;

				cmd.ExecuteNonQuery();
			}
		}

		public static void ObrisiVezu(string korisnik, int projekat)
		{
			using (OracleCommand cmd = new OracleCommand("P_RADI_NA.Obrisi_Vezu", DBConnection.con))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add(new OracleParameter("p_korisnik", OracleDbType.Varchar2, ParameterDirection.Input));
				cmd.Parameters.Add(new OracleParameter("p_projekat", OracleDbType.Decimal, ParameterDirection.Input));
				cmd.Parameters["p_korisnik"].Value = korisnik;
				cmd.Parameters["p_projekat"].Value = projekat;

				cmd.ExecuteNonQuery();
			}
		}

		public static int VratiBrojKorisnika(int id_projekta)
		{
			using (OracleCommand cmd = new OracleCommand("P_RADI_NA.Vrati_Br_Korisnika", DBConnection.con))
			{
				cmd.CommandType = CommandType.StoredProcedure;


				cmd.Parameters.Add(new OracleParameter("br_korisnika", OracleDbType.Decimal, ParameterDirection.ReturnValue));
				cmd.Parameters.Add(new OracleParameter("p_id", OracleDbType.Decimal, ParameterDirection.Input));
				cmd.Parameters["p_id"].Value = id_projekta;

				cmd.ExecuteNonQuery();

				return (int)cmd.Parameters["br_korisnika"].Value;
			}
		}
	}
}
