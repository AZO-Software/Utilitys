using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AZO_Library.ControlUtilitys
{
    public class DataGridConfiguration
    {
        private static ArrayList configurations;
        private static string dataGridConfigurationPath = "Configuration.txt";
        public static ArrayList grdsName;

        /// <summary>
        /// Guarda el tamaño de las columnas de un grid en un archivo interno
        /// </summary>
        /// <param name="dataGrd"></param>
        public static void SaveConfiguration(DataGridView dataGrd)
        {
            try
            {
                if (File.Exists(dataGridConfigurationPath) && dataGrd.ColumnCount > 0)
                {
                    //ingreso el nombre del grid al inicio de la cadena de configuracion
                    String cadena = dataGrd.Name + ";";

                    //ingreso el tamaño de c/u de las columnas del grid
                    for (int i = 0; i < dataGrd.ColumnCount - 1; i++)
                    {
                        cadena += dataGrd.Columns[i].Width.ToString() + ",";
                    }
                    cadena += dataGrd.Columns[dataGrd.ColumnCount - 1].Width.ToString();

                    StreamWriter writer = new StreamWriter(dataGridConfigurationPath);
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
                Tools.ManagerExceptions.writeToLog("DataGridConfiguration", "setGridConfiguration", ex);
            }
        }

        /// <summary>
        /// Revisa si existe el grid en los guardados previamente para obtener su configuracion
        /// </summary>
        /// <param name="dataGrd"></param>
        public static void UpdateConfiguration(DataGridView dataGrd)
        {
            try
            {
                if (dataGrd.ColumnCount > 0)
                {
                    if (!File.Exists(dataGridConfigurationPath))
                    {
                        //Capturo el Stream para poder cerrar el archivo creado y no genere errores
                        StreamWriter writer = File.CreateText(dataGridConfigurationPath);
                        writer.Close();

                        configurations = new ArrayList();
                        grdsName = new ArrayList();

                        grdsName.Add(dataGrd.Name);
                    }
                    else
                    {
                        StreamReader objReader = new StreamReader(dataGridConfigurationPath);
                        configurations = new ArrayList();
                        grdsName = new ArrayList();
                        string strLinea = objReader.ReadLine();

                        while (strLinea != null)
                        {
                            configurations.Add(strLinea);
                            //agregamos el nombre de los grids ya almacenados
                            grdsName.Add(strLinea.ToString().Split(';')[0]);
                            strLinea = objReader.ReadLine();
                        }

                        objReader.Close();
                        objReader.Dispose();

                        if (configurations.Count != 0)
                        {
                            for (int i = 0; i < grdsName.Count; i++)
                            {
                                String auxConfiguration = configurations[i].ToString().Split(';')[1];
                                if (dataGrd.Name.Equals(grdsName[i]))
                                {
                                    UpdateConfiguration(dataGrd, auxConfiguration.Split(','));
                                    break;
                                }
                            }
                        }
                        else
                        {
                            grdsName.Add(dataGrd.Name);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Tools.ManagerExceptions.writeToLog("DataGridConfiguration", "UpdateConfiguration", ex);
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
                Tools.ManagerExceptions.writeToLog("DataGridConfiguration", "UpdateConfiguration", ex);
            }
        }
    }
}
