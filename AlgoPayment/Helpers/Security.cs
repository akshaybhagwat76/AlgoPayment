using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace AlgoPayment.Helpers
{
    public class Security
    {
        private const string rgbIVValue = "xovhlHXMkxezprx!";
        private const string keyValue = "mpsjinhyexcqmtgjejxoerqnosipsibm";
        private const string chars = "ABCDEFGHIJKLMNPQRSTUVWXYZ123456789";
        private const int textLength = 6;
        public static string Encrypt(string inputText)
        {
            if (String.IsNullOrWhiteSpace(inputText))
                inputText = "";

            byte[] clearTextBytes = Encoding.UTF8.GetBytes(inputText);

            System.Security.Cryptography.SymmetricAlgorithm rijn = SymmetricAlgorithm.Create();

            MemoryStream ms = new MemoryStream();
            byte[] rgbIV = Encoding.ASCII.GetBytes(rgbIVValue);
            byte[] key = Encoding.ASCII.GetBytes(keyValue);
            CryptoStream cs = new CryptoStream(ms, rijn.CreateEncryptor(key, rgbIV), CryptoStreamMode.Write);

            cs.Write(clearTextBytes, 0, clearTextBytes.Length);

            cs.Close();

            return Convert.ToBase64String(ms.ToArray());
        }

        public static string Decrypt(string encryptedText)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(encryptedText))
                    return "";

                byte[] encryptedTextBytes = Convert.FromBase64String(encryptedText);

                MemoryStream ms = new MemoryStream();

                System.Security.Cryptography.SymmetricAlgorithm rijn = SymmetricAlgorithm.Create();


                byte[] rgbIV = Encoding.ASCII.GetBytes(rgbIVValue);
                byte[] key = Encoding.ASCII.GetBytes(keyValue);

                CryptoStream cs = new CryptoStream(ms, rijn.CreateDecryptor(key, rgbIV),
                CryptoStreamMode.Write);

                cs.Write(encryptedTextBytes, 0, encryptedTextBytes.Length);

                cs.Close();

                return Encoding.UTF8.GetString(ms.ToArray());
            }
            catch (Exception ex)
            {
                return encryptedText;
            }
        }
    }
}