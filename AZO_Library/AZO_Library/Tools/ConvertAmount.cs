using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace AZO_Library.Tools
{
    /// <summary>
    /// Convierte números de su expresión numérica a su numeral cardinal
    /// </summary>
    public sealed class ConvertAmount
    {
        #region Constants

        private const int UNI = 0, DIECI = 1, DECENA = 2, CENTENA = 3;
        private static string[,] _matriz = new string[5, 10]
        {
            {null," uno", " dos", " tres", " cuatro", " cinco", " seis", " siete", " ocho", " nueve"},
            {" diez"," once"," doce"," trece"," catorce"," quince"," dieciseis"," diecisiete"," dieciocho"," diecinueve"},
            {" veinte", " veintiun", " veintidos", " veintitres", " veinticuatro", " veinticinco", " veintiseis", " veintisiete", " veintiocho", " veintinueve"},
            {null,null," veinti"," treinta"," cuarenta"," cincuenta"," sesenta"," setenta"," ochenta"," noventa"},
            {null," cien"," doscientos"," trescientos"," cuatrocientos"," quinientos"," seiscientos"," setecientos"," ochocientos"," novecientos"}
        };

        private const Char sub = (Char)26;
        //Cambiar acá si se quiere otro comportamiento en los métodos de clase
        public const String SeparadorDecimalSalidaDefault = "con";
        public const String MascaraSalidaDecimalDefault = "00'/100.-m.n.'";
        public const Int32 DecimalesDefault = 2;
        public const Boolean LetraCapitalDefault = false;
        public const Boolean ConvertirDecimalesDefault = false;
        public const Boolean ApocoparUnoParteEnteraDefault = false;
        public const Boolean ApocoparUnoParteDecimalDefault = false;
        public const string MONEY = "pesos";
        public const string DENOMINACION = "m.n.";

        #endregion

        #region Propiedades

        private Int32 _posiciones = DecimalesDefault;
        
        /// <summary>
        /// Indica la cantidad de decimales que se pasarán a entero para la conversión
        /// </summary>
        /// <remarks>Esta propiedad cambia al cambiar MascaraDecimal por un valor que empieze con '0'</remarks>
        public Int32 Decimales
        {
            get { return _decimales; }
            set
            {
                if (value > 10)
                {
                    throw new ArgumentException(value.ToString() + " excede el número máximo de decimales admitidos, solo se admiten hasta 10.");
                }
                _decimales = value;
            }
        }
        private Int32 _decimales = DecimalesDefault;

        /// <summary>
        /// Objeto CultureInfo utilizado para convertir las cadenas de entrada en números
        /// </summary>
        public CultureInfo CultureInfo
        {
            get { return _cultureInfo; }
            set { _cultureInfo = value; }
        }
        private CultureInfo _cultureInfo = CultureInfo.CurrentCulture;

        /// <summary>
        /// Indica la cadena a intercalar entre la parte entera y la decimal del número
        /// </summary>
        public String SeparadorDecimalSalida
        {
            get { return _separadorDecimalSalida; }
            set
            {
                _separadorDecimalSalida = value;
                //Si el separador decimal es compuesto, infiero que estoy cuantificando algo,
                //por lo que apocopo el "uno" convirtiéndolo en "un"
                if (value.Trim().IndexOf(" ") > 0)
                {
                    //_apocoparUnoParteEntera = true;
                    this.ApocoparUnoParteEntera = true;
                }
                else
                {
                    //_apocoparUnoParteEntera = false;
                    this.ApocoparUnoParteEntera = false;
                }
            }
        }
        private String _separadorDecimalSalida = SeparadorDecimalSalidaDefault;

        /// <summary>
        /// Indica el formato que se le dara a la parte decimal del número
        /// </summary>
        public String MascaraSalidaDecimal
        {
            get
            {
                if (!String.IsNullOrEmpty(_mascaraSalidaDecimal))
                {
                    return _mascaraSalidaDecimal;
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                //determino la cantidad de cifras a redondear a partir de la cantidad de '0' o '#' 
                //que haya al principio de la cadena, y también si es una máscara numérica
                int i = 0;
                while (i < value.Length && (value[i] == '0') | value[i] == '#')
                {
                    i++;
                }
                _posiciones = i;
                if (i > 0)
                {
                    _decimales = i;
                    _esMascaraNumerica = true;
                }
                else
                {
                    _esMascaraNumerica = false;
                }
                _mascaraSalidaDecimal = value;
                if (_esMascaraNumerica)
                {
                    _mascaraSalidaDecimalInterna = value.Substring(0, _posiciones) + "'"
                        + value.Substring(_posiciones)
                        .Replace("''", sub.ToString())
                        .Replace("'", string.Empty)
                        .Replace(sub.ToString(), "'") + "'";
                }
                else
                {
                    _mascaraSalidaDecimalInterna = value
                        .Replace("''", sub.ToString())
                        .Replace("'", string.Empty)
                        .Replace(sub.ToString(), "'");
                }
            }
        }
        private string _mascaraSalidaDecimal;
        private string _mascaraSalidaDecimalInterna = MascaraSalidaDecimalDefault;
        private Boolean _esMascaraNumerica = true;

        /// <summary>
        /// Indica si la primera letra del resultado debe estár en mayúscula
        /// </summary>
        public Boolean LetraCapital
        {
            //get { return _letraCapital; }
            //set { _letraCapital = value; }
            get;
            set;
        }
        //private Boolean _letraCapital = LetraCapitalDefault;

        /// <summary>
        /// Indica si se deben convertir los decimales a su expresión nominal
        /// </summary>
        public Boolean ConvertirDecimales
        {
            get { return _convertirDecimales; }
            set
            {
                _convertirDecimales = value;
                //_apocoparUnoParteDecimal = value;
                this.ApocoparUnoParteDecimal = value;
                if (value)
                {// Si la máscara es la default, la borro
                    if (_mascaraSalidaDecimal == MascaraSalidaDecimalDefault)
                    {
                        MascaraSalidaDecimal = string.Empty;
                    }
                }
                else if (String.IsNullOrEmpty(_mascaraSalidaDecimal))
                {
                    //Si no hay máscara dejo la default
                    MascaraSalidaDecimal = MascaraSalidaDecimalDefault;
                }
            }
        }
        private Boolean _convertirDecimales = ConvertirDecimalesDefault;

        /// <summary>
        /// Indica si de debe cambiar "uno" por "un" en las unidades.
        /// </summary>
        public Boolean ApocoparUnoParteEntera
        {
            //get { return _apocoparUnoParteEntera; }
            //set { _apocoparUnoParteEntera = value; }
            get;
            set;
        }
        //private Boolean _apocoparUnoParteEntera = false;

        /// <summary>
        /// Determina si se debe apococopar el "uno" en la parte decimal
        /// </summary>
        /// <remarks>El valor de esta propiedad cambia al setear ConvertirDecimales</remarks>
        public Boolean ApocoparUnoParteDecimal
        {
            //get { return _apocoparUnoParteDecimal; }
            //set { _apocoparUnoParteDecimal = value; }
            get;
            set;
        }
        //private Boolean _apocoparUnoParteDecimal;

        #endregion

        #region Constructores

        public ConvertAmount()
        {
            MascaraSalidaDecimal = MascaraSalidaDecimalDefault;
            SeparadorDecimalSalida = SeparadorDecimalSalidaDefault;
            LetraCapital = LetraCapitalDefault;
            ConvertirDecimales = _convertirDecimales;
        }

        public ConvertAmount(Boolean ConvertirDecimales, String MascaraSalidaDecimal, String SeparadorDecimalSalida, Boolean LetraCapital)
        {
            if (!String.IsNullOrEmpty(MascaraSalidaDecimal))
            {
                this.MascaraSalidaDecimal = MascaraSalidaDecimal;
            }
            if (!String.IsNullOrEmpty(SeparadorDecimalSalida))
            {
                _separadorDecimalSalida = SeparadorDecimalSalida;
            }
            //_letraCapital = LetraCapital;
            this.LetraCapital = LetraCapital;
            _convertirDecimales = ConvertirDecimales;
        }
        #endregion

        #region Conversores de instancia

        public String ToCustomCardinal(Double Numero)
        { 
            return Convertir((Decimal)Numero, _decimales, _separadorDecimalSalida, _mascaraSalidaDecimalInterna, _esMascaraNumerica, this.LetraCapital, _convertirDecimales, this.ApocoparUnoParteEntera, this.ApocoparUnoParteDecimal); 
        }

        public String ToCustomCardinal(String Numero)
        {
            Double dNumero;
            if (Double.TryParse(Numero, NumberStyles.Float, _cultureInfo, out dNumero))
            {
                return ToCustomCardinal(dNumero);
            }
            else
            {
                throw new ArgumentException("'" + Numero + "' no es un número válido.");
            }
        }

        public String ToCustomCardinal(Decimal Numero)
        { 
            return ToCardinal((Numero)); 
        }

        public String ToCustomCardinal(Int32 Numero)
        { 
            return Convertir((Decimal)Numero, 0, _separadorDecimalSalida, _mascaraSalidaDecimalInterna, _esMascaraNumerica, this.LetraCapital, _convertirDecimales, this.ApocoparUnoParteEntera, false); 
        }

        #endregion

        #region Conversores estáticos

        public static String ToCardinal(Int32 Numero)
        {
            return Convertir((Decimal)Numero, 0, null, null, true, LetraCapitalDefault, ConvertirDecimalesDefault, ApocoparUnoParteEnteraDefault, ApocoparUnoParteDecimalDefault);
        }

        public static String ToCardinal(Double Numero)
        {
            return ToCardinal((Decimal)Numero);
        }

        public static String ToCardinal(String Numero, CultureInfo ReferenciaCultural)
        {
            Double dNumero;
            if (Double.TryParse(Numero, NumberStyles.Float, ReferenciaCultural, out dNumero))
            {
                return ToCardinal(dNumero);
            }
            else
            {
                throw new ArgumentException("'" + Numero + "' no es un número válido.");
            }
        }

        public static String ToCardinal(String Numero)
        {
            return ConvertAmount.ToCardinal(Numero, CultureInfo.CurrentCulture);
        }

        public static String ToCardinal(Decimal Numero)
        {
            return Convertir(Numero, DecimalesDefault, SeparadorDecimalSalidaDefault, MascaraSalidaDecimalDefault, true, LetraCapitalDefault, ConvertirDecimalesDefault, ApocoparUnoParteEnteraDefault, ApocoparUnoParteDecimalDefault);
        }

        #endregion

        private static String Convertir(Decimal Numero, Int32 Decimales, String SeparadorDecimalSalida, String MascaraSalidaDecimal, Boolean EsMascaraNumerica, Boolean LetraCapital, Boolean ConvertirDecimales, Boolean ApocoparUnoParteEntera, Boolean ApocoparUnoParteDecimal)
        {
            Int64 Num;
            Int32 terna, centenaTerna, decenaTerna, unidadTerna, iTerna;
            String cadTerna;
            StringBuilder Resultado = new StringBuilder();

            Num = (Int64)Math.Abs(Numero);

            if (Num >= 1000000000000 || Num < 0)
            {
                throw new ArgumentException("El número '" + Numero.ToString() + "' excedió los límites del conversor: [0;1.000.000.000.000)");
            }
            if (Num == 0)
            {
                Resultado.Append(" cero");
            }
            else
            {
                iTerna = 0;
                while (Num > 0)
                {
                    iTerna++;
                    cadTerna = string.Empty;
                    terna = (Int32)(Num % 1000);

                    centenaTerna = (Int32)(terna / 100);
                    decenaTerna = terna % 100;
                    unidadTerna = terna % 10;

                    if ((decenaTerna > 0) && (decenaTerna < 30))
                    {
                        cadTerna = _matriz[(Int32)(decenaTerna / 10), unidadTerna];
                    }
                    else if ((decenaTerna >= 30) && (decenaTerna < 100))
                    {
                        if (unidadTerna != 0)
                        {
                            cadTerna = _matriz[3, (Int32)(decenaTerna / 10)] + " y" + _matriz[UNI, unidadTerna];
                        }
                        else
                        {
                            cadTerna += _matriz[3, (Int32)(decenaTerna / 10)];
                        }
                    }

                    if(centenaTerna > 0)
                    {
                        if (decenaTerna > 0 && centenaTerna == 1)
                        {
                            cadTerna = " ciento" + cadTerna;
                        }
                        else
                        {
                            cadTerna = _matriz[4, centenaTerna] + cadTerna;
                        }
                    }

                    //Reemplazo el 'uno' por 'un' si no es en las únidades o si se solicító apocopar
                    if ((iTerna > 1 | ApocoparUnoParteEntera) && decenaTerna == 21)
                    {
                        cadTerna = cadTerna.Replace("veintiuno", "veintiun");
                    }
                    else if ((iTerna > 1 | ApocoparUnoParteEntera) && unidadTerna == 1 && decenaTerna != 11)
                    {
                        cadTerna = cadTerna.Substring(0, cadTerna.Length - 1);
                    }

                    //Completo miles y millones
                    switch (iTerna)
                    {
                        case 3:
                            if (Numero < 2000000)
                            {
                                cadTerna += " millón";
                            }
                            else
                            {
                                cadTerna += " millones";
                            }
                            break;
                        case 2:
                        case 4:
                            if (terna > 0)
                            {
                                cadTerna += " mil";
                            }
                            break;
                    }
                    Resultado.Insert(0, cadTerna);
                    Num = (Int32)(Num / 1000);
                } //while
            }

            //Se agregan los decimales si corresponde
            if (Decimales > 0)
            {
                Resultado.Append(" " + MONEY + " " + SeparadorDecimalSalida + " ");
                Int32 EnteroDecimal = (Int32)Math.Round((Double)(Numero - (Int64)Numero) * Math.Pow(10, Decimales), 0);
                if (ConvertirDecimales)
                {
                    Boolean esMascaraDecimalDefault = MascaraSalidaDecimal == MascaraSalidaDecimalDefault;
                    Resultado.Append(Convertir((Decimal)EnteroDecimal, 0, null, null, EsMascaraNumerica, false, false, (ApocoparUnoParteDecimal && !EsMascaraNumerica/*&& !esMascaraDecimalDefault*/), false) + " "
                        + (EsMascaraNumerica ? string.Empty : MascaraSalidaDecimal));
                }
                else
                {
                    if (EsMascaraNumerica)
                    {
                        Resultado.Append(EnteroDecimal.ToString(MascaraSalidaDecimal));
                    }
                    else
                    {
                        Resultado.Append(EnteroDecimal.ToString() + " " + MascaraSalidaDecimal);
                    }
                }
            }
            //Se pone la primer letra en mayúscula si corresponde y se retorna el resultado
            if (LetraCapital)
            {
                return Resultado[1].ToString().ToUpper() + Resultado.ToString(2, Resultado.Length - 2);
            }
            else
            {
                return Resultado.ToString().Substring(1);
            }
        }
    }
}
