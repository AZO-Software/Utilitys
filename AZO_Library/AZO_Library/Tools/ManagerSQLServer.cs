using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace AZO_Library.Tools
{
    public class ManagerSQLServer
    {
        private SqlConnection connection = new SqlConnection();
        private SqlCommand command;
        private SqlDataAdapter adapter;
        private SqlTransaction transaction;
        //esta variable indica si hay una transaccion en curso o no
        private bool beginTransaction;

        //stringConexion = System.Configuration.ConfigurationManager.ConnectionStrings["sqlServer"].ConnectionString;

        protected void SetConnectionString(String strConnection)
        {
            this.connection.ConnectionString = strConnection;

            command = new SqlCommand();
            command.Connection = connection;
        }

        protected bool AddQuery(String query)
        {
            try
            {
                //este if evita que se cierre la transaccion en curso(cuando hay)
                if (!beginTransaction)
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                    connection.Open();
                }

                command.Parameters.Clear();
                command.CommandType = System.Data.CommandType.Text;
                command.CommandText = query;
                return true;
            }
            catch (SqlException ex)
            {
                Tools.ManagerExceptions.writeToLog("ManagerSQLServer", "AddQuery(String)", ex);
                return false;
            }
        }

        protected bool AddStoredProcedure(String storedProcedure)
        {
            try
            {
                if (!beginTransaction)
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                    connection.Open();
                }

                command.Parameters.Clear();
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.CommandText = storedProcedure;
                return true;
            }
            catch (SqlException ex)
            {
                Tools.ManagerExceptions.writeToLog("ManagerSQLServer", "AddStoredProcedure(String)", ex);
                return false;
            }
        }

        protected void AddParameter(String parameter, Object value)
        {
            command.Parameters.AddWithValue(parameter, value);
        }

        protected int ExecuteQuery(bool beginTransaction)
        {
            try
            {
                this.beginTransaction = beginTransaction;
                if(beginTransaction)
                {
                    BeginTransaction();
                }

                return command.ExecuteNonQuery();
            }
            catch(Exception ex)
            {
                if(beginTransaction)
                {
                    beginTransaction = false;
                    transaction.Rollback();
                }
                Tools.ManagerExceptions.writeToLog("ManagerSQLServer", "ExecuteQuery(bool)", ex);
                return -1;
            }
        }

        protected SqlDataReader GetReader(bool beginTransaction)
        {
            try
            {
                this.beginTransaction = beginTransaction;
                if (beginTransaction)
                {
                    BeginTransaction();
                }

                return command.ExecuteReader();
            }
            catch (Exception ex)
            {
                if (beginTransaction)
                {
                    beginTransaction = false;
                    transaction.Rollback();
                }
                Tools.ManagerExceptions.writeToLog("ManagerSQLServer", "GetReader(bool)", ex);
                return null;
            }
        }

        protected DataTable GetTable()
        {
            try
            {
                if (connection.State == ConnectionState.Open)
                {
                    DataSet ds = new DataSet();
                    adapter = new SqlDataAdapter(command);
                    adapter.Fill(ds, "VALSEC");
                    return ds.Tables[0];
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Tools.ManagerExceptions.writeToLog("ManagerSQLServer", "GetTable", ex);
                return null;
            }
        }

        /// <summary>
        /// Regresa una tabla en especifico de las obtenidas de la consulta
        /// </summary>
        /// <param name="tableNumber"></param>
        /// <returns></returns>
        protected DataTable GetTable(byte tableNumber)
        {
            try
            {
                if (connection.State == ConnectionState.Open)
                {
                    DataSet ds = new DataSet();
                    adapter = new SqlDataAdapter(command);
                    adapter.Fill(ds, "VALSEC");
                    return ds.Tables[tableNumber];
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Tools.ManagerExceptions.writeToLog("ManagerSQLServer", "GetTable(byte)", ex);
                return null;
            }
        }

        /// <summary>
        /// El campo del valor de retorno siempre debe llamarse returnValue
        /// </summary>
        /// <returns></returns>
        protected object ExecuteAndGetReturnValue()
        {
            try
            {
                object returnValue = null;

                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    returnValue = reader["returnValue"];
                }
                reader.Close();
                return returnValue;
            }
            catch(Exception ex)
            {
                Tools.ManagerExceptions.writeToLog("ManagerSQLServer", "ExecuteAndGetReturnValue", ex);
                return null;
            }
        }

        protected void BeginTransaction()
        {
            try
            {
                transaction = connection.BeginTransaction();
                command.Transaction = transaction;
                beginTransaction = true;
            }
            catch (Exception ex)
            {
                command.Transaction = null;
                beginTransaction = false;
                Tools.ManagerExceptions.writeToLog("ManagerSQLServer", "BeginTransaction", ex);
            }
        }

        protected void EndTransaction()
        {
            try
            {
                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Tools.ManagerExceptions.writeToLog("ManagerSQLServer", "EndTransaction", ex);
            }
            beginTransaction = false;
        }

        private Dictionary<string, string> GenDictionary(SqlDataReader reader)
        {
            try
            {
                Dictionary<string, string> dictionary = null;
                if (reader.Read())
                {
                    dictionary = new Dictionary<string, string>();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        dictionary[reader.GetName(i)] = reader[i].ToString();
                    }
                }
                reader.Close();
                return dictionary;
            }
            catch(Exception ex)
            {
                Tools.ManagerExceptions.writeToLog("ManagerSQLServer", "GenDictionary(SqlDataReader)", ex);
                return null;
            }
        }
    }
}
