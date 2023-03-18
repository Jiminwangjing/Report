using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace KEDI.Core.Security.Cryptography
{
    public class AesFactory
    {
        static string SECRET_KEY => "{#/=?0+1-2*3&4%5#[(-!-)]#6,7;8@9$?=/#}";      
        static byte[] SALT => new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 };
        public static string Encrypt(string clearText, string secretKey, PaddingMode paddingMode = PaddingMode.ISO10126)
        {
             try
             {
                if (string.IsNullOrWhiteSpace(clearText)) { return string.Empty; }
                byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
                using (Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(secretKey, SALT, 10000))
                {
                    using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
                    {                              
                        aes.Key = pdb.GetBytes(aes.KeySize/8);
                        aes.IV = pdb.GetBytes(aes.BlockSize/8);
                        aes.Padding = paddingMode;
                        byte[] cipherBytes = aes.CreateEncryptor().TransformFinalBlock(clearBytes, 0, clearBytes.Length);
                        return Convert.ToBase64String(cipherBytes);                          
                    }
                }                  
             } catch (Exception e) { return e.Message; }
             
        }

        public static string Decrypt(string cipherText, string secretKey, PaddingMode paddingMode = PaddingMode.ISO10126)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(cipherText)) { return string.Empty; }
                byte[] cipherBytes = Convert.FromBase64String(cipherText);
                using (Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(secretKey, SALT, 10000))
                {
                    using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
                    {
                        aes.Key = pdb.GetBytes(aes.KeySize / 8);
                        aes.IV = pdb.GetBytes(aes.BlockSize / 8);
                        aes.Padding = paddingMode;
                        byte[] clearBytes = aes.CreateDecryptor().TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
                        return Encoding.Unicode.GetString(clearBytes);
                    }
                }
            } catch (Exception e) { return e.Message; }
        }

        public static string Encrypt(string clearText, PaddingMode paddingMode = PaddingMode.ISO10126)
        {
            return Encrypt(clearText, SECRET_KEY, paddingMode);
        }

        public static string Decrypt(string cipherText, PaddingMode paddingMode = PaddingMode.ISO10126)
        {
            return Decrypt(cipherText, SECRET_KEY, paddingMode);
        }
    }
}
