using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace KEDI.Core.Cryptography
{
    public class RsaSecurity
    {
       public static RSA Create()
       {
           return RSA.Create();
       }
        public static RSACryptoServiceProvider CreateCsp()
        {
            return new RSACryptoServiceProvider();
        }

        public static string ExportEncryptedPkcs8PrivateKey(ReadOnlySpan<char> password,
            PbeEncryptionAlgorithm pbeAlg = PbeEncryptionAlgorithm.Aes256Cbc, int iteration = 10000)
        {
            return ExportEncryptedPkcs8PrivateKey(password, HashAlgorithmName.SHA256, pbeAlg, iteration);
        }

        public static string ExportEncryptedPkcs8PrivateKey(ReadOnlySpan<char> password, HashAlgorithmName algName, 
            PbeEncryptionAlgorithm pbeAlg = PbeEncryptionAlgorithm.Aes256Cbc, int iteration = 10000)
        {
            using (var rsa = CreateCsp())
            {
                byte[] privateKeyBytes = rsa.ExportEncryptedPkcs8PrivateKey(password,
                    new PbeParameters(pbeAlg, algName, iteration));
                return Convert.ToBase64String(privateKeyBytes);
            }
        }

        //Export keys in bytes XML string format
        public static bool TryExportKeys(out string privateKeyXml, out string publicKeyXml)
        {
            using (var rsa = CreateCsp())
            {
                privateKeyXml = rsa.ToXmlString(true);
                publicKeyXml = rsa.ToXmlString(false);
                return !privateKeyXml.Equals(publicKeyXml);
            }
        }

        //Export keys in bytes array format
        public static bool TryExportKeys(out byte[] privateKey, out byte[] publicKey)
        {
            using (var rsa = CreateCsp())
            {
                privateKey = rsa.ExportRSAPrivateKey();
                publicKey = rsa.ExportRSAPublicKey();
                return !privateKey.Equals(publicKey);
            }
        }

        //Export keys in parameter object format
        public static bool TryExport(out RSAParameters privateKeyParam, out RSAParameters publicKeyParam)
        {
            using (var rsa = CreateCsp())
            {
                privateKeyParam = rsa.ExportParameters(true);
                publicKeyParam = rsa.ExportParameters(false);
                return !privateKeyParam.Equals(publicKeyParam);
            }
        }

        public static string Encrypt(ReadOnlySpan<char> plainText, ReadOnlySpan<char> password,
           ReadOnlySpan<char> encryptedPkcs8PrivateKey, RSAEncryptionPadding padding = null!)
        {
            ReadOnlySpan<byte> _encryptedPkcs8 = Convert.FromBase64String(encryptedPkcs8PrivateKey.ToString());
            return Encrypt(plainText, password, _encryptedPkcs8, padding);
        }

        public static string Decrypt(ReadOnlySpan<char> cipherText, ReadOnlySpan<char> password,
            ReadOnlySpan<char> encryptedPkcs8PrivateKey, RSAEncryptionPadding padding = null!)
        {
            ReadOnlySpan<byte> _encryptedPkcs8 = Convert.FromBase64String(encryptedPkcs8PrivateKey.ToString());
            return Decrypt(cipherText, password, _encryptedPkcs8, padding);
        }

        public static string Encrypt(ReadOnlySpan<char> plainText, ReadOnlySpan<char> password, 
            ReadOnlySpan<byte> encryptedPkcs8PrivateKey, RSAEncryptionPadding padding = null!)
        {
            try
            {
                using (var rsa = CreateCsp())
                {
                    rsa.ImportEncryptedPkcs8PrivateKey(password, encryptedPkcs8PrivateKey, out int _);
                    return Encrypt(rsa, plainText, padding);
                }
            }
            catch { return string.Empty; }
        }

        public static string Decrypt(ReadOnlySpan<char> cipherText, ReadOnlySpan<char> password, 
            ReadOnlySpan<byte> encryptedPkcs8PrivateKey, RSAEncryptionPadding padding = null!)
        {
            try
            {
                using (var rsa = CreateCsp())
                {
                    rsa.ImportEncryptedPkcs8PrivateKey(password, encryptedPkcs8PrivateKey, out int _);
                    return Decrypt(rsa, cipherText, padding);
                }
            }
            catch { return string.Empty; }
        }

        public static string Encrypt(ReadOnlySpan<char> plainText, ReadOnlySpan<char> publicKeyXml, RSAEncryptionPadding padding = null!)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(plainText.ToString())) { return string.Empty; }
                using (var rsaWrite = CreateCsp())
                {
                    rsaWrite.FromXmlString(publicKeyXml.ToString());
                    return Encrypt(rsaWrite, plainText, padding);
                }
            }
            catch { return string.Empty; }
        }

        public static string Decrypt(ReadOnlySpan<char> cipherText, ReadOnlySpan<char> privateKeyXml, RSAEncryptionPadding padding = null!)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(cipherText.ToString())) { return string.Empty; }
                using (var rsaRead = CreateCsp())
                {
                    rsaRead.FromXmlString(privateKeyXml.ToString());
                    return Decrypt(rsaRead, cipherText, padding);
                }
            }
            catch { return string.Empty; }
        }

        public static string Encrypt(ReadOnlySpan<char> plainText, ReadOnlySpan<byte> publicKey, RSAEncryptionPadding padding = null!)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(plainText.ToString())) { return string.Empty; }
                using (var rsaWrite = CreateCsp())
                {
                    rsaWrite.ImportRSAPublicKey(publicKey, out int _bytesRead);
                    return Encrypt(rsaWrite, plainText, padding);
                }
            }
            catch { return string.Empty; }
        }

        public static string Decrypt(ReadOnlySpan<char> cipherText, ReadOnlySpan<byte> privateKey, RSAEncryptionPadding padding = null!)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(cipherText.ToString())) { return string.Empty; }
                using (var rsaRead = CreateCsp())
                {
                    rsaRead.ImportRSAPrivateKey(privateKey, out int _bytesRead);
                    return Decrypt(rsaRead, cipherText, padding);
                }
            }
            catch { return string.Empty; }
        }

        public static string Encrypt(ReadOnlySpan<char> plainText, RSAParameters publicKeyParam, RSAEncryptionPadding padding = null!)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(plainText.ToString())) { return string.Empty; }
                using (var rsaWrite = CreateCsp())
                {
                    rsaWrite.ImportParameters(publicKeyParam);
                    return Encrypt(rsaWrite, plainText, padding);
                }
            }
            catch { return string.Empty; }
        }

        public static string Decrypt(ReadOnlySpan<char> cipherText, RSAParameters publicKeyParam, RSAEncryptionPadding padding = null!)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(cipherText.ToString())) { return string.Empty; }
                using (var rsaRead = CreateCsp())
                {
                    rsaRead.ImportParameters(publicKeyParam);
                    return Decrypt(rsaRead, cipherText, padding);
                }
            }
            catch { return string.Empty; }
        }

        public static string Encrypt(RSACryptoServiceProvider rsa, ReadOnlySpan<char> plainText, RSAEncryptionPadding padding = null!)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(plainText.ToString())) { return string.Empty; }
                byte[] plainBytes = Encoding.Unicode.GetBytes(plainText.ToString());
                byte[] cipherBytes = rsa.Encrypt(plainBytes, padding ?? RSAEncryptionPadding.Pkcs1);
                return Convert.ToBase64String(cipherBytes);
            }
            catch { return string.Empty; }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="rsa"></param>
        /// <param name="cipherText"></param>
        /// <param name="padding"></param>
        /// <returns></returns>
        public static string Decrypt(RSACryptoServiceProvider rsa, ReadOnlySpan<char> cipherText, RSAEncryptionPadding padding = null!)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(cipherText.ToString())) { return string.Empty; }
                byte[] cipherBytes = Convert.FromBase64String(cipherText.ToString());
                byte[] plainBytes = rsa.Decrypt(cipherBytes, padding?? RSAEncryptionPadding.Pkcs1);
                return Encoding.Unicode.GetString(plainBytes);
            }
            catch { return string.Empty; }
        }

        //Sign & verify data with specified hash algorithm in parameter object format keys
        public static string SignData(ReadOnlySpan<char> plainData, RSAParameters privateKeyParam, RSASignaturePadding padding = null!)
        {
            return SignData(plainData, privateKeyParam, HashAlgorithmName.SHA256, padding);
        }

        public static bool VerifyData(ReadOnlySpan<char> plainData, ReadOnlySpan<char> signedData,
            RSAParameters publicKeyParam, RSASignaturePadding padding = null!)
        {
            return VerifyData(plainData, signedData, publicKeyParam, HashAlgorithmName.SHA256, padding);
        }

        public static string SignData(ReadOnlySpan<char> plainData, ReadOnlySpan<char> privateKeyXml, RSASignaturePadding padding = null!)
        {
            return SignData(plainData, privateKeyXml, HashAlgorithmName.SHA256, padding);
        }

        public static bool VerifyData(ReadOnlySpan<char> plainData, ReadOnlySpan<char> signedData,
            ReadOnlySpan<char> publicKeyXml, RSASignaturePadding padding = null!)
        {
            return VerifyData(plainData, signedData, publicKeyXml, HashAlgorithmName.SHA256, padding);
        }

        public static string SignData(ReadOnlySpan<char> plainData, ReadOnlySpan<byte> privateKey, RSASignaturePadding padding = null!)
        {
            return SignData(plainData, privateKey, HashAlgorithmName.SHA256, padding);
        }

        public static bool VerifyData(ReadOnlySpan<char> plainData, ReadOnlySpan<char> signedData,
            ReadOnlySpan<byte> publicKey, RSASignaturePadding padding = null!)
        {
            return VerifyData(plainData, signedData, publicKey, HashAlgorithmName.SHA256, padding);
        }

       

        public static string SignData(ReadOnlySpan<char> plainData, RSAParameters privateKeyParam,
            HashAlgorithmName hashAlg, RSASignaturePadding padding = null!)
        {
            using (var rsaWrite = CreateCsp())
            {
                rsaWrite.ImportParameters(privateKeyParam);
                return SignData(rsaWrite, plainData, hashAlg, padding);
            }
        }

        public static bool VerifyData(ReadOnlySpan<char> plainData, ReadOnlySpan<char> signedData, 
            RSAParameters publicKeyParam, HashAlgorithmName hashAlg, RSASignaturePadding padding = null!)
        {
            using (var rsaRead = CreateCsp())
            {
                rsaRead.ImportParameters(publicKeyParam);
                return VerifyData(rsaRead, plainData, signedData, hashAlg, padding);
            }
        }

        public static string SignData(ReadOnlySpan<char> plainData, ReadOnlySpan<char> privateKeyXml, 
            HashAlgorithmName hashAlg, RSASignaturePadding padding = null!)
        {
            using (var rsaWrite = CreateCsp())
            {
                rsaWrite.FromXmlString(privateKeyXml.ToString());
                return SignData(rsaWrite, plainData, hashAlg, padding);
            }
        }

        public static bool VerifyData(ReadOnlySpan<char> plainData, ReadOnlySpan<char> signedData, 
            ReadOnlySpan<char> publicKeyXml, HashAlgorithmName hashAlg, RSASignaturePadding padding = null!)
        {
            using (var rsaRead = CreateCsp())
            {
                rsaRead.FromXmlString(publicKeyXml.ToString());
                return VerifyData(rsaRead, plainData, signedData, hashAlg, padding);
            }
        }

        public static string SignData(ReadOnlySpan<char> plainData, ReadOnlySpan<byte> privateKey,
            HashAlgorithmName hashAlg, RSASignaturePadding padding = null!)
        {
            using (var rsaWrite = CreateCsp())
            {
                rsaWrite.ImportRSAPrivateKey(privateKey, out int _);
                return SignData(rsaWrite, plainData, hashAlg, padding);
            }
        }

        public static bool VerifyData(ReadOnlySpan<char> plainData, ReadOnlySpan<char> signedData,
            ReadOnlySpan<byte> publicKey, HashAlgorithmName hashAlg, RSASignaturePadding padding = null!)
        {
            using (var rsaRead = CreateCsp())
            {
                rsaRead.ImportRSAPublicKey(publicKey, out int _);
                return VerifyData(rsaRead, plainData, signedData, hashAlg, padding);
            }
        }

        public static string SignData(RSA rsa, ReadOnlySpan<char> plainData, 
            HashAlgorithmName hashAlg, RSASignaturePadding padding = null!)
        {
            byte[] plainBytes = Encoding.Unicode.GetBytes(plainData.ToArray());
            byte[] signedBytes = rsa.SignData(plainBytes, hashAlg, padding?? RSASignaturePadding.Pkcs1);
            return Convert.ToBase64String(signedBytes);
        }

        public static bool VerifyData(RSA rsa, ReadOnlySpan<char> plainData, ReadOnlySpan<char> signedData,
            HashAlgorithmName hashAlg, RSASignaturePadding padding = null!)
        {
            byte[] plainBytes = Encoding.Unicode.GetBytes(plainData.ToArray());
            byte[] signature = Convert.FromBase64String(signedData.ToString());
            return rsa.VerifyData(plainBytes, signature, hashAlg, padding ?? RSASignaturePadding.Pkcs1);
        }
    }
}
