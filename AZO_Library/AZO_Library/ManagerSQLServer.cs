using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace AZO_Library
{
    public abstract class ManagerSQLServer
    {
        private static SqlConnection connection = new SqlConnection();
        private static SqlCommand command;
        private static SqlDataAdapter adapter;

        //stringConexion = System.Configuration.ConfigurationManager.ConnectionStrings["sqlServer"].ConnectionString;
        //connection.ConnectionString = stringConexion;

        public sealed bool AddStoredProcedure(String storedProcedure)
        {
            try
            {
                if (connection.State.ToString().Equals("Open"))
                    connection.Close();
                connection.Open();

                command = new SqlCommand();
                command.Connection = connection;
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.CommandText = storedProcedure;
                return true;
            }
            catch (SqlException ex)
            {
                return false;
            }
        }

        public sealed void AddParameter(String parameter, Object value)
        {
            command.Parameters.AddWithValue(parameter, value);
        }

        private DataTable GetTable()
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
