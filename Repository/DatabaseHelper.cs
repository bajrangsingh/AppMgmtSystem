//====================================================================
// Copyright (C) 2005 BinaryIntellect Consulting. All rights reserved.
// Visit us at www.binaryintellect.com
//====================================================================

using System;
using System.Data;
using System.Configuration;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Data.Odbc;
using System.IO;

namespace ApprovalPortal.Repository
{
    public class DatabaseHelper : IDisposable
    {
        private string strConnectionString;
        private DbConnection objConnection;
        private DbCommand objCommand;
        private DbProviderFactory objFactory = null;
        private bool boolHandleErrors;
        private string strLastError;
        private bool boolLogError;
        private string strLogFile;
        private int intLastInsertedID;
        public int IntLastInsertedID
        {
            get { return intLastInsertedID; }
            set { intLastInsertedID = value; }
        }
        public DatabaseHelper(string connectionstring, Providers provider)
        {
            strConnectionString = connectionstring;
            switch (provider)
            {
                case Providers.SqlServer:
                    objFactory = SqlClientFactory.Instance;
                    break;
                case Providers.OleDb:
                    objFactory = OleDbFactory.Instance;
                    break;
                case Providers.ODBC:
                    objFactory = OdbcFactory.Instance;
                    break;
                case Providers.Oracle:
                    // objFactory = OracleClientFactory.Instance;
                    break;
                case Providers.ConfigDefined:
                    string providername = ConfigurationManager.ConnectionStrings["connectionstring"].ProviderName;
                    switch (providername)
                    {
                        case "System.Data.SqlClient":
                            objFactory = SqlClientFactory.Instance;
                            break;
                        case "System.Data.OleDb":
                            objFactory = OleDbFactory.Instance;
                            break;
                        case "System.Data.Odbc":
                            objFactory = OdbcFactory.Instance;
                            break;
                        case "System.Data.OracleClient":
                            //objFactory = OracleClientFactory.Instance;
                            break;
                    }
                    break;
            }

            objConnection = objFactory.CreateConnection();
            objCommand = objFactory.CreateCommand();

            objConnection.ConnectionString = strConnectionString;
            objCommand.Connection = objConnection;
        }
        public DatabaseHelper(Providers provider)
            : this(ConfigurationManager.ConnectionStrings["connectionstring"].ConnectionString, provider)
        { }
        public DatabaseHelper(string connectionstring)
            : this(connectionstring, Providers.SqlServer)
        { }
        public DatabaseHelper()
            : this(ConfigurationManager.ConnectionStrings["connectionstring"].ConnectionString, Providers.ConfigDefined)
        { }
        public bool HandleErrors
        {
            get
            {
                return boolHandleErrors;
            }
            set
            {
                boolHandleErrors = value;
            }
        }
        public string LastError
        {
            get
            {
                return strLastError;
            }
        }
        public bool LogErrors
        {
            get
            {
                return boolLogError;
            }
            set
            {
                boolLogError = value;
            }
        }
        public string LogFile
        {
            get
            {
                return strLogFile;
            }
            set
            {
                strLogFile = value;
            }
        }
        public int AddParameter(string name, object value)
        {
            DbParameter p = objFactory.CreateParameter();
            p.ParameterName = name;
            p.Value = value;
            return objCommand.Parameters.Add(p);
            //return AddParameter(name, value, ParameterDirection.Input,DbType.Int32);
        }
        public int AddParameter(string name, object value, DbType DBDataType)
        {
            DbParameter p = objFactory.CreateParameter();
            p.ParameterName = name;
            p.Value = value;
            return objCommand.Parameters.Add(p);
            //return AddParameter(name, value, ParameterDirection.Input,DBDataType);  
        }
        public int AddParameter(string name, object value, ParameterDirection direction, DbType DBDataType)
        {
            DbParameter p = objFactory.CreateParameter();
            p.ParameterName = name;
            if (direction == ParameterDirection.Output)
            {
                p.DbType = DBDataType;
            }
            else
            {
                p.Value = value;
            }
            p.Direction = direction;
            return objCommand.Parameters.Add(p);
        }
        // overload to return parameter value
        public DbParameter AddParameter(string name, ParameterDirection direction, DbType DBDataType)
        {
            DbParameter p = objFactory.CreateParameter();
            p.ParameterName = name;
            if (direction == ParameterDirection.Output)
            {
                p.DbType = DBDataType;
            }
            p.Direction = direction;
            objCommand.Parameters.Add(p);
            return p;
        }
        public int AddParameter(DbParameter parameter)
        {
            return objCommand.Parameters.Add(parameter);
        }
        public int ExecuteAction(string sql, SqlParameter[] p)
        {
            SqlConnection cnn = new SqlConnection(strConnectionString);
            cnn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cnn;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = sql;
            for (int i = 0; i < p.Length; i++)
            {
                cmd.Parameters.Add(p[i]);
            }
            int j = cmd.ExecuteNonQuery();
            cnn.Close();
            return j;
        }
        public DbCommand Command
        {
            get
            {
                return objCommand;
            }
        }
        public void BeginTransaction()
        {
            if (objConnection.State == System.Data.ConnectionState.Closed)
            {
                objConnection.Open();
            }
            objCommand.Transaction = objConnection.BeginTransaction();
        }
        public void CommitTransaction()
        {
            objCommand.Transaction.Commit();
            objConnection.Close();
        }
        public void RollbackTransaction()
        {
            objCommand.Transaction.Rollback();
            objConnection.Close();
        }
        public int ExecuteNonQuery(string query)
        {
            return ExecuteNonQuery(query, CommandType.Text, ConnectionState.CloseOnExit);
        }
        public int ExecuteNonQuery(string query, CommandType commandtype)
        {
            return ExecuteNonQuery(query, commandtype, ConnectionState.CloseOnExit);
        }
        public int ExecuteNonQuery(string query, ConnectionState connectionstate)
        {
            return ExecuteNonQuery(query, CommandType.Text, connectionstate);
        }
        public int ExecuteNonQuery(string query, CommandType commandtype, ConnectionState connectionstate)
        {
            objCommand.CommandText = query;
            objCommand.CommandType = commandtype;
            int i = -1;
            try
            {
                if (objConnection.State == System.Data.ConnectionState.Closed)
                {
                    objConnection.Open();
                }
                i = objCommand.ExecuteNonQuery();
                intLastInsertedID = -1;
            }
            catch (Exception ex)
            {
                HandleExceptions(ex);
            }
            finally
            {
                objCommand.Parameters.Clear();
                if (connectionstate == ConnectionState.CloseOnExit)
                {
                    objConnection.Close();
                }
            }

            return i;
        }
        public object ExecuteScalar(string query)
        {
            return ExecuteScalar(query, CommandType.Text, ConnectionState.CloseOnExit);
        }
        public object ExecuteScalar(string query, CommandType commandtype)
        {
            return ExecuteScalar(query, commandtype, ConnectionState.CloseOnExit);
        }
        public object ExecuteScalar(string query, ConnectionState connectionstate)
        {
            return ExecuteScalar(query, CommandType.Text, connectionstate);
        }
        public object ExecuteScalar(string query, CommandType commandtype, ConnectionState connectionstate)
        {
            objCommand.CommandText = query;
            objCommand.CommandType = commandtype;
            object o = null;
            try
            {
                if (objConnection.State == System.Data.ConnectionState.Closed)
                {
                    objConnection.Open();
                }
                o = objCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {
                HandleExceptions(ex);
            }
            finally
            {
                objCommand.Parameters.Clear();
                if (connectionstate == ConnectionState.CloseOnExit)
                {
                    objConnection.Close();
                }
            }

            return o;
        }
        public DbDataReader ExecuteReader(string query)
        {
            return ExecuteReader(query, CommandType.Text, ConnectionState.CloseOnExit);
        }
        public DbDataReader ExecuteReader(string query, CommandType commandtype)
        {
            return ExecuteReader(query, commandtype, ConnectionState.CloseOnExit);
        }
        public DbDataReader ExecuteReader(string query, ConnectionState connectionstate)
        {
            return ExecuteReader(query, CommandType.Text, connectionstate);
        }
        public DbDataReader ExecuteReader(string query, CommandType commandtype, ConnectionState connectionstate)
        {
            objCommand.CommandText = query;
            objCommand.CommandType = commandtype;
            DbDataReader reader = null;
            try
            {
                if (objConnection.State == System.Data.ConnectionState.Closed)
                {
                    objConnection.Open();
                }
                reader = connectionstate == ConnectionState.CloseOnExit ? objCommand.ExecuteReader(CommandBehavior.CloseConnection) : objCommand.ExecuteReader();
            }
            catch (Exception ex)
            {
                HandleExceptions(ex);
            }
            finally
            {
                objCommand.Parameters.Clear();
            }

            return reader;
        }
        public DataSet ExecuteDataSet(string query)
        {
            return ExecuteDataSet(query, CommandType.Text, ConnectionState.CloseOnExit);
        }
        public DataSet ExecuteDataSet(string query, CommandType commandtype)
        {
            return ExecuteDataSet(query, commandtype, ConnectionState.CloseOnExit);
        }
        public DataSet ExecuteDataSet(string query, ConnectionState connectionstate)
        {
            return ExecuteDataSet(query, CommandType.Text, connectionstate);
        }



        public DataSet ExecuteDataSet(string query, CommandType commandtype, ConnectionState connectionstate)
        {
            DbDataAdapter adapter = objFactory.CreateDataAdapter();
            objCommand.CommandText = query;
            objCommand.CommandType = commandtype;
            adapter.SelectCommand = objCommand;
            objCommand.CommandTimeout = 120;
            DataSet ds = new DataSet();
            try
            {
                adapter.Fill(ds);
            }
            catch (Exception ex)
            {
                HandleExceptions(ex);
            }
            finally
            {
                objCommand.Parameters.Clear();
                if (connectionstate == ConnectionState.CloseOnExit)
                {
                    if (objConnection.State == System.Data.ConnectionState.Open)
                    {
                        objConnection.Close();
                    }
                }
            }
            return ds;
        }
        public void ExecuteDataSet(DataSet ds, string query, String[] MappingTableName, CommandType commandtype, ConnectionState connectionstate)
        {
            DbDataAdapter adapter = objFactory.CreateDataAdapter();
            objCommand.CommandText = query;
            objCommand.CommandType = commandtype;
            adapter.SelectCommand = objCommand;

            for (int i = 0; i < MappingTableName.Length; i++)
            {
                if (i == 0)
                {
                    adapter.TableMappings.Add("Table", MappingTableName[i]);
                }
                else
                {
                    adapter.TableMappings.Add("Table" + i, MappingTableName[i]);
                }
            }

            //DataSet ds = new DataSet();
            try
            {
                adapter.Fill(ds);
            }
            catch (Exception ex)
            {
                HandleExceptions(ex);
            }
            finally
            {
                objCommand.Parameters.Clear();
                if (connectionstate == ConnectionState.CloseOnExit)
                {
                    if (objConnection.State == System.Data.ConnectionState.Open)
                    {
                        objConnection.Close();
                    }
                }
            }
            //return ds;
        }
        private void HandleExceptions(Exception ex)
        {
            if (LogErrors)
            {
                WriteToLog(ex.Message);
            }
            if (HandleErrors)
            {
                strLastError = ex.Message;
            }
            else
            {
                throw ex;
            }
        }
        private void WriteToLog(string msg)
        {
            StreamWriter writer = File.AppendText(LogFile);
            writer.WriteLine(DateTime.Now + " - " + msg);
            writer.Close();
        }
        public void Dispose()
        {
            objConnection.Close();
            objConnection.Dispose();
            objCommand.Dispose();
        }


        public DataTable DatatableToDatatable(DataTable dt, string[] strArray)
        {
            DataTable table = new DataTable();
            foreach (string str in strArray)
            {
                table.Columns.Add(str, System.Type.GetType(dt.Columns[str].DataType.ToString()));

            }

            foreach (DataRow dr in dt.Rows)
            {
                DataRow drnewRow = table.NewRow();
                foreach (DataColumn dc in table.Columns)
                {
                    drnewRow[dc.ColumnName] = dr[dc.ColumnName];
                }
                table.Rows.Add(drnewRow);
            }

            return table;
        }

        // new added by san
        #region IDisposable Members

        void IDisposable.Dispose()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
    public enum Providers
    {
        SqlServer, OleDb, Oracle, ODBC, ConfigDefined
    }
    public enum ConnectionState
    {
        KeepOpen, CloseOnExit
    }




    ////////////////////////
    
}