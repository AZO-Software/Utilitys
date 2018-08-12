using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AZO_Library.Tools
{
    /// <summary>
    /// Clase encargada de guardar la informacion de las excepciones generadas dentro de un try/catch
    /// </summary>
    public class ManagerExceptions //: IObservable<ManagerExceptions>
    {
        #region Constants

        //Archivo donde se registrara el historial de logs
        private const string LOG_FILE = "LogsFile.txt";

        #endregion

        #region Globals

        /// <summary>
        /// Path en la que se guardara el archivo con la informacion de las excepciones
        /// </summary>
        private static string DEFUALT_LOGS_FOLDER = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\LogsFolder\\";

        private static Queue<string> Cache = new Queue<string>();

        #endregion

        #region Constructor

        static ManagerExceptions()
        {
            if (!System.IO.Directory.Exists(DEFUALT_LOGS_FOLDER))
            {
                System.IO.Directory.CreateDirectory(DEFUALT_LOGS_FOLDER);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Edita la path del archivo que contiene la informacion de las excepciones
        /// </summary>
        /// <param name="path"></param>
        public static void SetDefaultLogsFolder(string path)
        {
            DEFUALT_LOGS_FOLDER = path;
        }

        /// <summary>
        /// Guarda la informacion de la excepcion en el archivo LogsFile.txt
        /// </summary>
        /// <param name="message"></param>
        public static void WriteToLog(string message)
        {
            try
            {
                TextWriter tw = new StreamWriter(DEFUALT_LOGS_FOLDER + LOG_FILE, true);
                tw.WriteLine("On " + DateTime.Now.ToString() + ":" + message);
                tw.Close();
            }
            catch (Exception)
            {

            }
        }

        public static void WriteToLog(Exception exception)
        {
            try
            {
                TextWriter tw = new StreamWriter(DEFUALT_LOGS_FOLDER + LOG_FILE, true);
                tw.WriteLine(
                    "On (" + DateTime.Now.ToString() + "), Class: " + exception.Source + "; Method: " + exception.TargetSite + 
                    "; [" + exception.Message + "] \n"
                    );
                tw.Close();
            }
            catch (Exception ex)
            {
                WriteToLog(ex.Message);
            }
        }

        /// <summary>
        /// Guarda la informacion de la excepcion en el archivo LogsFile.txt
        /// </summary>
        /// <param name="className">Nombre de la clase en la que se genero la excepcion</param>
        /// <param name="methods">metodos ejecutados antes de que se generara la excepcion</param>
        /// <param name="exception"></param>
        public static void WriteToLog(string className, string methods, Exception exception)
        {
            try
            {
                //aqui se debe de obtener lo almacenado en cache
                TextWriter tw = new StreamWriter(DEFUALT_LOGS_FOLDER + LOG_FILE, true);
                //tw.WriteLine(
                //    "->On (" + DateTime.Now.ToString() + "), Class:" + className + "; \nMethods: {\n" + methods + "}" +
                //    "\nException {\n" + exception.InnerException + "}\n Description: [\n" + exception.Message + "] \n" + 
                //    "**********************"
                //    );

                tw.WriteLine(
                    string.Format("->On ({0}), Class:{1}; \nMethods: [\n{2}{3}]\nException: [\n{4}]\n Description: [\n{5}] \n**********************",
                    DateTime.Now.ToString(), className, methods, GetCache(), exception.InnerException, exception.Message));
                tw.Close();
            }
            catch (Exception ex)
            {
                WriteToLog(ex.Message);
            }
        }

        public static void WriteToLog(string className, string methods, string exception)
        {
            try
            {
                TextWriter tw = new StreamWriter(DEFUALT_LOGS_FOLDER + LOG_FILE, true);
                tw.WriteLine(
                    "->On (" + DateTime.Now.ToString() + "), Class:" + className + "; Methods: \n{" + methods + 
                    "}\n Exception: [" + exception + "] \n"
                    );
                tw.Close();
            }
            catch (Exception ex)
            {
                WriteToLog(ex.Message);
            }
        }

        private static bool ActiveWarning(string exception)
        {
            if (exception.Contains("Valor de Timeout caducado"))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static void InsertCache(string value)
        {
            //verificamos que la cantidad de elementos no sobrepase los 10
            if (Cache.Count > 10)
            {
                Cache.Dequeue();
            }

            Cache.Enqueue(value);
        }

        public static string GetCache()
        {
            string result = "(";

            while(Cache.Count > 0)
            {
                result += string.Format("{0},", Cache.Dequeue());
            }
            result += ")";

            return result;
        }

        #endregion
    }
}
