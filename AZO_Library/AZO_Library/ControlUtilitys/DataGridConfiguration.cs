using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AZO_Library.ControlUtilitys
{
    public class DataGridConfiguration
    {
        #region Properties

        //ruta del archivo que almacena la configuracion de los dataGrid
        public static string ConfigurationFilePath { get; set; }

        #endregion

        #region Globals

        private static string dataGridConfigurationPath = "Configuration.txt";
        private static ArrayList configurations = new ArrayList();
        public static ArrayList grdsName = new ArrayList();

        #endregion

        #region Methods

        /// <summary>
        /// Guarda el tamaño de las columnas de un grid en un archivo interno
        /// </summary>
        /// <param name="dataGrd"></param>
        public static void SaveConfiguration(DataGridView dataGrd)
        {
            try
            {
                if (File.Exists(ConfigurationFilePath + dataGridConfigurationPath) && dataGrd.ColumnCount > 0)
                {
                    //ingreso el nombre del grid al inicio de la cadena de configuracion
                    String cadena = dataGrd.Name + ";";

                    //ingreso el tamaño de c/u de las columnas del grid
                    for (int i = 0; i < dataGrd.ColumnCount - 1; i++)
                    {
                        cadena += dataGrd.Columns[i].Width.ToString() + ",";
                    }
                    cadena += dataGrd.Columns[dataGrd.ColumnCount - 1].Width.ToString();

                    StreamWriter writer = new StreamWriter(ConfigurationFilePath + dataGridConfigurationPath);
                    for (int i = 0; i < grdsName.Count; i++)
                    {
                        if (configurations.Count < grdsName.Count)//este es para cuando se reinicio/borro el archivo
                        {
                            configurations.Add(cadena);
                        }
                        else if (dataGrd.Name.Equals(grdsName[i]))
                        {
                            configurations[i] = cadena;//cada uno de estos representa un DataGridView
                        }
                        writer.WriteLine(configurations[i]);
                    }
                    writer.Close();
                }
            }
            catch (Exception ex)
            {
                Tools.ManagerExceptions.WriteToLog("DataGridConfiguration", "SetGridConfiguration(DataGridView)", ex);
            }
        }

        /// <summary>
        /// Revisa si existe el grid en los guardados previamente para obtener su configuracion
        /// </summary>
        /// <param name="dataGrd"></param>
        public static void UpdateConfiguration(DataGridView dataGrd)
        {
            //guarda el avance del programa antes de generar una exepcion
            string cache = string.Empty;

            try
            {
                if (dataGrd.ColumnCount > 0)
                {
                    //verificamos la existencia del folder
                    CreateConfigurationFolder();

                    if (!File.Exists(ConfigurationFilePath + dataGridConfigurationPath))
                    {
                        //Capturo el Stream para poder cerrar el archivo creado y no genere errores
                        StreamWriter writer = File.CreateText(ConfigurationFilePath + dataGridConfigurationPath);
                        writer.Close();

                        configurations = new ArrayList();
                        grdsName = new ArrayList();

                        grdsName.Add(dataGrd.Name);
                        cache += "L102,";
                    }
                    else
                    {
                        StreamReader objReader = new StreamReader(ConfigurationFilePath + dataGridConfigurationPath);
                        string strLinea = objReader.ReadLine();

                        while (strLinea != null)
                        {
                            //se verifica si la linea leida corresponde al grid recibido
                            if (!grdsName.Contains(strLinea.ToString().Split(';')[0]))
                            {
                                //agregamos la linea con la informacion del grid (nombre y tamaño de columnas)
                                configurations.Add(strLinea);
                                //agregamos el nombre de los grids ya almacenados
                                grdsName.Add(strLinea.ToString().Split(';')[0]);
                            }
                            strLinea = objReader.ReadLine();
                        }
                        cache += "L119,";

                        objReader.Close();
                        objReader.Dispose();

                        int index = grdsName.IndexOf(dataGrd.Name);

                        if(index >= 0)
                        {
                            //separamos el tamaño de las columnas del nombre del grid
                            string columnSize = configurations[index].ToString().Split(';')[1];
                            //ingresamos en el grid la informacion preeviamente guardada
                            UpdateConfiguration(dataGrd, columnSize.Split(','));
                        }
                        else
                        {//llegar aqui indica que la informacion del grid no se pudo obtener del archivo porque no se ha guardado preeviamente
                            //se guarda el nombre del nuevo grid a la lista de grids exitentes
                            grdsName.Add(dataGrd.Name);
                            //despues se guarda la informacion del grid en el archivo de configuracion
                            SaveConfiguration(dataGrd);
                        }

                        //if (configurations.Count != 0)
                        //{
                        //    for (int i = 0; i < grdsName.Count; i++)
                        //    {
                        //        cache += "L128,";
                        //        //se verifica si el grid recibido corresponde a alguno de los ya almacenados
                        //        if (dataGrd.Name.Equals(grdsName[i]))
                        //        {
                        //            cache += "L130,";
                        //            //separamos el tamaño de las columnas del nombre del grid
                        //            String auxConfiguration = configurations[i].ToString().Split(';')[1];
                        //            //ingresamos en el grid la informacion preeviamente guardada
                        //            UpdateConfiguration(dataGrd, auxConfiguration.Split(','));
                        //            return;
                        //        }
                        //    }
                        //    cache += "L135,";
                        //}
                        ////llegar aqui indica que la informacion del grid no se pudo obtener del archivo porque no se ha guardado preeviamente
                        ////se guarda el nombre del nuevo grid a la lista de grids exitentes
                        //grdsName.Add(dataGrd.Name);
                        ////despues se guarda la informacion del grid en el archivo de configuracion
                        //SaveConfiguration(dataGrd);
                    }
                }
                cache += "L140,";
            }
            catch (Exception ex)
            {
                Tools.ManagerExceptions.WriteToLog("DataGridConfiguration", "UpdateConfiguration(DataGridView)->cache[" + cache + configurations.Count + "," + grdsName.Count + "]", ex);
            }
        }

        /// <summary>
        /// Actualiza el tamaño de las columnas del grid
        /// </summary>
        /// <param name="dataGrd"></param>
        /// <param name="configuration"></param>
        private static void UpdateConfiguration(DataGridView dataGrd, String[] configuration)
        {
            try
            {
                if (configuration != null && configuration.Length != 1)
                {
                    int x = 0;
                    while (x < dataGrd.ColumnCount && x < configuration.Length)
                    {
                        dataGrd.Columns[x].Width = int.Parse(configuration[x]);
                        x++;
                    }
                }
            }
            catch (Exception ex)
            {
                Tools.ManagerExceptions.WriteToLog("DataGridConfiguration", "UpdateConfiguration(DataGridView, String[])", ex);
            }
        }

        /// <summary>
        /// Verifica si existe el folder designado para guardar el archivo Configuration.txt,
        /// creandolo en caso de no existir
        /// </summary>
        private static void CreateConfigurationFolder()
        {
            if (!string.IsNullOrEmpty(ConfigurationFilePath) && !System.IO.Directory.Exists(ConfigurationFilePath))
            {
                System.IO.Directory.CreateDirectory(ConfigurationFilePath);
            }
        }

        #endregion Methods
    }
}
