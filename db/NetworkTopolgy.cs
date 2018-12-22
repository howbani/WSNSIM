using MiniSDN.Dataplane;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;

namespace MiniSDN.db
{
    public class NetworkTopolgy
    {

        //create table name: network name
        public bool createNewTopology(string NetworkName) 
        {
            bool re = false;

            OleDbConnection con = new OleDbConnection(DbSetting.ConnectionString);
            con.Open();
            try
            {
                DataTable dataTable = con.GetOleDbSchemaTable(OleDbSchemaGuid.Tables,
                    new object[] { null, null, NetworkName, "TABLE" });
                if (dataTable.Rows.Count > 0)
                {
                    re = true;// true. the table is created BEFORE!
                }
                else
                {
                    // Dim cmd As New OleDb.OleDbCommand(, con)
                    OleDbCommand cmd = new OleDbCommand("CREATE TABLE [" + NetworkName + "] " +
                        "([ID] counter," +
                        "[NodeID] Text(10) PRIMARY KEY," +
                        "[Pox] Currency," +
                        "[Poy] Currency," +
                        "[R] Currency" +
                         ")", con);
                    if (cmd.ExecuteNonQuery() > 0) 
                        re = true;
                    con.Close();
                    
                }
            }
            catch { con.Close(); }
            return re;

        }


       public bool SaveSensor( Sensor sensor,string TopologName)
        {
            string[] cols = new string[]
            { 
                    "[NodeID]",
                    "[Pox]",
                    "[Poy]",
                    "[R]"

            };
            string[] vals = new string[] 
            { 
                sensor.ID.ToString(),
                sensor.Position.X.ToString(),
                sensor.Position.Y.ToString(),
                sensor.VisualizedRadius.ToString()
            };
            int re = 0;
            OleDbConnection connection = null;
            OleDbCommand command = null;
            try
            {

                connection = new OleDbConnection();
                connection.ConnectionString = DbSetting.ConnectionString;
                connection.Open();
                string sql = SqlOperations.InsertInTo.Insert(TopologName, cols, vals);
                command = new OleDbCommand();
                command.Connection = connection;
                command.CommandText = sql;
                re = command.ExecuteNonQuery();
            }
            catch { }
            finally
            {
                if (connection != null) { connection.Close(); connection.Dispose(); }
                if (command != null) { command.Dispose(); }
            }

            return re > 0;
        }

       /// <summary>
       /// get the names of networks.
       /// </summary>
       /// <returns></returns>
       public static List<NetwokImport>  ImportNetworkNames(UiImportTopology ui)
       {
           List<NetwokImport> networks = new List<NetwokImport>();
           List<string> tables = DatabaseManager.LoadTables(DbSetting.DATABASENAME, DbSetting.DATABASEPASSWORD);
           // all tables.
           int i=1;
           foreach (string table in tables)
           {
               NetwokImport net = new NetwokImport();
               net.UiImportTopology = ui;
               net.lbl_id.Content = i++;
               net.lbl_network_name.Content = table;
               networks.Add(net);
           }
           return networks;
       }

        public static List<NetwokImport> ImportNetworkNames()
        {
            List<NetwokImport> networks = new List<NetwokImport>();
            List<string> tables = DatabaseManager.LoadTables(DbSetting.DATABASENAME, DbSetting.DATABASEPASSWORD);
            // all tables.
            int i = 1;
            foreach (string table in tables)
            {
                NetwokImport net = new NetwokImport();
                net.lbl_id.Content = i++;
                net.lbl_network_name.Content = table;
                networks.Add(net);
            }
            return networks;
        }


        public static List<string> ImportNetworkNamesAsStrings() 
        {
            List<string> networks = new List<string>();
            List<string> tables = DatabaseManager.LoadTables(DbSetting.DATABASENAME, DbSetting.DATABASEPASSWORD);
            // all tables.
#pragma warning disable CS0219 // The variable 'i' is assigned but its value is never used
            int i = 1;
#pragma warning restore CS0219 // The variable 'i' is assigned but its value is never used
            foreach (string table in tables)
            {
                networks.Add(table);
            }
            return networks;
        }


        public static void ImportNetwok(NetwokImport Netwok)
        {
            string tabelName = Netwok.lbl_network_name.Content.ToString();
            OleDbConnection oldb_connectionProvidor = null;
            OleDbCommand command = null;
            OleDbDataReader Dr = null;

            try
            {
                oldb_connectionProvidor = new OleDbConnection();
                oldb_connectionProvidor.ConnectionString = DbSetting.ConnectionString;
                oldb_connectionProvidor.Open();
                string txt = SqlOperations.Select.AllWithNoCondition(tabelName);

                command = new OleDbCommand(txt, oldb_connectionProvidor);
                Dr = command.ExecuteReader();
                while (Dr.Read())
                {

                    ImportedSensor sensor = new ImportedSensor();
                    sensor.NodeID = Convert.ToInt16(Dr["NodeID"]);
                    sensor.Pox = Convert.ToDouble(Dr["Pox"]);
                    sensor.Poy = Convert.ToDouble(Dr["Poy"]);
                    sensor.R = Convert.ToDouble(Dr["R"]);
                    Netwok.ImportedSensorSensors.Add(sensor);
                }
            }
            catch { }
            finally
            {
                if (oldb_connectionProvidor != null) { oldb_connectionProvidor.Close(); oldb_connectionProvidor.Dispose(); }
                if (command != null) { command.Dispose(); }
                if (Dr != null) { Dr.Close(); Dr.Dispose(); }

            }
        }


        /// <summary>
        /// import table.
        /// </summary>
        /// <param name="NetwokName"></param>
        public static List<ImportedSensor> ImportNetwok(string NetwokName)
        {
            string tabelName = NetwokName;
            OleDbConnection oldb_connectionProvidor = null;
            OleDbCommand command = null;
            OleDbDataReader Dr = null;
            List<ImportedSensor> ImportedSensorSensors = new List<ImportedSensor>();
            try
            {
                oldb_connectionProvidor = new OleDbConnection();
                oldb_connectionProvidor.ConnectionString = DbSetting.ConnectionString;
                oldb_connectionProvidor.Open();
                string txt = SqlOperations.Select.AllWithNoCondition(tabelName);

                command = new OleDbCommand(txt, oldb_connectionProvidor);
                Dr = command.ExecuteReader();
                while (Dr.Read())
                {

                    ImportedSensor sensor = new ImportedSensor();
                    sensor.NodeID = Convert.ToInt16(Dr["NodeID"]);
                    sensor.Pox = Convert.ToDouble(Dr["Pox"]);
                    sensor.Poy = Convert.ToDouble(Dr["Poy"]);
                    sensor.R = Convert.ToDouble(Dr["R"]);

                    ImportedSensorSensors.Add(sensor);
                }
            }
            catch { }
            finally
            {
                if (oldb_connectionProvidor != null) { oldb_connectionProvidor.Close(); oldb_connectionProvidor.Dispose(); }
                if (command != null) { command.Dispose(); }
                if (Dr != null) { Dr.Close(); Dr.Dispose(); }
            }
            return ImportedSensorSensors;
        }






    }
}
