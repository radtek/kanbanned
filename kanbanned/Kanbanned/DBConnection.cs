using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Oracle.ManagedDataAccess.Client;

namespace Kanbanned
{
	public class DBConnection
	{
		public static OracleConnection con;
		public static string conStr = "User Id=kanbanned; Password=s18; Data Source=92.60.227.37:1521/xe;";
		//public static string Jezik { get; set; }

		public static void Open()
		{
			conStr += "Statement Cache Size=1";
			DBConnection.con = new OracleConnection(conStr);
			con.Open();
		}

		public static void Close()
		{
            con.Close();
		}
	}
}
