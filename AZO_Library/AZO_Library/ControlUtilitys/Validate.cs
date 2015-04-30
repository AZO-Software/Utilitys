using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AZO_Library.ControlUtilitys
{
    public class Validate
    {
        #region CONSTANTS
        
        private const string REG_EXP_DIGIT = "^\\d*$";
        private const string REG_EXP_ALPHANUMERIC = "^[a-zñA-ZÑ0-9]*$";
        private const string REG_EXP_ALPHANUMERIC_WITH_WHITESPICE = "^([a-zñA-ZÑ0-9]*|[a-zñA-ZÑ0-9]+\\s)*$";
        private const string REG_EXP_DOUBLE = "^\\d{0,4}(\\.[0-9]?[0-9]?)?$";
        private const string REG_EXP_CHARACTER = "^([a-zñA-ZÑ]*|[a-zñA-ZÑ]+\\s){0,3}$";
        private const string REG_EXP_PORCENT = "^\\d{1,3}(\\.[0-9]?[0-9]?)?$";

        #endregion

        #region Validaciones

        /// <summary>
        /// Se valida que el caracter ingresado en el TextBox sea un digito
        /// </summary>
        /// <param name="txtToValidate"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public bool isDigit(string textToValidate, KeyPressEventArgs e)
        {
            //se pone e.Handled = true para cancelar la ultima tecla presionada
            if (!System.Text.RegularExpressions.Regex.IsMatch(textToValidate + e.KeyChar, REG_EXP_DIGIT))
            {
                if (e.KeyChar != Convert.ToChar(Keys.Back))
                    e.Handled = true;
            }
            else
            {
                //se pone e.Handled = false para permitir que aparesca la ultima tecla presionada
                e.Handled = false;
            }
            return !e.Handled;
        }

        /// <summary>
        /// Se valida que el caracter ingresado en el TextBox sea un digito 
        /// </summary>
        /// <param name="textToValidate"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public bool isDigit(string textToValidate, KeyEventArgs e)
        {
            //se pone e.Handled = true para cancelar la ultima tecla presionada
            if (!System.Text.RegularExpressions.Regex.IsMatch(textToValidate, REG_EXP_DIGIT))
            {
                if (e.KeyCode != Keys.Back)
                    e.Handled = true;
            }
            else
            {
                //se pone e.Handled = false para permitir que aparesca la ultima tecla presionada
                e.Handled = false;
            }
            
            return !e.Handled;
        }

        /// <summary>
        /// Se valida que la cadena sea un digito
        /// </summary>
        /// <param name="textToValidate"></param>
        /// <returns></returns>
        public bool isDigit(string textToValidate)
        {
            return (System.Text.RegularExpressions.Regex.IsMatch(textToValidate, REG_EXP_DIGIT));
        }

        /// <summary>
        /// Se valida que el caracter ingresado en el TextBox sea un valor alfanumerico
        /// </summary>
        /// <param name="textToValidate"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public bool isAlphanumeric(string textToValidate, KeyPressEventArgs e)
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(textToValidate + e.KeyChar, REG_EXP_ALPHANUMERIC))
            {
                if (e.KeyChar != Convert.ToChar(Keys.Back))
                    e.Handled = true;
            }
            else
            {
                //se pone e.Handled = false para permitir que aparesca la ultima tecla presionada
                e.Handled = false;
            }
            return !e.Handled;
        }

        /// <summary>
        /// Se valida que el caracter ingresado en el TextBox sea un valor alfanumerico con no mas de un espacio en blanco a la vez
        /// </summary>
        /// <param name="textToValidate"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public bool isAlphanumericWithWhitespice(string textToValidate, KeyPressEventArgs e)
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(textToValidate + e.KeyChar, REG_EXP_ALPHANUMERIC_WITH_WHITESPICE))
            {
                if (e.KeyChar != Convert.ToChar(Keys.Back))
                    e.Handled = true;
            }
            else
            {
                //se pone e.Handled = false para permitir que aparesca la ultima tecla presionada
                e.Handled = false;
            }
            return !e.Handled;
        }

        /// <summary>
        /// Se valida que el caracter ingresado en el TextBox sea un numero de tipo double
        /// </summary>
        /// <param name="txtToValidate"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public bool isDouble(TextBox txtToValidate, KeyPressEventArgs e)
        {
            //(String.IsNullOrWhiteSpace(txtToValidate.SelectedText) ? txtToValidate.Text : "") --> permite escribir cuando el texto esta seleccionado
            if (!System.Text.RegularExpressions.Regex.IsMatch(
                (String.IsNullOrWhiteSpace(txtToValidate.SelectedText) ? txtToValidate.Text : "") + e.KeyChar,
                REG_EXP_DOUBLE))
            {
                if (e.KeyChar != Convert.ToChar(Keys.Back))
                    e.Handled = true;
            }
            else
            {
                //se pone e.Handled = false para permitir que aparesca la ultima tecla presionada
                e.Handled = false;
            }
            return !e.Handled;
        }

        /// <summary>
        /// Se valida que el caracter ingresado en el TextBox sea un numero que concuerde con un valor porcentual
        /// </summary>
        /// <param name="textToValidate"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public bool isPorcent(string textToValidate, KeyPressEventArgs e)
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(textToValidate + e.KeyChar, REG_EXP_PORCENT))
            {
                if (e.KeyChar != Convert.ToChar(Keys.Back))
                    e.Handled = true;
            }
            else if (double.Parse(textToValidate + e.KeyChar) > 100)
            {
                e.Handled = true;
            }
            else
            {
                //se pone e.Handled = false para permitir que aparesca la ultima tecla presionada
                e.Handled = false;
            }
            return !e.Handled;
        }

        /// <summary>
        /// Se valida que el caracter ingresado en el TextBox sea un caracter sin dos o mas espacios en blanco seguidos
        /// </summary>
        /// <param name="textToValidate"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public bool isCharacter(string textToValidate, KeyPressEventArgs e)
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(textToValidate + e.KeyChar, REG_EXP_CHARACTER))
            {
                if (e.KeyChar != Convert.ToChar(Keys.Back))
                    e.Handled = true;
            }
            else
            {
                //se pone e.Handled = false para permitir que aparesca la ultima tecla presionada
                e.Handled = false;
            }
            return !e.Handled;
        }

        #endregion
    }
}
