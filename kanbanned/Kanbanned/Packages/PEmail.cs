using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace Kanbanned.Packages
{
	class PEmail
	{
		public static void Dodaj(string email, string username)
		{
			using (OracleCommand cmd = new OracleCommand("P_EMAIL.Dodaj", DBConnection.con))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add(new OracleParameter("email", OracleDbType.Varchar2, ParameterDirection.Input));
				cmd.Parameters.Add(new OracleParameter("username", OracleDbType.Varchar2, ParameterDirection.Input));
				cmd.Parameters["email"].Value = email;
				cmd.Parameters["username"].Value = username;

				cmd.ExecuteNonQuery();
			}
		}

		public static void Obrisi(string email, string username)
		{
			using (OracleCommand cmd = new OracleCommand("P_EMAIL.Obrisi", DBConnection.con))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add(new OracleParameter("email", OracleDbType.Varchar2, ParameterDirection.Input));
				cmd.Parameters.Add(new OracleParameter("username", OracleDbType.Varchar2, ParameterDirection.Input));
				cmd.Parameters["email"].Value = email;
				cmd.Parameters["username"].Value = username;

				cmd.ExecuteNonQuery();
			}
		}

		public static List<string> VratiSve(string username)
		{
			using (OracleCommand cmd = new OracleCommand("P_EMAIL.Vrati_Sve", DBConnection.con))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add(new OracleParameter("mejlovi", OracleDbType.RefCursor, ParameterDirection.ReturnValue));
				cmd.Parameters.Add(new OracleParameter("username", OracleDbType.Varchar2, ParameterDirection.Input));
				cmd.Parameters["username"].Value = username;

				OracleDataReader dr = cmd.ExecuteReader();

				List<string> mejlovi = new List<string>();

				while (dr.Read())
					mejlovi.Add(dr.GetString(0));

				return mejlovi;
			}
		}
	}
}
