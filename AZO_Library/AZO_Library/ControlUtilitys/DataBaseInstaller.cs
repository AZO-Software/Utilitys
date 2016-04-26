using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZO_Library.ControlUtilitys
{
    /// <summary>
    /// Clase encargada de realizar la instalacion de una base de datos en una PC desde codigo
    /// </summary>
    public class DataBaseInstaller
    {
        #region Properties

        private string PathDataBaseScript { set; get; }
        private string ConnectionString { get; set; }

        #endregion

        #region Globals

        Forms.frmProgressBar prgInstallation;

        #endregion

        #region Constructor

        public DataBaseInstaller()
        {
            this.PathDataBaseScript = Tools.ManagerSQLServer.PathDataBaseScript;
        }

        public DataBaseInstaller(string pathDataBaseScript, string connectionString)
        {
            this.PathDataBaseScript = pathDataBaseScript;
            this.ConnectionString = connectionString;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Inicia la instalacion en el sistema de la Base de Datos especificada
        /// </summary>
        public void Install()
        {
            prgInstallation = new Forms.frmProgressBar();
            prgInstallation.Show();

            BackgroundWorker backbroundWorker = new BackgroundWorker();
            backbroundWorker.DoWork += bgwProgress_DoWork;
            backbroundWorker.WorkerReportsProgress = true;
            backbroundWorker.ProgressChanged += backgroundWorker_ProgressChanged;

            backbroundWorker.RunWorkerAsync();
        }

        private void bgwProgress_DoWork(object sender, DoWorkEventArgs e)
        {
            var backgroundWorker = sender as BackgroundWorker;

            if (!System.IO.File.Exists(this.PathDataBaseScript))
            {
                //MessageBox.Show("No se ha localizado el Archivo en la direccion especificada!");
                prgInstallation.SetErrorMessage("Archivo no localizado!");
                return;
            }
            else
            {
                StringBuilder scriptFile = null;
                string line;

                try
                {
                    System.IO.StreamReader objReader = new System.IO.StreamReader(this.PathDataBaseScript);
                    scriptFile = new StringBuilder();
                    line = objReader.ReadLine();

                    while (line != null)
                    {
                        scriptFile.Append(line);
                        line = objReader.ReadLine();
                    }

                    objReader.Close();
                    objReader.Dispose();
                    backgroundWorker.ReportProgress(30);

                }
                catch (Exception ex)
                {
                    //MessageBox.Show("Error al leer el Archivo de origen");
                    prgInstallation.SetErrorMessage("Error al leer el Archivo de origen");
                    return;
                }

                string dataBaseScript = Tools.Words.DecryptAES(scriptFile.ToString());
                backgroundWorker.ReportProgress(60);

                DataBase dataBase = new DataBase(this.ConnectionString, this.PathDataBaseScript);
                dataBase.Install();
                backgroundWorker.ReportProgress(100);
            }
        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            prgInstallation.SetProgress(e.ProgressPercentage);
        }

        #endregion

        /// <summary>
        /// Clase encargada de Realizar la conexion y Ejecutar el Script para la base de datos
        /// </summary>
        private class DataBase : Tools.ManagerSQLServer
        {
            public DataBase(string connectionString, string script)
            {
                try
                {
                    SetConnectionString(connectionString);
                    AddQuery(script);
                }
                catch(Exception ex)
                {
                    Tools.ManagerExceptions.WriteToLog("DataBase", "DataBase(string, string)", ex);
                }
            }

            public void Install()
            {
                ExecuteQuery();
            }
        }
    }
}
