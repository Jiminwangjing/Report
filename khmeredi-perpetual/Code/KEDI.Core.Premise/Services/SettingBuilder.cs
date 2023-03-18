using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;

namespace Kernel.Package
{
    public class SettingBuilder
    {
        private static string BasePath => AppDomain.CurrentDomain.BaseDirectory;
        private static string ContentBasePath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../");
        private static JToken Settings { get; set; }
        public SettingBuilder() { }
        public SettingBuilder(string fileName)
        {
            UseFileAsync(fileName);
        }

        public static Task UseFileAsync(string fileName)
        {
            string filePath = Path.Combine(ContentBasePath, $"{fileName}");
            if (File.Exists(filePath))
            {
                Settings = JToken.Parse(File.ReadAllText(filePath));
            }
            return Task.CompletedTask;
        }

        public static SettingBuilder UseFileJson(string fileName)
        {
            if (fileName.EndsWith(".json"))
            {
                return new SettingBuilder(fileName);
            }
            return new SettingBuilder($"{fileName}.json");
        }

        public static SettingBuilder UseFile(string fileName)
        {
            return new SettingBuilder(fileName);
        }

        public object GetValue(string key)
        {
            return Settings.SelectToken(key)?? JValue.CreateNull();
        }

        public string this[string key] => GetValue(key).ToString();

        public string CombineUrls(params string[] urls)
        {            
            string path = "";
            foreach (string p in urls) { path += p; }
            path = Regex.Replace(path, "(?<!(http:|https:))//", "/");
            return path;
        }

        public string CombineUrlsByKey(params string[] keys)
        {
            string path = "";
            foreach (string p in keys) { path += this[p]; }
            path = Regex.Replace(path, "(?<!(http:|https:))//", "/");
            return path;
        }
    }
}
