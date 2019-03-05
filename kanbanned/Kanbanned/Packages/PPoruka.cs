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
	public class PPoruka
	{
		public static string VratiPrevod(string id)
		{
			using (OracleCommand cmd = new OracleCommand("P_PORUKA.Vrati_Prevod", DBConnection.con))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add(new OracleParameter("opis_poruke", OracleDbType.Varchar2, ParameterDirection.ReturnValue));
				cmd.Parameters.Add(new OracleParameter("p_id", OracleDbType.Varchar2, ParameterDirection.Input));
				cmd.Parameters.Add(new OracleParameter("p_jezik", OracleDbType.Varchar2, ParameterDirection.Input));
				cmd.Parameters["opis_poruke"].Size = 500;
				cmd.Parameters["p_id"].Value = id;
				cmd.Parameters["p_jezik"].Value = Globals.Jezik;

				cmd.ExecuteNonQuery();

				return cmd.Parameters["opis_poruke"].Value.ToString();
			}
		}
	}
}
