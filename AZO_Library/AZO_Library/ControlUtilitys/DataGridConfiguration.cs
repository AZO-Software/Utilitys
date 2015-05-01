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
        private static ArrayList dataGridConfiguration;
        private static string dataGridConfigurationPath;
        public static string[] grdsName = { "grdInvoices", "grdItems", "grdAllItems", "grdItemsInfo" };

        public static void setGridConfiguration(DataGridView dataGrd)
        {
            try
            {
                if (File.Exists(dataGridConfigurationPath) && dataGrd.RowCount > 0)
                {
                    String cadena = "";

                    for (int i = 0; i < dataGrd.ColumnCount - 1; i++)
                    {
                        cadena += dataGrd.Columns[i].Width.ToString() + ",";
                    }
                    cadena += dataGrd.Columns[dataGrd.ColumnCount - 1].Width.ToString();

                    StreamWriter writer = new StreamWriter(dataGridConfigurationPath);
                    for (int i = 0; i < grdsName.Length; i++)
                    {
                        if (dataGridConfiguration.Count < grdsName.Length)//este es para cuando se reinicio/borro el archivo
                            dataGridConfiguration.Add(cadena);
                        else if (dataGrd.Name.Equals(grdsName[i]))
                            dataGridConfiguration[i] = cadena;//cada uno de estos representa un DataGridView
                        writer.WriteLine(dataGridConfiguration[i]);
                    }
                    writer.Close();
                }
            }
            catch (Exception ex)
            {
                ManagerExceptions.writeToLog("ManagerControls", "setGridConfiguration", ex);
            }
        }

        public static void getGridConfiguration(DataGridView dataGrd)
        {
            try
            {
                if (dataGrd.ColumnCount > 0)
                {
                    if (!File.Exists(dataGridConfigurationPath))
                    {
                        StreamWriter writer = File.CreateText(dataGridConfigurationPath);
                        writer.Close();
                        dataGridConfiguration = new ArrayList();
                    }
                    else
                    {
                        StreamReader objReader = new StreamReader(dataGridConfigurationPath);
                        string strLinea = objReader.ReadLine();
                        ArrayList arrText = new ArrayList();

                        while (strLinea != null)
                        {
                            arrText.Add(strLinea);
                            strLinea = objReader.ReadLine();
                        }
                        dataGridConfiguration = arrText;

                        objReader.Close();
                        objReader.Dispose();

                        for (int i = 0; i < grdsName.Length; i++)
                        {
                            if (dataGrd.Name.Equals(grdsName[i]))
                            {
                                insertConfigurationInGrid(dataGrd,
                                    (dataGridConfiguration.Count != 0) ? dataGridConfiguration[i].ToString().Split(',') : null);
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ManagerExceptions.writeToLog("ManagerControls", "getGridConfiguration", ex);
            }
        }

        private static void insertConfigurationInGrid(DataGridView dataGrd, String[] configuration)
        {
            try
            {
                if (configuration != null && configuration.Length != 1)
                    for (int i = 0; i < dataGrd.ColumnCount; i++)
                    {
                        dataGrd.Columns[i].Width = int.Parse(configuration[i]);
                    }
            }
            catch (Exception ex)
            {
                ManagerExceptions.writeToLog("ManagerControls", "insertConfigurationInGrid", ex);
            }
        }
    }
}
