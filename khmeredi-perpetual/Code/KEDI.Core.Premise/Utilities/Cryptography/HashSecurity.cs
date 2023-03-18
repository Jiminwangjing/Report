using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace KEDI.Core.Cryptography
{
    public class HashSecurity
    {
        public static long RandomizeNumber(int size)
        {
            const string baseNumber = "0123456789";
            string output = Randomize(size, baseNumber);
            return long.Parse(output);
        }

        public static string Randomize(int size, string baseValue)
        {
            Random rnd = new Random();
            char[] chars = new char[size];
            var keyLength = rnd.Next(size, size);
            for (int i = 0; i < keyLength; ++i)
            {
                chars[i] = baseValue[rnd.Next(baseValue.Length)];
            }
            return new string(chars);
        }

        public static byte[] RandomizeKey(int size)
        {
            using (var provider = RandomNumberGenerator.Create())
            {
                byte[] randomBytes = new byte[size];
                provider.GetBytes(randomBytes);
                return randomBytes;
            }
        }

        public static void TryCompute(string plainText, out string hash, out string salt, int saltSize = 128, int iteration = 10000, int cb = 256)
        {
            hash = Compute(plainText, out string _salt, saltSize, iteration, cb);
            salt = _salt;
        }

        public static bool Verify(string plainText, string hash, string salt, int iteration = 10000, int cb = 256)
        {
            byte[] _salt = Convert.FromBase64String(salt);
            return Verify(plainText, hash, _salt, HashAlgorithmName.SHA256, iteration, cb);
        }

        public static string Compute(string plainText, out string salt, int saltSize = 128, int iteration = 10000, int cb = 256)
        {
            string hash = Compute(plainText, out byte[] _salt, saltSize, iteration, cb);
            salt = Convert.ToBase64String(_salt);
            return hash;
        }

        public static bool Verify(string plainText, string hash, string salt, HashAlgorithmName hashAlg, int iteration = 10000, int cb = 256)
        {
            byte[] _salt = Convert.FromBase64String(salt);
            return Verify(plainText, hash, _salt, hashAlg, iteration, cb);
        }

        public static string Compute(string plainText, out byte[] salt, int saltSize = 128, int iteration = 10000, int cb = 256)
        {
            byte[] _salt = RandomizeKey(saltSize);
            string hashedPhrase = Compute(plainText, _salt, HashAlgorithmName.SHA256, iteration, cb);
            salt = _salt;
            return hashedPhrase;
        }

        public static bool Verify(string plainText, string hash, byte[] salt, int iteration = 10000, int cb = 256)
        {
            return Verify(plainText, hash, salt, HashAlgorithmName.SHA256, iteration, cb);
        }

        public static string Compute(string plainText, byte[] salt, HashAlgorithmName hashAlg, int iteration = 10000, int cb = 256)
        {
            try
            {
                var rfc2898DeriveBytes = new Rfc2898DeriveBytes(plainText, salt, iteration, hashAlg);
                var hash = Convert.ToBase64String(rfc2898DeriveBytes.GetBytes(cb));
                return hash;
            }
            catch { return string.Empty; }
        }

        public static bool Verify(string plainText, string hash, byte[] salt, HashAlgorithmName hashAlg, int iteration = 10000, int cb = 256)
        {
            return hash == Compute(plainText, salt, hashAlg, iteration, cb);
        }
    }
}
