using System;
using System.IO;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace DesktopSisters
{
    public class Configuration
    {
        private string _path;

        // ReSharper disable once UnusedMember.Local
        private Configuration(){}

        private Configuration(string path)
        {
            _path = path;
        }

        public string Coordinates { get; set; } = "";

        public bool UseNewArtStyle { get; set; } = true;

        #region Load & Save
        public static Configuration LoadConfig(string path)
        {
            if (!File.Exists(path))
            {
                
                return CreateDefaultConfig(path);
            }
            else
            {
                var config = JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(path));
                config._path = path;
                return config;
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