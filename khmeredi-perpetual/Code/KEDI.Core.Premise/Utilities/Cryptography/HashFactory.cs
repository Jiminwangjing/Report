using KEDI.Core.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace KEDI.Core.Security.Cryptography
{
    public class HashFactory
    {
        static string ALPHANUMERICS => "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        static string SPECIALS => "!@#$%^&*()_-+={[}];<,>.?/";
        public static string RandomizeKey(int length, bool onlyAlphaNumerics, string optionalChars = "")
        {
            string solution = ALPHANUMERICS;
            if (!string.IsNullOrWhiteSpace(optionalChars))
            {
                if (!onlyAlphaNumerics)
                {
                    solution += SPECIALS;
                }
                solution += optionalChars;
            }

            Random rnd = new Random();
            char[] chars = new char[length];
            var keyLength = rnd.Next(length, length);
            for (int i = 0; i < keyLength; ++i)
            {
                chars[i] = solution[rnd.Next(solution.Length)];
            }

            return new string(chars);
        }

        public static string ComputeSHA256(string secretKey, params string[] values)
        {
            return Convert.ToBase64String(ComputeSHA256Bytes(secretKey, values));
        }

        public static byte[] ComputeSHA256Bytes(string secretKey, params string[] values)
        {
            byte[] secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);
            using (var hmac = new HMACSHA256(secretKeyBytes))
            {
                string combinedKey = "";
                foreach (string value in values)
                {
                    combinedKey += value;
                }
                byte[] inputBytes = Encoding.UTF8.GetBytes(combinedKey);
                byte[] hashValue = hmac.ComputeHash(inputBytes);
                return hashValue;
            }
        }

        public static string ComputeSHA384(string secretKey, params string[] values)
        {
            return Convert.ToBase64String(ComputeSHA384Bytes(secretKey, values));
        }

        public static byte[] ComputeSHA384Bytes(string secretKey, params string[] values)
        {
            byte[] secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);
            using (var hmac = new HMACSHA384(secretKeyBytes))
            {
                string combinedKey = "";
                foreach (string value in values)
                {
                    combinedKey += value;
                }
                byte[] inputBytes = Encoding.UTF8.GetBytes(combinedKey);
                byte[] hashValue = hmac.ComputeHash(inputBytes);
                return hashValue;
            }
        }

        public static string ComputeSHA512(string secretKey, params string[] values)
        {
            return Convert.ToBase64String(ComputeSHA512Bytes(secretKey, values));
        }

        public static byte[] ComputeSHA512Bytes(string secretKey, params string[] values)
        {
            byte[] secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);
            using (var hmac = new HMACSHA512(secretKeyBytes))
            {
                string combinedKey = "";
                foreach (string value in values)
                {
                    combinedKey += value;
                }
                byte[] inputBytes = Encoding.UTF8.GetBytes(combinedKey);
                byte[] hashValue = hmac.ComputeHash(inputBytes);
                return hashValue;
            }
        }

        public static byte[] RandomizeSalt(int size)
        {
            using (RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider())
            {
                byte[] randomBytes = new byte[size];
                provider.GetBytes(randomBytes);
                return randomBytes;
            }
        }

        public static string TryCompute(string clearPhrase, out string saltedPhrase, out string hashedPhrase)
        {
            string _hashedPhrase = TryCompute(clearPhrase, out string _saltedPhrase);
            saltedPhrase = _saltedPhrase;
            hashedPhrase = _hashedPhrase;
            return _hashedPhrase;
        }

        public static string TryCompute(string clearPhrase, byte[] securityToken, out string saltedPhrase, out string hashedPhrase)
        {
            string _hashedPhrase = TryCompute(clearPhrase, securityToken, out string _saltedPhrase);
            saltedPhrase = _saltedPhrase;
            hashedPhrase = _hashedPhrase;
            return _hashedPhrase;
        }

        public static string TryCompute(string clearPhrase, out string saltedPhrase)
        {
            byte[] saltBytes = RandomizeSalt(32);
            string hashedPhrase = TryCompute(clearPhrase, saltBytes, out string _saltedPhrase);
            saltedPhrase = _saltedPhrase;
            return hashedPhrase;
        }

        public static string TryCompute(string clearPhrase, byte[] securityToken, out string saltedPhrase)
        {
            string cipherText = AesFactory.Encrypt(clearPhrase, PaddingMode.PKCS7);
            var rfc2898DeriveBytes = new Rfc2898DeriveBytes(cipherText, securityToken, 10000);
            var hashedPhrase = Convert.ToBase64String(rfc2898DeriveBytes.GetBytes(128));
            saltedPhrase = AesFactory.Encrypt(Convert.ToBase64String(securityToken));
            return hashedPhrase;
        }

        public static bool Verify(string clearPhrase, string hashedPhrase, string saltedPhrase)
        {
            try
            {
                byte[] salt = Convert.FromBase64String(AesFactory.Decrypt(saltedPhrase));
                var rfc2898DeriveBytes = new Rfc2898DeriveBytes(AesFactory.Encrypt(clearPhrase, PaddingMode.PKCS7), salt, 10000);
                return Convert.ToBase64String(rfc2898DeriveBytes.GetBytes(128)) == hashedPhrase;
            }
            catch
            {
                return false;
            }         
        }
    }
}
