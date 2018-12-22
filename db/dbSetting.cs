using MiniSDN.ui;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Windows;

namespace MiniSDN.db
{

    public class DbSetting  
	{

        public static string DATABASENAME = "db";
        public static string DATABASEPASSWORD = "_12_LG1705504004";
        public static string ConnectionString = ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + DATABASENAME + ".accdb;Jet OLEDB:Database Password=" + DATABASEPASSWORD + ";";

	}

    /// <summary>
    ///  NOT PUBLIC
    /// </summary>
    class ConnectDb
    {
        public static string Connect(string DATABASENAME, string DATABASEPASSWORD)
        {
            string ConnectionString = ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + DATABASENAME + ".accdb;Jet OLEDB:Database Password=" + DATABASEPASSWORD + ";";
            return ConnectionString;
        }
    }

}
