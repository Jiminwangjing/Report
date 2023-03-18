using System;
using System.Security.Cryptography;
using System.Text;

namespace KEDI.Core.Cryptography
{
    public class AesSecurity
    {
        static byte[] _salt => new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 };

        //Encryption
        public static string Encrypt(string plainText, string password, int iteration = 10000, PaddingMode paddingMode = PaddingMode.PKCS7)
        {
            if (string.IsNullOrWhiteSpace(plainText)) { return string.Empty; }
            return Encrypt(plainText, password, HashAlgorithmName.SHA256, iteration, paddingMode);
        }

        public static string Encrypt(string plainText, string password, HashAlgorithmName hashAlgo, int iteration = 10000, PaddingMode paddingMode = PaddingMode.PKCS7)
        {
            if (string.IsNullOrWhiteSpace(plainText)) { return string.Empty; }
            byte[] clearBytes = Encoding.Unicode.GetBytes(plainText);
            byte[] passwordBytes = Encoding.Unicode.GetBytes(password);
            return Encrypt(clearBytes, passwordBytes, _salt, hashAlgo, iteration, paddingMode);
        }
        public static string Encrypt(string plainText, string password, string salt, int iteration = 10000, PaddingMode paddingMode = PaddingMode.PKCS7)
        {
            if (string.IsNullOrWhiteSpace(plainText)) { return string.Empty; }
            return Encrypt(plainText, password, salt, HashAlgorithmName.SHA256, iteration, paddingMode);
        }

        public static string Encrypt(string plainText, string password, string salt, HashAlgorithmName hashAlgo, int iteration = 10000, PaddingMode paddingMode = PaddingMode.PKCS7)
        {
            if (string.IsNullOrWhiteSpace(plainText)) { return string.Empty; }
            byte[] clearBytes = Encoding.Unicode.GetBytes(plainText);
            byte[] passwordBytes = Encoding.Unicode.GetBytes(password);
            byte[] saltBytes = Encoding.Unicode.GetBytes(salt);
            return Encrypt(clearBytes, passwordBytes, saltBytes, hashAlgo, iteration, paddingMode);
        }

        public static string Encrypt(byte[] plainValue, byte[] password, int iteration = 10000, PaddingMode paddingMode = PaddingMode.PKCS7)
        {
            return Encrypt(plainValue, password, HashAlgorithmName.SHA256, iteration, paddingMode);
        }

        public static string Encrypt(byte[] plainValue, byte[] password, HashAlgorithmName hashAlg, int iteration = 10000, PaddingMode paddingMode = PaddingMode.PKCS7)
        {
            return Encrypt(plainValue, password, _salt, hashAlg, iteration, paddingMode);
        }

        public static string Encrypt(byte[] plainValue, byte[] password, byte[] salt, int iteration = 10000, PaddingMode paddingMode = PaddingMode.PKCS7)
        {
            return Encrypt(plainValue, password, salt, HashAlgorithmName.SHA256, iteration, paddingMode);
        }

        public static string Encrypt(byte[] plainValue, byte[] password, byte[] salt, HashAlgorithmName hashAlg, int iteration = 10000, PaddingMode paddingMode = PaddingMode.PKCS7)
        {
            try
            {
                using (Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(password, salt, iteration, hashAlg))
                {
                    using (var aes = Aes.Create())
                    {
                        aes.Key = pdb.GetBytes(aes.KeySize / 8);
                        aes.IV = pdb.GetBytes(aes.BlockSize / 8);
                        aes.Padding = paddingMode;
                        ICryptoTransform encryptor = aes.CreateEncryptor();
                        byte[] cipherBytes = encryptor.TransformFinalBlock(plainValue, 0, plainValue.Length);
                        return Convert.ToBase64String(cipherBytes);
                    }
                }
            }
            catch { return string.Empty; }
        }

        //Decryption
        public static string Decrypt(string cipherText, string password, int iteration = 10000, PaddingMode paddingMode = PaddingMode.PKCS7)
        {
            if (string.IsNullOrWhiteSpace(cipherText)) { return string.Empty; }
            return Decrypt(cipherText, password, HashAlgorithmName.SHA256, iteration, paddingMode);
        }

        public static string Decrypt(string cipherText, string password, HashAlgorithmName hashAlg, int iteration = 10000, PaddingMode paddingMode = PaddingMode.PKCS7)
        {
           try {
                if (string.IsNullOrWhiteSpace(cipherText)) { return string.Empty; }
                byte[] cipherBytes = Convert.FromBase64String(cipherText);
                byte[] passwordBytes = Encoding.Unicode.GetBytes(password);
                return Decrypt(cipherBytes, passwordBytes, _salt, hashAlg, iteration, paddingMode);
           } catch { return string.Empty; }
        }

        public static string Decrypt(string cipherText, string password, string salt, int iteration = 10000, PaddingMode paddingMode = PaddingMode.PKCS7)
        {
            if (string.IsNullOrWhiteSpace(cipherText)) { return string.Empty; }
            return Decrypt(cipherText, password, salt, HashAlgorithmName.SHA256, iteration, paddingMode);
        }

        public static string Decrypt(string cipherText, string password, string salt, HashAlgorithmName hashAlg, int iteration = 10000, PaddingMode paddingMode = PaddingMode.PKCS7)
        {
            if (string.IsNullOrWhiteSpace(cipherText)) { return string.Empty; }
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            byte[] passwordBytes = Encoding.Unicode.GetBytes(password);
            byte[] saltBytes = Encoding.Unicode.GetBytes(salt);
            return Decrypt(cipherBytes, passwordBytes, saltBytes, hashAlg, iteration, paddingMode);
        }

        public static string Decrypt(byte[] cipherValue, byte[] password, int iteration = 10000, PaddingMode paddingMode = PaddingMode.PKCS7)
        {
            return Decrypt(cipherValue, password, HashAlgorithmName.SHA256, iteration, paddingMode);
        }

        public static string Decrypt(byte[] cipherValue, byte[] password, HashAlgorithmName hashAlg, int iteration = 10000, PaddingMode paddingMode = PaddingMode.PKCS7)
        {
            return Decrypt(cipherValue, password, _salt, hashAlg, iteration, paddingMode);
        }

        public static string Decrypt(byte[] cipherValue, byte[] password, byte[] salt, int iteration = 10000, PaddingMode paddingMode = PaddingMode.PKCS7)
        {
            return Decrypt(cipherValue, password, salt, HashAlgorithmName.SHA256, iteration, paddingMode);
        }

        public static string Decrypt(byte[] cipherValue, byte[] password, byte[] salt, HashAlgorithmName hashAlg, int iteration = 10000, PaddingMode paddingMode = PaddingMode.PKCS7)
        {
            try
            {
                using (Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(password, salt, iteration, hashAlg))
                {
                    using (var aes = Aes.Create())
                    {
                        aes.Key = pdb.GetBytes(aes.KeySize / 8);
                        aes.IV = pdb.GetBytes(aes.BlockSize / 8);
                        aes.Padding = paddingMode;
                        ICryptoTransform decryptor = aes.CreateDecryptor();
                        byte[] plainBytes = decryptor.TransformFinalBlock(cipherValue, 0, cipherValue.Length);
                        return Encoding.Unicode.GetString(plainBytes);
                    }
                }
            }
            catch { return string.Empty; }
        }
    }
}
