using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace KEDI.Core.Security.Cryptography
{
    public class RsaLite
    {       
        public static RSA Create()
        {
            return new RSACryptoServiceProvider();
        }

        //Export keys in bytes XML string format
        public static bool TryExportKeys(out string privateKeyXml, out string publicKeyXml)
        {
            using (var rsa = Create())
            {
                privateKeyXml = rsa.ToXmlString(true);
                publicKeyXml = rsa.ToXmlString(false);
                return !privateKeyXml.Equals(publicKeyXml);
            }
        }

        //Export keys in parameter object format
        public static bool TryExportKeys(out RSAParameters privateKeyParam, out RSAParameters publicKeyParam)
        {
            using (var rsa = Create())
            {
                privateKeyParam = rsa.ExportParameters(true);
                publicKeyParam = rsa.ExportParameters(false);
                return !privateKeyParam.Equals(publicKeyParam);
            }
        }

        public static RSA ImportKeyXml(string keyXmlFormat)
        {
            var rsa = Create();
            rsa.FromXmlString(keyXmlFormat);
            return rsa;
        }       

        public static RSA ImportParameters(RSAParameters keysParam)
        {
            var rsa = Create();
            rsa.ImportParameters(keysParam);
            return rsa;
        }

        //Encrypt Decrypt
        public static string Encrypt(string plaintext, string publicKeyXml)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(plaintext)) { return string.Empty; }
                using (var rsaWrite = new RSACryptoServiceProvider())
                {
                    rsaWrite.FromXmlString(publicKeyXml);
                    byte[] plainBytes = Encoding.Unicode.GetBytes(plaintext);
                    byte[] cipherBytes = rsaWrite.Encrypt(plainBytes, false);
                    return Convert.ToBase64String(cipherBytes);
                }
            }
            catch (Exception ex) { return ex.Message; }
        }

        public static string Decrypt(string cipherText, string privateKeyXml)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(cipherText)) { return string.Empty; }
                using (var rsaRead = new RSACryptoServiceProvider())
                {
                    rsaRead.FromXmlString(privateKeyXml);
                    byte[] cipherBytes = Convert.FromBase64String(cipherText);
                    byte[] plainBytes = rsaRead.Decrypt(cipherBytes, false);
                    return Encoding.Unicode.GetString(plainBytes);
                }

            }
            catch (Exception ex) { return ex.Message; }
        }

        public static string Encrypt(string plaintext, RSAParameters publicKeyParam)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(plaintext)) { return string.Empty; }
                using (var rsaWrite = new RSACryptoServiceProvider())
                {
                    rsaWrite.ImportParameters(publicKeyParam);
                    byte[] plainBytes = Encoding.Unicode.GetBytes(plaintext);
                    byte[] cipherBytes = rsaWrite.Encrypt(plainBytes, false);
                    return Convert.ToBase64String(cipherBytes);
                }
            }
            catch (Exception ex) { return ex.Message; }
        }

        public static string Decrypt(string cipherText, RSAParameters privateKeyParam)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(cipherText)) { return string.Empty; }
                using (var rsaRead = new RSACryptoServiceProvider())
                {
                    rsaRead.ImportParameters(privateKeyParam);
                    byte[] cipherBytes = Convert.FromBase64String(cipherText);
                    byte[] plainBytes = rsaRead.Decrypt(cipherBytes, false);
                    return Encoding.Unicode.GetString(plainBytes);
                }
            }
            catch (Exception ex) { return ex.Message; }
        }

        //Sign & verify data with specified hash algorithm in XML format keys
        public static string SignData(string plainData, string privateKeyXml, object halg)
        {
            using (var rsaWrite = new RSACryptoServiceProvider())
            {
                rsaWrite.FromXmlString(privateKeyXml);
                byte[] plainBytes = Encoding.Unicode.GetBytes(plainData);
                byte[] signedBytes = rsaWrite.SignData(plainBytes, halg);
                return Convert.ToBase64String(signedBytes);
            }
        }

        public static bool VerifyData(string plainData, string signedData, string publicKeyXml, object halg)
        {
            using (var rsaRead = new RSACryptoServiceProvider())
            {
                rsaRead.FromXmlString(publicKeyXml);
                byte[] plainBytes = Encoding.Unicode.GetBytes(plainData);
                byte[] signature = Convert.FromBase64String(signedData);
                return rsaRead.VerifyData(plainBytes, halg, signature);
            }
        }

        //Sign & verify data with specified hash algorithm in parameter object format keys
        public static string SignData(string plainData, RSAParameters privateKeyParam, object halg)
        {
            using (var rsaWrite = new RSACryptoServiceProvider())
            {
                rsaWrite.ImportParameters(privateKeyParam);
                byte[] plainBytes = Encoding.Unicode.GetBytes(plainData);
                byte[] signedBytes = rsaWrite.SignData(plainBytes, halg);
                return Convert.ToBase64String(signedBytes);
            }
        }

        public static bool VerifyData(string plainData, string signedData, RSAParameters publicKeyParam, object halg)
        {
            using (var rsaRead = new RSACryptoServiceProvider())
            {
                rsaRead.ImportParameters(publicKeyParam);
                byte[] plainBytes = Encoding.Unicode.GetBytes(plainData);
                byte[] signature = Convert.FromBase64String(signedData);
                return rsaRead.VerifyData(plainBytes, halg, signature);
            }
        }

        //SHA-256 XML string format keys
        public static string SignDataSHA256(string plainData, string privateKeyXml)
        {
            return SignData(plainData, privateKeyXml, new SHA256CryptoServiceProvider());
        }

        public static bool VerifyDataSHA256(string plainData, string signedData, string publicKeyXml)
        {
            return VerifyData(plainData, signedData, publicKeyXml, new SHA256CryptoServiceProvider());
        }

        //SHA-256 parameter object format keys
        public static string SignDataSHA256(string plainData, RSAParameters privateKeyParam)
        {
            return SignData(plainData, privateKeyParam, new SHA256CryptoServiceProvider());
        }

        public static bool VerifyDataSHA256(string plainData, string signedData, RSAParameters publicKeyParam)
        {
            return VerifyData(plainData, signedData, publicKeyParam, new SHA256CryptoServiceProvider());
        }

        //SHA-384 in XML string format keys
        public static string SignDataSHA384(string plainData, string privateKeyXml)
        {
            return SignData(plainData, privateKeyXml, new SHA384CryptoServiceProvider());
        }

        public static bool VerifyDataSHA384(string plainData, string signedData, string publicKeyXml)
        {
            return VerifyData(plainData, signedData, publicKeyXml, new SHA384CryptoServiceProvider());
        }

        //SHA-384 in parameter object format keys
        public static string SignDataSHA384(string plainData, RSAParameters privateKeyParam)
        {
            return SignData(plainData, privateKeyParam, new SHA384CryptoServiceProvider());
        }

        public static bool VerifyDataSHA384(string plainData, string signedData, RSAParameters publicKeyParam)
        {
            return VerifyData(plainData, signedData, publicKeyParam, new SHA384CryptoServiceProvider());
        }

        //SHA-512 in XML string format keys
        public static string SignDataSHA512(string plainData, string privateKeyXml)
        {
            return SignData(plainData, privateKeyXml, new SHA512CryptoServiceProvider());
        }

        public static bool VerifyDataSHA512(string plainData, string signedData, string publicKeyXml)
        {
            return VerifyData(plainData, signedData, publicKeyXml, new SHA512CryptoServiceProvider());
        }

        //SHA-512 in parameter object format keys
        public static string SignDataSHA512(string plainData, RSAParameters privateKeyParam)
        {
            return SignData(plainData, privateKeyParam, new SHA512CryptoServiceProvider());
        }

        public static bool VerifyDataSHA512(string plainData, string signedData, RSAParameters publicKeyParam)
        {
            return VerifyData(plainData, signedData, publicKeyParam, new SHA512CryptoServiceProvider());
        }
    }
}
