using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZO_Library.Tools
{
    public class ConsoleTools
    {
        #region Globals

        public static int TextLength = 78;
        public static char Character = '*';
        public static string BusinessName { get; set;}

        public static ConsoleColor WarningColor = ConsoleColor.Yellow;
        public static ConsoleColor TextColor = ConsoleColor.DarkCyan;
        public static ConsoleColor ReadTextColor = ConsoleColor.White;
        public static ConsoleColor HighlighTextColor = ConsoleColor.DarkRed;

        #endregion Globals

        #region Methods

        /// <summary>
        /// Da formato a la cadena especificada cuando no cumple con la longitud indicada
        /// </summary>
        /// <param name="text">Texto al que se le dara formato en caso de ser necesario</param>
        /// <param name="length">longitud que debe de tener el texto para cumplir con el formato</param>
        /// <returns></returns>
        public static string InsertFormat(string text, int length)
        {
            //almacena la diferencia entre la longitud que debe tener la cadena y la longitud actual de la misma
            int difference = length - text.Length;
            //almacena el texto con el formato ya aplicado
            string result = string.Empty;

            //verifico si el texto ya cumple con la longitud requerida
            if (difference > 0)
            {
                //ingreso los caracteres del lado izquierdo
                //se divide entre 2 para poner la misma cantidad de caracteres de ambos lados
                result = text.PadLeft(text.Length + (int)(difference / 2), Character);
                //ingreso los caracteres del lado derecho
                result = result.PadRight(result.Length + (int)(difference / 2), Character);
            }

            return result;
        }

        /// <summary>
        /// Escribe el encabezado en la consola, dandole formato requerido
        /// </summary>
        /// <param name="title"></param>
        public static void WriteTitle(string title)
        {
            //longitud que debe de tener la cadena
            //int textLength = 78;

            Console.Clear();
            Console.ForegroundColor = TextColor;
            Console.WriteLine(InsertFormat(string.Empty, TextLength));
            Console.WriteLine(InsertFormat(BusinessName, TextLength));
            Console.WriteLine(InsertFormat(string.Format(" {0} ", title), TextLength));
            Console.WriteLine(InsertFormat(string.Empty, TextLength));
            Console.WriteLine();
        }

        /// <summary>
        /// Captura una linea de informacion de la consola
        /// </summary>
        /// <returns></returns>
        public static string GetInformation()
        {
            Console.ForegroundColor = ReadTextColor;
            string information = Console.ReadLine();
            Console.ForegroundColor = TextColor;

            return information;
        }

        /// <summary>
        /// Resalta el texto especificado
        /// </summary>
        /// <param name="text"></param>
        public static void HighlighText(string text)
        {
            Console.ForegroundColor = HighlighTextColor;
            Console.WriteLine(text);
            Console.ForegroundColor = TextColor;
        }

        /// <summary>
        /// Escribe un mensaje de advertencia en la consola con un formato especifico
        /// </summary>
        /// <param name="text"></param>
        public static void WriteWarning(string text)
        {
            Console.ForegroundColor = WarningColor;
            Console.WriteLine();
            Console.WriteLine(text);
            Console.ReadKey(false);
            Console.WriteLine();
            Console.ForegroundColor = TextColor;
        }

        /// <summary>
        /// Ingresa un menu mostrando las opciones especificadas
        /// </summary>
        /// <param name="options"></param>
        public static void WriteMenu(string[] options)
        {
            int index = 1;
            Console.WriteLine("Opciones:");
            foreach(string option in options)
            {
                Console.WriteLine(string.Format("{0}.-  {1}", index, option));
                index++;
            }
            Console.WriteLine();
            Console.WriteLine("Seleccione una Opcion y Presione[ENTER] ...");
        }

        public static void ExecuteCommand(string command)
        {
            //Indicamos que deseamos inicializar el proceso cmd.exe junto a un comando de arranque. 
            //(/C, le indicamos al proceso cmd que deseamos que cuando termine la tarea asignada se cierre el proceso).
            //Para mas informacion consulte la ayuda de la consola con cmd.exe /? 
            System.Diagnostics.ProcessStartInfo procStartInfo = new System.Diagnostics.ProcessStartInfo("cmd", "/c " + command);
            // Indicamos que la salida del proceso se redireccione en un Stream
            procStartInfo.RedirectStandardOutput = true;
            procStartInfo.UseShellExecute = false;
            //Indica que el proceso no despliegue una pantalla negra (El proceso se ejecuta en background)
            procStartInfo.CreateNoWindow = false;
            //Inicializa el proceso
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.StartInfo = procStartInfo;
            proc.Start();
            //Consigue la salida de la Consola(Stream) y devuelve una cadena de texto
            string result = proc.StandardOutput.ReadToEnd();
            //Muestra en pantalla la salida del Comando
            Console.WriteLine(result);
        }


        ///// <summary>
        ///// Muestra un submenu que permite mostrar la informacion de un recibo
        ///// </summary>
        ///// <param name="title"></param>
        ///// <returns></returns>
        //private static Receipt GetInformation(string title)
        //{
        //    string response = string.Empty;
        //    Receipt receipt = null;
        //    ConsoleKeyInfo key;

        //    do
        //    {
        //        Console.Write(title);
        //        response = GetInformation();

        //        //relizamos la busqueda solo si se ingreso un folio valido, de no ser asi se mantiene en null la variable
        //        if (AZO_Library.ControlUtilitys.ValidateControlText.IsDigit(response))
        //        {
        //            receipt = DataBase.CyberAdministrator.GetReceipt(response);
        //        }

        //        if (receipt != null)
        //        {
        //            ShowReceipt(receipt);
        //            Console.WriteLine("Deseas continuar con la operacion?");
        //            Console.WriteLine("Presiona [Enter] para si o Cualquier otra tecla para salir ...");
        //            key = Console.ReadKey(false);
        //            response = key.KeyChar.ToString();

        //            //entra cuando el usuario presiona una tecla distinta a [Enter]
        //            if (response != "\r")
        //            {
        //                //se asigna null porque se cancelo la operacion
        //                receipt = null;
        //                //se asigna el valor de [Enter] para que salga del siclo
        //                response = "\r";
        //            }
        //        }
        //        else
        //        {
        //            Console.WriteLine();
        //            Console.ForegroundColor = ConsoleColor.Yellow;
        //            Console.WriteLine("No se encontro ningun recibo con el folio ingresado ...");
        //            Console.WriteLine("Presiona [Enter] para volverlo a intentar o Cualquier otra tecla para salir ...");
        //            Console.ForegroundColor = ConsoleColor.DarkCyan;
        //            key = Console.ReadKey(false);

        //            if (key.KeyChar == '\r')
        //            {
        //                //cambiamos el valor a cadena vacia para que no salga del ciclo
        //                response = string.Empty;
        //            }
        //            else
        //            {
        //                //se asigna el valor de [Enter] para que salga del siclo
        //                response = "\r";
        //            }
        //            Console.WriteLine();
        //        }
        //    }
        //    while (!response.ToUpper().Equals("\r"));

        //    return receipt;
        //}

        ///// <summary>
        ///// Muestra la informacion de un Recibo en la consola
        ///// </summary>
        ///// <param name="receipt"></param>
        //private static void ShowReceipt(Receipt receipt)
        //{
        //    Console.WriteLine();
        //    Console.WriteLine("Informacion del Recibo");
        //    Console.Write("FOLIO: ");
        //    Console.ForegroundColor = ConsoleColor.White;
        //    Console.WriteLine(receipt.Id);
        //    Console.ForegroundColor = ConsoleColor.DarkCyan;
        //    Console.Write("FECHA: ");
        //    Console.ForegroundColor = ConsoleColor.White;
        //    Console.WriteLine(receipt.Date);
        //    Console.ForegroundColor = ConsoleColor.DarkCyan;
        //    Console.Write("AUTORIZO: ");
        //    Console.ForegroundColor = ConsoleColor.White;
        //    Console.WriteLine(receipt.Username);
        //    Console.ForegroundColor = ConsoleColor.DarkCyan;
        //    Console.Write("RECIBI DE: ");
        //    Console.ForegroundColor = ConsoleColor.White;
        //    Console.WriteLine(receipt.Name);
        //    Console.ForegroundColor = ConsoleColor.DarkCyan;
        //    Console.Write("MONTO: ");
        //    Console.ForegroundColor = ConsoleColor.White;
        //    Console.WriteLine("$" + receipt.Amount);
        //    Console.ForegroundColor = ConsoleColor.DarkCyan;
        //    Console.Write("NOTAS: ");
        //    Console.ForegroundColor = ConsoleColor.White;
        //    Console.WriteLine(receipt.Notes);
        //    Console.ForegroundColor = ConsoleColor.DarkCyan;
        //    Console.Write("ESTADO ACTUAL: ");
        //    Console.ForegroundColor = ConsoleColor.White;

        //    if (receipt.Status == Receipt.STATUS_GENERATE)
        //    {
        //        Console.WriteLine("GENERADO");
        //    }
        //    else if (receipt.Status == Receipt.STATUS_REGISTRY)
        //    {
        //        Console.WriteLine("REGISTRADO");
        //    }
        //    else
        //    {
        //        Console.WriteLine("CANCELADO");
        //    }
        //    Console.ForegroundColor = ConsoleColor.DarkCyan;
        //    Console.WriteLine();
        //}

        #endregion
    }
}
