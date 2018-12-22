using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Data.Odbc;

namespace MiniSDN.db
{
    /// <summary>
    /// AMMAR HOWBANI 
    /// PARROT SERVER 0.08
    /// data base and ADO.NET
    /// </summary>
    [Serializable]
   public class DatabaseManager
    {
        private string _database;
        // the decleration for providors.
        static private string ConnectionString;
        private string _password = ""; 
        public DatabaseManager(string db, string password) 
        {
            _database = db;
            _password = password;
            ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + db + ".accdb;Jet OLEDB:Database Password=" + password + ";";
       }

       /// <summary>
       /// inset.
       /// </summary>
        /// <param name="UserSqlCommnad">string UserSqlCommnad</param>
        /// <returns>OleDbDataReader</returns>
        public OleDbDataReader ExecuteReader(string UserSqlCommnad)
        {
            OleDbConnection oldb_connectionProvidor = null;
            OleDbDataReader oldb_dataReader = null;
            OleDbCommand oldb_commandProvidor = null;
            try
            {
                oldb_connectionProvidor = new OleDbConnection();
                oldb_connectionProvidor.ConnectionString = ConnectionString;
                oldb_connectionProvidor.Open();
                oldb_commandProvidor = new OleDbCommand(UserSqlCommnad, oldb_connectionProvidor);
                oldb_dataReader = oldb_commandProvidor.ExecuteReader();
            }
            catch
            {
                
                
               
            }
            finally 
            {
                //if (oldb_connectionProvidor != null)
                //{
                //    oldb_connectionProvidor.Close();
                //    oldb_connectionProvidor.Dispose();

                //}
            }
            return oldb_dataReader;
        }

        /// <summary>
        /// for insert , deelete abd update
        /// </summary>
        /// <param name="UserSqlCommnad"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(string UserSqlCommnad)
        {
            OleDbTransaction transaction = null;
            OleDbConnection oldb_connectionProvidor = null;
            OleDbCommand oldb_commandProvidor = null;
            int re = 0;
            try
            {
                oldb_connectionProvidor = new OleDbConnection();
                oldb_connectionProvidor.ConnectionString = ConnectionString;
                oldb_connectionProvidor.Open();
                transaction = oldb_connectionProvidor.BeginTransaction();
                oldb_commandProvidor = new OleDbCommand(UserSqlCommnad, oldb_connectionProvidor);
                oldb_commandProvidor.Transaction = transaction;
                re = oldb_commandProvidor.ExecuteNonQuery();
                transaction.Commit();
            }
            catch 
            {
                re = 0;
                transaction.Rollback();
            }
            finally
            {
                if (oldb_connectionProvidor != null)
                {
                    oldb_connectionProvidor.Close();
                    oldb_connectionProvidor.Dispose();

                }
                if (oldb_commandProvidor != null)
                {
                    oldb_commandProvidor.Dispose();
                }

            }
            return re;
        }

       /// <summary>
       /// return true if the table is found , false if no such table name
       /// </summary>
       /// <param name="dbName"></param>
       /// <param name="dbPassword"></param>
       /// <param name="TableName"></param>
       /// <returns></returns>
        public static bool IsFoundTable(string dbName, string dbPassword, string TableName) 
        {
            bool re = false;
            OleDbConnection con = null;
            DataTable dataTable = null;
            try
            {
                string cs = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + dbName + ".accdb;Jet OLEDB:Database Password=" + dbPassword + ";";
                con = new OleDbConnection(cs);
                con.Open();
                dataTable = con.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, TableName, "TABLE" });
                if (dataTable.Rows.Count > 0)
                {
                    re = true;
                }

            }
            catch
            {
                //msg.Error(exp.Message);
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                    con.Dispose();
                }
                if(dataTable!=null)
                {
                    dataTable.Dispose();
                    dataTable.Clear();
                }


            }
            return re;
        }

        /// <summary>
        /// load the all tables name form DB.
        /// </summary>
        /// <param name="dbName"></param>
        /// <param name="dbPassword"></param>
        /// <returns></returns>
        public static List<string> LoadTables(string dbName, string dbPassword) 
        {
            OleDbConnection con = null;
            DataTable dataTable = null;

            List<string> re = new List<string>();
            try
            {
                con = new OleDbConnection();
                string cs = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + dbName + ".accdb;Jet OLEDB:Database Password=" + dbPassword + ";";
                con.ConnectionString = cs;
                con.Open();
                dataTable = con.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
                if (dataTable.Rows.Count > 0)
                {
                    foreach (DataRow row in dataTable.Rows)
                    {
                        re.Add(row[2].ToString());
                    }
                }

            }
            catch { con.Close(); }

            finally
            {
                if (con != null)
                {
                    con.Close();
                    con.Dispose();
                }
                if (dataTable != null)
                {
                    dataTable.Dispose();
                    dataTable.Clear();
                }
            }
            return re;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public  string[] getCols(string table)
        {
            OleDbDataReader dr = null;
            string[] re1 = null;
            try
            {
                dr = this.ExecuteReader(SqlOperations.Select.AllWithNoCondition(table));
                string[] re = new string[dr.FieldCount];
                re1 = new string[dr.FieldCount - 1];
                for (int i = dr.FieldCount - 1; i > 0; i--) // i>0 no need for ID
                {
                    re[i] = dr.GetName(i);
                }

                for (int i = 1; i < re.Length; i++)
                {
                    re1[i - 1] = re[i];
                }
            }
#pragma warning disable CS0168 // The variable 'exp' is declared but never used
            catch (Exception exp)
#pragma warning restore CS0168 // The variable 'exp' is declared but never used
            {
               

            }
            finally
            {
                if (dr != null)
                {
                    //dr.Close();
                    //dr.Dispose();
                }

            }
            return re1;
        }

    }
}
