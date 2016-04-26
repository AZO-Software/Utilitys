using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AZO_Library.ControlUtilitys
{
    public class ManagerControls
    {
        /// <summary>
        /// Llena un combobox con la informacion de la tabla proporcionada
        /// </summary>
        /// <param name="cbxAux"></param>Combobox al que se le agregara la informacion
        /// <param name="dataTable"></param>
        /// <param name="displayMember"></param>Campo de la tabla que se mostrara dentro del combobox
        /// <param name="valueMember"></param>
        /// <param name="autoComplete"></param> especifica si tambien se agregara la parte del autocomplete al combobox
        public static void FillCbxWithTable(ComboBox cbxAux, DataTable dataTable, string displayMember, string valueMember, bool autoComplete)
        {
            try
            {
                cbxAux.BeginUpdate();

                //vacia el combobox si este ya tenia valores
                if (cbxAux.Items.Count > 0)
                {
                    cbxAux.DataSource = null;
                }

                cbxAux.DisplayMember = displayMember;
                cbxAux.ValueMember = valueMember;
                cbxAux.DataSource = dataTable;

                //llena la parte del autocompletado si el usuario asi lo especifico
                if (autoComplete)
                {
                    AutoCompleteStringCollection collection = new AutoCompleteStringCollection();
                    foreach (DataRow name in dataTable.Rows)
                    {
                        collection.Add(name[displayMember].ToString());
                    }
                    cbxAux.AutoCompleteCustomSource = collection;
                    cbxAux.AutoCompleteMode = AutoCompleteMode.Suggest;
                    cbxAux.AutoCompleteSource = AutoCompleteSource.CustomSource;
                }

                cbxAux.EndUpdate();
            }
            catch (Exception ex)
            {
                Tools.ManagerExceptions.WriteToLog("ManagerControls", "FillCbxWithTable", ex);
            }
        }

        /// <summary>
        /// Ingresa la informacion de la tabla proporcionada en un combobox
        /// </summary>
        /// <param name="cbxAux"></param>Combobox al que se le agregara la informacion
        /// <param name="dataTable"></param>
        /// <param name="displayMember"></param>Campo de la tabla que se mostrara dentro del combobox
        /// <param name="valueMember"></param>
        /// <param name="autoComplete"></param> especifica si tambien se agregara la parte del autocomplete al combobox
        public static void FillCbxWithTable(ComboBox cbxAux, DataTable dataTable, string displayMember, string valueMember, string autoComplete = null)
        {
            try
            {
                cbxAux.BeginUpdate();

                //vacia el combobox si este ya tenia valores
                if (cbxAux.Items.Count > 0)
                {
                    cbxAux.DataSource = null;
                }

                //verifica si se debe muestrar el displey member o debe de aparecer vacio
                if (!string.IsNullOrEmpty(displayMember))
                {
                    cbxAux.DisplayMember = displayMember;
                    cbxAux.ValueMember = valueMember;
                }
                cbxAux.DataSource = dataTable;

                //llena la parte del autocompletado si el usuario asi lo especifico
                if (!string.IsNullOrEmpty(autoComplete))
                {
                    AutoCompleteStringCollection collection = new AutoCompleteStringCollection();
                    foreach (DataRow name in dataTable.Rows)
                    {
                        collection.Add(name[autoComplete].ToString());
                    }
                    cbxAux.AutoCompleteCustomSource = collection;
                    cbxAux.AutoCompleteMode = AutoCompleteMode.Suggest;
                    cbxAux.AutoCompleteSource = AutoCompleteSource.CustomSource;
                }

                cbxAux.EndUpdate();
            }
            catch (Exception ex)
            {
                Tools.ManagerExceptions.WriteToLog("ManagerControls", "FillCbxWithTable", ex);
            }
        }

        public static void TextBoxAddAutoComplete(TextBox textBox, DataTable dataTable, string displayMember)
        {
            try
            {
                AutoCompleteStringCollection collection = new AutoCompleteStringCollection();
                foreach (DataRow name in dataTable.Rows)
                {
                    collection.Add(name[displayMember].ToString());
                }

                textBox.AutoCompleteCustomSource = collection;
                textBox.AutoCompleteMode = AutoCompleteMode.Suggest;
                textBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
            }
            catch (Exception ex)
            {
                Tools.ManagerExceptions.WriteToLog("ManagerControls", "TextBoxAddAutoComplete(DataTable, string, string)", ex);
            }
        }

        /// <summary>
        /// Llena un listbox con la informacion contenida dentro de la tabla especificada
        /// </summary>
        /// <param name="ltbxAux"></param>
        /// <param name="dataTable"></param>
        /// <param name="displayMember"></param>
        /// <param name="valueMember"></param>
        public static void FillLtbxWithTable(ListBox ltbxAux, DataTable dataTable, string displayMember, string valueMember)
        {
            try
            {
                ltbxAux.BeginUpdate();

                //vacia la lista si esta ya tenia valores
                if (ltbxAux.Items.Count > 0)
                {
                    ltbxAux.DataSource = null;
                }

                ltbxAux.DisplayMember = displayMember;
                ltbxAux.ValueMember = valueMember;
                ltbxAux.DataSource = dataTable;

                ltbxAux.EndUpdate();
            }
            catch (Exception ex)
            {
                Tools.ManagerExceptions.WriteToLog("ManagerControls", "FillLtbxWithTable", ex);
            }
        }

        /// <summary>
        /// Actualiza el filtro del DataGridView especificado
        /// </summary>
        /// <param name="grid">Grid al que se aplicara el filtro</param>
        /// <param name="filter">Filtro que se desea aplicar</param>
        public static void UpdateGridFilter(DataGridView grid, string filter)
        {
            try
            {
                DataView dv = null;
                //verifica si el datasource es una tabla, y si es asi la convierte a dataview sino solo hace un casting
                if (grid.DataSource.GetType() == typeof(DataTable))
                {
                    dv = new DataView((DataTable)grid.DataSource);
                }
                else
                {
                    dv = (DataView)grid.DataSource;
                }

                dv.RowFilter = filter;
                grid.DataSource = dv;
            }
            catch (Exception ex)
            {
                Tools.ManagerExceptions.WriteToLog("ManagerControls", "UpdateFilter", ex);
            }
        }

        /// <summary>
        /// Método que exporta a un fichero Excel el contenido de un DataGridView
        /// </summary>
        /// <param name="dgvInformation">DataGridView que contiene los datos a exportar</param>
        /// <returns>True si se llevo a cabo la exportacion de manera satisfactoria,
        /// False en caso de no haberse almacenado toda la informacion en el archivo .xls</returns>
        public static bool ExportDataGridViewToExcelFile(DataGridView dgvInformation)
        {
            try
            {
                SaveFileDialog fichero = new SaveFileDialog();
                fichero.Filter = "Excel (*.xls)|*.xls";
                if (fichero.ShowDialog() == DialogResult.OK)
                {
                    //pones el cursor en espera
                    Cursor.Current = Cursors.WaitCursor;

                    Microsoft.Office.Interop.Excel.Application application;
                    Microsoft.Office.Interop.Excel.Workbook workBook;
                    Microsoft.Office.Interop.Excel.Worksheet workSheet;
                    application = new Microsoft.Office.Interop.Excel.Application();
                    workBook = application.Workbooks.Add();
                    workSheet = (Microsoft.Office.Interop.Excel.Worksheet)workBook.Worksheets.get_Item(1);

                    //Recorremos el DataGridView rellenando la hoja de trabajo
                    for (int i = 0; i < dgvInformation.Rows.Count; i++)
                    {
                        int ignoredColumns = 0;//representa las columnas omitidas
                        for (int j = 0; j < dgvInformation.Columns.Count; j++)
                        {
                            //Si no es visible la columna no la exporto
                            if (!dgvInformation.Rows[i].Cells[j].Visible)
                            {
                                ignoredColumns++;
                                continue;
                            }

                            //agrego el nombre de la columna en base al grid, esto cuando es la primer fila que se va a agregar
                            if (i == 0)
                            {
                                workSheet.Cells[i + 1, j + 1 - ignoredColumns] = dgvInformation.Columns[j].HeaderText;
                            }

                            //exporto solo las columnas visibles
                            workSheet.Cells[i + 2, j + 1 - ignoredColumns] = dgvInformation.Rows[i].Cells[j].Value.ToString();
                        }
                    }
                    workBook.SaveAs(fichero.FileName, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal);
                    workBook.Close(true);
                    application.Quit();

                    //regreso el cursor a su estado normal
                    Cursor.Current = Cursors.Default;
                    return true;
                }
            }
            catch (Exception ex)
            {
                //regreso el cursor a su estado normal
                Cursor.Current = Cursors.Default;
                Tools.ManagerExceptions.WriteToLog("ManagerControls", "ExportarDataGridViewExcel", ex);
            }

            return false;
        }
    }
}
