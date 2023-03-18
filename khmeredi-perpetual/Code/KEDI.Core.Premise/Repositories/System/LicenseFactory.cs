
using KEDI.Core.Security.Cryptography;
using KEDI.Core.System.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
namespace KEDI.Core.Security
{
    public class LicenseFactory
    {
        static string EncryptionKey => "@Kernel0+1-2*3&4%5?[(-$-)]?6,7@8!9/CipherZ";
        static string AlphaNumericKey => "012ABCDEFGHIJKLMNOPQRSTUVWXYZ4365abcdefghijklmnopqrstuvwxyz987";
        public string DefaultDirectory => Path.Combine(AppDomain.CurrentDomain.BaseDirectory);
        public string DefaultPath => Path.Combine(DefaultDirectory, Guid.NewGuid().ToString() + DefaultExt);
        public string DefaultExt => ".lck";
        public string DefaultPattern => string.Format($"*{DefaultExt}");
        public string DefaultFilter => string.Format($"(*{DefaultExt})|*{DefaultExt}");
        private static string SystemPath {
            get
            {     
                string pathWithEnv = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\KERNEL";
                return pathWithEnv;
            } 
        }

        private static FileAttributes RemoveAttribute(FileAttributes attributes, FileAttributes attributesToRemove)
        {
            return attributes & ~attributesToRemove;
        }

        public string[] FileNames
        {
            get
            {
                if (!Directory.Exists(DefaultDirectory))
                {
                    Directory.CreateDirectory(DefaultDirectory);
                }

                string[] files = Directory.GetFiles(DefaultDirectory, DefaultPattern);
                if (files.Length > 0)
                {
                    return files;
                }
                return new string[] { DefaultPath };
            }
        }
        public DateTimeOffset GetNtpUtc()
        {
            try
            {
                var tcp = new TcpClient("time.nist.gov", 13);
                string resp;
                using (var rdr = new StreamReader(tcp.GetStream()))
                {
                    resp = rdr.ReadToEnd();
                }

                if(string.IsNullOrWhiteSpace(resp)){
                    resp = DateTimeOffset.UtcNow.ToString();
                }

                string utc = resp.Substring(7, 17);
                _ = DateTimeOffset.TryParseExact(utc, "yy-MM-dd HH:mm:ss",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal,
                    out DateTimeOffset _utc);
                return _utc;
            }
            catch
            {
                return DateTimeOffset.UtcNow;
            }

        }
        public SystemLicense Read(string filename = "")
        {
            try
            {
                if (string.IsNullOrWhiteSpace(filename))
                {
                    filename = FileNames[0];
                }

                using (Stream stream = File.OpenRead(filename))
                {
                    return Read(stream);
                }
            }
            catch
            {
                return new SystemLicense();
            }
            
        }

        public SystemLicense Read(Stream input)
        {
            try
            {              
                if(input.Length > 0)
                {
                    using (BinaryReader br = new BinaryReader(input))
                    {
                        var license = JsonSerializer.Deserialize<SystemLicense>(Decrypt(br.ReadString()));
                        return license;
                    }
                }
                return new SystemLicense();
            }
            catch
            {
                return new SystemLicense();
            }
        }

        public void Write<TLicense>(TLicense license, string fullpath) where TLicense : SystemLicense
        {
            if (!Directory.Exists(DefaultDirectory))
            {
                Directory.CreateDirectory(DefaultDirectory);
            }

            using (FileStream fs = new FileStream(fullpath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
            {
                license.TenantID = CreateGUID();   
                license.EntryKey = Join(GenerateKey(4, "0123456789"), 1, "-");                
                var _obj = JsonSerializer.Serialize(license);
                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    bw.Write(Encrypt(_obj));
                }
            }
        }

        private static string CreateGUID(bool dashed = false)
        {
            return dashed ? Guid.NewGuid().ToString() : Guid.NewGuid().ToString("N");
        }

        public string GetDeviceKey()
        {
            try
            {
                var id = File.ReadAllText(SystemPath + "\\system.log").ToString();
                return AesFactory.Decrypt(id);
            }
            catch
            {
                if (!Directory.Exists(SystemPath))
                {
                    Directory.CreateDirectory(SystemPath);
                }
                string guid = Guid.NewGuid().ToString();            
                File.WriteAllText(SystemPath + "\\system.log", AesFactory.Encrypt(guid));
                return guid;
            }           
        }

        public string FindUUID()
        {
            try
            {
                Process process = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.FileName = "CMD.exe";
                startInfo.Arguments = "%SystemDrive%\\windows\\system32\\wbem /c wmic csproduct get UUID";
                process.StartInfo = startInfo;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.Start();
                process.WaitForExit();
                string output = process.StandardOutput.ReadToEnd();
                output = Regex.Replace(output, "\\s+", "").Substring(4);
                return output;
            }
            catch
            {
                return string.Empty;
            }
        }

        bool IsNullOrEmpty(object anyObject)
        {
            if(anyObject == null) { return true; }
            foreach (PropertyInfo pi in anyObject.GetType().GetProperties())
            {             
                if (pi.PropertyType == typeof(string))
                {
                    string value = (string)pi.GetValue(anyObject);
                    if (string.IsNullOrEmpty(value))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public string TryCreateProductKey(out string raw)
        {
            raw = GenerateKey(25);
            return Join(raw, 5, "-").ToUpper();
        }

        public string TryCreateImportKey(out string raw)
        {
            raw = GenerateKey(25);
            return Join(raw, 5, "-");
        }

        public void Flush()
        {
            foreach (string path in FileNames)
            {
                File.Delete(path);
            }
        }

        //Static section
        public static LicenseFactory Create()
        {
            return new LicenseFactory();
        }

        public static string Join(string[] words, string separator)
        {
            string text = "";
            for (int i = 0; i < words.Length; i++)
            {
                text += words[i];
                if (i < words.Length - 1)
                {
                    text += separator;
                }
            }
            return text;
        }

        public static string Join(string value, int size, string separator)
        {
            string result = "";
            if (!string.IsNullOrWhiteSpace(value))
            {
                int length = value.Length / size;
                int remainder = value.Length % size;
                for (int i = 0; i < length; i++)
                {
                    result += value.Substring(i * size, size);
                    if (i < length - 1)
                    {
                        result += separator;
                    }
                }

                if (remainder != 0)
                {
                    result += separator + value.Substring(length * size, remainder);
                }
            }

            return result;
        }

        public static IEnumerable<string> Separate(string value, int chunkSize)
        {
            List<string> values = new List<string>();
            int length = value.Length / chunkSize;
            int remainder = value.Length % chunkSize;
            for (int i = 0; i < length; i++)
            {
                values.Add(value.Substring(i * chunkSize, chunkSize));
            }

            if (remainder != 0)
            {
                values.Add(value.Substring(length * chunkSize, remainder));
            }
            return values;
        }

        public static string GenerateKey(int length, string alphaNumerics = "")
        {
            string solution = string.Format($"{AlphaNumericKey}{DateTime.UtcNow.ToBinary()}");
            if (!string.IsNullOrWhiteSpace(alphaNumerics))
            {
                solution = alphaNumerics;
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

        public static string[] Randomize(string[] words)
        {
            Random rnd = new Random();
            for(int i = 0; i < words.Length; i++)
            {
                words = words.OrderBy(w => rnd.Next(i)).ToArray();
            }
            
            return words;
        }

        public static string Encrypt(string clearText)
        {
            if (!string.IsNullOrEmpty(clearText))
            {
                try
                {
                    byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
                    using (RijndaelManaged encryptor = new RijndaelManaged())
                    {
                        using (Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 }))
                        {
                            encryptor.Key = pdb.GetBytes(32);
                            encryptor.IV = pdb.GetBytes(16);
                            using (MemoryStream ms = new MemoryStream())
                            {
                                using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                                {
                                    cs.Write(clearBytes, 0, clearBytes.Length);
                                }

                                clearText = Convert.ToBase64String(ms.ToArray());
                            }
                        }

                    }
                } catch(Exception e)
                {
                    Debug.Write(e);
                }
                
            }

            return clearText;
        }

        public static string Decrypt(string cipherText)
        {
            if (!string.IsNullOrEmpty(cipherText))
            {
                try
                {
                    byte[] cipherBytes = Convert.FromBase64String(cipherText);
                    using (RijndaelManaged encryptor = new RijndaelManaged())
                    {
                        using (Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 }))
                        {
                            encryptor.Key = pdb.GetBytes(32);
                            encryptor.IV = pdb.GetBytes(16);
                            using (MemoryStream ms = new MemoryStream())
                            {
                                using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                                {
                                    cs.Write(cipherBytes, 0, cipherBytes.Length);
                                }
                                cipherText = Encoding.Unicode.GetString(ms.ToArray());
                            }
                        }

                    }
                } catch(Exception e)
                {
                    Debug.Write(e);
                }
            }
            return cipherText;
        }
    }
}
