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

        private static ASCIIEncoding codificador = new ASCIIEncoding();

        #endregion

        #region Methods

        /// <summary>
        /// Encripta la palabra especificada, este metodo de encriptacion no se puede desencriptar
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public static string Encrypting(String word)
        {
            byte[] b = System.Text.Encoding.Default.GetBytes(word);//esto es para encriptar, solo falta pasarlo a base decimal.
            return Convert.ToBase64String(b, 0, b.Length);//se convierte a String antes de hacer return
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
                byte[] encrypted;
                using (AesManaged ALG = new AesManaged())
                {
                    // Defaults
                    // CipherMode = CBC
                    // Padding = PKCS7

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

                    ICryptoTransform encryptor = ALG.CreateEncryptor();
                    encrypted = encryptor.TransformFinalBlock(data, 0, data.Length);
                }
                string result = Convert.ToBase64String(encrypted);

                return result;
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
                string result = Encoding.UTF8.GetString(decrypted);

                return result;
            }
            catch (Exception ex)
            {
                Tools.ManagerExceptions.WriteToLog("Words", "DecryptAES", ex);
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

        #endregion
    }
}
