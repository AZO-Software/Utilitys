using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AZO_Library.ControlUtilitys.Forms
{
    public partial class SelectionOneRowBox : Form
    {
        #region Properties

        /// <summary>
        /// Ultima fila seleccionada antes de presionar aceptar
        /// </summary>
        private DataRow SelectionRow { get; set; }

        #endregion

        #region Constructor

        public SelectionOneRowBox()
        {
            InitializeComponent();
        }

        #endregion

        #region Methods

        public static DataRow ShowBox(DataTable table, string []enableColumns)
        {
            using (SelectionOneRowBox selectionOneRowBox = new SelectionOneRowBox())
            {
                selectionOneRowBox.ChargeInformation(table, enableColumns);
                selectionOneRowBox.ShowDialog();

                return selectionOneRowBox.SelectionRow;
            }
        }

        /// <summary>
        /// Carga la informacion en el grid
        /// </summary>
        /// <param name="table"></param>
        /// <param name="enableColumns">Columnas que se ocultaran en el grid</param>
        private void ChargeInformation(DataTable table, string []enableColumns)
        {
            dgdInformation.DataSource = table;
            if (dgdInformation.ColumnCount > 0)
            {
                //oculta las columnas especificadas
                foreach (string column in enableColumns)
                {
                    dgdInformation.Columns[column].Visible = false;
                }
            }
        }

        #endregion

        #region Events

        private void btnAccept_Click(object sender, EventArgs e)
        {
            SelectionRow = ((DataRowView)dgdInformation.CurrentRow.DataBoundItem).Row;
        }

        #endregion
    }
}
