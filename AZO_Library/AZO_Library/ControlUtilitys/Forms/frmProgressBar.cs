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
    public partial class frmProgressBar : Form
    {
        #region Constructor

        public frmProgressBar()
        {
            InitializeComponent();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Modifica el porcentaje de progreso de la barra
        /// </summary>
        /// <param name="progressPercent"></param>
        public void SetProgress(int progressPercent)
        {
            if (this.InvokeRequired)
            {
                SetProgressDelegate progressDelegate = new SetProgressDelegate(SetProgress);
                object[] parameters = new object[] { progressPercent};
                this.Invoke(progressDelegate, parameters);
            }
            else
            {
                prgStatus.Value = progressPercent;
                lblProgress.Text = string.Format("Progreso: {0} %", progressPercent);
            }
        }

        delegate void SetProgressDelegate(int progressPercent);

        /// <summary>
        /// Establece el mensaje de error que se muestra
        /// </summary>
        /// <param name="message"></param>
        public void SetErrorMessage(string message)
        {
            //preguntamos si la llamada se hace desde un hilo 
            if (this.InvokeRequired)
            {
                //si es así entonces volvemos a llamar a SetErrorMessage pero esta vez a través del delegado 
                //instanciamos el delegado indicandole el método que va a ejecutar 
                SetErrorMessageDelegate errorMessageDelegate = new SetErrorMessageDelegate(SetErrorMessage);
                //ya que el delegado invocará a SetErrorMessage debemos indicarle los parámetros 
                object[] parameters = new object[] { message};
                //invocamos el método a través del mismo contexto del formulario (this) y enviamos los parámetros 
                this.Invoke(errorMessageDelegate, parameters);
            }
            else
            {
                //en caso contrario, se realiza el llamado a los controles 
                tlsErrorMessage.Text = message;
                tlsErrorMessage.Visible = true;
                btnAccept.Enabled = true;
            }
        }

        delegate void SetErrorMessageDelegate(string message);

        private void btnAccept_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion
    }
}
