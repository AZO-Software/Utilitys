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
                ManagerExceptions.writeToLog("ManagerControls", "fillCbxWithTable", ex);
            }
        }

        /// <summary>
        /// Llena un listbox con la informacion contenida dentra la tabla especificada
        /// </summary>
        /// <param name="ltbxAux"></param>
        /// <param name="dataTable"></param>
        /// <param name="displayMember"></param>
        /// <param name="valueMember"></param>
        public static void FillLtbxWithOffers(ListBox ltbxAux, DataTable dataTable, string displayMember, string valueMember)
        {
            ltbxAux.BeginUpdate();

            if (ltbxAux.Items.Count > 0)
            {
                ltbxAux.DataSource = null;
            }

            ltbxAux.DisplayMember = displayMember;
            ltbxAux.ValueMember = valueMember;
            ltbxAux.DataSource = dataTable;

            ltbxAux.EndUpdate();
        }
    }
}
