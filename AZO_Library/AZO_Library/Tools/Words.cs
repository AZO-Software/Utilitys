using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AZO_Library.Tools
{
    /// <summary>
    /// Operaciones que se realizan exclusivamente sobre cadenas de texto
    /// </summary>
    public class Words
    {
        #region Properties

        /// <summary>
        /// Establece la clave secreta utilizada en el algoritmo
        /// </summary>
        public static string Key { private get; set; }//"A0Y r3b1c y @1r3l3p@p";

        #endregion

        #region Globals

        private static ASCIIEncoding encoding = new ASCIIEncoding();

        #endregion

        #region Methods

        /// <summary>
        /// Encripta la palabra especificada, este metodo de encriptacion no se puede desencriptar
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public static string EncryptDefault(String word)
        {
            //esto es para encriptar, solo falta pasarlo a base decimal.
            byte[] b = Encoding.Default.GetBytes(word);
            //se convierte a String antes de hacer return
            return Convert.ToBase64String(b, 0, b.Length);
        }

        /// <summary>
        /// Encripta una cadena por el metodo de encriptacion "AES"
        /// </summary>
        /// <param name="dataEncode"></param>
        /// <returns></returns>
        public static string EncryptAES(string dataEncode)
        {
            byte[] data = Encoding.UTF8.GetBytes(dataEncode);

            if (data == null || data.Length <= 0)
            {
                throw new ArgumentNullException("data");
            }
            try
            {
                //variable que contendra la informacion recien encriptada
                byte[] encrypted;

                using (AesManaged ALG = new AesManaged())
                {
                    // Defaults
                    // CipherMode = CBC
                    // Padding = PKCS7

                    //configuracion del AESManaged encargado de crear el objeto que encriptara la informacion
                    ALG.KeySize = 128;
                    ALG.BlockSize = 128;
                    ALG.Key = encoding.GetBytes(ReverseString(Key.PadRight(32, '0')));

                    using (var cryptoProvider = new SHA1CryptoServiceProvider())
                    {
                        byte[] aux = cryptoProvider.ComputeHash(ALG.Key);
                        byte[] iv = new byte[16];
                        for (int i = 0; i < iv.Length; i++)
                        {
                            iv[i] = aux[i];
                        }
                        ALG.IV = iv;
                    }

                    //generamos el objeto que realizara la encriptacion final
                    ICryptoTransform encryptor = ALG.CreateEncryptor();
                    //se realiza la encriptacion AES de los datos ingresados
                    encrypted = encryptor.TransformFinalBlock(data, 0, data.Length);
                }

                //convertimos el arreglo con la informacion encriptada a una cadena string
                return Convert.ToBase64String(encrypted);
            }
            catch (Exception ex)
            {
                Tools.ManagerExceptions.WriteToLog("Words", "EncryptAES(string)", ex);
                return null;
            }
        }

        /// <summary>
        /// Desencripta una cadena previamente encriptada por el metodo de encriptacion "AES"
        /// </summary>
        /// <param name="dataDecrypt"></param>
        /// <returns></returns>
        public static string DecryptAES(string dataDecrypt)
        {
            System.Text.ASCIIEncoding codificador = new System.Text.ASCIIEncoding();
            byte[] cipherText = Convert.FromBase64String(dataDecrypt);

            try
            {
                //arreglo que contendra la informacion desencriptada
                byte[] decrypted;

                using (AesManaged ALG = new AesManaged())
                {
                    ALG.KeySize = 128;
                    ALG.BlockSize = 128;
                    ALG.Key = codificador.GetBytes(ReverseString(Key.PadRight(32, '0')));

                    using (var cryptoProvider = new SHA1CryptoServiceProvider())
                    {
                        byte[] aux = cryptoProvider.ComputeHash(ALG.Key);
                        byte[] iv = new byte[16];
                        for (int i = 0; i < iv.Length; i++)
                        {
                            iv[i] = aux[i];
                        }
                        ALG.IV = iv;
                    }

                    ICryptoTransform decryptor = ALG.CreateDecryptor();
                    decrypted = decryptor.TransformFinalBlock(cipherText, 0, cipherText.Length);
                }

                return Encoding.UTF8.GetString(decrypted);
            }
            catch (Exception ex)
            {
                Tools.ManagerExceptions.WriteToLog("Words", "DecryptAES(string)", ex);
                return null;
            }
        }

        /// <summary>
        /// Invierte el orden de los caracteres de una cadena
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ReverseString(string s)
        {
            char[] arr = s.ToCharArray();
            Array.Reverse(arr);
            return new string(arr);
        }

        /// <summary>
        /// Quita las vocales existentes en una cadena
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public static string DeleteVowels(string word)
        {
            string[] vowels = { "A", "a", "E", "e", "I", "i", "O", "o", "U", "u"};

            foreach (string vowel in vowels)
            {
                word = word.Replace(vowel, string.Empty);
            }

            return word;
        }

        /// <summary>
        /// Genera un objeto AesManaged con la configuracion inicial ya establecida
        /// </summary>
        /// <returns></returns>
        private static AesManaged CreateAesManaged()
        {
            using (AesManaged ALG = new AesManaged())
            {

                //configuracion del AESManaged encargado de crear el objeto que encriptara la informacion
                ALG.KeySize = 128;
                ALG.BlockSize = 128;
                ALG.Key = encoding.GetBytes(ReverseString(Key.PadRight(32, '0')));

                using (var cryptoProvider = new SHA1CryptoServiceProvider())
                {
                    byte[] aux = cryptoProvider.ComputeHash(ALG.Key);
                    byte[] iv = new byte[16];
                    for (int i = 0; i < iv.Length; i++)
                    {
                        iv[i] = aux[i];
                    }
                    ALG.IV = iv;
                }

                return ALG;
            }
        }

        #endregion
    }
}
