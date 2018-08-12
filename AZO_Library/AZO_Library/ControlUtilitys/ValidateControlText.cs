using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace AZO_Library.ControlUtilitys
{
    public class ValidateControlText
    {
        #region CONSTANTS
        
        //Expresiones Regulares para validar algun tipo de dato
        private const string REG_EXP_DIGIT = "^\\d*$";
        private const string REG_EXP_ALPHANUMERIC = "^[a-zñA-ZÑ0-9]*$";
        private const string REG_EXP_ALPHANUMERIC_WITH_WHITESPICE = "^([a-zñA-ZÑ0-9]*|[a-zñA-ZÑ0-9]+\\s)*$";
        private const string REG_EXP_DOUBLE = "^\\d*(\\.[0-9]?[0-9]?)?$";
        private const string REG_EXP_CHARACTER = "^([a-zñA-ZÑ]*|[a-zñA-ZÑ]+\\s)*$";
        private const string REG_EXP_PORCENT = "^\\d{1,3}(\\.[0-9]?[0-9]?)?$";
        private const string REG_EXP_EMAIL = "^[_a-zA-Z0-9\\-]+(\\.[_a-zA-Z0-9\\-]+)*@[a-zA-Z0-9\\-]+(\\.[a-zA-Z0-9\\-]+)*(\\.[a-zA-Z]{2,3})$";
        private const string REG_EXP_TELEPHONE_NUMBER = "^(\\d{1,3}|\\d{1,3}\\-)(\\d{1,3}|\\d{1,3}\\-)?(\\d{1,4})?$";
        private const int MAX_STRING_LENGTH = 9;

        #endregion CONSTANTS

        #region Validaciones

        /// <summary>
        /// Se valida que el caracter ingresado en el TextBox sea un digito
        /// </summary>
        /// <param name="txtToValidate"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public static bool IsDigit(string textToValidate, KeyPressEventArgs e)
        {
            //se pone e.Handled = true para cancelar la ultima tecla presionada
            if (!System.Text.RegularExpressions.Regex.IsMatch(textToValidate + e.KeyChar, REG_EXP_DIGIT))
            {
                if (e.KeyChar != Convert.ToChar(Keys.Back))
                {
                    e.Handled = true;
                }
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
        public static bool IsDigit(string textToValidate, KeyEventArgs e)
        {
            //se pone e.Handled = true para cancelar la ultima tecla presionada
            if (!System.Text.RegularExpressions.Regex.IsMatch(textToValidate, REG_EXP_DIGIT))
            {
                if (e.KeyCode != Keys.Back)
                {
                    e.Handled = true;
                }
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
        public static bool IsDigit(string textToValidate)
        {
            return (System.Text.RegularExpressions.Regex.IsMatch(textToValidate, REG_EXP_DIGIT));
        }

        /// <summary>
        /// Se valida que el caracter ingresado en el TextBox sea un valor alfanumerico
        /// </summary>
        /// <param name="textToValidate"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public static bool IsAlphanumeric(string textToValidate, KeyPressEventArgs e)
        {
            //valida si la cadena es mayor a la cantidad maxima para recortar los ultimos caracteres y continuar la validacion sobre estos
            if (textToValidate.Length > MAX_STRING_LENGTH)
            {
                textToValidate = textToValidate.Substring(textToValidate.Length - MAX_STRING_LENGTH);
            }

            if (e.KeyChar != Convert.ToChar(Keys.Back) && !System.Text.RegularExpressions.Regex.IsMatch(textToValidate + e.KeyChar, REG_EXP_ALPHANUMERIC))
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
        /// Se valida que la cadena sea un alfanumerico
        /// </summary>
        /// <param name="textToValidate"></param>
        /// <returns></returns>
        public static bool IsAlphanumeric(string textToValidate)
        {
            return (System.Text.RegularExpressions.Regex.IsMatch(textToValidate, REG_EXP_ALPHANUMERIC));
        }

        /// <summary>
        /// Se valida que el caracter ingresado en el TextBox sea un valor alfanumerico con no mas de un espacio en blanco a la vez
        /// </summary>
        /// <param name="textToValidate"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public static bool IsAlphanumericWithWhitespice(string textToValidate, KeyPressEventArgs e)
        {
            //valida si la cadena es mayor a la cantidad maxima para recortar los ultimos caracteres y continuar la validacion sobre estos
            if (textToValidate.Length > MAX_STRING_LENGTH)
            {
                //este if evita que la cadena no cumple con la expresion por iniciar con ' ', despues de recortarla
                if (textToValidate[textToValidate.Length - MAX_STRING_LENGTH] == ' ')
                {
                    textToValidate = textToValidate.Substring(textToValidate.Length - MAX_STRING_LENGTH - 1);
                }
                else
                {
                    textToValidate = textToValidate.Substring(textToValidate.Length - MAX_STRING_LENGTH);
                }
            }

            //si no es la tecla de borrar y no cumple con la EXP_REG, entonces
            if (e.KeyChar != Convert.ToChar(Keys.Back) && !Regex.IsMatch(textToValidate + e.KeyChar, REG_EXP_ALPHANUMERIC_WITH_WHITESPICE))
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
        /// Se valida que el caracter ingresado en el TextBox sea un numero de tipo double
        /// </summary>
        /// <param name="txtToValidate"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public static bool IsDouble(TextBox txtToValidate, KeyPressEventArgs e)
        {
            //(String.IsNullOrWhiteSpace(txtToValidate.SelectedText) ? txtToValidate.Text : "") --> permite escribir cuando el texto esta seleccionado
            if (!System.Text.RegularExpressions.Regex.IsMatch(
                (String.IsNullOrWhiteSpace(txtToValidate.SelectedText) ? txtToValidate.Text : string.Empty) + e.KeyChar,
                REG_EXP_DOUBLE))
            {
                if (e.KeyChar != Convert.ToChar(Keys.Back))
                {
                    e.Handled = true;
                }
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
        public static bool IsPorcent(string textToValidate, KeyPressEventArgs e)
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(textToValidate + e.KeyChar, REG_EXP_PORCENT))
            {
                if (e.KeyChar != Convert.ToChar(Keys.Back))
                {
                    e.Handled = true;
                }
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
        public static bool IsCharacter(string textToValidate, KeyPressEventArgs e)
        {
            //valida si la cadena es mayor a la cantidad maxima para recortar los ultimos caracteres y continuar la validacion sobre estos
            if (textToValidate.Length > MAX_STRING_LENGTH)
            {
                //este if evita que la cadena no cumple con la expresion por iniciar con ' ', despues de recortarla
                if (textToValidate[textToValidate.Length - MAX_STRING_LENGTH] == ' ')
                {
                    textToValidate = textToValidate.Substring(textToValidate.Length - MAX_STRING_LENGTH - 1);
                }
                else
                {
                    textToValidate = textToValidate.Substring(textToValidate.Length - MAX_STRING_LENGTH);
                }
            }

            if (e.KeyChar != Convert.ToChar(Keys.Back) && !System.Text.RegularExpressions.Regex.IsMatch(textToValidate + e.KeyChar, REG_EXP_CHARACTER))
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
        /// Se valida que la cadena corresponda a un correo electronico
        /// </summary>
        /// <param name="textToValidate"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public static bool IsEmail(string textToValidate, KeyPressEventArgs e)
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(textToValidate + e.KeyChar, REG_EXP_EMAIL))
            {
                if (e.KeyChar != Convert.ToChar(Keys.Back))
                {
                    e.Handled = true;
                }
            }
            else
            {
                //se pone e.Handled = false para permitir que aparesca la ultima tecla presionada
                e.Handled = false;
            }
            return !e.Handled;
        }

        /// <summary>
        /// Se valida que la cadena corresponda a un correo electronico
        /// </summary>
        /// <param name="textToValidate"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public static bool IsEmail(string textToValidate)
        {
            return (System.Text.RegularExpressions.Regex.IsMatch(textToValidate, REG_EXP_EMAIL));
        }

        /// <summary>
        /// Se valida que la cadena corresponda al formato para un numero de telefono
        /// </summary>
        /// <param name="textToValidate"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public static bool IsTelephoneNumber(string textToValidate, KeyPressEventArgs e)
        {
            //se pone e.Handled = true para cancelar la ultima tecla presionada
            if ((e.KeyChar != Convert.ToChar(Keys.Back)) && !System.Text.RegularExpressions.Regex.IsMatch(textToValidate + e.KeyChar, REG_EXP_TELEPHONE_NUMBER))
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

        #endregion
    }
}
