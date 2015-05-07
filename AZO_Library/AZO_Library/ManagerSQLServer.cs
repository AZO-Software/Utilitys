using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace AZO_Library
{
    public class ManagerSQLServer
    {
        private SqlConnection connection = new SqlConnection();
        private SqlCommand command;
        private SqlDataAdapter adapter;
        private SqlTransaction transaction;
        private bool beginTransaction;

        //stringConexion = System.Configuration.ConfigurationManager.ConnectionStrings["sqlServer"].ConnectionString;

        protected void SetConnectionString(String strConnection)
        {
            this.connection.ConnectionString = strConnection;

            command = new SqlCommand();
            command.Connection = connection;
        }

        protected bool AddStoredProcedure(String storedProcedure)
        {
            try
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
                connection.Open();

                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.CommandText = storedProcedure;
                return true;
            }
            catch (SqlException ex)
            {
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
            catch(Exception)
            {
                if(beginTransaction)
                {
                    beginTransaction = false;
                    transaction.Rollback();
                }
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
            catch (Exception)
            {
                if (beginTransaction)
                {
                    beginTransaction = false;
                    transaction.Rollback();
                }
                return null;
            }
        }

        protected DataTable GetTable()
        {
            try
            {
                DataSet ds = new DataSet();
                adapter = new SqlDataAdapter(command);
                adapter.Fill(ds, "VALSEC");
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        protected DataTable GetTable(byte tableNumber)
        {
            try
            {
                DataSet ds = new DataSet();
                adapter = new SqlDataAdapter(command);
                adapter.Fill(ds, "VALSEC");
                return ds.Tables[tableNumber];
            }
            catch (Exception ex)
            {
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
            catch (Exception)
            {
                command.Transaction = null;
                beginTransaction = false;
            }
        }

        protected void EndTransaction()
        {
            try
            {
                transaction.Commit();
                beginTransaction = false;
            }
            catch (Exception)
            {
                transaction.Rollback();
                beginTransaction = false;
            }
        }

        //public Dictionary<string, string> ExecuteQuery()
        //{
        //    try
        //    {
        //        return GenDictionary(command.ExecuteReader());
        //    }
        //    catch (Exception)
        //    {
        //        return null;
        //    }
        //}

        //public DataTable ExecuteQuery()
        //{
        //    try
        //    {
        //        return GetTable();
        //    }
        //    catch (Exception)
        //    {
        //        return null;
        //    }
        //}

        private Dictionary<string, string> GenDictionary(SqlDataReader reader)
        {
            try
            {
                Dictionary<string, string> dictionary = null;
                if (reader.Read())
                {
                    dictionary = new Dictionary<string, string>();
                    for (int i = 0; i < reader.FieldCount; i++)
                        dictionary[reader.GetName(i)] = reader[i].ToString();
                }
                reader.Close();
                return dictionary;
            }
            catch(Exception)
            {
                return null;
            }
        }
    }
}
