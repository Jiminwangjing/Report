using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Utilities
{
    public interface ISecuritySigner
    {
        byte[] SignHash(string inputData, byte[] privateKey);
        bool VerifyHash(string inputData, byte[] signature, byte[] publicKey);
        byte[] SignHash(string inputData, string secretKey, byte[] privateKey);
        bool VerifyHash(string inputData, string secretKey, byte[] signature, byte[] publicKey);
        byte[] SignHash(byte[] inputHash, byte[] privateKey);
        bool VerifyHash(byte[] inputHash, byte[] signature, byte[] publicKey);
        byte[] SignData(string inputData, byte[] privateKey);
        bool VerifyData(string inputData, byte[] signature, byte[] publicKey);
        byte[] SignData(byte[] inputData, byte[] privateKey);
        bool VerifyData(byte[] inputData, byte[] signature, byte[] publicKey);
        byte[] ComputHash(byte[] inputData, byte[] secretKey);
        byte[] ComputHash(string inputData, string secretKey);
    }

    public class SecuritySigner : ISecuritySigner
    {
        public byte[] SignHash(string inputData, byte[] privateKey)
        {
            return SignHash(inputData, string.Empty, privateKey);
        }

        public bool VerifyHash(string inputData, byte[] signature, byte[] publicKey)
        {
            return VerifyHash(inputData, string.Empty, signature, publicKey);
        }

        public byte[] SignHash(string inputData, string secretKey, byte[] privateKey)
        {
            byte[] hashBytes = ComputHash(inputData, secretKey);
            return SignHash(hashBytes, privateKey);
        }

        public bool VerifyHash(string inputData, string secretKey, byte[] signature, byte[] publicKey)
        {
            byte[] hashBytes = ComputHash(inputData, secretKey);
            return VerifyHash(hashBytes, signature, publicKey);
        }

        public byte[] SignHash(byte[] inputHash, byte[] privateKey)
        {
            using var rsa = RSA.Create();
            rsa.ImportRSAPrivateKey(privateKey, out int _);
            byte[] signedBytes = rsa.SignHash(inputHash, HashAlgorithmName.SHA512, RSASignaturePadding.Pkcs1);
            return signedBytes;
        }

        public bool VerifyHash(byte[] inputHash, byte[] signature, byte[] publicKey)
        {
            using var rsa = RSA.Create();
            rsa.ImportRSAPublicKey(publicKey, out int _);
            bool verified = rsa.VerifyHash(inputHash, signature, HashAlgorithmName.SHA512, RSASignaturePadding.Pkcs1);
            return verified;
        }

        public byte[] SignData(string inputData, byte[] privateKey)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(inputData);
            return SignData(inputBytes, privateKey);
        }

        public bool VerifyData(string inputData, byte[] signature, byte[] publicKey)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(inputData);
            return VerifyData(inputBytes, signature, publicKey);
        }

        public byte[] SignData(byte[] inputData, byte[] privateKey)
        {
            using var rsa = RSA.Create();
            rsa.ImportRSAPrivateKey(privateKey, out int _);
            byte[] signedBytes = rsa.SignData(inputData, HashAlgorithmName.SHA512, RSASignaturePadding.Pkcs1);
            return signedBytes;
        }

        public bool VerifyData(byte[] inputData, byte[] signature, byte[] publicKey)
        {
            using var rsa = RSA.Create();
            rsa.ImportRSAPublicKey(publicKey, out int _);
            bool verified = rsa.VerifyData(inputData, signature, HashAlgorithmName.SHA512, RSASignaturePadding.Pkcs1);
            return verified;
        }

        public byte[] ComputHash(string inputData, string secretKey)
        {
            byte[] secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);
            byte[] inputBytes = Encoding.UTF8.GetBytes(inputData);
            return ComputHash(inputBytes, secretKeyBytes);
        }

        public byte[] ComputHash(byte[] inputData, byte[] secretKey)
        {
            using (var hmac = new HMACSHA512(secretKey))
            {
                return hmac.ComputeHash(inputData);
            }
        }
    }
}