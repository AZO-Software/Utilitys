using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AZO_Library.Tools
{
    class Words
    {
        /// <summary>
        /// Encripta la palabra especificada, este metodo de encriptacion no se puede desencriptar
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public static String Encrypting(String word)
        {
            byte[] b = System.Text.Encoding.Default.GetBytes(word);//esto es para encriptar, solo falta pasarlo a base decimal.
            return Convert.ToBase64String(b, 0, b.Length);//se convierte a String antes de hacer return
        }

        /// <summary>
        /// Desencripta una cadena encriptada por el metodo de encriptacion "AES"
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
                    ALG.Key = codificador.GetBytes(ReverseString("A0Y r3b1c y @1r3l3p@p".PadRight(32, '0')));

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
            catch (Exception e)
            {
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
    }
}
