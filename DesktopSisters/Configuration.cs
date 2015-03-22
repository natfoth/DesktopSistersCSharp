using System;
using System.IO;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace DesktopSisters
{
    public class Configuration
    {
        private readonly string _path;

        // ReSharper disable once UnusedMember.Local
        private Configuration(){}

        private Configuration(string path)
        {
            _path = path;
        }

        public string Coordinates { get; set; } = "";


#region Load & Save
        public static Configuration LoadConfig(string path)
        {
            if (!File.Exists(path))
            {
                return CreateDefaultConfig(path);
            }
            else
            {
                return JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(path));
            }
        }

        private static Configuration CreateDefaultConfig(string path)
        {
            var config = new Configuration(path);

            config.Save();

            return config;
        }

        public void Save()
        {
            File.WriteAllText(_path, JsonConvert.SerializeObject(this));
        }
#endregion
    }
}