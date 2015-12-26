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

        /// <summary>
        /// Especifica la localizacion de la base de datos sobre la que se trabajara
        /// </summary>
        /// <param name="strConnection"></param>
        protected void SetConnectionString(String strConnection)
        {
            this.connection.ConnectionString = strConnection;

            command = new SqlCommand();
            command.Connection = connection;
        }

        /// <summary>
        /// Recibe y prepara un query para llevar a cabo su ejecucion
        /// </summary>
        /// <param name="query"></param>
        /// <returns>True si se agrego el query correctamente y False si genero algun error</returns>
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
                Tools.ManagerExceptions.WriteToLog("ManagerSQLServer", "AddQuery(String)", ex);
                return false;
            }
        }

        /// <summary>
        /// Recibe y prepara un store procedure para llevar a cabo su ejecucion
        /// </summary>
        /// <param name="storedProcedure"></param>
        /// <returns>True si se agrego el StoreProcedure correctamente y False si genero algun error</returns>
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
                Tools.ManagerExceptions.WriteToLog("ManagerSQLServer", "AddStoredProcedure(String)", ex);
                return false;
            }
        }

        /// <summary>
        /// Agrega un parametro y su valor al StoreProcedure previamente agregado
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="value"></param>
        protected void AddParameter(String parameter, Object value)
        {
            command.Parameters.AddWithValue(parameter, value);
        }

        /// <summary>
        /// Ejecuta la instruccion previamente ingresada y regresa la cantidad de filas afectadas
        /// </summary>
        /// <param name="beginTransaction"></param>
        /// <returns>Devuelve la cantidad de filas afectadas o -1 en caso de algun error</returns>
        protected int ExecuteQuery(bool beginTransaction = false)
        {
            try
            {
                if(beginTransaction)
                {
                    this.beginTransaction = beginTransaction;
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
                Tools.ManagerExceptions.WriteToLog("ManagerSQLServer", "ExecuteQuery(bool)", ex);
                return -1;
            }
        }

        /// <summary>
        /// Ejecuta la instruccion previamente ingresada y devuelve un objeto SqlDataReader con el resultado
        /// </summary>
        /// <param name="beginTransaction"></param>
        /// <returns>Genera un objeto SqlDataReader con el resultado de la instruccion, null en caso error</returns>
        protected SqlDataReader GetReader(bool beginTransaction = false)
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
                Tools.ManagerExceptions.WriteToLog("ManagerSQLServer", "GetReader(bool)", ex);
                return null;
            }
        }

        //verificar k funciona con un solo metodo agregando un valor por default al parametro
        //protected DataTable GetTable()
        //{
        //    try
        //    {
        //        if (connection.State == ConnectionState.Open)
        //        {
        //            DataSet ds = new DataSet();
        //            adapter = new SqlDataAdapter(command);
        //            adapter.Fill(ds, "VALSEC");
        //            return ds.Tables[0];
        //        }
        //        else
        //        {
        //            return null;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Tools.ManagerExceptions.writeToLog("ManagerSQLServer", "GetTable", ex);
        //        return null;
        //    }
        //}

        /// <summary>
        /// Regresa una tabla en especifico de las obtenidas de la consulta
        /// </summary>
        /// <param name="tableNumber">Tabla que se desea obtener</param>
        /// <returns>Regresa la tabla especificada o null en caso haberse generado error o no haber obtenido resultado de la instruccion</returns>
        protected DataTable GetTable(byte tableNumber = 0)
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
                Tools.ManagerExceptions.WriteToLog("ManagerSQLServer", "GetTable(byte)", ex);
                return null;
            }
        }

        /// <summary>
        /// Ejecuta la instruccion previamente ingresada regresando un valor de retorno especificado
        /// </summary>
        /// <returns>Regresa un valor especificado en la instruccion, el valor de retorno siempre debe llamarse returnValue</returns>
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
                if (beginTransaction)
                {
                    beginTransaction = false;
                    transaction.Rollback();
                }
                Tools.ManagerExceptions.WriteToLog("ManagerSQLServer", "ExecuteAndGetReturnValue", ex);
                return null;
            }
        }

        /// <summary>
        /// Indica el inicio de una transaccion
        /// </summary>
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
                Tools.ManagerExceptions.WriteToLog("ManagerSQLServer", "BeginTransaction", ex);
            }
        }

        /// <summary>
        /// Indica la finalizacion de la transaccion
        /// </summary>
        protected void EndTransaction()
        {
            try
            {
                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Tools.ManagerExceptions.WriteToLog("ManagerSQLServer", "EndTransaction", ex);
            }
            beginTransaction = false;
        }

        /// <summary>
        /// Genera la cancelacion de la transaccion en curso
        /// </summary>
        protected void RollbackTransaction()
        {
            try
            {
                if (beginTransaction)
                {
                    transaction.Rollback();
                }
            }
            catch(Exception ex)
            {
                Tools.ManagerExceptions.WriteToLog("ManagerSQLServer", "RollbackTransaction", ex);
            }
            beginTransaction = false;
        }

        /// <summary>
        /// Genera un diccionario apartir del SqlDataReader recibido
        /// </summary>
        /// <param name="reader"></param>
        /// <returns>Regresa el diccionario generado con el SqlDataReader recibido y null en caso de error</returns>
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
                Tools.ManagerExceptions.WriteToLog("ManagerSQLServer", "GenDictionary(SqlDataReader)", ex);
                return null;
            }
        }
    }
}
