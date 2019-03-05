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
	public class PPromena
	{
		public enum TipIstorije
		{
			Faze = 0,
			Zadaci = 1
		}

		public static List<string> VratiIstoriju(TipIstorije tip, int br_promena, int id_projekta)
		{
			string funkcija = "P_PROMENA.";

			switch (tip)
			{
				case TipIstorije.Faze:
					funkcija += "Vrati_Istoriju_Faza";
					break;
				case TipIstorije.Zadaci:
					funkcija += "Vrati_Istoriju_Zadataka";
					break;
			}

			using (OracleCommand cmd = new OracleCommand(funkcija, DBConnection.con))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add(new OracleParameter("istorija", OracleDbType.Varchar2, ParameterDirection.ReturnValue));
				cmd.Parameters["istorija"].CollectionType = OracleCollectionType.PLSQLAssociativeArray;
				cmd.Parameters["istorija"].Size = br_promena;
				cmd.Parameters["istorija"].ArrayBindSize = new int[br_promena];
				for (int i = 0; i <= br_promena - 1; i++)
					cmd.Parameters["istorija"].ArrayBindSize[i] = 4000;

				cmd.Parameters.Add(new OracleParameter("p_n", OracleDbType.Decimal, ParameterDirection.Input));
				cmd.Parameters.Add(new OracleParameter("p_jezik", OracleDbType.Varchar2, ParameterDirection.Input));
				cmd.Parameters.Add(new OracleParameter("p_proj", OracleDbType.Decimal, ParameterDirection.Input));

				cmd.Parameters["p_n"].Value = br_promena;
				cmd.Parameters["p_jezik"].Value = Globals.Jezik;
				cmd.Parameters["p_proj"].Value = id_projekta;

				cmd.ExecuteNonQuery();

                List<string> promene = new List<string>();
                br_promena = ((OracleString[])cmd.Parameters["istorija"].Value).Count();

                for (int i = 0; i <= br_promena - 1; i++)
                    promene.Add(((OracleString[])cmd.Parameters["istorija"].Value)[i].ToString());

                return promene;
			}
		}

		public static int ProveriPromene(DateTime prosla_provera, int id_projekta)
		{
			using (OracleCommand cmd = new OracleCommand("P_PROMENA.Proveri_Promene", DBConnection.con))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add(new OracleParameter("br_promena", OracleDbType.Decimal, ParameterDirection.ReturnValue));
				cmd.Parameters.Add(new OracleParameter("vreme_prosle_provere", OracleDbType.Date, ParameterDirection.Input));
				cmd.Parameters.Add(new OracleParameter("id_proj", OracleDbType.Decimal, ParameterDirection.Input));
				cmd.Parameters["vreme_prosle_provere"].Value = prosla_provera;
				cmd.Parameters["id_proj"].Value = id_projekta;

				cmd.ExecuteNonQuery();

				return int.Parse(cmd.Parameters["id_proj"].Value.ToString());
			}
		}
	}
}
