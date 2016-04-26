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
    public partial class InputBox : Form
    {
        #region Constants

        public const int VALIDATE_DIGIT = 1;
        public const int VALIDATE_DOUBLE = 2;
        public const int VALIDATE_ALPHANUMERIC = 3;
        public const int VALIDATE_ALPHANUMERIC_WITH_A_SPACE = 4;

        #endregion

        #region Globals

        private static int ValidationType { get; set; }

        #endregion

        #region Constructor

        public InputBox()
        {
            InitializeComponent();
        }

        #endregion

        #region Methods

        public static DialogResult ShowBox(string message, ref string inputText, int validate = 0)
        {
            using (InputBox inputBox = new InputBox())
            {
                ValidationType = validate;

                inputBox.Text = string.Empty;
                inputBox.lblMessage.Text = message;
                inputBox.txtInputText.Text = inputText;
                inputBox.txtInputText.SelectAll();

                DialogResult result = inputBox.ShowDialog();
                inputText = inputBox.txtInputText.Text;
                return result;
            }
        }

        public static DialogResult ShowBox(string headerText, string message, ref string inputText, int validate = 0)
        {
            using (InputBox inputBox = new InputBox())
            {
                ValidationType = validate;

                inputBox.Text = headerText;
                inputBox.lblMessage.Text = message;
                inputBox.txtInputText.Text = inputText;
                inputBox.txtInputText.SelectAll();

                DialogResult result = inputBox.ShowDialog();
                inputText = inputBox.txtInputText.Text;
                return result;
            }
        }

        #endregion

        #region Events

        private void btnAccept_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtInputText_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            switch(ValidationType)
            {
                case 1:
                    ValidateControlText.IsDigit(textBox.Text, e);
                    break;
                case 2:
                    ValidateControlText.IsDouble(textBox, e);
                    break;
                case 3:
                    ValidateControlText.IsAlphanumeric(textBox.Text, e);
                    break;
                case 4:
                    ValidateControlText.IsAlphanumericWithWhitespice(textBox.Text, e);
                    break;
            }
        }

        #endregion
    }
}
