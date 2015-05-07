using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AZO_Library.ControlUtilitys
{
    public class ManagerExceptions //: IObservable<ManagerExceptions>
    {

        private const string LOG_FILE = "LogsFile.txt";

        #region Write to File Log

        /// <summary>
        /// Sobre escritura del Metodo writeToLog cuya funcion es guardar informacion de 
        /// la excepsion en el archivo Log
        /// </summary>
        /// <param name="message"></param>
        public static void writeToLog(string message)
        {
            try
            {
                TextWriter tw = new StreamWriter(LOG_FILE, true);
                tw.WriteLine("On " + DateTime.Now.ToString() + ":" + message);
                tw.Close();
            }
            catch (Exception)
            {

            }
        }

        public static void writeToLog(Exception exception)
        {
            try
            {
                TextWriter tw = new StreamWriter(LOG_FILE, true);
                tw.WriteLine(
                    "On (" + DateTime.Now.ToString() + "), Class: " + exception.Source + "; Method: " + exception.TargetSite + 
                    "; [" + exception.Message + "] \n"
                    );
                tw.Close();
            }
            catch (Exception ex)
            {
                writeToLog(ex.Message);
            }
        }

        public static void writeToLog(string className, string methods, Exception exception)
        {
            try
            {
                TextWriter tw = new StreamWriter(LOG_FILE, true);
                tw.WriteLine(
                    "->On (" + DateTime.Now.ToString() + "), Class:" + className + "; \nMethods: {\n" + methods + "}" +
                    "\nException {\n" + exception.InnerException + "}\n Description: [\n" + exception.Message + "] \n" + 
                    "**********************"
                    );
                tw.Close();
            }
            catch (Exception ex)
            {
                writeToLog(ex.Message);
            }
        }

        public static void writeToLog(string className, string methods, string exception)
        {
            try
            {
                TextWriter tw = new StreamWriter(LOG_FILE, true);
                tw.WriteLine(
                    "->On (" + DateTime.Now.ToString() + "), Class:" + className + "; Methods: \n{" + methods + 
                    "}\n Exception: [" + exception + "] \n"
                    );
                tw.Close();
            }
            catch (Exception ex)
            {
                writeToLog(ex.Message);
            }
        }

        private static bool activeWarning(string exception)
        {
            if (exception.Contains("Valor de Timeout caducado"))
            {
                return false;
            }
            else
                return true;
        }

        #endregion
    }
}
