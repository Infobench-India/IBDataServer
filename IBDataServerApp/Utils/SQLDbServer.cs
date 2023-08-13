using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBDataServerApp.Utils
{
    class SQLServerDB
    {
        public static short rowlimit = 50;
        public static string connString = string.Empty;

        //Executing query method directly by passing query in string
        public static void ExecuteQuery(string str)
        {
            SqlConnection myConn = new SqlConnection(connString);
            SqlCommand myCommand = new SqlCommand(str, myConn);
            try
            {
                myConn.Open();
                myCommand.ExecuteNonQuery();
                myConn.Close();
            }
            catch (System.Exception ex)
            {
                Helper.WriteErrorLog(ex);
            }
            finally
            {
                if (myConn.State == ConnectionState.Open)
                {
                    myConn.Close();
                }
            }

        }

        //Checking database exist or not method
        public static bool CheckDatabaseExists(string connectionString, string databaseName)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                using (var command = new SqlCommand(string.Format(
                        "SELECT db_id('{0}')", databaseName), connection))
                {
                    connection.Open();
                    return (command.ExecuteScalar() != DBNull.Value);
                }
            }
        }

        //Creating tables in sql server method
        public static string CreateTableQuery(DataTable table)
        {
            string tableName = table.TableName;
            string sqlsc;
            sqlsc = "CREATE TABLE " + tableName + "(";
            for (int i = 0; i < table.Columns.Count; i++)
            {
                sqlsc += "\n [" + table.Columns[i].ColumnName + "] ";
                string columnType = table.Columns[i].DataType.ToString();
                switch (columnType)
                {
                    case "System.Int32":
                        sqlsc += " int ";
                        break;
                    case "System.Int64":
                        sqlsc += " bigint ";
                        break;
                    case "System.Int16":
                        sqlsc += " smallint";
                        break;
                    case "System.Byte":
                        sqlsc += " tinyint";
                        break;
                    case "System.Decimal":
                        sqlsc += " decimal ";
                        break;
                    case "System.DateTime":
                        sqlsc += " datetime ";
                        break;
                    case "System.String":
                    default:
                        sqlsc += string.Format(" nvarchar({0}) ", table.Columns[i].MaxLength == -1 ? "max" : table.Columns[i].MaxLength.ToString());
                        break;
                }
                if (table.Columns[i].AutoIncrement)
                    sqlsc += " IDENTITY(" + table.Columns[i].AutoIncrementSeed.ToString() + "," + table.Columns[i].AutoIncrementStep.ToString() + ") ";
                if (!table.Columns[i].AllowDBNull)
                    sqlsc += " NOT NULL ";
                sqlsc += ",";
            }
            return sqlsc.Substring(0, sqlsc.Length - 1) + "\n)";
        }

        //Checking table exist or not in sql server
        private static bool TableExist(string tableName)
        {
            bool exists = false;
            string tableQuery = @"select 1 from INFORMATION_SCHEMA.TABLES where TABLE_NAME='{0}'";

            try
            {
                string cmdText = string.Format(tableQuery, tableName);
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(cmdText, conn))
                    {
                        object o = cmd.ExecuteScalar();
                        if (o == null)
                        {
                            exists = false;
                        }
                        else
                        {
                            exists = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return exists;
        }

        //Checking db connection
        public static void connectionTest()
        {
            try
            {
                SqlConnection conn = new SqlConnection(connString);
                conn.Open();

                if (conn.State == ConnectionState.Open)
                {
                    Helper.WriteLogMsg("Database Connected");
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                Helper.WriteErrorLog(ex);
            }
        }

        //Create table method 
        public static void createTables(DataTable LocalDT)
        {
            bool vTableExist = false;
            vTableExist = TableExist(LocalDT.TableName);
            Helper.WriteLogMsg(LocalDT.TableName + " Exist:" + vTableExist.ToString());
            if (vTableExist.ToString() == "False")
            {
                ExecuteQuery(CreateTableQuery(LocalDT));
                Helper.WriteLogMsg(LocalDT.TableName + " Created");
            }
            if (vTableExist.ToString() == "True")
            {
                BulkInsertDataTable(LocalDT.TableName, LocalDT);
            }

        }
        public static bool BulkInsertDataTable(string tableName, DataTable dataTable)
        {
            bool isSuccuss;
            try
            {
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connString))
                {
                    bulkCopy.DestinationTableName = tableName;

                    // Write from the source to the destination.
                    bulkCopy.WriteToServer(dataTable);
                    isSuccuss = true;

                }
            }
            catch (Exception ex)
            {
                isSuccuss = false;
            }
            return isSuccuss;
        }
        public static string SQLType(string TypeName, DataTable table)
        {
            string sqlsc = string.Empty;
            sqlsc = "CREATE TYPE " + TypeName + " AS Table (";
            for (int i = 0; i < table.Columns.Count; i++)
            {
                sqlsc += "\n [" + table.Columns[i].ColumnName + "] ";
                string columnType = table.Columns[i].DataType.ToString();
                Helper.WriteLogMsg(columnType);
                switch (columnType)
                {
                    case "System.Int32":
                        sqlsc += " int ";
                        break;
                    case "System.Int64":
                        sqlsc += " bigint ";
                        break;
                    case "System.Int16":
                        sqlsc += " smallint";
                        break;
                    case "System.Byte":
                        sqlsc += " tinyint";
                        break;
                    case "System.Decimal":
                        sqlsc += " decimal ";
                        break;
                    case "System.DateTime":
                        sqlsc += " datetime ";
                        break;
                    case "System.String":
                        sqlsc += " varchar(max) ";
                        break;
                    default:
                        sqlsc += string.Format(" nvarchar({0}) ", table.Columns[i].MaxLength == -1 ? "max" : table.Columns[i].MaxLength.ToString());
                        break;
                }
                if (table.Columns[i].AutoIncrement)
                    sqlsc += " IDENTITY(" + table.Columns[i].AutoIncrementSeed.ToString() + "," + table.Columns[i].AutoIncrementStep.ToString() + ") ";
                if (!table.Columns[i].AllowDBNull)
                    sqlsc += " NOT NULL ";
                sqlsc += ",";
            }
            return sqlsc; //.Substring(0,sqlsc.Length-1) + "\n)";
        }

        //Insert data method for inserting data in sqlserver
        public void insert_data(DataTable DT)
        {
            try
            {
                if (DT.Rows.Count > 0)
                {
                    string insert = "insert into " + DT.TableName + " Values ";
                    //Code Edited - START			
                    string ValueToInsert = String.Empty;
                    string tempstring = string.Empty;
                    for (int i = 0; i <= DT.Rows.Count - 1; i++)
                    {
                        for (int j = 0; j <= DT.Columns.Count - 1; j++)
                        {
                            if (j == 0) { tempstring = "'" + DT.Rows[i][j].ToString() + "'"; } else { tempstring = tempstring + "," + "'" + DT.Rows[i][j].ToString() + "'"; }
                        } //Enf j
                        if (ValueToInsert == "") { ValueToInsert = "(" + tempstring + ")"; } else { ValueToInsert = ValueToInsert + ",(" + tempstring + ")"; }
                    } //End i
                    insert = insert + ValueToInsert;
                    ExecuteQuery(insert);
                    DT.Clear();
                    insert = "";
                }
            }
            catch (Exception ex)
            {
                Helper.WriteErrorLog(ex);
            }

        }

        public static string GenerateQueryAuditTrail(string TableName)
        {
            SqlConnection connection = null;

            try
            {
                if (!TableExist(TableName))
                {
                    return "select * from AuditLog where TimeStamp > '2000-01-01 00:00:00' ORDER BY TimeStamp ASC limit " + rowlimit + "";
                }
                DataTable dataTble = new DataTable();
                connection = new SqlConnection(connString);
                connection.Open();
                SqlCommand command = new SqlCommand(@"SELECT TOP 1 TimeStamp=CONVERT(varchar, TimeStamp, 121) FROM " + TableName + " ORDER BY TimeStamp DESC ", connection);
                dataTble.Load(command.ExecuteReader());
                connection.Close();
                if (dataTble.Rows.Count > 0)
                    return "select * from AuditLog where TimeStamp > '" + Convert.ToDateTime(dataTble.Rows[0]["TimeStamp"].ToString()).AddMilliseconds(1).ToString("yyyy-MM-dd HH:mm:ss.fff") + "' ORDER BY TimeStamp ASC limit " + rowlimit + "";
                return "select * from AuditLog where TimeStamp > '2000-01-01 00:00:00' ORDER BY TimeStamp ASC limit " + rowlimit + "";

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
            }
        }
        public static string GenerateQueryAlarm(string ServerTableName, string HmiTableName)
        {
            SqlConnection connection = null;

            try
            {
                if (!TableExist(ServerTableName))
                {
                    return "select * from " + HmiTableName + " where State ='Normal' AND NormalTime > '2000-01-01 00:00:00' ORDER BY NormalTime ASC limit " + rowlimit + "";
                }
                DataTable dataTble = new DataTable();
                connection = new SqlConnection(connString);
                connection.Open();
                SqlCommand command = new SqlCommand(@"SELECT TOP 1 NormalTime=CONVERT(varchar, NormalTime, 121) FROM " + ServerTableName + " ORDER BY NormalTime DESC ", connection);
                dataTble.Load(command.ExecuteReader());
                connection.Close();
                if (dataTble.Rows.Count > 0)
                    return "select * from " + HmiTableName + " where State ='Normal' AND NormalTime > '" + Convert.ToDateTime(dataTble.Rows[0]["NormalTime"].ToString()).AddMilliseconds(1).ToString("yyyy-MM-dd HH:mm:ss.fff") + "' ORDER BY NormalTime ASC limit " + rowlimit + "";
                return "select * from " + HmiTableName + " where State ='Normal' AND NormalTime > '2000-01-01 00:00:00' ORDER BY NormalTime ASC limit " + rowlimit + "";

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
            }
        }
        public static string GenerateQueryDataloggers(string ServerTableName, string HMITableName)
        {
            SqlConnection connection = null;

            try
            {
                if (!TableExist(ServerTableName))
                {
                    return "select * from " + HMITableName + " where Time > '2000-01-01 00:00:00' ORDER BY Time ASC limit " + rowlimit + "";
                }
                DataTable dataTble = new DataTable();
                connection = new SqlConnection(connString);
                connection.Open();
                SqlCommand command = new SqlCommand(@"SELECT TOP 1 Time=CONVERT(varchar, Time, 121) FROM " + ServerTableName + " ORDER BY Time DESC ", connection);
                dataTble.Load(command.ExecuteReader());
                connection.Close();
                if (dataTble.Rows.Count > 0)
                    return "select * from " + HMITableName + " where Time > '" + Convert.ToDateTime(dataTble.Rows[0]["Time"].ToString()).AddMilliseconds(1).ToString("yyyy-MM-dd HH:mm:ss.fff") + "' ORDER BY Time ASC limit " + rowlimit + "";
                return "select * from " + HMITableName + " where Time > '2000-01-01 00:00:00' ORDER BY Time ASC limit " + rowlimit + "";

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
            }
        }
    }

}
